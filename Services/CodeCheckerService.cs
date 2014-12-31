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
    public interface ICodeCheckerService: IDependency
    {
        CodeCheckerMaintModel CodeCheckerMaintModel(int? activeID, int page, int pageSize);
        CodeCheckerAttempts CodeCheckerAttempts(int activeID, int page, int pageSize);
        bool CheckCode(int id, string publiccode, string answer, out string remarks);
        CodeCheckerMaintModel DeleteCode(int id, int page, int pageSize);
        CodeCheckerMaintModel CreateCode(GCEuCodeCheckCode code, int page, int pageSize);
        CodeCheckerMaintModel UpdateCode(GCEuCodeCheckCode code, int page, int pageSize);
    }

    public class CodeCheckerService : ICodeCheckerService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IMessageService _messageService;
        private readonly IGCEuUserSettingsService _gcEuUserSettingsService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public CodeCheckerService(IMessageService messageService,
            IGCEuUserSettingsService gcEuUserSettingsService,
            IWorkContextAccessor workContextAccessor)
        {
            _messageService = messageService;
            _gcEuUserSettingsService = gcEuUserSettingsService;
            _workContextAccessor = workContextAccessor;
        }

        public CodeCheckerMaintModel UpdateCode(GCEuCodeCheckCode code, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<GCEuCodeCheckCode>("where UserID=@0 and ID=@1", settings.YafUserID, code.ID);
                if (m != null)
                {
                    m.AnswerCode = code.AnswerCode;
                    m.AnswerText = code.AnswerText;
                    m.CaseSensative = code.CaseSensative;
                    m.DelaySeconds = code.DelaySeconds;
                    m.EmailNotifyOnFail = code.EmailNotifyOnFail;
                    m.EmailNotifyOnSuccess = code.EmailNotifyOnSuccess;
                    m.GroundspeakAuthReq = code.GroundspeakAuthReq;
                    m.PublicCode = code.PublicCode;
                    db.Save(m);
                }
            }
            return CodeCheckerMaintModel(code.ID, page, pageSize);
        }

        public CodeCheckerMaintModel CreateCode(GCEuCodeCheckCode code, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            CodeCheckerMaintModel result = null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                if (db.FirstOrDefault<GCEuCodeCheckCode>("where UserID=@0 and PublicCode=@1", settings.YafUserID, code.PublicCode) == null)
                {
                    code.UserID = settings.YafUserID;
                    db.Insert(code);
                    result = CodeCheckerMaintModel(code.ID, page, pageSize);
                }
            }
            return result;
        }

        public CodeCheckerMaintModel DeleteCode(int id, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var m = db.FirstOrDefault<GCEuCodeCheckCode>("where UserID=@0 and ID=@1", settings.YafUserID, id);
                if (m != null)
                {
                    db.Execute("delete from GCEuCodeCheckAttempt where CodeID=@0", id);
                    db.Execute("delete from GCEuCodeCheckCode where ID=@0", id);
                }
            }
            return CodeCheckerMaintModel(null, page, pageSize);
        }

        public bool CheckCode(int id, string publiccode, string answer, out string remarks)
        {
            remarks = "";
            bool result = false;

            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCEuCodeCheckCode cc = db.FirstOrDefault<GCEuCodeCheckCode>("where UserID=@0 and PublicCode=@1", id, publiccode);
                if (cc == null)
                {
                    remarks = "De opgegeven code bevat geen code checker.";
                }
                else
                {
                    string gcComUserName = "";
                    var settings = _gcEuUserSettingsService.GetSettings();
                    if (settings != null && !string.IsNullOrEmpty(settings.LiveAPIToken))
                    {
                        var usr = LiveAPIClient.GetMemberProfile(settings.LiveAPIToken);
                        if (usr != null)
                        {
                            gcComUserName = usr.User.UserName;
                        }
                    }

                    if (cc.GroundspeakAuthReq && string.IsNullOrEmpty(gcComUserName))
                    {
                        remarks = "Voor deze checker moet je Live API autorisatie uitgevoerd hebben. Ga hiervoor naar Persoonlijk en dan Live API.";
                    }
                    else
                    {
                        string visitorID = _workContextAccessor.GetContext().HttpContext.Request.UserHostAddress.Replace(".", "").Replace(":", "");
                        var attempts = db.Fetch<GCEuCodeCheckAttempt>("where CodeID=@0 and VisitorID=@1 and AttemptAt>=@2", cc.ID, visitorID, DateTime.Now.AddSeconds(-1 * cc.DelaySeconds));
                        if (attempts == null || attempts.Count == 0)
                        {
                            GCEuCodeCheckAttempt attempt = new GCEuCodeCheckAttempt();
                            attempt.AttemptAt = DateTime.Now;
                            attempt.VisitorID = visitorID;
                            attempt.Answer = answer;
                            attempt.GroundspeakUserName = gcComUserName;
                            attempt.CodeID = cc.ID;
                            db.Insert(attempt);

                            //ok, valid attempt
                            if (cc.CaseSensative)
                            {
                                result = (cc.AnswerCode == answer);
                            }
                            else
                            {
                                result = (cc.AnswerCode.ToLower() == answer.ToLower());
                            }


                            StringBuilder sb = new StringBuilder();
                            if (result)
                            {
                                remarks = cc.AnswerText;
                                sb.AppendFormat("Er is een Code Check uitgevoerd op Code {0}", answer);
                                sb.AppendLine();
                                sb.AppendLine("Het antwoord was goed.");
                            }
                            else
                            {
                                remarks = "Het antwoord is niet juist.";
                                sb.AppendFormat("Er is een Code Check uitgevoerd op Code {0}", answer);
                                sb.AppendLine();
                                sb.AppendLine("Het antwoord was fout.");
                                sb.AppendLine();
                                sb.AppendFormat("Ingevulde code was: {0}", publiccode);
                                sb.AppendLine();
                            }
                            if ((result && cc.EmailNotifyOnSuccess) || (!result && cc.EmailNotifyOnFail))
                            {
                                var parameters = new Dictionary<string, object> {
                                {"Subject", result?"Code checker email notificatie - Goed":"Code checker email notificatie - Fout"},
                                {"Body", sb.ToString()},
                                {"Recipients",  _gcEuUserSettingsService.GetEMail(cc.UserID) }
                            };
                                _messageService.Send("Email", parameters);
                            }
                        }
                        else
                        {
                            remarks = string.Format("Je mag maximaal 1 poging per {0} seconden ingeven. Wacht even en probeer het dan nog eens.", cc.DelaySeconds);
                        }
                    }
                }
            }
            return result;
        }

        public CodeCheckerMaintModel CodeCheckerMaintModel(int? activeID, int page, int pageSize)
        {
            var settings = _gcEuUserSettingsService.GetSettings();
            if (settings.YafUserID <= 1) return null;
            CodeCheckerMaintModel result = new CodeCheckerMaintModel();
            result.OwnerID = settings.YafUserID;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result.Codes = db.Fetch<GCEuCodeCheckCode>("where UserID=@0 order by ID", settings.YafUserID);
                //result.Codes = db.Fetch<GCEuCoordCheckCode>("order by Code");
                foreach (var c in result.Codes)
                {
                    c.EncodedPublicCode = HttpUtility.UrlEncode(c.PublicCode);
                }

                if (activeID!=null)
                {
                    result.ActiveCode = (from a in result.Codes where a.ID==(int)activeID select a).FirstOrDefault();
                }
                if (result.ActiveCode == null && result.Codes.Count > 0)
                {
                    result.ActiveCode = result.Codes[0];
                }
                if (result.ActiveCode != null)
                {
                    result.AttemptInfo = CodeCheckerAttempts(result.ActiveCode.ID, page, pageSize);
                }
                else
                {
                    result.AttemptInfo = new CodeCheckerAttempts();
                    result.AttemptInfo.CurrentPage = 1;
                    result.AttemptInfo.PageCount = 1;
                    result.AttemptInfo.TotalCount = 0;
                    result.AttemptInfo.Attempts = new List<GCEuCodeCheckAttempt>();
                }
            }
            return result;
        }

        public CodeCheckerAttempts CodeCheckerAttempts(int activeID, int page, int pageSize)
        {
            CodeCheckerAttempts result = new CodeCheckerAttempts();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var settings = _gcEuUserSettingsService.GetSettings();
                if (settings.YafUserID <= 1) return null;
                if (db.FirstOrDefault<GCEuCodeCheckCode>("where UserID=@0 and ID=@1", settings.YafUserID, activeID) != null)
                {
                    var items = db.Page<GCEuCodeCheckAttempt>(page, pageSize, "select * from GCEuCodeCheckAttempt where CodeID=@0 order by AttemptAt desc", activeID);
                    result.Attempts = items.Items.ToList();
                    result.CurrentPage = items.CurrentPage;
                    result.PageCount = items.TotalPages;
                    result.TotalCount = items.TotalItems;
                }
            }
            return result;
        }

    }
}