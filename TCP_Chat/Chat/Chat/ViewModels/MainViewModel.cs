using Chat.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Chat.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        public static ObservableCollection<MessageBlock> msgBlocks { get; set; }
        //for client
        bool isConnected = false;

        private string name;
        private string ip;
        private string port;
        //connect or disconnect
        private string cd;
        private string message;
        private bool isenable1;
        private string vis;
        private int selectedI;

       
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        public string Ip
        {
            get { return ip; }
            set { ip = value; OnPropertyChanged("Ip"); }
        }
        public string Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged("Port"); }
        }
        public string Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged("Message"); }
        }
        public string Cd
        {
            get { return cd; }
            set { cd = value; OnPropertyChanged("Cd"); }
        }
        public bool IsEnable1
        {
            get { return isenable1; }
            set { isenable1 = value; OnPropertyChanged("IsEnable1"); }
        }
        public string Vis
        {
            get { return vis; }
            set { vis = value; OnPropertyChanged("Vis"); }
        }
        public int SelectedI
        {
            get { return selectedI; }
            set { selectedI = value; OnPropertyChanged("SelectedI"); }
        }

        TcpClient client;
        //write
        StreamWriter sw;
        //read
        StreamReader sr;

        public MainViewModel()
        {
            msgBlocks = new ObservableCollection<MessageBlock>();
            Name = null;
            Cd = "Connect";
            Vis = "Hidden";
            IsEnable1 = true;

            Task.Factory.StartNew(() => {

                while (true)
                {
                    try
                    {
                        if(client?.Connected == true)
                        {
                            //line will be something like this : name*message
                            var line = sr.ReadLine();

                            if(line != null)
                            {
                                string[] msg = line.Split(new char[] { '*' });

                                Console.WriteLine(msg.Length);

                                App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    msgBlocks.Add(new MessageBlock { Name = msg[0], Message = msg[1], Time = DateTime.Now.ToShortTimeString() });
                                    SelectedI = msgBlocks.Count - 1;
                                });
                            }
                            else
                            {
                                client.Close();
                            }
                        }
                        else
                        {
                            isConnected = false;
                            Cd = "Connect";
                        }
                        Task.Delay(10).Wait();
                    }
                    catch(Exception) {  }
                }
            });
        }

        //or disconnect
        public AsyncCommand ConnectCommand
        {
            get
            {
                return new AsyncCommand(() => {

                    return Task.Factory.StartNew(() => {
                        if (!isConnected)
                        {
                            try
                            {

                                client = new TcpClient();
                                client.Connect(Ip, int.Parse(Port));
                                sr = new StreamReader(client.GetStream());
                                sw = new StreamWriter(client.GetStream());
                                sw.AutoFlush = true;

                                sw.WriteLine($"Login: {Name}");

                                Cd = "Disconnect";
                                Vis = "Visible";
                       
                                isConnected = true;
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                        }
                        else
                        {
                            try
                            {
                                sw.WriteLine($"Leave: {Name}");

                                Cd = "Connect";
                                Vis = "Hidden";

                                App.Current.Dispatcher.BeginInvoke((Action)delegate ()
                                {
                                    msgBlocks.Clear();
                                });

                                isConnected = false;
                            }
                            catch (Exception) { }
                        }
                    });
                }, () => client == null && Name != null && Ip != null && Port != null);
            }
        }


        public AsyncCommand SendText
        {
            get
            {
                return new AsyncCommand(() =>
                {
                    return Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            sw.WriteLine($"{Name}*{Message}");
                            Message = String.Empty;
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    });
                }, () => client?.Connected == true && !string.IsNullOrWhiteSpace(Message));
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (isConnected == true)
            {
                sw.WriteLine($"Leave: {Name}");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
