# DevHabit API

This is a .NET Aspire-based application for the DevHabit platform.

## Prerequisites

- .NET 9.0 SDK
- Docker Desktop (for local development)
- Azure CLI (for deployment)
- GitHub repository with Actions enabled

## Local Development

1. Clone the repository:

   ```bash
   git clone https://github.com/farooq-teqniqly/devhabit-api.git
   cd devhabit-api
   ```

2. Build and run the application:

   ```bash
   dotnet run --project DevHabit.AppHost
   ```

This will start the Aspire dashboard and all services locally using Docker Compose.

## Database Migrations

This project uses Entity Framework Core for database operations. You may need to update the EF Core tools to work with the latest migrations.

### Updating .NET EF Core Tool

The .NET Entity Framework Core tools are required for database migrations. To ensure you have the latest version:

1. **Check current version**:

   ```bash
   dotnet ef --version
   ```

2. **Update to the latest version**:

   ```bash
   dotnet tool update --global dotnet-ef
   ```

3. **Install if not already installed**:

   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Verify installation**:

   ```bash
   dotnet ef --version
   ```

### Running Migrations

After ensuring you have the latest EF tools, you can run migrations:

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project DevHabit.Api

# Update the database
dotnet ef database update --project DevHabit.Api
```

## Deployment to Azure

The project includes automated deployment via GitHub Actions using Azure Bicep templates and Azure CLI.

### Setup Prerequisites

1. **Azure Subscription**: Ensure you have an active Azure subscription and get your subscription ID:

   ```bash
   az account show --query id -o tsv
   ```

2. **Service Principal**: Create an Azure service principal for GitHub Actions:

   ```bash
   az ad sp create-for-rbac --name "devhabit-api-sp" --role Contributor --scopes /subscriptions/YOUR_SUBSCRIPTION_ID --sdk-auth

   az role assignment create --assignee $(az ad sp list --display-name "devhabit-api-sp" --query "[].appId" -o tsv) --role "User Access Administrator" --scope /subscriptions/YOUR_SUBSCRIPTION_ID
   ```

3. **GitHub Secrets**: Add the following secrets to your GitHub repository (Settings > Secrets and variables > Actions):
   - `AZURE_CREDENTIALS`: The complete JSON output from the service principal creation
   - `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID
   - `SQL_USERNAME`: The admin username for the SQL server (e.g., 'sqladmin')
   - `SQL_PASSWORD`: The admin password for the SQL server (must meet Azure SQL password requirements)

### Resource Naming

The deployment uses deterministic naming with the following pattern:

- Managed Identity: `{rootName}-mid`
- Container Registry: `{rootName}-acr`
- App Service Plan: `{rootName}-asp`
- App Service: `{rootName}-appsvc`
- SQL Server: `{rootName}-sql`
- Resource Group: `{rootName}-rg`

Default root name: `tq123devhabit`

⚠️ **Important**: Choose a unique root name to avoid naming conflicts with existing Azure resources. The root name becomes part of all resource names and should be globally unique within your Azure subscription. The root name must contain only letters, numbers, and underscores (no hyphens).

### Triggering Deployment

The GitHub Action (`deploy.yml`) runs automatically on:

- Push to the `main` branch
- Manual trigger via GitHub Actions UI

#### Manual Deployment

1. Go to your GitHub repository > Actions tab
2. Select "Deploy to Azure" workflow
3. Click "Run workflow"
4. Enter the desired root name (default: `tq123devhabit`)
5. Click "Run workflow"

This will:

- Deploy infrastructure resources (MI, ACR, ASP) to the specified region (default: westus2)
- Build and push Docker container images
- Deploy the web app with container settings

### Manual CLI Deployment (Alternative)

If you prefer manual deployment using the .NET Aspire CLI:

1. Install .NET Aspire:

   ```bash
   dotnet tool install --global Aspire.Cli --prerelease
   ```

2. Authenticate with Azure:

   ```bash
   az login
   ```

3. Deploy using Aspire CLI:

   ```bash
   aspire deploy
   ```

   Follow the prompts to select subscription, region (westus2), resource group, and other settings.

### Monitoring

After deployment, you can monitor the application through:

- Azure Portal: Check the resource group `{rootName}-rg`
- App Service logs
- Azure Container Registry for image history

## Architecture

- **DevHabit.Api**: Main API project (ASP.NET Core Web API)
- **DevHabit.ServiceDefaults**: Shared service configurations
- **DevHabit.AppHost**: Aspire application host for orchestration
