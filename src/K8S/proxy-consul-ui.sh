#!/bin/bash

docLink="https://developer.hashicorp.com/consul/tutorials/kubernetes/kubernetes-kind";
printf "Configuring to use consul CLI\n";
printf "For more information please read here %s\n" "$docLink";

export CONSUL_HTTP_TOKEN=$(kubectl get --namespace consul secrets/consul-bootstrap-acl-token --template={{.data.token}} | base64 -d)
export CONSUL_HTTP_ADDR=http://127.0.0.1:8500
export CONSUL_HTTP_SSL_VERIFY=false

printf "UI: http://localhost:8500\n";
printf "Portforwarding, please open another terminal!\nYour token is: \n%s\n" "$CONSUL_HTTP_TOKEN";
kubectl port-forward consul-server-0 --namespace consul 8500:8500