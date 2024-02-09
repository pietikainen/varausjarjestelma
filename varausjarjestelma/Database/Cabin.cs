﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class Cabin
    {
        [Key]
        public int mokki_id { get; set; }
        public int alue_id { get; set; }
        public int postinro { get; set; }
        public string mokkinimi { get; set; }
        public string katuosoite { get; set; }
        public double hinta { get; set; }
        public string kuvaus { get; set; }
        public int henkilomaara { get; set; }
        public string varustelu { get; set; }
    }
}