﻿using System.Collections.Generic;
using System.Linq;
using GSoft.Dynamite.Common.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using GSoft.Dynamite.Search;
using GSoft.Dynamite.Search.Enums;
using GSoft.Dynamite.Taxonomy;

namespace GSoft.Dynamite.Publishing.Core.Configuration
{
    /// <summary>
    /// Configuration for the Faceted Navigation settings. Used with OOTB refinements panel Web Part.
    /// </summary>
    public class PublishingFacetedNavigationInfoConfig : IPublishingFacetedNavigationInfoConfig
    {
        private readonly IPublishingTaxonomyConfig publishingTaxonomyConfig;
        private readonly IConsolidatedManagedPropertyConfig consolidatedManagedPropertyConfig;
        private readonly IPublishingDisplayTemplateInfoConfig publishingDisplayTemplateInfoConfig;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="publishingTaxonomyConfig">The publishing taxonomy configuration.</param>
        /// <param name="consolidatedManagedPropertyConfig">The common managed property configuration.</param>
        /// <param name="publishingDisplayTemplateInfoConfig">The publishing display template information configuration.</param>
        public PublishingFacetedNavigationInfoConfig(
            IPublishingTaxonomyConfig publishingTaxonomyConfig,
            IConsolidatedManagedPropertyConfig consolidatedManagedPropertyConfig,
            IPublishingDisplayTemplateInfoConfig publishingDisplayTemplateInfoConfig)
        {
            this.publishingTaxonomyConfig = publishingTaxonomyConfig;
            this.consolidatedManagedPropertyConfig = consolidatedManagedPropertyConfig;
            this.publishingDisplayTemplateInfoConfig = publishingDisplayTemplateInfoConfig;
        }

        /// <summary>
        /// Property that return all the faceted navigation settings to use in the publishing module
        /// </summary>
        /// <returns>The faceted navigation settings</returns>
        public IList<FacetedNavigationInfo> FacetedNavigationInfos
        {
            get
            {
                return new List<FacetedNavigationInfo>()
                {
                    this.NewsFacetedNavigation
                };
            }
        }

        private FacetedNavigationInfo NewsFacetedNavigation
        {
            get
            {
                var navigationManagedProperty = this.consolidatedManagedPropertyConfig.GetManagedPropertyInfoByName(PublishingManagedPropertyInfos.NavigationText.Name);
                var displayTemplate = this.publishingDisplayTemplateInfoConfig.GetDisplayTemplateInfoByName(PublishingDisplayTemplateInfos.DefaultFilterCategoryRefinement.Name);
                var newsTerm = this.publishingTaxonomyConfig.GetTermById(PublishingTermInfos.News.Id);

                var navigation = new RefinerInfo(navigationManagedProperty.Name, RefinerType.Text, false);
                navigation.DisplayTemplateJsLocation = displayTemplate.ItemTemplateTokenizedPath;

                var created = new RefinerInfo("Created", RefinerType.DateTime, false);

                var refiners = new List<RefinerInfo>()
                {
                    navigation,
                    created,
                };

                return new FacetedNavigationInfo(newsTerm, refiners);
            }
        }

        /// <summary>
        /// Gets the faceted navigation information by term information from this configuration.
        /// </summary>
        /// <param name="term">The term information.</param>
        /// <returns>
        /// The faceted navigation information
        /// </returns>
        public FacetedNavigationInfo GetFacetedNavigationInfoByTerm(TermInfo term)
        {
            return this.FacetedNavigationInfos.Single(f => f.Term.Equals(term));
        }
    }
}
