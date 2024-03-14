using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using varausjarjestelma.Database;

namespace varausjarjestelma.Database
{
    public class InvoiceCollection
    {
        private readonly AppContext _context;

        public InvoiceCollection()
        {
        }

        public InvoiceCollection(AppContext context)
        {
            _context = context;
        }

        //public async Task<List<InvoiceDetails>> GetInvoiceDetailsAsync(int id)
        //{
        //    // to be written
        //    await return;

        //}

        public class InvoiceDetails
        {
            public int InvoiceId { get; set; }
            public int CustomerId { get; set; }
            public string FullName { get; set; }
            public string Address { get; set; }
            public string PostalCode { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string CabinName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public double Total { get; set; }
            public double Tax { get; set; }
            public List<Service> Services { get; set; }
        }

        public class ServiceOnInvoice
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public double Tax { get; set; }
        }
    }
}