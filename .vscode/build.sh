#!/bin/bash
set -e

CONFIGURATION="Debug"
TARGET="$HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/RimWorld/Mods/ilvyion.NutrientDispenserDispensableFix"

mkdir -p .savedatafolder/1.6
mkdir -p .savedatafolder/1.5

# build dlls
export RimWorldVersion="1.5"
dotnet build --configuration "$CONFIGURATION" ilvyion.NutrientDispenserDispensableFix.sln
export RimWorldVersion="1.6"
dotnet build --configuration "$CONFIGURATION" ilvyion.NutrientDispenserDispensableFix.sln

# remove mod folder
rm -rf "$TARGET"

# copy mod files
mkdir -p "$TARGET"
cp -r 1.5 "$TARGET/1.5"
cp -r 1.6 "$TARGET/1.6"
cp -r Common "$TARGET/Common"

# copy interop mod files
# <NONE>

# copy metadata files
mkdir -p "$TARGET/About"
cp About/About.xml "$TARGET/About/"
cp About/Preview.png "$TARGET/About/"
cp About/ModIcon.png "$TARGET/About/"
cp About/PublishedFileId.txt "$TARGET/About/"

# copy other files
cp CHANGELOG.md "$TARGET/"
cp LICENSE "$TARGET/"
cp LICENSE.Apache-2.0 "$TARGET/"
cp LICENSE.MIT "$TARGET/"
cp README.md "$TARGET/"
cp LoadFolders.xml "$TARGET/"

# Trigger auto-hotswap
mkdir -p "$TARGET/1.5/Assemblies"
touch "$TARGET/1.5/Assemblies/ilvyion.NutrientDispenserDispensableFix.dll.hotswap"
mkdir -p "$TARGET/1.6/Assemblies"
touch "$TARGET/1.6/Assemblies/ilvyion.NutrientDispenserDispensableFix.dll.hotswap"
