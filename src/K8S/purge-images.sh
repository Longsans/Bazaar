#!/bin/bash

absolutePath=$(readlink -f "$0");
absolutePath=$(dirname -- "$absolutePath");
cd "$absolutePath" || exit 1;
source ".env"

printf "Delete all image contain [bazaar-]: "
read -r str
if [ -z "$str" ]; then
    str=bazaar-;
fi

if [ -z "$DOCKER_REGISTRY" ]; then
    echo "Found no registry."
    docker rmi -f $(docker images "*${str}*")
fi
docker rmi -f $(docker images "*/${str}*" -q)