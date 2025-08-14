# OMNIPULSE

## Installation Manual (Docker - Development)

### Prerequisites

- Docker Desktop

> If you’re on Apple Silicon and use the official SQL Server image, performance is fine but emulated. We pin the container to `linux/amd64` automatically in Compose.

### Environment variables and secrets

- Copy [`.env.example`](./.env.example) to [`.env`](./.env) in the repository root:

```bash
cp .env.example .env
```

- Set a strong `SA_PASSWORD` (see [`.env.example`](./.env.example) for guidance).
- **Do not commit [`.env`](./.env) to version control.**
- Password must meet [SQL Server password complexity requirements](https://learn.microsoft.com/en-us/sql/relational-databases/security/password-policy#password-complexity).
- This is the SQL Server administrator password used by the DB and injected into the API connection string via Compose.

### Start the stack

You can use the helper script or plain Docker Compose.

#### Helper Script [`docker-dev.sh`](./docker-dev.sh) (recommended):

**Note:** If you have not already, make the script executable with:

```bash
chmod +x docker-dev.sh
```

Run the following commands as needed:

```bash
./docker-dev.sh start           # Full stack (API, DB, Frontend)
./docker-dev.sh client          # Frontend only
./docker-dev.sh server          # API & DB only
./docker-dev.sh logs            # Tail logs for all services
./docker-dev.sh logs server-dev # Tail logs for API only
```

#### Plain Docker Compose (alternative):

```bash
docker compose -f docker-compose.dev.yml up --build
```

Services and URLs:

- Frontend: `http://localhost:3000`
- Backend: `http://localhost:5100` (no page)
  - Swagger UI: `http://localhost:5100/swagger`
- SQL Server: `localhost:1433`

Data is persisted in the named volume `omnipulse_sql_data`.

### Stop the stack

#### Helper Script:

```bash
./docker-dev.sh stop
```

#### Plain Docker Compose:

```bash
docker compose -f docker-compose.dev.yml down
```

### Remove volumes (wipes DB data):

#### Helper Script:

```bash
./docker-dev.sh delete-sql # Removes only the SQL volume used by this project
./docker-dev.sh cleanup    # Stops services, removes volumes, prunes unused Docker resources
```

#### Plain Docker Compose:

```bash
docker compose -f docker-compose.dev.yml down -v
```

### Optional: enable development seeding

Database creation is automatic in Development. To enable seeding, set `SeedDatabaseEnabled=true` in [appsettings.Development.json](./server/src/api/appsettings.Development.json) and restart:

```json
"SeedDatabaseEnabled": true
```

Then re-run compose.

> **Note:** Enabling database seeding may cause unintended behaviour. Use this option with caution and ensure you do not have important data in your local environment before enabling seeding.

### Troubleshooting

- API cannot connect to DB: ensure [`.env`](./.env) exists and `SA_PASSWORD` meets complexity requirements.
- First boot timing: the compose file includes a DB healthcheck; `--wait` ensures the API starts after the DB is healthy. Inspect with:

```bash
docker compose -f docker-compose.dev.yml ps
docker logs <db-container-name>
```

## Backend

1. `cd server`
2. If you are using Visual Studio Code, install the official [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) extension from Microsoft
3. Install the [NuGet Gallery](https://marketplace.visualstudio.com/items?itemName=patcx.vscode-nuget-gallery) extension
4. Open "Solution Explorer"
5. Set the startup projects to Api project
6. `dotnet run`

### Documentation

The backend documentation can be generated and served using DocFX.

Navigate to the documentation directory:

```bash
cd server/docs/DocFX
```

Install DocFX via Homebrew:

```bash
brew install docfx
```

Build and serve the documentation:

```bash
docfx metadata
docfx build
docfx serve _site
```

Access the documentation at http://localhost:8080

## Frontend

```bash
cd client
npm install
npm run dev
```

## Git hooks with Husky

This project uses Husky to run pre-commit hooks.

To install Husky, run the following command:

```bash
npm install
```
