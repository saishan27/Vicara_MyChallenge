using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperWebSocket;
using System;
using System.Collections.Generic;

namespace Vicara
{

    class Program
    {
        private static WebSocketServer  wsServer;
        static void Main(string[] args)
        {
            wsServer = new WebSocketServer();
            int port = 8088;
            wsServer.Setup(port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Server is running on port " + port + ". Press ENTER to exit....");
            Console.ReadKey();
        }

        private static void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("SessionClosed");
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("NewDataReceived");
        }

        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine("NewMessageReceived: " + value);
            if (value == "Hello server")
            {
                session.Send("Hello client");
            }

            //dynamic Obj = JObject.Parse(value)
            
            var Obj = JsonConvert.DeserializeObject<inpu>(value);
                Console.WriteLine(Obj.type);
                //Console.WriteLine(Obj.input.A[0,1]);
                String Type = Obj.type;
                
                string matr = "matrixMultiplication";
            if (string.Equals(Type, matr))
            {
                int ma = Obj.input.A.GetLength(0);
                int na = Obj.input.A.GetLength(1);
                int mb = Obj.input.B.GetLength(0);
                int nb = Obj.input.B.GetLength(1);
                if ((ma == mb && na == nb && ma == na))
                {
                    int[,] C = MultiplyMatrices(Obj.input.A, Obj.input.B, ma);
                    string outmat = JsonConvert.SerializeObject(C);
                    string output = "{" +
                        "\"success\":true," +
                        " \"type\":\" " + matr + "\", " +
                        "\"output\":" + outmat +
                        "}";
                    //string output = JsonConvert.SerializeObject(outp);
                    session.Send(output);
                    Console.WriteLine("Output data sent");

                }
                else if ((ma != mb || na != nb || ma != na))
                {

                    string output = "{" +
                           "\"success\":false," +
                           " \"type\":\" " + matr + "\", " +
                           "\"error\":" + "\"invalidParameters\"" +
                           "}";
                    //string output = JsonConvert.SerializeObject(outp);
                    session.Send(output);
                    Console.WriteLine("Output data sent");
                }
            }
                string prim = "generatePrimes";
            if (string.Equals(Type, prim))
            {

                if ((Obj.input.low < Obj.input.high) && (Obj.input.low <= 0))
                {
                    string outpu = GeneratePrimes(Obj.input.low, Obj.input.high);
                    string output = "{" +
                            "\"success\":true," +
                            " \"type\":\" " + prim + "\", " +
                            "\"output\":" + outpu +
                            "}";
                    session.Send(output);
                    Console.WriteLine("Output data sent");
                }
                else if (string.Equals(Type, prim))
                {
                    string output = "{" +
                          "\"success\":false," +
                          " \"type\":\" " + prim + "\", " +
                          "\"error\":" + "\"invalidParameters\"" +
                          "}";
                    session.Send(output);
                    Console.WriteLine("Output data sent");

                }
            }
            
            
        }

        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("NewSessionConnected");
        }
        public static int[,] MultiplyMatrices(int[,] a, int[,] b, int m)
        {
            int i, j;
            int[,] C = new int[m, m];
            for (i = 0; i < m; i++)
            {
                for (j = 0; j < m; j++)
                {
                    C[i, j] = 0;
                    for (int k = 0; k < 2; k++)
                    {
                        C[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return C;
        }
        public static string GeneratePrimes(int stno,int enno)
        {
            int num, i, ctr;
            List<int> list = new List<int>();
            for (num = stno; num <= enno; num++)
            {
                ctr = 0;

                for (i = 2; i <= num / 2; i++)
                {
                    if (num % i == 0)
                    {
                        ctr++;
                        break;
                    }
                }

                if (ctr == 0 && num != 1)
                    list.Add(num);
            }
            return JsonConvert.SerializeObject(list);



        }
    }
    public class inpu
    {
        public string type { get; set; }
        public input input { get; set; }
    }
    public class input
    {
        public int[,] A { get; set; }
        public int[,] B { get; set; }
        public int low { get; set; }
        public int high { get; set; }
    }

   




}