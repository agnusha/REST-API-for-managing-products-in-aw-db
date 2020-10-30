using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Models
{
    public class AwDbContext : DbContext
    {
        public AwDbContext(): base()
        {
        }

        public virtual DbSet<Product> Products { get; set; }
    }
}
