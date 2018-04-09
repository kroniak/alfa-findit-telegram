#!/usr/bin/env bash

docker exec -it alfa-findit-mongodb-prod mongoexport --db FindIT --collection Students --out /data/db/exports/students.json
docker exec -it alfa-findit-mongodb-prod mongoexport --db FindIT --collection Students --type csv --fields TelegramName,Name,Phone,EMail,University,Profession --out /data/db/exports/students.csv