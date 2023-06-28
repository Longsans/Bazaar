#!/bin/bash

workDir=$(dirname -- "$(readlink -f "$0")");
name="$1"
chartName="$2";
valueFile="$3";

input="";
command="template";

echo "Do you want to install helm chart or just check? [y/N]";
read -r input;

if [ -n "$input" ] && [ "${input,,}" != "n" ]; then
    command=install;
fi

helm "$command" "$name" "$workDir/Helm_charts/$chartName" --values "$workDir/Helm_values/$chartName/$valueFile.yaml"