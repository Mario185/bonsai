

function global:__bonsai_b {

  $outputFile = [System.IO.Path]::GetTempFileName()
  
  $currentLocation = Get-Location
  & bonsai f "$outputFile" "$currentLocation" "$args"

  $result = Get-Content $outputFile -Raw
  Remove-Item $outputFile

  if (![string]::IsNullOrEmpty($result) -and $LASTEXITCODE -eq 0) {
    $isDirectory = $result[0] -EQ "d"
    $isDefault = $result[1] -EQ "y"
    $command = $result.Substring(2)

    if ($isDefault -and $isDirectory)
    {
      Set-Location $command
      return;
    }

    Invoke-Expression $command
  }
}

function global:__bonsai_dotdot {__bonsai_b ".." }
function global:__bonsai_backslash {__bonsai_b "\" }

Set-Alias -Name b -Value __bonsai_b -Option AllScope -Scope Global -Force
Set-Alias -Name b.. -Value __bonsai_dotdot -Option AllScope -Scope Global -Force
Set-Alias -Name b\ -Value __bonsai_backslash -Option AllScope -Scope Global -Force

# Invoke-Expression (& { (bonsai init powershell | Out-String) })