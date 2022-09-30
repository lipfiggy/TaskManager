using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobsController:ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobsController(BlobServiceClient blobServiceClient) => _blobServiceClient = blobServiceClient;

        [HttpGet]
        public IActionResult GetBlobAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("about");
            var blobClient = containerClient.GetBlobClient(fileName);
            var blobDownloadInfo = blobClient.DownloadContent();
            byte[] blobByteArray = blobDownloadInfo.Value.Content.ToArray();
            return File(blobByteArray, "image/jpg");
            
        }  
    }
}
