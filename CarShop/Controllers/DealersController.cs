using BusinessEntities;
using BusinessServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    public class DealersController : ApiController
    {
        private readonly IDealerServices _dealerServices;
        private AuthContext _ctx;
        private UserManager<IdentityUser> _userManager;

        #region Public Constructor

        /// <summary>
        /// Public constructor to initialize dealer service instance
        /// </summary>
        public DealersController()
        {
            _dealerServices = new DealerServices();
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        #endregion

        public bool IsAdmin()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var isAdmin = claims.FirstOrDefault(x => x.Type.Equals("IsAdmin"));
            if (isAdmin != null && isAdmin.Value.Equals("True", StringComparison.CurrentCultureIgnoreCase))
                return true;
            else
                return false;

        }

        public HttpResponseMessage Get()
        {
            if (IsAdmin())
            {
                var Dealers = _dealerServices.GetAllDealers();
                var productEntities = Dealers as List<DealerEntity> ?? Dealers.ToList();
                if (productEntities.Any())
                    return Request.CreateResponse(HttpStatusCode.OK, productEntities);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Dealers not found");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin can perform this action");
        }

        public HttpResponseMessage Get(int id)
        {
            if (IsAdmin())
            {
                var car = _dealerServices.GetDealerById(id);
                if (car != null)
                    return Request.CreateResponse(HttpStatusCode.OK, car);
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No dealer found for this id");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin can perform this action");
        }

        [HttpGet]
        public HttpResponseMessage MyInfo()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var username = claims.FirstOrDefault(x => x.Type.Equals("sub"));
            var dealer = _dealerServices.GetDealerByUsername(username.Value);
            if (dealer != null)
                return Request.CreateResponse(HttpStatusCode.OK, dealer);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No dealer found");
        }

        [HttpGet]
        public async System.Threading.Tasks.Task<HttpResponseMessage> SendMySummary(string emailAddress)
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var username = claims.FirstOrDefault(x => x.Type.Equals("sub"));

            var sendEmail = await _dealerServices.GenerateSummaryEmail(emailAddress, username.Value);
            if (sendEmail.Equals("Ok"))
                return Request.CreateResponse(HttpStatusCode.OK);
            else
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Cannot send email. Error: " + sendEmail);
        }

        [HttpPut]
        public HttpResponseMessage UpdateMyInfo([FromBody]DealerEntity DealerEntity)
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var username = claims.FirstOrDefault(x => x.Type.Equals("sub"));
            var dealer = _dealerServices.GetDealerByUsername(username.Value);
            if (dealer != null)
                return Request.CreateResponse(HttpStatusCode.OK, _dealerServices.UpdateDealer(dealer.DealerId, DealerEntity));
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No dealer found");
        }

        public HttpResponseMessage Post([FromBody] DealerEntity DealerEntity)
        {
            if (IsAdmin())
            {
                return Request.CreateResponse(HttpStatusCode.OK, _dealerServices.CreateDealer(DealerEntity));
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin can perform this action");
        }

        public HttpResponseMessage Put(int id, [FromBody]DealerEntity DealerEntity)
        {
            if (IsAdmin())
            {
                if (id > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, _dealerServices.UpdateDealer(id, DealerEntity));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Id must be greater than 0");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin can perform this action");
            }
        }

        public HttpResponseMessage Delete(int id)
        {
            if (IsAdmin())
            {
                if (id > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, _dealerServices.DeleteDealer(id));
                else
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Id must be greater than 0");
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Only Admin can perform this action");
        }
    }
}