param([string]$NUGET_SOURCE_URL = "http://packages.mk6.local")

$PSScriptRoot = split-path -parent $MyInvocation.MyCommand.Definition;
$TOOLS_DIR = Join-Path $PSScriptRoot "tools"
$NUGET_EXE = Join-Path $TOOLS_DIR "nuget.exe"
$NUGET_URL = "http://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$CAKE_BUILD_PACKAGE_ID = "MK6.Tools.CakeBuild"

# Make sure tools folder exists
if ((Test-Path $PSScriptRoot) -and !(Test-Path $TOOLS_DIR)) {
    Write-Verbose -Message "Creating tools directory..."
    New-Item -Path $TOOLS_DIR -Type directory | out-null
}


# Try find NuGet.exe in path if not exists
if (!(Test-Path $NUGET_EXE)) {
    Write-Verbose -Message "Trying to find nuget.exe in PATH..."
    $existingPaths = $Env:Path -Split ';' | Where-Object { (![string]::IsNullOrEmpty($_)) -and (Test-Path $_) }
    $NUGET_EXE_IN_PATH = Get-ChildItem -Path $existingPaths -Filter "nuget.exe" | Select -First 1
    if ($NUGET_EXE_IN_PATH -ne $null -and (Test-Path $NUGET_EXE_IN_PATH.FullName)) {
        Write-Verbose -Message  "Found in PATH at $($NUGET_EXE_IN_PATH.FullName)."
        $NUGET_EXE = $NUGET_EXE_IN_PATH.FullName
    }
}

# Try download NuGet.exe if not exists
if (!(Test-Path $NUGET_EXE)) {
    Write-Host "Downloading NuGet.exe..." -ForegroundColor White
    try {
        (New-Object System.Net.WebClient).DownloadFile($NUGET_URL, $NUGET_EXE)
    } catch {
        Throw "Could not download NuGet.exe."
    }
}

# Restore tools from NuGet
Push-Location
Set-Location $TOOLS_DIR
Write-Host "Restoring tools from NuGet..." -ForegroundColor White
rm $CAKE_BUILD_PACKAGE_ID -Recurse -Force
$NuGetOutput = Invoke-Expression "&`"$NUGET_EXE`" install $CAKE_BUILD_PACKAGE_ID -ExcludeVersion -OutputDirectory `"$TOOLS_DIR`" -Source `"$NUGET_SOURCE_URL`""
if ($LASTEXITCODE -ne 0) {
    Throw "An error occured while restoring NuGet tools."
}
Write-Verbose -Message ($NuGetOutput | out-string)
Pop-Location


#copy scripts from the TOOLS_DIR into the PSScriptRoot
Write-Host "Copying build files to solution root..." -ForegroundColor White
$cakeFilePath = Join-Path $PSScriptRoot "build.cake"
$buildPs1Path = Join-Path $PSScriptRoot "build.ps1"
if((Test-Path $cakeFilePath) -or (Test-Path $buildPs1Path)) {
  Write-Warning "Skipping copying of build.cake/.ps1 since they already exist in your solution root"
  cp "$TOOLS_DIR\$CAKE_BUILD_PACKAGE_ID\*" $PSScriptRoot -Recurse -Force -Exclude @("build.cake", "build.ps1")
}
else {
  cp "$TOOLS_DIR\$CAKE_BUILD_PACKAGE_ID\*" $PSScriptRoot -Recurse -Force
}

rm "$CAKE_BUILD_PACKAGE_ID.nupkg"

Write-Host "Build successfully bootstrapped!" -ForegroundColor Green
Write-Host

Write-Host "==================================================" -ForegroundColor white
Write-Host "====Examples on running your build using cake=====" -ForegroundColor white
Write-Host "==================================================" -ForegroundColor white
Write-Host
Write-Host "==================================================="
Write-Host "| Running with no params will run the build using |"
Write-Host "| the default 'Build' target from the build.cake  |"
Write-Host "| script. It also defaults the version to 1.0.0   |"
Write-Host "| and uses the configuration of Release. It will  |"
Write-Host "| attempt to find your .sln file. The .sln has to |"
Write-Host "| be in the same dir as the build scripts. Also,  |"
Write-Host "| there can only be a single .sln in that folder. |"
Write-Host "==================================================="
Write-Host "Example: .\build.ps1" -ForegroundColor White
Write-Host "==================================================="
Write-Host "| Running with all parameters specified           |"
Write-Host "==================================================="
Write-Host "Example: .\build.ps1 -Script .\build.cake -Target Package -Configuration Debug -AppVer 0.0.1 -SolutionPath .\MySln.sln" -ForegroundColor White
Write-Host "==================================================="