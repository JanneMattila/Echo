# Instructions

## Working with 'Echo'

### How to create image locally

```batch
# Build container image
docker build . -t echo:latest

# Run container using command
docker run -p "2001:80" echo:latest
``` 

### How to deploy to Azure Container Instances (ACI)

Deploy published image to [Azure Container Instances (ACI)](https://docs.microsoft.com/en-us/azure/container-instances/) the Azure CLI way:

```batch
# Variables
aciName="echo"
resourceGroup="echo-dev-rg"
location="westeurope"
image="jannemattila/echo:latest"

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
$image="jannemattila/echo:latest"

# Login to Azure
Login-AzureRmAccount

# *Explicitly* select your working context
Select-AzureRmSubscription -SubscriptionName <YourSubscriptionName>

# Create new resource group
New-AzureRmResourceGroup -Name $resourceGroup -Location $location

# Create ACI
New-AzureRmContainerGroup -Name $aciName -Image $image -ResourceGroupName $resourceGroup -IpAddressType Public

# Show the properties
Get-AzureRmContainerGroup -Name $aciName -ResourceGroupName $resourceGroup

# Show the logs
Get-AzureRmContainerInstanceLog -ContainerGroupName $aciName -ResourceGroupName $resourceGroup

# Wipe out the resources
Remove-AzureRmResourceGroup -Name $resourceGroup -Force
```
