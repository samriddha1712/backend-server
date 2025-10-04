# AI Smart Sheet .NET Backend - Quick Start Script
# This script helps you get started quickly with the backend API

Write-Host "🚀 AI Smart Sheet - .NET Backend Setup" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Check if .NET 8.0 SDK is installed
Write-Host "Checking for .NET 8.0 SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET 8.0 SDK not found. Please install from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Red
    exit 1
}

# Navigate to backend directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "`n📦 Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}

# Check if appsettings.json is configured
Write-Host "`n⚙️  Checking configuration..." -ForegroundColor Yellow
$appsettings = Get-Content "appsettings.json" -Raw | ConvertFrom-Json

if ($appsettings.ConnectionStrings.DefaultConnection -like "*YOUR_DATABASE_PASSWORD_HERE*") {
    Write-Host "⚠️  WARNING: Database password not configured!" -ForegroundColor Yellow
    Write-Host "   Please get your password from Supabase Dashboard and update appsettings.json" -ForegroundColor Yellow
    Write-Host "   See DATABASE_CONNECTION_SETUP.md for instructions`n" -ForegroundColor Cyan
    
    $continue = Read-Host "   Continue anyway? (y/n)"
    if ($continue -ne "y") {
        exit 0
    }
} else {
    Write-Host "✅ Database connection configured" -ForegroundColor Green
}

if ($appsettings.JwtSettings.SecretKey -like "*YOUR_JWT_SECRET*") {
    Write-Host "⚠️  WARNING: JWT Secret Key not configured!" -ForegroundColor Yellow
    Write-Host "   Please update appsettings.json with a secure secret key (min 32 characters)" -ForegroundColor Yellow
    
    $continue = Read-Host "`n   Continue anyway? (y/n)"
    if ($continue -ne "y") {
        exit 0
    }
}

Write-Host "`n🏗️  Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful" -ForegroundColor Green
} else {
    Write-Host "❌ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host "`n🚀 Starting API server..." -ForegroundColor Yellow
Write-Host "   API will be available at:" -ForegroundColor Cyan
Write-Host "   - HTTP:  http://localhost:5000" -ForegroundColor Green
Write-Host "   - HTTPS: https://localhost:5001" -ForegroundColor Green
Write-Host "   - Swagger: http://localhost:5000 (root endpoint)" -ForegroundColor Green
Write-Host ""
Write-Host "   Press Ctrl+C to stop the server" -ForegroundColor Yellow
Write-Host ""

# Run the application
dotnet run
