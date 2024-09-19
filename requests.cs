using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public class requests {
    string Token = "";

    public  async Task<string> getToken()
    {
        string T_url =  "https://infopartidos3-ws.azurewebsites.net/api/Auth/marco?usuario=marco&password=582DA691BE0FE558BA0427C47EA6969D&modulo=IP3%3A66";
        var response = await get(T_url);
        if ( response != null && response.IsSuccessStatusCode)
        {
            Console.WriteLine(response);
        }
        return "";
    }

    public  async Task<HttpResponseMessage> get(string url)
    {
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            return response;
        } catch
        {
            return null;
        }
    }

     public async Task<HttpResponseMessage> Post(string body)
    {
        try {
            HttpClient client = new HttpClient();
            string Token =  await getToken(); 
            string url = "https://infopartidos3-ws.azurewebsites.net/api/NetDiscover";
            
            var content = new StringContent(body, Encoding.UTF8, "application/json");
                            client.DefaultRequestHeaders.Add("User-Agent","xxxxxxxxx");      
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
            var response = await client.PostAsync(url,  content);
            return response;
        } 
        catch
        {
            return null;
        }
    } 

}