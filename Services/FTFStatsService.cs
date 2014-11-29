using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IFTFStatsService : IDependency
    {
        FTFStatsModel GetFTFRanking(string UserName, int Jaar, int RankingType, int page, int pageSize);
        FTFAdminModel GetUnassignedFTF(int page, int pageSize);
        bool SetFTFCompleted(long id);
        bool ClearFTFAssignment(long id);
        bool ClearSTFAssignment(long id);
        bool ClearTTFAssignment(long id);
        bool SetFTFAssignment(long geocacheId, long logId);
        bool SetSTFAssignment(long geocacheId, long logId);
        bool SetTTFAssignment(long geocacheId, long logId);
        void ResetFTFCounter(long id);
    }

    public class FTFStatsService : IFTFStatsService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        public FTFStatsService()
        {
        }

        public void ResetFTFCounter(long id)
        {
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                if (id > 0)
                {
                    db.Execute("update GCEuGeocache set FTFFoundCount = FoundCount where ID=@0", id);
                }
                else
                {
                    db.Execute("update GCEuGeocache set FTFFoundCount = FoundCount");
                }
            }
        }

        public bool SetFTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId!=null)
                {
                    result = db.Execute("update GCEuGeocache set FTFUserID = @0, FTFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }
        public bool SetSTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId != null)
                {
                    result = db.Execute("update GCEuGeocache set STFUserID = @0, STFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }
        public bool SetTTFAssignment(long geocacheId, long logId)
        {
            bool result = false;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                GCComGeocacheLog l = db.FirstOrDefault<GCComGeocacheLog>("select * from GCComData.dbo.GCComGeocacheLog where ID=@0", logId);
                if (l != null && l.FinderId != null)
                {
                    result = db.Execute("update GCEuGeocache set TTFUserID = @0, TTFAtDate = @1 where ID=@2", l.FinderId, l.VisitDate, geocacheId) > 0;
                }
            }
            return result;
        }

        public bool ClearFTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set FTFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }
        public bool ClearSTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set STFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }
        public bool ClearTTFAssignment(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set TTFUserID = NULL where ID=@0", id) > 0;
            }
            return result;
        }

        public bool SetFTFCompleted(long id)
        {
            bool result;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                result = db.Execute("update GCEuGeocache set FTFCompleted=1 where ID=@0", id) > 0;
            }
            return result;
        }

        public FTFAdminModel GetUnassignedFTF(int page, int pageSize)
        {
            FTFAdminModel result = new FTFAdminModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Fetch<FTFAdminInfo>("select GCEuGeocache.ID, GCEuGeocache.FTFUserID, GCEuGeocache.STFUserID, GCEuGeocache.TTFUserID, GCEuGeocache.FoundCount, GCEuGeocache.FTFFoundCount, GCEuGeocache.PublishedAtDate, GCComGeocache.Code, GCComGeocacheType.GeocacheTypeName from GCEuGeocache inner join GCComData.dbo.GCComGeocache on GCComGeocache.ID = GCEuGeocache.ID inner join GCComData.dbo.GCComGeocacheType on GCComGeocacheType.ID = GCComGeocache.GeocacheTypeId where (FTFUserID is null or STFUserID is null or TTFUserID is null) and GCEuGeocache.FTFCompleted = 0 and GCComGeocache.CountryID = 141");
                foreach (var g in items)
                {
                    g.NewLogs = g.FoundCount - g.FTFFoundCount;
                }
                result.Geocaches = items.OrderByDescending(x => x.NewLogs).ThenBy(x => x.PublishedAtDate).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                result.CurrentPage = page;
                result.TotalCount = items.Count();
                result.PageCount = result.TotalCount / pageSize;
                if (result.TotalCount > 0 && (result.TotalCount % pageSize) != 0)
                {
                    result.PageCount++;
                }
            }
            return result;
        }

        public FTFStatsModel GetFTFRanking(string UserName, int Jaar, int RankingType, int page, int pageSize)
        {
            FTFStatsModel result = new FTFStatsModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            result.Jaar = Jaar;
            result.UserName = UserName ?? "";
            result.RankingType = RankingType;

            string whereUserClause;
            if (string.IsNullOrEmpty(UserName))
            {
                whereUserClause = "";
            }
            else
            {
                whereUserClause = string.Format(" and GCComData.dbo.GCComUser.UserName like '%{0}%'", UserName.Replace("'", "''").Replace("@", "@@"));
            }
            string orderClause;
            if (RankingType == 0)
            {
                orderClause = "PositionPoints";
            }
            else
            {
                orderClause = "Position";
            }
            string yearClause;
            if (Jaar > 2000)
            {
                yearClause = string.Format("Jaar = {0}", Jaar);
            }
            else
            {
                yearClause = "Jaar is null";
            }
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<FTFStatsInfo>(page, pageSize, string.Format("select GCEuFTFStats.*, GCComUser.AvatarUrl, GCComUser.FindCount, GCComUser.PublicGuid, GCComUser.UserName from GCEuFTFStats inner join GCComData.dbo.GCComUser on GCEuFTFStats.UserID = GCComData.dbo.GCComUser.ID where {0} {1} order by {2}", yearClause, whereUserClause, orderClause));
                result.FTFInfo = items.Items.ToList();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}