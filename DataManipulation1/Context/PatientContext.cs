using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataManipulation1.Models;

namespace DataManipulation1.Context
{
    public class PatientContext : DbContext
    {
        public DbSet<Patient> Patient { get; set; }
    }
}
