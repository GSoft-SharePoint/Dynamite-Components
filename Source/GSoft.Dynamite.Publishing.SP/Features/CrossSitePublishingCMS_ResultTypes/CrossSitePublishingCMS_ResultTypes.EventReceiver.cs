using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Autofac;
using GSoft.Dynamite.Definitions;
using GSoft.Dynamite.Helpers;
using GSoft.Dynamite.Logging;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Configuration.Extensions;
using Microsoft.SharePoint;

namespace GSoft.Dynamite.Publishing.SP.Features.CrossSitePublishingCMS_ResultTypes
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("19aff989-e6ab-4b69-a793-ad0473157ec8")]
    public class CrossSitePublishingCMS_ResultTypesEventReceiver : SPFeatureReceiver
    {
       public override void FeatureActivated(SPFeatureReceiverProperties properties)
       {
            var site = properties.Feature.Parent as SPSite;

            if (site != null)
            {
                using (var featureScope = PublishingContainerProxy.BeginFeatureLifetimeScope(properties.Feature))
                {
                    var logger = featureScope.Resolve<ILogger>();
                    var searchHelper = featureScope.Resolve<SearchHelper>();

                    var baseResultTypeInfoConfig = featureScope.Resolve<IBasePublishingResultTypeInfoConfig>();

                    IList<ResultTypeInfo> resultTypes = baseResultTypeInfoConfig.ResultTypes();

                    // Create base result types
                    foreach (var resultType in resultTypes)
                    {
                        searchHelper.EnsureResultType(site, resultType);
                    }

                    // Check if custom configuration is present
                    ICustomPublishingResultTypeInfoConfig customResultTypeInfoConfig = null;
                    if (featureScope.TryResolve(out customResultTypeInfoConfig))
                    {
                        logger.Info("Custom result types configuration override found!");
                        resultTypes = customResultTypeInfoConfig.ResultTypes();

                        // Create custom result types
                        foreach (var resultType in resultTypes)
                        {
                            searchHelper.EnsureResultType(site, resultType);
                        }
                    }
                    else
                    {
                        logger.Info("No custom result types configuration override found!");

                    }
                }
            }
       }

       public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
       {
           var site = properties.Feature.Parent as SPSite;

           if (site != null)
           {
               using (var featureScope = PublishingContainerProxy.BeginFeatureLifetimeScope(properties.Feature))
               {
                   var logger = featureScope.Resolve<ILogger>();
                   var searchHelper = featureScope.Resolve<SearchHelper>();

                   var baseResultTypInfoConfig = featureScope.Resolve<IBasePublishingResultTypeInfoConfig>();

                   IList<ResultTypeInfo> resultTypes = baseResultTypInfoConfig.ResultTypes();

                   // Delete base result types
                   foreach (var resultType in resultTypes)
                   {
                       searchHelper.DeleteResultType(site, resultType);
                   }

                   // Check if custom configuration is present
                   ICustomPublishingResultTypeInfoConfig customResultTypeInfoConfig = null;
                   if (featureScope.TryResolve(out customResultTypeInfoConfig))
                   {
                       logger.Info("Custom result types configuration override found!");
                       resultTypes = customResultTypeInfoConfig.ResultTypes();

                       // Delete custom result types
                       foreach (var resultType in resultTypes)
                       {
                           searchHelper.DeleteResultType(site, resultType);
                       }
                   }
                   else
                   {
                       logger.Info("No custom result types configuration override found!");

                   }
               }
           }
       }
    }
}
