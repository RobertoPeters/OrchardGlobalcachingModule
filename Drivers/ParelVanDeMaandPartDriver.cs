using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard.ContentManagement.Drivers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Globalcaching.Drivers
{
    public class ParelVanDeMaandPartDriver : ContentPartDriver<ParelVanDeMaandPart>
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        protected override string Prefix { get { return ""; } }

        public ParelVanDeMaandPartDriver()
        {
        }

        protected override DriverResult Display(ParelVanDeMaandPart part, string displayType, dynamic shapeHelper)
        {
            ParelVanDeMaandModel m = new ParelVanDeMaandModel();

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                m.Belgium = db.FirstOrDefault<ParelVanDeMaandInfo>("select top 1 GCComGeocache.Code, GCComGeocache.Name, GCComGeocacheType.GeocacheTypeName, GCEuGeocache.PublishedAtDate from GCEuData.dbo.GCEuParel inner join GCComGeocache on GCEuParel.GeocacheID=GCComGeocache.ID inner join GCEuData.dbo.GCEuGeocache on GCEuParel.GeocacheID=GCEuGeocache.ID inner join GCComGeocacheType on GCComGeocache.GeocacheTypeId=GCComGeocacheType.ID where GCComGeocache.CountryID=4 order by GCEuGeocache.PublishedAtDate desc");
                m.Netherlands = db.FirstOrDefault<ParelVanDeMaandInfo>("select top 1 GCComGeocache.Code, GCComGeocache.Name, GCComGeocacheType.GeocacheTypeName, GCEuGeocache.PublishedAtDate from GCEuData.dbo.GCEuParel inner join GCComGeocache on GCEuParel.GeocacheID=GCComGeocache.ID inner join GCEuData.dbo.GCEuGeocache on GCEuParel.GeocacheID=GCEuGeocache.ID inner join GCComGeocacheType on GCComGeocache.GeocacheTypeId=GCComGeocacheType.ID where GCComGeocache.CountryID=141 order by GCEuGeocache.PublishedAtDate desc");
            }
            CultureInfo ci = new CultureInfo("nl");
            m.Belgium.Month = m.Belgium.PublishedAtDate.ToString("MMMM", ci);
            m.Netherlands.Month = m.Netherlands.PublishedAtDate.ToString("MMMM", ci);
            m.Belgium.BannerUrl = string.Format("Images/Parelvandemaand/BE-{0}-{1}.jpg", m.Belgium.PublishedAtDate.ToString("MMM", ci), m.Belgium.PublishedAtDate.Year);
            m.Netherlands.BannerUrl = string.Format("Images/Parelvandemaand/NL-{0}-{1}.jpg", m.Netherlands.PublishedAtDate.ToString("MMM", ci), m.Netherlands.PublishedAtDate.Year);

            return ContentShape("Parts_ParelVanDeMaand",
                    () => shapeHelper.DisplayTemplate(
                            TemplateName: "Parts.ParelVanDeMaand",
                            Model: m,
                            Prefix: Prefix));
        }
    }
}