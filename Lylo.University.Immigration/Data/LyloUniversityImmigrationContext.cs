using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Lylo.University.Immigration.Models;

namespace Lylo.University.Immigration.Data
{
    public class LyloUniversityImmigrationContext : DbContext
    {
        public LyloUniversityImmigrationContext (DbContextOptions<LyloUniversityImmigrationContext> options)
            : base(options)
        {
        }

        public DbSet<Lylo.University.Immigration.Models.ImmigrationData> ImmigrationDatas { get; set; } = default!;
    }
}
