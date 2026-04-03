#! /usr/bin/env pwsh
#Requires -Version 7.0
#Requires -PSEdition Core

param(
    [switch] $PublishResults,
    [string] $Filter = ""
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "build" "install-sdk.ps1")

$additionalArgs = @()

if ($PublishResults) {
    $additionalArgs += "--exporters", "json"
}

if ($Filter) {
    $additionalArgs += "--filter", $Filter
}

$benchmarkProject = Join-Path $PSScriptRoot "benchmarks" "HwoodiwissHelper.Benchmarks" "HwoodiwissHelper.Benchmarks.csproj"

dotnet run --project $benchmarkProject --configuration Release -- $additionalArgs

exit $LASTEXITCODE
