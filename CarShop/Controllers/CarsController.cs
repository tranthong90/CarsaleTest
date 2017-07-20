using BusinessEntities;
using BusinessServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace CarShop.Controllers
{
    [Authorize]
    public class CarsController : ApiController
    {
        private readonly ICarServices _carServices;

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize car service instance
        /// </summary>
        public CarsController()
        {
            _carServices = new CarServices();
        }

        #endregion

        public bool IsAdminOrDealer(out string Username, out bool IsDealer)
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            Username = claims.FirstOrDefault(x => x.Type.Equals("sub")).Value;
            var isDealer = claims.FirstOrDefault(x => x.Type.Equals("IsDealer"));
            if (isDealer != null && isDealer.Value.Equals("True", StringComparison.CurrentCultureIgnoreCase))
            {
                IsDealer = true;
                return true;
            }
            else
                IsDealer = false;

            var isAdmin = claims.FirstOrDefault(x => x.Type.Equals("IsAdmin"));
            if (isAdmin != null && isAdmin.Value.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;

        }
        [HttpGet]
        public HttpResponseMessage GetMyCars()
        {
            string Username;
            bool IsDealer;
            if (IsAdminOrDealer(out Username, out IsDealer))
            {
                var Cars = _carServices.GetAllCarsByDealerName(Username);
                var carEntities = Cars as List<CarEntity> ?? Cars.ToList();
                if (carEntities.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, carEntities);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cars not found");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin and Dealer can perform this action");
        }

        [HttpGet]
        public HttpResponseMessage GetCars(int? year, string make, string model, string badge, string engineSize, string tranmission, DateTime? created, int? dealerId)
        {
            var Cars = _carServices.GetAllCarsByConditions(year, make, model, badge, engineSize, tranmission, created, dealerId);
            var carEntities = Cars as List<CarEntity> ?? Cars.ToList();
            if (carEntities.Any())
                return Request.CreateResponse(HttpStatusCode.OK, carEntities);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Cars not found");
        }

        public HttpResponseMessage Get(int id)
        {
            var car = _carServices.GetCarById(id);
            if (car != null)
                return Request.CreateResponse(HttpStatusCode.OK, car);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No car found for this id");
        }

        public HttpResponseMessage Post([FromBody] CarEntity CarEntity)
        {
            string Username;
            bool IsDealer;
            if (IsAdminOrDealer(out Username, out IsDealer))
            {
                return Request.CreateResponse(HttpStatusCode.OK, _carServices.CreateCarByDealer(CarEntity, Username));
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin and Dealer can perform this action");
        }

        public HttpResponseMessage Put(int id, [FromBody]CarEntity CarEntity)
        {
            string Username;
            bool IsDealer;
            if (IsAdminOrDealer(out Username, out IsDealer))
            {
                if (id > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, _carServices.UpdateCarByDealer(id, CarEntity, Username));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Id must be greater than 0");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin and Dealer can perform this action");

        }

        public HttpResponseMessage Delete(int id)
        {
            string Username;
            bool IsDealer;
            if (IsAdminOrDealer(out Username, out IsDealer))
            {
                if (id > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, _carServices.DeleteCarByDealer(id, Username));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Id must be greater than 0");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin and Dealer can perform this action");

        }
    }
}