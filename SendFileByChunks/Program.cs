using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SendFileByChunks
{
    class SendFileByChunks
    {
        private const int MAX_BYTES = 1000000;

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
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer ");

            Console.WriteLine($"Reading file");
            var file = await File.ReadAllBytesAsync(@"Example.xlsx");
            string fileName = Path.GetFileName(@"Example.xlsx");
            FileByChunksRequest request = new FileByChunksRequest()
            {
                FileName = fileName,
                FileSize = file.Length.ToString(),
            };

            Console.WriteLine($"Configurating settings");
            int totalChunks = (file.Length / MAX_BYTES) + 1;
            int offSet = 0;
            string url = "https://localhost:5001/api/v1/File/Chunks";
            HttpResponseMessage response;
            string responseContent;

            Console.WriteLine($"Starting to upload by chunks");
            for (int i = 0; i < totalChunks; i++)
            {
                
                var isLastChunk = i == totalChunks - 1;
                
                if (isLastChunk)
                {
                    int lastBytes = file.Length - offSet;
                    byte[] bytes = new byte[lastBytes];           
                    Array.Copy(file, offSet, bytes, 0, lastBytes);
                    request.Bytes = bytes;
                    request.IsLastChunk = isLastChunk;
                    request.Offset = offSet.ToString();
                    response = await httpClient.PostAsJsonAsync(url, request);
                    responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Upload of file unsuccesfull, Status: {response.StatusCode}, " +
                            $"Response: {responseContent}");
                    }
                    Console.WriteLine($"Uploaded file succefully");
                }
                else
                {
                    byte[] bytes = new byte[MAX_BYTES];
                    Array.Copy(file, offSet, bytes, 0, MAX_BYTES);
                    request.Bytes = bytes;
                    request.IsLastChunk = isLastChunk;
                    request.Offset = offSet.ToString();
                    response = await httpClient.PostAsJsonAsync(url, request);
                    responseContent = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Upload of file unsuccesfull, Status: {response.StatusCode}, " +
                            $"Response: {responseContent}");
                    }

                    offSet += MAX_BYTES;
                    Console.WriteLine($"Uploaded {offSet} out of {file.Length}");
                }             
            }       
        }
    }
}
