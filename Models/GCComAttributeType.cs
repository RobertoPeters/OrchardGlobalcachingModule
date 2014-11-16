using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCComAttributeType")]
    public class GCComAttributeType
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public bool HasNoOption { get; set; }
        public bool HasYesOption { get; set; }
        public string IconName { get; set; }
        public string Name { get; set; }
        public string NoIconName { get; set; }
        public string NotChosenIconName { get; set; }
        public string YesIconName { get; set; }
    }
}