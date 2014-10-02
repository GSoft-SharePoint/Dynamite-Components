﻿# Verbose preference
$VerbosePreference ="Continue"

# Unblock files if they're from another computer
gci -Recurse | Unblock-File

# Build Package Path
#$packagePath = Join-Path (Get-Location).ToString() "../package"

# Make Path if not exist
#$packageDirectory = New-Item -ItemType Directory -Force -Path $packagePath

# Update Tokens and shit in that path
#Update-DSPTokens -PackagePath $packageDirectory.FullName

Update-DSPTokens -UseHostName

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

# ********** LOG INIT ********** #

$ScriptName = [System.IO.Path]::GetFileNameWithoutExtension($MyInvocation.MyCommand.Name)

$LogFolderPath = ((Get-Location).Path + "\Logs")

if(!(Test-Path -Path $LogFolderPath)){
	New-Item -ItemType directory -Path $LogFolderPath
}
else
{
	# Reset the log folder
	Get-ChildItem $LogFolderPath | Foreach-Object { Remove-Item  $_.FullName -Force }
}

$LogTime = Get-Date -Format "MM-dd-yyyy_hh-mm-ss"
$LogFile = $LogFolderPath + "\" + $ScriptName +"_Dynamite_"+$LogTime +".log"

# Stat log transcript
Start-Transcript -Path $LogFile

# ******** SOLUTIONS DEPLOYMENT ********* #

#./Solutions/Copy-Solutions.ps1

#./Solutions/Deploy-Solutions.ps1 [[DSP_IsDistributedEnvironment]]

$values = @{"Solution Model: " = "[CrossSitePublishingCMS]";}
New-HeaderDrawing -Values $Values

# ********** PUBLISHING MODULE ********** #

##### PUB 01
$Script = $Script = $CommandDirectory + "\Modules\Publishing\PUB_01\Install-PUB01.ps1"
Start-Process PowerShell -ArgumentList $Script, $LogFolderPath -Wait

##### PUB 02
$Script = $Script = $CommandDirectory + "\Modules\Publishing\PUB_02\Install-PUB02.ps1"
Start-Process PowerShell -ArgumentList $Script, $LogFolderPath -Wait

# ********** LOG END ********** #
# Stop log transcript
Stop-Transcript
