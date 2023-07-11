#!/bin/bash

workDir=$( readlink -f "$0" );
workDir=$( dirname -- "$workDir" );

function install() {
    name="$1";
    chartName="$2";
    yamlName="$3"

    "$workDir"/helm-install.sh "$name" "$chartName" "$yamlName" "$4" "$5"
}

install rabbitmq eventbus rabbitmq y y
# install dashboard dashboard dashboard y y

chmod +x "$workDir"/helm-install-base.sh;
"$workDir"/helm-install-base.sh

chmod +x "$workDir"/helm-install-services.sh;
"$workDir"/helm-install-services.sh