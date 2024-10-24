function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,  
        [Parameter(Mandatory = $true)]
        [int]$Port,  
        [Parameter(Mandatory = $true)]
        [string]$IP  
    )

    validateFile -QueryFile $QueryFile

    $ipEndPoint = Get-EndPoint -IP $IP -Port $Port

    ProcessQueryFile -QueryFile $QueryFile -ipEndPoint $ipEndPoint -Port $Port -IP $IP
}

function validateFile {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile 
    )

    if ([System.IO.Path]::GetExtension($QueryFile) -ne ".tinysql") {
        Write-Host -ForegroundColor Red "Error: El archivo debe tener la extensión .tinysql."
        exit 1
    }

    if (-not (Test-Path $QueryFile)) {
        Write-Host -ForegroundColor Red "Error: El archivo $QueryFile no existe. Verifique la ruta."
        exit 1
    }
}

function Get-EndPoint {
    param (
        [Parameter(Mandatory = $true)]
        [string]$IP,
        [Parameter(Mandatory = $true)]
        [int]$Port
    )
    
    try {
        $ipEndPoint = [System.Net.IPEndPoint]::new([System.Net.IPAddress]::Parse($IP), $Port)
        return $ipEndPoint
    } catch {
        Write-Host -ForegroundColor Red "Error: La IP o el puerto son incorrectos."
        exit 1
    }
}

function ProcessQueryFile {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,
        [System.Net.IPEndPoint]$ipEndPoint,
        [int]$Port,  
        [string]$IP  
    )

    Write-Host -ForegroundColor Yellow "Leyendo el archivo de instrucciones: $QueryFile"
    $lines = Get-Content $QueryFile
    foreach ($line in $lines) {
        if (-not [string]::IsNullOrWhiteSpace($line)) {
            $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

            Send-SQLCommand -command $line -ipEndPoint $ipEndPoint -Port $Port -IP $IP

            $stopwatch.Stop()
            $elapsedTime = $stopwatch.ElapsedMilliseconds

            Write-Host -ForegroundColor Cyan "El comando '$line' tardó $elapsedTime ms en ejecutarse."
        }
    }
}

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
        [string]$command,
        [System.Net.IPEndPoint]$ipEndPoint,
        [int]$Port,  
        [string]$IP  
    )

    $client = New-Object System.Net.Sockets.Socket($ipEndPoint.AddressFamily, [System.Net.Sockets.SocketType]::Stream, [System.Net.Sockets.ProtocolType]::Tcp)

    try {
        $client.Connect($ipEndPoint)
    } catch {
        Write-Host -ForegroundColor Red "Error: No se pudo conectar al servidor en $IP :$Port. Verifique la dirección IP y el puerto, o si el servidor está activo."
        exit 1
    }
    
    $requestObject = [PSCustomObject]@{
        RequestType = 0;
        RequestBody = $command
    }

    Write-Host -ForegroundColor Green "Enviando comando: $command"

    $jsonMessage = ConvertTo-Json -InputObject $requestObject -Compress
    Send-Message -client $client -message $jsonMessage

    $response = Receive-Message -client $client
    Write-Host -ForegroundColor Green "Respuesta recibida: $response"

    $responseObject = ConvertFrom-Json -InputObject $response
    Write-Output $responseObject

    $client.Shutdown([System.Net.Sockets.SocketShutdown]::Both)
    $client.Close()
}
