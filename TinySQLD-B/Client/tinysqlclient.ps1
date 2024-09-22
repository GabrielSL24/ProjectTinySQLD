param (
    [Parameter(Mandatory = $true)]
    [string]$IP,
    [Parameter(Mandatory = $true)]
    [int]$Port,
    [Parameter(Mandatory = $true)]
    [string]$QueryFile  # Archivo de instrucciones 
)

$ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)

function Send-Message {
    param (
        [Parameter(Mandatory=$true)]
        [pscustomobject]$message,
        [Parameter(Mandatory=$true)]
        [System.Net.Sockets.Socket]$client
    )

    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $writer = New-Object System.IO.StreamWriter($stream)
    try {
        $writer.WriteLine($message)
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}

function Receive-Message {
    param (
        [System.Net.Sockets.Socket]$client
    )
    $stream = New-Object System.Net.Sockets.NetworkStream($client)
    $reader = New-Object System.IO.StreamReader($stream)
    try {
        return $null -ne $reader.ReadLine ? $reader.ReadLine() : ""
    }
    finally {
        $reader.Close()
        $stream.Close()
    }
}
function Send-SQLCommand {
    param (
        [string]$command
    )
    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)

    try {
        $client.Connect($ipEndPoint)
    } catch {
        Write-Host -ForegroundColor Red "Error: No se pudo conectar al servidor en $IP : $Port. Verifique que la direccion IP y el puerto sean correctos, o si el servidor esta activo."
        exit 1
    }

    $requestObject = [PSCustomObject]@{
        RequestType = 0;
        RequestBody = $command
    }
    Write-Host -ForegroundColor Green "Sending command: $command"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage
    $response = Receive-Message -client $client

    Write-Host -ForegroundColor Green "Response received: $response"
    
    $responseObject = ConvertFrom-Json -InputObject $response
    Write-Output $responseObject
    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()
}

# Verificar que el archivo tenga extensión .txt
if ([System.IO.Path]::GetExtension($QueryFile) -ne ".tinysql") {
    Write-Host -ForegroundColor Red "Error: El archivo debe tener la extension .tinysql."
    exit 1
}

if (Test-Path $QueryFile) {
    Write-Host -ForegroundColor Yellow "Leyendo el archivo de instrucciones: $QueryFile"
    
    # Leer archivo línea por línea
    $lines = Get-Content $QueryFile
    foreach ($line in $lines) {
        if (-not [string]::IsNullOrWhiteSpace($line)) {
            Send-SQLCommand -command $line
        }
    }
} else {
    Write-Host -ForegroundColor Red "Error: El archivo $QueryFile no existe. Verifique la ruta y vuelva a intentarlo."
    exit 1
}