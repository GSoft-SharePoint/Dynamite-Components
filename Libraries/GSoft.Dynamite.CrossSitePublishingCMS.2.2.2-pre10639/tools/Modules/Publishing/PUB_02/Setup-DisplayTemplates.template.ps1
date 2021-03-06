﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-DisplayTemplates.ps1.template
# Description	: Create display templates
# -----------------------------------------------------------------------

Write-Warning "Applying Display Templates configuration..."

# Activate feature on the root web on the publishing site collection
Initialize-DSPFeature -Url [[DSP_PortalPublishingSiteUrl]]  -Id [[DSP_CrossSitePublishingCMS_PUB_DisplayTemplates]]

