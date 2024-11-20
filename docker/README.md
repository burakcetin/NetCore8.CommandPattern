# Docker Environment for Command Pattern

## Services
- SQL Server (localhost:1433)
  - User: sa
  - Password: YourStrong@Passw0rd
- Redis (localhost:6379)
  - Password: YourStrongRedisPassword
- Redis Commander (http://localhost:8081)
- Prometheus (http://localhost:9090)
- Grafana (http://localhost:3000)
  - User: admin
  - Password: admin

## Usage
1. Start the services:
```bash
docker-compose up -d
```

2. Check service status:
```bash
docker-compose ps
```

3. View logs:
```bash
docker-compose logs -f
```

4. Stop services:
```bash
docker-compose down
```

## Connection Strings
Add these to your appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=CommandPatternDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True",
    "Redis": "localhost:6379,password=YourStrongRedisPassword"
  }
}
```
