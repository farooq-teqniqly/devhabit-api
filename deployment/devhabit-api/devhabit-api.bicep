@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param appservice_env_outputs_azure_container_registry_endpoint string

param appservice_env_outputs_planid string

param appservice_env_outputs_azure_container_registry_managed_identity_id string

param appservice_env_outputs_azure_container_registry_managed_identity_client_id string

param devhabit_api_containerimage string

param devhabit_api_containerport string

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

resource webapp 'Microsoft.Web/sites@2024-11-01' = {
  name: take('${toLower('devhabit-api')}-${uniqueString(resourceGroup().id)}', 60)
  location: location
  properties: {
    serverFarmId: appservice_env_outputs_planid
    siteConfig: {
      linuxFxVersion: 'SITECONTAINERS'
      acrUseManagedIdentityCreds: true
      acrUserManagedIdentityID: appservice_env_outputs_azure_container_registry_managed_identity_client_id
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