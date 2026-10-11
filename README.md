# Application Foundation

Shared .NET libraries for PerspectiveV applications: primitives, common application utilities, EF Core data access, and ASP.NET Core web components.

The documentation source is [`index.mdx`](index.mdx), with detailed guides under [`docs/`](docs/). [`docs.json`](docs.json) remains the Mintlify configuration; a Fumadocs local preview in [`docs-site/`](docs-site/) renders the same source files.

## Local documentation preview

Use Node.js 22 or newer, then run:

```powershell
cd docs-site
npm ci
npm run dev
```

Open the URL printed in the terminal (normally http://127.0.0.1:3000; Next.js chooses another free port if needed). The preview reads the root homepage and `docs/` pages directly, so no duplicate content needs to be maintained.

## Packages

| Package | Purpose |
| --- | --- |
| `PerspectiveV.Primitives` | Entity and model contracts and base types. |
| `PerspectiveV.Common` | Configuration, identity, mapping, paging, helpers, logging, and mailing. |
| `PerspectiveV.EntityFramework` | EF Core extensions and async repository abstractions. |
| `PerspectiveV.Web` | MVC/REST base classes, response filters, and S3 storage. |

All projects currently target .NET 10. Package IDs, versions, and direct dependencies are defined in their `.csproj` files.

## Build

```powershell
dotnet restore
dotnet build PerspectiveVShell.sln -c Release
```

The solution currently contains four class library projects and no test project. See [architecture](docs/architecture.mdx), [getting started](docs/getting-started.mdx), and [repository workflow](docs/repository-workflow.mdx) for the package graph and publishing details.

## Package publishing

Run [the GitHub Actions publishing workflow](.github/workflows/publish-nuget.yml) manually from `main` after updating package versions in the project files. It builds and pushes the packages to the Gitea NuGet registry. Configure the `nuget-publish` GitHub environment with `REGISTRY_USERNAME` and `REGISTRY_PASSWORD` secrets for a Gitea account/token that has package write access.

`nuget.config`, `azure-pipelines.yml`, and `publish-nuget-*.bat` remain ignored local files; the GitHub workflow configures its feed source at run time. Never commit feed credentials.
