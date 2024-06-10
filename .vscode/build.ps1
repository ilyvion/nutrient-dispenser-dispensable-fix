$ErrorActionPreference = 'Stop'

$env:RimWorldVersion = $args[0]
$Configuration = 'Debug'

$VersionTargetPrefix = "D:\RimWorld"
$VersionTargetSuffix = "Mods\ilvyion.NutrientDispenserDispensableFix"
$Target = "$VersionTargetPrefix\1.5\$VersionTargetSuffix"

# build dlls
dotnet build --configuration $Configuration ilvyion.NutrientDispenserDispensableFix.sln
if ($LASTEXITCODE -gt 0) {
    throw "Build failed"
}

# remove mod folder
Remove-Item -Path $Target -Recurse -ErrorAction SilentlyContinue

# copy mod files
Copy-Item -Path Assemblies $Target\Assemblies -Recurse

New-Item -Path $Target -ItemType Directory -Name About
Copy-Item -Path About\About.xml $Target\About
Copy-Item -Path About\Preview.png $Target\About
Copy-Item -Path About\ModIcon.png $Target\About
