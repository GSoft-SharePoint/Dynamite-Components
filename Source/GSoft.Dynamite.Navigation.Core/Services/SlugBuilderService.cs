﻿using System;
using System.Globalization;
using GSoft.Dynamite.Logging;
using GSoft.Dynamite.Navigation.Contracts.Constants;
using GSoft.Dynamite.Navigation.Contracts.Services;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;

namespace GSoft.Dynamite.Navigation.Core.Services
{
    /// <summary>
    /// The service for the URL slugs generation
    /// </summary>
    public class SlugBuilderService : ISlugBuilderService
    {
        private readonly ILogger _logger;
        private readonly INavigationHelper _navigationHelper;
        private readonly NavigationFieldInfos _navigationFieldInfos;
        private readonly PublishingFieldInfos publishingFieldInfos;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">The logger</param>
        /// <param name="navigationHelper">The navigation helper</param>
        /// <param name="navigationFieldInfos">The field info objects configuration</param>
        /// <param name="publishingFieldInfos">The publishing field information.</param>
        public SlugBuilderService(
            ILogger logger, 
            INavigationHelper navigationHelper, 
            NavigationFieldInfos navigationFieldInfos,
            PublishingFieldInfos publishingFieldInfos)
        {
            this._logger = logger;
            this._navigationHelper = navigationHelper;
            this._navigationFieldInfos = navigationFieldInfos;
            this.publishingFieldInfos = publishingFieldInfos;
        }

        /// <summary>
        /// Sets the friendly URL slug.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetFriendlyUrlSlug(SPListItem item)
        {
            var itemDateFieldName = this.publishingFieldInfos.PublishingStartDate().InternalName;
            var dateSlugFieldName = this._navigationFieldInfos.DateSlug().InternalName;
            var titleSlugFieldName = this._navigationFieldInfos.TitleSlug().InternalName;

            // Generate title slug
            if (item.Fields.ContainsField(titleSlugFieldName))
            {
                item[titleSlugFieldName] = this._navigationHelper.GenerateFriendlyUrlSlug(item.Title);
                this._logger.Info(
                    "ContentAssociation.SetFriendlyUrlSlug: Set title slug '{0}' on item '{1}' in web '{2}'.",
                    item[titleSlugFieldName],
                    item.Title,
                    item.Web.Url);
            }

            // Generate date slug
            if (item.Fields.ContainsField(dateSlugFieldName) && item.Fields.ContainsField(itemDateFieldName))
            {
                item[dateSlugFieldName] = GetFriendlyUrlDate(item, itemDateFieldName);
                this._logger.Info(
                    "ContentAssociation.SetFriendlyUrlSlug: Set date slug '{0}' on item '{1}' in web '{2}'.",
                    item[dateSlugFieldName],
                    item.Title,
                    item.Web.Url);
            }

            item.SystemUpdate();
        }

        private static string GetFriendlyUrlDate(SPListItem item, string fieldName)
        {
            var dateFieldValue = item[fieldName];
            var date = (DateTime)dateFieldValue;
            var publishingWeb = PublishingWeb.GetPublishingWeb(item.Web);
            CultureInfo culture;

            // If the site has a variation label
            if (publishingWeb.Label != null)
            {
                var language = publishingWeb.Label.Language;
                culture = new CultureInfo(language);
            }
            else
            {
                culture = publishingWeb.Web.Locale;
            }

            return date.ToString("d", culture).Replace('/', '-');
        }
    }
}
