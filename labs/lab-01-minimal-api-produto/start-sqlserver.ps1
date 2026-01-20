# Script para iniciar o SQL Server via Docker Compose
# Execute este script antes de rodar a aplicação

Write-Host "Iniciando SQL Server..." -ForegroundColor Green
cd ..
docker-compose up -d sqlserver

Write-Host "Aguardando SQL Server ficar pronto..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "Verificando status do container..." -ForegroundColor Cyan
docker ps --filter "name=labs-sqlserver" --format "table {{.Names}}\t{{.Status}}"

Write-Host "`nSQL Server deve estar disponível em:" -ForegroundColor Green
Write-Host "  - Host: localhost" -ForegroundColor White
Write-Host "  - Porta: 1433" -ForegroundColor White
Write-Host "  - Usuário: sa" -ForegroundColor White
Write-Host "  - Senha: Lab@Mvp24Hours!" -ForegroundColor White
