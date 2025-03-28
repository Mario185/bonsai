$CurrentSearchDirectory = $PSScriptRoot;
$NukeBuildSchemaJsonPath = "";

while($CurrentSearchDirectory)
{
  $NukeBuildSchemaJsonTestPath = Join-Path -Path $CurrentSearchDirectory  -ChildPath ".nuke\build.schema.json";
  Write-Host "Searching for .nuke\build.schema.json in $NukeBuildSchemaJsonTestPath" ;
  
  if (Test-Path $NukeBuildSchemaJsonTestPath)
  {
    $NukeBuildSchemaJsonPath = $NukeBuildSchemaJsonTestPath;
    break;
  }
  $CurrentSearchDirectory = Split-Path $CurrentSearchDirectory -Parent;

}

if (!$NukeBuildSchemaJsonPath)
{
  Write-Error "build.schema.json not found.";
  exit 1;
}

Write-Host "Found build.schema.json in $NukeBuildSchemaJsonPath";


$BuildSchemaJson = Get-Content $NukeBuildSchemaJsonPath | ConvertFrom-Json;

$LaunchSettings =  New-Object PSCustomObject
$LaunchSettings | Add-Member  -MemberType NoteProperty -Name "profiles" -Value (New-Object PSCustomObject)

foreach($target in $BuildSchemaJson.definitions.ExecutableTarget.enum)
{
  $LaunchProfile = [PSCustomObject]@{
    commandName = "Project"
    commandLineArgs = "--target $target --verbosity normal"
    dotnetRunMessages = $true
  }
  
  $LaunchSettings.profiles | Add-Member -MemberType NoteProperty -Name "$target" -Value $LaunchProfile
 
}

$launchSettingsFilePath = Join-Path -Path $PSScriptRoot  -ChildPath "Properties\launchSettings.json";

Write-Host "Writing launchsettings.json to $launchSettingsFilePath"

($LaunchSettings | ConvertTo-Json) | Out-File -FilePath $launchSettingsFilePath;

Write-Host "Done";