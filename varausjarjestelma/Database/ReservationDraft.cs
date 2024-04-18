using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using varausjarjestelma.Database;
using varausjarjestelma.Controller;

namespace varausjarjestelma.Database
{
    public class ReservationDraft
    {
        public Customer customer { get; set; }
        public Cabin cabin { get; set; }
        public List<ServiceOnReservation> services{ get; set; }




    }


}
