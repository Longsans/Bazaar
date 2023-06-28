#!/bin/bash

source '.env'

absolutePath=$(readlink -f "$0");
absolutePath=$(dirname -- "$absolutePath");

images=$(docker images "$DOCKER_REGISTRY*" -q);
for image in $images; do
    docker push "$image"
done;