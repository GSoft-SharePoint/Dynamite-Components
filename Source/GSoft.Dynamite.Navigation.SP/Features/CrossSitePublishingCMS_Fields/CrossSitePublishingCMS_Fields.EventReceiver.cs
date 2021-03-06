using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Autofac;
using GSoft.Dynamite.Fields;
using GSoft.Dynamite.Helpers;
using GSoft.Dynamite.Logging;
using GSoft.Dynamite.Navigation.Contracts.Configuration;
using GSoft.Dynamite.Security;
using Microsoft.SharePoint;

namespace GSoft.Dynamite.Navigation.SP.Features.CrossSitePublishingCMS_Fields
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    [Guid("ced7a8ec-f023-439e-8202-4eb529363ed9")]
    public class CrossSitePublishingCMS_FieldsEventReceiver : SPFeatureReceiver
    {
        /// <summary>
        /// Creates fields for the navigation module
        /// </summary>
        /// <param name="properties">The event properties</param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;

            if (site != null)
            {
                using (var featureScope = NavigationContainerProxy.BeginFeatureLifetimeScope(properties.Feature))
                {
                    var fieldHelper = featureScope.Resolve<IFieldHelper>();
                    var baseFieldInfoConfig = featureScope.Resolve<INavigationFieldInfoConfig>();
                    var baseFields = baseFieldInfoConfig.Fields;
                    var logger = featureScope.Resolve<ILogger>();

                    using (new Unsafe(site.RootWeb))
                    {
                        // Create fields
                        foreach (BaseFieldInfo field in baseFields)
                        {
                            logger.Info("Creating field {0}", field.InternalName);
                            fieldHelper.EnsureField(site.RootWeb.Fields, field);
                        } 
                    }
                }
            }
        }
    }
}
