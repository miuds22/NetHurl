
using System.Text.Json;
using Microsoft.Win32;


class  Enviroment 
{
        public string nro_inventario, Sede, descripcion, nro_inventario_Monitor, aps_instaladas, Clave_activacion;
        public string SistemaOperativo = Environment.OSVersion.ToString();
        public long   espacio_disco ,espacio_libre;
        public int cant_discos;

        public Enviroment()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NetExplorer");
            if (key != null)
                {readRegistry();}
            else
                {setRegistry();}
        }

        public void setRegistry()
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
        espacio_disco = sumDiscos;
        espacio_disco = sumDiscos;
        espacio_libre = sumLibres;
        cant_discos = cdiscos;
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

    public void obtenerCDWindows()
    {
        if (Environment.OSVersion.ToString().Contains("Windows"))   
        {
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
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


    NetDiscover readRegistry()
    {
        //si el sistema operativo es windows leemos del registry
        if (Environment.OSVersion.ToString().Contains("Windows"))
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\NetExplorer");
            NetDiscover info = new NetDiscover();
            info.nro_inventario = key.GetValue("nro_inventario").ToString();
            info.Sede = key.GetValue("Sede").ToString();
            info.descripcion = key.GetValue("descripcion").ToString();
            info.nro_inventario_Monitor = key.GetValue("nro_inventario_Monitor").ToString();
            return info;
        }
        else
        {
            return new NetDiscover();
        }

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
}