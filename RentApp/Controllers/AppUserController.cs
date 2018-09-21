using Newtonsoft.Json;
using RentApp.Crypting;
using RentApp.ETag;
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
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Script.Serialization;

namespace RentApp.Controllers
{
    [RoutePrefix("api/appUser")]
    public class AppUserController : ApiController
    {

        JsonSerializerSettings setting = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        private readonly IUnitOfWork _unitOfWork;


        public AppUserController(IUnitOfWork unitOfWork)
        {

            this._unitOfWork = unitOfWork;
        }


       

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("allUsers/{pageIndex}/{pageSize}/{type}/{editedFirst}/{approvedFirst}")]
        public IHttpActionResult GetAllUsers(int pageIndex, int pageSize, string type, bool editedFirst, bool approvedFirst)
        {

            RADBContext db = new RADBContext();
            var role = db.Roles.SingleOrDefault(m => m.Name == type);
            var usersByRole = db.Users.Include(x => x.AppUser).Where(m => m.Roles.All(r => r.RoleId == role.Id));

            List<AppUser> source = usersByRole.Select(x => x.AppUser).ToList();

            if (source == null || source.Count() < 1)
            {
                if (type == "AppUser")
                {
                    return BadRequest("There are no Users");
                }
                else
                {
                    return BadRequest("There are no Managers");
                }
            }

            
            if (editedFirst)
            {
                source = source.OrderByDescending(x => x.ProfileEdited).ToList();

            }
            else if (approvedFirst)
            {
                source = source.OrderByDescending(x => x.Activated).ToList();
            }
            

            // Get's No of Rows Count   
            int count = source.Count();


            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Returns List of Customer after applying Paging   
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();


            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize,
                currentPage = pageIndex,
                totalPages = TotalPages
            };

            // Setting Header  
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", "Paging-Headers");
            HttpContext.Current.Response.Headers.Add("Paging-Headers", JsonConvert.SerializeObject(paginationMetadata));

            return Ok(items);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getDocumentPicture")]
        public HttpResponseMessage GetDocumentPicture(string path)
        {
            if (path == null)
            {
                path = "default-placeholderCrypt.png";
            }

            var filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            if (!File.Exists(filePath))
            {
                path = "default-placeholderCrypt.png";
                filePath = HttpContext.Current.Server.MapPath("~/Images/" + path);
            }
            var ext = Path.GetExtension(filePath);


            byte[] contents = null;

            string eSecretKey = SecretKey.LoadKey(HttpRuntime.AppDomainAppPath + "Images\\SecretKey.txt");
            AES_Symm_Algorithm.DecryptFile(filePath, out contents, eSecretKey);


            MemoryStream ms = new MemoryStream(contents);

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(ms);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/" + ext);

            return response;
        }
        

        [HttpGet]
        [Route("getCurrentUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppUser()
        {
            AppUser appUser;
            try
            {
                var username = User.Identity.Name;

                var user = _unitOfWork.AppUsers.Find(u => u.Email == username).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("Data could not be retrieved, try to relog.");
                }
                appUser = user;

                var jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
                var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));

                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

                if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) != null && HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') == eTag)
                    return new StatusCodeResult(HttpStatusCode.NotModified, new HttpRequestMessage());



                return Ok(appUser);
            }
            catch
            {
                return BadRequest("Data could not be retrieved, try to relog.");
            }
        }

        [Authorize(Roles = "Admin, AppUser")]
        [HttpGet]
        [Route("getUserById/{userId}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetAppUserById(int userId)
        {
            try
            {

                var appUser = _unitOfWork.AppUsers.Find(x => x.UserId == userId).FirstOrDefault();


                var jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
                var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

                if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) != null && HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') == eTag)
                    return new StatusCodeResult(HttpStatusCode.NotModified, new HttpRequestMessage());



                return Ok(appUser);
            }
            catch
            {
                return BadRequest("Data could not be retrieved, try to relog.");
            }
        }


        

        [HttpPost]
        [Route("editAppUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult EditUser()
        {
            var httpRequest = HttpContext.Current.Request;

            string imageName = null;


            AppUser appUser;
            try
            {
                var username = User.Identity.Name;

                var user = _unitOfWork.AppUsers.Find(u => u.Email == username).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("Data could not be retrieved, try to relog.");
                }
                appUser = user;

            }
            catch
            {
                return BadRequest("Data could not be retrieved, try to relog.");
            }


            var jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));



            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());

            }

            appUser.FullName = httpRequest["FullName"].Trim();
            appUser.BirthDate = DateTime.Parse(httpRequest["BirthDate"]);
            appUser.Email = httpRequest["Email"].Trim();
            appUser.ProfileEdited = true;

            if (appUser.DocumentPicture == null || appUser.DocumentPicture == "")
            {
                var postedFile = httpRequest.Files["Image"];
                if (postedFile != null)
                {
                    imageName = new string(Path.GetFileNameWithoutExtension(postedFile.FileName).Take(10).ToArray()).Replace(" ", "-");
                    imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(postedFile.FileName);
                    var filePath = HttpContext.Current.Server.MapPath("~/Images/" + imageName);

                    appUser.DocumentPicture = imageName;



                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(postedFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                    }


                    string eSecretKey = SecretKey.LoadKey(HttpRuntime.AppDomainAppPath + "Images\\SecretKey.txt");
                    AES_Symm_Algorithm.EncryptFile(fileData, filePath, eSecretKey);
                }
            }

            jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
            eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));

            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            _unitOfWork.AppUsers.Update(appUser);
            _unitOfWork.Complete();

            return Ok(appUser);
        }

        [Authorize(Roles = "Admin, AppUser")]
        [HttpGet]
        [Route("deleteUser/{userId}")]
        public IHttpActionResult DeleteUser(int userId)
        {
            AppUser appUser = _unitOfWork.AppUsers.Get(userId);
            if (appUser == null)
            {
                return NotFound();
            }

            try
            {
                if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + appUser.DocumentPicture))
                {
                    File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + appUser.DocumentPicture);
                }

                List<Order> source = _unitOfWork.Orders.Find(x=>x.UserId== appUser.UserId).ToList();

                foreach (Order o in source)
                {
                    _unitOfWork.Orders.Remove(o);
                    _unitOfWork.Complete();
                }
                

                _unitOfWork.AppUsers.Remove(appUser);
                _unitOfWork.Complete();
            }
            catch(Exception e)
            {
                return BadRequest("User could not be deleted");
            }
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("activateUser/{userId}/{activated}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult ActivateUser(int userId, bool activated)
        {


            AppUser appUser = _unitOfWork.AppUsers.Get(userId);
            if (appUser == null)
            {
                return BadRequest("User does not exist");
            }

            var jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
            var eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));


            if (HttpContext.Current.Request.Headers.Get(ETagHelper.MATCH_HEADER) == null || HttpContext.Current.Request.Headers[ETagHelper.MATCH_HEADER].Trim('"') != eTag)
            {
                HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
                HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));
                return new StatusCodeResult(HttpStatusCode.PreconditionFailed, new HttpRequestMessage());

            }


            MailMessage mail = new MailMessage("easyrent.e3@gmail.com", "easyrent.e3@gmail.com");
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("easyrent.e3@gmail.com", "e3942014pusgs2018");
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            mail.From = new MailAddress("easyrent.e3@gmail.com");
            mail.To.Add(appUser.Email);



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

                if (File.Exists(HttpRuntime.AppDomainAppPath + "Images\\" + appUser.DocumentPicture))
                {
                    File.Delete(HttpRuntime.AppDomainAppPath + "Images\\" + appUser.DocumentPicture);

                    appUser.DocumentPicture = null;
                }

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

            jsonObj = JsonConvert.SerializeObject(appUser, Formatting.None, setting);
            eTag = ETagHelper.GetETag(Encoding.UTF8.GetBytes(jsonObj));
            HttpContext.Current.Response.Headers.Add("Access-Control-Expose-Headers", ETagHelper.ETAG_HEADER);
            HttpContext.Current.Response.Headers.Add(ETagHelper.ETAG_HEADER, JsonConvert.SerializeObject(eTag));

            return Ok(appUser);
        }
        [HttpGet]
        [Route("canUserOrder")]
        public IHttpActionResult CanUserOrder()
        {
            AppUser appUser;
            try
            {
                var username = User.Identity.Name;

                var user = _unitOfWork.AppUsers.Find(u => u.Email == username).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest("Data could not be retrieved, try to relog.");
                }
                appUser = user;
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