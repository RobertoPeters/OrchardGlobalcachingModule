using Globalcaching.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Core
{
    public class CachedData
    {
        private static CachedData _uniqueInstance = null;
        private static object _lockObject = new object();
        public static string dbGcComDataConnString = ConfigurationManager.ConnectionStrings["GCComDataConnectionString"].ToString();

        public class GeocacheStateInfo
        {
            public int StateID { get; set; }
            public string State { get; set; }
        }

        public List<GeocacheStateInfo> StatesInfo { get; private set; }
        public List<GCComAttributeType> AttributesInfo { get; private set; }
        public List<GCComGeocacheType> GeocacheTypesFilter { get; private set; }       

        private CachedData()
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcComDataConnString, "System.Data.SqlClient"))
            {
                AttributesInfo = db.Fetch<GCComAttributeType>("order by id");
                StatesInfo = db.Fetch<GeocacheStateInfo>("select distinct StateID, State from GCComGeocache order by StateID");
                GeocacheTypesFilter = db.Fetch<GCComGeocacheType>("where ID in (2, 3, 4, 5, 6, 8, 11, 12, 15, 137, 1858) order by ID");
            }
        }

        public static CachedData Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new CachedData();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }
    }
}