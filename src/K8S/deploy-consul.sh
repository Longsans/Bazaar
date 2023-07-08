#!/bin/bash

helm repo add hashicorp https://helm.releases.hashicorp.com;
helm install --values Consul/values.yaml consul hashicorp/consul --create-namespace --namespace consul