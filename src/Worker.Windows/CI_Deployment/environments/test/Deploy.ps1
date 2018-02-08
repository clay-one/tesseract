param ([string]$sitePath)
$ErrorActionPreference = "Stop"
$cttPath = Resolve-Path "$sitePath\CI_Deployment\tools\ctt.exe"
$envPath = Resolve-Path "$sitePath\CI_Deployment\environments\test"
$connectionsConfigPathToCopy = Join-Path -Path $envPath -ChildPath Connections.config
$connectionsConfigDestinitionPath = Join-Path -Path $sitePath -ChildPath Connections.config

#Copy enviroment specific connections.config
Copy-Item -Path $connectionsConfigPathToCopy -Destination $connectionsConfigDestinitionPath -Force