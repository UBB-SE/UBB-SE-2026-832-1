# Architecture Rules:

- Keep business logic in services.
- Keep model definitions in `ClassLibrary/Models`; do not redefine them in `WinUI`.
- Keep repository interfaces in `ClassLibrary/IRepositories`.
- Keep repository implementations in `ClassLibrary/Repositories`.
- Keep EF context definitions in `ClassLibrary/Data`.
- Model object links via class references, not foreign-key id fields.
- `WebAPI` should call extension methods for data-layer wiring, not reference data-layer internals directly.
- Keep registration ownership explicit: `WebAPI` registers API services; `ClassLibrary` registers data access.
- `WinUI` communicates with backend through services/proxies, not direct database access.
- `WinUI` proxy services must receive all dependencies via constructor injection (e.g., `HttpClient`, config, auth handlers) and avoid instantiating dependencies inside the proxy.
- Request flow: `WinUI` (`ServiceProxy`) -> `WebAPI` (`Controller`) -> `WebAPI` (`IUserService`/ `Service`) -> `ClassLibrary` (`IRepository` + `Repository` + `AppDbContext`) -> `database`

# Formatting guidelines

## Naming conventions

### Code

1. Classes:
  - every class must use `PascalCase` naming convention
2. Interfaces:
  - every interface must use `IPascalCase` naming convention
3. Structs:
  - every struct must use `camelCase` naming convention
4. Variables:
  - public member variables must use `camelCase` naming convention
  - private member variables must use `camelCase` naming convention
  - local variables, parameters must use `camelCase` naming convention
  - avoid abbreviations
  - accepted short forms are only standard technical identifiers such as `id`, `url`, `api`, `dto`, `db` ...
5. Constants
  - whenever possible to use `const` keyword before a variable, it must be used
  - constants must use `UPPER_CASE_WITH_UNDERSCORES` naming convention

### Files

- filenames and directory names must use `PascalCase`
- filename must be the same as the main class/struct/interface in that file
- in each file there is a single class/struct/interface

## Organization

- `using` directives for namespace use always must be declared at the top of the file
- modifiers occur in the following order: `public protected internal private new abstract virtual override sealed static readonly extern unsafe volatile async`
- use explicit constructors over primary constructors
- Class member ordering:
  - Group class members in the following order: 
    - Nested classes, enums, delegates and events.
    - Static, const and readonly fields.
    - Fields and properties.
    - Constructors
    - Methods
  - Within each group, elements should be in the following order: 
    - `private`
    - `protected`
    - `public`

## Whitespace rules

- a maximum of one statement per line
- a maximum of one assignment per statement
- braces should not be omitted
- code should not contain multiple blank lines in a row
