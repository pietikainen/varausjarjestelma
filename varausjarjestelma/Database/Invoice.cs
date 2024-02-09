using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Invoice
    {
        [Key]
        public int lasku_id { get; set; }
        public int varaus_id { get; set; }
        public double summa { get; set; }
        public double alv { get; set; }
        public int maksettu { get; set; }
    }
}
