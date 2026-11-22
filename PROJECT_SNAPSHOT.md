# FlashCardApp — Project Snapshot for Exact Regeneration

This file is a complete, byte-exact snapshot of the `flashcard-app` repository as of 2026-11-13. An AI agent (or a person) can use it to recreate every file of the project with identical content, encoding and line endings, and then verify the result with SHA-256 hashes.

This snapshot file itself (`PROJECT_SNAPSHOT.md`) is not part of the project and must not be recreated inside the output.

---

## 0. Instructions for the regenerating agent

1. **Goal:** recreate the 29 source files in §4 at their exact relative paths, byte for byte, plus the empty directory `src/FlashCardApp/Models/`. Then regenerate `swagger.json` as described in §6.
2. **Do not change anything.** Don't reformat, rename, fix bugs, fix typos, add or remove usings, update package versions, or "improve" anything. The oddities listed in §3 belong to the snapshot and must be reproduced as they are.
3. **Preferred method (automatic):** save this markdown file unchanged, copy the Python script from §5 into `extract_snapshot.py`, and run it. The script writes every file with the correct BOM, line endings and final newline, and checks each SHA-256.
4. **Fallback method (manual):** for each file in §4, write exactly the text between the opening fence line and the closing fence line. Then apply the attributes from the manifest in §2:
   - `EOL = CRLF` means every line ends with `\r\n`.
   - `BOM = yes` means the file starts with the bytes `EF BB BF`.
   - `Final newline = no` means the file ends right after the last character of the last line.

   Keep the trailing whitespace listed in §2.1. Finish by checking the hashes (§7).
5. **How file blocks are delimited:** each file in §4 has a heading, then a marker line `<!-- file: PATH | bom=… | eol=… | final_newline=… | sha256=… -->`, then a fenced block. The fence is three or more backticks. The closing fence has exactly the same number of backticks and no language tag. The language tag on the opening fence is not part of the file. `flashcard_set_crud_design.md` uses a four-backtick fence because it contains triple-backtick blocks itself.
6. **Do not create** `bin/`, `obj/` or `.vs/`; they are build output. Nothing else exists in the repository: there is no `.gitignore`, `.editorconfig`, `global.json`, `Directory.Build.props` or test project, and the folder is not a git repository.

---

## 1. Project overview (context only; the file contents in §4 are authoritative)

**What it is:** an ASP.NET Core Web API skeleton, built with controllers, that simulates Quizlet-style flash card sets. Every action is a placeholder. Actions return hard-coded sample data or `204 No Content`. There is no database, no persistence and no real business logic. The design for a future PostgreSQL data model is in `flashcard_set_crud_design.md`.

**Toolchain and dependencies**

| Item | Value |
|---|---|
| .NET SDK used | 10.0.404 (no `global.json`, so any 10.0.x SDK works) |
| Target framework | `net10.0`, `Nullable` enabled, `ImplicitUsings` enabled, root namespace `FlashCardApp` |
| Solution format | `FlashCardApp.slnx` (XML solution, needs SDK 9.0.200+) |
| Direct packages | `Microsoft.AspNetCore.OpenApi` 10.0.1, `Swashbuckle.AspNetCore` 10.0.1 |
| Resolved transitive packages | `Microsoft.OpenApi` 2.3.0, `Microsoft.Extensions.ApiDescription.Server` 10.0.0, `Swashbuckle.AspNetCore.Swagger`/`SwaggerGen`/`SwaggerUI` 10.0.1 |

**Directory layout**

```
flashcard-app/
├── FlashCardApp.slnx
├── flashcard_set_crud_design.md          design notes (PostgreSQL schema, delete/privacy policy)
├── swagger.json                          generated OpenAPI 3.0.4 document (see §6)
└── src/
    └── FlashCardApp/
        ├── FlashCardApp.csproj
        ├── Program.cs                    calls builder.AddFlashCardApp() / app.UseFlashCardApp()
        ├── FlashCardAppBuilder.cs        all service registration and middleware pipeline
        ├── appsettings.json
        ├── appsettings.Development.json
        ├── flashcard-app.http
        ├── Controllers/
        │   ├── FlashCardController.cs
        │   └── FlashCardSetController.cs
        ├── Dtos/                         12 request/response DTO classes
        ├── Enums/
        │   └── FlashCardSetVisibility.cs
        ├── ExceptionHandlers/
        │   └── GlobalExceptionHandler.cs
        ├── ModelConventions/
        │   └── RoutePrefixConvention.cs
        ├── Models/                       EMPTY directory (must exist, contains no files)
        ├── Properties/
        │   └── launchSettings.json
        ├── Swashbuckle/
        │   └── Filters/
        │       ├── AddFlashCardAppGlobalTagFilter.cs
        │       └── AddFlashCardAppTagFilter.cs
        └── Utils/
            └── AppConstants.cs
```

**Service registration (`AddFlashCardApp`)**
- `AddControllers` with `RoutePrefixConvention("api")`. This puts `api/` in front of every controller route.
- `InvalidModelStateResponseFactory` returns `400` with `DataResponse<object>.Fail("Validation failed.", errors)`, where `errors` is a `Dictionary<string, string[]?>`.
- `AddSwaggerGen` with document `v1` (title "FlashCardApp API") and two filters. `AddFlashCardAppTagFilter` adds the `FlashCardApp` tag to every operation. `AddFlashCardAppGlobalTagFilter` adds that tag to the document with the description "All FlashCardApp endpoints".
- `AddExceptionHandler<GlobalExceptionHandler>()` returns `500` as a `DataResponse` and shows the exception message only in Development. `AddProblemDetails()` is also registered.

**Middleware order (`UseFlashCardApp`)**
`UseExceptionHandler` → `UseStatusCodePages` → `UseSwagger` + `UseSwaggerUI` (Development only) → `UseHttpsRedirection` → `UseAuthentication` → `UseAuthorization` → `MapControllers`.

`UseStatusCodePages` writes a JSON `DataResponse` envelope: "Not Found" for 404, "Method Not Allowed" for 405, otherwise "Request failed with status code N.".

**Response envelope:** `{"data": <T or null>, "success": <bool>, "message": <string or null>}`.

**Endpoints.** Both controllers have `[ApiController]` and `[Authorize]`. The "Returns" column shows what each placeholder action returns. See §3, item 1, for what actually happens at runtime.

| Method | Path | Route name / operationId | Returns |
|---|---|---|---|
| GET | `/api/flash-card-sets` | `get_flash_card_sets` | 200, one sample `FlashCardSetResponseDto` |
| GET | `/api/flash-card-sets/{id}` | `get_flash_card_set_by_id` | 200, sample set with the given id |
| POST | `/api/flash-card-sets` | `create_flash_card_set` | 201 via `CreatedAtAction(nameof(GetFlashCardSetById))` |
| DELETE | `/api/flash-card-sets/{id}` | `delete_flash_card_set` | 204 |
| PUT | `/api/flash-card-sets/{id}` | `update_flash_card_set` | 204 |
| PUT | `/api/flash-card-sets/{id}/visibility` | `update_flash_card_visibility` | 204 |
| GET | `/api/flash-card-sets/{flash-card-set-id}/flash-cards` | `get_flash_cards` | 200, empty array |
| GET | `/api/flash-card-sets/{flash-card-set-id}/flash-cards/{id}` | `get_flash_card` | 200, empty `FlashCardResponseDto` |
| POST | `/api/flash-card-sets/{flash-card-set-id}/flash-cards` | `create_flash_card` | 201 via `CreatedAtRoute("get_flash_card", new { id = "new-id" })` |
| PATCH | `/api/flash-card-sets/{flash-card-set-id}/flash-cards` | `update_flash_card_position` | 204 (`id` binds from the query string) |
| PATCH | `/api/flash-card-sets/{flash-card-set-id}/flash-cards/{id}/flashcards` | `update_flash_card` | 204 |
| DELETE | `/api/flash-card-sets/{flash-card-set-id}/flash-cards/{id}/flashcards` | `delete_flash_card` | 204 |

**Run:** `dotnet run --project src/FlashCardApp --launch-profile http` listens on `http://localhost:5260`. The `https` profile also listens on `https://localhost:7107`. Swagger UI is at `/swagger` in Development.

---

## 2. File manifest

All text files are UTF-8. The C#, JSON, XML and HTTP files use CRLF line endings. The design document uses LF. Only `launchSettings.json` has a UTF-8 BOM. Several files have no final newline. Sizes are in bytes. The SHA-256 is computed over the exact file bytes.

| Path | Bytes | Encoding | BOM | EOL | Final newline | SHA-256 |
|---|---|---|---|---|---|---|
| `FlashCardApp.slnx` | 124 | UTF-8 | no | CRLF | yes | `277320f3367177b7aa50f534b8b22d5b7456d598116aea7db224d4e21b383221` |
| `src/FlashCardApp/FlashCardApp.csproj` | 458 | UTF-8 | no | CRLF | yes | `71be3210a9fdd11ba02f2b7f753f1ae1669eb9850ba2b03e2327799d85e1c2b6` |
| `src/FlashCardApp/Program.cs` | 209 | UTF-8 | no | CRLF | yes | `0e7ef01f27856fbe348a520765f58d08a747fc2b95dc020dbbd202798ee10f9a` |
| `src/FlashCardApp/FlashCardAppBuilder.cs` | 3922 | UTF-8 | no | CRLF | no | `56f44d4f049a1337c68f9b886b58d926ec78342a9d430a2fa95e1f8406caa5b5` |
| `src/FlashCardApp/Utils/AppConstants.cs` | 616 | UTF-8 | no | CRLF | no | `c3fa13cc7112c3ffef236215338a4244bfd6a3a392d198b1ddf96e97a9251dd8` |
| `src/FlashCardApp/ModelConventions/RoutePrefixConvention.cs` | 893 | UTF-8 | no | CRLF | no | `c57238c2e655d28844b613e5a80fab0a1172544eab1cab0edd6bb7cf715e921a` |
| `src/FlashCardApp/ExceptionHandlers/GlobalExceptionHandler.cs` | 1195 | UTF-8 | no | CRLF | no | `c5c971b218d85fe5ec7388dbbe039a534cc32bf384997e9bb7a66726dd659c14` |
| `src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppTagFilter.cs` | 562 | UTF-8 | no | CRLF | no | `9ce4759942b6df897d59ac62090fd3a0a2e1a9590159e240e1a99c6ab84fe61a` |
| `src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppGlobalTagFilter.cs` | 671 | UTF-8 | no | CRLF | no | `d235dbf3eb3dea732fa0a5066cbf47d9d76a672c5d8713f652f5f0e8610886df` |
| `src/FlashCardApp/Enums/FlashCardSetVisibility.cs` | 110 | UTF-8 | no | CRLF | no | `faeab7406863af06c954e08df23495a0d39a4e953427bf14f1fa8f2d7b69656c` |
| `src/FlashCardApp/Dtos/BaseResponse.cs` | 425 | UTF-8 | no | CRLF | yes | `3149b058c6f1f372a9ce20ce498ec260b953fab42b8b3db2eff996ba91421eb8` |
| `src/FlashCardApp/Dtos/DataResponse.cs` | 752 | UTF-8 | no | CRLF | no | `dc8f2e95356b8176fad28ee2870c9f08d0d3a303d3d331f2a0219fe3b19c85b2` |
| `src/FlashCardApp/Dtos/CreateFlashCardSetRequest.cs` | 1020 | UTF-8 | no | CRLF | yes | `827b0bf77f437485891c35138bd911a441d56aad162ca1756195b2b97234a122` |
| `src/FlashCardApp/Dtos/CreateFlashCardRequest.cs` | 844 | UTF-8 | no | CRLF | yes | `59c9fc26426d65b472bcd7e9144425b5c39b9a6354b3965a19ad00bdf49529fe` |
| `src/FlashCardApp/Dtos/CreatedFlashCardSetResponse.cs` | 1315 | UTF-8 | no | CRLF | yes | `cdf6b7c6375654c31ba24d9c2106c7ef545027aab6edb771f03ffeebc2cdc98a` |
| `src/FlashCardApp/Dtos/CreatedFlashCardResponse.cs` | 995 | UTF-8 | no | CRLF | no | `31c953a42d26f12bd0a8e71ff473ed6a915c9068bb49cc8a923bd77aafc55f15` |
| `src/FlashCardApp/Dtos/FlashCardSetResponseDto.cs` | 1307 | UTF-8 | no | CRLF | yes | `40fa465bd646e05069b236b5872b78e6ef5e84cf54bad4581727f4ffecc1044e` |
| `src/FlashCardApp/Dtos/FlashCardResponseDto.cs` | 1041 | UTF-8 | no | CRLF | no | `c3b51d4553646378dbffd349d3724165188834e256fc10f528e0abfb0bb815cf` |
| `src/FlashCardApp/Dtos/UpdateFlashCardSetRequest.cs` | 815 | UTF-8 | no | CRLF | yes | `e9093f032f20f4b28914ad81a931ef0d4bc0a6fb91febd67eb79f95c5d4b202d` |
| `src/FlashCardApp/Dtos/UpdateFlashCardRequest.cs` | 772 | UTF-8 | no | CRLF | no | `fc440d4da0e94d17127fd9312d4573a812d95cbc2a28899567b6079c12b33caf` |
| `src/FlashCardApp/Dtos/UpdateFlashCardPositionRequest.cs` | 443 | UTF-8 | no | CRLF | no | `750bcba49f0e49bb1c84a63f66dce5c62fbdb0919ab1c0f96d6493eef77f6e34` |
| `src/FlashCardApp/Dtos/UpdateFlashCardVisibilityRequest.cs` | 323 | UTF-8 | no | CRLF | no | `98a99b654ca60af6f1c3b3b126eef7dd570a43e473a91c8ef1a98153e67daba3` |
| `src/FlashCardApp/Controllers/FlashCardSetController.cs` | 7201 | UTF-8 | no | CRLF | yes | `16a54672a7a88a07f98c2cb4fe98bcfb980ad7cf9d158ff098475dd83ebaca25` |
| `src/FlashCardApp/Controllers/FlashCardController.cs` | 6134 | UTF-8 | no | CRLF | yes | `9e1524e1719ac37c5ecac0786eadc407b7c4e61e005c76c7f1c89f6f458ac065` |
| `src/FlashCardApp/appsettings.json` | 151 | UTF-8 | no | CRLF | yes | `200332b434757a0054a3d356c91cde01f2c48712be8aa2bcb0f3e973619f06e3` |
| `src/FlashCardApp/appsettings.Development.json` | 127 | UTF-8 | no | CRLF | yes | `73f95f9e0ceb205fc1c4dc50c07697fcfa29d7087868c2aef1d504cb38c771ec` |
| `src/FlashCardApp/Properties/launchSettings.json` | 644 | UTF-8 | yes | CRLF | yes | `6d28f8b68546fa63b9c1f9722b22d9e002957f30ebe6dc1e93cdb7f508538995` |
| `src/FlashCardApp/flashcard-app.http` | 139 | UTF-8 | no | CRLF | yes | `278fbac9c5b8b5d4ccf8d15f170bb8b17e37f5da4292bfd07b39694b2c2f0f0d` |
| `flashcard_set_crud_design.md` | 16029 | UTF-8 (non-ASCII) | no | LF | yes | `0bc023708b95ebb017ab2b3b3257d90ab5fa3c74d6400979aaec1c6de42288b0` |
| `swagger.json` *(generated, §6)* | 127264 | UTF-16 LE | yes (`FF FE`) | CRLF | yes | `0b98e76d190eb5fa4002852400f56bd122121af13bcee6e536f3b9f62494bfb0` |

### 2.1 Lines with trailing whitespace (must be preserved)

The fenced blocks in §4 contain this whitespace. If your editor or tooling strips trailing whitespace, add it back. The hashes will not match without it.

| File | Line | Trailing whitespace |
|---|---|---|
| `src/FlashCardApp/FlashCardAppBuilder.cs` | 32 | 16 spaces (line is whitespace only) |
| `src/FlashCardApp/FlashCardAppBuilder.cs` | 37 | 8 spaces (line is whitespace only) |
| `src/FlashCardApp/FlashCardAppBuilder.cs` | 43 | 1 space |
| `src/FlashCardApp/FlashCardAppBuilder.cs` | 44 | 1 space |
| `src/FlashCardApp/Utils/AppConstants.cs` | 23 | 1 space |
| `src/FlashCardApp/Dtos/UpdateFlashCardSetRequest.cs` | 17 | 4 spaces (line is whitespace only) |
| `src/FlashCardApp/Dtos/UpdateFlashCardPositionRequest.cs` | 3 | 4 spaces |
| `src/FlashCardApp/Controllers/FlashCardSetController.cs` | 15 | 4 spaces (line is whitespace only) |
| `src/FlashCardApp/Controllers/FlashCardSetController.cs` | 70 | 1 space |
| `src/FlashCardApp/Controllers/FlashCardSetController.cs` | 71 | 1 space |
| `src/FlashCardApp/Controllers/FlashCardSetController.cs` | 87 | 1 space |

---

## 3. Known quirks: reproduce them, do not fix them

These are part of the current state. A faithful regeneration keeps all of them.

1. **Every API endpoint returns HTTP 500 at runtime.** Both controllers have `[Authorize]`, and `UseAuthentication()` is called, but no authentication scheme is ever registered (there is no `AddAuthentication(...)`). Observed response for any `/api/...` call: `{"data":null,"success":false,"message":"No authenticationScheme was specified, and there was no DefaultChallengeScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions)."}`. Unknown paths correctly return `404 {"data":null,"success":false,"message":"Not Found"}`, wrong verbs return the `405` envelope, and Swagger UI works.
2. `OpenApiContact.Url` is `new Uri("https://support@flashcardapp.com")`, an email address written as a URL.
3. `FlashCardAppBuilder.cs` contains a stale comment `// builder.Services.AddTodoApi();`. The `AddControllers(...)` chain has unusual formatting: `})`, then a line of 8 spaces, then `; // Add support for controllers`.
4. Route templates `"{id}/flashcards"` on `DeleteFlashCard` and `UpdateFlashCard` produce paths ending in `/flash-cards/{id}/flashcards`.
5. `UpdateFlashCardPosition(string id, ...)` has no `{id}` in its route, so `id` is bound from the query string.
6. `CreateFlashCardSetRequest.Visibility` is a `string`. All other visibility properties use the `FlashCardSetVisibility` enum (`Public`, `Private`, `Link`), which is serialized as an integer because no `JsonStringEnumConverter` is configured.
7. JSON names are inconsistent: `UpdateFlashCardSetRequest.FlashCards` is `"flash_cards"`, while the other DTOs use `"flashcards"`. `FlashCardResponseDto.FlashCardSetId` maps to `"set_id"`.
8. `using` placement is inconsistent: some DTO files put `namespace` before the `using` directives. Some files also have patterns like `int Position { get; set; } = default!;`. `UpdateFlashCardVisibilityRequest` has no `[Description]` attribute.
9. `Microsoft.AspNetCore.OpenApi` is referenced but not used; Swashbuckle produces the document. `AddEndpointsApiExplorer()` is commented out.
10. `flashcard-app.http` still targets the template endpoint `/weatherforecast/`, which does not exist.
11. `flashcard_set_crud_design.md` has several issues that must be kept:
    - The SQL is missing a comma after `created_at TIMESTAMPTZ NOT NULL DEFAULT now()` in `app_user`.
    - Section numbering jumps from 4 to 6.
    - It mentions an `index_outbox` table that the schema doesn't define.
    - It uses LF line endings.
12. `src/FlashCardApp/Models/` is an empty directory.
13. `dotnet build` reports warning **NU1903** ("Package 'Microsoft.OpenApi' 2.3.0 has a known high severity vulnerability, GHSA-v5pm-xwqc-g5wc"). This is expected; do not bump the package.

---

## 4. File contents

### `FlashCardApp.slnx`

<!-- file: FlashCardApp.slnx | bom=no | eol=crlf | final_newline=yes | sha256=277320f3367177b7aa50f534b8b22d5b7456d598116aea7db224d4e21b383221 -->
```xml
<Solution>
  <Folder Name="/src/">
    <Project Path="src/FlashCardApp/FlashCardApp.csproj" />
  </Folder>
</Solution>
```

### `src/FlashCardApp/FlashCardApp.csproj`

<!-- file: src/FlashCardApp/FlashCardApp.csproj | bom=no | eol=crlf | final_newline=yes | sha256=71be3210a9fdd11ba02f2b7f753f1ae1669eb9850ba2b03e2327799d85e1c2b6 -->
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>FlashCardApp</RootNamespace>
    <TargetFramework>net10.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.1" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="10.0.1" />
  </ItemGroup>

</Project>
```

### `src/FlashCardApp/Program.cs`

<!-- file: src/FlashCardApp/Program.cs | bom=no | eol=crlf | final_newline=yes | sha256=0e7ef01f27856fbe348a520765f58d08a747fc2b95dc020dbbd202798ee10f9a -->
```csharp
using FlashCardApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddFlashCardApp();

var app = builder.Build();

app.UseFlashCardApp();

app.Run();
```

### `src/FlashCardApp/FlashCardAppBuilder.cs`

<!-- file: src/FlashCardApp/FlashCardAppBuilder.cs | bom=no | eol=crlf | final_newline=no | sha256=56f44d4f049a1337c68f9b886b58d926ec78342a9d430a2fa95e1f8406caa5b5 -->
```csharp
using FlashCardApp.Dtos;
using FlashCardApp.ExceptionHandlers;
using FlashCardApp.ModelConventions;
using FlashCardApp.Swashbuckle.Filters;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace FlashCardApp;

public static class FlashCardAppBuilder
{
    public static WebApplicationBuilder AddFlashCardApp(this WebApplicationBuilder builder)
    {
        // Config the Repository and associated services for the FlashCardApp
        // builder.Services.AddTodoApi();

        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RoutePrefixConvention(new RouteAttribute(AppConstants.Default_Api_Prefix.Value)));
        }).ConfigureApiBehaviorOptions(options =>
        {
            // 400 Handling for invalid model state
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                
                var response = DataResponse<object>.Fail("Validation failed.", errors);
                return new BadRequestObjectResult(response);
            };
        })
        
        ; // Add support for controllers
        // builder.Services.AddEndpointsApiExplorer(); // Add support for API endpoint exploration for minimal APIs (Omit here since using controllers only)
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { 
                Title = "FlashCardApp API", 
                Description = "An API for simulating Quizlet functionality.",
                Contact = new OpenApiContact
                {
                    Name = "ErrQD0303 (Dat Nguyen)",
                    Url = new Uri("https://support@flashcardapp.com")
                },
                Version = "v1"
            });

            options.OperationFilter<AddFlashCardAppTagFilter>();
            options.DocumentFilter<AddFlashCardAppGlobalTagFilter>();
        });

        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Add any custom configuration for the FlashCardApp here
        return builder;
    }

    public static WebApplication UseFlashCardApp(this WebApplication app)
    {
        // 500 Internal Server Error handling
        app.UseExceptionHandler(); // Use the global exception handler for unhandled exceptions

        // Other 4xx error handling except 400, 401, 403
        app.UseStatusCodePages(async context =>
        {
            var response = context.HttpContext.Response;
            if (response.HasStarted)
            {
                return;
            }

            response.ContentType = "application/json";
            var message = response.StatusCode switch
            {
                StatusCodes.Status404NotFound => "Not Found",
                StatusCodes.Status405MethodNotAllowed => "Method Not Allowed",
                _ => $"Request failed with status code {response.StatusCode}."
            };

            var dataResponse = DataResponse<object>.Fail(message);
            await response.WriteAsJsonAsync(dataResponse);
        });

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication(); // Use authentication middleware
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
```

### `src/FlashCardApp/Utils/AppConstants.cs`

<!-- file: src/FlashCardApp/Utils/AppConstants.cs | bom=no | eol=crlf | final_newline=no | sha256=c3fa13cc7112c3ffef236215338a4244bfd6a3a392d198b1ddf96e97a9251dd8 -->
```csharp
namespace FlashCardApp.Utils;

public static class AppConstants
{
    public static class Default_Api_Prefix
    {
        public const string Value = "api";
    }

    public static class ApiRoutes
    {
        public const string FlashCardSets = "flash-card-sets";
        public const string FlashCards = "flash-cards";
    }

    public static class Tags
    {
        public const string FlashCardApp = "FlashCardApp";
        public const string FlashCardSets = "FlashCardSets";
        public const string FlashCards = "FlashCards";
        public const string Users = "Users";
    }
} 
```

### `src/FlashCardApp/ModelConventions/RoutePrefixConvention.cs`

<!-- file: src/FlashCardApp/ModelConventions/RoutePrefixConvention.cs | bom=no | eol=crlf | final_newline=no | sha256=c57238c2e655d28844b613e5a80fab0a1172544eab1cab0edd6bb7cf715e921a -->
```csharp
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;

namespace FlashCardApp.ModelConventions;

public class RoutePrefixConvention : IApplicationModelConvention
{

    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(IRouteTemplateProvider route)
    {
        _routePrefix = new AttributeRouteModel(route);
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var selector in application.Controllers.SelectMany(c => c.Selectors))
        {
            if (selector.AttributeRouteModel != null)
            {
                selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(_routePrefix, selector.AttributeRouteModel);
                continue;
            }

            selector.AttributeRouteModel = _routePrefix;
        }
    }
}
```

### `src/FlashCardApp/ExceptionHandlers/GlobalExceptionHandler.cs`

<!-- file: src/FlashCardApp/ExceptionHandlers/GlobalExceptionHandler.cs | bom=no | eol=crlf | final_newline=no | sha256=c5c971b218d85fe5ec7388dbbe039a534cc32bf384997e9bb7a66726dd659c14 -->
```csharp
namespace FlashCardApp.ExceptionHandlers;

using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Diagnostics;

// Handles global exceptions and returns a standardized JSON response. (500 Internal Server Error)
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {exceptionMessage}", exception.Message);

        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        var message = _env.IsDevelopment() ? exception.Message : "An unexpected internal error occurred.";

        var response = DataResponse<object>.Fail(message);

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
```

### `src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppTagFilter.cs`

<!-- file: src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppTagFilter.cs | bom=no | eol=crlf | final_newline=no | sha256=9ce4759942b6df897d59ac62090fd3a0a2e1a9590159e240e1a99c6ab84fe61a -->
```csharp
using FlashCardApp.Utils;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlashCardApp.Swashbuckle.Filters;

public class AddFlashCardAppTagFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Tags ??= new HashSet<OpenApiTagReference>();

        if (!operation.Tags.Any(t => t.Name == AppConstants.Tags.FlashCardApp))
        {
            operation.Tags.Add(new OpenApiTagReference(AppConstants.Tags.FlashCardApp));
        }
    }
}
```

### `src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppGlobalTagFilter.cs`

<!-- file: src/FlashCardApp/Swashbuckle/Filters/AddFlashCardAppGlobalTagFilter.cs | bom=no | eol=crlf | final_newline=no | sha256=d235dbf3eb3dea732fa0a5066cbf47d9d76a672c5d8713f652f5f0e8610886df -->
```csharp
using FlashCardApp.Utils;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlashCardApp.Swashbuckle.Filters;

public class AddFlashCardAppGlobalTagFilter : IDocumentFilter
{

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags ??= new HashSet<OpenApiTag>();

        if (!swaggerDoc.Tags.Any(tag => tag.Name == AppConstants.Tags.FlashCardApp))
        {
            swaggerDoc.Tags.Add(new OpenApiTag
            {
                Name = AppConstants.Tags.FlashCardApp,
                Description = "All FlashCardApp endpoints"
            });
        }
    }
}
```

### `src/FlashCardApp/Enums/FlashCardSetVisibility.cs`

<!-- file: src/FlashCardApp/Enums/FlashCardSetVisibility.cs | bom=no | eol=crlf | final_newline=no | sha256=faeab7406863af06c954e08df23495a0d39a4e953427bf14f1fa8f2d7b69656c -->
```csharp
namespace FlashCardApp.Enums;

public enum FlashCardSetVisibility
{
    Public,
    Private,
    Link
}
```

### `src/FlashCardApp/Dtos/BaseResponse.cs`

<!-- file: src/FlashCardApp/Dtos/BaseResponse.cs | bom=no | eol=crlf | final_newline=yes | sha256=3149b058c6f1f372a9ce20ce498ec260b953fab42b8b3db2eff996ba91421eb8 -->
```csharp
namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;
public class BaseResponse
{
    [JsonPropertyName("success")]
    [Description("Indicates whether the operation was successful")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    [Description("The message associated with the operation result")]
    public string? Message { get; set; }
}
```

### `src/FlashCardApp/Dtos/DataResponse.cs`

<!-- file: src/FlashCardApp/Dtos/DataResponse.cs | bom=no | eol=crlf | final_newline=no | sha256=dc8f2e95356b8176fad28ee2870c9f08d0d3a303d3d331f2a0219fe3b19c85b2 -->
```csharp
namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;
public class DataResponse<T> : BaseResponse
{
    [JsonPropertyName("data")]
    [Description("The data associated with the response")]
    public T? Data { get; set; }

    public static DataResponse<T> Ok(T data, string? message = null)
    {
        return new DataResponse<T>
        {
            Data = data,
            Success = true,
            Message = message
        };
    }

    public static DataResponse<T> Fail(string errorMessage, T? data = default)
    {
        return new DataResponse<T>
        {
            Success = false,
            Message = errorMessage,
            Data = data
        };
    }
}
```

### `src/FlashCardApp/Dtos/CreateFlashCardSetRequest.cs`

<!-- file: src/FlashCardApp/Dtos/CreateFlashCardSetRequest.cs | bom=no | eol=crlf | final_newline=yes | sha256=827b0bf77f437485891c35138bd911a441d56aad162ca1756195b2b97234a122 -->
```csharp
namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
public class CreateFlashCardSetRequest
{
    [Required]
    [JsonPropertyName("title")]
    [Description("The title of the flash card set")]
    public string Title { get; set; } = default!;

    [Required]
    [JsonPropertyName("description")]
    [Description("The description of the flash card set")]
    public string Description { get; set; } = default!;

    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flash card set")]
    public string Visibility { get; set; } = default!;
    [JsonPropertyName("source_set_id")]
    [Description("The ID of the source flash card set")]
    public string SourceSetId { get; set; } = default!;

    [JsonPropertyName("flashcards")]
    [Description("The flash cards within the flash card set")]
    public CreateFlashCardRequest[] FlashCards { get; set; } = [];
}
```

### `src/FlashCardApp/Dtos/CreateFlashCardRequest.cs`

<!-- file: src/FlashCardApp/Dtos/CreateFlashCardRequest.cs | bom=no | eol=crlf | final_newline=yes | sha256=59c9fc26426d65b472bcd7e9144425b5c39b9a6354b3965a19ad00bdf49529fe -->
```csharp
namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CreateFlashCardRequest
{
    [Required]
    [JsonPropertyName("set_id")]
    [Description("The ID of the flash card set this flash card belongs to")]
    public string SetId { get; set; } = default!;

    [Required]
    [JsonPropertyName("term")]
    [Description("The term of the flash card")]
    public string Term { get; set; } = default!;

    [Required]
    [JsonPropertyName("definition")]
    [Description("The definition of the flash card")]
    public string Definition { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The position of the flash card within the set")]
    public int Position { get; set; } = default!;
}
```

### `src/FlashCardApp/Dtos/CreatedFlashCardSetResponse.cs`

<!-- file: src/FlashCardApp/Dtos/CreatedFlashCardSetResponse.cs | bom=no | eol=crlf | final_newline=yes | sha256=cdf6b7c6375654c31ba24d9c2106c7ef545027aab6edb771f03ffeebc2cdc98a -->
```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FlashCardApp.Enums;

namespace FlashCardApp.Dtos;

public class CreatedFlashCardSetResponse
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the flashcard set")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("owner_id")]
    [Description("The unique identifier of the owner of the flashcard set")]
    public string OwnerId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("title")]
    [Description("The title of the flashcard set")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("description")]
    [Description("The description of the flashcard set")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flashcard set")]
    public FlashCardSetVisibility Visibility { get; set; } = FlashCardSetVisibility.Public;

    [Required]
    [JsonPropertyName("flashcards")]
    [Description("The list of flashcards in the flashcard set")]
    public List<CreatedFlashCardResponse> FlashCards { get; set; } = [];
}
```

### `src/FlashCardApp/Dtos/CreatedFlashCardResponse.cs`

<!-- file: src/FlashCardApp/Dtos/CreatedFlashCardResponse.cs | bom=no | eol=crlf | final_newline=no | sha256=31c953a42d26f12bd0a8e71ff473ed6a915c9068bb49cc8a923bd77aafc55f15 -->
```csharp
namespace FlashCardApp.Dtos;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class CreatedFlashCardResponse
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The ID of the created flash card")]
    public string Id { get; set; } = default!;

    [Required]
    [JsonPropertyName("set_id")]
    [Description("The ID of the flash card set this flash card belongs to")]
    public string SetId { get; set; } = default!;

    [Required]
    [JsonPropertyName("term")]
    [Description("The term of the flash card")]
    public string Term { get; set; } = default!;

    [Required]
    [JsonPropertyName("definition")]
    [Description("The definition of the flash card")]
    public string Definition { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The position of the flash card within the set")]
    public int Position { get; set; } = default!;
}
```

### `src/FlashCardApp/Dtos/FlashCardSetResponseDto.cs`

<!-- file: src/FlashCardApp/Dtos/FlashCardSetResponseDto.cs | bom=no | eol=crlf | final_newline=yes | sha256=40fa465bd646e05069b236b5872b78e6ef5e84cf54bad4581727f4ffecc1044e -->
```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using FlashCardApp.Enums;

namespace FlashCardApp.Dtos;

public class FlashCardSetResponseDto
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the flashcard set")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("owner_id")]
    [Description("The unique identifier of the owner of the flashcard set")]
    public string OwnerId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("title")]
    [Description("The title of the flashcard set")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("description")]
    [Description("The description of the flashcard set")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("visibility")]
    [Description("The visibility of the flashcard set")]
    public FlashCardSetVisibility Visibility { get; set; } = FlashCardSetVisibility.Public;

    [Required]
    [JsonPropertyName("flashcards")]
    [Description("The list of flashcards in the flashcard set")]
    public List<FlashCardResponseDto> FlashCards { get; set; } = [];
}
```

### `src/FlashCardApp/Dtos/FlashCardResponseDto.cs`

<!-- file: src/FlashCardApp/Dtos/FlashCardResponseDto.cs | bom=no | eol=crlf | final_newline=no | sha256=c3b51d4553646378dbffd349d3724165188834e256fc10f528e0abfb0bb815cf -->
```csharp
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashCardApp.Dtos;

public class FlashCardResponseDto
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The unique identifier of the flashcard")]
    public string Id { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("set_id")]
    [Description("The unique identifier of the flashcard set this flashcard belongs to")]
    public string FlashCardSetId { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("term")]
    [Description("The term of the flashcard")]
    public string Term { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("definition")]
    [Description("The definition of the flashcard")]
    public string Definition { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("position")]
    [Description("The position of the flashcard within the set")]
    public int Position { get; set; } = 0;
}
```

### `src/FlashCardApp/Dtos/UpdateFlashCardSetRequest.cs`

<!-- file: src/FlashCardApp/Dtos/UpdateFlashCardSetRequest.cs | bom=no | eol=crlf | final_newline=yes | sha256=e9093f032f20f4b28914ad81a931ef0d4bc0a6fb91febd67eb79f95c5d4b202d -->
```csharp
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardSetRequest
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The ID of the flash card set")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("title")]
    [Description("The title of the flash card set")]
    public string Title { get; set; } = default!;
    
    [JsonPropertyName("description")]
    [Description("The description of the flash card set")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("flash_cards")]
    [Description("The flash cards within the flash card set")]
    public UpdateFlashCardRequest[] FlashCards { get; set; } = [];
}
```

### `src/FlashCardApp/Dtos/UpdateFlashCardRequest.cs`

<!-- file: src/FlashCardApp/Dtos/UpdateFlashCardRequest.cs | bom=no | eol=crlf | final_newline=no | sha256=fc440d4da0e94d17127fd9312d4573a812d95cbc2a28899567b6079c12b33caf -->
```csharp
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardRequest
{
    [Required]
    [JsonPropertyName("id")]
    [Description("The ID of the flash card")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("term")]
    [Description("The term of the flash card")]
    public string Term { get; set; } = default!;

    [JsonPropertyName("definition")]
    [Description("The definition of the flash card")]
    public string Definition { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The position of the flash card within the set")]
    public int Position { get; set; } = default!;
}
```

### `src/FlashCardApp/Dtos/UpdateFlashCardPositionRequest.cs`

<!-- file: src/FlashCardApp/Dtos/UpdateFlashCardPositionRequest.cs | bom=no | eol=crlf | final_newline=no | sha256=750bcba49f0e49bb1c84a63f66dce5c62fbdb0919ab1c0f96d6493eef77f6e34 -->
```csharp
namespace FlashCardApp.Dtos;
using System.Text.Json.Serialization;
using System.ComponentModel;    
public class UpdateFlashCardPositionRequest
{
    [JsonPropertyName("flash_card_id")]
    [Description("The ID of the flash card")]
    public string FlashCardId { get; set; } = default!;

    [JsonPropertyName("position")]
    [Description("The new position of the flash card")]
    public int Position { get; set; } = default!;
}
```

### `src/FlashCardApp/Dtos/UpdateFlashCardVisibilityRequest.cs`

<!-- file: src/FlashCardApp/Dtos/UpdateFlashCardVisibilityRequest.cs | bom=no | eol=crlf | final_newline=no | sha256=98a99b654ca60af6f1c3b3b126eef7dd570a43e473a91c8ef1a98153e67daba3 -->
```csharp
using FlashCardApp.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FlashCardApp.Dtos;

public class UpdateFlashCardVisibilityRequest
{
    [Required]
    [JsonPropertyName("visibility")]
    public FlashCardSetVisibility Visibility { get; set; } = default!;
}
```

### `src/FlashCardApp/Controllers/FlashCardSetController.cs`

<!-- file: src/FlashCardApp/Controllers/FlashCardSetController.cs | bom=no | eol=crlf | final_newline=yes | sha256=16a54672a7a88a07f98c2cb4fe98bcfb980ad7cf9d158ff098475dd83ebaca25 -->
```csharp
using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;
using FlashCardApp.Enums;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.FlashCardSets)]
[Tags(AppConstants.Tags.FlashCardSets)]
[Authorize]
public class FlashCardSetController : ControllerBase
{
    
    [HttpGet(Name = "get_flash_card_sets")] // Fetch all flash card sets by GET /api/flash-card-sets
    [EndpointSummary("Get all flash card sets")]
    [EndpointDescription("Get all of the current user's flash card sets")]
    [ProducesResponseType(typeof(DataResponse<FlashCardSetResponseDto[]>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult GetFlashCardSets()
    {
        // Placeholder for fetching flash card sets
        return Ok(DataResponse<FlashCardSetResponseDto[]>.Ok([
            new FlashCardSetResponseDto
            {
                Id = "SampleId",
                Title = "Sample FlashCardSet",
                Description = "Sample description for the flash card set",
                FlashCards = [],
                OwnerId = "SampleOwnerId",
                Visibility = FlashCardSetVisibility.Public
            }
        ]));
    }

    [HttpGet("{id}", Name = "get_flash_card_set_by_id")] // Fetch a flash card set by GET /api/flash-card-sets/{id}
    [EndpointSummary("Get a flash card set by ID")]
    [EndpointDescription("Get a specific flash card set for the current user by its ID")]
    [ProducesResponseType(typeof(DataResponse<FlashCardSetResponseDto>),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetFlashCardSetById(string id)
    {
        // Placeholder for fetching a flash card set by ID
        return Ok(DataResponse<FlashCardSetResponseDto>.Ok(new FlashCardSetResponseDto
        {
            Id = id,
            Title = "Sample FlashCardSet",
            Description = "Sample description for the flash card set",
            FlashCards = [],
            OwnerId = "SampleOwnerId",
            Visibility = FlashCardSetVisibility.Public
        }));
    }

    [HttpPost(Name = "create_flash_card_set")] // Create a new flash card set by POST /api/flash-card-sets
    [EndpointSummary("Create a new flash card set")]
    [EndpointDescription("Create a new flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<CreatedFlashCardSetResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    public IActionResult CreateFlashCardSet([FromBody] CreateFlashCardSetRequest flashCardSet)
    {  // 1. Save to database and generate the new ID
        var createdSet = new CreatedFlashCardSetResponse 
        { 
            Id = "SampleId", // Replace with actual generated ID
            Title = flashCardSet.Title,
            Description = flashCardSet.Description,
            FlashCards = [],
            OwnerId = "SampleOwnerId",
            Visibility = FlashCardSetVisibility.Public
        };

        var response = DataResponse<CreatedFlashCardSetResponse>.Ok(createdSet, "Flash card set created successfully");

        // 2. Pass routeValues (must match the parameter name in GetFlashCardSetById)
        return CreatedAtAction(
            actionName: nameof(GetFlashCardSetById),
            routeValues: new { id = createdSet.Id },
            value: response
        ); 
    }

    [HttpDelete("{id}", Name = "delete_flash_card_set")] // Delete a flash card set by DELETE /api/flash-card-sets/{id}
    [EndpointSummary("Delete a flash card set")]
    [EndpointDescription("Delete a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult DeleteFlashCardSet(string id)
    {
        // Placeholder for deleting a flash card set
        return NoContent();
    }

    [HttpPut("{id}", Name = "update_flash_card_set")] // Update a flash card set by PUT /api/flash-card-sets/{id}
    [EndpointSummary("Update a flash card set")]
    [EndpointDescription("Update a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCardSet(string id, [FromBody] UpdateFlashCardSetRequest flashCardSet)
    {
        // Placeholder for updating a flash card set
        return NoContent();
    }

    [HttpPut("{id}/visibility", Name = "update_flash_card_visibility")] // Update the visibility of a flash card set
    [EndpointSummary("Update the visibility of a flash card set")]
    [EndpointDescription("Update the visibility of a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCardVisibility(string id, [FromBody] UpdateFlashCardVisibilityRequest updateFlashCardVisibility)
    {
        // Placeholder for updating the visibility of a flash card set
        return NoContent();
    }
}
```

### `src/FlashCardApp/Controllers/FlashCardController.cs`

<!-- file: src/FlashCardApp/Controllers/FlashCardController.cs | bom=no | eol=crlf | final_newline=yes | sha256=9e1524e1719ac37c5ecac0786eadc407b7c4e61e005c76c7f1c89f6f458ac065 -->
```csharp
using FlashCardApp.Dtos;
using Microsoft.AspNetCore.Mvc;
using FlashCardApp.Utils;
using Microsoft.AspNetCore.Authorization;

namespace FlashCardApp.Controllers;

[ApiController]
[Route(AppConstants.ApiRoutes.FlashCardSets + "/{flash-card-set-id}/" + AppConstants.ApiRoutes.FlashCards)]
[Tags(AppConstants.Tags.FlashCards)]
[Authorize]
public class FlashCardController : ControllerBase
{
    [HttpGet(Name = "get_flash_cards")]
    [EndpointSummary("Get all flash cards within a flash card set")]
    [EndpointDescription("Retrieve all flash cards within a specific flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<FlashCardResponseDto[]>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetFlashCards()
    {
        // Placeholder for retrieving all flash cards within a flash card set
        return Ok(new DataResponse<FlashCardResponseDto[]>
        {
            Data = []
        });
    }

    [HttpGet("{id}", Name = "get_flash_card")]
    [EndpointSummary("Get a specific flash card within a flash card set")]
    [EndpointDescription("Retrieve a specific flash card within a flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<FlashCardResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult GetFlashCard(string id)
    {
        // Placeholder for retrieving a specific flash card within a flash card set
        return Ok(new DataResponse<FlashCardResponseDto>
        {
            Data = new FlashCardResponseDto()
        });
    }

    [HttpPost(Name = "create_flash_card")]
    [EndpointSummary("Create a new flash card within a flash card set")]
    [EndpointDescription("Create a new flash card within a specific flash card set for the current user")]
    [ProducesResponseType(typeof(DataResponse<CreatedFlashCardResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult CreateFlashCard([FromBody] CreateFlashCardRequest request)
    {
        // Placeholder for creating a new flash card within a flash card set
        return CreatedAtRoute("get_flash_card", new { id = "new-id" }, new DataResponse<CreatedFlashCardResponse>
        {
            Data = new CreatedFlashCardResponse()
        });
    }

    [HttpDelete("{id}/flashcards", Name = "delete_flash_card")]
    [EndpointSummary("Delete a flash card within a set")]
    [EndpointDescription("Delete a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult DeleteFlashCard(string id)
    {
        // Placeholder for deleting a flash card within a set
        return NoContent();
    }

    [HttpPatch("{id}/flashcards", Name = "update_flash_card")]
    [EndpointSummary("Update a flash card within a set")]
    [EndpointDescription("Update a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCard(string id, [FromBody] UpdateFlashCardRequest updateFlashCard)
    {
        // Placeholder for updating a flash card within a set
        return NoContent();
    }

    [HttpPatch(Name = "update_flash_card_position")]
    [EndpointSummary("Update the position of a flash card within a set")]
    [EndpointDescription("Update the position of a flash card within a set for the current user")]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(DataResponse<Dictionary<string, string[]?>?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(DataResponse<object>), StatusCodes.Status404NotFound)]
    public IActionResult UpdateFlashCardPosition(string id, [FromBody] UpdateFlashCardPositionRequest updateFlashCardPosition)
    {
        // Placeholder for updating the position of a flash card within a set
        return NoContent();
    }
}
```

### `src/FlashCardApp/appsettings.json`

<!-- file: src/FlashCardApp/appsettings.json | bom=no | eol=crlf | final_newline=yes | sha256=200332b434757a0054a3d356c91cde01f2c48712be8aa2bcb0f3e973619f06e3 -->
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### `src/FlashCardApp/appsettings.Development.json`

<!-- file: src/FlashCardApp/appsettings.Development.json | bom=no | eol=crlf | final_newline=yes | sha256=73f95f9e0ceb205fc1c4dc50c07697fcfa29d7087868c2aef1d504cb38c771ec -->
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### `src/FlashCardApp/Properties/launchSettings.json`

<!-- file: src/FlashCardApp/Properties/launchSettings.json | bom=yes | eol=crlf | final_newline=yes | sha256=6d28f8b68546fa63b9c1f9722b22d9e002957f30ebe6dc1e93cdb7f508538995 -->
```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5260",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7107;http://localhost:5260",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### `src/FlashCardApp/flashcard-app.http`

<!-- file: src/FlashCardApp/flashcard-app.http | bom=no | eol=crlf | final_newline=yes | sha256=278fbac9c5b8b5d4ccf8d15f170bb8b17e37f5da4292bfd07b39694b2c2f0f0d -->
```http
@flashcard_app_HostAddress = http://localhost:5260

GET {{flashcard_app_HostAddress}}/weatherforecast/
Accept: application/json

###
```

### `flashcard_set_crud_design.md`

<!-- file: flashcard_set_crud_design.md | bom=no | eol=lf | final_newline=yes | sha256=0bc023708b95ebb017ab2b3b3257d90ab5fa3c74d6400979aaec1c6de42288b0 -->
````markdown
# Flashcard Set CRUD: Design Notes (Quizlet-style)

Scope: create, read, update, and delete for flashcard sets, plus study sessions, hard-delete privacy rules, document indexing, and backups. Business analysis of Quizlet is out of scope.

> **Status of this document.** The domain model and schema below are a *design proposal*, not Quizlet's actual (non-public) schema. The Quizlet behavior described comes from help-center pages and tutorials and may have changed. The privacy section is engineering guidance, not legal advice.

---

## 1. How Quizlet handles set CRUD (observed behavior)

| Operation | Behavior |
|---|---|
| **Create** | From the Create button: title, description, term/definition pairs. Also spreadsheet import, AI generation from uploaded notes, and "Save and Edit" (copy of someone else's set). Free users can create sets. Diagram sets are premium. |
| **Read** | Set page and five study modes (Flashcards, Learn, Test, Match, Q-Chat). Search with filters. Default visibility lets anyone view; only the owner edits. Folders and tags organize sets; only teachers create classes. |
| **Update** | Edit title, description, terms, and order. Owner can allow others to edit (password-protected editing, or "Allow class to edit"). Others edit via a copy. |
| **Delete** | Owner deletes from the set menu and confirms. One low-quality source says deletion is not recoverable; confirm in-app. |

---

## 2. Domain model

### Relationships

```
User 1 ──── N FlashcardSet          (owner)
FlashcardSet 1 ──── N Flashcard

User 1 ──── N StudySession
FlashcardSet 1 ──── N StudySession  (which set was studied)

StudySession 1 ──── N CardReview ──── N:1 Flashcard
```

- A set has one or many cards (zero is possible while editing).
- Starting to study creates a new `StudySession` (one run through a set in one mode).
- A card does **not** belong to a session directly. `StudySession <-> Flashcard` is many-to-many through `CardReview`, which records the result of each card attempt. A card can be reviewed several times in one session (e.g. Learn mode).
- Sessions belong to a **user**; progress is meaningless without one.

### Entities and key fields

| Entity | Key fields |
|---|---|
| `app_user` | id, username, email |
| `flashcard_set` | id, owner_id, title, description, visibility, source_set_id (copy lineage), timestamps |
| `flashcard` | id, set_id, term, definition, position, timestamps |
| `study_session` | id, user_id, set_id, mode, started_at, ended_at |
| `card_review` | id, session_id, card_id, result, response_time_ms, reviewed_at |
| `user_card_progress` (optional) | user_id, card_id, mastery_level, correct_streak, next_review_at. Current state per user and card; `card_review` is the history |

---

## 3. Delete policy: decision history

1. **First proposal (soft delete):** `deleted_at` on sets and cards. Cards soft-deleted so `CardReview` history stays valid; sets soft-deleted so data could feed document indexing.
2. **Privacy review:** soft delete is not erasure. Users who delete a set generally expect it gone, so the design moved to **hard delete**.

### Final hard-delete rules

| You delete | What happens |
|---|---|
| **A user** | Their sets, cards, sessions, reviews, and progress are removed |
| **A set** | Its cards, **all** sessions on it (including other users'), and those sessions' reviews are removed |
| **A card** | Its `user_card_progress` rows are removed. Its `card_review` rows stay with `card_id = NULL` (result and timing only, no content) |

Notes:
- No table other than `flashcard` stores card text, so nothing readable survives a delete.
- To drop review history entirely when a card is deleted, change `ON DELETE SET NULL` to `ON DELETE CASCADE` on `card_review.card_id`.
- Trade-off: deleting a set removes other users' study history on it. This is the cleanest privacy outcome; the alternative is `ON DELETE SET NULL` on `study_session.set_id`.
- Copies made with "Save and Edit" are independent sets; deleting the original leaves them intact (`source_set_id` is set to NULL).

---

## 4. Full PostgreSQL schema

Tested on PostgreSQL 16. Verified behaviors: schema loads without errors; a review referencing a card from another set is rejected; card positions can be swapped inside one transaction (deferred unique constraint); deleting a card nulls its reviews' `card_id` and removes its progress; deleting a set leaves zero cards, sessions, and reviews; the outbox records delete events for cards and the set.

```sql
-- =============================================================================
-- Flashcard sets + study sessions  |  PostgreSQL 14+  |  HARD DELETE (privacy)
--
-- Delete behavior
--   Delete a user       -> their sets, cards, sessions, reviews, progress are removed
--   Delete a set        -> its cards, ALL sessions on it, and their reviews are removed
--   Delete a card       -> its progress rows are removed; its review rows are kept
--                          with card_id = NULL (result/time stats only, no content)
-- Nothing in the review/session tables stores card text, so no content survives.
-- =============================================================================

BEGIN;

-- ---------- Types ------------------------------------------------------------
CREATE TYPE set_visibility AS ENUM ('private', 'link', 'public');
CREATE TYPE study_mode     AS ENUM ('flashcards', 'learn', 'test', 'match');
CREATE TYPE review_result  AS ENUM ('correct', 'incorrect', 'skipped');

-- ---------- Shared trigger: keep updated_at fresh ----------------------------
CREATE FUNCTION set_updated_at() RETURNS trigger AS $$
BEGIN
  NEW.updated_at := now();
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ---------- app_user ---------------------------------------------------------
CREATE TABLE app_user (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  username   TEXT        NOT NULL CHECK (length(btrim(username)) > 0),
  email      TEXT        NOT NULL CHECK (position('@' IN email) > 1),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now()
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);
CREATE UNIQUE INDEX uq_app_user_username ON app_user (lower(username));
CREATE UNIQUE INDEX uq_app_user_email    ON app_user (lower(email));

-- ---------- flashcard_set ----------------------------------------------------
CREATE TABLE flashcard_set (
  id            BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  owner_id      BIGINT         NOT NULL REFERENCES app_user (id) ON DELETE CASCADE,
  title         TEXT           NOT NULL CHECK (length(btrim(title)) > 0),
  description   TEXT,
  visibility    set_visibility NOT NULL DEFAULT 'private',
  -- "Save and Edit" copies: keep lineage, but deleting the original must not
  -- delete the copy.
  source_set_id BIGINT REFERENCES flashcard_set (id) ON DELETE SET NULL,
  created_at    TIMESTAMPTZ    NOT NULL DEFAULT now(),
  updated_at    TIMESTAMPTZ    NOT NULL DEFAULT now()
);
CREATE INDEX ix_set_owner      ON flashcard_set (owner_id);
CREATE INDEX ix_set_source     ON flashcard_set (source_set_id) WHERE source_set_id IS NOT NULL;
CREATE INDEX ix_set_public     ON flashcard_set (created_at DESC) WHERE visibility = 'public';

CREATE TRIGGER trg_set_updated_at
  BEFORE UPDATE ON flashcard_set
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ---------- flashcard --------------------------------------------------------
CREATE TABLE flashcard (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  set_id     BIGINT      NOT NULL REFERENCES flashcard_set (id) ON DELETE CASCADE,
  term       TEXT        NOT NULL CHECK (length(btrim(term)) > 0),
  definition TEXT        NOT NULL CHECK (length(btrim(definition)) > 0),
  position   INT         NOT NULL CHECK (position >= 0),
  created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  -- Deferred so cards can be reordered/swapped inside one transaction.
  CONSTRAINT uq_card_position UNIQUE (set_id, position) DEFERRABLE INITIALLY DEFERRED
);
-- (set_id, position) unique constraint already indexes set_id lookups.

CREATE TRIGGER trg_card_updated_at
  BEFORE UPDATE ON flashcard
  FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ---------- study_session ----------------------------------------------------
-- One row each time a user starts studying a set in a given mode.
CREATE TABLE study_session (
  id         BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  user_id    BIGINT      NOT NULL REFERENCES app_user (id)      ON DELETE CASCADE,
  set_id     BIGINT      NOT NULL REFERENCES flashcard_set (id) ON DELETE CASCADE,
  mode       study_mode  NOT NULL,
  started_at TIMESTAMPTZ NOT NULL DEFAULT now(),
  ended_at   TIMESTAMPTZ,
  CHECK (ended_at IS NULL OR ended_at >= started_at)
);
CREATE INDEX ix_session_user_started ON study_session (user_id, started_at DESC);
CREATE INDEX ix_session_set          ON study_session (set_id);

-- ---------- card_review ------------------------------------------------------
-- Join entity: Flashcard <-> StudySession is many-to-many through this table.
-- A card may be reviewed several times in one session (e.g. Learn mode).
CREATE TABLE card_review (
  id               BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  session_id       BIGINT        NOT NULL REFERENCES study_session (id) ON DELETE CASCADE,
  card_id          BIGINT        REFERENCES flashcard (id) ON DELETE SET NULL,
  result           review_result NOT NULL,
  response_time_ms INT           CHECK (response_time_ms >= 0),
  reviewed_at      TIMESTAMPTZ   NOT NULL DEFAULT now()
);
CREATE INDEX ix_review_session ON card_review (session_id);
CREATE INDEX ix_review_card    ON card_review (card_id) WHERE card_id IS NOT NULL;

-- A reviewed card must belong to the set the session is studying.
CREATE FUNCTION check_review_card_in_session_set() RETURNS trigger AS $$
BEGIN
  IF NEW.card_id IS NOT NULL AND NOT EXISTS (
       SELECT 1
       FROM study_session s
       JOIN flashcard f ON f.set_id = s.set_id
       WHERE s.id = NEW.session_id AND f.id = NEW.card_id
     ) THEN
    RAISE EXCEPTION 'card % does not belong to the set of session %',
      NEW.card_id, NEW.session_id USING ERRCODE = 'foreign_key_violation';
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trg_review_card_in_set
  BEFORE INSERT OR UPDATE OF card_id, session_id ON card_review
  FOR EACH ROW EXECUTE FUNCTION check_review_card_in_session_set();

-- ---------- user_card_progress (optional: mastery / spaced repetition) -------
-- Current state per user+card. card_review is the history; this is the summary.
-- Deleted with the card, since progress on a nonexistent card is meaningless.
CREATE TABLE user_card_progress (
  user_id          BIGINT   NOT NULL REFERENCES app_user (id)  ON DELETE CASCADE,
  card_id          BIGINT   NOT NULL REFERENCES flashcard (id) ON DELETE CASCADE,
  mastery_level    SMALLINT NOT NULL DEFAULT 0 CHECK (mastery_level BETWEEN 0 AND 5),
  correct_streak   INT      NOT NULL DEFAULT 0 CHECK (correct_streak >= 0),
  last_reviewed_at TIMESTAMPTZ,
  next_review_at   TIMESTAMPTZ,
  PRIMARY KEY (user_id, card_id)
);
CREATE INDEX ix_progress_due ON user_card_progress (user_id, next_review_at);

-- =============================================================================
-- Operational notes
--   * Purge the outbox on a schedule, e.g.:
--       DELETE FROM index_outbox WHERE processed_at < now() - interval '7 days';
--   * Hard delete also has to reach: the search index, caches, replicas, logs
--     and backups (let backups expire on a fixed retention window).
--   * Reorder cards inside one transaction (the position constraint is deferred):
--       BEGIN;
--       UPDATE flashcard SET position = 1 WHERE id = :a;
--       UPDATE flashcard SET position = 0 WHERE id = :b;
--       COMMIT;
-- =============================================================================
```

---

## 6. Backups and privacy

Deleting rows only fixes the primary database. The same text may remain in the search index, caches, replicas, logs, and backups.

- **Search index:** handled by the outbox (section 4), provided the indexer keeps running.
- **Caches and replicas:** set sensible expiry; check lag.
- **Logs:** never log card content.
- **Backups:** you can't edit a snapshot record by record, so the accepted approach is: delete from live systems immediately, put backup copies *beyond use*, and let a documented retention schedule overwrite them.

### Three commitments

1. **Fixed retention window** (e.g. 30 days) covering every backup type, including WAL archives. Shorter means data disappears sooner but leaves less recovery time.
2. **Beyond use:** nobody queries or browses backups; they are encrypted, access-restricted and logged, and used only for disaster recovery.
3. **Post-restore re-delete.** Restoring an old backup resurrects data deleted since. After any restore, re-run all deletions recorded since the backup was taken.

### Deletion log (add to the schema)

The outbox is purged after days, which is shorter than the backup window, so keep a separate long-lived log of **ids only**:

```sql
CREATE TABLE deletion_log (
  entity_type TEXT        NOT NULL CHECK (entity_type IN ('user','flashcard_set','flashcard')),
  entity_id   BIGINT      NOT NULL,
  deleted_at  TIMESTAMPTZ NOT NULL DEFAULT now(),
  PRIMARY KEY (entity_type, entity_id)
);
-- Keep rows at least as long as the longest backup, then purge:
-- DELETE FROM deletion_log WHERE deleted_at < now() - interval '35 days';
```

- Identity ids are never reused, so replaying deletes cannot remove the wrong rows.
- The log must live outside the restored database (or come from a source newer than the backup), or the restore erases the list of things to delete.
- Cascades handle children: re-deleting a set removes its cards and sessions.
- Populate it with delete triggers on `app_user`, `flashcard_set`, and `flashcard` (not yet in the schema file).

### Sample privacy-policy wording (adapt with advice)

> When you delete a study set or your account, we remove it from our live systems immediately. Encrypted backup copies may remain for up to 30 days before being automatically overwritten. During that time we don't use backup data for any purpose other than disaster recovery, and if we restore from a backup we delete your data again.

The same facts belong in any reply to a user who asks for confirmation of deletion: live data deleted, copies remain in backups, the date they'll be overwritten, and no other use in the meantime.

### Regulatory note

The UK ICO accepts "beyond use" until backups are overwritten on a documented schedule. EU authorities have not all taken the same line; French guidance reportedly allows temporary retention when clearly disclosed with the retention period. GDPR Article 5(2) makes you responsible for demonstrating compliance, so keep written backup policies and restore-test records. CCPA and other regimes differ. Confirm which apply with legal counsel.

---

## 7. Checklist

- [ ] Load the schema and add `deletion_log` plus its triggers
- [ ] Run an indexer worker that consumes `index_outbox`
- [ ] Schedule outbox and deletion-log purges
- [ ] Configure backup retention (including WAL archives) and encryption
- [ ] Restrict and log backup access
- [ ] Write and test the post-restore re-delete step in the runbook
- [ ] Never log card content
- [ ] Add the backup-retention statement to the privacy policy
- [ ] Keep records of backup policy and restore tests
- [ ] Confirm applicable regulations with counsel

## 8. Open decisions

- Backup retention length (30 days is typical; weigh against recovery needs)
- Whether deleting a set should delete other users' sessions or keep them with `set_id = NULL`
- Whether to keep anonymized review stats after a card is deleted
- Whether private sets should ever reach the indexing pipeline
- Technology stack for the CRUD API (not yet chosen)
````

---

## 5. Automated extraction script

Save this as `extract_snapshot.py` anywhere outside the output directory. Run it with Python 3.8+:

```
python extract_snapshot.py PROJECT_SNAPSHOT.md ./flashcard-app
```

It writes every file from §4, creates the empty `Models/` directory, and prints `OK` or `FAIL` for each hash. It exits with code 0 only when all 29 files match.

```python
#!/usr/bin/env python3
"""Recreate the FlashCardApp project from PROJECT_SNAPSHOT.md.

Usage:  python extract_snapshot.py PROJECT_SNAPSHOT.md <output-dir>
Exit code 0 means every file was written and its SHA-256 matches the snapshot.
"""
import hashlib
import pathlib
import re
import sys

MARKER = re.compile(
    r"^<!-- file: (?P<path>\S+) \| bom=(?P<bom>yes|no) \| eol=(?P<eol>crlf|lf) "
    r"\| final_newline=(?P<final>yes|no) \| sha256=(?P<sha>[0-9a-f]{64}) -->$"
)
EXPECTED_FILES = 29
EMPTY_DIRS = ["src/FlashCardApp/Models"]


def main(md_path: str, out_dir: str) -> int:
    # Universal-newline read: works whether this markdown is stored with LF or CRLF.
    lines = pathlib.Path(md_path).read_text(encoding="utf-8-sig").split("\n")
    out = pathlib.Path(out_dir)
    written = failures = 0
    i = 0
    while i < len(lines):
        m = MARKER.match(lines[i])
        if not m:
            i += 1
            continue
        fence = re.match(r"^`{3,}", lines[i + 1]).group(0)
        end = lines.index(fence, i + 2)  # closing fence: same backticks, no tag
        text = "\n".join(lines[i + 2:end])
        if m["final"] == "yes":
            text += "\n"
        if m["eol"] == "crlf":
            text = text.replace("\n", "\r\n")
        data = (b"\xef\xbb\xbf" if m["bom"] == "yes" else b"") + text.encode("utf-8")
        target = out / m["path"]
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(data)
        ok = hashlib.sha256(data).hexdigest() == m["sha"]
        failures += not ok
        written += 1
        print(f"{'OK  ' if ok else 'FAIL'}  {m['path']}")
        i = end + 1
    for d in EMPTY_DIRS:
        (out / d).mkdir(parents=True, exist_ok=True)
    print(f"{written} files written to {out.resolve()}; {failures} hash mismatch(es)")
    return 0 if failures == 0 and written == EXPECTED_FILES else 1


if __name__ == "__main__":
    sys.exit(main(sys.argv[1], sys.argv[2]))
```

---

## 6. Regenerating `swagger.json`

`swagger.json` (127,264 bytes) is the app's own `/swagger/v1/swagger.json` output for the code above. It is not embedded here because the running app regenerates it exactly. The saved file differs from the raw HTTP response in only three ways, all matching a Windows PowerShell 5.1 `>` redirect:

- encoding is UTF-16 LE with a BOM;
- every line ending is CRLF;
- one extra CRLF is appended at the end.

Steps (from the regenerated repository root):

1. Start the app: `dotnet run --project src/FlashCardApp --launch-profile http`. It must run in the `Development` environment, which the launch profile sets, or Swagger is not served.
2. In a second terminal, save the script below as `regenerate_swagger.py` and run `python regenerate_swagger.py http://localhost:5260`.
3. Stop the app.

```python
#!/usr/bin/env python3
"""Fetch the running app's OpenAPI document and save it exactly like the snapshot's swagger.json."""
import hashlib
import sys
import urllib.request

EXPECTED_SHA256 = "0b98e76d190eb5fa4002852400f56bd122121af13bcee6e536f3b9f62494bfb0"
base = sys.argv[1] if len(sys.argv) > 1 else "http://localhost:5260"
with urllib.request.urlopen(base.rstrip("/") + "/swagger/v1/swagger.json") as resp:
    text = resp.read().decode("utf-8")
text = text.replace("\r\n", "\n").replace("\n", "\r\n") + "\r\n"
data = b"\xff\xfe" + text.encode("utf-16-le")
with open("swagger.json", "wb") as fh:
    fh.write(data)
actual = hashlib.sha256(data).hexdigest()
print("swagger.json:", "OK" if actual == EXPECTED_SHA256 else "HASH MISMATCH " + actual)
sys.exit(0 if actual == EXPECTED_SHA256 else 1)
```

The document's paths (12 operations), tags, schemas and `info` block must match §1. If the hash differs, the most likely cause is a different Swashbuckle or Microsoft.OpenApi version from those in §1.

---

## 7. Verification checklist

1. `extract_snapshot.py` reports 29 × `OK` and exits 0. To check hashes manually instead:
   - PowerShell: `Get-FileHash -Algorithm SHA256 <file>`
   - Linux/macOS: `sha256sum <file>`

   Compare each result with §2.
2. `src/FlashCardApp/Models/` exists and is empty.
3. `dotnet build FlashCardApp.slnx` reports **0 errors** and `2 Warning(s)`. Both are the same expected NU1903 warning, reported once for the project and once for the solution (§3, item 13).
4. With the app running, `GET /swagger/index.html` returns 200, `GET /does-not-exist` returns the 404 JSON envelope, and `GET /api/flash-card-sets` returns the 500 envelope from §3, item 1.
5. `regenerate_swagger.py` prints `OK`.
