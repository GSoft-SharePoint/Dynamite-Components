﻿using System;
using System.Collections.Generic;
using System.Linq;
using GSoft.Dynamite.Common.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using GSoft.Dynamite.Search;

namespace GSoft.Dynamite.Publishing.Core.Configuration
{
    /// <summary>
    /// Configuration for the creation of the Result Types
    /// </summary>
    public class PublishingResultTypeInfoConfig : IPublishingResultTypeInfoConfig
    {
        private readonly IPublishingDisplayTemplateInfoConfig publishingDisplayTemplateInfoConfig;
        private readonly IPublishingResultSourceInfoConfig publishingResultSourceInfoConfig;
        private readonly IConsolidatedManagedPropertyConfig consolidatedManagedPropertyConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishingResultTypeInfoConfig"/> class.
        /// </summary>
        /// <param name="publishingDisplayTemplateInfoConfig">The publishing display template information configuration.</param>
        /// <param name="publishingResultSourceInfoConfig">The publishing result source information configuration.</param>
        /// <param name="consolidatedManagedPropertyConfig">The publishing managed property information configuration.</param>
        public PublishingResultTypeInfoConfig(
            IPublishingDisplayTemplateInfoConfig publishingDisplayTemplateInfoConfig,
            IPublishingResultSourceInfoConfig publishingResultSourceInfoConfig,
            IConsolidatedManagedPropertyConfig consolidatedManagedPropertyConfig)
        {
            this.publishingDisplayTemplateInfoConfig = publishingDisplayTemplateInfoConfig;
            this.publishingResultSourceInfoConfig = publishingResultSourceInfoConfig;
            this.consolidatedManagedPropertyConfig = consolidatedManagedPropertyConfig;
        }

        /// <summary>
        /// Property that return all the result types to create in the publishing module
        /// </summary>
        public IList<ResultTypeInfo> ResultTypes
        {
            get
            {
                var resultTypes = new List<ResultTypeInfo>()
                {
                    this.NewsPageResultType,
                    this.ContentPageResultType,
                    this.CategoryItemResultType
                };

                return resultTypes;               
            }         
        }

        private ResultTypeInfo NewsPageResultType
        {
            get
            {
                var resultType = PublishingResultTypeInfos.NewsPageResultType;
                resultType.DisplayTemplate = this.publishingDisplayTemplateInfoConfig.GetDisplayTemplateInfoByName(PublishingDisplayTemplateInfos.ItemSingleNewsItemContent.Name);
                resultType.ResultSource = this.publishingResultSourceInfoConfig.GetResultSourceInfoByName(PublishingResultSourceInfos.SingleCatalogItem.Name);

                return resultType;
            }
        }

        private ResultTypeInfo ContentPageResultType
        {
            get
            {
                var resultType = PublishingResultTypeInfos.ContentPageResultType;
                resultType.DisplayTemplate = this.publishingDisplayTemplateInfoConfig.GetDisplayTemplateInfoByName(PublishingDisplayTemplateInfos.ItemSingleContentItem.Name);
                resultType.ResultSource = this.publishingResultSourceInfoConfig.GetResultSourceInfoByName(PublishingResultSourceInfos.SingleTargetItem.Name);

                return resultType;
            }
        }

        private ResultTypeInfo CategoryItemResultType
        {
            get
            {
                var resultType = PublishingResultTypeInfos.ContentPageResultType;
                resultType.DisplayTemplate = this.publishingDisplayTemplateInfoConfig.GetDisplayTemplateInfoByName(PublishingDisplayTemplateInfos.ItemNewsCategoryItem.Name);
                resultType.ResultSource = this.publishingResultSourceInfoConfig.GetResultSourceInfoByName(PublishingResultSourceInfos.CatalogCategoryItems.Name);
                resultType.DisplayProperties.Add(this.consolidatedManagedPropertyConfig.GetManagedPropertyInfoByName(PublishingManagedPropertyInfos.Summary.Name));

                return resultType;
            }
        }

        /// <summary>
        /// Gets the result type information by name from this configuration.
        /// </summary>
        /// <param name="name">The name of the Result Type.</param>
        /// <returns>
        /// The result type information
        /// </returns>
        public ResultTypeInfo GetResultTypeInfoByName(string name)
        {
            return this.ResultTypes.Single(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
