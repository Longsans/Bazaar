#!/bin/bash

absolutePath=$(readlink -f "$0");
cd "$(dirname -- "$absolutePath")" || exit
docker-compose build