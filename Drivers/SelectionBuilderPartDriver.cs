using Globalcaching.Models;
using Globalcaching.Services;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class SelectionBuilderPartDriver : ContentPartDriver<SelectionBuilderPart>
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly ISelectionBuilderService _selectionBuilderService;

        public SelectionBuilderPartDriver(ISelectionBuilderService selectionBuilderService,
            IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _selectionBuilderService = selectionBuilderService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        protected override DriverResult Display(SelectionBuilderPart part, string displayType, dynamic shapeHelper)
        {
            var m = _selectionBuilderService.GetOwnSelectionGraphs(0);
            var settings = _gcEuUserSettingsService.GetSettings();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                m.GeocacheTypes = db.Fetch<GCComGeocacheType>("where ID in (2, 3, 4, 5, 6, 8, 11, 12, 15, 137, 1858) order by ID");
                m.Containers = new List<int> { 1, 2, 3, 4, 5, 6, 8 };
                m.AttributeTypes = db.Fetch<GCComAttributeType>("order by ID");
                m.UserSettings = new GCEuUserSettings();
                m.UserSettings.DefaultCountryCode = settings.DefaultCountryCode;
                m.UserSettings.GCComUserID = settings.GCComUserID;
                m.UserSettings.HomelocationLat = settings.HomelocationLat;
                m.UserSettings.HomelocationLon = settings.HomelocationLon;
                m.UserSettings.IsPM = settings.IsPM;
            }
            return ContentShape("Parts_SelectionBuilder",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SelectionBuilder",
                            Model: m,
                            Prefix: Prefix));
        }
    }

}