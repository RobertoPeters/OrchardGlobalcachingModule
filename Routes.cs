using System.Collections.Generic;
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
                        "Donateurs",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingDonation"},
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
                        "GetDonatorRecord",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingDonation"},
                            {"action", "GetDonatorRecord"}
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
                        "SaveDonatorRecord",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingDonation"},
                            {"action", "SaveDonatorRecord"}
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
                        "GetDonators",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingDonation"},
                            {"action", "GetDonators"}
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
                        "GCComGebruikerInfo/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComUserSearch"},
                            {"action", "ShowGCComUserInfo"}
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
                        "GetGeocachesByOwner",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheBatchLog"},
                            {"action", "GetGeocachesByOwner"}
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
                        "GetGeocachesByCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheBatchLog"},
                            {"action", "GetGeocachesByCode"}
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
                        "GetGeocachesByName",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheBatchLog"},
                            {"action", "GetGeocachesByName"}
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
                        "ListGAPPAuthorizations",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GAPPInfo"},
                            {"action", "ListAuthorizations"}
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
                        "images/nlcombirank.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CombiRanking"},
                            {"action", "Banner"}
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
                        "GetCombiRanking",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CombiRanking"},
                            {"action", "GetCombiRanking"}
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
                        "GevondenInLandBanner/{id}/{countryid}/{year}/{type}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FoundsPerCountryRanking"},
                            {"action", "Banner"}
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
                        "GetFoundsRanking",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FoundsPerCountryRanking"},
                            {"action", "GetFoundsRanking"}
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
                        "TrackableGroep/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "ShowGroup"}
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
                        "TrackableGroup/UpdateTrackable",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "UpdateTrackable"}
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
                        "TrackableGroup/GetGroups",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "GetGroups"}
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
                        "TrackableGroup/DeleteGroup",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "DeleteGroup"}
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
                        "TrackableGroup/GetGroupInfo",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "GetGroupInfo"}
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
                        "TrackableGroup/SaveGroup",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "SaveGroup"}
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
                        "TrackableGroup/AddTrackable",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "AddTrackable"}
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
                        "TrackableGroup/DeleteTrackable",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "TrackableGroup"},
                            {"action", "DeleteTrackable"}
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
                        "BezoekersActiviteiten",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "UsersOnline"},
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
                        "SaveGraph",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "SelectionBuilder"},
                            {"action", "SaveGraph"}
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
                        "LoadGraph",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "SelectionBuilder"},
                            {"action", "LoadGraph"}
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
                        "DeleteGraph",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "SelectionBuilder"},
                            {"action", "DeleteGraph"}
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
                        "LogCorrection/SubmitGeocache",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogCorrection"},
                            {"action", "SubmitGeocache"}
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
                        "CodeCheck/CheckCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "CheckCode"}
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
                        "CodeCheck/GetAttempts",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "GetAttempts"}
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
                        "CodeCheck/GetCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "GetCode"}
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
                        "CodeCheck/DeleteCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "DeleteCode"}
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
                        "CodeCheck/CreateCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "CreateCode"}
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
                        "CodeCheck/UpdateCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "UpdateCode"}
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
                        "CheckCoord",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "CheckCoord"}
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
                        "GetCCCRequests",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "GetRequests"}
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
                        "GetCCCRequestsPage",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "GetRequestsPage"}
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
                        "cachers/ccc.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "RedirectFromOldSite"}
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
                        "caches/coordcheck.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "RedirectFromOldSite"}
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
                        "caches/codecheck.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CodeChecker"},
                            {"action", "RedirectFromOldSite"}
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
                        "GetAttempts",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "GetAttempts"}
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
                        "GetCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "GetCode"}
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
                        "CreateCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "CreateCode"}
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
                        "DeleteCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "DeleteCode"}
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
                        "UpdateCode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CoordChecker"},
                            {"action", "UpdateCode"}
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
                        "cache/large/{code}/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Images"},
                            {"action", "GeocachingComCacheLargeImage"}
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
                        "default.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "OldMainPage"}
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
                        "AanroepOverzicht",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
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
                        "GetServiceCallsPage",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "GetServiceCallsPage"}
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
                        "Service/GeoRSS.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "GeoRSS"}
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
                        "Service/GeocacheCodes.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "GeocacheCodes"}
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
                        "Service/GeocacheCodesEx.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "GeocacheCodesEx"}
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
                        "Service/GeocacheCodesExFilter.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "GeocacheCodesExFilter"}
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
                        "Service/CacheFavorites.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "CacheFavorites"}
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
                        "Service/CacheFavoritesWithFoundCount",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "CacheFavoritesWithFoundCount"}
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
                        "Service/CacheDistance.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "CacheDistance"}
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
                        "Service/Archived.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "Archived"}
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
                        "gcpqgen/pqset.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "PQSetNL"}
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
                        "gcpqgen/pqsetbe.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "PQSetBE"}
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
                        "gcpqgen/pqsetlu.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GlobalcachingServices"},
                            {"action", "PQSetLU"}
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
                        "ListMemberSettings",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "EditUserSettings"},
                            {"action", "ListMemberSettings"}
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
                        "ContactForm",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "ContactForm"},
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
                        "IngediendeContactFormulieren",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "ContactForm"},
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
                        "GetContactForms",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "ContactForm"},
                            {"action", "GetContactForms"}
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
                        "FTFGeocaches/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "FTFLog"}
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
                        "STFGeocaches/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "STFLog"}
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
                        "TTFGeocaches/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "TTFLog"}
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
                        "FTFLogs/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "FTFLogs"}
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
                        "FTFLogsOfUser",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "FTFLogsOfUser"}
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
                        "STFLogs/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "STFLogs"}
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
                        "STFLogsOfUser",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "STFLogsOfUser"}
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
                        "TTFLogs/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "TTFLogs"}
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
                        "TTFLogsOfUser",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GCComGeocacheLogsSearch"},
                            {"action", "TTFLogsOfUser"}
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
                        "Bookmark/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "Bookmark"}
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
                        "CachesGepubliceerdAfgelopenDagen/{countryid}/{maxdaysago}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CacheList"},
                            {"action", "PublishedMaxDaysAgo"}
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
                        "ListCCCMembers",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "ListCCCMembers"}
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
                        "DeactivateCCCMember",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "CheckCCC"},
                            {"action", "DeactivateCCCMember"}
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
                        "SimulateLogGC",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LogGCCom"},
                            {"action", "SimulateLogGC"}
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
                        "GetLiveAPIGetLogs",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "GetLiveAPIGetLogs"}
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
                        "GetLiveAPILogDownloadStatus",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "GetLiveAPILogDownloadStatus"}
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
                        "UpdateLiveAPILimits",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "UpdateLiveAPILimits"}
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
                        "StartLiveAPILogDownload",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "LiveAPIDownload"},
                            {"action", "StartLogDownload"}
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
                        "images/ftfimg.aspx",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "Banner"}
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
                        "FTF/AddGeocache",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "FTFStats"},
                            {"action", "AddGeocache"}
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
                        "globalcaching/admin/refreshforparels",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Admin"},
                            {"action", "RefreshForParels"}
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
                        "SetDistanceHandledBy",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheDistance"},
                            {"action", "SetDistanceHandledBy"}
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
                 },
                 new RouteDescriptor {
                    Priority = 5,
                    Route = new Route(
                        "GetGeocacheMaintenanceInfo",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "GeocacheMaintenance"},
                            {"action", "GetGeocacheMaintenanceInfo"}
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
                        "SetNewestCachesMode",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "NewestCaches"},
                            {"action", "SetMode"}
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
                        "GetBookmarks",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Bookmarks"},
                            {"action", "GetBookmarks"}
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
                        "athom/webhooks/{id}/{token}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Athom"},
                            {"action", "webhooks"}
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
                        "ShopInfo",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "ShopInfo"}
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
                        "ShopAuthorize",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "Authorize"}
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
                        "GetShopAccessToken",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "GetAccessToken"}
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
                        "RefreshAccessToken",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "RefreshAccessToken"}
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
                        "SetMasterCategoryId/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "SetMasterCategoryId"}
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
                        "GetUserProduct",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "GetUserProduct"}
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
                        "AddUserProduct",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "AddUserProduct"}
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
                        "DeleteUserProduct",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "DeleteUserProduct"}
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
                        "SaveUserProduct",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "SaveUserProduct"}
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
                        "UploadProductImage",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "UploadProductImage"}
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
                        "GetProductImage/{id}",
                        new RouteValueDictionary {
                            {"area", "Globalcaching"},
                            {"controller", "Shop"},
                            {"action", "GetProductImage"}
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