#Requires -Version 7.0

[CmdletBinding()]
param(
    [ValidateNotNullOrEmpty()]
    [string]$PAT,

    [ValidateNotNullOrEmpty()]
    [string]$SolutionFolder = $PSScriptRoot
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "Required command 'dotnet' was not found on PATH."
}

$solutions = @(Get-ChildItem -LiteralPath $SolutionFolder -Filter '*.sln' -File)
if ($solutions.Count -ne 1) {
    throw "Expected exactly one solution in '$SolutionFolder'."
}

if (-not $PSBoundParameters.ContainsKey('PAT')) {
    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
        throw "Required command 'gh' was not found on PATH."
    }

    $statusJson = & gh auth status --json hosts
    $account = $null
    if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace($statusJson)) {
        $state = $statusJson | ConvertFrom-Json
        $account = $state.hosts.'github.com' | Where-Object { $_.active } | Select-Object -First 1
    }

    if (-not $account) {
        & gh auth login --hostname github.com --scopes 'write:packages,read:packages'
        if ($LASTEXITCODE -ne 0) {
            throw 'GitHub authentication failed.'
        }
    }
    else {
        $scopes = ($account.scopes -split ',').Trim()
        if ($scopes -notcontains 'write:packages') {
            & gh auth refresh --hostname github.com --scopes 'write:packages,read:packages'
            if ($LASTEXITCODE -ne 0) {
                throw 'Could not grant the required GitHub package scopes.'
            }
        }
    }

    $PAT = & gh auth token --hostname github.com
    if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($PAT)) {
        throw 'Could not retrieve the GitHub authentication token.'
    }
}

if ([string]::IsNullOrWhiteSpace($PAT)) {
    throw 'The PAT must not be blank.'
}

$PAT = $PAT.Trim()
$packageDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "StructDocExtractor-packages-$([guid]::NewGuid())"

Push-Location $SolutionFolder
try {
    New-Item -ItemType Directory -Path $packageDirectory | Out-Null
    & dotnet pack $solutions[0].FullName --configuration Release --output $packageDirectory
    if ($LASTEXITCODE -ne 0) {
        throw 'Packing the solution failed. No packages were pushed.'
    }

    $packages = @(Get-ChildItem -LiteralPath $packageDirectory -Filter '*.nupkg' -File)
    if ($packages.Count -eq 0) {
        throw 'Packing did not produce any NuGet packages.'
    }

    foreach ($package in $packages) {
        & dotnet nuget push $package.FullName --source github --api-key $PAT --skip-duplicate
        if ($LASTEXITCODE -ne 0) {
            throw "Pushing '$($package.Name)' failed."
        }
    }
}
finally {
    if (Test-Path -LiteralPath $packageDirectory) {
        Remove-Item -LiteralPath $packageDirectory -Recurse -Force
    }
    Pop-Location
    Remove-Variable -Name PAT
}
