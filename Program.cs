// See https://aka.ms/new-console-template for more information
using System;
using System.Dynamic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Reflection.Metadata;
using System.Text.Json;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Microsoft.Win32;



Info info = new Info();

string path = "C:\\Users\\dp\\Desktop\\hurlinet\\te.txt";

//si existe la ruta leemos, si no vamos a obtener datos
if (File.Exists(path))
    {info = LeerRegistry(path);}
else
    {info.ObtenerDatos(path);}

info.Get_memorias();
info.getConectividad();
info.obtenerApps();
info.obtenerCDWindows();

EnviarJson(info);

void EnviarJson(Info info)
{
    string json = JsonSerializer.Serialize(info);
    string url = "http://localhost:5000/api/NET_EquiposIU";
    var client = new HttpClient();
    
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    client.DefaultRequestHeaders.Add("User-Agent","xxxxxxxxx");      

    var response = client.PostAsync(url,  content).Result;
    if (response.IsSuccessStatusCode)
    {
        Console.WriteLine("Se envio correctamente el json.");
    }
    else
    {

        Console.WriteLine(json);
    }
}

Info LeerRegistry(string path)
{
    //si el sistema operativo es windows leemos del registry
    if (Environment.OSVersion.ToString().Contains("Windows"))
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NetExplorer");
        Info info = new Info();
        info.nro_inventario = key.GetValue("nro_inventario").ToString();
        info.Sede = key.GetValue("Sede").ToString();
        info.descripcion = key.GetValue("descripcion").ToString();
        info.nro_inventario_Monitor = key.GetValue("nro_inventario_Monitor").ToString();
        return info;
    }
    else
    {
        return new Info();
    }

}


public class Info 
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
    public string? MAC {get;set;}
    public string? MACGW {get;set;}
    public string? ip_publica {get;set;}
    public string? ip_privada {get;set;}
    public string? latencia {get;set;}
    public static readonly HttpClient client = new();
    public string SistemaOperativo = Environment.OSVersion.ToString();
    public    string usuario = Environment.UserName;


    public void ObtenerDatos(string path)
    {
            nro_inventario = Pedir("Ingrese el numero de inventario.");            
            Sede = Pedir("Ingrese el ID de sede, propiciado por el administrador del sistema.");
            descripcion = Pedir("Ingrese la descripcion de la ubicacion del equipo.( piso, oficina, etc)");
            nro_inventario_Monitor = Pedir("Ingrese el numero de inventario del monitor.");

            // si el SO es windows obtenemos volcamos los datos en el registry 
            if (Environment.OSVersion.ToString().Contains("Windows"))
            {
                //insertamos nro de inventario sede descripcion en  el registry
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\NetExplorer");
                key.SetValue("nro_inventario", nro_inventario);  
                key.SetValue("Sede", Sede);  
                key.SetValue("descripcion", descripcion);  
                key.SetValue("nro_inventario_Monitor", nro_inventario_Monitor);  
            }

    }
    public void getConectividad()
    {
        MAC = GetMACAddress(); 
        MACGW = GetMACAddressGW();
        ip_publica = new WebClient().DownloadString("https://ipv4.icanhazip.com/");
        latencia = new Ping().Send("www.google.com").RoundtripTime.ToString();  
        var request = (HttpWebRequest)WebRequest.Create("http://www.icanhazip.com");
    }

    public string GetMacAddress(string ipAddress)
    {
        string macAddress = string.Empty;
        System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = "arp";
        pProcess.StartInfo.Arguments = "-a " + ipAddress;
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.Start();
        string strOutput = pProcess.StandardOutput.ReadToEnd();
        string[] substrings = strOutput.Split('-');
        string os = Environment.OSVersion.ToString();
        // windows
        if (substrings.Length >= 8)
        {
        macAddress = substrings[3].Substring(Math.Max(0, substrings[3].Length - 2)) 
                    + "-" + substrings[4] + "-" + substrings[5] + "-" + substrings[6] 
                    + "-" + substrings[7] + "-" 
                    + substrings[8].Substring(0, 2);
            return macAddress;
        }
        // unix
        else if (os.Contains("Unix"))
        {
            macAddress = strOutput.Split(' ')[3];
            return macAddress;
        }
        else
        {
            return "not found";
        }
    }
    public void Get_memorias()
    {
        long sumDiscos = 0;
        long sumLibres = 0;            
        int cdiscos = 0;
        DriveInfo[] drives = DriveInfo.GetDrives();
        
        foreach (DriveInfo drive in drives)
        {
            try
            {
                if (drive.IsReady)
                {
                    sumDiscos = sumDiscos + drive.TotalSize;
                    sumLibres = sumLibres + drive.TotalFreeSpace;
                    cdiscos++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading drive information: {ex.Message}");
            }
        }
        espacio_disco = sumDiscos;
        espacio_libre = sumLibres;
        cant_discos = cdiscos;
    }

    private string? GetMACAddress()
    {
        //obtenemos la mac del equipo con salida a internet
        string mac = string.Empty;
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            OperationalStatus status = nic.OperationalStatus;
            if (
                (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    && status == OperationalStatus.Up)
            {
                mac = nic.GetPhysicalAddress().ToString();
                mac = string.Join("-", Enumerable.Range(0, mac.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => mac.Substring(x, 2)));
                break;
            }
        }
        return mac;
    }
        private string? GetMACAddressGW()
    {
        //obtenemos la mac del equipo con salida a internet
        string mac = string.Empty;
        foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            OperationalStatus status = nic.OperationalStatus;
                foreach (GatewayIPAddressInformation g  in nic.GetIPProperties().GatewayAddresses)
            {
                mac = GetMacAddress(g.Address.ToString());
                if (mac != "not found")
                {
                    break;
                }

            } 
        }
        return mac;
    }

    private void VolcarAJson(String path)
    {
        //creamos un objeto con los valores que nos dieron y lo hacemos json
        dynamic objeto = new ExpandoObject();
        objeto.nro_inventario = nro_inventario;
        objeto.Sector = Sector;
        objeto.nro_inventario_Monitor = nro_inventario_Monitor;
        objeto.Clave_activacion = Clave_activacion;
        objeto.cant_discos = cant_discos;
        objeto.latencia = latencia;
        objeto.SistemaOperativo = SistemaOperativo;
        objeto.usuario = usuario;

        //objeto a json
        string json = JsonSerializer.Serialize(objeto);
        //guardamos el json en un archivo
        File.WriteAllText(path, json);
    }

    private string? Pedir(string mensaje)
    {
        Console.WriteLine(mensaje);
        string? valor = Console.ReadLine();
        if (string.IsNullOrEmpty(valor))
            {
                Console.WriteLine("El valor no puede ser nulo.");
                return  Pedir(mensaje);
            }
        return valor;
    }
    public  void obtenerApps()
    {
        string[] aps = new string[500];
        if (Environment.OSVersion.ToString().Contains("Windows"))
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string subKeyName in key.GetSubKeyNames())
            {
                RegistryKey subKey = key.OpenSubKey(subKeyName);
                string displayName = (string)subKey.GetValue("DisplayName");
                if (displayName != null)
                {
                    // armamos una lista de las aplicaciones
                    aps[Array.IndexOf(aps, null)] = displayName;
                }
            }
            //devolvemos un json con las 
            aps_instaladas = JsonSerializer.Serialize(aps);
        }
    }

    public void obtenerCDWindows(){
        if (Environment.OSVersion.ToString().Contains("Windows"))   
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine,
                                                RegistryView.Registry64);

            string keyPath = @"Software\Microsoft\Windows NT\CurrentVersion";
            byte[] rpk = (byte[])key.OpenSubKey(keyPath).GetValue("DigitalProductId");
            Clave_activacion = DecodeProductKey(rpk);
        }

    }

    public string DecodeProductKey(byte[] digitalProductId)
{
    // Possible alpha-numeric characters in product key.
    const string digits = "BCDFGHJKMPQRTVWXY2346789";
    // Length of decoded product key in byte-form. Each byte represents 2 chars.
    const int decodeStringLength = 15;
    // Decoded product key is of length 29
    char[] decodedChars = new char[29];

    // Extract encoded product key from bytes [52,67]
    List<byte> hexPid = new List<byte>();
    for (int i = 52; i <= 67; i++)
    {
        hexPid.Add(digitalProductId[i]);
    }

    // Decode characters
    for (int i = decodedChars.Length - 1; i >= 0; i--)
    {
        // Every sixth char is a separator.
        if ((i + 1) % 6 == 0)
        {
            decodedChars[i] = '-';
        }
        else
        {
            // Do the actual decoding.
            int digitMapIndex = 0;
            for (int j = decodeStringLength - 1; j >= 0; j--)
            {
                int byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
                hexPid[j] = (byte)(byteValue / 24);
                digitMapIndex = byteValue % 24;
                decodedChars[i] = digits[digitMapIndex];
            }
        }
    }

    return new string(decodedChars);
}
}   



