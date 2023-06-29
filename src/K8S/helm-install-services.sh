#!/bin/bash

workDir=$( readlink -f "$0" );
workDir=$( dirname -- "$workDir" );

function install() {
    name=$1;
    "$workDir"/helm-install.sh "$name"-api service "$name" y y;
}

for service in "$workDir"/HelmValues/service/*.yaml; do
    service=$( basename -- "$service" );
    install "${service%.*}";
done