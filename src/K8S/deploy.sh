#!/bin/bash

workDir=$(readlink -f "$0");
workDir=$(dirname -- "$workDir");

chmod +x "$workDir"/create-cluster.sh;
"$workDir"/create-cluster.sh "bazaar" "y" || exit 1;

chmod +x "$workDir"/deploy-consul.sh;
"$workDir"/deploy-consul.sh || exit 1;

chmod +x "$workDir"/helm-install-all.sh;
chmod +x "$workDir"/helm-install.sh;
"$workDir"/helm-install-all.sh || exit 1;