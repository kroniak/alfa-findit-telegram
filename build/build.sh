#!/usr/bin/env bash

docker build -t alfabank/findit-telegram:1.2 .
docker rmi $(docker images -f 'dangling=true' -q)