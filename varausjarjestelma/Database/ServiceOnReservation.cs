using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class ServiceOnReservation
    {
        [Key]
        public int varaus_id { get; set; }
        public int palvelu_id { get; set; }
        public int lkm { get; set; }
    }
}
