using Globalcaching.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Handlers
{
    public class FinancialYearStatusHandler : ContentHandler {
        public FinancialYearStatusHandler(IRepository<FinancialYearStatusRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}