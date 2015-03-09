﻿using System;
using System.Web.Script.Serialization;
using System.Web.UI;
using Autofac;
using GSoft.Dynamite.Multilingualism.Contracts.Constants;
using GSoft.Dynamite.Navigation.Contracts.Constants;
using GSoft.Dynamite.Navigation.Contracts.Services;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using GSoft.Dynamite.Search;
using Microsoft.SharePoint;

namespace GSoft.Dynamite.Navigation.SP.CONTROLTEMPLATES.GSoft.Dynamite.Navigation
{
    /// <summary>
    /// Code behind of the main menu user control
    /// </summary>
    public partial class MainMenu : UserControl
    {
        /// <summary>
        /// The raw row for the main menu
        /// </summary>
        public string MenuJson { get; set; }

        /// <summary>
        /// Loads the data in the page
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var serializer = new JavaScriptSerializer();

            using (var scope = NavigationContainerProxy.BeginWebLifetimeScope(SPContext.Current.Web))
            {
                var publishingManagedPropertyInfos = scope.Resolve<PublishingManagedPropertyInfos>();
                var navigationManagedPropertyInfos = scope.Resolve<NavigationManagedPropertyInfos>();
                var multilingualismManagedPropertyInfos = scope.Resolve<MultilingualismManagedPropertyInfos>();
                var publishingContentTypeInfos = scope.Resolve<PublishingContentTypeInfos>();
                var dynamiteNavigationService = scope.Resolve<IDynamiteNavigationService>();
                var navigationResultSourceInfos = scope.Resolve<NavigationResultSourceInfos>();

                // Creates the properties object
                var properties = new NavigationManagedProperties()
                 {
                     Title = BuiltInManagedProperties.Title,
                     ResultSourceName = navigationResultSourceInfos.AllMenuItems().Name,
                     Navigation = publishingManagedPropertyInfos.Navigation.Name,
                     ItemLanguage = multilingualismManagedPropertyInfos.ItemLanguage.Name,
                     CatalogItemContentTypeId = publishingContentTypeInfos.CatalogContentItem().ContentTypeId,
                     TargetItemContentTypeId = publishingContentTypeInfos.TargetContentItem().ContentTypeId,
                     FilterManagedPropertyName = navigationManagedPropertyInfos.OccurrenceLinkLocationManagedPropertyText.Name,
                     FilterManagedPropertyValue = "Main Menu",
                     FriendlyUrlRequiredProperties = new[] 
                     { 
                         publishingManagedPropertyInfos.Navigation.Name, 
                         BuiltInManagedProperties.Url, 
                         BuiltInManagedProperties.SiteUrl, 
                         BuiltInManagedProperties.ListId                                           
                     },
                 };

                // Call the navigation service
                var navigationData = dynamiteNavigationService.GetMenuNodes(SPContext.Current.Web, properties, 12);

                // Serializes the data
                this.MenuJson = serializer.Serialize(navigationData);
            }
        }
    }
}