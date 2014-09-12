﻿using System.Collections.Generic;
using GSoft.Dynamite.Definitions;

namespace GSoft.Dynamite.Publishing.Contracts.Configuration
{
    public interface IBaseCatalogInfoConfig
    {
        IDictionary<string, CatalogInfo> Catalogs();
    }
}
