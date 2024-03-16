using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //connection parameters
       [ForeignKey("postinro")]
        public PostalCode PostalCode { get; set; }

        public string FullName
        {
            get { return $"{etunimi} {sukunimi}"; }
        }

        public string CustomerId
        {
            get { return $"{asiakas_id}"; }
        }

        public string Address
        {
            get { return $"{lahiosoite}"; }
        }

        public string PostalCodeData
        {
            get { return $"{postinro}"; }
        }

        public string PhoneNumber
        {
            get { return $"{puhelinnro}"; }
        }

        public string Email
        {
            get { return $"{email}"; }
        }
    }
}
