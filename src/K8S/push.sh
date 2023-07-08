#!/bin/bash

absolutePath=$(readlink -f "$0");
absolutePath=$(dirname -- "$absolutePath");
cd "$absolutePath" || exit 1;
source ".env"

printf "Docker registry: %s\n" "$DOCKER_REGISTRY";
images=$(docker images --format "{{.Repository}}:latest" "$DOCKER_REGISTRY*" -q);
for image in $images; do
    printf "Uploading image %s\n" "$image"
    docker push "$image"
done;