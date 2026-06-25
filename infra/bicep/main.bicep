targetScope = 'subscription'

@description('Azure region for all resources.')
param location string = 'eastus'

@description('Environment suffix used in resource names (for example: dev, qa, prod).')
param environmentName string = 'dev'

@description('Resource group name to create.')
param resourceGroupName string = 'rg-logs-viewer-${environmentName}'

@description('Storage account name (3-24 lowercase letters and numbers).')
param storageAccountName string = take('st${uniqueString(subscription().id, resourceGroupName)}', 24)

@description('Function App name.')
param functionAppName string = 'func-logs-viewer-${environmentName}'

@description('App Service plan name required by Function App.')
param hostingPlanName string = 'asp-logs-viewer-${environmentName}'

@description('Queue name consumed by the function trigger.')
param queueName string = 'logsqueue'

@description('Function runtime stack.')
@allowed([
  'dotnet-isolated'
])
param functionsWorkerRuntime string = 'dotnet-isolated'

@description('Function runtime version.')
param functionsExtensionVersion string = '~4'

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: resourceGroupName
  location: location
}

module workload './workload.bicep' = {
  scope: rg
  params: {
    location: location
    storageAccountName: storageAccountName
    queueName: queueName
    hostingPlanName: hostingPlanName
    functionAppName: functionAppName
    functionsWorkerRuntime: functionsWorkerRuntime
    functionsExtensionVersion: functionsExtensionVersion
  }
}

output createdResourceGroup string = rg.name
output createdStorageAccount string = workload.outputs.createdStorageAccount
output createdQueue string = workload.outputs.createdQueue
output createdFunctionApp string = workload.outputs.createdFunctionApp
