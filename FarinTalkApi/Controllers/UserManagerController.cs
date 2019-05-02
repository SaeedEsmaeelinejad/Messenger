using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using FarinTalkApi.Hubs;
using FarinTalkApi.Models;

namespace FarinTalkApi.Controllers
{
    [EnableCors("*","*","*")]
    [Route("userManager")]
    public class UserManagerController : ApiControllerWithHub<ServerHub>
    {
        [HttpPost]
        [Route("createUser")]
        public IHttpActionResult CreateUser([FromBody] InputParam input)
        {
            var response = new FarinResponseMessage() {IsSuccess = true, StatusCode = (int) HttpStatusCode.OK};
            try
            {
                if (DbContext.Users.Any(x => x.UserName == input.UserName))
                {
                    return Json(response);
                }

                input.UserName = Utility.GetClearPhoneNumber(input.UserName);
                DbContext.Users.Add(new User {UserName = input.UserName,IsOnline = true,LastStatus = DateTime.Now});
                var logPath = System.Configuration.ConfigurationManager.AppSettings["CreateUserLogPath"];
                if (!File.Exists(logPath))
                    File.Create(logPath);
                File.AppendAllText(logPath,Environment.NewLine+ "UserName: " + input.UserName + " CreationDate: " + DateTime.Now);
                
                return Json(response);
            }
            catch(Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.ErrorMessage =ex.Message;
                response.IsSuccess = false;

            }

            return Json(response);

        }

        [HttpPost]
        [Route("turnUserOnline")]
        public IHttpActionResult TurnUserOnline([FromBody] InputParam input)
        {
            
            var response = new FarinResponseMessage() { IsSuccess = true, StatusCode = (int)HttpStatusCode.OK };
            try
            {
                input.UserName = Utility.GetClearPhoneNumber(input.UserName);
                var existItem = DbContext.UserConnections.FirstOrDefault(x => x.ConnectionId == input.ConnectionId && x.UserName==input.UserName);
                if (existItem==null)
                {
                    DbContext.UserConnections.Add(new UserConnection(){UserName = input.UserName,ConnectionId = input.ConnectionId});
                }
               
                var user = DbContext.Users.FirstOrDefault(x => x.UserName == input.UserName);
                var userIndex=DbContext.Users.IndexOf(user);
                DbContext.Users[userIndex].IsOnline = true;
                DbContext.Users[userIndex].LastStatus = DateTime.Now;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
                response.ErrorMessage = e.Message;
            }

            return Json(response);
        }

        [HttpGet]
        [Route("isUserOnLine")]
        public IHttpActionResult IsUserOnLine([FromUri] string userName)
        {
            var response=new FarinResponseMessage(){IsSuccess = true,StatusCode = (int)HttpStatusCode.OK};
            try
            {
                userName = Utility.GetClearPhoneNumber(userName);
                var userStatus = DbContext.Users.FirstOrDefault(x => x.UserName == userName);
                if (userStatus == null)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "User Not Found.";
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    response.DataItem = userStatus;
                }
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.ErrorMessage = e.Message;
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }
           
            return Json(response);
        }
    }
}
