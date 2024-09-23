function Execute-MyQuery {
    param (
        [Parameter(Mandatory = $true)]
        [string]$QueryFile,  # Ruta del archivo con las sentencias SQL
        [Parameter(Mandatory = $true)]
        [int]$Port,  # Puerto en el que la API escucha
        [Parameter(Mandatory = $true)]
        [string]$IP  # Direcci�n IP en la que la API escucha
    )

    if ([System.IO.Path]::GetExtension($QueryFile) -ne ".tinysql") {
        Write-Host -ForegroundColor Red "Error: El archivo debe tener la extension .tinysql."
        exit 1
    }

    # Verificar si el archivo existe
    if (-not (Test-Path $QueryFile)) {
        Write-Host -ForegroundColor Red "Error: El archivo $QueryFile no existe. Verifique la ruta."
        exit 1
    }

    if ($Port -lt 1 -or $Port -gt 65535) {
        Write-Host -ForegroundColor Red "Error: El puerto '$Port' no es valido. Debe estar entre 1 y 65535."
        exit 1
    }

    # Crear el EndPoint con IP v�lida y puerto correcto
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
            Write-Host -ForegroundColor Red "Error: No se pudo conectar al servidor en $IP :$Port. Verifique que la direccion IP y el puerto sean correctos, o si el servidor esta activo."
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

    #Leer el archivo
    Write-Host -ForegroundColor Yellow "Leyendo el archivo de instrucciones: $QueryFile"
    $lines = Get-Content $QueryFile
    foreach ($line in $lines) {
        if (-not [string]::IsNullOrWhiteSpace($line)) {
            Send-SQLCommand -command $line
        }
    }

}
