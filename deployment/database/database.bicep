@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

@description('The root name used to generate unique resource names (letters, numbers, underscores only - no hyphens)')
@minLength(3)
@maxLength(30)
param rootName string

@description('Tags to apply to resources')
param tags object = {}

@description('The principal ID of the user-assigned managed identity')
param managedIdentityPrincipalId string

@description('The SQL server admin username')
param sqlUsername string

@secure()
@description('The SQL server admin password')
param sqlPassword string

resource sqlServer 'Microsoft.Sql/servers@2024-05-01-preview' = {
  name: take('${rootName}-sql', 63)
  location: location
  properties: {
    administratorLogin: sqlUsername
    administratorLoginPassword: sqlPassword
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: false
      sid: managedIdentityPrincipalId
      tenantId: subscription().tenantId
      login: '${rootName}-mi-admin'
      principalType: 'ServicePrincipal'
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
  tags: tags
}

resource firewallRuleAllowAllAzure 'Microsoft.Sql/servers/firewallRules@2024-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource database 'Microsoft.Sql/servers/databases@2024-05-01-preview' = {
  parent: sqlServer
  name: 'devhabit'
  location: location
  sku: {
    name: 'S0'
    tier: 'Standard'
  }
  tags: tags
}

output sqlServerName string = sqlServer.name

output sqlServerFullyQualifiedDomainName string = sqlServer.properties.fullyQualifiedDomainName

output databaseName string = database.name
