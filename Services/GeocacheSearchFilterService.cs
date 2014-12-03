﻿using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IGeocacheSearchFilterService: IDependency
    {
        PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter);
        PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, bool addOrderByClause);
        PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, double? minLat, double? minLon, double? maxLat, double? maxLon);
        PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, double? minLat, double? minLon, double? maxLat, double? maxLon, bool addOrderByClause);
    }

    public class GeocacheSearchFilterService : IGeocacheSearchFilterService
    {
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GeocacheSearchFilterService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter)
        {
            return AddWhereClause(sql, filter, null, null, null, null, false);
        }
        public PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, bool addOrderByClause)
        {
            return AddWhereClause(sql, filter, null, null, null, null, addOrderByClause);
        }
        public PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, double? minLat, double? minLon, double? maxLat, double? maxLon)
        {
            return AddWhereClause(sql, filter, minLat, minLon, maxLat, maxLon, false);
        }
        public PetaPoco.Sql AddWhereClause(PetaPoco.Sql sql, GeocacheSearchFilter filter, double? minLat, double? minLon, double? maxLat, double? maxLon, bool addOrderByClause)
        {
            string euDatabase = Core.Helper.GetTableNameFromConnectionString(dbGcEuDataConnString);
            sql = sql.From("GCComGeocache with (nolock)")
                .InnerJoin(string.Format("[{0}].[dbo].[GCEuGeocache]", euDatabase)).On("GCComGeocache.ID = GCEuGeocache.ID")
                .InnerJoin(string.Format("GCComUser", euDatabase)).On("GCComGeocache.OwnerId = GCComUser.ID");
            if (filter.MacroResult != null)
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                sql = sql.InnerJoin(string.Format("GCEuMacroData.dbo.macro_{0}_Resultaat", settings.YafUserID)).On(string.Format("GCComGeocache.ID = GCEuMacroData.dbo.macro_{0}_Resultaat.ID", settings.YafUserID));
                if (minLat != null)
                {
                    sql = sql.Where("GCComGeocache.Latitude>=@0", minLat);
                    sql = sql.Append("and GCComGeocache.Latitude<=@0", maxLat);
                    sql = sql.Append("and GCComGeocache.Longitude>=@0", minLon);
                    sql = sql.Append("and GCComGeocache.Longitude<=@0", maxLon);
                }
            }
            else
            {
                if (filter.Parel == true)
                {
                    sql = sql.InnerJoin(string.Format("[{0}].[dbo].[GCEuParel]", euDatabase)).On("GCComGeocache.ID = GCEuParel.GeocacheID");
                }
                sql = sql.Where("Archived=@0", false);
                if (filter.OwnerID != null)
                {
                    sql = sql.Append("and GCComUser.ID=@0", filter.OwnerID);
                }
                if (filter.CountryID != null)
                {
                    sql = sql.Append("and CountryID=@0", filter.CountryID);
                }
                if (!string.IsNullOrEmpty(filter.NameContainsWord))
                {
                    sql = sql.Append(string.Format("and GCComGeocache.Name like '%{0}%'", filter.NameContainsWord.Replace("'", "''").Replace("@", "@@")));
                }
                if (filter.OwnerName != null)
                {
                    sql = sql.Append(string.Format("and GCComUser.UserName like '%{0}%'", filter.OwnerName.Replace("'", "''").Replace("@", "@@")));
                }
                if (filter.CenterLat != null && filter.CenterLon != null && filter.Radius != null)
                {
                    sql.Append("and dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, @0, @1) < @2", filter.HomeLat, filter.HomeLon, filter.Radius);
                }
                if (!string.IsNullOrEmpty(filter.NameSeriesMatch))
                {
                    //sep = ...
                    int pos = filter.NameSeriesMatch.IndexOf("---");
                    if (pos >= 0)
                    {
                        string ltext = filter.NameSeriesMatch.Substring(0, pos);
                        string rtext = filter.NameSeriesMatch.Substring(pos + 3);
                        sql.Append(string.Format("and GCComGeocache.Name LIKE '{0}%' AND GCComGeocache.Name LIKE '%{1}'", ltext.Replace("'", "''").Replace("@", "@@"), rtext.Replace("'", "''").Replace("@", "@@")));
                    }
                }
                if (minLat != null)
                {
                    sql = sql.Append("and GCComGeocache.Latitude>=@0", minLat);
                    sql = sql.Append("and GCComGeocache.Latitude<=@0", maxLat);
                    sql = sql.Append("and GCComGeocache.Longitude>=@0", minLon);
                    sql = sql.Append("and GCComGeocache.Longitude<=@0", maxLon);
                }
            }
            if (filter.MaxResult > 0 && filter.OrderBy != null && filter.OrderByDirection != null)
            {
                int orderby = (int)filter.OrderBy;
                int orderbydir = (int)filter.OrderByDirection;
                string orderdir = orderbydir > 0 ? "ASC" : "DESC";
                sql = addOrderByState(sql, orderby, orderdir, filter);
            }
            else if (addOrderByClause)
            {
                int orderby = filter.OrderBy ?? (int)GeocacheSearchFilterOrderOnItem.DistanceFromHome;
                int orderbydir = filter.OrderByDirection ?? 1;
                if (orderby == (int)GeocacheSearchFilterOrderOnItem.DistanceFromHome && (filter.HomeLat == null || filter.HomeLon == null))
                {
                    orderby = (int)GeocacheSearchFilterOrderOnItem.PublicationDate;
                    orderbydir = -1;
                }
                string orderdir = orderbydir > 0 ? "ASC" : "DESC";
                sql = addOrderByState(sql, orderby, orderdir, filter);
                filter.OrderBy = orderby;
                filter.OrderByDirection = orderbydir;
            }
            return sql;
        }

        private PetaPoco.Sql addOrderByState(PetaPoco.Sql sql, int orderby, string orderdir, GeocacheSearchFilter filter)
        {
            switch (orderby)
            {
                case (int)GeocacheSearchFilterOrderOnItem.DistanceFromHome:
                    sql = sql.OrderBy(string.Format("dbo.F_GREAT_CIRCLE_DISTANCE(GCComGeocache.Latitude, GCComGeocache.Longitude, {0}, {1}) {2}", filter.HomeLat.ToString().Replace(',', '.'), filter.HomeLon.ToString().Replace(',', '.'), orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.HiddenDate:
                    sql = sql.OrderBy(string.Format("UTCPlaceDate {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.PublicationDate:
                    sql = sql.OrderBy(string.Format("PublishedAtDate {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Code:
                    sql = sql.OrderBy(string.Format("Code {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Difficulty:
                    sql = sql.OrderBy(string.Format("Difficulty {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Distance:
                    sql = sql.OrderBy(string.Format("Distance {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Favorites:
                    sql = sql.OrderBy(string.Format("FavoritePoints {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.FavoritesPercentage:
                    sql = sql.OrderBy(string.Format("FavPer100Found {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Founds:
                    sql = sql.OrderBy(string.Format("FoundCount {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.GeocacheType:
                    sql = sql.OrderBy(string.Format("GeocacheTypeId {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.MostRecentFoundDate:
                    sql = sql.OrderBy(string.Format("MostRecentFoundDate {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Name:
                    sql = sql.OrderBy(string.Format("Name {0}", orderdir));
                    break;
                case (int)GeocacheSearchFilterOrderOnItem.Terrain:
                    sql = sql.OrderBy(string.Format("Terrain {0}", orderdir));
                    break;
            }
            return sql;
        }

    }
}