# Bazaar
A high-scalability and high-availability distributed e-commerce application built with Microservices architecture. Named after one of the oldest and largest markets in the world.
This demo is deployed using Kubernetes and monitored with prometheus. All the internal communication has been encrypted with consul TLS using envoy proxy.
For data visualization we use Grafana.

# Deploy instruction
 - open terminal at src/K8S
 - chmod +x ./deploy.sh
 - ./deploy.sh
 - done!! - now wait for 10 - 30 minutes

[Full instruction](src/K8S/README.MD)
