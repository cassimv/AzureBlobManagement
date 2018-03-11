using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ViewModel;

namespace BlobBusiness
{
    public class BlobBusiness
    {

        public List<BlobViewModel> GetListOfBlobs(string containername)
        {
            var container = GetBlobContainer(containername);

            var returnList = new List<BlobViewModel>();

            // Loop over items within the container and output the length and URI.
            foreach (var item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    var blob = (CloudBlockBlob)item;               

                    returnList.Add(new BlobViewModel()
                        {
                            Name = blob.Name,
                            URI = blob.Uri.ToString()
                        }
                        );

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    var pageBlob = (CloudPageBlob)item;

                    returnList.Add(new BlobViewModel()
                        {
                            Name = pageBlob.Name,
                            URI = pageBlob.Uri.ToString()
                        }
                    );

                }
            }
            return returnList;
        }

        public void UploadPhoto(string containername, HttpPostedFileBase file)
        {
            var container = GetBlobContainer(containername);

            //Get File Name
            var fileName = Path.GetFileName(file.FileName);

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(fileName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            blockBlob.UploadFromStream(file.InputStream);
        }

        private static CloudBlobContainer GetBlobContainer(string containername)
        {
            // Retrieve storage account from connection string.
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the blob client.
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            var container = blobClient.GetContainerReference(containername);
            return container;
        }
    }


}
