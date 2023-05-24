using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lylo.University.Immigration.Models
{
    
    [Table("Immigrations")]
    public class ImmigrationData
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Country { get; set; }

        public bool Children { get; set; }

    }
}
