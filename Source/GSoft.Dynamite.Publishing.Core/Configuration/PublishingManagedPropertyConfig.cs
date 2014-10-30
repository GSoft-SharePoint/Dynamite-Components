﻿using System.Collections.Generic;
using GSoft.Dynamite.Definitions;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Constants;

namespace GSoft.Dynamite.Publishing.Core.Configuration
{
    public class PublishingManagedPropertyConfig : IGlobalManagedPropertyInfosConfig
    {
        private readonly PublishingManagedPropertyInfos publishingManagedPropertyInfos;

        public PublishingManagedPropertyConfig(PublishingManagedPropertyInfos publishingManagedPropertyInfos)
        {
            this.publishingManagedPropertyInfos = publishingManagedPropertyInfos;
        }

        public IList<ManagedPropertyInfo> ManagedProperties
        {
            get
            {
                var managedProperties = new List<ManagedPropertyInfo>()
                {
                    {this.publishingManagedPropertyInfos.Navigation}
                };

                return managedProperties;
            }         
        }
    }
}