﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-PageLayouts.ps1.template
# Description	: Create page layouts
# -----------------------------------------------------------------------

# Verbose preference
$VerbosePreference ="Continue"

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

$DefaultConfigurationFile = "./Default/Default-PageLayouts.xml"

$ConfigurationFilePath = $CommandDirectory + ".\" + $DefaultConfigurationFile

Write-Warning "Applying Page Layouts configuration..."

# Apply default site columns creation and content types
[xml]$featureXml = Get-Content $ConfigurationFilePath

# Activate features
Initialize-DSPSiteCollectionsFeatures $featureXml $true