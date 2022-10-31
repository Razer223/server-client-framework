﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientFramework {
    public class Commands {
        public static void Help() {
            Console.WriteLine("Commands: ");
            Console.WriteLine();
            Console.WriteLine("Clear        | Clears console");
            Console.WriteLine("Users        | Users connected to server");
            Console.WriteLine("Connect      | Connect to server");
            Console.WriteLine("Disconnect   | Disconnect from server");
            Console.WriteLine("Status       | Check if connected to server");
            Console.WriteLine("SendData     | Sends a command to another client/server");
            Console.WriteLine("RequestData  | Gets a value from another client/server");
            Console.WriteLine("Exit         | Closes application");
        }
        public static void UserList() {
            if (Network.OtherClients.Count() == 0) {
                Console.WriteLine("No other clients connected");
                return;
            }
            Console.WriteLine("Connected clients: ");

            int i = 1;
            foreach (Network.OtherClient _client in Network.OtherClients) {
                Console.WriteLine($"    ({i}) ID={_client.Id} Name={_client.UserName}");
                i++;
            }
        }
        public static void SendData() {
            if (!Network.Client.HandshakeDone)
                throw new Exception("Connect to server first!");

            Console.WriteLine();
            Console.WriteLine("method to be sent to client/server: ");
            string method = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("DATA string to be sent to client/server: ");
            string data = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Target ID: (Blank or 0 for all clients)");
            string target;
            while (true) {
                target = Console.ReadLine();
                if (!string.IsNullOrEmpty(target)) break;
                Console.WriteLine("Invalid target, try again!");
            }

            Network.NetworkMessage message = new Network.NetworkMessage {
                Parameters = Network.SerializeParameters(data),
                ParametersNew = data,
                MethodName = "test",
                TargetId = Int32.Parse(target)
            };
            Network.SendData(message);
        }
        public static void RequestData() {
            if (!Network.HandshakeDone)
                throw new Exception("Connect to server first!");

            Console.WriteLine();
            Console.WriteLine("method to be sent to client/server: ");
            string method = Console.ReadLine();
            Console.WriteLine();
            Console.WriteLine("Target ID: (Blank or 0 for all clients)");
            string target;
            while (true) {
                target = Console.ReadLine();
                if (!string.IsNullOrEmpty(target)) break;
                Console.WriteLine("Invalid target, try again!");
            }

            Network.NetworkMessage message = new Network.NetworkMessage {
                Parameters = Network.SerializeParameters("Hello From Client"),
                MethodName = "Test",
                TargetId = Int32.Parse(target)
            };
            string a = Network.RequestData<string>(message);
            Console.WriteLine($"RETURNED:{a}");
        }
        public static void RequestDataType() {
            if (!Network.Client.Connected)
                throw new Exception("Connect to server first!");

            Network.NetworkMessage message = new Network.NetworkMessage {
                Parameters = Network.SerializeParameters("Hello From Client"),
                MethodName = "TestType",
                TargetId = 1
            };
            TestClass a = Network.RequestData<TestClass>(message);
            Console.WriteLine($"RETURNED:{a.StringTest}");
            Console.WriteLine($"RETURNED:{a.Data[0]}");
        }
    }
}
