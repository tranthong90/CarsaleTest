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
    public class CarServices : ICarServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public CarServices()
        {
            _unitOfWork = new UnitOfWork();
        }

        /// <summary>
        /// Fetches Car details by id
        /// </summary>
        /// <param name="CarId"></param>
        /// <returns></returns>
        public BusinessEntities.CarEntity GetCarById(int CarId)
        {
            var car = _unitOfWork.CarRepository.GetByID(CarId);
            if (car != null)
            {
                Mapper.CreateMap<Car, CarEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var CarModel = Mapper.Map<Car, CarEntity>(car);
                return CarModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the Cars.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BusinessEntities.CarEntity> GetAllCars()
        {
            var cars = _unitOfWork.CarRepository.GetAll().ToList();
            if (cars.Any())
            {
                Mapper.CreateMap<Car, CarEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var CarsModel = Mapper.Map<List<Car>, List<CarEntity>>(cars);
                return CarsModel;
            }
            return null;
        }

        public IEnumerable<BusinessEntities.CarEntity> GetAllCarsByConditions(int? year, string make, string model, string badge, string engineSize, string tranmission, DateTime? created, int? dealerId)
        {
            var cars = _unitOfWork.CarRepository.GetAllQueryable();
            if (year != null)
                cars = cars.Where(x => x.Year == year);
            if (!string.IsNullOrEmpty(make))
                cars = cars.Where(x => x.Make.Equals(make, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrEmpty(model))
                cars = cars.Where(x => x.Model.Equals(model, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrEmpty(badge))
                cars = cars.Where(x => x.Badge.Equals(badge, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrEmpty(engineSize))
                cars = cars.Where(x => x.EngineSize.Equals(engineSize, StringComparison.CurrentCultureIgnoreCase));
            if (!string.IsNullOrEmpty(tranmission))
                cars = cars.Where(x => x.Transmission.Equals(tranmission, StringComparison.CurrentCultureIgnoreCase));
            if (created != null)
                cars = cars.Where(x => x.DateCreated >= created);
            if (dealerId != null)
                cars = cars.Where(x => x.DealerId == dealerId);
            if (cars.Any())
            {
                Mapper.CreateMap<Car, CarEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var CarsModel = Mapper.Map<List<Car>, List<CarEntity>>(cars.ToList());
                return CarsModel;
            }
            return null;
        }


        public IEnumerable<BusinessEntities.CarEntity> GetAllCarsByDealerName(string dealerUsername)
        {
            var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(dealerUsername, StringComparison.CurrentCultureIgnoreCase));
            var cars = _unitOfWork.CarRepository.GetMany(x => x.DealerId == dealer.DealerId);
            if (cars != null && cars.Any())
            {
                Mapper.CreateMap<Car, CarEntity>();
                Mapper.CreateMap<Dealer, DealerEntity>();
                var CarsModel = Mapper.Map<List<Car>, List<CarEntity>>(cars.ToList());
                return CarsModel;
            }
            return null;
        }

        /// <summary>
        /// Creates a Car
        /// </summary>
        /// <param name="CarEntity"></param>
        /// <returns></returns>
        public int CreateCar(BusinessEntities.CarEntity CarEntity)
        {
            using (var scope = new TransactionScope())
            {
                var car = new Car
                {
                    Year = CarEntity.Year,
                    Make = CarEntity.Make,
                    Model = CarEntity.Model,
                    Badge = CarEntity.Badge,
                    EngineSize = CarEntity.EngineSize,
                    Transmission = CarEntity.Transmission,
                    DateCreated = DateTime.Now,
                    DealerId = CarEntity.DealerId
                };
                _unitOfWork.CarRepository.Insert(car);
                _unitOfWork.Save();
                scope.Complete();
                return car.CarId;
            }
        }

        /// <summary>
        /// Creates a Car by Dealer
        /// </summary>
        /// <param name="CarEntity"></param>
        /// <returns></returns>
        public int CreateCarByDealer(BusinessEntities.CarEntity CarEntity, string dealerUsername)
        {
            using (var scope = new TransactionScope())
            {
                var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(dealerUsername, StringComparison.CurrentCultureIgnoreCase));
                var car = new Car
                {
                    Year = CarEntity.Year,
                    Make = CarEntity.Make,
                    Model = CarEntity.Model,
                    Badge = CarEntity.Badge,
                    EngineSize = CarEntity.EngineSize,
                    Transmission = CarEntity.Transmission,
                    DateCreated = DateTime.Now,
                    DealerId = dealer.DealerId
                };
                _unitOfWork.CarRepository.Insert(car);
                _unitOfWork.Save();
                scope.Complete();
                return car.CarId;
            }
        }

        /// <summary>
        /// Updates a Car
        /// </summary>
        /// <param name="CarId"></param>
        /// <param name="CarEntity"></param>
        /// <returns></returns>
        public bool UpdateCar(int CarId, BusinessEntities.CarEntity CarEntity)
        {
            var success = false;
            if (CarEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var car = _unitOfWork.CarRepository.GetByID(CarId);
                    if (car != null)
                    {
                        car.Year = CarEntity.Year;
                        car.Make = CarEntity.Make;
                        car.Model = CarEntity.Model;
                        car.Badge = CarEntity.Badge;
                        car.EngineSize = CarEntity.EngineSize;
                        car.Transmission = CarEntity.Transmission;
                        car.DealerId = CarEntity.DealerId;
                        _unitOfWork.CarRepository.Update(car);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Updates a Car by Dealer
        /// </summary>
        /// <param name="CarId"></param>
        /// <param name="CarEntity"></param>
        /// <returns></returns>
        public bool UpdateCarByDealer(int CarId, BusinessEntities.CarEntity CarEntity, string dealerUsername)
        {
            var success = false;
            if (CarEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(dealerUsername, StringComparison.CurrentCultureIgnoreCase));
                    var car = _unitOfWork.CarRepository.GetByID(CarId);
                    if (car != null)
                    {
                        if (car.DealerId != dealer.DealerId)
                            return false;
                        car.Year = CarEntity.Year;
                        car.Make = CarEntity.Make;
                        car.Model = CarEntity.Model;
                        car.Badge = CarEntity.Badge;
                        car.EngineSize = CarEntity.EngineSize;
                        car.Transmission = CarEntity.Transmission;
                        car.DealerId = CarEntity.DealerId;
                        _unitOfWork.CarRepository.Update(car);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular Car
        /// </summary>
        /// <param name="CarId"></param>
        /// <returns></returns>
        public bool DeleteCar(int CarId)
        {
            var success = false;
            if (CarId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var car = _unitOfWork.CarRepository.GetByID(CarId);
                    if (car != null)
                    {
                        _unitOfWork.CarRepository.Delete(car);
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Deletes a particular Car by Dealer
        /// </summary>
        /// <param name="CarId"></param>
        /// <returns></returns>
        public bool DeleteCarByDealer(int CarId, string dealerUsername)
        {
            var success = false;
            if (CarId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(dealerUsername, StringComparison.CurrentCultureIgnoreCase));
                    var car = _unitOfWork.CarRepository.GetByID(CarId);
                    if (car != null)
                    {
                        if (car.DealerId != dealer.DealerId)
                            return false;
                        _unitOfWork.CarRepository.Delete(car);
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
