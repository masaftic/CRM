using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Messaging;

namespace Shared.Infrastructure.Data.Outbox;

public class OutboxProcessor<TDbContext>(
    IServiceScopeFactory serviceScopeFactory,
    JsonSerializerOptions jsonSerializerOptions,
    ILogger<OutboxProcessor<TDbContext>> logger) : BackgroundService
    where TDbContext : DbContext, IOutboxDbContext
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessBatchAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task ProcessBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

        var messages = await dbContext.OutboxMessages
            .Where(m => !m.ProcessedOn.HasValue && m.RetryCount < 3)
            .OrderBy(m => m.OccurredOn)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var messageBody = JsonSerializer.Deserialize(message.JsonData, message.Type, jsonSerializerOptions)
                    ?? throw new InvalidOperationException($"Failed to deserialize outbox message {message.Id} as {message.Type.FullName}.");

                await messageBus.PublishAsync(messageBody, message.Type, cancellationToken);

                message.ProcessedOn = DateTimeOffset.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;

                logger.LogError(ex, "Error processing outbox message {OutboxMessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
