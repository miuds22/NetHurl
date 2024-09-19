using System.Net.NetworkInformation;
using System.Net;

public class  Network {
    public string publicIP { get;  set; }
    public string latency  { get;  set; }
     public string MACgateway  { get;  set; }
    public string MAChost { get;  set; }

    //init class
    public Network(){
        publicIP = getPublicIP();
        latency = getLatency("8.8.8.8");
        MACgateway = GetMACAddressGW();
        MAChost = GetMACAddress();
    }
    string getPublicIP(){
        string ip = string.Empty;
        try
            {ip = new WebClient().DownloadString("http://icanhazip.com");}
        catch (Exception ex)
            {Console.WriteLine(ex.Message);}
        return ip;
    }

    private string getLatency(string ip)
    {
        string lat = string.Empty;
        try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(ip);
                lat = reply.RoundtripTime.ToString();
            }
            catch (Exception ex)
            {Console.WriteLine(ex.Message);}
        return lat;
    }

    public string GetMacAddressFrom(string ipAddress)
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
    private  string? GetMACAddressGW()
        {
            //obtenemos la mac del equipo con salida a internet
            string mac = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                OperationalStatus status = nic.OperationalStatus;
                    foreach (GatewayIPAddressInformation g  in nic.GetIPProperties().GatewayAddresses)
                {
                    mac = GetMacAddressFrom(g.Address.ToString());
                    if (mac != "not found")
                    {
                        break;
                    }

                } 
            }
            return mac;
        }

  
}
