#!/usr/bin/env bash

docker exec -it alfa-bot-prod mongoexport --db FindIT --collection Users --out /data/db/exports/users.json
docker exec -it alfa-bot-prod mongoexport --db FindIT --collection Users --type csv --fields TelegramName,Name,Phone,EMail,University,Profession --out /data/db/exports/users.csv