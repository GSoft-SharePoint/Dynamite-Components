﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2015
# Model  		: Cross Site Publishing CMS
# File          : Setup-Content.ps1.template
# Description	: Import Content into SharePoint
# -----------------------------------------------------------------------

param([string]$LogFolderPath)

$0 = $myInvocation.MyCommand.Definition
$CommandDirectory = [System.IO.Path]::GetDirectoryName($0)

if ([string]::IsNullOrEmpty($LogFolderPath))
{
	$LogFolderPath = $CommandDirectory
}

# Script to fix file ACLs for bug fix with Sharegate import
$AclScript = $CommandDirectory + "\Restore-AclInheritance.ps1" 


# ******************************************
# Local utility methods
# ******************************************
function Get-FullPath {
    param(
		[Parameter(Mandatory=$true)]
        [string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path) -ne $true) {
        if ($Path.StartsWith(".\")) {
            $Path = $path.Replace(".\", ($CommandDirectory + "\"))
        } else {
            $Path = Join-Path -Path $CommandDirectory -ChildPath $Path
        }
    }

    return $Path;
}

function Get-CustomPropertyMappings {
    param(
		[Parameter(Mandatory=$true)]
        [string]$PropertyDisplayName)

    # Custom property mapping settings
	$MappingSettings = New-MappingSettings 

	# Remove default keys
	Remove-PropertyMapping -MappingSettings $MappingSettings -Destination Title | Out-Null
	Remove-PropertyMapping -MappingSettings $MappingSettings -Destination Created | Out-Null

	# Add custom keys
	Set-PropertyMapping -MappingSettings $MappingSettings -Source $PropertyDisplayName -Destination $PropertyDisplayName -Key | Out-Null

    return $MappingSettings
}

function Wait-VariationSyncTimerJob {

	# We don't need to sync manually items because we force the "Approved" status which automatically fires variations event receiver 
	# Sync item with timer job
	$Site = Get-SPSite "[[DSP_PortalPublishingHostNamePath]]"
	Write-Warning "Waiting for 'VariationsPropagateListItem' timer job to finish..."
	Wait-SPTimerJob -Name "VariationsPropagateListItem" -Site $Site
	Start-Sleep -Seconds 15
}


# Chech Restore ACL inheritance token
$RestoreAclInheritance = [System.Convert]::ToBoolean("[[DSP_RestoreAclInheritance]]")

# ******************************************
# Configure folder to URL mappings
# ******************************************
$IsMultilingual = [System.Convert]::ToBoolean("[[DSP_IsMultilingual]]")
$SourceLabel ="[[DSP_SourceLabel]]"
$DSP_MigrationFolderMappings = [[DSP_MigrationFolderMappings]]
$DSP_MigrationAssociationKeys = [[DSP_MigrationAssociationKeys]]

# If not already defined, create default folder to URL mappings
if($DSP_MigrationFolderMappings -eq $null) {
    $DSP_MigrationFolderMappings = [Ordered]@{ ".\Default\Default (english only)" = "[[DSP_PortalAuthoringSiteUrl]]" }

    # If the solution is multilingual then the source folder corresponds to the default variation label folder
    if ($IsMultilingual) {
	    $DSP_MigrationFolderMappings = [Ordered]@{ 
        ".\Default\Multilingual\Root" = "[[DSP_PortalAuthoringSiteUrl]]";
        ".\Default\Multilingual\Source" = "[[DSP_PortalAuthoringSiteUrl]]/[[DSP_SourceLabel]]" }

        [[DSP_VariationsTargetLabels]] |  Foreach-Object {
            if ($_ -ne "[[DSP_SourceLabel]]") {
                $FromFolder = (".\Default\Multilingual\Targets\" + $_)
                $ToUrl = ("[[DSP_PortalAuthoringSiteUrl]]/" + $_)
                
                $DSP_MigrationFolderMappings.Add($FromFolder, $ToUrl)
            }
        }
    }
}

# ******************************************
# Configure migration data before import
# ******************************************
$DSP_MigrationDataConfigurationScript = "[[DSP_MigrationDataConfigurationScript]]"
$DSP_MigrationFolderMappings.Keys | ForEach-Object {
    $Folder = Get-FullPath -Path $_
    $ConfigurationScript = Get-FullPath -Path $DSP_MigrationDataConfigurationScript
    & $ConfigurationScript -FolderPath $Folder
}

# ******************************************
# Import "non-variation" data
# ******************************************
# Defines mappings keys for "non-variation" content and variation targets
$mappingKeys = $DSP_MigrationFolderMappings.Keys | where { -not $_.ToUpperInvariant().Contains("TARGETS") }
$mappingKeys | ForEach-Object {
    $fromFolder = Get-FullPath -Path $_
    $toUrl = $DSP_MigrationFolderMappings[$_]

    if ($RestoreAclInheritance)
    {
        Write-Warning "Restore ACL inheritance on folder '$fromFolder'..."
        # Fix ACLs before importing data
        & $AclScript -folderPath $fromFolder
    }

    if ($_.ToUpperInvariant().Contains("SOURCE")) {

		# Configure mapping settings
	    $MappingSettings = Get-CustomPropertyMappings -PropertyDisplayName $DSP_MigrationAssociationKeys[$SourceLabel]

		# Configure property settings
		$DSP_MigrationDataMappingsFile = Get-FullPath -Path "[[DSP_MigrationDataSourceMappings]]"
		$DSP_MigrationDataMappingsName = "[[DSP_MigrationDataSourceMappingsName]]"

        Import-DSPData -FromFolder $fromFolder -ToUrl $toUrl -LogFolder $LogFolderPath -MappingSettings $MappingSettings -PropertyTemplateFile $DSP_MigrationDataMappingsFile -TemplateName $DSP_MigrationDataMappingsName
    } else {
        Import-DSPData -FromFolder $fromFolder -ToUrl $toUrl -LogFolder $LogFolderPath 
    }
}

# ******************************************
# If multilingual, sync and the process target label data
# ******************************************
if($IsMultilingual) {

    # Wait for variations to sync data to target label(s)
    Wait-VariationSyncTimerJob

    # Defines mappings keys for variation targets
    $TargetMappingKeys = $DSP_MigrationFolderMappings.Keys | where { $_.ToUpperInvariant().Contains("TARGETS") }
    $TargetMappingKeys | ForEach-Object {
        $FromFolder = Get-FullPath -Path $_
        $ToUrl = $DSP_MigrationFolderMappings[$_]

        if ($RestoreAclInheritance)
        {
            Write-Warning "Restore ACL inheritance on folder '$fromFolder'..."
            # Fix ACLs before importing data
            & $AclScript -folderPath $fromFolder
        }

        # Get target label language to determine what association key display name to use
        $IndexOfTargetLanguage = $FromFolder.LastIndexOf("\") + 1
        $TargetLabelLanguage = $FromFolder.Substring($IndexOfTargetLanguage)

		# Configure mapping settings
	    $MappingSettings = Get-CustomPropertyMappings -PropertyDisplayName $DSP_MigrationAssociationKeys[$TargetLabelLanguage]

		# Configure property settings
		$DSP_MigrationDataMappingsFile = Get-FullPath -Path "[[DSP_MigrationDataTargetMappings]]"
		$DSP_MigrationDataMappingsName = "[[DSP_MigrationDataTargetMappingsName]]"

		# Import data
        Import-DSPData -FromFolder $FromFolder -ToUrl $ToUrl -LogFolder $LogFolderPath -MappingSettings $MappingSettings -PropertyTemplateFile $DSP_MigrationDataMappingsFile -TemplateName $DSP_MigrationDataMappingsName
    }
}