# 🚀 Supabase Kurulum Rehberi

Bu proje artık **Supabase** kullanıyor. Docker ve local PostgreSQL yerine hem development hem production için Supabase ücretsiz tier'ını kullanıyoruz.

---

## 📊 Supabase Ücretsiz Tier (Free Plan)

✅ **5000+ kullanıcıyı destekler!**

- 500MB veritabanı
- Sınırsız API çağrıları
- 50,000 aylık aktif kullanıcı (authentication)
- Otomatik backup (7 günlük)
- SSL/TLS şifreleme
- 2GB dosya depolama
- 1GB bandwidth

**Maliyet:** Tamamen ücretsiz! Kredi kartı gerekmez.

---

## 🎯 Adım 1: Supabase Projesi Oluşturma

1. [Supabase](https://supabase.com) sitesine git
2. **Start your project** butonuna tıkla
3. GitHub ile giriş yap
4. **New Project** oluştur:
   - **Organization:** Kendi organizasyonunu seç veya yeni oluştur
   - **Name:** `turlagitsin-api` (veya istediğin isim)
   - **Database Password:** Güçlü bir şifre belirle (**bunu kaydet!**)
   - **Region:** `Frankfurt (eu-central-1)` (Render ile aynı)
   - **Pricing Plan:** Free (varsayılan)
5. **Create new project** butonuna tıkla
6. 1-2 dakika bekle, proje hazırlanıyor...

---

## 🔑 Adım 2: Connection String'i Alma

Proje hazır olduğunda:

1. Sol menüden **Settings** > **Database** bölümüne git
2. **Connection String** sekmesine tıkla
3. **URI** formatını seç (PostgreSQL URI)
4. Connection string şu formatta olacak:

```
postgresql://postgres.[PROJECT-REF]:[YOUR-PASSWORD]@aws-0-eu-central-1.pooler.supabase.com:5432/postgres
```

5. `[YOUR-PASSWORD]` kısmını, proje oluştururken belirlediğin şifre ile değiştir

**Örnek:**
```
postgresql://postgres.abcdefghijk:MySecurePassword123!@aws-0-eu-central-1.pooler.supabase.com:5432/postgres
```

---

## 💻 Adım 3: Development Ortamı (Yerel Bilgisayar)

### Yöntem 1: Environment Variable (Önerilen)

Windows PowerShell'de:
```powershell
$env:DATABASE_URL = "postgresql://postgres.abcdefghijk:MySecurePassword123!@aws-0-eu-central-1.pooler.supabase.com:5432/postgres"
```

Kalıcı olarak ayarlamak için (Windows):
```powershell
[System.Environment]::SetEnvironmentVariable('DATABASE_URL', 'postgresql://postgres.abcdefghijk:MySecurePassword123!@...', 'User')
```

### Yöntem 2: appsettings.Development.json

`src/Api/appsettings.Development.json` dosyasını düzenle:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "postgresql://postgres.abcdefghijk:MySecurePassword123!@aws-0-eu-central-1.pooler.supabase.com:5432/postgres"
  }
}
```

**⚠️ Dikkat:** Bu dosyayı Git'e commit etme! `.gitignore` dosyasına eklenmiş olmalı.

---

## 🌐 Adım 4: Production Ortamı (Render)

1. [Render Dashboard](https://dashboard.render.com) a git
2. `bitur-app` servisini seç
3. **Environment** sekmesine tıkla
4. **Add Environment Variable** butonuna tıkla:
   - **Key:** `DATABASE_URL`
   - **Value:** Supabase connection string'ini yapıştır
5. **Save Changes** butonuna tıkla
6. Render otomatik olarak yeniden deploy edecek

---

## ✅ Adım 5: Migration ve Test

### Migration çalıştırma

Proje otomatik olarak migration yapıyor, ancak manuel çalıştırmak isterseniz:

```powershell
cd src/Api
dotnet ef database update
```

### Uygulamayı çalıştırma

```powershell
cd src/Api
dotnet run
```

Çıktıda şunu görmelisiniz:
```
🚀 Starting application in Development environment
✅ Successfully connected to Supabase: aws-0-eu-central-1.pooler.supabase.com
🔄 Starting database migration...
✅ Database migration completed successfully
```

### Test endpoint'i

Tarayıcıdan veya Postman'den:
```
http://localhost:5000/health
```

Cevap:
```json
{
  "status": "healthy",
  "environment": "Development",
  "timestamp": "2026-02-23T10:30:00Z",
  "version": "1.0.0"
}
```

---

## 🔍 Supabase Dashboard'da Tablolarınızı Görüntüleme

1. Supabase Dashboard > **Table Editor** sekmesine git
2. Migration sonrası oluşan tüm tablolarınızı görebilirsiniz:
   - `Users`
   - `Trips`
   - `Reservations`
   - `Companies`
   - vb.

---

## 🛠️ Faydalı Komutlar

### Yeni migration oluşturma

```powershell
cd src/Repository
dotnet ef migrations add MigrationName --startup-project ../Api
```

### Migration'ları listeleme

```powershell
cd src/Api
dotnet ef migrations list
```

### Son migration'ı geri alma

```powershell
cd src/Api
dotnet ef migrations remove
```

---

## 🔐 Güvenlik Önerileri

1. **DATABASE_URL'i asla Git'e commit etme**
   - `appsettings.Development.json` `.gitignore` da olmalı
   - Sadece environment variable kullan

2. **Güçlü şifre kullan**
   - En az 16 karakter
   - Büyük/küçük harf, sayı, özel karakter

3. **Row Level Security (RLS) aktifleştir**
   - Supabase Dashboard > Authentication > Policies
   - Tablolarınıza erişim kuralları ekleyin

4. **JWT Secret'ları değiştir**
   - Render'da `JWT_KEY` environment variable'ı zaten otomatik oluşuyor

---

## 📱 Monitoring ve Performans

### Supabase Dashboard'dan izleme:

1. **Database** > **Database Health**: CPU, RAM, disk kullanımı
2. **Logs**: Sorgu logları, hata logları
3. **Reports**: Bağlantı sayısı, sorgu performansı

### Performans optimizasyonu:

- **Pooling aktif**: Connection pool max 20 (production), 5 (dev)
- **Indexler**: Migration'larda gerekli indexleri ekleyin
- **Vakuuming**: Supabase otomatik yapıyor

---

## 🆘 Sorun Giderme

### Hata: "Cannot connect to database"

1. Connection string'i kontrol et
2. Supabase şifresinin doğru olduğundan emin ol
3. Supabase projesinin aktif olduğunu kontrol et

### Hata: "SSL connection required"

Program.cs'de `SslMode = SslMode.Require` zaten ayarlı. Eğer sorun devam ediyorsa:

```csharp
SslMode = SslMode.VerifyFull
```

### Hata: "Too many connections"

Free tier'da maksimum 60 bağlantı var. Connection pool ayarlarını azaltın:
- Production: `MaxPoolSize = 10`
- Development: `MaxPoolSize = 3`

---

## 💰 Maliyet Karşılaştırması

| Servis | Free Tier | 5000 Kullanıcı Desteği | Limit |
|--------|-----------|------------------------|-------|
| **Supabase** | ✅ Ücretsiz | ✅ Evet | 500MB DB, 50k auth/ay |
| Render PostgreSQL | ❌ 90 gün sonra durur | ⚠️ $7/ay | 1GB |
| Azure SQL | ✅ Ücretsiz | ⚠️ Karmaşık | 100k vCore sn/ay |
| PlanetScale | ✅ Ücretsiz | ✅ Evet | 5GB, 1B satır/ay |

**Kazanç: $7/ay = 200₺/ay**

---

## 🎓 Ek Kaynaklar

- [Supabase Docs](https://supabase.com/docs)
- [PostgreSQL Connection Pooling](https://supabase.com/docs/guides/database/connecting-to-postgres#connection-pooler)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

## ✨ Önceki Yapılandırma (Docker)

Eski Docker tabanlı yapılandırmaya dönmek isterseniz:

1. `docker-compose.yml` dosyasındaki comment'ları kaldırın
2. `appsettings.Development.json` dosyasını eski haline döndürün
3. `docker-compose up -d` komutunu çalıştırın

Ancak **Supabase kullanımı önerilir** çünkü:
- Ücretsiz
- Daha hızlı
- Otomatik backup
- Production ile aynı ortam
- SSL/TLS şifreleme
- Monitoring ve dashboard

---

**İyi çalışmalar! 🚀**
