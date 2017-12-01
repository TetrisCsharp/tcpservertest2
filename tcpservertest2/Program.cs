using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace tcpservertest2
{
    class TcpServer
    {
        private TcpListener _server;
        private Boolean _isRunning;
        private ConcurrentBag<Client> clientPool =  new ConcurrentBag<Client>();
        public object GlobalHost { get; private set; }

        private ConcurrentBag<Client> ClientPool {
            get
            {
               return clientPool;
            }
            set
            {
                this.clientPool = value;
            }
         }

        public TcpServer(int port)
        {
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _server.Start();
            

            _isRunning = true;
            
            LoopClients();
        }

        public void LoopClients()
        {
            while (_isRunning)
            {
                
                TcpClient newClient = _server.AcceptTcpClient();
 
                Console.WriteLine("Nouveau client");
                Client oui =  new Client(newClient);
                oui.SetServer(this);
                ClientPool.Add(oui);
               
                
            }
        }


        class Client
        {
            private StreamReader reader;
            private StreamWriter writer;
            private TcpClient client;
            private TcpServer server;


            public void SetServer(TcpServer server)
            {
                this.server = server;
            }


            public Client(TcpClient tcpclient)
            {
                this.client = tcpclient;
                writer = new StreamWriter(client.GetStream(), Encoding.ASCII);
                reader = new StreamReader(client.GetStream(), Encoding.ASCII);
                
                new Thread(Reception).Start();

            }


            private void Reception()
            {
                while (true) //change ça
                {
                    // reads from stream
                    String sData = reader.ReadLine();

                    // shows content on the console.
                    Console.WriteLine("Client &gt; " + sData);

                    // to write something back.

                    if (sData == "askpiece")
                    {
                        writer.WriteLine("piecegiven");
                        Console.WriteLine("piecegivent");
                        writer.Flush();
                    }
                    if (sData == "line")
                    {
                        foreach(Client cl in server.ClientPool)
                        {
                            if (cl != this)
                            {
                                cl.writer.WriteLine("+1 line");
                                Console.WriteLine("+1 line");
                                cl.writer.Flush();
                            }
                        }
                    }
                    if (sData == "finished")
                    {

                        foreach (Client cl in server.ClientPool)
                        {
                            if (cl != this)
                            {
                                cl.writer.WriteLine("gj");
                                cl.writer.Flush();
                            }
                        }
                    }

                }

            }
        }

      /*  public void HandleClient(object obj)
        {
            // retrieve client from parameter passed to thread
            TcpClient client = (TcpClient)obj;

            // sets two streams
            StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
            StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
            // you could use the NetworkStream to read and write, 
            // but there is no forcing flush, even when requested

            Boolean bClientConnected = true;
            String sData = null;

            while (bClientConnected)
            {
                // reads from stream
                sData = sReader.ReadLine();

                // shows content on the console.
                Console.WriteLine("Client &gt; " + sData);

                // to write something back.

                if(sData =="askpiece")
                {
                    sWriter.WriteLine("piecegiven");
                    sWriter.Flush();
                }
                if (sData == "line")
                {
                    sWriter.WriteLine("linedone");
                    sWriter.Flush();
                }
                if (sData == "finished")
                {
                    sWriter.WriteLine("gj");
                    sWriter.Flush();
                    
                }
               

            }
        }
        */
            
       
    static void Main(string[] args)
        {
            Console.WriteLine("Multi-Threaded TCP Server Demo");
            TcpServer server = new TcpServer(5555);
        }
    }
}
