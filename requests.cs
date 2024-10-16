using Newtonsoft.Json; 
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;
using session;
using Microsoft.Win32;
using System.Threading.Tasks;




public class requests {
    string urlDomain = "http://localhost:1025";
    string Token = GetContent("http://localhost:1025/api/Auth/marco?usuario=marco&password=e3f5c333503580edd71a1be8b24b337e&modulo=IP3%3A66");
    private readonly HttpClient _httpClient;

   public requests()
   {
    _httpClient = new HttpClient();
   }

    public async void SendReport(NetDiscover info)
    {
        try
        {
            //FormatPost json = new FormatPost(info);
            string postDomain = "http://localhost:1025/api/Entidad";
            string response =  Post(postDomain, info);
            Console.WriteLine("Reporte enviado");
            
        } catch(Exception e)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorerStatus");
            key.SetValue( "ultimo intento fallido:" , DateTime.Now.ToString());
            key.SetValue( "error:" , e.Message);
        }
    } 

    static public  string GetContent(string url)
    {   
       string jsonString = string.Empty;
       string Token = string.Empty;
        try
            {
                jsonString = new WebClient().DownloadString(url);
                 var welcome = Welcome.FromJson(jsonString); 
                Token = welcome[0].Sesion;               
            }
        catch (Exception ex)
            {Console.WriteLine(ex.Message);}
        return Token;
    }

    
    public string Post(string url, object data)
    {
        _httpClient.DefaultRequestHeaders.Add("x-sesion", Token);
        _httpClient.DefaultRequestHeaders.Add("x-prefix", "NET");
        // ingresamos las variables entidad y value en el body
        var content = new StringContent( "entidad=Equipos&value=" + JsonConvert.SerializeObject(data), Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = _httpClient.PostAsync(url, content);
        response.Result.EnsureSuccessStatusCode();
        return response.Result.Content.ReadAsStringAsync().Result;
    }
}

internal class FormatPost
{
    public NetDiscover value { get; set; }
    public string entidad { get; set; }
    public FormatPost(NetDiscover info)
    {
        this.entidad = "Equipos_IU";
        this.value = info;
    }
}

public interface IMyService
{
    Task<string> GetAsync(string url);
    Task<string> PostAsync(string url, object data);
}