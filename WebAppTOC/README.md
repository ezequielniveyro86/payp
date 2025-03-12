# WebAppTOC - API de Administración de Billeteras

WebAppTOC es una API RESTful desarrollada con ASP.NET Core Minimal API que permite administrar billeteras digitales y realizar transacciones (depósitos y retiros).

## 📋 Características

- Creación y administración de billeteras
- Registro de transacciones (depósitos y retiros)
- Consulta de saldos y movimientos
- API RESTful con documentación Swagger
- Persistencia de datos con Entity Framework Core y SQLite

## 🔧 Tecnologías utilizadas

- .NET 9.0
- ASP.NET Core Minimal API
- Entity Framework Core 9.0
- SQLite
- Swagger / OpenAPI

## ⚙️ Requisitos previos

- .NET 9.0 SDK o superior
- Herramientas de Entity Framework Core (`dotnet tool install --global dotnet-ef`)

## 🚀 Instalación

1. Clona el repositorio:
   ```
   git clone https://github.com/ezequielniveyro86/WebAppTOC.git
   cd WebAppTOC
   ```

2. Restaura los paquetes NuGet:
   ```
   dotnet restore
   ```

3. Crea la base de datos SQLite:
   ```
   dotnet ef database update
   ```

4. Ejecuta la aplicación:
   ```
   dotnet run
   ```

La API estará disponible en:
- https://localhost:7013/swagger (HTTPS)
- http://localhost:5104/swagger (HTTP)

## 📊 Estructura de la base de datos

La aplicación utiliza dos tablas principales:

### Tabla Wallets
- `Id`: Identificador único (autoincrementable)
- `DocumentId`: Identificador de documento
- `Name`: Nombre de la billetera
- `Balance`: Saldo actual
- `CreatedAt`: Fecha de creación
- `UpdatedAt`: Fecha de última actualización

### Tabla Transactions
- `Id`: Identificador único (autoincrementable)
- `WalletId`: ID de la billetera asociada
- `Amount`: Monto de la transacción
- `Type`: Tipo de transacción (DEPOSIT/WITHDRAW)
- `CreatedAt`: Fecha de la transacción

## 🔍 Uso de la API

### Endpoints disponibles

#### Billeteras

- **GET /wallets**: Obtiene todas las billeteras
- **GET /wallets/{id}**: Obtiene una billetera específica por su ID
- **POST /wallets**: Crea una nueva billetera
- **PUT /wallets/{id}**: Actualiza el nombre de una billetera existente

#### Transacciones

- **GET /transactions**: Obtiene todas las transacciones
- **GET /wallets/{walletId}/transactions**: Obtiene todas las transacciones de una billetera específica
- **POST /wallets/{walletId}/transactions**: Registra una nueva transacción para una billetera

### Ejemplos de uso

#### Crear una billetera

```http
POST /wallets
Content-Type: application/json

{
  "name": "Mi Billetera Principal",
  "documentId": 12345
}
```

#### Realizar un depósito

```http
POST /wallets/1/transactions
Content-Type: application/json

{
  "amount": 100.50,
  "type": "DEPOSIT"
}
```

#### Realizar un retiro

```http
POST /wallets/1/transactions
Content-Type: application/json

{
  "amount": 50.25,
  "type": "WITHDRAW"
}
```

## 🧪 Pruebas

Para ejecutar las pruebas del proyecto (si están disponibles):

```
dotnet test
```

## 💻 Desarrollo

### Migración de base de datos

Para crear una nueva migración después de cambiar los modelos:

```
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Generar script SQL

Para generar un script SQL a partir de las migraciones:

```
dotnet ef migrations script -o script.sql
```

## 📖 Documentación

La API incluye documentación interactiva mediante Swagger/OpenAPI, accesible en:

- `/swagger` cuando la aplicación está en ejecución

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - consulta el archivo LICENSE para más detalles.

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor, abre un Issue o Pull Request para sugerencias o mejoras. 