using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace RecognizeText20
{
    static class Program
    {
        static void Main()
        {
            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "7a378d378c8f426dbccd26bc660d770d");
            string imageFilePath = "Newhire-page-001.jpg";

            // Request parameters
            queryString["mode"] = "Printed";
            var uri = "https://westus.api.cognitive.microsoft.com/vision/v2.0/recognizeText/?mode=Handwritten" ;
            //var uri = "https://westus.api.cognitive.microsoft.com/vision/v2.0/textOperations/75b89c16-64a0-42a3-a6a0-1a25c57b2399";

            HttpResponseMessage response;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{body}");
            byte[] byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }

            IEnumerable<string> r = response.Headers.GetValues("Operation-Location");
            string textOperation = r.First();
            response = await client.GetAsync(textOperation);
            string contentString = await response.Content.ReadAsStringAsync();
            JObjectTest j = new JObjectTest();

            // Display the JSON response.
            Console.WriteLine("\nResponse:\n\n{0}\n",
                j.JParse(JToken.Parse(contentString).ToString()));

            //JObjectTest j = new JObjectTest();
            //j.JParse("hello");



        }

        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
