using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class PostalCode
    {
        [Key]
        public int postinro { get; set; }
        public string toimipaikka { get; set; }
    }
}
