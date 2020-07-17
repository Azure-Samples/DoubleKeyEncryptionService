# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

function ParseAuthResponse
{
    param(
        [parameter(Mandatory = $true)]
        [string] $AuthHeader
    )
    if (($AuthHeader -match "^\s*bearer") -eq $False) {
      throw "Auth header does not contain bearer"
    }
    $authFields = $AuthHeader.Split(',') -replace "bearer", "";
    $returnedAuthKeyValues = @{}
    foreach ($item in $authFields) {
        $matchedAuthLine = [regex]::Match($item, "\s*(?<name>[^=]+)\=\s*\`"(?<value>[^\`"]*)\`"")
        if ($matchedAuthLine.Success -eq $False) {
          throw "Invalid auth value: $item"
        }

        $returnedAuthKeyValues.Add($matchedAuthLine.Groups["name"].value.Trim(), $matchedAuthLine.Groups["value"].value.Trim())
    }
    return $returnedAuthKeyValues;
}

if ($args.Count -ne 1) {
  Write-Host "
cmdlet to test a double key customer key store
Please enter the url with key name to test
Ex. .\key_store_tester.ps1 https://mykeystoreurl.com/mykey1
  "
  exit
}

Write-Host "Validation request started: $($args[0])"

if (-Not $args[0].Trim().StartsWith("https")) {
  Write-Host -ForegroundColor red "Validation failure: Url must be begin with 'https'"
  exit
}

try {
  $publicKeyResponse = Invoke-WebRequest -uri $args[0] -Method 'GET'
} catch [System.Net.WebException] {
  Write-Host -ForegroundColor red "Validation failure: Unable to access the provided url $($_.Exception.Response.StatusDescription)"
  exit
} catch {
  Write-Host -ForegroundColor red "Unexpected error $($_.Exception)"
  exit
}

Write-Host "Received content from url: $($publicKeyResponse.Content)"

$jsonResponse = ConvertFrom-Json -InputObject $publicKeyResponse.Content

if (-Not ([bool]($jsonResponse -match "key"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the key"
  exit
}

$jsonKeyResponse = $jsonResponse.key

if (-Not ([bool]($jsonKeyResponse -match "kty"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the key type"
  exit
}

if (-Not ([bool]($jsonKeyResponse -match "n"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the modulus"
  exit
}

if (-Not ([bool]($jsonKeyResponse -match "e"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the exponent"
  exit
}

if (-Not ([bool]($jsonKeyResponse -match "alg"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the algorithm"
  exit
}

if (-Not ([bool]($jsonKeyResponse -match "kid"))) {
  Write-Host -ForegroundColor red "Validation failure: Response does not contain the key id"
  exit
}

if ($jsonKeyResponse.kty -ne "RSA") {
  Write-Host -ForegroundColor red "Validation failure: Invalid key type"
}

if ($jsonKeyResponse.alg -ne "RS256") {
  Write-Host -ForegroundColor red "Validation failure: Invalid key algorithm"
}

Write-Host "Public key API validation complete"

try {
  $decryptUrl = "$($jsonKeyResponse.kid)/decrypt"
  Write-Host "Attempting to access url: $($decryptUrl)"
  $decryptResponse = Invoke-WebRequest -uri $decryptUrl -Method 'POST'
  Write-Host -ForegroundColor red "Validation failure: Decryption unexpected response"
  exit
} catch [System.Net.WebException] {
  # it is expected that the call will throw an exception because auth failed
  if($_.Exception.Response.StatusCode -ne 401) {
    Write-Host -ForegroundColor red "Validation failure: Decryption unexpected response - $($_.Exception.Response.StatusDescription)"
    exit
  }

  $headerCount = $_.Exception.Response.Headers.Count
  $authFound = $False
  for ($index = 0; $index -lt $headerCount; $index++) {
    if ( $_.Exception.Response.Headers.Keys[$index] -eq "WWW-Authenticate") {
      Write-Host "Found auth header - $($_.Exception.Response.Headers[$index])"
      $authFound = $True;

      $responeFields = ParseAuthResponse $_.Exception.Response.Headers[$index]
      if (-Not ($responeFields.ContainsKey("resource"))) {
        Write-Host -ForegroundColor red "resource auth field not found: $authResourceUri"
        exit
      }

      if (-Not ($responeFields.ContainsKey("authorization"))) {
        Write-Host -ForegroundColor red "authorization auth field not found: $authResourceUri"
        exit
      }

      if (-Not ($responeFields.ContainsKey("realm"))) {
        Write-Host -ForegroundColor red "realm auth field not found: $authResourceUri"
        exit
      }

      $resourceAuthField = $responeFields["resource"]

      if (-Not ($resourceAuthField.StartsWith("https://"))) {
        Write-Host -ForegroundColor red "Resource auth field ($($resourceAuthField)) must contains 'https://'.  Ensure that the `"JwtAudience`" value in appsettings.json contains 'https://'"
        exit
      }

      $authResourceUri = [System.Uri]$resourceAuthField
      $decryptUrlUri = [System.Uri]$decryptUrl

      Write-Host "Validated parsed resource: $($authResourceUri)"

      if ($authResourceUri.host -ne $decryptUrlUri.host) {
        Write-Host -ForegroundColor red "Hostname mismatch between auth resource ($($authResourceUri.host)) and key url ($($decryptUrlUri.host)).  Ensure that the `"JwtAudience`" value in appsettings.json matches the host where the key store has been published"
        exit
      }
    }
  }

  if($authFound -eq $False) {
    Write-Host -ForegroundColor red "Validation failure: WWW-Authenticate header not found"
    exit
  }
} catch {
  Write-Host -ForegroundColor red "Unexpected error $($_.Exception)"
  exit
}

Write-Host -ForegroundColor green "Validation successful!"
