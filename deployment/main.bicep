targetScope = 'subscription'

param resourceGroupName string

param location string

param principalId string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module appservice_env 'appservice-env/appservice-env.bicep' = {
  name: 'appservice-env'
  scope: rg
  params: {
    location: location
    userPrincipalId: principalId
  }
}

output appservice_env_AZURE_CONTAINER_REGISTRY_NAME string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_NAME

output appservice_env_AZURE_CONTAINER_REGISTRY_ENDPOINT string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID

output appservice_env_planId string = appservice_env.outputs.planId

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID