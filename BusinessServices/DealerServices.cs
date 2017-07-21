using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BusinessServices
{
    public class DealerServices : IDealerServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public DealerServices()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Fetches Dealer details by id
        /// </summary>
        /// <param name="DealerId"></param>
        /// <returns></returns>
        public BusinessEntities.DealerEntity GetDealerById(int DealerId)
        {
            var dealer = _unitOfWork.DealerRepository.GetByID(DealerId);
            if (dealer != null)
            {
                Mapper.CreateMap<Dealer, DealerEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var DealerAddress = Mapper.Map<Dealer, DealerEntity>(dealer);
                return DealerAddress;
            }
            return null;
        }

        /// <summary>
        /// Fetches Dealer details by Username
        /// </summary>
        /// <param name="dealerUsername"></param>
        /// <returns></returns>
        public BusinessEntities.DealerEntity GetDealerByUsername(string dealerUsername)
        {
            var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(dealerUsername));
            if (dealer != null)
            {
                Mapper.CreateMap<Dealer, DealerEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var DealerAddress = Mapper.Map<Dealer, DealerEntity>(dealer);
                return DealerAddress;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the Dealers.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.DealerEntity> GetAllDealers()
        {
            var dealers = _unitOfWork.DealerRepository.GetAll().ToList();
            if (dealers.Any())
            {
                Mapper.CreateMap<Dealer, DealerEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var DealersAddress = Mapper.Map<List<Dealer>, List<DealerEntity>>(dealers);
                return DealersAddress;
            }
            return null;
        }

        /// <summary>
        /// Creates a Dealer
        /// </summary>
        /// <param name="DealerEntity"></param>
        /// <returns></returns>
        public int CreateDealer(BusinessEntities.DealerEntity DealerEntity)
        {
            using (var scope = new TransactionScope())
            {
                var dealer = new Dealer
                {
                    Name = DealerEntity.Name,
                    Email = DealerEntity.Email,
                    Address = DealerEntity.Address,
                };
                _unitOfWork.DealerRepository.Insert(dealer);
                _unitOfWork.Save();
                scope.Complete();
                return dealer.DealerId;
            }
        }

        /// <summary>
        /// Updates a Dealer
        /// </summary>
        /// <param name="DealerId"></param>
        /// <param name="DealerEntity"></param>
        /// <returns></returns>
        public bool UpdateDealer(int DealerId, BusinessEntities.DealerEntity DealerEntity)
        {
            var success = false;
            if (DealerEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var dealer = _unitOfWork.DealerRepository.GetByID(DealerId);
                    if (dealer != null)
                    {
                        dealer.Name = DealerEntity.Name;
                        dealer.Email = DealerEntity.Email;
                        dealer.Address = DealerEntity.Address;
                        _unitOfWork.DealerRepository.Update(dealer);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular Dealer
        /// </summary>
        /// <param name="DealerId"></param>
        /// <returns></returns>
        public bool DeleteDealer(int DealerId)
        {
            var success = false;
            if (DealerId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var dealer = _unitOfWork.DealerRepository.GetByID(DealerId);
                    if (dealer != null)
                    {
                        _unitOfWork.DealerRepository.Delete(dealer);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

      
    }
}
