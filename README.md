# Instructions

## Build Status

[![Build Status](https://dev.azure.com/jannemattila/_apis/build/status/JanneMattila.Echo?branchName=master)](https://dev.azure.com/jannemattila/_build/latest?definitionId=43&branchName=master)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Working with 'Echo'

### How to create image locally

```batch
# Build container image
docker build . -t echo:latest

# Run container using command
docker run -p "2001:80" echo:latest
``` 

### How to test locally

Using `curl`:

```batch
curl -d '{ "firstName": "John", "lastName": "Doe" }' -H "Content-Type: application/json" -X POST http://localhost:2001/api/echo
``` 

Using `PowerShell`:

```powershell
$url = "http://localhost:2001/api/echo"
$data = @{
    firstName = "John"
    lastName = "Doe"
}
$body = ConvertTo-Json $data
Invoke-RestMethod -Body $body -ContentType "application/json" -Method "POST" -DisableKeepAlive -Uri $url
``` 

### How to deploy to Azure Container Instances (ACI)

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure CLI way:

```batch
# Variables
aciName="echo"
resourceGroup="echo-dev-rg"
location="westeurope"
image="jannemattila/echo"

# Login to Azure
az login

# *Explicitly* select your working context
az account set --subscription <YourSubscriptionName>

# Create new resource group
az group create --name $resourceGroup --location $location

# Create ACI
az container create --name $aciName --image $image --resource-group $resourceGroup --ip-address public

# Show the properties
az container show --name $aciName --resource-group $resourceGroup

# Show the logs
az container logs --name $aciName --resource-group $resourceGroup

# Wipe out the resources
az group delete --name $resourceGroup -y
``` 

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure PowerShell way:

```powershell
# Variables
$aciName="echo"
$resourceGroup="echo-dev-rg"
$location="westeurope"
$image="jannemattila/echo"

# Login to Azure
Login-AzAccount

# *Explicitly* select your working context
Select-AzSubscription -SubscriptionName <YourSubscriptionName>

# Create new resource group
New-AzResourceGroup -Name $resourceGroup -Location $location

# Create ACI
New-AzContainerGroup -Name $aciName -Image $image -ResourceGroupName $resourceGroup -IpAddressType Public

# Show the properties
Get-AzContainerGroup -Name $aciName -ResourceGroupName $resourceGroup

# Show the logs
Get-AzContainerInstanceLog -ContainerGroupName $aciName -ResourceGroupName $resourceGroup

# Wipe out the resources
Remove-AzResourceGroup -Name $resourceGroup -Force
```
