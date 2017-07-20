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

        /// <summary>
        /// Send summary email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<string> GenerateSummaryEmail(string emailAddress, string username)
        {
            string body = "";
            var dealer = _unitOfWork.DealerRepository.Get(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
            var cars = _unitOfWork.CarRepository.GetMany(x => x.DealerId == dealer.DealerId);
            if (cars != null && cars.Any())
            {

                var carCreatedThisMonth = cars.Where(x => x.DateCreated >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).ToList();
                if (carCreatedThisMonth == null)
                {
                    body += "<p>You have not posted any car this month</p>";
                }
                else
                {
                    body += @"<p><h2>Car created today</h2><table>
                            <tr>
                                <th>Year</th>
                                <th>Make</th>
                                <th>Model</th>
	                            <th>Badge</th>
	                            <th>EngineSize</th>
	                            <th>Transmission</th>
	                            <th>DateCreated</th>
	                            <th>Archived</th>
                              </tr>
                              ";
                    var carCreatedToday = carCreatedThisMonth.Where(x => x.DateCreated == DateTime.Now);
                    if (carCreatedToday != null && carCreatedToday.Any())
                    {

                        foreach (var car in carCreatedToday)
                        {
                            body += "<tr>";
                            body += "<td>" + car.Year + "</td>";
                            body += "<td>" + car.Make + "</td>";
                            body += "<td>" + car.Model + "</td>";
                            body += "<td>" + car.Badge + "</td>";
                            body += "<td>" + car.EngineSize + "</td>";
                            body += "<td>" + car.Transmission + "</td>";
                            body += "<td>" + car.DateCreated.ToString("DD/mm/yyyy") + "</td>";
                            body += "<td>" + (car.Archived == null ? "" : ((DateTime)car.Archived).ToString("DD/mm/yyyy")) + "</td>";
                            body += "</tr>";
                        }
                    }
                    body += "</table>";

                    body += @"<p><h2>Car created this week</h2><table>
                            <tr>
                                <th>Year</th>
                                <th>Make</th>
                                <th>Model</th>
	                            <th>Badge</th>
	                            <th>EngineSize</th>
	                            <th>Transmission</th>
	                            <th>DateCreated</th>
	                            <th>Archived</th>
                              </tr>";

                    var carCreatedThisWeek = carCreatedThisMonth.Where(x => x.DateCreated >= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek));
                    if (carCreatedThisWeek != null && carCreatedThisWeek.Any())
                    {
                        foreach (var car in carCreatedThisWeek)
                        {
                            body += "<tr>";
                            body += "<td>" + car.Year + "</td>";
                            body += "<td>" + car.Make + "</td>";
                            body += "<td>" + car.Model + "</td>";
                            body += "<td>" + car.Badge + "</td>";
                            body += "<td>" + car.EngineSize + "</td>";
                            body += "<td>" + car.Transmission + "</td>";
                            body += "<td>" + car.DateCreated.ToString("DD/mm/yyyy") + "</td>";
                            body += "<td>" + (car.Archived == null ? "" : ((DateTime)car.Archived).ToString("DD/mm/yyyy")) + "</td>";
                            body += "</tr>";
                        }
                    }
                    body += "</table>";

                    body += @"<p><h2>Car created this month</h2><table>
                            <tr>
                                <th>Year</th>
                                <th>Make</th>
                                <th>Model</th>
	                            <th>Badge</th>
	                            <th>EngineSize</th>
	                            <th>Transmission</th>
	                            <th>DateCreated</th>
	                            <th>Archived</th>
                              </tr>";


                    if (carCreatedThisMonth != null && carCreatedThisMonth.Any())
                    {
                        foreach (var car in carCreatedThisMonth)
                        {
                            body += "<tr>";
                            body += "<td>" + car.Year + "</td>";
                            body += "<td>" + car.Make + "</td>";
                            body += "<td>" + car.Model + "</td>";
                            body += "<td>" + car.Badge + "</td>";
                            body += "<td>" + car.EngineSize + "</td>";
                            body += "<td>" + car.Transmission + "</td>";
                            body += "<td>" + car.DateCreated.ToString("DD/mm/yyyy") + "</td>";
                            body += "<td>" + (car.Archived == null ? "" : ((DateTime)car.Archived).ToString("DD/mm/yyyy")) + "</td>";
                            body += "</tr>";
                        }
                    }
                    body += "</table>";
                }
            }
            if (string.IsNullOrEmpty(emailAddress))
                emailAddress = dealer.Email;
            string result = await Utility.SendEmail(emailAddress, "Summary Report", body);
            return result;


        }
    }
}
