# Script para enviar un mensaje de prueba a la cola

$connectionString = "UseDevelopmentStorage=true"

# Cargar el assembly de Azure Storage
Add-Type -Path "F:\Desarrollo\Repositorios\logs_viewer\src\AppFunction\bin\Debug\net8.0\Azure.Storage.Queues.dll"

# Crear cliente de cola
$queueServiceClient = [Azure.Storage.Queues.QueueServiceClient]::new($connectionString)
$queueClient = $queueServiceClient.GetQueueClient("logsqueue")

# Crear el mensaje de prueba
$message = @{
    Source = "TestSource"
    PublishedAtUtc = [DateTime]::UtcNow.ToString("O")
    Event = @{
        Timestamp = [DateTime]::UtcNow.ToString("O")
        Level = "INFO"
        Logger = "TestLogger"
        Message = "Test message"
        Exception = $null
    }
} | ConvertTo-Json -Compress

Write-Output "Mensaje JSON: $message"
Write-Output "Enviando mensaje a la cola 'logsqueue'..."

try {
    $queueClient.SendMessage($message)
    Write-Output "OK: Mensaje enviado exitosamente"
} catch {
    Write-Output "ERROR al enviar mensaje: $_"
}
