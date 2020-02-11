using System;
using System.Net;
using System.Net.Sockets;

namespace ChatRoomServer2
{
    class Program
    {
        static void Main(string[] args)
        {
            //初始化
            TcpListener tcpListener = new TcpListener(IPAddress.Any,27105);
            tcpListener.Start();
            Console.WriteLine("服务建立完成，开始侦听");
            while(true)
            {
                //侦听，接收后放进connection的新线程
                TcpClient c = tcpListener.AcceptTcpClient();
                Connection connection = new Connection(c);
                connection.StartService();
                Console.WriteLine("建立了新的连接");
            }
        }
    }
}
