﻿using Globalcaching.Models;
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
            m.GeocacheTypes = Core.CachedData.Instance.GeocacheTypesFilter;
            m.Containers = new List<int> { 1, 2, 3, 4, 5, 6, 8 };
            m.AttributeTypes = Core.CachedData.Instance.AttributesInfo;
            m.States = Core.CachedData.Instance.StatesInfo.OrderBy(x => x.State).ToList();
            m.UserSettings = new GCEuUserSettings();
            m.UserSettings.DefaultCountryCode = settings.DefaultCountryCode;
            m.UserSettings.GCComUserID = settings.GCComUserID;
            m.UserSettings.HomelocationLat = settings.HomelocationLat;
            m.UserSettings.HomelocationLon = settings.HomelocationLon;
            m.UserSettings.IsPM = settings.IsPM;
            return ContentShape("Parts_SelectionBuilder",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.SelectionBuilder",
                            Model: m,
                            Prefix: Prefix));
        }
    }

}