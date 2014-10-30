﻿using Autofac;
using GSoft.Dynamite.Docs.Contracts.Configuration;
using GSoft.Dynamite.Docs.Contracts.Constants;
using GSoft.Dynamite.Docs.Contracts.Resources;
using GSoft.Dynamite.Docs.Core.Configuration;
using GSoft.Dynamite.Globalization;
using GSoft.Dynamite.Publishing.Contracts.Configuration;

namespace GSoft.Dynamite.Docs.Core.RegistrationModules
{
    public class DocsRegistrationModule : Module
    {
        /// <summary>
        /// Registers the modules type bindings
        /// </summary>
        /// <param name="builder">
        /// The container builder.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            // Resource locator
            builder.RegisterType<DocsResourceLocatorConfig>().As<IResourceLocatorConfig>();

            // Configuration Values
            builder.RegisterType<DocsFieldInfos>();

            // Fields Configuration
            builder.RegisterType<DocsFieldInfoConfig>().As<IDocsFieldInfoConfig>();

            // Content Types Configuration
            builder.RegisterType<DocsContentTypeInfoConfig>().As<IDocsContentTypeInfoConfig>();

            // Managed Properties
            builder.RegisterType<DocsManagedPropertyInfosConfig>().As<IDocsManagedPropertyInfoConfig>();
        }
    }
}