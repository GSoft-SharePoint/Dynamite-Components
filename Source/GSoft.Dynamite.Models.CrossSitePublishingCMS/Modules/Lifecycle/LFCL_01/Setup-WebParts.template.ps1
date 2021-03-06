﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-WebParts.ps1.template
# Description	: Install Life Cycle Web Parts
# -----------------------------------------------------------------------

Write-Warning "Deploying Life Cycle Web Parts..."

# Activate features on publishing site collection.
Initialize-DSPFeature -Url [[DSP_PortalPublishingSiteUrl]] -Id "[[DSP_CrossSitePublishingCMS_LFCL_WebParts]]"