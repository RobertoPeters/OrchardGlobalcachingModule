using Orchard.ContentManagement.MetaData;
using Orchard.Data.Migration;

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

    }
}