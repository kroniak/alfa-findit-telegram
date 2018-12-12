# alfa-findit-telegram

Бот для сбора обратной связи на выставке НайтиИТ и подобных

## Требования к хосту

Бот запускается на любом хосте, поддерживающим контейнерезацию через `docker`.

Настройка баз данных и SDK не требуется.

Бот был протестирован на VM `ubuntu16.04LTS x64, docker 17, 2VCPU, 2GRAM`.

Пользователь должен иметь права для запуска котейнеров через `sudo` или через группу `docker`.

## Установка

Перед запуском бота необходимо отредактировать файл `docker-compose.yml` и добавить в него ваши секретные ключи.

```bash
cd ~
git clone https://github.com/kroniak/alfa-findit-telegram.git
cd alfa-findit-telegram
./build/build.sh
nano docker-compose.yml
docker-compose up -d
```

## Управление ботом

Управление может осуществляться дистанционно по урлу вида: `http://example.org:5000/botcontrol/start?secretKey=KEY`
Поддерживаются endpoint: `start`, `stop`, `ping`

## Выгрузка данных

Для выгрузки данных в `json\csv` используйте скрипт `scripts/export.sh`. Он выгрузит данные в двух форматах в папку `~/alfa-findit-telegram-data/exports/`

Также можно сделать выгрузку через endpoint:

`http://example.org:5000/export/json?secretKey=KEY`

`http://example.org:5000/export/csv?secretKey=KEY`

## Changes

### 1.2

- Обновлена версия dotnet core до 2.2
- Обновлена сборка проекта до images dotnet core 2.2

### 1.1

- Обновлена версия dotnet core до 2.1
- Обновлена сборка проекта до docker-compose 3 и images dotnet core 2.1
