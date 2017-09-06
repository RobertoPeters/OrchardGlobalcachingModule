using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;
using System.Data;

namespace Globalcaching
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterTypeDefinition("YafMostRecentPostsWidget",
                cfg => cfg
                    .DisplayedAs("Yaf Most Recent Posts")
                    .WithPart("YafMostRecentPostsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            ContentDefinitionManager.AlterTypeDefinition("GCComSearchUserWidget",
                cfg => cfg
                    .DisplayedAs("GCCom Search User")
                    .WithPart("GCComSearchUserPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 2;
        }

        public int UpdateFrom2()
        {
            ContentDefinitionManager.AlterTypeDefinition("YafMostRecentShoutWidget",
                cfg => cfg
                    .DisplayedAs("Yaf Most Recent Shout")
                    .WithPart("YafMostRecentShoutPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 3;
        }

        public int UpdateFrom3()
        {
            ContentDefinitionManager.AlterTypeDefinition("EditUserSettingsWidget",
                cfg => cfg
                    .DisplayedAs("Edit User Settings")
                    .WithPart("GCEuUserSettingsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 4;
        }

        public int UpdateFrom4()
        {
            ContentDefinitionManager.AlterTypeDefinition("GCComSearchLogsOfUserWidget",
               cfg => cfg
                   .DisplayedAs("GCCom Search Logs Of User")
                   .WithPart("GCComSearchLogsPart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget")
               );

            return 5;
        }

        public int UpdateFrom5()
        {
            ContentDefinitionManager.AlterTypeDefinition("LiveAPISettingsWidget",
               cfg => cfg
                   .DisplayedAs("Live API Settings")
                   .WithPart("LiveAPISettingsPart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget")
               );

            return 6;
        }

        public int UpdateFrom6()
        {
            ContentDefinitionManager.AlterTypeDefinition("QuickGeocacheSearchWidget",
               cfg => cfg
                   .DisplayedAs("Quick Geocache Search")
                   .WithPart("GCComQuickGeocacheSearchPart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget")
               );

            return 7;
        }

        public int UpdateFrom7()
        {
            ContentDefinitionManager.AlterTypeDefinition("SearchLogImagesWidget",
               cfg => cfg
                   .DisplayedAs("Search Log Images")
                   .WithPart("GCComSearchLogImagesPart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget")
               );

            return 8;
        }

        public int UpdateFrom8()
        {
            ContentDefinitionManager.AlterTypeDefinition("UsersOnlineWidget",
               cfg => cfg
                   .DisplayedAs("Users Online")
                   .WithPart("UsersOnlinePart")
                   .WithPart("CommonPart")
                   .WithPart("WidgetPart")
                   .WithSetting("Stereotype", "Widget")
               );

            return 9;
        }

        public int UpdateFrom9()
        {
            ContentDefinitionManager.AlterTypeDefinition("EditCCCSettingsWidget",
                cfg => cfg
                    .DisplayedAs("Edit CCC Settings")
                    .WithPart("GCEuCCCSettingsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 10;
        }

        public int UpdateFrom10()
        {
            ContentDefinitionManager.AlterTypeDefinition("CheckCCCWidget",
                cfg => cfg
                    .DisplayedAs("Check CCC")
                    .WithPart("CheckCCCPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 11;
        }

        public int UpdateFrom11()
        {
            ContentDefinitionManager.AlterTypeDefinition("StatisticsGeocachesPerYearWidget",
                cfg => cfg
                    .DisplayedAs("Statistics Geocaches Per Year")
                    .WithPart("StatisticsGeocachesPerYearPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 12;
        }

        public int UpdateFrom12()
        {
            ContentDefinitionManager.AlterTypeDefinition("GCEuGeocacheFilterMacroWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Filter Macro")
                    .WithPart("GCEuGeocacheFilterMacroPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 13;
        }

        public int UpdateFrom13()
        {
            ContentDefinitionManager.AlterTypeDefinition("SearchByAttributesWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Search By Attributes")
                    .WithPart("SearchByAttributesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 14;
        }

        public int UpdateFrom14()
        {
            ContentDefinitionManager.AlterTypeDefinition("AreaInformationWidget",
                cfg => cfg
                    .DisplayedAs("Area Information")
                    .WithPart("AreaInformationPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 15;
        }

        public int UpdateFrom15()
        {
            ContentDefinitionManager.AlterTypeDefinition("OnlineToolsWidget",
                cfg => cfg
                    .DisplayedAs("Online Tools")
                    .WithPart("OnlineToolsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 16;
        }

        public int UpdateFrom16()
        {
            ContentDefinitionManager.AlterTypeDefinition("TrackableBatchLogWidget",
                cfg => cfg
                    .DisplayedAs("Trackable Batch Log")
                    .WithPart("TrackableBatchLogPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 17;
        }

        public int UpdateFrom17()
        {
            ContentDefinitionManager.AlterTypeDefinition("FavoriteGeocachesWidget",
                cfg => cfg
                    .DisplayedAs("Favorite Geocaches")
                    .WithPart("FavoriteGeocachesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 18;
        }

        public int UpdateFrom18()
        {
            ContentDefinitionManager.AlterTypeDefinition("FavoriteGeocachersWidget",
                cfg => cfg
                    .DisplayedAs("Favorite Geocachers")
                    .WithPart("FavoriteGeocachersPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 19;
        }

        public int UpdateFrom19()
        {
            ContentDefinitionManager.AlterTypeDefinition("LogImageStatsWidget",
                cfg => cfg
                    .DisplayedAs("Log Image Stats")
                    .WithPart("LogImageStatsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 20;
        }

        public int UpdateFrom20()
        {
            ContentDefinitionManager.AlterTypeDefinition("GeocacheSeriesWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Series")
                    .WithPart("GeocacheSeriesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 21;
        }

        public int UpdateFrom21()
        {
            ContentDefinitionManager.AlterTypeDefinition("EventCalendarWidget",
                cfg => cfg
                    .DisplayedAs("Event Calendar")
                    .WithPart("EventCalendarPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 22;
        }

        public int UpdateFrom22()
        {
            ContentDefinitionManager.AlterTypeDefinition("LiveAPIDownloadWidget",
                cfg => cfg
                    .DisplayedAs("Live API Download")
                    .WithPart("LiveAPIDownloadPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 23;
        }

        public int UpdateFrom23()
        {
            ContentDefinitionManager.AlterTypeDefinition("FTFStatsWidget",
                cfg => cfg
                    .DisplayedAs("FTF Stats")
                    .WithPart("FTFStatsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 24;
        }

        public int UpdateFrom24()
        {
            ContentDefinitionManager.AlterTypeDefinition("GlobalcachingMessagesWidget",
                cfg => cfg
                    .DisplayedAs("Globalcaching Messages")
                    .WithPart("GlobalcachingMessagesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 25;
        }

        public int UpdateFrom25()
        {
            ContentDefinitionManager.AlterTypeDefinition("ParelVanDeMaandWidget",
                cfg => cfg
                    .DisplayedAs("Parel Van De Maand")
                    .WithPart("ParelVanDeMaandPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 26;
        }

        public int UpdateFrom26()
        {
            ContentDefinitionManager.AlterTypeDefinition("NewestCachesWidget",
                cfg => cfg
                    .DisplayedAs("Newest Caches")
                    .WithPart("NewestCachesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 27;
        }

        public int UpdateFrom27()
        {
            ContentDefinitionManager.AlterTypeDefinition("FeatureTilesWidget",
                cfg => cfg
                    .DisplayedAs("Feature Tiles")
                    .WithPart("FeatureTilesPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 28;
        }

        public int UpdateFrom28()
        {
            ContentDefinitionManager.AlterTypeDefinition("FTFBannerWidget",
                cfg => cfg
                    .DisplayedAs("FTF Banner")
                    .WithPart("FTFBannerPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 29;
        }

        public int UpdateFrom29()
        {
            ContentDefinitionManager.AlterTypeDefinition("ContactFormWidget",
                cfg => cfg
                    .DisplayedAs("Contact Form")
                    .WithPart("ContactFormPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 30;
        }

        public int UpdateFrom30()
        {
            ContentDefinitionManager.AlterTypeDefinition("CoordCheckerCheckWidget",
                cfg => cfg
                    .DisplayedAs("Coord Checker Check")
                    .WithPart("CoordCheckerCheckPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 31;
        }

        public int UpdateFrom31()
        {
            ContentDefinitionManager.AlterTypeDefinition("CoordCheckerMaintWidget",
                cfg => cfg
                    .DisplayedAs("Coord Checker Maint")
                    .WithPart("CoordCheckerMaintPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 32;
        }

        public int UpdateFrom32()
        {
            ContentDefinitionManager.AlterTypeDefinition("CodeCheckerCheckWidget",
                cfg => cfg
                    .DisplayedAs("Code Checker Check")
                    .WithPart("CodeCheckerCheckPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 33;
        }

        public int UpdateFrom33()
        {
            ContentDefinitionManager.AlterTypeDefinition("CodeCheckerMaintWidget",
                cfg => cfg
                    .DisplayedAs("Code Checker Maint")
                    .WithPart("CodeCheckerMaintPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 34;
        }

        public int UpdateFrom34()
        {
            ContentDefinitionManager.AlterTypeDefinition("LogCorrectionWidget",
                cfg => cfg
                    .DisplayedAs("Log Correction")
                    .WithPart("LogCorrectionPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 35;
        }

        public int UpdateFrom35()
        {
            ContentDefinitionManager.AlterTypeDefinition("YafMostRecentContentWidget",
                cfg => cfg
                    .DisplayedAs("Yaf Most Recent Content")
                    .WithPart("YafMostRecentContentPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 36;
        }

        public int UpdateFrom36()
        {
            ContentDefinitionManager.AlterTypeDefinition("SelectionBuilderWidget",
                cfg => cfg
                    .DisplayedAs("Selection Builder")
                    .WithPart("SelectionBuilderPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 37;
        }

        public int UpdateFrom37()
        {
            ContentDefinitionManager.AlterTypeDefinition("TrackableGroupMaintWidget",
                cfg => cfg
                    .DisplayedAs("Trackable Group Maint")
                    .WithPart("TrackableGroupMaintPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 38;
        }

        public int UpdateFrom38()
        {
            ContentDefinitionManager.AlterTypeDefinition("TrackableGroupsWidget",
                cfg => cfg
                    .DisplayedAs("Trackable Groups")
                    .WithPart("TrackableGroupsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 39;
        }

        public int UpdateFrom39()
        {
            ContentDefinitionManager.AlterTypeDefinition("FoundsPerCountryRankingWidget",
                cfg => cfg
                    .DisplayedAs("Founds Per Country Ranking")
                    .WithPart("FoundsPerCountryRankingPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 40;
        }

        public int UpdateFrom40()
        {
            ContentDefinitionManager.AlterTypeDefinition("CombiRankingWidget",
                cfg => cfg
                    .DisplayedAs("Combi Ranking")
                    .WithPart("CombiRankingPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 41;
        }

        public int UpdateFrom41()
        {
            ContentDefinitionManager.AlterTypeDefinition("FoundsPerCountryBannerWidget",
                cfg => cfg
                    .DisplayedAs("Founds Per Country Banner")
                    .WithPart("FoundsPerCountryBannerPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 42;
        }

        public int UpdateFrom42()
        {
            ContentDefinitionManager.AlterTypeDefinition("GeocacheBatchLogWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Batch Log")
                    .WithPart("GeocacheBatchLogPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 43;
        }

        public int UpdateFrom43()
        {
            ContentDefinitionManager.AlterTypeDefinition("GeocacheTypeStatsWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Type Stats")
                    .WithPart("GeocacheTypeStatsPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 44;
        }

        public int UpdateFrom44()
        {
            ContentDefinitionManager.AlterTypeDefinition("CombiBannerWidget",
                cfg => cfg
                    .DisplayedAs("Combi Banner")
                    .WithPart("CombiBannerPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 45;
        }

        public int UpdateFrom45()
        {
            ContentDefinitionManager.AlterTypeDefinition("ForDonatorsOnlyWidget",
                cfg => cfg
                    .DisplayedAs("For Donators Only")
                    .WithPart("ForDonatorsOnlyPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 46;
        }

        public int UpdateFrom46()
        {
            ContentDefinitionManager.AlterTypeDefinition("GeocacheMaintenanceWidget",
                cfg => cfg
                    .DisplayedAs("Geocache Maintenance")
                    .WithPart("GeocacheMaintenancePart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 47;
        }

        public int UpdateFrom47()
        {
            ContentDefinitionManager.AlterTypeDefinition("BookmarksWidget",
                cfg => cfg
                    .DisplayedAs("Bookmarks")
                    .WithPart("BookmarksPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 48;
        }

        public int UpdateFrom48()
        {
            SchemaBuilder.CreateTable("FinancialYearStatusRecord", table => table
                            .ContentPartRecord()
                            .Column("Year", DbType.Int32)
                            .Column("TotalCosts", DbType.Double)
                        );

            ContentDefinitionManager.AlterTypeDefinition("FinancialYearStatusWidget",
                cfg => cfg
                    .DisplayedAs("FinancialYearStatus")
                    .WithPart("FinancialYearStatusPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 49;
        }

        public int UpdateFrom49()
        {
            ContentDefinitionManager.AlterTypeDefinition("LiveAPILogDownloadWidget",
                cfg => cfg
                    .DisplayedAs("LiveAPILogDownload")
                    .WithPart("LiveAPILogDownloadPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 50;
        }

        public int UpdateFrom50()
        {
            ContentDefinitionManager.AlterTypeDefinition("LiveAPILogSearchWidget",
                cfg => cfg
                    .DisplayedAs("LiveAPILogSearch")
                    .WithPart("LiveAPILogSearchPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 51;
        }

        public int UpdateFrom51()
        {
            ContentDefinitionManager.AlterTypeDefinition("ShopUserProductWidget",
                cfg => cfg
                    .DisplayedAs("ShopUserProduct")
                    .WithPart("ShopUserProductPart")
                    .WithPart("CommonPart")
                    .WithPart("WidgetPart")
                    .WithSetting("Stereotype", "Widget")
                );

            return 52;
        }

    }
}