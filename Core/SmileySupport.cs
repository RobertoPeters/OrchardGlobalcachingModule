using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Globalcaching.Core
{
    public class SmileySupport
    {
        private static SmileySupport _uniqueInstance = null;
        private static object _lockObject = new object();

        private Hashtable _smileys;
        private Hashtable _htmlEncodedSmileys;

        private SmileySupport()
        {
            _smileys = new Hashtable();
            _htmlEncodedSmileys = new Hashtable();

            using (DBCon dbcon = new DBCon(DBCon.dbForumConnString))
            {
                using (SqlCommand cmd = dbcon.Connection.CreateCommand())
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.CommandText = "yaf_smiley_list";
                    cmd.Parameters.AddWithValue("BoardID", 1);
                    cmd.Parameters.AddWithValue("SmileyID", null);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        _smileys.Add(dr["Code"], dr["Icon"]);
                        _htmlEncodedSmileys.Add(HttpUtility.HtmlEncode(dr["Code"].ToString()), dr["Icon"]);
                    }
                    dr.Close();
                }
            }
        }

        public static SmileySupport Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new SmileySupport();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        public Hashtable AvailableSmileys
        {
            get { return _smileys; }
        }

        public string ReplaceCodes(string s)
        {
            string result = s;
            foreach (string code in _smileys.Keys)
            {
                result = result.Replace(code, string.Format("<img src=\"http://www.globalcaching.eu/forum/images/emoticons/{0}\" />", _smileys[code]));
            }
            return result;
        }

        public string ReplaceHtmlEncodedCodes(string s)
        {
            string result = s;
            foreach (string code in _htmlEncodedSmileys.Keys)
            {
                result = result.Replace(code, string.Format("<img src=\"http://www.globalcaching.eu/forum/images/emoticons/{0}\" />", _htmlEncodedSmileys[code]));
            }
            return result;
        }
    }
}