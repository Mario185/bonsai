function global:__bonsai_b {

  # Get a temp file name where we can later put the command in
  $outputFile = [System.IO.Path]::GetTempFileName()
  
  $currentLocation = Get-Location
  
  # if no args are given open the explorer app otherwise the navigation app
  if ($args.Count -EQ 0)  {
	  & bonsai main "$outputFile" "$currentLocation"
  }
  else  {
	  & bonsai nav "$outputFile" "$currentLocation" "$args"
  }

  # Get the content of the temp file
  $result = Get-Content $outputFile -Raw

  # Delete the temp file
  Remove-Item $outputFile

  # If the temp file had content and the exit code was 0 process
  if (![string]::IsNullOrEmpty($result) -and $LASTEXITCODE -eq 0) {
    $isDirectory = $result[0] -EQ "d"
    $isDefault = $result[1] -EQ "y"
    $command = $result.Substring(2)

    # in case it was the default command for a directory just use set-location
    if ($isDefault -and $isDirectory)
    {
      Set-Location $command
      return;
    }

    # in any other case we invoke the given expression
    Invoke-Expression $command
  }
}

# Register function for b..
function global:__bonsai_dotdot {__bonsai_b ".." }
# Register function for b\
function global:__bonsai_backslash {__bonsai_b "\" }

# Register defaul alias
Set-Alias -Name b -Value __bonsai_b -Option AllScope -Scope Global -Force

#Register alias for b.. and b\ so we can enter "b.." and "b\" without space after b.
Set-Alias -Name b.. -Value __bonsai_dotdot -Option AllScope -Scope Global -Force
Set-Alias -Name b\ -Value __bonsai_backslash -Option AllScope -Scope Global -Force

# Add the following line to your powershell profile to initialize bonsai
# Invoke-Expression (& { (bonsai init powershell | Out-String) })