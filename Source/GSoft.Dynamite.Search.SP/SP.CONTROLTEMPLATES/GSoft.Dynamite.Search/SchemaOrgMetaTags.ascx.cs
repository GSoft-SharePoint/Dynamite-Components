﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using GSoft.Dynamite.Search.Core.Controls;
using GSoft.Dynamite.Search.Core.Controls.Templates;

namespace GSoft.Dynamite.Search.SP.SP.CONTROLTEMPLATES.GSoft.Dynamite.Search
{
    /// <summary>
    /// The user control for the Social Meta Tags.
    /// </summary>
    public partial class SchemaOrgMetaTags : SocialMetaTagsControl
    {
        /// <summary>
        /// On PreRender override
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            var title = new TemplatedControlWrapper()
            {
                Control = this.GetCatalogItemReuseXmlControl(BuiltInManagedProperties.Title.Name),
                ContentTemplate = new MetaTagTemplate("itemprop", "name")
            };
            this.SchemaOrg.Controls.Add(title);

            var description = new TemplatedControlWrapper()
            {
                Control = this.GetCatalogItemReuseXmlControl(BuiltInManagedProperties.MetaDescription.Name),
                ContentTemplate = new MetaTagTemplate("itemprop", "description")
            };
            this.SchemaOrg.Controls.Add(description);

            var path = new UrlElementControlWrapper()
            {
                Control = this.GetCatalogItemReuseXmlControl(BuiltInManagedProperties.Url.Name),
                ContentTemplate = new MetaTagTemplate("itemprop", "url")
            };
            this.SchemaOrg.Controls.Add(path);

            var picture = new ImageElementControlWrapper()
            {
                Control = this.GetCatalogItemReuseXmlControl(BuiltInManagedProperties.PublishingImage.Name),
                ContentTemplate = new MetaTagTemplate("itemprop", "image")
            };
            this.SchemaOrg.Controls.Add(picture);
        }
    }
}
