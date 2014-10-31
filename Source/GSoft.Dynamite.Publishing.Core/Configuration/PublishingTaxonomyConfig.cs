﻿using System.Collections.Generic;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using GSoft.Dynamite.Taxonomy;

namespace GSoft.Dynamite.Publishing.Core.Configuration
{
    public class PublishingTaxonomyConfig : IPublishingTaxonomyConfig
    {
        private readonly PublishingTermGroupInfos _termGroupInfoValues;

        public PublishingTaxonomyConfig(PublishingTermGroupInfos termGroupInfoValues)
        {
            _termGroupInfoValues = termGroupInfoValues;
        }

        public IList<TermGroupInfo> TermGroups()
        {
            var termGroups = new List<TermGroupInfo>
            {
                {_termGroupInfoValues.Navigation()}
            };

            return termGroups;
        }
    }
}
