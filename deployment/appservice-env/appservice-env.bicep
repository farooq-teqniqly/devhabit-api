@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param userPrincipalId string = ''

param tags object = { }

resource appservice_env_mi 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' = {
  name: take('appservice_env_mi-${uniqueString(resourceGroup().id)}', 128)
  location: location
  tags: tags
}

resource appservice_env_acr 'Microsoft.ContainerRegistry/registries@2025-04-01' = {
  name: take('appserviceenvacr${uniqueString(resourceGroup().id)}', 50)
  location: location
  sku: {
    name: 'Basic'
  }
  tags: tags
}

resource appservice_env_acr_appservice_env_mi_AcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(appservice_env_acr.id, appservice_env_mi.id, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d'))
  properties: {
    principalId: appservice_env_mi.properties.principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
    principalType: 'ServicePrincipal'
  }
  scope: appservice_env_acr
}

resource appservice_env_asplan 'Microsoft.Web/serverfarms@2024-11-01' = {
  name: take('appserviceenvasplan-${uniqueString(resourceGroup().id)}', 60)
  location: location
  properties: {
    reserved: true
  }
  kind: 'Linux'
  sku: {
    name: 'P0V3'
    tier: 'Premium'
  }
}

output name string = appservice_env_asplan.name

output planId string = appservice_env_asplan.id

output AZURE_CONTAINER_REGISTRY_NAME string = appservice_env_acr.name

output AZURE_CONTAINER_REGISTRY_ENDPOINT string = appservice_env_acr.properties.loginServer

output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = appservice_env_mi.id

output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID string = appservice_env_mi.properties.clientId