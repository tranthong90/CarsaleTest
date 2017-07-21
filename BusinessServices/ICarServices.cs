using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessServices
{
    // <summary>
    /// Car Service Contract
    /// </summary>
    public interface ICarServices
    {
        CarEntity GetCarById(int carId);
        IEnumerable<CarEntity> GetAllCars();
        IEnumerable<CarEntity> GetAllCarsByConditions(int? year, string make, string model, string badge, string engineSize, string tranmission, DateTime? created, int? dealerId);
        IEnumerable<CarEntity> GetAllCarsByDealerName(string dealerUsername);
        int CreateCar(CarEntity carEntity);
        int CreateCarByDealer(CarEntity carEntity,string dealerUsername);
        bool UpdateCar(int carId, CarEntity carEntity);
        bool UpdateCarByDealer(int carId, CarEntity carEntity, string dealerUsername);
        bool DeleteCar(int carId);
        bool DeleteCarByDealer(int carId, string dealerUsername);
        bool ArchiveCar(int carId);
        bool ArchiveCarByDealer(int carId, string dealerUsername);
        Task<string> GenerateSummaryEmail(string emailAddress, string username);


    }
}
