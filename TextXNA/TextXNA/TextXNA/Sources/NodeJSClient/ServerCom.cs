
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SocketIOClient;

namespace TestXNA.Sources.NodeJSClient
{
    class ServerCom
    {

        /////////////////////SINGLETON/////////////////////

        private static ServerCom _instance;

        public static ServerCom Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new ServerCom();
                }
                return _instance;
            }
            set { _instance = value; }
        }



        //////////////////CLASS////////////////

        private Client socket;
        public Action<SocketIOClient.Messages.IMessage> playerConnectCB;
        public Action<SocketIOClient.Messages.IMessage> majPlayerInfoCB;
        public Action<SocketIOClient.Messages.IMessage> captureZonesCB;
        public Action<SocketIOClient.Messages.IMessage> placeCommandersCB;
        public Action<SocketIOClient.Messages.IMessage> questionCB;
        public Action<SocketIOClient.Messages.IMessage> answersReceivedCB;
        public Action<SocketIOClient.Messages.IMessage> playerMoveCB;
        public Action<SocketIOClient.Messages.IMessage> battleResultCB;
        public Action<SocketIOClient.Messages.IMessage> resultCB;

        public void Execute()
        {
            Console.WriteLine("Starting TestSocketIOClient Example...");

            //socket = new Client("http://192.168.1.2:8080"); // url to the nodejs / socket.io instance
            //socket = new Client("http://127.0.0.1:8080"); 
            socket = new Client(Utils.LocalIPAddress());

            socket.Opened += SocketOpened;
            socket.Message += SocketMessage;
            socket.SocketConnectionClosed += SocketConnectionClosed;
            socket.Error += SocketError;
            

            //MESSAGE _ _ BINDINGS 

            // register for 'connect' event with io server
            socket.On("connect", (fn) =>
            {
                Console.WriteLine("\r\nConnected event...\r\n");
                //Console.WriteLine("Emit Part object");

                // emit Json Serializable object, anonymous types, or strings
                //Part newPart = new Part() { PartNumber = "K4P2G324EC", Code = "DDR2", Level = 1 };
                //socket.Emit("partInfo", newPart);

            });

            socket.On("question", (fn) =>
            {
                Console.WriteLine("received question");
                if (questionCB != null)
                {
                    questionCB(fn);
                }
            });

            socket.On("playerMoveAsked", (fn) =>
            {
                Console.WriteLine("\nreceived playerMoveAsked");
                if (playerMoveCB != null)
                {
                    playerMoveCB(fn);
                }
            });

            socket.On("battleResult", (fn) =>
            {
                Console.WriteLine("received battleResult");
                if (battleResultCB != null)
                {
                    battleResultCB(fn);
                }
            });

            socket.On("questionAnswered", (fn) =>
            {
                Console.WriteLine("\nreceived answers to question\n");
                if (answersReceivedCB != null)
                {
                    answersReceivedCB(fn);
                }
            });


            socket.On("requestIdentity", (mes) =>
            {
                Console.WriteLine("\r\nrequest ID event...\r\n");
                socket.Emit("tableIdentity", new { pseudo = "fack yah !" });

            });

            socket.On("results", (mes) =>
            {
                Console.WriteLine("\n results received \n");

                if (resultCB != null)
                {
                    resultCB(mes);
                }

            });

            socket.On("newPlayer", playerConnectCB);

            socket.On("majPlayerInfo", (data) =>
                {
                    Console.WriteLine("received MajPlayerInfo");
                    if (majPlayerInfoCB != null)
                    {
                        majPlayerInfoCB(data);
                    }
                });

            socket.On("captureTerritories", (data) =>
            {
                Console.WriteLine("received captureTerritories");
                if (captureZonesCB != null)
                {
                    captureZonesCB(data);
                }
            });

            socket.On("placeCommanders", (data) =>
            {
                Console.WriteLine("received place commanders");
                if (placeCommandersCB != null)
                {
                    placeCommandersCB(data);
                }
            });


            // register for 'update' events - message is a json 'Part' object
            socket.On("update", (data) =>
            {
                Console.WriteLine("recv [socket].[update] event");
                //Console.WriteLine("  raw message:      {0}", data.RawMessage);
                //Console.WriteLine("  string message:   {0}", data.MessageText);
                //Console.WriteLine("  json data string: {0}", data.Json.ToJsonString());
                //Console.WriteLine("  json raw:         {0}", data.Json.Args[0]);

                // cast message as Part - use type cast helper
                //Part part = data.Json.GetFirstArgAs<Part>();
                //Console.WriteLine(" Part Level:   {0}\r\n", part.Level);

            });

            // make the socket.io connection
            socket.Connect();
        }

        void SocketError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("socket client error:");
            Console.WriteLine(e.Message);
        }

        void SocketConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("WebSocketConnection was terminated!");
        }

        void SocketMessage(object sender, MessageEventArgs e)
        {
            // uncomment to show any non-registered messages
            if (string.IsNullOrEmpty(e.Message.Event))
                Console.WriteLine("Generic SocketMessage: {0}", e.Message.MessageText);
            else
                Console.WriteLine("Generic SocketMessage: {0} : {1}", e.Message.Event, e.Message.JsonEncodedMessage.ToJsonString());
        }

        void SocketOpened(object sender, EventArgs e)
        {

        }


        public void sendSimpleMessage(string message)
        {
            socket.Emit(message, new { });
        }

        public void Close()
        {
            if (this.socket != null)
            {
                socket.Opened -= SocketOpened;
                socket.Message -= SocketMessage;
                socket.SocketConnectionClosed -= SocketConnectionClosed;
                socket.Error -= SocketError;
                this.socket.Dispose(); // close & dispose of socket client
            }
        }

        public Client Socket
        {
            get { return socket; }
            set { socket = value; }
        }
    }
}
