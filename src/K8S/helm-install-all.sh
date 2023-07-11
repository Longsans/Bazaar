#!/bin/bash

workDir=$( readlink -f "$0" );
workDir=$( dirname -- "$workDir" );

function install() {
    name="$1";
    chartName="$2";
    yamlName="$3"

    "$workDir"/helm-install.sh "$name" "$chartName" "$yamlName" "$4" "$5"
    sleep 10s; #This is required
}

install rabbitmq eventbus rabbitmq y y
install dashboard dashboard dashboard y y
install webapigw proxy webapigw y y
install internallb proxy internallb y y
install webbff webbff webbff y y
install ingressgw consulgw ingressgw y y
sleep 10s;
install configs configs configs y y

sleep 20s;
install data-volume volume data y n

echo "> Waiting for rabbitmq pod"
sleep 60s;
kubectl wait --namespace default --for=condition=ready pod/rabbitmq-0 --timeout=30s


"$workDir"/helm-install-services.sh