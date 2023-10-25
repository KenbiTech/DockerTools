#!/bin/bash

# For DEBUG set -Eeuxo pipefail
set -Eeuo pipefail

echo '---------------------------------'
echo 'Setting variables '
echo '---------------------------------'

# Hardcoded
server=https://kenbi-artifacts-567677647219.d.codeartifact.eu-central-1.amazonaws.com/nuget/nuget-store/v3/index.json

# Hardcoded per project
relpath=
project=DockerTools
tagprefix=

echo '---------------------------------'
echo 'Packing '
echo '---------------------------------'
dotnet pack \
    --nologo \
    --configuration Release \
    "$relpath$project"

package=$(find "$relpath$project/bin/Release" -maxdepth 1 -name '*.nupkg' | tail -n1)

echo '---------------------------------'
echo 'You are about to push package to remote nuget server'
echo '---------------------------------'
dotnet nuget push \
    --source "$server" \
    "$package" \
    --skip-duplicate

version=${package/.nupkg/}
version=${version/$relpath$project\/bin\/Release\/Kenbi.$project./$tagprefix-}

echo "Version: $version"