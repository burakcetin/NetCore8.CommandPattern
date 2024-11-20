# Command Pattern Demo Projesi

## Proje Hakkında
Bu proje, Command Pattern'in pratik bir uygulamasını göstermek amacıyla geliştirilmiş bir e-ticaret senaryosu üzerine kurulmuştur. Modern yazılım mimarisi prensiplerini ve enterprise-level uygulama geliştirme pratiklerini içermektedir.

## Temel Özellikler
- 🎯 Command Pattern implementasyonu
- 📊 Prometheus metrik izleme
- 📈 Grafana dashboard'ları
- 🚀 Redis cache entegrasyonu
- 🔐 JWT tabanlı kimlik doğrulama
- 📝 Detaylı audit logging
- ♻️ Transaction yönetimi
- 🔄 Retry mekanizması
- ⚡ Rate limiting

## Teknik Altyapı
- .NET 8
- Entity Framework Core
- Redis
- SQL Server
- Prometheus
- Grafana
- Docker

## Proje Yapısı
```
NetCore8.CommandPattern/
├── src/
│   ├── NetCore8.CommandPattern.Core/           # Domain entities, interfaces
│   ├── NetCore8.CommandPattern.Infrastructure/ # Implementasyonlar, veritabanı
│   └── NetCore8.CommandPattern.Application/    # API endpoints, komutlar
└── docker/
    ├── docker-compose.yml
    ├── prometheus.yml
    └── grafana/
```

## Kurulum

### Ön Gereksinimler
- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 veya JetBrains Rider

### Geliştirme Ortamını Hazırlama
1. Repoyu klonlayın:
```bash
git clone https://github.com/burakcetin/NetCore8.CommandPattern.git
```

2. Docker container'ları başlatın:
```bash
cd NetCore8.CommandPattern/docker
docker-compose up -d
```

3. Projeyi derleyin:
```bash
dotnet build
```

4. Migration'ları uygulayın:
```bash
cd src/NetCore8.CommandPattern.Application
dotnet ef database update
```

## Servisler ve Portlar
- Web API: http://localhost:5000
- SQL Server: localhost,1433
- Redis: localhost:6379
- Redis Commander: http://localhost:8081
- Prometheus: http://localhost:9090
- Grafana: http://localhost:3000

## Kullanım Senaryoları

### 1. Sipariş Oluşturma
```csharp
var command = new CreateOrderCommand(userId, items);
var result = await _commandHandler.HandleAsync(command);
```

### 2. Sipariş Listeleme (Cache'li)
```csharp
var command = new ListOrdersCommand(page: 1, pageSize: 10);
var result = await _commandHandler.HandleAsync(command);
```

### 3. Ödeme İşlemi (Retry Mekanizmalı)
```csharp
var command = new ProcessPaymentCommand(orderId, amount);
var result = await _commandHandler.HandleAsync(command);
```

## Metrikler ve İzleme

### Temel Metrikler
- Command çalıştırma sayısı
- Başarılı/başarısız komut sayısı
- Komut çalışma süreleri
- Cache hit/miss oranları
- Hata oranları

### Grafana Dashboard'ları
- Command Performance Dashboard
- Error Tracking Dashboard
- Cache Performance Dashboard

## Mimari Özellikler

### Command Pattern İmplementasyonu
- Merkezi command handler
- Transaction yönetimi
- Validation
- Audit logging
- Cache desteği

### Cross-Cutting Concerns
- Exception handling
- Logging
- Caching
- Authentication
- Authorization
- Validation
- Transaction management

## Örnek Kullanım Senaryoları

### E-ticaret Senaryosu
1. Kullanıcı siparişi oluşturur (CreateOrderCommand)
2. Sistem stok kontrolü yapar
3. Ödeme işlemi gerçekleşir (ProcessPaymentCommand)
4. Sipariş durumu güncellenir (UpdateOrderCommand)
5. Kullanıcıya bildirim gönderilir

### Cache Kullanımı
```csharp
public class GetOrderByIdCommand : ICacheableCommand
{
    public string CacheKey => $"order_{OrderId}";
    public TimeSpan CacheDuration => TimeSpan.FromMinutes(10);
}
```

### Retry Mekanizması
```csharp
public class ProcessPaymentCommand : IRetryableCommand
{
    public int MaxRetries => 3;
    public TimeSpan RetryDelay => TimeSpan.FromSeconds(1);
}
```
 

## Lisans
Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakınız.

## İletişim
https://github.com/burakcetin

## Teşekkürler
Bu projeye katkıda bulunan herkese teşekkürler! 🚀