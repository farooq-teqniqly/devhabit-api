@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('The Azure region for resource deployment')
param location string = resourceGroup().location

@description('The SQL server admin username')
@minLength(3)
@maxLength(30)
param sqlUsername string

@secure()
@description('The SQL server admin password')
@minLength(10)
param sqlPassword string

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

module database 'database/database.bicep' = {
  name: 'database'
  params: {
    location: location
    rootName: rootName
    tags: tags
    managedIdentityPrincipalId: appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_PRINCIPAL_ID
    sqlUsername: sqlUsername
    sqlPassword: sqlPassword
  }
}

output appservice_env_AZURE_CONTAINER_REGISTRY_NAME string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_NAME

output appservice_env_AZURE_CONTAINER_REGISTRY_ENDPOINT string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID

output appservice_env_planId string = appservice_env.outputs.planId

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID

output appservice_env_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_PRINCIPAL_ID string = appservice_env.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_PRINCIPAL_ID

output database_sqlServerName string = database.outputs.sqlServerName

output database_sqlServerFullyQualifiedDomainName string = database.outputs.sqlServerFullyQualifiedDomainName

output database_databaseName string = database.outputs.databaseName
