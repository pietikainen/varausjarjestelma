using Microsoft.EntityFrameworkCore;
//using MySql.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace varausjarjestelma.Database
{
    public class AppContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=ohjelmisto1-sql-pietikainen-6a40.a.aivencloud.com;Database=vn;User=kayttaja;Password=AVNS_DpLuKO1gwxqxgTFe1dy;Port=11244;");
        }

        public AppContext()
        {
        }

        public DbSet<Area> alue { get; set; }
        public DbSet<Customer> asiakas { get; set; }
        public DbSet<Invoice> lasku { get; set; }
        public DbSet<Cabin> mokki { get; set; }
        public DbSet<Service> palvelu { get; set; }
        public DbSet<PostalCode> posti { get; set; }
        public DbSet<ServiceOnReservation> varauksen_palvelut { get; set; }
        public DbSet<Reservation> varaus { get; set; }

    }
}
