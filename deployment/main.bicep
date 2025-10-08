@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('The Azure region for resource deployment')
param location string = resourceGroup().location

param tags object = { 
  environment: 'production'
  project: 'devhabit'
  owner: 'farooq-teqniqly'
}

module appservice_env 'appservice-env/appservice-env.bicep' = {
  name: 'appservice-env'
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
