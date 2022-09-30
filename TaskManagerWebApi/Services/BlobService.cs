using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;

namespace TaskManagerWebApi.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient) => _blobServiceClient = blobServiceClient;
        public async Task<BinaryData> GetBlobAsync(string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient("about");
            var blobClient = containerClient.GetBlobClient(fileName);
            var blobDownloadInfo = await blobClient.DownloadContentAsync();
            return blobDownloadInfo.Value.Content;
            
        }  
    }
}
