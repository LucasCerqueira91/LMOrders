# Script para gerar imagens a partir dos arquivos PlantUML
# Usa o servidor online do PlantUML

$plantumlServer = "https://www.plantuml.com/plantuml/png"

function Generate-Image {
    param (
        [string]$pumlFile,
        [string]$outputFile
    )
    
    Write-Host "Gerando imagem de $pumlFile..."
    
    # Ler o conteúdo do arquivo .puml
    $content = Get-Content $pumlFile -Raw
    
    # Codificar em UTF-8
    $encoder = [System.Text.Encoding]::UTF8
    $bytes = $encoder.GetBytes($content)
    
    # Comprimir usando Deflate (necessário para PlantUML)
    $deflateStream = New-Object System.IO.MemoryStream
    $gzipStream = New-Object System.IO.Compression.DeflateStream($deflateStream, [System.IO.Compression.CompressionMode]::Compress)
    $gzipStream.Write($bytes, 0, $bytes.Length)
    $gzipStream.Close()
    
    $compressedBytes = $deflateStream.ToArray()
    
    # Converter para Base64 usando a codificação do PlantUML
    $base64 = [Convert]::ToBase64String($compressedBytes)
    $urlSafeBase64 = $base64 -replace '\+', '-' -replace '/', '_' -replace '=', ''
    
    # Construir URL
    $url = "$plantumlServer/$urlSafeBase64"
    
    # Baixar a imagem
    try {
        Invoke-WebRequest -Uri $url -OutFile $outputFile
        Write-Host "Imagem gerada: $outputFile"
    }
    catch {
        Write-Host "Erro ao gerar imagem: $_"
    }
}

# Lista de arquivos para processar
$diagrams = @(
    @{ Source = "docs/LMOrders-Modules.puml"; Output = "docs/LMOrders-Modules.png" },
    @{ Source = "docs/LMOrders-Folders.puml"; Output = "docs/LMOrders-Folders.png" },
    @{ Source = "docs/LMOrders-CQRS-CreateOrder.puml"; Output = "docs/LMOrders-CQRS-CreateOrder.png" }
)

Write-Host "Gerando diagramas PlantUML..." -ForegroundColor Green
Write-Host ""

foreach ($diagram in $diagrams) {
    Generate-Image -pumlFile $diagram.Source -outputFile $diagram.Output
}

Write-Host ""
Write-Host "Concluído! Verifique a pasta docs/ para as imagens geradas." -ForegroundColor Green
