# EcommerceAPI (.NET 8)
E-ticaret Web API’si. Modüller: Ürünler, Sepetler, Müşteriler. Teknolojiler: EF Core (SQL Server), FluentValidation, AutoMapper, Swagger.

**Özellikler**
- Katmanlar: API, İş Mantığı, Veri Erişimi, Varlıklar, DTO’lar
- EF Core ile veritabanı yönetimi
- FluentValidation ile doğrulama
- AutoMapper ile DTO eşleme
- Swagger ile test

**Yapı**
- **EcommerceAPI**: Kontrolörler, Swagger
- **Business**: Servisler, doğrulayıcılar
- **DataAccess**: DbContext, depolar
- **Entities**: Ürün, Müşteri, Sepet
- **Core**: DTO’lar, istek modelleri

**Gereksinimler**
- .NET SDK 8.x
- SQL Server LocalDB
- Visual Studio 2022

**Kurulum**
1. EcommerceAPI.sln’yi Visual Studio’da aç, başlangıç projesi yap.
2. **Bağlantı Dizesi** (appsettings.json):
   ```json
   "ECommerceDb": "Server=(localdb)\\MSSQLLocalDB;Database=EcommerceApiDb;Trusted_Connection=True"
   ```
3. Çözümü derle, göçleri uygula:
   - PMC: `Update-Database -StartupProject EcommerceAPI -Project EcommerceAPI.DataAccess`
   - CLI: `dotnet ef database update --startup-project .\EcommerceAPI --project .\EcommerceAPI.DataAccess`
4. Çalıştır (F5), Swagger: https://localhost:<port>/swagger

**Uç Noktalar**
- **Ürünler**: GET/POST/PUT/DELETE /api/products
  ```json
  {"name": "Kulaklık", "description": "Kablolu", "price": 5, "stockQuantity": 12}
  ```
- **Müşteriler**: GET/POST/PUT/DELETE /api/customers
  ```json
  {"firstName": "Sami", "lastName": "Balataci", "email": "sami.balataci@gmail.com", "phone": "5000000000"}
  ```
- **Sepetler**: GET/POST/DELETE /api/carts/{customerId}
  ```json
  {"productId": "<GUID>", "quantity": 2}
  ```

**Validasyon**
- Ürün: İsim (max 200), Fiyat/Stok >= 0
- Müşteri: Ad/Soyad (max 100), E-posta geçerli
- Sepet: Miktar > 0

**Veri Notları**
- Ürün Durumu: string
- Müşteri E-posta: benzersiz
- Sepet: (CartId, ProductId) benzersiz

**EF Komutları**
- Göç ekle: `Add-Migration <Ad> -StartupProject EcommerceAPI -Project EcommerceAPI.DataAccess`
- Güncelle: `Update-Database -StartupProject EcommerceAPI -Project EcommerceAPI.DataAccess`

**Sorun Giderme**
- EF Tools: Microsoft.EntityFrameworkCore.Tools yükle
- Swagger: Swashbuckle.AspNetCore kontrol et
