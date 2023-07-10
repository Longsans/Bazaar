#!/bin/bash

workDir=$(dirname -- "$(readlink -f "$0")");
input="$1";

clusterName="bazaar";

if [ -z "$input" ]; then
    printf "󰳟  Cluster name [%s]: \n" "$clusterName"
    read -r input;
fi

if [ -n "$input" ]; then
    clusterName="$input";
fi

kind create cluster -n "$clusterName" --config "$workDir"/Kind/cluster.yaml

printf "\n󰳟  Change default cluster to %s? [Y/n]\n" "$clusterName"

input="$2";
if [ -z "$input" ]; then
    read -r input;
fi

if [ -n "$input" ] && [ "${input,,}" != 'y' ]; then
    exit 0;
fi

kubectl cluster-info --context "kind-$clusterName";
# Install ingress
# kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml

# Apply consul api gateway configs
kubectl apply --kustomize "github.com/hashicorp/consul-api-gateway/config/crd?ref=v0.5.1"

# Install dashboard
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml

# echo "> Waiting for the ingress-controller to set up."
# sleep 1m # Don't worry :)) the ingress controller gonna take loger than this to set up anyway
# kubectl wait --namespace ingress-nginx --for=condition=ready pod --selector=app.kubernetes.io/component=controller --timeout=90s