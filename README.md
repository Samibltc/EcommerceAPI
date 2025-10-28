# EcommerceAPI (.NET 8)

Katmanlı, modüler bir e‑ticaret API’si. Özellikler: Ürünler, Müşteriler, Sepet; kimlik yönetimi (ASP.NET Core Identity + JWT), FluentValidation, AutoMapper, EF Core (SQL Server), Swagger.

## Katmanlar (Mimari)

- EcommerceAPI (API)
  - Controller’lar, global `ValidationFilter`, JWT auth, Swagger, middleware (ProblemDetails).
- EcommerceAPI.Business (Business)
  - Servisler: `IProductService`, `ICustomerService`, `ICartService`, `IAccountService`
  - Validasyon: FluentValidation
  - Eşlemeler: AutoMapper Profilleri
- EcommerceAPI.DataAccess (Veri Erişimi)
  - `ECommerceDbContext` (IdentityDbContext), EF Core konfigurasyonları
  - Repository’ler (EF), Identity (AspNetUsers)
- EcommerceAPI.Entities (Domain)
  - `Product`, `Customer`, `Cart`, `CartItem`, `ProductStatus`, `BaseEntity`
- EcommerceAPI.Core (Contracts)
  - DTO’lar (Product/Customer/Cart/Account)
  - Abstraction’lar: `IEmailSender`, `ITokenService`

## Kullanılan NuGet Paketleri (katmanlara göre)

- API
  - Microsoft.EntityFrameworkCore.Design (9.0.10)
  - FluentValidation.DependencyInjectionExtensions (12.x)
  - Swashbuckle.AspNetCore (6.6.2)
  - Microsoft.AspNetCore.Authentication.JwtBearer (8.0.21)

- Business
  - AutoMapper (12.0.1)
  - AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
  - FluentValidation (12.0.0)
  - Microsoft.Extensions.Configuration.Abstractions (8.0.0)
  - FrameworkReference: Microsoft.AspNetCore.App (Identity türleri için)

- DataAccess
  - Microsoft.EntityFrameworkCore (9.0.10)
  - Microsoft.EntityFrameworkCore.SqlServer (9.0.10)
  - Microsoft.EntityFrameworkCore.Tools (9.0.10)
  - Microsoft.EntityFrameworkCore.Design (9.0.10)
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.21)


## Kurulum ve Çalıştırma

1) Repoyu klonla
```
git clone https://github.com/Samibltc/EcommerceAPI.git
cd EcommerceAPI  
```
2) Restore + DB şema kurulumu
```
dotnet restore
dotnet ef database update --startup-project .\EcommerceAPI --project .\EcommerceAPI.DataAccess
```
3) Çalıştır
```
dotnet watch run --project .\EcommerceAPI
```
4)- Swagger: https://localhost:<port>/swagger

Not: Bu repo seed data içermez. Boş şema ile başlar.

## Konfigürasyon

- `EcommerceAPI/appsettings.json`
  - ConnectionStrings:ECommerceDb (LocalDB varsayılan)
  - Jwt: Issuer, Audience, Key, TokenLifetimeMinutes
- Geliştirme için `appsettings.Development.json` kullanabilirsiniz (gitignore’da gizli).

## Swagger ile Akış (Auth ve CRUD)

1) Kayıt (Account)
- POST `/api/accounts/register`
- Dönüş: `userId` (Identity) ve `customerId` (Domain) içerir.
- UserId ≠ CustomerId
2) E‑posta Onayı
- GET `/api/accounts/confirm-email?userId=<UserId>&token=<Token>`
- Token loglarda “EmailConfirm URL:” satırında plain-text olarak da yazılır.
- HTML logundan kopyalarken `&amp;` → `&` çevirin.

3) Giriş (JWT Al)
- POST `/api/accounts/login`
- Dönüş: `accessToken`, `expiresAtUtc`
- Swagger’da sağ üst “Authorize” > Bearer şemasına SADECE “raw token” yapıştırın (önüne “Bearer ” yazmayın).


4) Kendi Hesabını Sil (Authorize zorunlu)
- DELETE `/api/accounts/me`
- Token’daki kullanıcı (JWT içindeki kimlik) silinir.

5) Kullanıcıyı Id ile Sil (Authorize zorunlu)
- DELETE `/api/accounts/{userId}`
- Şu anda herhangi bir authenticated kullanıcı bu uç noktayı çağırabilir.

6) Ürün ve Müşteri Uçları
- Products: GET/POST/PUT/DELETE `/api/products`
- Customers: GET/GET by id/PUT `/api/customers`

## Token, UserId ve CustomerId

- Token Nereden Gelir?
  - `POST /api/accounts/login` çağrısı ile `TokenService` bir JWT üretir. Claim’ler:
    - `sub`: Identity UserId (AspNetUsers.Id)
    - `email`: kullanıcı e‑postası
    - `cid`: CustomerId (Customers.Id)
    - Uyumluluk için `ClaimTypes.NameIdentifier` (sub ile aynı) da eklenir.
- Neden UserId ≠ CustomerId?
  - `UserId`: kimlik/doğrulama katmanının anahtarı (Identity, AspNetUsers).
  - `CustomerId`: domain müşteri profilinin anahtarı (Customers).
  - `AppUser.CustomerId` ile 1:1 bağlanır. Kimlik verisini domain profilinden ayırmak esnekliği ve güvenliği artırır.
