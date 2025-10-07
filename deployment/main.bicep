targetScope = 'subscription'

@description('The root name used to generate unique resource names')
@minLength(3)
@maxLength(50)
param rootName string

@description('The Azure region for resource deployment')
param location string = 'westus2'

@description('The name of the resource group to create')
param resourceGroupName string = '${rootName}-rg'

param tags object = { 
  environment: 'production'
  project: 'devhabit'
  owner: 'farooq-teqniqly'
}

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
  tags: tags
}

module appservice_env 'appservice-env/appservice-env.bicep' = {
  name: 'appservice-env'
  scope: rg
  params: {
    location: location
    rootName: rootName
    tags: tags
  }
}

output appservice_env_AZURE_CONTAINER_REGISTRY_NAME string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_NAME

output appservice_env_AZURE_CONTAINER_REGISTRY_ENDPOINT string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID

output appservice_env_planId string = appservice_env.outputs.planId

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID
