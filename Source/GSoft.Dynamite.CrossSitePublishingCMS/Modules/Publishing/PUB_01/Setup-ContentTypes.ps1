﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-ContentTypes.ps1.template
# Description	: Create content types structure
# -----------------------------------------------------------------------

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

$DefaultConfigurationFile = "./Default/Default-ContentTypes.xml"

$ConfigurationFilePath = $CommandDirectory + ".\" + $DefaultConfigurationFile

Write-Warning "Applying Content Types configuration..."

# Apply default site columns creation and content types
[xml]$featureXml = Get-Content $ConfigurationFilePath

# Activate features
Initialize-DSPSiteCollectionsFeatures $featureXml $true



