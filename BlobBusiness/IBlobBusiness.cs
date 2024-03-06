using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModel;

namespace BlobBusiness
{
    public interface IBlobBusiness
    {
        List<BlobViewModel> GetListOfBlobs(string containername);
        void UpdatePhotoLease(string containername, string fileName, byte[] fileData);
        Task UploadPhotoAsync(string containername, string fileName, byte[] fileData);
        Task UploadPhotoOptimisticAsync(string containername, string fileName, byte[] fileData);
    }
}