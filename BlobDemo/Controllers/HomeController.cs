using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModel;

namespace BlobDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var blobbusiness = new BlobBusiness.BlobBusiness();
            return View(blobbusiness.GetListOfBlobs("images"));
        }

        public ActionResult Upload()
        {
            ViewBag.Message = "Upload page.";

            return View();
        }
        [HttpPost]
        public ActionResult Upload(PhotoUpload photo)
        {
            if (ModelState.IsValid)
            {
                if (photo.FileUpload != null && photo.FileUpload.ContentLength > 0)
                {
                    var blobbusiness = new BlobBusiness.BlobBusiness();
                    blobbusiness.UploadPhoto("images", photo.FileUpload);
                }
            }

            return RedirectToAction("Index");            

        }
    }
}