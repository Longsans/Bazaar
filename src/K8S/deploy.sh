#!/bin/bash

workDir=$(readlink -f "$0");
workDir=$(dirname -- "$workDir");

chmod +x "$workDir"/create-cluster.sh;
"$workDir"/create-cluster.sh "bazaar" "y";

chmod +x "$workDir"/deploy-consul.sh;
"$workDir"/deploy-consul.sh

chmod +x "$workDir"/helm-install-all.sh;
"$workDir"/helm-install-all.sh;