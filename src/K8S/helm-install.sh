#!/bin/bash

workDir=$(dirname -- "$(readlink -f "$0")");
name="$1"
chartName="$2";
valueFile="$3";

if [ -z "$name" ]; then
    read -p "Please enter helm installation's name: " -r name;
fi

if [ -z "$chartName" ]; then
    read -p "Please enter the helm chart's name: " -r chartName;
fi

if [ -z "$valueFile" ]; then
    read -p "Please enter the value file's name: " -r valueFile;
fi

input="";
command="template";

echo "Do you want to install helm chart or just check? [y/N]";
read -r input;

if [ -n "$input" ] && [ "${input,,}" != "n" ]; then
    command=install;
fi

helm "$command" "$name" "$workDir/HelmCharts/$chartName" --values "$workDir/HelmValues/$chartName/$valueFile.yaml";