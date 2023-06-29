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

input="$4";
command="template";

if [ -z "$input" ]; then
    echo "Do you want to install helm chart or just check? [y/N]";
    read -r input;
fi

if [ -n "$input" ] && [ "${input,,}" != "n" ]; then
    command=install;
    existingInstall=$(helm list | grep "$name");
    if [ -n "$existingInstall" ]; then
        delete="$5";

        if [ -z "$delete" ]; then
            read -r -p "There is an installation with the same name do you want to delete it? [y/N]: " delete;
        fi

        if [ -n "$delete" ] && [ "${delete,,}" != "n" ]; then
            helm uninstall "$name";
        fi
    fi
fi

installPath="$workDir/HelmCharts/$chartName"
if [ -f "$installPath.yaml" ]; then
    installPath=$(<"$installPath.yaml");
fi
helm "$command" "$name" "$installPath" --values "$workDir/HelmValues/$chartName/$valueFile.yaml";