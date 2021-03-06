﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-CatalogConnections.ps1.template
# Description	: Create catalog connections
# -----------------------------------------------------------------------

Write-Warning "Starting search crawl..."
Start-DSPContentSourceCrawl -ContentSourceName "[[DSP_SearchContentSourceName]]" -SearchServiceApplicationName "[[DSP_SearchServiceApplicationName]]" | Wait-DSPContentSourceCrawl

Write-Warning "Applying Catalog Connections configuration..."

$HashTable = [[DSP_CrossSiteMappings]]

$HashTable.Keys | Foreach-Object { 

	# Activate feature on each authoring webs
	Initialize-DSPFeature -Url $_ -Id [[DSP_CrossSitePublishingCMS_NAV_CatalogConnections]]

}