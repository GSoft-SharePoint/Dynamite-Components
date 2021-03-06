﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-WebParts.ps1.template
# Description	: Create Web Parts
# -----------------------------------------------------------------------

Write-Warning "Applying Web Parts configuration..."

# Activate feature on the root web on the publishing site collection
Initialize-DSPFeature -Url [[DSP_PortalPublishingSiteUrl]]  -Id [[DSP_CommonCMS_PUB_WebParts]]