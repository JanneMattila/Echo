# Instructions

[![Build Status](https://dev.azure.com/jannemattila/jannemattila/_apis/build/status/JanneMattila.Echo?branchName=master)](https://dev.azure.com/jannemattila/jannemattila/_build/latest?definitionId=43&branchName=master)
[![Docker Pulls](https://img.shields.io/docker/pulls/jannemattila/echo?style=plastic)](https://hub.docker.com/r/jannemattila/echo)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

![Echo screenshot](https://user-images.githubusercontent.com/2357647/194714325-9507a653-d41b-40d0-a66c-c45cf60a70b3.png)

Echo can be used for testing various integrations and webhooks.
It echoes back the request headers and request body, so you can
study content better.

## Working with 'Echo'

### How to create image locally

```batch
# Build container image
docker build . -t echo:latest

# Run container using command
docker run -it --rm -p "8080:8080" echo:latest
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

Using Visual Studio Code with [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension:

```http
### Invoke Echo
POST http://localhost:2001/api/echo HTTP/1.1
Content-Type: application/json; charset=utf-8

{
    firstName: "John"
    lastName: "Doe"
}
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
