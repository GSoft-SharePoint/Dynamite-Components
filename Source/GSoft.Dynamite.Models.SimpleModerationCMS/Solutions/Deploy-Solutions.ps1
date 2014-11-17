﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Deploy-Solutions.ps1.template
# Description	: Deploys WSP Solutions
# -----------------------------------------------------------------------
param([switch] $RemoveOnly=$false, [switch] $Distributed=$false)

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

$DefaultConfigurationFile = "./Default/Default-Solutions.xml"
$CustomConfigurationFile = "./Custom/Custom-Solutions.xml"

$ConfigurationFilePath = $CommandDirectory + ".\" + $DefaultConfigurationFile

$xmlInput = Get-Content $ConfigurationFilePath

Deploy-DSPSolution -Config $xmlInput -RemoveOnly:$RemoveOnly

if(![string]::IsNullOrEmpty($CustomConfigurationFile))
{
	$ConfigurationFilePath = $CommandDirectory + ".\" + $CustomConfigurationFile

	$xmlInput = Get-Content $ConfigurationFilePath

	Deploy-DSPSolution -Config $xmlInput -RemoveOnly:$RemoveOnly
}

if($Distributed)
{
	Read-Host "Please restart the OWSTIMER.exe process an all SharePoint servers (APP and WFE) before continue. Press enter when its done..."
}
else
{
	Restart-SPTimer
	Write-Warning "OWSTimer.exe process restarted locally."
}
