﻿using System.Collections.Generic;
using GSoft.Dynamite.Docs.Contracts.Configuration;
using GSoft.Dynamite.Docs.Contracts.Constants;
using GSoft.Dynamite.Fields;

namespace GSoft.Dynamite.Docs.Core.Configuration
{
    /// <summary>
    /// Fields configuration for the document management module
    /// </summary>
    public class DocsFieldInfoConfig : IDocsFieldInfoConfig
    {
        private readonly DocsFieldInfos docsFieldInfos;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="docsFieldInfos">The field definitions from the multilingualism module</param>
        public DocsFieldInfoConfig(DocsFieldInfos docsFieldInfos)
        {
            this.docsFieldInfos = docsFieldInfos;
        }

        /// <summary>
        /// Property that return all the fields to create or configure in the document management module
        /// </summary>
        public IList<IFieldInfo> Fields 
        {
            get
            {
                return new List<IFieldInfo>()
                {
                    this.docsFieldInfos.InternalId()
                };
            }
        }
    }
}
