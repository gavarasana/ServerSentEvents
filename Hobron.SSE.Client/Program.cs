// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

HttpClient httpClient = new HttpClient();
httpClient.Timeout = TimeSpan.FromSeconds(60);

var clientId = 1;
var url = $"http://localhost:5243/api/notification/subscribe/{clientId}";
var keepRunning = true;
while (keepRunning)
{
    try
    {
        Console.WriteLine($"Establising connection with the server. Client-Id: {clientId}");
        var httpResponse = await httpClient.PostAsync(url,new StringContent(string.Empty));
        using var streamReader = new StreamReader(await httpResponse.Content.ReadAsStreamAsync());
        while (!streamReader.EndOfStream)
        {
            var message = await streamReader.ReadLineAsync();
            Console.WriteLine(message);
        }
    } catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        //keepRunning = false;
    }
}
