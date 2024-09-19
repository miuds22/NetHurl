
using System.Text.Json;
using Microsoft.Win32;


public class Enviroment 
{
        public string nroInventory { get;  set; }
        public string Place { get;  set; }
        public string description { get;  set; }
        public string screen_NroInventory { get;  set; }
        public string installedApps { get;  set; }
        public string serialKey { get;  set; }
        public string OS { get;  set; } = Environment.OSVersion.ToString();
        public long   TotalSize  { get;  set; }
        public long TotalFreeSpace { get;  set; }
        public int availableStorages { get;  set; }

        public Enviroment()
        {        
            getRegistry();
            getStorage();
            getInstalledApps();
            getCDWindows();
        }

    public void getStorage()
    {
        long sumStorage = 0;
        long sumFree = 0;            
        int cdiscos = 0;
        DriveInfo[] drives = DriveInfo.GetDrives();
        
        foreach (DriveInfo drive in drives)
        {
            try
            {
                if (drive.IsReady)
                {
                    sumStorage = sumStorage + drive.TotalSize;
                    sumFree = sumFree + drive.TotalFreeSpace;
                    cdiscos++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading drive information: {ex.Message}");
            }
        }
        TotalSize = sumStorage;
        TotalFreeSpace = sumFree;
        availableStorages = cdiscos;
    }
        

    public  void getInstalledApps()
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
            installedApps = JsonSerializer.Serialize(aps);
        }
    }

    public void getCDWindows()
    {
        if (Environment.OSVersion.ToString().Contains("Windows"))   
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            string keyPath = @"Software\Microsoft\Windows NT\CurrentVersion";
            byte[] rpk = (byte[])key.OpenSubKey(keyPath).GetValue("DigitalProductId");
            serialKey = DecodeProductKey(rpk);
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


    void getRegistry()
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Records_NetExplorer");
        //si el sistema operativo es windows leemos del registry
        if (key != null)
        {
            nroInventory = key.GetValue("nroInventory").ToString();
            Place = key.GetValue("Place").ToString();
            description = key.GetValue("description").ToString();
            screen_NroInventory = key.GetValue("screen_nroInventory").ToString();
        }
        else
        {
                nroInventory = Ask("Ingrese el numero de inventario.");            
                Place = Ask("Ingrese el ID de Place, propiciado por  el administrador del sistema.");
                description = Ask("Ingrese la descripcion de la ubicacion del equipo.( piso, oficina, etc)");
                screen_NroInventory = Ask("Ingrese el numero de inventario del monitor.");

                if (Environment.OSVersion.ToString().Contains("Windows"))
                {
                    key = Registry.CurrentUser.CreateSubKey(@"Software\Records_NetExplorer");

                    key.SetValue("nroInventory", nroInventory);  
                    key.SetValue("Place", Place);  
                    key.SetValue("description", description);  
                    key.SetValue("screen_nroInventory", screen_NroInventory);  
                }
        }
    }

    private string? Ask(string mensaje)
    {
        Console.WriteLine(mensaje);
        string? valor = Console.ReadLine();
        if (string.IsNullOrEmpty(valor))
            {
                Console.WriteLine("El valor no puede ser nulo.");
                return  Ask(mensaje);
            }
        return valor;
    }
}