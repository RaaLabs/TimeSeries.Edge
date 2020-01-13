#!/bin/bash
# Assumes that the NuGet API key has been set: 
# $ nuget setApiKey <key> -Source https://api.nuget.org/v3/index.json

export PACKAGEDIR=$PWD/Artifacts/NuGet
export DOLITTLERELEASE=true

rm -rf $PWD/Artifacts

$PWD/Build/build.sh DeployFromLocal

for f in $PACKAGEDIR/*.symbols.nupkg; do
    nuget push $f -Source https://api.nuget.org/v3/index.json   
done

cd $PWD/Build
git reset --hard
cd $PWD