using System.Text;
using System.Text.Json;
using Microsoft.Win32;


class Program
{

void EnviarJson(NetDiscover info)
{
    try
    {
        string json = JsonSerializer.Serialize(info);
        string url = "http://localhost:1025/api/Entidad/NET_Equipos_IU";
        var client = new HttpClient();
        
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        client.DefaultRequestHeaders.Add("User-Agent","xxxxxxxxx");      

        var response = client.PostAsync(url,  content).Result;
        if (response.IsSuccessStatusCode)
        {
            //agregamos log de envio correcto al registry
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorerStatus");
            key.SetValue("ultimo intento Exitoso:" , DateTime.Now.ToString());
        }
        else
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorerStatus");
            key.SetValue("ultimo intento fallido:" , DateTime.Now.ToString());
        }
    } catch
    {
        RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorerStatus");
        key.SetValue("ultimo intento fallido:" , DateTime.Now.ToString());
    }
}
}




    public class NetDiscover 
    {
        public string? nro_inventario { get;set; }
        public string? Sede { get;set; }
        public string? Sector { get;set; }
        public string? nro_inventario_Monitor { get;set; }
        public string? descripcion { get;set; }
        public string? Clave_activacion {get;set;}
        public int? cant_discos {get;set;}
        public string? aps_instaladas {get;set;}
        public   long? espacio_disco  {get;set;}
        public   long? espacio_libre {get;set;}
        internal Network network { get; private set; }
        internal Enviroment enviroment { get; private set; }

    public static readonly HttpClient client = new();
        public    string usuario = Environment.UserName;


        static void Main()
        {
            NetDiscover info = new ();
            info.network = new Network();
            info.enviroment = new Enviroment();
            
        }
    }
    