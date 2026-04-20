# Nimble.Modulith

**Nimble.Modulith** — це [демонстраційний лабораторний проект](https://github.com/NimblePros/ModularMonolithWorkshop), що ілюструє побудову архітектури **Modular Monolith** на платформі .NET 10.

Проект розділений на ізольовані, слабо пов'язані модулі, кожен з яких відповідає за певну бізнес-доменну область, має власну базу даних (або схему) та комунікує з іншими частинами системи виключно через чітко визначені контракти та події.

---

## Стек технологій та патернів

- **Framework:** .NET 10
- **Orchestration:** [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
- **API Architecture:** [FastEndpoints](https://fast-endpoints.com//) (реалізація патерну REPR — Request-Endpoint-Response)
- **Міжмодульна комунікація:**
  - **Синхронна:** `Mediator` (Source Generated) для внутрішніх запитів (наприклад, отримання ціни з модуля Products).
  - **Асинхронна (Event-Driven):** Domain/Integration Events для публікації подій (наприклад, сповіщення про створення замовлення).
- **Робота з даними:**
  - **Entity Framework Core:** з підтримкою Resilience/Retry політик.
  - **Dapper:** для високоефективних аналітичних запитів.
  - **MS SQL Server:** як база даних.

---

## Структура модулів

Система складається з п'яти незалежних бізнес-модулів:

1.  **Users Module:** Керування користувачами, JWT-автентифікація, автоматична генерація тимчасових паролів та їх скидання.
2.  **Products Module:** Каталог товарів, управління номенклатурою та ціноутворенням.
3.  **Customers Module:** Профілі клієнтів та повний життєвий цикл замовлень (від створення до підтвердження).
4.  **Email Module:** Фонова асинхронна відправка транзакційних листів (Welcome-листи, чеки, відновлення пароля) через `Channel<T>` та `BackgroundService`.
5.  **Reporting Module:** Модуль аналітики, що будується на основі подій. Використовує Star Schema та підтримує експорт звітів у форматах JSON та CSV.

---

## Архітектурні рішення

- **Сувора інкапсуляція (Implementation vs Contracts):** Кожен модуль фізично розділений на два проекти. `Module.Contracts` містить лише DTO, команди, події та інтерфейси — це єдине, що бачать інші модулі. Основний проект `Module` містить бізнес-логіку, доступ до БД та ендпоінти, залишаючись закритим для зовнішнього світу.
- **Ізоляція даних:** Кожен модуль має власний `DbContext` та ізольовану схему в базі даних (`Users`, `Products`, `Customers`, `Reporting`). Жодних прямих `JOIN` між таблицями різних модулів не допускається.
- **Асинхронна обробка (Producer-Consumer):** Модуль Email не блокує HTTP-запити. Листи потрапляють у In-Memory чергу і обробляються фоновим воркером, забезпечуючи високу швидкість відповіді API.
- **Reporting Separation (CQRS/OLAP):** Модуль звітів не навантажує транзакційні бази. Він слухає подію `OrderCreatedEvent` і будує власну базу фактів (`FactOrders`) та вимірів (`DimDate`, `DimCustomer`, `DimProduct`), що дозволяє виконувати складні агрегації миттєво.

---

## Архітектурні діаграми

_Нижче наведені схеми архітектури на основі моделі C4:_

### System Context

![System Context](./docs/diagrams/context.png))

### Container

![Container](./docs/diagrams/container.png)

### Component

![Component](./docs/diagrams/component.png)

---

## Інструкція з розгортання

### Попередні вимоги

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (запущений)

### Запуск проекту

1.  Клонуйте репозиторій:
    ```bash
    git clone https://github.com/TeseySTD/practicum-2-4-Enterprise-Arch-
    cd practicum-2-4-Enterprise-Arch
    ```
2.  Виконайте збірку рішення:
    ```bash
    dotnet build
    ```
3.  Запустіть проект через Aspire AppHost:
    ```bash
    dotnet run --project Nimble.Modulith.AppHost/Nimble.Modulith.AppHost.csproj
    ```
4.  У консолі з'явиться посилання на **Aspire Dashboard** (зазвичай `https://localhost:17188`). Там ви знайдете посилання на:
    - **webapi (Endpoint):** Swagger UI для тестування API.
    - **papercut (Endpoint):** Локальний веб-інтерфейс (порт `37408`) для перегляду всіх відправлених системою email-листів.

---
