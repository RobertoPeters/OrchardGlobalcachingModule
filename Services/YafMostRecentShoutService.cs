using Globalcaching.Core;
using Globalcaching.Models;
using Orchard;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IYafMostRecentShoutService : IDependency
    {
        IEnumerable<YafShout> GetMostRecentShout();
    }

    public class YafMostRecentShoutService : IYafMostRecentShoutService
    {
        private readonly IOrchardServices _orchardServices;

        public YafMostRecentShoutService(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices; 
        }

        public IEnumerable<YafShout> GetMostRecentShout()
        {
            List<YafShout> result = new List<YafShout>();
            using (DBCon dbcon = new DBCon(DBCon.dbForumConnString))
            using (SqlCommand cmd = dbcon.Connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "yaf_shoutbox_getmessages";
                cmd.Parameters.AddWithValue("BoardID", 1);
                cmd.Parameters.AddWithValue("NumberOfMessages", 8);
                cmd.Parameters.AddWithValue("StyledNicks", false);
                SqlDataReader dr = cmd.ExecuteReader();

                bool canSeeMessage = _orchardServices.WorkContext.CurrentUser != null && _orchardServices.WorkContext.CurrentUser.Id > 0;

                while (dr.Read())
                {
                    YafShout yp = new YafShout();
                    yp.Username = dr["Username"] as string;
                    yp.PostedAt = ((DateTime)dr["Date"]).Add(TimeZone.CurrentTimeZone.GetUtcOffset((DateTime)dr["Date"]));
                    if (canSeeMessage)
                    {
                        yp.Message = Core.SmileySupport.Instance.ReplaceCodes(dr["Message"] as string ?? "");
                    }
                    else
                    {
                        yp.Message = "Meld je aan om het bericht te zien.";
                    }
                    result.Add(yp);
                }
                dr.Close();
            }

            return result.ToArray();
        }
    }
}