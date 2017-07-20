using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessEntities
{
    public class DealerEntity
    {
        public int DealerId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }

        public string Address { get; set; }

      //  public virtual ICollection<CarEntity> Cars { get; set; }
    }
}
