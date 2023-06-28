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

printf "\n󰳟  Change default cluster to %s? [Y/n]\n" "$clusterName"

input="";
read -r input;
if [ -n "$input" ] && [ "${input,,}" != 'y' ]; then
    exit 0;
fi

kubectl cluster-info --context "$clusterName";