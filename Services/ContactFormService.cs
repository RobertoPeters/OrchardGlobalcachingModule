using Globalcaching.Models;
using Globalcaching.ViewModels;
using Orchard;
using Orchard.Messaging.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Globalcaching.Services
{
    public interface IContactFormService: IDependency
    {
        void SubmitContactForm(GCEuContactForm m);
        ContactFormModel GetContactForms(int page, int pageSize);
    }

    public class ContactFormService : IContactFormService
    {
        public static string dbGcEuDataConnString = ConfigurationManager.ConnectionStrings["GCEuDataConnectionString"].ToString();

        private readonly IMessageService _messageService;

        public ContactFormService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public void SubmitContactForm(GCEuContactForm m)
        {
            if (m.Name == "barnypok" || m.EMail == "jfvynms4281rt@hotmail.com")
            {
                return;
            }
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                m.Created = DateTime.Now;
                db.Insert(m);
            }
            var parameters = new Dictionary<string, object> {
                            {"Subject", m.Title},
                            {"Body", m.Comment},
                            {"Recipients", string.Format("{0},{1}", m.EMail, "globalcaching@gmail.com") }
                        };

            _messageService.Send("Email", parameters);
        }

        public ContactFormModel GetContactForms(int page, int pageSize)
        {
            ContactFormModel result = new ContactFormModel();
            result.PageCount = 1;
            result.CurrentPage = 1;
            using (PetaPoco.Database db = new PetaPoco.Database(dbGcEuDataConnString, "System.Data.SqlClient"))
            {
                var items = db.Page<GCEuContactForm>(page, pageSize, "select * from GCEuContactForm order by created desc");
                result.Items = items.Items.ToArray();
                result.CurrentPage = items.CurrentPage;
                result.PageCount = items.TotalPages;
                result.TotalCount = items.TotalItems;
            }
            return result;
        }

    }
}