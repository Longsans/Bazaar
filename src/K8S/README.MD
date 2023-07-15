## Install process:
 - chmod +x ./deploy.sh
 - run ./deploy.sh
 - done!!
 - not really, you'll have to wait around 10 -> 30 minutes, just ignore the error messages.

## To log into Dockerhub:
 - I'm not sure if this is needed before install process so just login just to be sure.
 - docker login -u cornycodingcorn
 - when it ask for password: dckr_pat_aJqPI0DHppyVLRSau0GHeXkmxVE

## To update images (need to be logged into dockerhub):
 - ./compose.sh - will run docker-compose build
 - ./push.sh - will push all images what contain bazaar in it

## To redeploy:
 - ./helm-install <install name> <chart name> <values file name>
 - ./helm-install-serivces.sh - Redeploy all services
 - ./helm-install-all.sh - Redeploy everything

## To use dashboard:
 - kubectl proxy
 - http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/login
 - ./get-dashboard-pass.sh admin
 - login

## To use consul dashboard:
 - ./proxy-consul-tls.sh
 - Copy paste the token.

## To use grafana
 - ./port-forward-grafana.sh
 - user: admin
 - pass: password
 - dashboard json file: src/K8S/Grafana/bazaar-dashboard.json

## To simulate clients
 - ./simulate-clients.sh

GET ME OUTTA HERE!!!!!!!! SEND HELP!!