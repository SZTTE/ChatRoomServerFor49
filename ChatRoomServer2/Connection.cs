using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatRoomServer2
{
    public class Connection
    {
        private TcpClient client;
        private delegate void bytesMethod(byte[] bs);
        private static event bytesMethod messageComeEvent;//时间相当于一个委托收集器，可以一次运行所有的委托
        NetworkStream networkStream;//每个实例的数据流
        public Connection(System.Net.Sockets.TcpClient c)
        {
            client = c;
        }
        public void StartService()
        {
            Thread t = new Thread(ConnectionThread);
            t.Start();
            messageComeEvent += new bytesMethod(this.Send);//把这个实例的发送方法，交给事件统一管理
        }
        private void ConnectionThread()
        {
            Console.WriteLine("已经进入连接线程");
            networkStream = client.GetStream();
            while(true)//在这里保持收听，并且在取得新消息之后转发
            {
                try
                {
                    if (networkStream.CanRead)//当取得新信息，就广播出去
                    {
                        Byte[] data = new Byte[1000];
                        int i = networkStream.Read(data, 0, data.Length);
                        string rawString = System.Text.Encoding.UTF8.GetString(data, 0, i);
                        Console.WriteLine("收取到新消息：");
                        Console.WriteLine(rawString);
                        if (!rawString.Contains('\x3'))
                        {
                            throw new System.IO.IOException();
                        }

                        messageComeEvent(data);
                    }
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine("一个连接断开了");
                    networkStream.Close();
                    client.Dispose();
                    return;
                }
            }
        }
        private void Send(byte[] bs)
        {
            if (client.Connected)
            {
                Console.WriteLine("转发了一次");
                networkStream.Write(bs, 0, bs.Length);
            }
        }
    }
}
