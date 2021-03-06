﻿using System;
using Autofac;
using GSoft.Dynamite.Globalization.Variations;
using GSoft.Dynamite.Logging;
using GSoft.Dynamite.Navigation.Contracts.Services;
using GSoft.Dynamite.Publishing.Contracts.Constants;
using GSoft.Dynamite.Taxonomy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace GSoft.Dynamite.Navigation.SP.Events
{
    /// <summary>
    /// Defines events for the target page content type
    /// </summary>
    public class TargetContentPageEvents : SPItemEventReceiver
    {
        // Even though these events are ASYNC and we typically shouldn't care about their sequence,
        // we still care about avoiding overlap between multiple threads processing these events in parallel.
        // We use this lock to ensure these events are always executed in a sequence instead of simultaneously.
        private static object eventLock = new object();

        /// <summary>
        /// Asynchronous After event that occurs after a new item has been added to its containing object.
        /// Be careful, on a document library like pages library, ItemAdded NOT FIRING on the "New document" option on the ribbon. Use ItemUpdated instead
        /// </summary>
        /// <param name="properties">Event properties</param>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            lock (eventLock)
            {
                base.ItemAdded(properties);

                using (var childScope = NavigationContainerProxy.BeginWebLifetimeScope(properties.Web))
                {
                    this.EventFiringEnabled = false;
                    var logger = childScope.Resolve<ILogger>();

                    try
                    {
                        var navigationTermService = childScope.Resolve<INavigationTermBuilderService>();
                        var variationHelper = childScope.Resolve<IVariationHelper>();

                        var item = properties.ListItem;

                        if (item != null)
                        {
                            // Set Term driven page
                            navigationTermService.SetTermDrivenPageForTerm(item);

                            if (variationHelper.IsCurrentWebSourceLabel(item.Web))
                            {
                                // Create term in other term sets
                                navigationTermService.SyncNavigationTerm(properties.ListItem);
                            }
                        }
                        else
                        {
                            logger.Warn("TargetContentPageEvents.ItemAdded: failed because of NULL properties.ListItem instance.");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e);
                    }
                    finally
                    {
                        this.EventFiringEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronous After event that occurs after an existing item is changed, for example, when the user changes data in one or more fields.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties" /> object that represents properties of the event handler.</param>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            lock (eventLock)
            {
                base.ItemUpdated(properties);

                using (var childScope = NavigationContainerProxy.BeginWebLifetimeScope(properties.Web))
                {
                    this.EventFiringEnabled = false;
                    var logger = childScope.Resolve<ILogger>();

                    try
                    {
                        var navigationTermService = childScope.Resolve<INavigationTermBuilderService>();
                        var variationHelper = childScope.Resolve<IVariationHelper>();
                        var navigationHelper = childScope.Resolve<INavigationHelper>();

                        var item = properties.ListItem;

                        if (item != null)
                        {
                            // Be careful, if a permission error occurs, check your Farm Account configuration in the Security section in the Central Administration.
                            // The current user shouldn't be present in the Farm Account component. (Configure Service Accounts), should be dev\spsfarm
                            // Set Term driven page
                            navigationTermService.SetTermDrivenPageForTerm(item);

                            if (variationHelper.IsCurrentWebSourceLabel(item.Web))
                            {
                                // Create term in other term sets
                                navigationTermService.SyncNavigationTerm(item);
                            }

                            if (properties.BeforeProperties != null && properties.AfterProperties != null)
                            {
                                // Get Navigation value. It can be null if never setted.
                                var beforeNavigationValue = properties.BeforeProperties[PublishingFieldInfos.Navigation.InternalName];
                                var afterNavigationValue = properties.AfterProperties[PublishingFieldInfos.Navigation.InternalName];

                                // Get the TaxonomyFieldValue, if the Navigation was never set, simply set it as null
                                var before = (beforeNavigationValue != null && !string.IsNullOrEmpty(beforeNavigationValue.ToString())) ? new TaxonomyFieldValue(beforeNavigationValue.ToString()) : null;
                                var after = (afterNavigationValue != null && !string.IsNullOrEmpty(afterNavigationValue.ToString())) ? new TaxonomyFieldValue(afterNavigationValue.ToString()) : null;

                                // Reset the previous term if different from the current term
                                if (before != null && after != null && after.TermGuid != before.TermGuid)
                                {
                                    navigationHelper.ResetTermDrivenPageToSimpleLinkUrl(item.Web, new TermInfo() { Id = new Guid(before.TermGuid) });
                                }
                            }
                        }
                        else
                        {
                            logger.Error("TargetContentPageEvents.ItemUpdated failed because of NULL properties.ListItem instance.");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Exception(e);
                    }
                    finally
                    {
                        this.EventFiringEnabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Synchronous event that occurs as an existing item is being deleted.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties" /> object that represents properties of the event handler.</param>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            lock (eventLock)
            {
                base.ItemDeleting(properties);

                using (var childScope = NavigationContainerProxy.BeginWebLifetimeScope(properties.Web))
                {
                    var logger = childScope.Resolve<ILogger>();
                    var variationHelper = childScope.Resolve<IVariationHelper>();
                    var navigationTermService = childScope.Resolve<INavigationTermBuilderService>();
                    var item = properties.ListItem;

                    if (item != null)
                    {
                        if (variationHelper.IsCurrentWebSourceLabel(item.Web))
                        {
                            logger.Info("Attempting to delete navigation term associated to page at URL {0} was deleted. OOTB implementation of friendly URL management tends to cause Orphaned Terms in some term reuse scenarios. We will attempt to pre-empt this when possible.", properties.ListItem.Url);
                            navigationTermService.DeleteAssociatedPageTerm(item);
                        }
                    }
                }
            }
        }
    }
}
