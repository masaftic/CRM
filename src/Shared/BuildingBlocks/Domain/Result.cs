namespace BuildingBlocks.Domain;

public interface IAppResult
{
    bool IsError { get; }
    List<AppError> Errors { get; }
    AppError FirstError { get; }
}

public interface IAppResult<T> : IAppResult
{
    T Value { get; }
}


/// <summary>Result for operations that return a value.</summary>
public sealed class Result<T> : IAppResult<T>
{
    private readonly T? _value;
    private readonly List<AppError>? _errors;

    private Result(T value)               => _value = value;
    private Result(List<AppError> errors) => _errors = errors;

    public bool IsError          => _errors is { Count: > 0 };
    public T Value               => IsError ? throw new InvalidOperationException("Result is an error.") : _value!;
    public List<AppError> Errors => _errors ?? [];
    public AppError FirstError   => _errors?[0] ?? throw new InvalidOperationException("No errors.");

    public static Result<T> Ok(T value)                 => new(value);
    public static Result<T> Fail(AppError error)        => new([error]);
    public static Result<T> Fail(List<AppError> errors) => new(errors);

    public static implicit operator Result<T>(T value)                => Ok(value);
    public static implicit operator Result<T>(AppError error)         => Fail(error);
    public static implicit operator Result<T>(List<AppError> errors)  => Fail(errors);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<List<AppError>, TResult> onError)
        => IsError ? onError(Errors) : onSuccess(Value);

    public Result<TNext> Bind<TNext>(Func<T, Result<TNext>> binder)
        => IsError ? Result<TNext>.Fail(Errors) : binder(Value);

    public Result Bind(Func<T, Result> binder)
        => IsError ? Result.Fail(Errors) : binder(Value);
}

/// <summary>Result for void operations (no return value).</summary>
public sealed class Result : IAppResult
{
    private readonly List<AppError>? _errors;

    private Result() { }
    private Result(List<AppError> errors) => _errors = errors;

    public bool IsError          => _errors is { Count: > 0 };
    public List<AppError> Errors => _errors ?? [];
    public AppError FirstError   => _errors?[0] ?? throw new InvalidOperationException("No errors.");

    public static Result Ok()                              => new();
    public static Result Fail(AppError error)              => new([error]);
    public static Result Fail(List<AppError> errors)       => new(errors);

    public static implicit operator Result(AppError error)         => Fail(error);
    public static implicit operator Result(List<AppError> errors)  => Fail(errors);

    public TResult Match<TResult>(Func<TResult> onSuccess, Func<List<AppError>, TResult> onError)
        => IsError ? onError(Errors) : onSuccess();

    public Result<TNext> Bind<TNext>(Func<Result<TNext>> binder)
        => IsError ? Result<TNext>.Fail(Errors) : binder();

    public Result Bind(Func<Result> binder)
        => IsError ? this : binder();
}
