using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using varausjarjestelma.Controller;

namespace varausjarjestelma.Database
{
    public class AppContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            MySqlConnection connection = MySqlController.GetConnection();
            optionsBuilder.UseMySQL(connection);
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
