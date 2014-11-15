using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    /*
create table GCEuUserSettings
(
YafUserID int not null,
GCComUserID bigint,
LiveAPIToken nvarchar(255),
ShowGeocachesOnGlobal bit,
HomelocationLat float,
HomelocationLon float,
DefaultCountryCode int
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [GCEuUserSettings_YafUserID] ON [dbo].[GCEuUserSettings] 
(
	[YafUserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

     */
    [PetaPoco.TableName("GCEuUserSettings")]
    public class GCEuUserSettings
    {
        public int YafUserID { get; set; }
        public long? GCComUserID { get; set; }
        public string LiveAPIToken { get; set; }
        public bool? ShowGeocachesOnGlobal { get; set; }
        public double? HomelocationLat { get; set; }
        public double? HomelocationLon { get; set; }
        public int? DefaultCountryCode { get; set; }
        public string MarkLogTextColor1 { get; set; }
        public string MarkLogTextColor2 { get; set; }
        public string MarkLogTextColor3 { get; set; }
    }
}