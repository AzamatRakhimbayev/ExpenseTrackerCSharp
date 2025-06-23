# Переходим в корневую директорию проекта
# Этот скрипт предполагает, что вы запускаете его из корневой папки 'my_first_csharp_app'

echo "---"
echo "Шаг 1: Остановка и удаление предыдущих контейнеров и томов..."
docker-compose down --volumes

echo "---"
echo "Шаг 2: Запуск Docker Compose и сборка образов..."
# Используем --build для пересборки образов, если были изменения в Dockerfile
# Используем -d для запуска в фоновом режиме
docker-compose up -d --build

# Добавим небольшую паузу, чтобы дать базе данных время полностью запуститься
echo "---"
echo "Пауза на 10 секунд для запуска базы данных..."
sleep 10

echo "---"
echo "Шаг 3: Применение миграций базы данных..."
# Используем --rm, чтобы временный контейнер ef_cli был удален после выполнения
# Убедитесь, что путь к проекту ExpenseTrackerApi/ExpenseTrackerApi.csproj правильный
docker-compose run --rm ef_cli dotnet ef database update --project ExpenseTrackerApi/ExpenseTrackerApi.csproj

echo "---"
echo "Шаг 4: Проверка статуса контейнеров..."
docker-compose ps

echo "---"
echo "Готово! Ваш Expense Tracker API должен быть доступен по адресу: http://localhost:5024/swagger"
echo "---"