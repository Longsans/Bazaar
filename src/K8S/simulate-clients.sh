#!/bin/bash

port=6969;
numberOfOrder=1;

function createOrder() {
    local type;
    type=$( shuf -i 0-3 -n 1 );

    curl --header "Content-Type: application/json" --insecure --request POST --data @Clients/post-data-"${type}".json https://localhost:${port}/bff/api/orders
    (( numberOfOrder++ ))
}

function queryOrders() {
    curl --header "Content-Type: application/json" --insecure --request GET https://localhost:${port}/o/api/orders
}

function queryOrder() {
    local id;
    id=$( shuf -i 1-$numberOfOrder -n 1 );
    _=$( curl --header "Content-Type: application/json" --insecure --request GET https://localhost:${port}/o/api/order/"$id" )
}

function queryCatalog() {
    local type;
    type=$( shuf -i 0-3 -n 1 );
    _=$( curl --header "Content-Type: application/json" --insecure --request GET https://localhost:${port}/ca/api/catalog/"${type}" )
}

orderCount=0;
allOrderCount=0;
createCount=0;
catalogCount=0;

function printInfo() {
    clear;
    echo "=Report="
    printf "Order req:   %s\n" "$orderCount";
    printf "Orders req:  %s\n" "$allOrderCount";
    printf "Create req:  %s\n" "$createCount";
    printf "Catalog req: %s\n" "$catalogCount";
    printf "Current req: %s\n" "$1";
}

while :; do
    type=$( shuf -i 0-100 -n 1 );
    delay=$( shuf -i 200-1500 -n 1 );
    req="";    

    if (( type < 50 )); then
        queryOrder;
        req="order"
        (( orderCount++ ))
    elif (( type < 75 )); then
        queryCatalog;
        req="catalog"
        (( catalogCount++ ))
    elif (( type < 90 )); then
        createOrder;
        req="create"
        (( createCount++ ))
    else
        queryOrders;
        req="all orders"
        (( allOrderCount++ ))
    fi

    printInfo "$req"
    sleep $(( delay / 1000 ));
done