# alfa-findit-telegram

Бот для сбора обратной связи на выставке НайтиИТ и подобных

## Требования к хосту

Бот запускается на любом хосте, поддерживающим Docker.

Настройка баз данных и SDK не требуется.

Бот был протестирован на VM `ubuntu16.04LTS x64, docker 17, 2VCPU, 2GRAM`.

Пользователь должен иметь права для запуска контейнеров через `sudo` или через группу `docker`.

## Установка

Перед запуском бота необходимо отредактировать файл `docker-compose.yml` и добавить в него ваши секретные ключи.

```bash
cd ~
git clone https://github.com/kroniak/alfa-findit-telegram.git
cd alfa-findit-telegram
./build/build.docker.sh
nano docker-compose.yml
docker-compose up -d
```

## Управление ботом

Управление может осуществляться дистанционно по урлу вида: `http://example.org/api/bot/start?secretKey=KEY`
Поддерживаются endpoint: `start`, `stop`, `ping`

Доступен Swagger по адресу `http://example.org/swagger`

Доступны HealthChecks по адресу `http://example.org/health`

## Выгрузка данных

Для выгрузки данных в `json\csv` используйте скрипт `scripts/export.sh`. Он выгрузит данные в двух форматах в папку `~/alfa-findit-telegram-data/exports/`

Также можно сделать выгрузку через endpoint:

`http://example.org/api/export/json?secretKey=KEY`

`http://example.org/api/export/csv?secretKey=KEY`

## Changes

### 2.0

- Произведен глобальный рефакторинг приложения
- Добавлены Swagger, HealthChecks
- Добавлена работа с очередями и ограничения TelegramApi
- Добавлены endpoints для добавления сообщений и логгирования

### 1.2

- Обновлена версия dotnet core до 2.2
- Обновлена сборка проекта до images dotnet core 2.2

### 1.1

- Обновлена версия dotnet core до 2.1
- Обновлена сборка проекта до docker-compose 3 и images dotnet core 2.1
