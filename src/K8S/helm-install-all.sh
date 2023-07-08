#!/bin/bash

workDir=$( readlink -f "$0" );
workDir=$( dirname -- "$workDir" );

function install() {
    name="$1";
    chartName="$2";
    yamlName="$3"

    "$workDir"/helm-install.sh "$name" "$chartName" "$yamlName" "$4" "$5"
    sleep 5s; #This is required
}

install rabbitmq eventbus rabbitmq y y
install configs configs configs y y
install dashboard dashboard dashboard y y
install webapigw apigateway webapigw y y
install internallb apigateway internallb y y
install webbff webbff webbff y y
install data-volume volume data y y

sleep 40s;
echo "> Waiting for rabbitmq pod"
kubectl wait --namespace default --for=condition=ready pod/rabbitmq-0 --timeout=30s


"$workDir"/helm-install-services.sh