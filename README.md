# Guvenior Backend

Guvenior, yeni mezunların ilk düzenli gelir döneminde ortaya çıkan plansız ve duygusal harcama davranışlarını analiz ederek daha sürdürülebilir finansal alışkanlıklar geliştirmesini hedefleyen yapay zeka destekli bir mobil fintech uygulamasıdır.

Bu repository projenin `.NET 8` ile geliştirilmiş backend tarafını içerir. Backend tarafında:

- Kullanıcı kaydı ve giriş işlemleri
- JWT tabanlı kimlik doğrulama
- Gelir ekleme ve listeleme
- Harcama ekleme ve listeleme
- PostgreSQL veritabanı entegrasyonu
- Entity Framework Core migration yapısı

bulunmaktadır.

## Kullanılan Teknolojiler

- `.NET 8`
- `ASP.NET Core Web API`
- `Entity Framework Core`
- `PostgreSQL`
- `ASP.NET Core Identity`
- `JWT Authentication`
- `Swagger`

## Proje Yapısı

- `Güvenior.API`: Controller'lar, `Program.cs`, Swagger ve API başlangıç noktası
- `Güvenior.Application`: DTO'lar, servisler, iş kuralları
- `Güvenior.Domain`: Entity'ler ve enum'lar
- `Güvenior.Infrastructure`: DbContext, repository'ler, migration'lar, JWT servisleri

## Gereksinimler

Projeyi çalıştırmadan önce aşağıdakilerin kurulu olması gerekir:

- `.NET SDK 8.0`
- `PostgreSQL` 15 veya üzeri
- İsteğe bağlı: `pgAdmin`
- İsteğe bağlı: `dotnet-ef`

`dotnet-ef` kurulu değilse:

```bash
dotnet tool install --global dotnet-ef
```

Kuruluysa güncellemek için:

```bash
dotnet tool update --global dotnet-ef
```

## Windows ve macOS İçin Kurulum

### 1. Repository'yi klonlayın

```bash
git clone <repo-url>
cd Guvenior-backend
```

### 2. PostgreSQL veritabanını hazırlayın

PostgreSQL içinde bir veritabanı oluşturun:

```sql
CREATE DATABASE "GuveniorDb";
```

Varsayılan connection string şu anda [appsettings.json](/c:/Guvenior/Guvenior-backend/Güvenior.API/appsettings.json) içinde duruyor:

```json
"DefaultConnection": "Host=localhost;Database=GuveniorDb;Username=postgres;Password="
```

Kendi bilgisayarınızdaki PostgreSQL kullanıcı adı veya şifresi farklıysa bu alanı kendinize göre değiştirin.

Önerilen format:

```json
"DefaultConnection": "Host=localhost;Database=GuveniorDb;Username=postgres;Password=SIFRENIZ"
```

Not:

- Ekip çalışmasında kişisel şifreleri doğrudan repoya commit etmeyin.
- İsterseniz herkes kendi local ayarını `appsettings.Development.json` dosyasında tutabilir.

### 3. Paketleri geri yükleyin

```bash
dotnet restore
```

### 4. Migration'ı veritabanına uygulayın

Bu projede hazır migration bulunmaktadır:

- [20260321113532_InitialCreate.cs](/c:/Guvenior/Guvenior-backend/Güvenior.Infrastructure/Migrations/20260321113532_InitialCreate.cs)

Veritabanını oluşturmak/güncellemek için:

```bash
dotnet ef database update --project Güvenior.Infrastructure --startup-project Güvenior.API
```

Bu komut aşağıdaki tabloları oluşturur:

- `AspNetUsers`
- `AspNetRoles`
- `Incomes`
- `Expenses`
- `Budgets`
- `Insights`

### 5. API'yi çalıştırın

```bash
dotnet run --project Güvenior.API
```

Varsayılan local adresler:

- `http://localhost:5219`
- `https://localhost:7017`

Swagger arayüzü:

- `http://localhost:5219/swagger`
- `https://localhost:7017/swagger`

## macOS Notları

macOS kullanan ekip arkadaşları için:

- `.NET 8 SDK` yüklemek için Microsoft installer veya Homebrew kullanılabilir.
- PostgreSQL yüklemek için genelde `brew install postgresql` tercih edilir.
- PostgreSQL servisini başlatmak için:

```bash
brew services start postgresql
```

- Eğer `postgres` kullanıcısı farklıysa, connection string kendi local kurulumuna göre güncellenmelidir.
- HTTPS sertifikası ile ilgili sorun yaşanırsa ilk deneme için Swagger'ı `http://localhost:5219/swagger` üzerinden açabilirsiniz.

## Uygulamayı İlk Kez Test Etme

### 1. Register

`POST /api/Auth/register`

Örnek body:

```json
{
  "fullName": "Test User",
  "email": "test@example.com",
  "password": "123456",
  "monthlyIncome": 30000,
  "salaryDay": 15
}
```

### 2. Login

`POST /api/Auth/login`

Örnek body:

```json
{
  "email": "test@example.com",
  "password": "123456"
}
```

Login sonrası dönen JWT token'ı alın.

### 3. Authorize

Swagger'da sağ üstteki `Authorize` butonuna tıklayıp şu formatta token girin:

```text
Bearer <JWT_TOKEN>
```

### 4. Gelir ekleme

`POST /api/Income`

Örnek body:

```json
{
  "title": "Mart Maaşı",
  "amount": 30000,
  "receivedDate": "2026-03-20T09:00:00Z",
  "type": 1
}
```

`IncomeType` değerleri:

- `1 = Salary`
- `2 = Freelance`
- `3 = Scholarship`
- `4 = FamilySupport`
- `5 = Other`

### 5. Harcama ekleme

`POST /api/Expense`

Örnek body:

```json
{
  "title": "Akşam yemeği",
  "amount": 450,
  "category": 1,
  "spentAt": "2026-03-20T20:30:00Z"
}
```

`ExpenseCategory` değerleri:

- `1 = Food`
- `2 = Transport`
- `3 = Rent`
- `4 = Shopping`
- `5 = Entertainment`
- `6 = Bills`
- `7 = Education`
- `8 = Other`

### 6. Listeleme endpoint'leri

- `GET /api/Income`
- `GET /api/Expense`

Bu endpoint'ler giriş yapan kullanıcının kendi verilerini döner.

## Yeni Migration Oluşturma

Entity veya DbContext üzerinde değişiklik yapılırsa:

```bash
dotnet ef migrations add MigrationAdi --project Güvenior.Infrastructure --startup-project Güvenior.API
```

Sonrasında veritabanını güncelleyin:

```bash
dotnet ef database update --project Güvenior.Infrastructure --startup-project Güvenior.API
```

Örnek:

```bash
dotnet ef migrations add AddBudgetTable --project Güvenior.Infrastructure --startup-project Güvenior.API
```

## Sık Karşılaşılan Problemler

### `password authentication failed for user`

PostgreSQL kullanıcı adı veya şifresi connection string ile uyuşmuyordur. [appsettings.json](/c:/Guvenior/Guvenior-backend/Güvenior.API/appsettings.json) dosyasını kontrol edin.

### `database "GuveniorDb" does not exist`

Önce veritabanını oluşturun:

```sql
CREATE DATABASE "GuveniorDb";
```

Sonra tekrar:

```bash
dotnet ef database update --project Güvenior.Infrastructure --startup-project Güvenior.API
```

### `No executable found matching command "dotnet-ef"`

Şu komutla aracı kurun:

```bash
dotnet tool install --global dotnet-ef
```

### Swagger açılıyor ama authorize sonrası istekler 401 dönüyor

- Token'ı `Bearer <token>` formatında girdiğinizden emin olun.
- Login'den alınan güncel token'ı kullandığınızdan emin olun.

## Mevcut Durum

Şu anda backend tarafında temel authentication, gelir ve harcama modülleri hazır durumdadır. Sonraki aşamalarda bu yapı üzerine:

- davranışsal analiz metrikleri
- kural tabanlı içgörü üretimi
- destekleyici finansal öneri mesajları
- yaşam planı simülasyonları

eklenebilir.
