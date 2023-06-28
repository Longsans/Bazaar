#!/bin/bash

printf "Delete all image contain [bazaar-]: "
read -r str
if [ -z "$str" ]; then
    str=bazaar-;
fi

docker rmi -f $(docker images "*$str*" -q)