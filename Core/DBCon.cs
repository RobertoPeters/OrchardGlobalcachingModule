using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Globalcaching.Core
{
    public class DBCon : IDisposable
    {
        public static string dbForumConnString = ConfigurationManager.ConnectionStrings["GlobalConnectionString"].ToString();

        private SqlConnection _dbcon = null;
        private SqlCommand _cmd = null;
        private SqlDataReader _rdr = null;

        public DBCon(string connectionString)
        {
            _dbcon = new SqlConnection(connectionString);
            _dbcon.Open();
        }

        public SqlConnection Connection
        {
            get
            {
                return _dbcon;
            }
        }

        public SqlCommand Command
        {
            get
            {
                if (_cmd == null)
                {
                    _cmd = _dbcon.CreateCommand();
                }
                return _cmd;
            }
        }

        public SqlDataReader ExecuteReader(string command)
        {
            if (_rdr != null && !_rdr.IsClosed)
            {
                _rdr.Close();
            }
            Command.CommandText = command;
            _rdr = _cmd.ExecuteReader();
            return _rdr;
        }

        public object ExecuteScalar(string command)
        {
            if (_rdr != null && !_rdr.IsClosed)
            {
                _rdr.Close();
            }
            Command.CommandText = command;
            return _cmd.ExecuteScalar();
        }

        public int ExecuteNonQuery(string command)
        {
            if (_rdr != null && !_rdr.IsClosed)
            {
                _rdr.Close();
            }
            Command.CommandText = command;
            return _cmd.ExecuteNonQuery();
        }

        public SqlDataReader GetCurrentSiteVisitors(int secondsThreshold, int userid, string sessionid)
        {
            SqlDataReader result = null;
            if (_rdr != null && !_rdr.IsClosed)
            {
                _rdr.Close();
            }
            DateTime thresholdTime = DateTime.Now - (new TimeSpan(0, 0, secondsThreshold));

            Command.CommandText = "GetCurrentVisitorInfo2";
            Command.CommandType = CommandType.StoredProcedure;

            Command.Parameters.Add(new SqlParameter("@BoardID", SqlDbType.Int));
            Command.Parameters["@BoardID"].Value = 1;
            Command.Parameters.Add(new SqlParameter("@Userid", SqlDbType.Int));
            Command.Parameters["@Userid"].Value = userid;
            Command.Parameters.Add(new SqlParameter("@sessionid", SqlDbType.NVarChar));
            Command.Parameters["@sessionid"].Value = sessionid;
            Command.Parameters.Add(new SqlParameter("@currenttime", SqlDbType.NVarChar));
            Command.Parameters["@currenttime"].Value = DateTime.Now.ToString("u");
            Command.Parameters.Add(new SqlParameter("@thresholdtime", SqlDbType.NVarChar));
            Command.Parameters["@thresholdtime"].Value = thresholdTime.ToString("u");

            result = Command.ExecuteReader();
            return result;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_rdr != null)
            {
                if (!_rdr.IsClosed)
                {
                    _rdr.Close();
                }
                _rdr.Dispose();
                _rdr = null;
            }
            if (_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }
            if (_dbcon != null)
            {
                _dbcon.Close();
                _dbcon = null;
            }
        }

        #endregion
    }

}