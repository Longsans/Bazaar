#!/bin/bash

workDir=$( readlink -f "$0" );
workDir=$( dirname -- "$workDir" );

function install() {
    name="$1";
    chartName="$2";
    yamlName="$3"

    "$workDir"/helm-install.sh "$name" "$chartName" "$yamlName" "$4" "$5"
}

install webapigw proxy webapigw y y
install internallb proxy internallb y y
install ingressgw consulgw ingressgw y y
install configs configs configs y y
install webbff webbff webbff y y
install data-volume volume data y n

install monitoring monitoring monitoring y y

echo "> Waiting for rabbitmq pod"
kubectl wait --namespace default --for=condition=ready pod/rabbitmq-0 --timeout=240s