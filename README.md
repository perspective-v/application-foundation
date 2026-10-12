# Application Foundation

Shared .NET libraries for PerspectiveV applications: primitives, common application utilities, EF Core data access, and ASP.NET Core web components.

The portable documentation source is under [`docs/`](docs/). It is organized as a reader guide and package-by-package API reference, and is formatted for Fumadocs and later import into the Librarian documentation portal. The [guide landing page](docs/index.mdx) links to each section.

## Packages

| Package | Purpose |
| --- | --- |
| `PerspectiveV.Primitives` | Entity and model contracts and base types. |
| `PerspectiveV.Common` | Configuration, identity, mapping, paging, helpers, logging, and mailing. |
| `PerspectiveV.EntityFramework` | EF Core extensions and async repository abstractions. |
| `PerspectiveV.Web` | MVC/REST base classes, response filters, and S3 storage. |

All projects currently target .NET 10. Package IDs, versions, and direct dependencies are defined in their `.csproj` files.

## Documentation guide

Use the guides in [`docs/`](docs/) to understand how to adopt and maintain these libraries:

- [Getting started](docs/getting-started.mdx) covers package setup and basic integration.
- [Architecture](docs/architecture.mdx) explains the repository structure and project dependency graph.
- [Versioning and migration](docs/versioning-and-migration.mdx) describes the current .NET 10 baseline and package versioning.
- [Repository workflow](docs/repository-workflow.mdx) covers local builds and package publishing.
- [Mapper and repository wiring](docs/mapper-and-repository-wiring.mdx) connects Mapperly, EF Core repositories, and the Web base classes.

Each library also has an overview and focused guides: [Primitives](docs/primitives/index.mdx) covers entity and model contracts; [Common](docs/common/index.mdx) covers shared configuration, identity, mapping, paging, and utilities; [Entity Framework](docs/entity-framework/index.mdx) covers data access and repositories; and [Web](docs/web/index.mdx) covers MVC, REST, and storage. The API reference under [`docs/reference/`](docs/reference/index.mdx) inventories each package's public types and members, with signatures, source locations, behavior, and relevant implementation caveats.

Navigation is defined by [`docs/meta.json`](docs/meta.json) and the package-level `meta.json` files. The pages use Markdown and static MDX, with source-relative internal links. Librarian resolves those links within its imported documentation tree; a standalone Fumadocs reader should configure `createRelativeLink` for them.

## Build

```powershell
dotnet restore
dotnet build PerspectiveVShell.sln -c Release
```

The solution currently contains four class library projects and no test project. See [architecture](docs/architecture.mdx), [getting started](docs/getting-started.mdx), and [repository workflow](docs/repository-workflow.mdx) for the package graph and publishing details.

## Package publishing

The [GitHub Actions publishing workflow](.github/workflows/publish-nuget.yml) runs on pushes to `deploy/feed`. It builds and pushes the packages to the configured Gitea NuGet feed. Configure the `nuget-publish` GitHub environment with `NUGET_FEED_URL`, `REGISTRY_USERNAME`, and `REGISTRY_PASSWORD` secrets.

`nuget.config`, `azure-pipelines.yml`, and `publish-nuget-*.bat` remain ignored local files; the GitHub workflow configures its feed source at run time. Never commit feed credentials.
