using Globalcaching.Models;
using Globalcaching.Services;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class SearchByAttributesPartDriver : ContentPartDriver<SearchByAttributesPart>
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        protected override string Prefix { get { return ""; } }

        public SearchByAttributesPartDriver(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(SearchByAttributesPart part, string displayType, dynamic shapeHelper)
        {
            SearchByAttributesModel m = new SearchByAttributesModel();

            var settings = _gcEuUserSettingsService.GetSettings();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                m.GeocacheTypes = db.Fetch<GCComGeocacheType>("where ID in (2, 3, 4, 5, 6, 8, 11, 12, 15, 137, 1858) order by ID");
                m.Containers = new List<int> { 1, 2, 3, 4, 5, 6, 8 };
                m.UserSettings = settings;
            }

            return ContentShape("Parts_SearchByAttributes",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SearchByAttributes",
                            Model: m,
                            Prefix: Prefix));
        }

    }
}