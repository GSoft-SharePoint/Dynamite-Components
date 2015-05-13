#
# Module 'Dynamite.PowerShell.Toolkit'
# Generated by: GSoft, Team Dynamite.
# Generated on: 01/28/2014
# > GSoft & Dynamite : http://www.gsoft.com
# > Dynamite Github : https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit
# > Documentation : https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit/wiki
#

function Add-DSPUserProfileSection {
    
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        $UserProfileApplication,

        [Parameter(Mandatory=$true, Position=1)]
        [System.Xml.XmlElement]$Sections,

        [Parameter(Mandatory=$false, Position=2)]
        [switch]$Delete
    )    

    Load-DSPUserProfileAssemblies

    $serviceContext = Get-DSPServiceContext $UserProfileApplication
    $userProfileConfigManager = New-Object Microsoft.Office.Server.UserProfiles.UserProfileConfigManager $serviceContext

    if ($Sections -ne $null)
    {    
        foreach ($newSection in $Sections)
        {
            $SectionName = $newSection.Name;
            $SectionDisplayName = $newSection.DisplayName;

            $allEntries = $userProfileConfigManager.GetPropertiesWithSection();

            $sectionExists =$false

            foreach ($temp in $allEntries) 
            {
                if($temp.Name -eq $SectionName) 
                {
                    Write-Verbose "Section $SectionName already exists"
                    $sectionExists = $true;
                    $section = $temp
                }
            }

            # Delete the previous section if specified
            if($section-ne $null)
            {
                Write-Verbose "Removing section $SectionName";
                $allEntries.RemoveSectionByName($SectionName)
                $sectionExists = $false
            }
            else
            {
                Write-Verbose "Section $SectionName doesn't exists"
            }

            if ($sectionExists -ne $true -and $Delete -eq $false)
            {
                #Create new section in User Profiles 
                Write-Verbose "Creating new section $SectionName" 

                $section = $allEntries.Create($true);
                $section.Name = $SectionName;
                $section.ChoiceType = [Microsoft.Office.Server.UserProfiles.ChoiceTypes]::Off;
                $section.DisplayName = $SectionDisplayName
                $section.Commit();
                Write-Verbose "Section $SectionName created!" 
            }
            
            # If localized display names are configured, set them
            if($newSection.DisplayNames -ne $null) {
                $newSection.DisplayNames.DisplayName | ForEach-Object {
                    Write-Verbose "Adding Section $SectionName display name $($_.Value) to LCID $($_.LCID)"
                    $section.DisplayNameLocalized[[int]$_.LCID] = $_.Value;          
                } 
                
                $section.Commit();
            }

            $newSection.UserProperty | ForEach-Object {
                Add-DSPUserProfileProperty $UserProfileApplication $_ $Delete              
            }           
        }
    }    
}


function Add-DSPUserProfileProperty {
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        $UserProfileApplication,

        [Parameter(Mandatory=$true, Position=1)]
        [System.Xml.XmlElement]$Properties,
        
        [Parameter(Mandatory=$false, Position=2)]
        [switch]$Delete
    )    

    Load-DSPUserProfileAssemblies

    $serviceContext = Get-DSPServiceContext $UserProfileApplication
    $userProfileConfigManager = New-Object Microsoft.Office.Server.UserProfiles.UserProfileConfigManager $serviceContext

    $userProfilePropertyManager = $userProfileConfigManager.ProfilePropertyManager
    $userProfilePropertyManager = $userProfilePropertyManager.GetCoreProperties()
    $userProfileTypeProperties = $userProfileConfigManager.ProfilePropertyManager.GetProfileTypeProperties([Microsoft.Office.Server.UserProfiles.ProfileType]::User)
    $userProfileSubTypeManager = [Microsoft.Office.Server.UserProfiles.ProfileSubTypeManager]::Get($serviceContext)

    $userProfile = $userProfileSubTypeManager.GetProfileSubtype([Microsoft.Office.Server.UserProfiles.ProfileSubtypeManager]::GetDefaultProfileName([Microsoft.Office.Server.UserProfiles.ProfileType]::User))
    $userProfileProperties = $userProfile.Properties 

    if ($Properties -ne $null)
    {    
        foreach ($newProperty in $Properties)
        {

            # Remove property if exists

            $UserPropertyName = $newProperty.GeneralSettings.Name
            $userProperty = $userProfilePropertyManager.GetPropertyByName($UserPropertyName)

            if($userProperty -ne $null)
            {
                # Remove in all cases
                Write-Verbose "Removing section $UserPropertyName";
                
                # Remove property mappings for each connection
                $synchConnections = $userProfileConfigManager.ConnectionManager
                foreach ($synchConn in $synchConnections)
                {
                    # Remove Import mappings
                    $prop = $synchConn.PropertyMapping[$UserPropertyName]
                    if( $prop -ne $null)
                    {
                        $prop.Delete()
                    }

                    # Remove Export mappings
                    $synchConn.PropertyMapping.RemoveAllExportMappingsForAttribute($UserPropertyName)

                }
                
                $userProfilePropertyManager.RemovePropertyByName($UserPropertyName)
            }
            else
            {
                Write-Verbose "User Property $UserPropertyName doesn't exists";
            }

            if($Delete -eq $false)
            {
                Write-Verbose "Creating User Property $UserPropertyName";

                $userProperty = $userProfilePropertyManager.Create($false)

                # General Settings

                $userProperty.Name = $UserPropertyName
                $userProperty.DisplayName = $newProperty.GeneralSettings.DisplayName
                $userProperty.Type = $newProperty.GeneralSettings.Type
                $userProperty.Length = $newProperty.GeneralSettings.Length
                $userProperty.IsAlias = [System.Convert]::ToBoolean($newProperty.GeneralSettings.IsAlias)
                $userProperty.IsSearchable = [System.Convert]::ToBoolean($newProperty.GeneralSettings.IsSearchable)
                $userProperty.IsMultivalued = [System.Convert]::ToBoolean($newProperty.GeneralSettings.IsMultivalued)

                # If localized display names are configured, set them
                if($newProperty.DisplayNames -ne $null) {
                    $newProperty.DisplayNames.DisplayName | ForEach-Object {
                        $userProperty.DisplayNameLocalized[[int]$_.LCID] = $_.Value;          
                    } 
                }

                # Taxonomy Settings

                if($newProperty.GeneralSettings.IsMultivalued -eq $true)
                {
                    $Separator =  $newProperty.TaxonomySettings.Separator
                
                    $userProperty.Separator = [Microsoft.Office.Server.UserProfiles.MultiValueSeparator]::$Separator
                }
           
                if($newProperty.TaxonomySettings.TermsetName -ne $null -and $newProperty.TaxonomySettings.TermsetGroup -ne $null)
                {

                    $userProperty.TermSet = Get-DSPTermSet -GroupName $newProperty.TaxonomySettings.TermsetGroup -TermSetName $newProperty.TaxonomySettings.TermsetName
                }

                $userProfilePropertyManager.Add($userProperty)
                $profileTypeProperty = $userProfileTypeProperties.Create($userProperty)

                # Display Settings

                $profileTypeProperty.IsVisibleOnEditor = [System.Convert]::ToBoolean($newProperty.DisplaySettings.IsVisibleOnEditor) 
                $profileTypeProperty.IsVisibleOnViewer = [System.Convert]::ToBoolean($newProperty.DisplaySettings.IsVisibleOnViewer)
                $profileTypeProperty.IsEventLog =[System.Convert]::ToBoolean($newProperty.DisplaySettings.IsEventLog) 


                $userProfileTypeProperties.Add($profileTypeProperty)
                $Privacy = $newProperty.DisplaySettings.Privacy
                $PrivacyPolicy =$newProperty.DisplaySettings.PrivacyPolicy
            
                $profileSubTypeProperty = $userProfileProperties.Create($profileTypeProperty)
                $profileSubTypeProperty.DefaultPrivacy =[Microsoft.Office.Server.UserProfiles.Privacy]::$Privacy
                $profileSubTypeProperty.PrivacyPolicy =    [Microsoft.Office.Server.UserProfiles.PrivacyPolicy]::$PrivacyPolicy
                $userProfileProperties.Add($profileSubTypeProperty)

                $profileTypeProperty.CoreProperty.Commit()
                $profileTypeProperty.Commit();
                
                # Add Mapping for Synchronization
                if($newProperty.MappingSettings -ne $null)
                {
                    $synchConnection = $userProfileConfigManager.ConnectionManager[$newProperty.MappingSettings.ConnectionName]
                    if([System.Convert]::ToBoolean($newProperty.MappingSettings.IsImport))
                    {
                        $synchConnection.PropertyMapping.AddNewMapping([Microsoft.Office.Server.UserProfiles.ProfileType]::User, $UserPropertyName, $newProperty.MappingSettings.MappedAttribute)
                    }
                    else
                    {
                        $synchConnection.PropertyMapping.AddNewExportMapping([Microsoft.Office.Server.UserProfiles.ProfileType]::User, $UserPropertyName, $newProperty.MappingSettings.MappedAttribute)
                    }
                
                }
            }
        }
    }
}

function Get-DSPServiceContext()
{
    [CmdletBinding()]
    Param
    (
        [Parameter(Mandatory=$true, Position=0)]
        $serviceApplication
    )
    
    return [Microsoft.SharePoint.SPServiceContext]::GetContext($serviceApplication.ServiceApplicationProxyGroup, [Microsoft.SharePoint.SPSiteSubscriptionIdentifier]::Default)
}

function Load-DSPUserProfileAssemblies()
{
    #Load SharePoint User Profile assemblies
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.Office.Server") > $null
    [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.Office.Server.UserProfiles") > $null
}

<#
    .SYNOPSIS
        Commandlet to create user profile properties and sections

    .DESCRIPTION
        Add user proeprty in the defaut schema
        Note: If you encounter access denied problems, grant full permissions to the current user to the user profile application.
        Note: Creation and Deletion are not instantaneous in UI

    --------------------------------------------------------------------------------------
    Module 'Dynamite.PowerShell.Toolkit'
    by: GSoft, Team Dynamite.
    > GSoft & Dynamite : http://www.gsoft.com
    > Dynamite Github : https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit
    > Documentation : https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit/wiki
    --------------------------------------------------------------------------------------
   
    .NOTES
         Here is the Structure XML schema.

         <!-- 
            UserProfileApplicationName: User profile application name
         -->
        <Configuration UserProfileApplicationName="User Profile Application">
            <!-- 
                Name: Internal name of the section
                DisplayName: The display name of the section
            -->
            <Section Name="AgropurTargetingSection" DisplayName="Informations">
                <UserProperty>
                    <!-- 
                        Name: Internal name of the property
                        DisplayName: The display name of the property
                        Description: Description of the property
                        Type: Type of the property
                        IsAlias: Value indicating whether this profile core property serves as the alias of the user.
                        IsMultivalued: Specifies if the proerty is multivalued
                        IsSearchable: Value indicating whether values of this profile core property are indexed by search.
                        IsSection: Specifies if the property is a section
                        Length: Length of the property
                    -->
                    <GeneralSettings Name="AgropurLocation" DisplayName="Factory" Description="Employee factory" Type= "string" IsAlias="false" IsMultivalued="false" IsSearchable="true" IsSection="false" Length="50" />
                    <!-- 
                        ConnectionName: Source Data Connection
                        MappedAttribute: Attribute to map with UserProperty for synchronization
                        IsImport: Direction of the mapping. Import if true, else Export.
                    -->
                    <MappingSettings ConnectionName="BCF" MappedAttribute="spUserFloor" IsImport="true" />
                    <!-- 
                        Separator: Value of the separator used by the user interface for user profile core properties that have multiple values.(http://msdn.microsoft.com/en-us/library/microsoft.office.server.userprofiles.multivalueseparator.aspx)
                        TermSetgGroup: Group of the term set
                        TermSetName: TermSet for this profile core property. This is the term set from which taxonomic terms are retrieved and to which they are stored. The keywords term set is used by default when no term set has been specified.
                    -->
                    <TaxonomySettings Separator="" TermSetGroup="Agropur - Profiles" TermSetName="User - Factories" />
                    <!-- 
                        IsEventLog: Value indicating whether changes to this property are returned for change tracking.
                        IsVisibleOnEditor: Value indicating whether this property is visible on the profile editing page.
                        IsVisibleOnViewer: Value indicating whether this property is visible on the default profile viewer page.
                        Privacy: Represents the privacy level that you can set on user profile data. (http://msdn.microsoft.com/en-us/library/microsoft.office.server.userprofiles.privacy.aspx)
                        PrivacyPolicy: Defines the privacy policy for whatever a user is applying to. (http://msdn.microsoft.com/en-us/library/microsoft.office.server.userprofiles.privacypolicy.aspx)
                    -->
                    <DisplaySettings IsEventLog ="false" IsVisibleOnEditor="true" IsVisibleOnViewer="true" Privacy="Public" PrivacyPolicy="Mandatory"/>
                </UserProperty>
            </Section>
        </Configuration>

    .PARAMETER  XmlPath (Mandatory)
        Physical path of the XML configuration file.
        
    .PARAMETER  Delete (Optionnal)
        If true, delete properties configuration from the the user profile schema
    .EXAMPLE
        PS C:\> Set-DSPUserProfileSchema "D:\Data.xml" -Delete
        PS C:\> Set-DSPUserProfileSchema

    .OUTPUTS
        n/a. 
    
  .LINK
    GSoft, Team Dynamite on Github
    > https://github.com/GSoft-SharePoint
    
    Dynamite PowerShell Toolkit on Github
    > https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit
    
    Documentation
    > https://github.com/GSoft-SharePoint/Dynamite-PowerShell-Toolkit/wiki
    
#>
function Set-DSPUserProfileSchema
{
    [CmdletBinding()]
    Param
    (
        [Parameter(ParameterSetName="Default", Mandatory=$true, Position=0)]
        [string]$XmlPath,

        [Parameter(Mandatory=$false, Position=1)]
        [switch]$Delete
    )

    [xml]$xmlContent = Get-Content $XmlPath

    if($xmlContent -ne $null)
    {
        $serviceApplication = Get-SPServiceApplication -Name $xmlContent.Configuration.UserProfileApplicationName
         
        Add-DSPUserProfileSection $serviceApplication $xmlContent.Configuration.Section $Delete
    }
   
}
