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
    public interface IYafMostRecentPostsService : IDependency
    {
        IEnumerable<YafPost> GetMostRecentPosts();
    }

    public class YafMostRecentPostsService : IYafMostRecentPostsService
    {
        public YafMostRecentPostsService()
        {
        }

        public IEnumerable<YafPost> GetMostRecentPosts()
        {
            List<YafPost> result = new List<YafPost>();
            using (DBCon dbcon = new DBCon(DBCon.dbForumConnString))
            using (SqlCommand cmd = dbcon.Connection.CreateCommand())
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "yaf_topic_latest_frontpage2";
                cmd.Parameters.AddWithValue("NumPosts", 8);
                cmd.Parameters.AddWithValue("BoardID", 1);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    YafPost yp = new YafPost();
                    yp.Username = dr["LastUserName"] as string;
                    yp.PostUrl = string.Format("http://www.globalcaching.eu/forum/Default.aspx?g=posts&amp;m={0}#post{0}", dr["LastMessageID"]);
                    yp.PostedAt = ((DateTime)dr["LastPosted"]).AddHours(2);
                    yp.Topic = dr["Topic"] as string;
                    result.Add(yp);
                }
                dr.Close();
            }

            return result.ToArray();
        }
    }
}