# Script para testar conexão com SQL Server
$connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=Lab@Mvp24Hours!;TrustServerCertificate=True;"

Write-Host "Testando conexão com SQL Server..." -ForegroundColor Yellow

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "✓ Conexão estabelecida com sucesso!" -ForegroundColor Green
    
    # Criar banco de dados se não existir
    $createDbQuery = @"
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Lab02_Clientes')
BEGIN
    CREATE DATABASE Lab02_Clientes;
    PRINT 'Banco de dados Lab02_Clientes criado com sucesso!';
END
ELSE
BEGIN
    PRINT 'Banco de dados Lab02_Clientes já existe.';
END
"@
    
    $command = New-Object System.Data.SqlClient.SqlCommand($createDbQuery, $connection)
    $command.ExecuteNonQuery() | Out-Null
    Write-Host "✓ Banco de dados verificado/criado!" -ForegroundColor Green
    
    $connection.Close()
}
catch {
    Write-Host "✗ Erro ao conectar: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique se:" -ForegroundColor Yellow
    Write-Host "  1. O SQL Server está rodando: docker ps | Select-String sqlserver" -ForegroundColor Cyan
    Write-Host "  2. A porta 1433 está acessível: Test-NetConnection -ComputerName localhost -Port 1433" -ForegroundColor Cyan
    Write-Host "  3. Se necessário, inicie o SQL Server: cd .. && docker-compose up -d sqlserver" -ForegroundColor Cyan
    exit 1
}
