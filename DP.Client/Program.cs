
using IdentityModel.Client;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DP.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            Console.WriteLine("Co chcesz zrobić?");

            Console.WriteLine("1 - Wyświetl informacje o pacjentach");
            Console.WriteLine("2 - Dodaj pacjenta");
            Console.WriteLine("3 - Wyrzuć wyjątek");
            Console.WriteLine("Inny klawisz - Wyjdź z programu");



            switch (Console.ReadLine())
            {
                case "1":
                 
                    ShowPatients(client);
                    Console.WriteLine("Wciśnij dowolny klawisz aby zakończyć program");
                    Console.ReadLine();
                break;

                case "2":
                    Console.WriteLine("Wypełnij dane dla pacjenta: ");

                    Console.WriteLine("Imię:");
                    string FirstNameResponse = Console.ReadLine();

                    Console.WriteLine("Nazwisko:");
                    string LastNameResponse = Console.ReadLine();

                    Console.WriteLine("Wiek:");
                    int AgeResponse = Convert.ToInt16(Console.ReadLine());

                    Console.WriteLine("Email:");
                    string EmailResponse = Console.ReadLine();

                    string token = await GetToken();
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + token);


                    Patient p = new Patient()
                    {
                        FirstName = FirstNameResponse,
                        LastName = LastNameResponse,
                        Age = AgeResponse,
                        PositiveTestDate = DateTime.Today,
                        Email = EmailResponse
                    };

                    string userJson = System.Text.Json.JsonSerializer.Serialize(p);
                    var message = await client.PostAsync("https://localhost:5001/api/patients",
                  new StringContent(userJson, Encoding.UTF8, "application/json"));

                    Console.WriteLine(message);
                    Console.WriteLine("Pomyślnie dodano pacjenta");

                    await ShowPatients(client);
                    Console.WriteLine("Wciśnij dowolny klawisz aby zakończyć program");
                    Console.ReadLine();
                break;


                case "3":


                    string exceptionToken = await GetToken();
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse("Bearer " + exceptionToken);

                    Patient exeptionPatient = new Patient()
                    {
                        FirstName = "Pacjent",
                        LastName = "Testowy",
                        Age = 401,
                        PositiveTestDate = DateTime.Today,
                        Email = "nullowyadres"
                    };



                    string userException = System.Text.Json.JsonSerializer.Serialize(exeptionPatient);

                    var exception = await client.PutAsync("https://localhost:5001/api/patients",
                     new StringContent(userException, Encoding.UTF8, "application/json"));

                    Console.ReadLine();

                    break;

            }
        }


        static async Task ShowPatients(HttpClient client)
        {
            Console.WriteLine("Pacjenci:");
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/patients");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            JToken parsedResponse = JToken.Parse(responseBody);
            Console.WriteLine(parsedResponse);
        }

        private static async Task<string> GetToken()
        {
            using var client = new HttpClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
            {
                Address = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/",
                Policy =
                    {
                        ValidateEndpoints = false
                    }
            });

            if (disco.IsError)
                throw new InvalidOperationException(
                    $"No discovery document. Details: {disco.Error}");

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "fce95216-40e5-4a34-b041-f287e46532be",
                ClientSecret = "XWGsyzt9uM-Ia2SA8WE7~gvUJ~4og-U_1p",
                Scope = "api://fce95216-40e5-4a34-b041-f287e46532be/.default"
            };

            var token = await client.RequestClientCredentialsTokenAsync(tokenRequest);

            if (token.IsError)
                throw new InvalidOperationException($"Couldn't gather token. Details: {token.Error}");

            return token.AccessToken;
        }

    }

    public class Patient
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public DateTime PositiveTestDate { get; set; }

        public string Email { get; set; }
    }
}