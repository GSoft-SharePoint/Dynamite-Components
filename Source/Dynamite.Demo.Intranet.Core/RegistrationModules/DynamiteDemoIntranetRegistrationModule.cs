﻿using Autofac;
using Dynamite.Demo.Intranet.Contracts.Constants;
using Dynamite.Demo.Intranet.Core.Configuration;
using Dynamite.Demo.Intranet.Core.Resources;
using GSoft.Dynamite.Globalization;
using GSoft.Dynamite.Publishing.Contracts.Configuration.Extensions;

namespace Dynamite.Demo.Intranet.Core.RegistrationModules
{
    public class DynamiteDemoIntranetRegistrationModule : Module
    {
        /// <summary>
        /// Registers the modules type bindings
        /// </summary>
        /// <param name="builder">
        /// The container builder.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            // Dynamite base config overrides
            builder.RegisterType<DynamiteDemoResourceLocatorConfig>().As<IResourceLocatorConfig>();

            // Overridable default Portal config (i.e. plugin hooks and slots, with sensible defaults)
            builder.RegisterType<DynamiteDemoPublishingContentTypeInfoConfig>().As<ICustomPublishingContentTypeInfoConfig>();
            builder.RegisterType<DynamiteDemoPublishingFieldInfoConfig>().As<ICustomPublishingFieldInfoConfig>();
            builder.RegisterType<DynamiteDemoPublishingCatalogInfoConfig>().As<ICustomPublishingCatalogInfoConfig>();
            
            // Configuration Values
            builder.RegisterType<DynamiteDemoPublishingFieldInfoValues>();
            builder.RegisterType<DynamiteDemoPublishingContentTypeInfoValues>();
            builder.RegisterType<DynamiteDemoPublishingCatalogInfoValues>(); 
        }
    }
}