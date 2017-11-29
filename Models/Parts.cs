using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    public class ShopArticleRecord : ContentPartRecord
    {
        public virtual int ArticleNumber { get; set; }
        public virtual string Comment { get; set; }
    }

    public class ShopArticlePart : ContentPart<ShopArticleRecord>
    {
        [Required]
        public int ArticleNumber
        {
            get { return Retrieve(r => r.ArticleNumber); }
            set { Store(r => r.ArticleNumber, value); }
        }

        public string Comment
        {
            get { return Retrieve(r => r.Comment); }
            set { Store(r => r.Comment, value); }
        }
    }

    public class FinancialYearStatusRecord : ContentPartRecord
    {
        public virtual int Year { get; set; }
        public virtual double TotalCosts { get; set; }
    }

    public class FinancialYearStatusPart : ContentPart<FinancialYearStatusRecord>
    {
        [Required]
        public int Year
        {
            get { return Retrieve(r => r.Year); }
            set { Store(r => r.Year, value); }
        }

        [Required]
        public double TotalCosts
        {
            get { return Retrieve(r => r.TotalCosts); }
            set { Store(r => r.TotalCosts, value); }
        }
    }

    public class ShopContactUserProductPart : ContentPart
    {
    }

    public class ShopUserProductPart : ContentPart
    {
    }

    public class LiveAPILogSearchPart : ContentPart
    {
    }

    public class LiveAPILogDownloadPart : ContentPart
    {
    }

    public class BookmarksPart : ContentPart
    {
    }

    public class GeocacheMaintenancePart : ContentPart
    {
    }

    public class ForDonatorsOnlyPart : ContentPart
    {
    }

    public class CombiBannerPart : ContentPart
    {
    }

    public class GeocacheTypeStatsPart : ContentPart
    {
    }

    public class GeocacheBatchLogPart : ContentPart
    {
    }

    public class FoundsPerCountryBannerPart : ContentPart
    {
    }

    public class CombiRankingPart : ContentPart
    {
    }

    public class FoundsPerCountryRankingPart : ContentPart
    {
    }

    public class TrackableGroupsPart : ContentPart
    {
    }

    public class TrackableGroupMaintPart : ContentPart
    {
    }

    public class SelectionBuilderPart : ContentPart
    {
    }

    public class YafMostRecentContentPart : ContentPart
    {
    }

    public class LogCorrectionPart : ContentPart
    {
    }

    public class CodeCheckerMaintPart : ContentPart
    {
    }

    public class CodeCheckerCheckPart : ContentPart
    {
    }

    public class CoordCheckerMaintPart : ContentPart
    {
    }

    public class CoordCheckerCheckPart : ContentPart
    {
    }

    public class ContactFormPart : ContentPart
    {
    }

    public class FTFBannerPart : ContentPart
    {
    }

    public class FeatureTilesPart : ContentPart
    {
    }

    public class NewestCachesPart : ContentPart
    {
    }

    public class ParelVanDeMaandPart : ContentPart
    {
    }

    public class GlobalcachingMessagesPart : ContentPart
    {
    }

    public class FTFStatsPart : ContentPart
    {
    }

    public class LiveAPIDownloadPart : ContentPart
    {
    }

    public class EventCalendarPart : ContentPart
    {
    }

    public class GeocacheSeriesPart : ContentPart
    {
    }

    public class LogImageStatsPart : ContentPart
    {
    }

    public class FavoriteGeocachesPart : ContentPart
    {
    }

    public class FavoriteGeocachersPart : ContentPart
    {
    }

    public class TrackableBatchLogPart : ContentPart
    {
    }

    public class OnlineToolsPart : ContentPart
    {
    }

    public class AreaInformationPart : ContentPart
    {
    }

    public class SearchByAttributesPart : ContentPart
    {
    }

    public class YafMostRecentPostsPart : ContentPart
    {
    }

    public class YafMostRecentShoutPart : ContentPart
    {
    }

    public class CheckCCCPart : ContentPart
    {
    }

    public class GCComQuickGeocacheSearchPart : ContentPart
    {
    }

    public class GCComSearchLogImagesPart : ContentPart
    {
    }

    public class GCComSearchLogsPart : ContentPart
    {
    }

    public class GCComSearchUserPart : ContentPart
    {
    }

    public class GCEuCCCSettingsPart : ContentPart
    {
    }

    public class GCEuGeocacheFilterMacroPart : ContentPart
    {
    }

    public class GCEuUserSettingsPart : ContentPart
    {
    }

    public class LiveAPISettingsPart : ContentPart
    {
    }

    public class StatisticsGeocachesPerYearPart : ContentPart
    {
    }

    public class UsersOnlinePart : ContentPart
    {
    }

}