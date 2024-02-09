using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Area
    {
        [Key]
        public int alue_id { get; set; }
        public string nimi { get; set; }
    }
}
