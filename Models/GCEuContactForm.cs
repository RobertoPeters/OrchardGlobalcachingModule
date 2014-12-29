using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Globalcaching.Models
{
    [PetaPoco.TableName("GCEuContactForm")]
    [PetaPoco.PrimaryKey("ID")]
    public class GCEuContactForm
    {
        public int ID { get; set; }
        public DateTime Created { get; set; }
        [StringLength(50), Required]
        public string Name { get; set; }
        [StringLength(50), Required]
        public string EMail { get; set; }
        [StringLength(50), Required]
        public string Title { get; set; }
        [StringLength(4000), Required]
        public string Comment { get; set; }
    }
}