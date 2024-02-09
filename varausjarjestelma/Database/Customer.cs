using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Customer
    {
        [Key]
        public int asiakas_id { get; set; }
        public string postinro { get; set; }
        public string etunimi { get; set; }
        public string sukunimi { get; set; }
        public string lahiosoite { get; set; }
        public string email { get; set; }
        public string puhelinnro { get; set; }
    
    public string FullName
    {
        get { return $"{etunimi} {sukunimi}"; }
    }
    
    }

}
