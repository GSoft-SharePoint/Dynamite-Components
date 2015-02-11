﻿# GSoft.Dynamite.Models.CrossSitePublishingCMS integration tests runner

# You must add Pester to your Modules folder and init script (see https://github.com/pester/Pester)
Import-Module Pester

# Make sure the current directory is the current file's parent folder
$path = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $path
Invoke-Pester


