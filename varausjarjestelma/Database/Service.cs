using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Service
    {
        [Key]
        public int palvelu_id { get; set; }
        public int alue_id { get; set; }
        public string nimi { get; set; }
        public string tyyppi { get; set; }
        public string kuvaus { get; set; }
        public double hinta { get; set; }
        public double alv { get; set; }
    }
}
