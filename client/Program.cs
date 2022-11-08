﻿
using System;
using System.Net.Mime;
using System.Reflection;


namespace ClientFramework {
    public class Program {
        
        
        
        public static void OnClientConnected(object sender, OnClientConnectEvent eventData){
            Console.WriteLine($"*EVENT* CLIENT CONNECTED! ({eventData.UserName} ID:{eventData.Id})");
        }
        public static void OnClientDisconnect(object sender, OnClientDisconnectEvent eventData){
            Console.WriteLine($"*EVENT* CLIENT DISCONNECTED! ({eventData.UserName} ID:{eventData.Id} SUCCESS:{eventData.Success})");
        }
        public static void OnServerShutdown(object sender, OnServerShutdownEvent eventData){
            Console.WriteLine($"*EVENT* SERVER STOPPED! SUCCESS:{eventData.Success}");
        }
        public static void OnMessageSent(object sender, OnMessageSentEvent eventData){
            Console.WriteLine($"*EVENT* MSG SENT: {eventData.Message.MethodName}");
        }
        public static void OnMessageReceived(object sender, OnMessageReceivedEvent eventData){
            Console.WriteLine($"*EVENT* MSG RECEIVED: {eventData.Message.MethodName}");
        }
        public static void Main(string[] args) {
            NetworkEvents.eventsListener = new NetworkEvents();
            NetworkEvents.eventsListener.ClientConnected += OnClientConnected;
            NetworkEvents.eventsListener.ClientDisconnect += OnClientDisconnect;
            NetworkEvents.eventsListener.ServerShutdown += OnServerShutdown;
            NetworkEvents.eventsListener.MessageSent += OnMessageSent;
            NetworkEvents.eventsListener.MessageReceived += OnMessageReceived;
            

            Console.Title = "CLIENT";
            Console.Clear();
            Console.WriteLine("Type 'help' for commands!");

            while (true) {
                Console.Write("> ");
                string? command = Console.ReadLine();
                command = command.ToLower();

                try {
                    switch (command)
                    {
                        case "help":
                            Commands.Help();
                            break;
                        case "clear":
                            Console.Clear();
                            break;
                        case "exit":
                            Environment.Exit(0);
                            break;
                        case "connect":
                            Console.WriteLine("Enter IP adress:");
                            string ip = Console.ReadLine();
                            if (string.IsNullOrEmpty(ip)) ip = "127.0.0.1";
                            Console.WriteLine("Enter Port");
                            string port = Console.ReadLine();
                            if (string.IsNullOrEmpty(port)) port = "5001";
                            Console.WriteLine("Username:");
                            string name = Console.ReadLine();
                            if (string.IsNullOrEmpty(name)) {
                                Random rd = new Random();
                                name = ("RANDOMUSER" + rd.Next(1,10).ToString());
                            }
                            Network.Connect(ip,Int32.Parse(port),name);
                            break;
                        case "disconnect":
                            Network.Disconnect();
                            break;
                        case "users":
                            Commands.UserList();
                            break;
                        case "senddata":
                            Commands.SendData();
                            break;
                        case "clientmethods":
                            Commands.GetClientMethods();
                            break;
                        case "servermethods":
                            Commands.GetServerMethods();
                            break;
                        case "requestdata":
                            Commands.RequestData();
                            break;
                        case "requestdatatype":
                            Commands.RequestDataType();
                            break;
                        case "status":
                            Console.WriteLine(Network.IsConnected() ? "Connected to server! ID:" + Network.Client.Id.ToString() : "NOT connected to server!");
                            break;
                        default:
                            Console.WriteLine("Unknown command!\nType 'help' for commands!");
                            break;
                    }  
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                Console.WriteLine();
            }
        }
    }
}