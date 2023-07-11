#!/bin/bash

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts;
helm repo add grafana https://grafana.github.io/helm-charts;
helm repo add hashicorp https://helm.releases.hashicorp.com;
helm repo update

helm install --values Consul/values.yaml consul hashicorp/consul --create-namespace --namespace consul --version "1.0.2" --wait