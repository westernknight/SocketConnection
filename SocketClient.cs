using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace SocketConnection
{
    class SocketClient : SocketConnection
    {
        string ip;
        int port;
        bool netExit = false;//manager不在线是否重拨
        public void Start(string ip, int port)
        {

            this.ip = ip;
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

            Msg("connect to server [" + ip + ":" + port + "]");

            socket.BeginConnect(ipep,ConnectCallback,socket);
        }
        private void ConnectCallback(System.IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);
                ar.AsyncWaitHandle.Close();

                socket.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                Msg("connect  [" + ip + ":" + port + "] success");

                //byte[] inValue = new byte[] { 1, 0, 0, 0, 0x88, 0x13, 0, 0, 0x88, 0x13, 0, 0 };
                //socketClient.IOControl(IOControlCode.KeepAliveValues, inValue, null);
            }
            catch (System.Exception ex)
            {
                Msg(ex);
                if (netExit == false)
                {
                    Thread.Sleep(1000);//sleep 1 second
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

                    socket.BeginConnect(
                    ipep,
                    new System.AsyncCallback(ConnectCallback),
                    socket);
                }

            }
        }

        public void SendPackage(string jsonData)
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    byte [] data = Encoding.UTF8.GetBytes(jsonData);
                    byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                    socket.Send(send_data_with_length);
                }
                
            }

        }
    }
}
