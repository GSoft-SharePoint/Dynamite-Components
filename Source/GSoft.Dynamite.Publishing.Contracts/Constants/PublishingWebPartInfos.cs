﻿using System.Web.UI.WebControls.WebParts;
using GSoft.Dynamite.Helpers;
using GSoft.Dynamite.WebParts;
using Microsoft.Office.Server.Search.WebControls;

namespace GSoft.Dynamite.Publishing.Contracts.Constants
{
    /// <summary>
    /// Class to create the Basic Web Parts
    /// </summary>
    public class PublishingWebPartInfos
    {
        private readonly PublishingResultSourceInfos resultSourceInfos;
        private readonly PublishingDisplayTemplateInfos displayTemplateInfos;
        private readonly IWebPartHelper webPartHelper;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="webPartHelper">The WebPart helper</param>
        /// <param name="resultSourceInfos">The Result Source infos</param>
        /// <param name="displayTemplateInfos">The display templates infos</param>
        public PublishingWebPartInfos(IWebPartHelper webPartHelper, PublishingResultSourceInfos resultSourceInfos, PublishingDisplayTemplateInfos displayTemplateInfos)
        {
            this.webPartHelper = webPartHelper;
            this.resultSourceInfos = resultSourceInfos;
            this.displayTemplateInfos = displayTemplateInfos;
        }

        /// <summary>
        /// Web part for Item Content
        /// </summary>
        /// <returns>WebPart info object</returns>
        public WebPartInfo TargetItemContentWebPart(string zoneName)
        {
            // When you set a result source to a Search WebPart, you need to use at least the Properties["SourceName"] and Properties["SourceLevel"] attributes
            var querySettings = new DataProviderScriptWebPart();
            querySettings.Properties["SourceName"] = this.resultSourceInfos.SingleTargetItem().Name;
            querySettings.Properties["SourceLevel"] = this.resultSourceInfos.SingleTargetItem().Level.ToString();
            querySettings.Properties["QueryTemplate"] = string.Empty;


            return new WebPartInfo("Target Item Content Webpart", zoneName)
            {
                WebPart = new ResultScriptWebPart()
                {
                    DataProviderJSON = querySettings.PropertiesJson,
                    // To set a display template manually, use this line
                    //ItemBodyTemplateId = this._displayTemplateInfos.ItemSingleContentItem().ItemTemplateIdUrl,
                    ChromeType = PartChromeType.None,
                    ShowAdvancedLink = false,
                    ShowBestBets = false,
                    ShowAlertMe = false,
                    ShowLanguageOptions = false,
                    ShowDidYouMean = false,
                    ShowPaging = false,
                    ShowResultCount = false,
                    ShowPersonalFavorites = false,
                    ShowSortOptions = false,
                    ShowViewDuplicates = false,
                    ShowDataErrors = false,
                    ShowDefinitions = false,
                    ShowPreferencesLink = false,
                    ShowUpScopeMessage = false,
                    ShowResults = true,
                    BypassResultTypes = false,
                    ResultsPerPage = 1
                }
            };
        }

        public WebPartInfo CatalogItemContentWebPart(string zoneName)
        {
            var querySettings = new DataProviderScriptWebPart();
            querySettings.Properties["SourceName"] = this.resultSourceInfos.SingleCatalogItem().Name;
            querySettings.Properties["SourceLevel"] = this.resultSourceInfos.SingleCatalogItem().Level.ToString();
            querySettings.Properties["QueryTemplate"] = string.Empty;


            return new WebPartInfo("Catalog Item Content Webpart", zoneName)
            {
                WebPart = new ResultScriptWebPart()
                {
                    DataProviderJSON = querySettings.PropertiesJson,
                    //ItemBodyTemplateId = this._displayTemplateInfos.ItemSingleContentItem().ItemTemplateIdUrl,
                    ChromeType = PartChromeType.None,
                    ShowAdvancedLink = false,
                    ShowBestBets = false,
                    ShowAlertMe = false,
                    ShowLanguageOptions = false,
                    ShowDidYouMean = false,
                    ShowPaging = false,
                    ShowResultCount = false,
                    ShowPersonalFavorites = false,
                    ShowSortOptions = false,
                    ShowViewDuplicates = false,
                    ShowDataErrors = false,
                    ShowDefinitions = false,
                    ShowPreferencesLink = false,
                    ShowUpScopeMessage = false,
                    ShowResults = true,
                    BypassResultTypes = false,
                    ResultsPerPage = 1
                }
            };
        }

        /// <summary>
        /// Create a Place Holder webpartinfo
        /// </summary>
        /// <param name="x">Horizontal dimension in pixel</param>
        /// <param name="y">Vertical dimension in pixel</param>
        /// <param name="backgroundColor">Background color in hex ex: <c>"ffffff"</c> or <c>"e3b489"</c></param>
        /// <param name="fontColor">font color in hex ex: <c>"ffffff"</c> or <c>"e3b489"</c></param>
        /// <returns>A webpartinfo containing the webpart</returns>
        public WebPartInfo PlaceHolder(string zoneName, int x, int y, string backgroundColor, string fontColor)
        {
            return new WebPartInfo("Place Holder", zoneName)
            {
                WebPart = this.webPartHelper.CreatePlaceholderWebPart(x, y, backgroundColor, fontColor)
            };
        }

        public WebPartInfo CatalogCategoryItemsMainWebPart(string zoneName)
        {
            var querySettings = new DataProviderScriptWebPart();
            querySettings.Properties["SourceName"] = this.resultSourceInfos.CatalogCategoryItems().Name;
            querySettings.Properties["SourceLevel"] = this.resultSourceInfos.CatalogCategoryItems().Level.ToString();
            querySettings.Properties["QueryTemplate"] = string.Empty;


            return new WebPartInfo("Catalog Category Items Main Content Webpart", zoneName)
            {
                WebPart = new ResultScriptWebPart()
                {
                    DataProviderJSON = querySettings.PropertiesJson,
                    //ItemBodyTemplateId = this._displayTemplateInfos.ItemSingleContentItem().ItemTemplateIdUrl,
                    ChromeType = PartChromeType.None,
                    ShowAdvancedLink = false,
                    ShowBestBets = false,
                    ShowAlertMe = false,
                    ShowLanguageOptions = false,
                    ShowDidYouMean = false,
                    ShowPaging = true,
                    ShowResultCount = false,
                    ShowPersonalFavorites = false,
                    ShowSortOptions = false,
                    ShowViewDuplicates = false,
                    ShowDataErrors = false,
                    ShowDefinitions = false,
                    ShowPreferencesLink = false,
                    ShowUpScopeMessage = false,
                    ShowResults = true,
                    BypassResultTypes = false,
                    ResultsPerPage = 20,
                    QueryGroupName = "CatalogCategoryItems"
                }
            };
        }

        public WebPartInfo CatalogCategoryRefinementWepart(string zoneName)
        {
            return new WebPartInfo("Catalog Category Items Refinement Content Webpart", zoneName)
            {
                WebPart = new RefinementScriptWebPart()
                {
                    UseManagedNavigationRefiners = true,
                    ChromeType = PartChromeType.None,               
                    ShowDataErrors = false,
                    QueryGroupName = "CatalogCategoryItems"
                }
            };
        }
    }
}
