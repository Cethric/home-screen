Push-Location -Path $PSScriptRoot/../

version = $( dotnet tool run dotnet-gitversion /output json /showvariable FullSemVer )

Read-Host "Confirm Destroy"

aspirate destroy

Read-Host "Confirm Generation of version $version"

aspirate generate --image-pull-policy Always --container-image-tag latest --container-image-tag $version

Read-Host "Confrim Applying version $version"

aspirate apply

Pop-Location