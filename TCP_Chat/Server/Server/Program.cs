using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Program
    {
        static TcpListener listener = new TcpListener(IPAddress.Any, 8888);
        static List<User> users = new List<User>();

        static void Main(string[] args)
        {

            listener.Start();
            Console.WriteLine("Server is started. Waiting connections...");

            Task.Factory.StartNew(() => {

                while (true)
                {
                    string command = Console.ReadLine();
                    if(command == "/ip")
                    {
                        try
                        {
                            string Host = Dns.GetHostName();
                            string IP = Dns.GetHostByName(Host).AddressList[0].ToString();
                            Console.WriteLine(IP);
                        }
                        catch(Exception ex) { Console.WriteLine(ex.Message); }
                    }
                }
            });

            while (true)
            {
                //wait client connection
                var client = listener.AcceptTcpClient();

                //new tread for login user
                Task.Factory.StartNew(() => {

                    var sr = new StreamReader(client.GetStream());

                    while (client.Connected)
                    {
                        //wait incoming message (transfer user name)
                        var line = sr.ReadLine();

                        if (line.Contains("Login: ") && !string.IsNullOrWhiteSpace(line.Replace("Login: ", String.Empty)))
                        {
                            var username = line.Replace("Login: ", String.Empty);

                            if (users.FirstOrDefault(c => c.Name == username) == null)
                            {
                                users.Add(new User { Name = username, Client = client });
                                Console.WriteLine($"New connection: {username}");
                                SendToAllClients($"System*{username} join the chat");
                                break;
                            }
                            else
                            {
                                var sw = new StreamWriter(client.GetStream());
                                sw.WriteLine("System*User with the same name already exists :(");
                                client.Client.Disconnect(false);
                            }
                        }

                    }

                    while (client.Connected)
                    {
                        try
                        {
                            sr = new StreamReader(client.GetStream());
                            var line = sr.ReadLine();
                            if (line != null)
                            {
                                if (line.Contains("Leave: ") && !string.IsNullOrWhiteSpace(line.Replace("Leave: ", String.Empty)))
                                {
                                    var user = users.FirstOrDefault(c => c.Name == line.Replace("Leave: ", String.Empty));
                                    user.Client.Client.Disconnect(false);
                                    users.Remove(user);
                                    Console.WriteLine($"Lost connection: {line.Replace("Leave: ", String.Empty)}");
                                    SendToAllClients($"System*{line.Replace("Leave: ", String.Empty)} leave the chat");
                                }
                                else
                                {
                                    Console.WriteLine(line);
                                    SendToAllClients(line);
                                }
                            }

                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }
                    }
                });
            }


        }

        static async void SendToAllClients(string message)
        {
            await Task.Factory.StartNew(() => { 
                foreach(var user in users)
                {
                    try
                    {
                        if (user.Client.Connected)
                        {
                            var sw = new StreamWriter(user.Client.GetStream());
                            sw.AutoFlush = true;

                            sw.WriteLine(message);
                        }
                        else
                        {
                            users.Remove(user);
                        }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message);  }
                }
            });
        }
    }
}
