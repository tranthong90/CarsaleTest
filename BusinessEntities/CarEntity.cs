using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class CarEntity
    {
        public int CarId { get; set; }

        public int Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Badge { get; set; }

        public string EngineSize { get; set; }

        public string Transmission { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? Archived { get; set; }

        public int DealerId { get; set; }

      //  public virtual DealerEntity Dealer { get; set; }
    }
}
