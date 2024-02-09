using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Reservation
    {
        [Key]
        public int varaus_id { get; set; }
        public int asiakas_id { get; set; }
        public int mokki_mokki_id { get; set; }
        public DateTime varattu_pvm { get; set; }
        public DateTime vahvistus_pvm { get; set; }
        public DateTime varattu_alkupvm { get; set; }
        public DateTime varattu_loppupvm { get; set; }
    }
}
