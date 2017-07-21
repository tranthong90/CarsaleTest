using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    public interface IDealerServices
    {
        DealerEntity GetDealerById(int dealerId);
        DealerEntity GetDealerByUsername(string dealerUsername);
        IEnumerable<DealerEntity> GetAllDealers();
        int CreateDealer(DealerEntity dealerEntity);
        bool UpdateDealer(int dealerId, DealerEntity dealerEntity);
        bool DeleteDealer(int dealerId);
    }
}
