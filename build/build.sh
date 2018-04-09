#!/usr/bin/env bash

docker build -t alfabank/findit-telegram:1.0 .
docker images -f 'dangling=true' -q