using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SendFileByChunks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                }
            };
            var httpClient = new HttpClient(httpClientHandler);

            var request = new FileByChunksRequest();
            var file = await File.ReadAllBytesAsync("C://Users/Rorro/Documents/github/net_core_sendFileByChunks/SendFileByChunks/Example.xlsx");
            request.Bytes = file;
            var url = "https://localhost:5001/api/v1/File/Chunks";
            var result = await httpClient.PostAsJsonAsync(url, request);
            Console.ReadKey();
        }
    }
}
