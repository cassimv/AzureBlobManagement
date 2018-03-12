using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Build.Framework;

namespace ViewModel
{
    public class PhotoUpload
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Please select file.")]
        public HttpPostedFileBase FileUpload { get; set; }
    }
}
