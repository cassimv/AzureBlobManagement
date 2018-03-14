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

            //check if there are any items in the container
            if (container.ListBlobs(null, false).Count() > 0)
            {
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

        //Upload photo using optimistic concurrency
        public void UploadPhotoOptimistic(string containername, HttpPostedFileBase file)
        {
            var container = GetBlobContainer(containername);

            //Get File Name
            var fileName = Path.GetFileName(file.FileName);

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(fileName);

            //Use the etag with optimistic concurrency
            AccessCondition accessCondition = new AccessCondition
            {
                IfMatchETag = blockBlob.Properties.ETag
            };

            try
            {
                // Create or overwrite the "myblob" blob with contents from a local file.
                blockBlob.UploadFromStream(file.InputStream, accessCondition);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.PreconditionFailed)
                {
                    throw new Exception("Precondition failure. Blob's orignal etag no longer matches");
                }
                else throw;
            }

        }

        //Update photo using pesimistic concurrency (lease)
        //You can only use a lease if you already have a blob created. Aquiring a lease on a new blob throws a HTTP 404 error
        //Lease works on the following: Put Blob, Set Blob Metadata, Set Blob Properties, Delete Blob, Put Block, Put Block List, Put Page, Append Block, Copy Blob
        public void UpdatePhotoLease(string containername, HttpPostedFileBase file)
        {
            var container = GetBlobContainer(containername);

            //Get File Name
            var fileName = Path.GetFileName(file.FileName);

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference(fileName);

            //Set the least period to 15 seconds
            AccessCondition accessCondition = new AccessCondition
            {
                LeaseId = blockBlob.AcquireLease(TimeSpan.FromSeconds(15), null)
            };

            try
            {
                // Overwrite the "myblob" blob with contents from a local file.
                blockBlob.UploadFromStream(file.InputStream, accessCondition);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)System.Net.HttpStatusCode.PreconditionFailed)
                    throw new Exception("Precondition failure. Error with the lease.");
                else
                    throw;
            }

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

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };

            //create container if it does not exist
            container.CreateIfNotExists();

            //set permission
            container.SetPermissions(permissions);

            return container;
        }
    }


}
