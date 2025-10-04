# 🔑 Supabase Database Connection Setup

## ⚠️ Database Password Required

I've configured your backend to connect to your Supabase database, but you need to add your database password.

### 📍 Your Supabase Project

- **Project URL**: `https://ozfugqmymfyxevkvioit.supabase.co`
- **Project Ref**: `ozfugqmymfyxevkvioit`
- **Database Host**: `db.ozfugqmymfyxevkvioit.supabase.co`

## 🔍 How to Get Your Database Password

### Option 1: From Supabase Dashboard

1. Go to [Supabase Dashboard](https://app.supabase.com)
2. Select your project: **ozfugqmymfyxevkvioit**
3. Go to **Settings** (gear icon) → **Database**
4. Scroll down to **Connection Info** section
5. Click **"Show"** next to the password field
6. Copy the **Database Password**

### Option 2: Connection String Method

1. Go to [Supabase Dashboard](https://app.supabase.com)
2. Select your project
3. Go to **Settings** → **Database**
4. Look for **Connection String** section
5. Select **URI** tab
6. The connection string format will be:
   ```
   postgresql://postgres.ozfugqmymfyxevkvioit:[YOUR-PASSWORD]@db.ozfugqmymfyxevkvioit.supabase.co:5432/postgres
   ```
7. Extract the password from between `:[YOUR-PASSWORD]@`

## 📝 Update Configuration Files

Once you have the password, update these files:

### 1. Update `appsettings.json`

Replace `YOUR_DATABASE_PASSWORD_HERE` with your actual password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ozfugqmymfyxevkvioit.supabase.co;Database=postgres;Username=postgres.ozfugqmymfyxevkvioit;Password=YOUR_ACTUAL_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### 2. Update `appsettings.Development.json`

Use the same password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.ozfugqmymfyxevkvioit.supabase.co;Database=postgres;Username=postgres.ozfugqmymfyxevkvioit;Password=YOUR_ACTUAL_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

## ✅ Connection String Format

Your connection string is already configured with:

- ✅ **Host**: `db.ozfugqmymfyxevkvioit.supabase.co`
- ✅ **Database**: `postgres`
- ✅ **Username**: `postgres.ozfugqmymfyxevkvioit`
- ⚠️ **Password**: `YOUR_DATABASE_PASSWORD_HERE` ← **You need to replace this**
- ✅ **Port**: `5432`
- ✅ **SSL Mode**: `Require`
- ✅ **Trust Server Certificate**: `true`

## 🧪 Test the Connection

After updating the password, test the connection:

```powershell
cd backend-dotnet
dotnet run
```

If the connection is successful, you'll see:
```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (XX ms)
```

If there's an error, check:
1. Password is correct
2. No extra spaces in the connection string
3. Firewall allows connection to Supabase
4. SSL Mode is set to `Require`

## 🔒 Security Best Practices

### ⚠️ Important: Never Commit Passwords

1. **Add to `.gitignore`** (already done):
   ```
   appsettings.json
   appsettings.Development.json
   appsettings.Production.json
   ```

2. **Use Environment Variables** (Alternative Method):

   Instead of putting the password in `appsettings.json`, you can use environment variables:

   **PowerShell:**
   ```powershell
   $env:ConnectionStrings__DefaultConnection="Host=db.ozfugqmymfyxevkvioit.supabase.co;Database=postgres;Username=postgres.ozfugqmymfyxevkvioit;Password=YOUR_PASSWORD;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
   
   dotnet run
   ```

   **Or create a `.env` file** and use a package like `DotNetEnv`

3. **For Production**: Use Azure Key Vault, AWS Secrets Manager, or Supabase connection pooling

## 🎯 What's Already Configured

I've already set up:

✅ Supabase URL in config
✅ Supabase Anon Key
✅ Supabase Service Role Key
✅ Correct database host
✅ Correct username format (`postgres.ozfugqmymfyxevkvioit`)
✅ SSL Mode enabled
✅ CORS configured for your Next.js app
✅ JWT authentication settings

**You only need to add your database password!**

## 📚 Additional Resources

- [Supabase Database Connection Docs](https://supabase.com/docs/guides/database/connecting-to-postgres)
- [.NET Connection Strings](https://www.npgsql.org/doc/connection-string-parameters.html)

---

**Once you add the password, your .NET backend will connect directly to your Supabase PostgreSQL database! 🚀**
