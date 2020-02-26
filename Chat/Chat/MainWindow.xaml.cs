using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Drawing;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Net;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using TCP_SERVER.Models;
using System.Net.Sockets;

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClientService clientService { get; set; }
        private Message _message { get; set; }

        private User MyCLient { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            clientService = new ClientService();

            clientService.CommonChatReciveEvent += CommonChatRecive;
            clientService.PrivatChatReciveEvent += PrivateChatRecive;
            clientService.ClientListUpdated += NewClientList;

            var ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.LastOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            MyCLient = new User() { Name = "", IpAddress = ip.ToString() };

            _message = new Message();
            _message.FromUser = MyCLient;
        }

        private void NewClientList(List<User> obj)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ContactStackPanel.Children.Clear();

                foreach (var user in obj)
                {

                    var btn = new Button()
                    {
                        Content = $"{user.Name}",
                        Tag = user,
                        Margin = new Thickness(10)
                    };
                    btn.Click += (x, y) =>
                    {
                        _message.ToUser = ((Button)x).Tag as User;
                    };

                    ContactStackPanel.Children.Add(btn);

                }


                var btn2 = new Button()
                {
                    Content = $"COMMON CHAT",
                    Tag = null,
                    Margin = new Thickness(10)
                };
                btn2.Click += (x, y) =>
                {
                    _message.ToUser = null;
                };

                ContactStackPanel.Children.Add(btn2);
            }));
        }

        private void PrivateChatRecive(Message obj)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

                MessagePanel.Children.Add(new TextBlock() { Text = $"{obj.FromUser.Name}  : {obj.Message_}", Foreground = Brushes.Red });
            }));


        }

        private void CommonChatRecive(Message obj)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessagePanel.Children.Add(new TextBlock() { Text = $"{obj.FromUser.Name}  : {obj.Message_}", Foreground = Brushes.Black });
            }));
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await clientService.Connect();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MyCLient.Name = NameTextBox.Text;
            _message.Message_ = MessageBox.Text;

            await clientService.SendMessage(_message);
        }



    }



}
