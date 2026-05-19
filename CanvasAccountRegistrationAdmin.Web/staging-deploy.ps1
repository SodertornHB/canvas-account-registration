ssh admin@89.47.185.130 'powershell -Command "Stop-Website -Name \"stg-canvas-account-registration-admin\""'

dotnet publish "WebAdmin.csproj" `
    -c Release `
    -p:PublishProfile=Properties\PublishProfiles\staging-publish.pubxml

$maxRetries = 5
$attempt = 0
$success = $false

while (-not $success -and $attempt -lt $maxRetries) {
    $attempt++
    Write-Host "Försök $attempt..."

    ssh admin@89.47.185.130 'powershell -Command "Remove-Item -Recurse -Force C:\inetpub\wwwroot\canvas-account-registration-admin\*"' 

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Körning lyckades."
        $success = $true
    }
    else {
        Write-Warning "Fel vid försök $attempt. Väntar 5 sekunder..."
        Start-Sleep -Seconds 5
    }
}

if (-not $success) {
    Write-Error "Misslyckades efter $maxRetries försök."
}


scp -r "bin\Release\net8.0\publish\*" admin@89.47.185.130:"/inetpub/wwwroot/canvas-account-registration-admin/"

ssh admin@89.47.185.130 'powershell -Command "Start-Website -Name \"stg-canvas-account-registration-admin\""'

Start-Sleep -Seconds 5

try {
    $response = Invoke-WebRequest -Uri "https://stg-canvas-account-registration-admin.shbiblioteket.se" -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Sajten svarar som förväntat med HTTP 200."
    }
    else {
        Write-Host "⚠️ Sajten svarar, men inte med HTTP 200. Kod: $($response.StatusCode)"
    }
}
catch {
    Write-Host "❌ Misslyckades att nå sajten: $_"
}