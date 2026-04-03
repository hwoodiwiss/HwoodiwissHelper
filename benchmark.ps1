#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param(
    [switch] $PublishResults,
    [Parameter(Mandatory = $false)][string] $Framework = "net10.0",
    [Parameter(Mandatory = $false)][string] $Filter = "*"
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "build" "install-sdk.ps1")
$dotnet = Join-Path "$env:DOTNET_INSTALL_DIR" "dotnet"

$additionalArgs = @()

if ($PublishResults) {
    $additionalArgs += "--exporters", "json"
}

if ($Filter) {
    $additionalArgs += "--filter", $Filter
}

$benchmarkProject = Join-Path $PSScriptRoot "benchmarks" "HwoodiwissHelper.Benchmarks" "HwoodiwissHelper.Benchmarks.csproj"

& $dotnet run --project $benchmarkProject --configuration Release -- $additionalArgs

exit $LASTEXITCODE
