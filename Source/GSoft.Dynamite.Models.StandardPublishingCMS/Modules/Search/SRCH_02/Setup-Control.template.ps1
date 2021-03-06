﻿# -----------------------------------------------------------------------
# Copyright		: GSoft @2014
# Model  		: Cross Site Publishing CMS
# File          : Setup-Control.template.ps1
# Description	: Add the control containing the Google Id Tracker in the additional page head
# -----------------------------------------------------------------------

Write-Warning "Applying Control configuration..."

$DefaultGoogleTrackerFeatureId = "[[DSP_CrossSitePublishingCMS_SRCH_GoogleAnalyticsTracking]]"

if(![string]::IsNullOrEmpty($DefaultGoogleTrackerFeatureId))
{
    Initialize-DSPFeature -Url [[DSP_PortalPublishingSiteUrl]]  -Id $DefaultGoogleTrackerFeatureId
}
