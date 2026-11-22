# Constitution

## Purpose and Scope:
- Purpose: A shared file between members belonged to a team that implement this project that provides the information about how to complete the features of this repository. All members must follow this constitution so the project's code can be consistent.
- Scope: the project's frontend, backend and infras.

## Principles:
- **P0. *Clean Architecture* is the philosophy.** So the project contains four main directories, each of them corresponding to a layer in *Clean Architecture*.
- **P1. Dependencies point inward.**  A layer may depend on any layer inside it, never on a layer outside it. Entities depend on nothing.
- **P2. Core business rules live in *Entity***, never in *Interface Adapters* and *Frameworks & Drivers*
- **P3. Use Cases defines the itnerfaces it needs; outer layers implement them.** Use Cases declared ports (such ash repository and gateway interfaces) and its input/output DTOs. Frameworks & Drivers implements theseports (EF Core, external clients). Interface Adapters converts between external formats (HTTP requests and resposnes) and the Use Cases DTOs. All data crossing a layer boundary must be a DTO, never an Entity.
- **P4. Never expose entities outside Application.** The API sends and receives data through DTOs only.
- **P5. Prefer simplicity.** Add a pattern only when it solves a real problem, and record why (for revision).

## Solution structure and allowed references
| Project | May reference | Must NOT reference |
| :--- | :--- | :--- |
| Entities | nothing | everything else |
| UseCases | Entities | Interface Adapters, Frameworks & Drivers |
| Interface Adapters | UseCases, Entities | Frameworks & Drivers |
| Frameworks & Drivers | Interface Adapters, Use Cases, Entities | nothing |

## Layer rules
**Entity**
- Entities, value objects, domain events, domain exceptions, enums, Result Pattern models
- No framework or NuGet dependencies, apart from trivial ones
- Entities protect their invariants (private setters, factory methods or constructors)

**Use Cases**
- Define Application's business logics using Services classes
- Define the repository/gateway interfaces and its input/output DTOs (the 'ports')

**Interface Adapters**
- Converts betrween the outside world and those ports (controllers, API request/resposne models, mappers)

**Frameworks & Drivers**
- Implement the ports (EF Core, external clients)

## Conventions
- All I/O is `async` and accepts a `CancellationToken`
- Use ASP.NET validation attribute first
- Errors are returned through Result pattern or domain exceptions, mapped to endpoints's responses.
- Logging uses ILogger<T> with structured templates, and never logs secrets or personal data
- Configuration is accessed through the Options pattern
- Naming: CreateOrderCommand, GetOrderByIdQuery, OrderDto, IOrderRepository

## How to add a feature (the recipe)
- Model or extend the entity in **Entities**
- Define the command or query, validators, handlers in **Use Cases**
- Add any needed interfaces, DTOs, repositories declarations, controllers in **Interface Adapters**
- Add a migration if the schema changed
- Expose an endpoint in Controllers inside **Interface Adapters**

## Testing rules
- **Entities** and **Use Cases** must have unit tests
- **Every endpoint has at least one integration test (`WebApplicationFactory`)
- Test must not depend on execution order or shared state.

## Enforcement
- **Architecture tests**: (NetArchTest or ArchUnitNET) that fail the build on violdations
- **PR checklist:** layer rules respected, no entities leaked, tests added, no secrets
- **CI gates:** build, tests, and analyzers must pass before merge
- **Analyzers and `.editorconfig`** for style.

## Exceptions and amendments
- Any deviation needs a short ADR (Architecture Decision Record) approved by the tech lead. Changes to this document go through a PR like code


### Fix
Moderate problems (ambiguity and inconsistency)
Services vs. CQRS. Use Cases says “command or query, validators, handlers using Services classes”. These are two different styles. Decide whether it’s MediatR handlers or service classes (IOrderService), and name the library if any.
Validation is contradictory. It “lives in application validators, not controllers”, yet you allow ASP.NET data annotations. State the split: for example, FluentValidation in Use Cases for all commands and queries, with data annotations not used.
Result pattern vs. domain exceptions. The Entity layer lists “domain exceptions” while conventions say errors use the Result pattern. Define when each applies (typically, exceptions for broken invariants and Results for expected failures like “not found”), and name the Result type and where it lives.
The Frameworks & Drivers list is muddled.
“Web: External api client” confuses things. In Clean Architecture, Web means the web framework itself (ASP.NET host, middleware, routing), while an external API client is a gateway implementation.
“External interfaces:” is empty.
The section says it defines “concrete classes or interfaces”, but interfaces belong in Use Cases.
Scope doesn’t match content. Scope says frontend, backend, and infras, but every rule is backend-only (and “UI: none”). Either narrow the scope or add frontend and infra sections.
P2 is imprecise. Entities hold enterprise-wide rules, while Use Cases hold application-specific rules. Your Layer rules say the latter, but P2 implies all business rules live in Entity.
The recipe has gaps. It lacks a validator step, DTO mapping, DI registration, tests, and the order of steps. It also says “Add a migration” without saying where migrations live.
Naming is only examples. “CreateOrderCommand, GetOrderByIdQuery...” isn’t a rule. State the patterns (<Verb><Noun>Command, Get<Noun>By<Criteria>Query, <Name>Handler, and so on) and which layer each lives in.
What’s missing
Exact project and folder names, since you mix “Entity”, “Entities”, “UseCases”, and “Use Cases”.
Composition root: where Program.cs and DI registration live, and a rule that each layer exposes its own AddXxx() extension.
Transaction boundary and Unit of Work: who commits, and where.
API conventions: ProblemDetails, versioning, pagination, auth.
Mapping rules: who maps entity to DTO to API model, and with what tool.
Security basics: secrets handling, authorization location.
A glossary of Entity, DTO, Port, and Gateway as you use them.
Must/should wording and a one-line why per rule.
The actual PR checklist (it’s referenced but not written), a reference feature in the repo, and a version/date for the document.
Test details: how to test Use Cases (mock the ports), and Testcontainers for integration tests.
Typos and formatting
P1 ends with > instead of .
Unbalanced ** in P4 and in “**Every endpoint has at least one integration test”
“interacts with eachother” → “interact with each other”; “belonged to” → “belonging to”; “business logics” → “business logic”; “seperate” → “separate”; “atrribute” → “attribute”; “anay” → “any”; “violdations” → “violations”; “declarations” is fine, but “Define what is needed” reads better as “Declare what is needed”
Suggested corrections for the core sections

Dependency table

Layer	May reference	Must NOT reference
Entities	nothing	all other layers
Use Cases	Entities	Interface Adapters, Frameworks & Drivers
Interface Adapters	Use Cases, Entities	Frameworks & Drivers
Frameworks & Drivers	all inner layers	(none; hosts the composition root)

Where things live

Concept	Layer
Entities, value objects, domain exceptions	Entities
Commands/queries, handlers, validators, repository interfaces, input/output DTOs, Result type	Use Cases
Controllers, API request/response models, mappers	Interface Adapters
EF Core, migrations, repository implementations, external API clients, Program.cs, middleware, DI wiring	Frameworks & Drivers

P3 rewritten: “Use Cases defines the interfaces (repositories, gateways) it needs. Frameworks & Drivers implements them. Outer layers talk to Use Cases only through its input/output DTOs.”

One last note: four separate projects mapped to Uncle Bob’s circles is stricter than what most .NET teams do (Domain, Application, Infrastructure, Api). It’s valid, but make sure your team understands the mapping, and consider adding a short table showing both vocabularies.

I can produce a corrected full version of your constitution, either here or as an editable doc you can share or download as Word or PDF, if you’d like.