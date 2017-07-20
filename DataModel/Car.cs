using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public string Badge { get; set; }

        public string EngineSize { get; set; }

        [Required]
        public string Transmission { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? Archived { get; set; }

        [Required]
        public int DealerId { get; set; }

        public virtual Dealer Dealer { get; set; }

    }
}
