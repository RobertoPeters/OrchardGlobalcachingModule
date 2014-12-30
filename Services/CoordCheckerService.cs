using Gavaghan.Geodesy;
using Globalcaching.Core;
using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Messaging.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace Globalcaching.Services
{
    public interface ICoordCheckerService: IDependency
    {
        bool CheckCoord(string code, string coord, out string remarks);
        CoordCheckerMaintModel CoordCheckerMaintModel(string activeCode, int page, int pageSize);
        CoordCheckerAttempts CoordCheckerAttempts(string activeCode, int page, int pageSize);
        CoordCheckerMaintModel CreateCode(GCEuCoordCheckCode code, int page, int pageSize);
        CoordCheckerMaintModel DeleteCode(string code, int page, int pageSize);
        CoordCheckerMaintModel UpdateCode(GCEuCoordCheckCode code, int page, int pageSize);
    }

    public class CoordCheckerService : ICoordCheckerService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IMessageService _messageService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CoordCheckerService(IMessageService messageService,
            IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _messageService = messageService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        public CoordCheckerMaintModel CreateCode(GCEuCoordCheckCode code, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            CoordCheckerMaintModel result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                if (db.FirstOrDefault<GCEuCoordCheckCode>("where Code=@0", code.Code) == null)
                {
                    code.UserID = settings.YafUserID;
                    db.Insert(code);
                    result = CoordCheckerMaintModel(code.Code, page, pageSize);
                }
            }
            return result;
        }

        public CoordCheckerMaintModel UpdateCode(GCEuCoordCheckCode code, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<GCEuCoordCheckCode>("where UserID=@0 and Code=@1", settings.YafUserID, code.Code);
                if (m != null)
                {
                    m.Lat = code.Lat;
                    m.Lon = code.Lon;
                    m.NotifyOnFailure = code.NotifyOnFailure;
                    m.NotifyOnSuccess = code.NotifyOnSuccess;
                    m.Radius = code.Radius;
                    db.Save(m);
                }
            }
            return CoordCheckerMaintModel(code.Code, page, pageSize);
        }

        public CoordCheckerMaintModel DeleteCode(string code, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<GCEuCoordCheckCode>("where UserID=@0 and Code=@1", settings.YafUserID, code);
                if (m != null)
                {
                    db.Execute("delete from GCEuCoordCheckAttempt where Waypoint=@0", code);
                    db.Execute("delete from GCEuCoordCheckCode where UserID=@0 and Code=@1", settings.YafUserID, code);
                }
            }
            return CoordCheckerMaintModel(null, page, pageSize);
        }

        public CoordCheckerMaintModel CoordCheckerMaintModel(string activeCode, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            CoordCheckerMaintModel result = new CoordCheckerMaintModel();
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.Codes = db.Fetch<GCEuCoordCheckCode>("where UserID=@0 order by Code", settings.YafUserID);
                //result.Codes = db.Fetch<GCEuCoordCheckCode>("order by Code");
                foreach (var r in result.Codes)
                {
                    r.Coordinates = Helper.GetCoordinatesPresentation(r.Lat, r.Lon);
                }

                if (!string.IsNullOrEmpty(activeCode))
                {
                    result.ActiveCode = (from a in result.Codes where string.Compare(a.Code, activeCode, true) == 0 select a).FirstOrDefault();
                }
                if (result.ActiveCode == null && result.Codes.Count > 0)
                {
                    result.ActiveCode = result.Codes[0];
                }
                if (result.ActiveCode != null)
                {
                    result.AttemptInfo = CoordCheckerAttempts(result.ActiveCode.Code, page, pageSize);
                }
                else
                {
                    result.AttemptInfo = new CoordCheckerAttempts();
                    result.AttemptInfo.CurrentPage = 1;
                    result.AttemptInfo.PageCount = 1;
                    result.AttemptInfo.TotalCount = 0;
                    result.AttemptInfo.Attempts = new List<GCEuCoordCheckAttempt>();
                }
            }
            return result;
        }

        public CoordCheckerAttempts CoordCheckerAttempts(string activeCode, int page, int pageSize)
        {
            CoordCheckerAttempts result = new CoordCheckerAttempts();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings.YafUserID <= 1) return null;
                if (db.FirstOrDefault<GCEuCoordCheckCode>("where UserID=@0 and Code=@1", settings.YafUserID, activeCode) != null)
                {
                    var items = db.Page<GCEuCoordCheckAttempt>(page, pageSize, "select * from GCEuCoordCheckAttempt where Waypoint=@0 order by AttemptAt desc", activeCode);
                    result.Attempts = items.Items.ToList();
                    result.CurrentPage = items.CurrentPage;
                    result.PageCount = items.TotalPages;
                    result.TotalCount = items.TotalItems;
                    foreach (var r in result.Attempts)
                    {
                        r.Coordinates = Helper.GetCoordinatesPresentation(r.Lat, r.Lon);
                    }
                }
            }
            return result;
        }

        public bool CheckCoord(string code, string coord, out string remarks)
        {
            double MULTIPLEFACTOR = 100000.0;
            double EPSSHIFT = 0.000001;
            double EPS = 0.000001;

            remarks = "";
            bool result = false;

            LatLon ll = LatLon.FromString(coord);
            if (ll != null)
            {
                using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                {
                    GCEuCoordCheckCode cc = db.FirstOrDefault<GCEuCoordCheckCode>("where Code=@0", code);
                    if (cc == null)
                    {
                        remarks = string.Format("De opgegeven code '{0}' bevat geen coordinaten checker.", code);
                    }
                    else
                    {
                        string visitorID = _workContextAccessor.GetContext().HttpContext.Request.UserHostAddress.Replace(".", "").Replace(":", "");
                        var attempts = db.Fetch<GCEuCoordCheckAttempt>("where Waypoint=@0 and VisitorID=@1 and AttemptAt>=@2", code, visitorID, DateTime.Now.AddSeconds(-60));
                        if (attempts == null || attempts.Count == 0)
                        {
                            GCEuCoordCheckAttempt attempt = new GCEuCoordCheckAttempt();
                            attempt.AttemptAt = DateTime.Now;
                            attempt.Lat = ll.lat;
                            attempt.Lon = ll.lon;
                            attempt.VisitorID = visitorID;
                            attempt.Waypoint = cc.Code;
                            db.Insert(attempt);

                            ll.lat = (int)(MULTIPLEFACTOR * ll.lat) / MULTIPLEFACTOR + EPSSHIFT;
                            ll.lon = (int)(MULTIPLEFACTOR * ll.lon) / MULTIPLEFACTOR + EPSSHIFT;

                            LatLon actLL = new LatLon();
                            actLL.lat = (int)(MULTIPLEFACTOR * cc.Lat) / MULTIPLEFACTOR + EPSSHIFT;
                            actLL.lon = (int)(MULTIPLEFACTOR * cc.Lon) / MULTIPLEFACTOR + EPSSHIFT;

                            result = (System.Math.Abs(ll.lat - actLL.lat) < EPS && System.Math.Abs(ll.lon - actLL.lon) < EPS);
                            if (!result && cc.Radius > 0)
                            {
                                //check for radius
                                GeodeticMeasurement gm = Helper.CalculateDistance(ll, actLL);
                                result = (gm.EllipsoidalDistance <= (double)cc.Radius);
                            }

                            StringBuilder sb = new StringBuilder();
                            if (result)
                            {
                                sb.AppendFormat("Er is een coordinaat controle uitgevoerd op Waypoint {0}", code);
                                sb.AppendLine();
                                sb.AppendLine("Het coordinaat was goed.");
                            }
                            else
                            {
                                remarks = "Het opgegeven coordinaat is niet juist.";
                                sb.AppendFormat("Er is een coordinaat controle uitgevoerd op Waypoint {0}", code);
                                sb.AppendLine();
                                sb.AppendLine("Het coordinaat was fout.");
                                sb.AppendLine();
                                sb.AppendFormat("Het coordinaat was {0}", Helper.GetCoordinatesPresentation(ll.lat, ll.lon));
                                sb.AppendLine();
                            }
                            if ((result && cc.NotifyOnSuccess) || (!result && cc.NotifyOnFailure))
                            {
                                var parameters = new Dictionary<string, object> {
                                {"Subject", result?"Coordinaten checker email notificatie - Goed":"Coordinaten checker email notificatie - Fout"},
                                {"Body", sb.ToString()},
                                {"Recipients",  _gcEuUserSettingsService.GetEMail(cc.UserID) }
                            };
                                _messageService.Send("Email", parameters);
                            }
                        }
                        else
                        {
                            remarks = "Je mag maximaal 1 poging per minuut ingeven. Wacht even en probeer het dan nog eens.";
                        }
                    }
                }
            }
            else
            {
                remarks = "Het opgegeven coordinaat is geen geldig formaat coordinaat.";
            }

            return result;
        }
    }
}