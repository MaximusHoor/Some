using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TCP_SERVER.Models;

namespace Chat
{
   public class ClientService
    {
        private TcpClient _Client { get; set; }
        public ClientList ListClients { get; set; }

        public event Action<Message> CommonChatReciveEvent;

        public event Action<Message> PrivatChatReciveEvent;

        public event Action<List<User>> ClientListUpdated;


        public ClientService()
        {
            _Client = new TcpClient();
        }

        public async Task Connect()
        {
           await _Client.ConnectAsync(IPAddress.Parse("40.115.111.3"), 7777);
            await RadLoopAsync();
        }

        public async Task RadLoopAsync()
        {
            while (true)
            {


                var buffer = new byte[2048];
                var count = await _Client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                var message = JsonConvert.DeserializeObject<Message>(Encoding.Unicode.GetString(buffer, 0, count));

                if (message.Header == "ClientList")
                {
                    ListClients = JsonConvert.DeserializeObject<ClientList>(message.JsonData);
                    ClientListUpdated.BeginInvoke(ListClients.UsersOnline,null,null);
                }
                else if (message.Header == "CommonChat")
                    CommonChatReciveEvent.BeginInvoke(message, null, null);
                else
                  PrivatChatReciveEvent.BeginInvoke(message, null, null);
            }

        }

        public async Task SendMessage(Message message)
        {
            var byteArray = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(message));
            await _Client.GetStream().WriteAsync(byteArray, 0, byteArray.Length);
        }  
    }
}
