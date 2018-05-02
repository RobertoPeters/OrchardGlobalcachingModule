using Globalcaching.Models;
using ICSharpCode.SharpZipLib.Zip;
using Orchard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Globalcaching.Services
{
    public interface IGDPRService : IDependency
    {
        void DownloadData(HttpResponseBase Response);
    }

    public class GDPRService: IGDPRService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();
        public readonly IGCEuUserSettingsService _gcEuUserSettingsService;

        public GDPRService(IGCEuUserSettingsService gcEuUserSettingsService)
        {
            _gcEuUserSettingsService = gcEuUserSettingsService;
        }

        public void DownloadData(HttpResponseBase Response)
        {
            var usr = _gcEuUserSettingsService.GetSettings();
            if (usr.YafUserID > 0)
            {
                Response.ContentType = "application/zip";
                Response.AppendHeader("content-disposition", "attachment;filename=4GeocachingData.zip");
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream oZipStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(Response.OutputStream))
                {
                    oZipStream.SetLevel(9); // maximum compression

                    StringBuilder sb = new StringBuilder();

                    ZipEntry oZipEntry = new ZipEntry("inhoud.txt");
                    sb.Append(@"4Geocaching data:
- www: alle data op https://www.4geocaching.eu
- forum: alle data op https://forum.4geocaching.eu
- chat: alle data op https://chat.4geocaching.eu

4Geocaching.eu heeft een data overeenkomst met Geocaching HQ (zie https://www.4geocaching.eu/wiezijnwij (Data License Agreement))
Indien je een koppeling hebt gemaakt met geocaching.com middels de Live API autorisatie, dan is er een koppeling met jouw account op 4Geocaching en jouw account op geocaching.com.
Deze data is niet toegevoegd, maar kun je opvragen bij geocaching.com.
");
                    byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(sb.ToString());
                    oZipEntry.Size = obuffer.Length;
                    oZipStream.PutNextEntry(oZipEntry);
                    oZipStream.Write(obuffer, 0, obuffer.Length);

                    using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
                    {
                        DownloadWWWData(db, usr.YafUserID, oZipStream);
                        DownloadForumData(db, usr.YafUserID, oZipStream);
                        DownloadChatData(db, usr.YafUserID, oZipStream);
                    }

                    oZipStream.Finish();
                    oZipStream.Flush();
                    oZipStream.Close();
                }
                Response.Flush();
                Response.End();
            }
        }

        private string SerializeObject<T>(T toSerialize)
        {
            if (toSerialize is System.Dynamic.ExpandoObject)
            {
                return (DynamicHelper.ToXmlString(toSerialize));
            }
            else if (toSerialize is List<dynamic>)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<objects>");
                foreach (var record in toSerialize as List<dynamic>)
                {
                    sb.AppendLine(DynamicHelper.ToXmlString(record));
                }
                sb.AppendLine("</objects>");
                return sb.ToString();
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

                using (StringWriter textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, toSerialize);
                    return textWriter.ToString();
                }
            }
        }

        private void AddObjectToStream<T>(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs, string filename, T objectToAdd)
        {
            ZipEntry ze = new ZipEntry(filename);
            var s = SerializeObject(objectToAdd);
            byte[] obuffer = System.Text.ASCIIEncoding.UTF8.GetBytes(s);
            ze.Size = obuffer.Length;
            zs.PutNextEntry(ze);
            zs.Write(obuffer, 0, obuffer.Length);
        }

        private void AddFileDataToStream(ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs, string filename, byte[] content)
        {
            ZipEntry ze = new ZipEntry(filename);
            ze.Size = content.Length;
            zs.PutNextEntry(ze);
            zs.Write(content, 0, content.Length);
        }

        private void DownloadWWWData(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            DownloadWWWGCEuUserSettings(db, yafUsrId, zs);
            DownloadWWWGCEuCCCUser(db, yafUsrId, zs);
            DownloadWWWGCEuCCCRequest(db, yafUsrId, zs);
            DownloadWWWGCEuCodeChecker(db, yafUsrId, zs);
            DownloadWWWGCEuCoordChecker(db, yafUsrId, zs);
            DownloadWWWGCEuDonations(db, yafUsrId, zs);
            DownloadWWWGCEuFilterMacro(db, yafUsrId, zs);
            DownloadWWWGCEuSelectionBuilders(db, yafUsrId, zs);
            DownloadWWWGCEuServerCalls(db, yafUsrId, zs);
        }

        private void DownloadForumData(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            DownloadForumAttachements(db, yafUsrId, zs);
            DownloadForumBuddy(db, yafUsrId, zs);
            DownloadForumMessage(db, yafUsrId, zs);
            DownloadForumMessageHistory(db, yafUsrId, zs);
            DownloadForumTopic(db, yafUsrId, zs);
            DownloadForumPMessage(db, yafUsrId, zs);
            DownloadForumPoll(db, yafUsrId, zs);
            DownloadForumShoutMessage(db, yafUsrId, zs);
            DownloadForumAlbum(db, yafUsrId, zs);
            DownloadForumProfile(db, yafUsrId, zs);
        }

        private void DownloadChatData(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var record = db.FirstOrDefault<dynamic>(@"select * FROM [Jabbr].[dbo].[ChatUsers] where ChatUsers.Name=@0", _gcEuUserSettingsService.GetName(yafUsrId));
            if (record != null)
            {
                int userKey = record.Key;
                DownloadChatMessages(db, userKey, zs);
            }
        }

        private void DownloadChatMessages(PetaPoco.Database db, int userKey, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Jabbr].[dbo].[ChatMessages] where ChatMessages.User_Key=@0", userKey);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "chat/messages.xml", records);
            }
        }

        private void DownloadForumProfile(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var record = db.FirstOrDefault<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_UserProfile] where yaf_UserProfile.UserID=@0", yafUsrId);
            AddObjectToStream(zs, "forum/profiel.xml", record);
        }

        private void DownloadForumAlbum(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_UserAlbum] where yaf_UserAlbum.UserID=@0 order by Updated", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/album/albums.xml", records);

                foreach (var record in records)
                {
                    var records2 = db.Fetch<dynamic>("select * from  Globalcaching.dbo.yaf_UserAlbumImage where yaf_UserAlbumImage.AlbumID=@0", record.AlbumID);
                    if (records2.Count > 0)
                    {
                        AddObjectToStream(zs, string.Format("forum/album/albums{0}.xml", record.AlbumID), records2);

                        foreach (var record2 in records2)
                        {
                            var f = Path.Combine(@"c:\inetpub\ForumGlobalcaching\forum\uploads", string.Format("{0}.{1}.{2}.yafalbum", yafUsrId, record.AlbumID, record2.FileName));
                            if (File.Exists(f))
                            {
                                AddFileDataToStream(zs, string.Format("forum/album/{0}/{1}", (int)record.AlbumID, (string)record2.FileName), File.ReadAllBytes(f));
                            }
                        }
                    }
                }
            }
        }

        private void DownloadForumTopic(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_Topic] where yaf_Topic.UserID=@0 order by Posted", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/onderwerpen.xml", records);
            }
        }

        private void DownloadForumShoutMessage(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_ShoutboxMessage] where yaf_ShoutboxMessage.UserID=@0 order by Date", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/shoutbox.xml", records);
            }
        }

        private void DownloadForumPoll(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_Poll] where yaf_Poll.UserID=@0 order by PollID", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/polls.xml", records);
            }
        }

        private void DownloadForumPMessage(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_PMessage] where yaf_PMessage.FromUserID=@0 order by Created", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/persoonlijkeberichten.xml", records);
            }
        }

        private void DownloadForumMessage(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_Message] where yaf_Message.UserID=@0 order by Posted", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/posts.xml", records);
            }
        }

        private void DownloadForumMessageHistory(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select * FROM [Globalcaching].[dbo].[yaf_MessageHistory] where yaf_MessageHistory.EditedBy=@0 order by Edited", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/postgeschiedenis.xml", records);
            }
        }

        private void DownloadForumBuddy(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<dynamic>(@"select [ID]
      ,[FromUserID]
      ,[ToUserID]
      ,[Approved]
      ,[Requested]
      , a.Name as FromUserName
      , b.Name as ToUserName
FROM [Globalcaching].[dbo].[yaf_Buddy]
  inner join [Globalcaching].[dbo].[yaf_User] a on [Globalcaching].[dbo].[yaf_Buddy].FromUserID = a.UserID
  inner join [Globalcaching].[dbo].[yaf_User] b on [Globalcaching].[dbo].[yaf_Buddy].ToUserID = b.UserID
where yaf_Buddy.FromUserID=@0 or yaf_Buddy.ToUserID=@1", yafUsrId, yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "forum/vrienden.xml", records);
            }
        }

        private void DownloadForumAttachements(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Query<dynamic>("select * from  Globalcaching.dbo.yaf_Attachment where yaf_Attachment.UserID=@0 and yaf_Attachment.FileData is not NULL", yafUsrId);
            foreach(var record in records)
            {
                AddFileDataToStream(zs, string.Format("forum/attachements/{0}/{1}", (int)record.AttachmentID, (string)record.FileName), (byte[])record.FileData);
                record.FileData = null;
                AddObjectToStream(zs, string.Format("forum/attachements/{0}/_info_.xml", record.AttachmentID), record);
            }
        }


        private void DownloadWWWGCEuUserSettings(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var record = db.FirstOrDefault<GCEuUserSettings>("select * from  GCEuData.dbo.GCEuUserSettings where GCEuUserSettings.YafUserID=@0", yafUsrId);
            record.LiveAPIToken = "--geheim--";
            AddObjectToStream(zs, "www/settings.xml", record);
        }

        private void DownloadWWWGCEuCCCUser(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var record = db.FirstOrDefault<GCEuCCCUser>("select * from  GCEuData.dbo.GCEuCCCUser where GCEuCCCUser.UserID=@0", yafUsrId);
            if (record != null)
            {
                AddObjectToStream(zs, "www/cccsettings.xml", record);
            }
        }

        private void DownloadWWWGCEuCCCRequest(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuCCCRequest>("select * from  GCEuData.dbo.GCEuCCCRequest where GCEuCCCRequest.UserName=@0", _gcEuUserSettingsService.GetName(yafUsrId));
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/cccverzoeken.xml", records);
            }
        }

        private void DownloadWWWGCEuCodeChecker(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuCodeCheckCode>("select * from  GCEuData.dbo.GCEuCodeCheckCode where GCEuCodeCheckCode.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/codecheckers.xml", records);
            }
        }

        private void DownloadWWWGCEuCoordChecker(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuCoordCheckCode>("select * from  GCEuData.dbo.GCEuCoordCheckCode where GCEuCoordCheckCode.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/coordcheckers.xml", records);
            }
        }

        private void DownloadWWWGCEuDonations(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuDonations>("select * from  GCEuData.dbo.GCEuDonations where GCEuDonations.UserID=@0 or GCEuDonations.PartnerUserID=@1", yafUsrId, yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/donaties.xml", records);
            }
        }

        private void DownloadWWWGCEuFilterMacro(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuGeocacheFilterMacro>("select * from  GCEuData.dbo.GCEuGeocacheFilterMacro where GCEuGeocacheFilterMacro.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/filtermacros.xml", records);
            }
        }

        private void DownloadWWWGCEuSelectionBuilders(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuSelectionBuilder>("select * from  GCEuData.dbo.GCEuSelectionBuilder where GCEuSelectionBuilder.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/selectionbuilders.xml", records);
            }
        }

        private void DownloadWWWGCEuServerCalls(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuServiceCall>("select * from  GCEuData.dbo.GCEuServiceCall where GCEuServiceCall.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/serviceaanroepen.xml", records);
            }
        }

        private void DownloadWWWGCEuTrackableGroups(PetaPoco.Database db, int yafUsrId, ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs)
        {
            var records = db.Fetch<GCEuTrackableGroup>("select * from  GCEuData.dbo.GCEuTrackableGroup where GCEuTrackableGroup.UserID=@0", yafUsrId);
            if (records.Count > 0)
            {
                AddObjectToStream(zs, "www/trackablegroepen.xml", records);
            }
        }
    }
}