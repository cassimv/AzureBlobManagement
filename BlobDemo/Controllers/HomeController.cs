using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [ActionName("Upload")]
        public ActionResult Upload()
        {
            ViewBag.Message = "Upload page.";

            return View();
        }
        [HttpPost]
        [ActionName("Upload")]
        public async Task<ActionResult> UploadAsync(PhotoUpload photo)
        {
            if (ModelState.IsValid)
            {
                if (photo.FileUpload != null && photo.FileUpload.ContentLength > 0)
                {
                    var blobbusiness = new BlobBusiness.BlobBusiness();
                    await blobbusiness.UploadPhotoAsync("images", photo.FileUpload);
                }
            }

            return RedirectToAction("Index");            

        }
    }
}