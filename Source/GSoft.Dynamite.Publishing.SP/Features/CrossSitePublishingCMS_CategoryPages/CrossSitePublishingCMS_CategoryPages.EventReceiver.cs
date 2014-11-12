using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Autofac;
using GSoft.Dynamite.Extensions;
using GSoft.Dynamite.Folders;
using GSoft.Dynamite.Logging;
using GSoft.Dynamite.Publishing.Contracts.Configuration;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;

namespace GSoft.Dynamite.Publishing.SP.Features.CrossSitePublishingCMS_CategoryPages
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("394329fa-e9bb-4be1-b1d4-15ea6930b95d")]
    public class CrossSitePublishingCMS_CategoryPagesEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var web = properties.Feature.Parent as SPWeb;

            using (var featureScope = PublishingContainerProxy.BeginFeatureLifetimeScope(properties.Feature))
            {
                var logger = featureScope.Resolve<ILogger>();

                if (web != null && PublishingWeb.IsPublishingWeb(web))
                {
                    var folderHelper = featureScope.Resolve<IFolderHelper>();

                    var baseFoldersConfig = featureScope.Resolve<IPublishingFolderInfoConfig>();
                    var publishingFolderInfos = featureScope.Resolve<PublishingFolderInfos>();
                    var folders = baseFoldersConfig.RootFolderHierarchies().ToList();

                    // Remove Category Page folder
                    folders.RemoveAll(f => f.Name.Equals(publishingFolderInfos.ItemPageTemplates().Name));

                    foreach (var rootFolderHierarchy in folders)
                    {
                        var pagesLibrary = web.GetPagesLibrary();

                        if (rootFolderHierarchy.Locale != null)
                        {
                            if (web.Locale.TwoLetterISOLanguageName == rootFolderHierarchy.Locale.TwoLetterISOLanguageName)
                            {
                                // Create folder hierarchy starting by the root folder
                                // NOTE: All pages are created through folders hierachy
                                folderHelper.EnsureFolderHierarchy(pagesLibrary, rootFolderHierarchy);
                            }
                            else
                            {
                                logger.Info("The root folder {0} does not apply on this web according to its locale {1}", rootFolderHierarchy.Name, rootFolderHierarchy.Locale.TwoLetterISOLanguageName);
                            }
                        }
                        else
                        {
                            folderHelper.EnsureFolderHierarchy(pagesLibrary, rootFolderHierarchy);
                        }
                    }
                }
                else
                {
                    logger.Warn("The web {0} is not a Publishing Web. This feature can only be activated on a SharePoint Publishing Web", web.Url);
                }
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var web = properties.Feature.Parent as SPWeb;

            using (var featureScope = PublishingContainerProxy.BeginFeatureLifetimeScope(properties.Feature))
            {
                var logger = featureScope.Resolve<ILogger>();

                if (web != null && PublishingWeb.IsPublishingWeb(web))
                {
                    var folderHelper = featureScope.Resolve<IFolderHelper>();
                    var baseFoldersConfig = featureScope.Resolve<IPublishingFolderInfoConfig>();

                    folderHelper.ResetWelcomePageToDefault(web);
                }
            }
            // Reset HomePage
        }
    }
}