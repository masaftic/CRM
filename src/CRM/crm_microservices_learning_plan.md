# CRM Microservices Learning Plan

## Starting point
- Solid in .NET and monoliths.
- New to microservices concepts.
- Goal: learn microservices by building a CRM, not by doing toy CRUD splits.
- Preferred approach: start from a **modular monolith**, then extract services only when the boundaries are clear.

## Core idea of the project
Build a CRM around real business flow:
- lead enters system
- sales qualifies lead
- opportunity/deal is created
- activities are tracked
- notifications/automations run
- analytics/search consume events
- customer account continues after close

## What to learn first
### 1) Microservices fundamentals
Learn these before splitting anything:
- service boundaries and bounded contexts
- sync vs async communication
- eventual consistency
- idempotency
- distributed logging/tracing
- API gateway basics
- versioned contracts
- data ownership per service

### 2) Practical distributed-systems patterns
Focus on the patterns you will actually use:
- outbox pattern
- message broker basics
- retries and dead-letter queues
- saga / orchestration vs choreography
- CQRS for read models
- event-driven integration
- schema evolution for events

### 3) Operational basics
Enough to run the system locally and understand failures:
- Docker Compose
- service-to-service auth
- correlation IDs
- structured logging
- health checks
- basic observability

## What to begin with
Start as a modular monolith in one repo.

### Initial modules inside the monolith
- Identity / Auth
- Contacts / Companies
- Leads
- Deals / Pipeline
- Activity Timeline
- Notifications
- Search projection later
- Analytics projection later

### Why this order
- Identity is cross-cutting.
- Leads and Deals are the core business flow.
- Activity gives you event thinking early.
- Notifications are a clean first async consumer.
- Search and Analytics are good later extractions because they are read-heavy projections.

## Build order
### Phase 1 — Modular monolith
Goal: model the CRM correctly before microservices.
- define domain entities and aggregates
- create use-case/application layer per module
- keep module boundaries strict
- use one database at first
- add domain events inside the monolith

Deliverable:
- login/auth
- create contact
- create lead
- qualify lead
- create deal
- move deal stage
- append activity timeline entries

### Phase 2 — Introduce async messaging
Goal: learn event-driven integration.
- add RabbitMQ or MassTransit
- publish integration events from domain changes
- consume events in separate handlers
- make handlers idempotent
- add outbox pattern

Deliverable:
- lead qualified event
- deal stage changed event
- notification sent from event
- analytics projection updated from event

### Phase 3 — Extract the first services
Extract only the easiest, most useful boundaries:
- Notification Service
- Analytics Service

Reason:
- they are naturally async
- they can tolerate eventual consistency
- they are read/side-effect oriented

### Phase 4 — Extract search and activity if needed
- Search Service for global lookup
- Activity Service for timeline/event history

### Phase 5 — Keep the core domain services last
Leave these together longest:
- Identity
- Contacts
- Leads
- Deals

These are the core transactional services and are hardest to split safely.

## Suggested service boundaries
### Identity Service
- users
- roles
- permissions
- tenants/workspaces

### Contacts Service
- people
- companies
- tags
- relationships
- notes

### Leads Service
- lead lifecycle
- qualification state
- assignment
- conversion

### Deals / Pipeline Service
- opportunities
- stages
- forecast
- revenue tracking

### Activity Service
- append-only timeline events
- calls
- emails
- meetings
- comments

### Notification Service
- email notifications
- reminders
- in-app notifications
- retry handling

### Search Service
- global search index
- autocomplete
- filters

### Analytics Service
- dashboards
- funnel metrics
- conversion stats
- revenue projections

## Recommended tech stack
Since you already know .NET:
- ASP.NET Core Minimal APIs or Controllers
- EF Core
- PostgreSQL
- RabbitMQ
- MassTransit
- Redis only when needed
- Docker Compose for local dev
- YARP if an API gateway is useful
- Serilog or similar structured logging

## Rules to avoid common mistakes
- Do not split into many services too early.
- Do not give every table its own service.
- Do not use microservices to avoid learning domain modeling.
- Do not start with Kubernetes.
- Do not introduce distributed transactions.
- Do not make “chatty” service calls for basic flows.
- Do not force event-driven design everywhere; use it where it fits.

## Milestone checklist
### MVP monolith
- [ ] auth
- [ ] contacts
- [ ] leads
- [ ] deals
- [ ] activities
- [ ] basic UI

### First distributed step
- [ ] message broker
- [ ] integration events
- [ ] notification consumer
- [ ] analytics consumer
- [ ] outbox pattern

### First extractions
- [ ] notification service
- [ ] analytics service

### Advanced learning
- [ ] search service
- [ ] saga/orchestration
- [ ] tracing and correlation IDs
- [ ] failure handling and retries
- [ ] service versioning

## Practical rule of thumb
If a capability mainly writes business truth, keep it close to the core.
If a capability mainly reacts to events, extract it earlier.

## Best first task
Build the CRM as a modular monolith with:
- Identity
- Contacts
- Leads
- Deals
- Activities

Then add RabbitMQ and extract Notifications and Analytics.

## Current continuation point
You are at the beginning stage.
Next step is to design the CRM domain model and define the first bounded contexts before writing service code.

