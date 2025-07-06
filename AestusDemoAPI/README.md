# FinTech Aestus API
Demo .NET Minimal API for processing financial transactions with built-in anomaly detection logic. Designed to handle up to 1000 transactions per second per user, with scalable architecture and batch-based persistence. (Based per Aestus Code assignement)

## 🔧 Features

- Minimal REST API with:
  - `POST /transactions` – Receive and queue transactions
  - `GET /users/{id}/anomalies` – Fetch suspicious transactions
  - `GET /dashboard` – Fetch dashboard data
- Custom anomaly detection using statistical and heuristic rules
- Asynchronous, high-throughput transaction handling
- Batch saving using a background worker service
- Entity Framework Core integration with SQL database
- Transaction persistence using `Infrastructure/Migrations` folder
- Easily extensible and container-ready

---

## 🚀 Getting Started

Follow these steps to set up the project on your local machine.

---

### 1️⃣ Clone the Repository

Download the code to your local machine:

```bash
git clone https://github.com/yourusername/fintech-aestus-api.git
cd fintech-aestus-api
```

### 2️⃣  Set Up the Database Connection String

The project uses Entity Framework Core to connect to a SQL database. You'll need to update the connection string to point to your local SQL Server instance.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AestusDb;Trusted_Connection=True;"
  }
}
```

### 3️⃣  Apply the Migration Locally

This will create the database and schema based on app model (Transaction.cs)

```bash
dotnet ef database update
```
or in Package Manager Console

```bash
Update-Database
```

### 4️⃣  Run the Application

App will run on

http://localhost:5019

https://localhost:7250
