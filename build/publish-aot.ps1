#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

# This script is for locally testing AOT compilation. It is not used in the build pipeline.

param (
    [string] $Configuration = "Release",
    [string] $RuntimeIdentifier
)

$publishProjectPaths = @(
    "src/HwoodiwissHelper/HwoodiwissHelper.csproj"
)

foreach ($publishProjectPath in $publishProjectPaths) {
    Write-Host "Publishing $publishProjectPath for $RuntimeIdentifier"
    dotnet publish $publishProjectPaths -c Release -r $RuntimeIdentifier --self-contained /p:GenerateAotMetadata=true

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}