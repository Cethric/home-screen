aspirate generate --image-pull-policy Always --container-image-tag latest --container-image-tag $(dotnet tool run dotnet-gitversion /output json /showvariable FullSemVer)

aspirate apply