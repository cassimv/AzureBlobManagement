using System.IO;

using System.Reflection;
using System.Threading.Tasks;
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
                    var stream = new MemoryStream();
                    photo.FileUpload.InputStream.CopyTo(stream);
                    var binData = stream.ToArray();
                    await blobbusiness.UploadPhotoAsync("images", photo.FileUpload.FileName, binData);
                }
            }

            return RedirectToAction("Index");            

        }
    }
}