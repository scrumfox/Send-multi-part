using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using static ConsoleApp2.WhatsAppServiceClient;


namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            WhatsAppServiceClient wsClient = new WhatsAppServiceClient();

            string appPath = Environment.CurrentDirectory;
            string filePath = "ers1.pdf";
            string fullpath = Path.Combine(appPath, filePath);

            var file = File.OpenRead(fullpath);

            var wsRequest = new WhatsAppServiceClient.WhatsAppBillRequest
            {
                BranchName = "calicut",
                Message = "Test Message from code",
                Name = "Coder",
                MessageType = MessageType.auto.ToString(),
                Phone = "919567793972",
                Invoice = file

            };

           bool result =  wsClient.SendAsync(wsRequest, "application/pdf").Result;

            if (result)
            {
                Console.WriteLine("Successfully uploaded!");
            }
            Console.ReadKey();
        }
    }

    public class WhatsAppServiceClient
    {
        public async System.Threading.Tasks.Task<bool> SendAsync(WhatsAppBillRequest request, string mediaType)
        {
            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(request.Name), "name");
                content.Add(new StringContent(request.Phone), "phone");
                content.Add(new StringContent(request.BranchName), "branch_name");
                content.Add(new StringContent(request.MessageType.ToString()), "msg_type");
                content.Add(new StringContent(request.Message), "msg");


                content.Add(new StreamContent(request.Invoice)
                {
                    Headers =
                                {
                                ContentLength = request.Invoice.Length,
                                ContentType = new MediaTypeHeaderValue(mediaType)
                                }
                }, "invoice", "ers1.pdf");

                var result = await client.PostAsync("https://autosender.in/visitors/api/billing_save.php", content);


                return false;
            }

        }

        public class WhatsAppBillRequest
        {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string BranchName { get; set; }
            public FileStream Invoice { get; set; }
            public string MessageType { get; set; }
            public string Message { get; set; }
        }

        public enum MessageType
        {
            auto,
            manual
        }
    }
}
