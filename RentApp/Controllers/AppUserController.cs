using Newtonsoft.Json;
using RentApp.Models.Entities;
using RentApp.Persistance;
using RentApp.Persistance.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/appUser")]
    public class AppUserController : ApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        //readonly IUnitOfWork unitOfWork = new DemoUnitOfWork(new RADBContext());

        public AppUserController(IUnitOfWork unitOfWork)
        {
        
            this._unitOfWork = unitOfWork;
        }


        /* public IEnumerable<AppUser> GetComments()
         {
             return _unitOfWork.AppUsers.GetAll();
         }*/

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("allUsers/{pageIndex}/{pageSize}/{type}/{editedFirst}/{approvedFirst}")]
        public IHttpActionResult GetAllUsers(int pageIndex, int pageSize, string type, bool editedFirst, bool approvedFirst)
        {

            RADBContext db = new RADBContext();
            var role = db.Roles.SingleOrDefault(m => m.Name == type);
            var usersByRole = db.Users.Include(x => x.AppUser).Where(m => m.Roles.All(r => r.RoleId == role.Id));

            List<AppUser> source = usersByRole.Select(x => x.AppUser).ToList();

            if (type == "AppUser") { 
                if (editedFirst)
                {
                    source.OrderBy(x => x.ProfileEdited == true);

                }
                else if (approvedFirst)
                {
                    source.OrderBy(x => x.Activated == true);
                }
        }
            else
            {
                if (editedFirst)
                {
                    source.OrderBy(x => x.Activated == true);

                }
                else if (approvedFirst)
                {
                    source.OrderBy(x => x.Activated == false);
                }
            }
           // var sourceFromBase = _unitOfWork.AppUsers.GetAll();

           // var hash = new HashSet<int>();

           // foreach (var user in usersByRole)
           // {
           //     hash.Add(user.AppUserId);
           // }
           ////List<AppUser> source = new List<AppUser>();
           // foreach (var user in sourceFromBase)
           // {
           //     if(hash.Contains(user.UserId))
           //     {
           //         source.Add(user);
           //     }
           // }
            // Get's No of Rows Count   
            int count = source.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
           // int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
           // int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = pageIndex > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = pageIndex < TotalPages ? "Yes" : "No";

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            return Ok(items);




            IEnumerable<AppUser> users = _unitOfWork.AppUsers.GetAll();
            return Ok(users);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getDocumentPicture")]
        public HttpResponseMessage GetDocumentPicture(string path)
        {
            if (path == null)
            {
                path = "default-placeholder.png";
            }

            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            if (!File.Exists(filePath))
            {
                path = "default-placeholder.png";
                filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            }
            var ext = Path.GetExtension(filePath);
            
            var contents = File.ReadAllBytes(filePath);

            MemoryStream ms = new MemoryStream(contents);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + ext);

            return response;
        }
        // GET: api/AppUser/5/
        [HttpGet]
        [Route("getUser/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppIdUser(int id)
        {
            AppUser user;
            try
            {
                user = _unitOfWork.AppUsers.Find(u => u.UserId == id).FirstOrDefault();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Cannot refresh average grade.");
            }
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        [Route("getCurrentUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppUser()
        {
            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;
               
                var user = db.Users.Where(u => u.UserName == username).Include(a => a.AppUser).First();
                var appUser = user.AppUser;
                return Ok(appUser);
            }
            catch
            {
                return BadRequest("Data could not be retrieved, try to relog.");
            }
        }

       

        //[HttpPost]
        //[Route("addAppUser")]
        //[ResponseType(typeof(AppUser))]
        //public IHttpActionResult addAppUser([FromBody]AppUser appUser)
        //{
        //    //if (!ModelState.IsValid)
        //    //{
        //    //    return BadRequest(ModelState);
        //    //}
           
        //    AppUser hj = _unitOfWork.AppUsers.Find(u => u.UserId == 1).FirstOrDefault();
        //    _unitOfWork.AppUsers.Add(appUser);

        //    try
        //    {
        //        int a=_unitOfWork.Complete();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return BadRequest("Cannot refresh average grade.");
        //    }

        //    return Ok(appUser);
        //}

        [HttpPost]
        [Route("editAppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult EditUser()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;
           
            

            AppUser appUser = _unitOfWork.AppUsers.Get(Int32.Parse(httpRequest["UserId"]));
            appUser.FullName = httpRequest["FullName"];
            appUser.BirthDate = DateTime.Parse(httpRequest["BirthDate"]);
            appUser.Email = httpRequest["Email"];
            appUser.ProfileEdited = true;

            if (appUser.DocumentPicture == null || appUser.DocumentPicture == "")
            {
                var postedFile = httpRequest.Files["Image"];
                imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);
                postedFile.SaveAs(filePath);
                appUser.DocumentPicture = imageName;
            }


            _unitOfWork.AppUsers.Update(appUser);
            _unitOfWork.Complete();

            return Ok("User edited");
        }

        [HttpGet]
        [Route("deleteUser/{userId}")]
        public IHttpActionResult DeleteUser(int userId)
        {
            AppUser appUser = _unitOfWork.AppUsers.Get(userId);
            if (appUser == null)
            {
                return NotFound();
            }

            _unitOfWork.AppUsers.Remove(appUser);
            _unitOfWork.Complete();

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("activateUser/{userId}/{activated}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult ActivateUser(int userId,bool activated)
        {

            AppUser appUser = _unitOfWork.AppUsers.Get(userId);

             //MailMessage mail = new MailMessage("foksfak@gmail.com", appUser.Email);
             MailMessage mail = new MailMessage("easyrent.e3@gmail.com", "easyrent.e3@gmail.com");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("easyrent.e3@gmail.com", "pusgse394");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            mail.From = new MailAddress("easyrent.e3@gmail.com");
            // mail.To.Add( appUser.Email);
            mail.To.Add("bojan.wow95@gmail.com");
            


            if (activated)
            {
                appUser.Activated = true;
                
                mail.Subject = "Profile approved";
                mail.Body = "Your profile was approved by our administrators!";
              
            }
            else
            {
                appUser.Activated = false;

                mail.Subject = "Profile wasn't approved";
                mail.Body = "Unfortunately your profile wasn't approved. Try changing your personal information.";
               
            }
            appUser.ProfileEdited = false;



            _unitOfWork.AppUsers.Update(appUser);
            _unitOfWork.Complete();

            try
            {
                client.Send(mail);
            }
            catch
            {

            }

            return Ok(appUser);
        }
        [HttpGet]
        [Route("canUserOrder")]
        public IHttpActionResult CanUserOrder()
        {
            RADBContext db = new RADBContext();
            try
            {
                var username = User.Identity.Name;

                var user = db.Users.Where(u => u.UserName == username).Include(a => a.AppUser).First();
                var appUser = user.AppUser;
                if (appUser.Activated)
                {
                    return Ok(true);
                }
                else
                {
                    return BadRequest("Your profile is not activated");
                }
            }
            catch
            {
                return BadRequest("User not found, try to relog");
            }
        }
        
    }
}
