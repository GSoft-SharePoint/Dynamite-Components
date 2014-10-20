﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GSoft.Dynamite.Binding;
using GSoft.Dynamite.Definitions;
using GSoft.Dynamite.Publishing.Contracts.Constants;

namespace GSoft.Dynamite.Multilingualism.Contracts.Constants
{
    /// <summary>
    /// Field infos for Multilingualism module
    /// </summary>
    public class BaseMultilingualismFieldInfos
    {
        private static readonly string ContentAssociationKeyFieldName = BasePublishingFieldInfos.FieldPrefix + "ContentAssociationKey";
        private static readonly string ItemLanguageFieldName = BasePublishingFieldInfos.FieldPrefix + "ItemLanguage";

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseMultilingualismFieldInfos()
        {
        }

        /// <summary>
        /// The ContentAssociationKey field information
        /// </summary>
        /// <returns>The ContentAssociationKey field</returns>
        public GuidFieldInfo ContentAssociationKey()
        {
            return new GuidFieldInfo(
                ContentAssociationKeyFieldName,
                new Guid("{71154E23-E1A9-48B7-8DC7-556F6F76E4EB}"),
                BaseMultilingualismResources.FieldContentAssociationKeyName,
                BaseMultilingualismResources.FieldContentAssociationKeyDescription,
                BasePublishingResources.FieldGroup
                )
            {
                Required = RequiredTypes.NotRequired,
                IsHiddenInDisplayForm = true,
                IsHiddenInEditForm = true,
                IsHiddenInListSettings = true,
                IsHiddenInNewForm = true
            };
        }

        /// <summary>
        /// The Item Language field information
        /// </summary>
        /// <returns>The ContentAssociationKey field</returns>
        public GuidFieldInfo ItemLanguage()
        {
            return new GuidFieldInfo(
                ItemLanguageFieldName,
                new Guid("{75DC379D-2A78-4EC0-A95B-CFE04AF6631E}"),
                BaseMultilingualismResources.FieldItemLanguageName,
                BaseMultilingualismResources.FieldItemLanguageDescription,
                BasePublishingResources.FieldGroup
                )
            {
                Required = RequiredTypes.NotRequired,
                IsHiddenInDisplayForm = true,
                IsHiddenInEditForm = true,
                IsHiddenInListSettings = true,
                IsHiddenInNewForm = true
            };
        }
    }
}
