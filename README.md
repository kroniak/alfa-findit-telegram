# alfa-findit-telegram

Бот для сбора обратной связи на выставке НайтиИТ и подобных

## Требования к хосту

Бот запускается на любом хосте, поддерживающим контейнерезацию через `docker`.
Настройка баз данных и SDK не требуется.
Бот был протестирован на VM `ubuntu16.04LTS x64, docker 17, 2VCPU, 2GRAM`.
Пользователь должен иметь права для запуска котейнеров через `sudo` или через группу `docker`.

## Установка

Перед запуском бота необходимо отредактировать файл `docker-compose.yml` и добавить в него ваши секретные ключи.

bash `
cd ~
git clone https://github.com/kroniak/alfa-findit-telegram.git
cd alfa-findit-telegram
./build/build.sh
nano docker-compose.yml
docker-compose up -d
`