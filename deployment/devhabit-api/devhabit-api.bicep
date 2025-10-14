@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('The resource ID of the App Service plan')
param appservice_env_outputs_planid string

@description('The resource ID of the user-assigned managed identity')
param appservice_env_outputs_azure_container_registry_managed_identity_id string

@description('The client ID of the user-assigned managed identity')
param appservice_env_outputs_azure_container_registry_managed_identity_client_id string

@description('The full container image URL in the format registry/image:tag')
param devhabit_api_containerimage string

@description('The container port number as a string (for environment variable HTTP_PORTS)')
param devhabit_api_containerport string = '8080'

@description('The database connection string for the application to connect to the database')
@secure()
param  devhabit_api_database_connection_string string

param tags object = { 
  environment: 'production'
  project: 'devhabit'
  owner: 'farooq-teqniqly'
}

resource webapp 'Microsoft.Web/sites@2024-11-01' = {
  name: take('${rootName}-appsvc', 60)
  location: location
  tags: tags
  properties: {
    serverFarmId: appservice_env_outputs_planid
    siteConfig: {
      linuxFxVersion: 'SITECONTAINERS'
      acrUseManagedIdentityCreds: true
      acrUserManagedIdentityID: appservice_env_outputs_azure_container_registry_managed_identity_client_id
      connectionStrings: [
        {
          name: 'devhabit-db'
          connectionString: devhabit_api_database_connection_string
          type: 'SQLAzure'
        }
      ]
      appSettings: [
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
          value: 'true'
        }
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
          value: 'true'
        }
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
          value: 'in_memory'
        }
        {
          name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
          value: 'true'
        }
        {
          name: 'HTTP_PORTS'
          value: devhabit_api_containerport
        }
      ]
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${appservice_env_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}


resource mainContainer 'Microsoft.Web/sites/sitecontainers@2024-11-01' = {
  name: 'main'
  properties: {
    authType: 'UserAssigned'
    image: devhabit_api_containerimage
    isMain: true
    userManagedIdentityClientId: appservice_env_outputs_azure_container_registry_managed_identity_client_id
  }
  parent: webapp
  
}

output webAppName string = webapp.name

output webAppId string = webapp.id

output webAppDefaultHostName string = webapp.properties.defaultHostName

output webAppUrl string = 'https://${webapp.properties.defaultHostName}'
