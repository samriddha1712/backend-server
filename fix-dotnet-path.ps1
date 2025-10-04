# Fix .NET SDK PATH Issue
# This script adds .NET to your PATH environment variable

Write-Host "🔍 Checking .NET installation..." -ForegroundColor Cyan

$dotnetPath = "C:\Program Files\dotnet"
$dotnetExe = Join-Path $dotnetPath "dotnet.exe"

if (Test-Path $dotnetExe) {
    Write-Host "✅ .NET SDK found at: $dotnetPath" -ForegroundColor Green
    
    # Check SDK versions
    Write-Host "`n📦 Installed SDK versions:" -ForegroundColor Cyan
    & $dotnetExe --list-sdks
    
    # Check current PATH
    $currentPath = [Environment]::GetEnvironmentVariable("Path", "User")
    
    if ($currentPath -like "*$dotnetPath*") {
        Write-Host "`n✅ .NET is already in your User PATH" -ForegroundColor Green
        Write-Host "⚠️  You may need to restart VS Code or your terminal for changes to take effect" -ForegroundColor Yellow
    } else {
        Write-Host "`n⚠️  .NET is NOT in your User PATH" -ForegroundColor Yellow
        Write-Host "`n🔧 Would you like to add it to your User PATH? (Y/N)" -ForegroundColor Cyan
        $response = Read-Host
        
        if ($response -eq 'Y' -or $response -eq 'y') {
            try {
                # Add to User PATH
                $newPath = "$currentPath;$dotnetPath"
                [Environment]::SetEnvironmentVariable("Path", $newPath, "User")
                
                # Also add to current session
                $env:Path = "$env:Path;$dotnetPath"
                
                Write-Host "`n✅ .NET has been added to your User PATH!" -ForegroundColor Green
                Write-Host "`n📋 Next steps:" -ForegroundColor Cyan
                Write-Host "   1. Close and reopen VS Code (or all terminal windows)" -ForegroundColor White
                Write-Host "   2. Test by running: dotnet --version" -ForegroundColor White
                Write-Host "   3. Then run: cd backend-dotnet" -ForegroundColor White
                Write-Host "   4. Finally run: .\start.ps1" -ForegroundColor White
                
                Write-Host "`n🎯 Testing in current session..." -ForegroundColor Cyan
                & dotnet --version
                Write-Host "✅ .NET is now available in this session!" -ForegroundColor Green
            }
            catch {
                Write-Host "`n❌ Error adding to PATH: $_" -ForegroundColor Red
                Write-Host "`n📋 Manual steps:" -ForegroundColor Yellow
                Write-Host "   1. Press Win + X and select 'System'" -ForegroundColor White
                Write-Host "   2. Click 'Advanced system settings'" -ForegroundColor White
                Write-Host "   3. Click 'Environment Variables'" -ForegroundColor White
                Write-Host "   4. Under 'User variables', select 'Path' and click 'Edit'" -ForegroundColor White
                Write-Host "   5. Click 'New' and add: C:\Program Files\dotnet" -ForegroundColor White
                Write-Host "   6. Click 'OK' on all dialogs" -ForegroundColor White
                Write-Host "   7. Restart VS Code" -ForegroundColor White
            }
        } else {
            Write-Host "`n📋 To add .NET to PATH manually:" -ForegroundColor Yellow
            Write-Host "   1. Press Win + X and select 'System'" -ForegroundColor White
            Write-Host "   2. Click 'Advanced system settings'" -ForegroundColor White
            Write-Host "   3. Click 'Environment Variables'" -ForegroundColor White
            Write-Host "   4. Under 'User variables', select 'Path' and click 'Edit'" -ForegroundColor White
            Write-Host "   5. Click 'New' and add: C:\Program Files\dotnet" -ForegroundColor White
            Write-Host "   6. Click 'OK' on all dialogs" -ForegroundColor White
            Write-Host "   7. Restart VS Code" -ForegroundColor White
        }
    }
} else {
    Write-Host "❌ .NET SDK not found at: $dotnetPath" -ForegroundColor Red
    Write-Host "`n📥 Download .NET 8.0 SDK from:" -ForegroundColor Yellow
    Write-Host "   https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
}

Write-Host "`n🔄 Alternative: Use full path in this session" -ForegroundColor Cyan
Write-Host "   You can run: `$env:Path = `"`$env:Path;C:\Program Files\dotnet`"" -ForegroundColor White
Write-Host "   Then test: dotnet --version" -ForegroundColor White
