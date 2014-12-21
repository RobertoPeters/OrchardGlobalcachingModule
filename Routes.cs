﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace Globalcaching
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GCComSearchUser",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComUserSearch"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "EditUserSettings/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "EditUserSettings"},
                            {"action", "Update"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "EditCCCSettings/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "EditCCCSettings"},
                            {"action", "Update"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GCComSearchGeocacheLogsOfUser",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Geocache/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "DisplayGeocache"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetAllLogs/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "DisplayGeocache"},
                            {"action", "GetAllLogs"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LiveAPIRequest",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIOAuth"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LiveAPICallback",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIOAuth"},
                            {"action", "RequestCallback"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "AlleCaches",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "UpdateSortingGeocaches",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "UpdateSortingGeocaches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Parels",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "Parels"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SearchGeocaches",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "SearchGeocaches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "RecenteCachesLand/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "MostRecentCountry"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "NaamSeries/{countryId}/{nameSeriesMatch}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "NameSeries"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "MacroResultaatLijst",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "MacroResult"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SnelZoeken",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "QuickSearch"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GeocachesVanEigenaar/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "FromOwner"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "CopyListToDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "CopyListToDownload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SearchLogImages",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComSearchLogImages"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GeocacheKaart",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },///{minLat}/{minLon}/{maxLat}/{maxLon}/{zoom}
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "CopyViewToDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "CopyViewToDownload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetGeocachesOnMap",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "SearchGeocaches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetLatLonCoords",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "GetLatLonCoords"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetLocationCoords",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "GetLocationCoords"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetAreaInfo",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "GetAreaInfo"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetAreaPolygons",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "GetAreaPolygons"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetGeocachesOnMap/{minLat}/{minLon}/{maxLat}/{maxLon}/{zoom}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "SearchGeocaches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetWaypointInfo",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "GetWaypointInfo"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "KaartMetFilter",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "ShowMap"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "MacroResultaatKaart",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheMap"},
                            {"action", "MacroResult"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "CheckCCC",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Layar/ccc.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "CCCCheck"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SaveMacro",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Macro"},
                            {"action", "Save"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "RunMacro",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Macro"},
                            {"action", "Run"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetMacro",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Macro"},
                            {"action", "GetMacro"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "DeleteMacro",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Macro"},
                            {"action", "DeleteMacro"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Rot13",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "Rot13"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "WoordWaarde",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "WoordWaarde"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ASCIIConvert",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "ASCIIConvert"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "FrequencyCounter",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "FrequencyCounter"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Cypher",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "Cypher"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "Projection",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "Projection"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "AfstandHoek",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "AfstandHoek"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "CoordConv",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "OnlineTools"},
                            {"action", "CoordConv"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LogGeocache/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogGCCom"},
                            {"action", "LogGeocache"}, 
                            {"id", UrlParameter.Optional}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LogGC",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogGCCom"},
                            {"action", "LogGC"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LogTB",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogGCCom"},
                            {"action", "LogTB"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetFavoriteGeocaches",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FavoriteGeocaches"},
                            {"action", "GetFavoriteGeocaches"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetFavoriteGeocachers",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FavoriteGeocachers"},
                            {"action", "GetFavoriteGeocachers"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetLogImageStats",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogImageStats"},
                            {"action", "GetLogImageStats"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetGeocacheSeries",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheSeries"},
                            {"action", "GetGeocacheSeries"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetLiveAPIDownloadStatus",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "GetLiveAPIDownloadStatus"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "StartLiveAPIDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "StartDownload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "StopLiveAPIDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "StopDownload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "CopyMacroResultToDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "CopyMacroResultToDownload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LiveAPIDownloadFile",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "DownloadFile"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "LiveAPIDownloadGeocache",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "DownloadGeocache"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "FixLogOldSite",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "FixLogOldSite"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetFTFStats",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "GetFTFStats"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "FTFToekenning",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetUnassignedFTF",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "GetUnassignedFTF"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ClearFTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "ClearFTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ClearSTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "ClearSTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ClearTTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "ClearTTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetFTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "SetFTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetSTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "SetSTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetTTFAssignment",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "SetTTFAssignment"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetFTFCompleted",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "SetFTFCompleted"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ResetFTFCounter",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "ResetFTFCounter"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "AddAllGeocachesToQueue",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "AddAllGeocachesToQueue"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "forum/{*pathInfo}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Forum"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "globalcaching/admin",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Admin"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "globalcaching/admin/restart",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Admin"},
                            {"action", "Restart"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "globalcaching/admin/AddParel",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Admin"},
                            {"action", "AddParel"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "AfstandToekenning",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "Index"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetDistanceChecked",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "SetDistanceChecked"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetUnassignedDistance",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "GetUnassignedDistance"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "SetDistance",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "SetDistance"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "ClearDistance",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "ClearDistance"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Globalcaching"}
                        },
                        new MvcRouteHandler())
                 }
            };
        }
    }
}