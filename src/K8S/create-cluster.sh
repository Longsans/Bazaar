#!/bin/bash

workDir=$(dirname -- "$(readlink -f "$0")");
input="";

clusterName="bazaar";
printf "󰳟  Cluster name [%s]: \n" "$clusterName"
read -r input;

if [ -n "$input" ]; then
    clusterName="$input";
fi

kind create cluster -n "$clusterName" --config "$workDir"/Kind/cluster.yaml
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
kubectl wait --namespace ingress-nginx \
  --for=condition=ready pod \
  --selector=app.kubernetes.io/component=controller \
  --timeout=90s

printf "\n󰳟  Change default cluster to %s? [Y/n]\n" "$clusterName"

input="";
read -r input;
if [ -n "$input" ] && [ "${input,,}" != 'y' ]; then
    exit 0;
fi

kubectl cluster-info --context "kind-$clusterName";