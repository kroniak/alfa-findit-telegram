#!/usr/bin/env bash

docker exec -it alfa-findit-mongodb-prod mongoexport --db FindIT --collection Users --out /data/db/exports/students.json
docker exec -it alfa-findit-mongodb-prod mongoexport --db FindIT --collection Users --type csv --fields TelegramName,Name,Phone,EMail,University,Profession --out /data/db/exports/students.csv