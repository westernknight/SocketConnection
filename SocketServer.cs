using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SocketConnection
{
    public class SocketServer : SocketConnection
    {
        List<Socket> socketList = new List<Socket>();
        
        public void Start(int port)
        {

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, port));
                socket.Listen(int.MaxValue);

                Msg("server on line,wait for connection...");
            }
            catch (Exception ex)
            {
                Msg(ex);
            }

            socket.BeginAccept((ar) =>
            {
                AcceptCallback(ar);
            }, socket);
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                socket.BeginAccept((ar2) =>
                {
                    AcceptCallback(ar2);

                }, socket);


                Socket ts = socket.EndAccept(ar);
                socketList.Add(ts);

                ar.AsyncWaitHandle.Close();
                Msg("one client has connect");

                ts.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, ServerReceiveCallback, ts);

            }
            catch (Exception e)
            {
                Msg(e);
            }
        }

        private void ServerReceiveCallback(IAsyncResult result)
        {
            try
            {
                Socket ts = (Socket)result.AsyncState;
                int c = ts.EndReceive(result);

                result.AsyncWaitHandle.Close();
                if (c == 0)
                {
                    ts.Disconnect(false);
                    socketList.Remove(ts);
                }
                else
                {
                    byte[] packageHeader = new byte[4];
                    packageHeader[0] = tmpData[0];
                    packageHeader[1] = tmpData[1];
                    packageHeader[2] = tmpData[2];
                    packageHeader[3] = tmpData[3];

                    int bodyLength = BitConverter.ToInt32(packageHeader, 0);
                    byte[] bodyData = new byte[bodyLength];
                    ReceivePackage(ts, c, tmpData, bodyLength, bodyData);
                    ts.BeginReceive(tmpData, 0, tmpData.Length, SocketFlags.None, ReceiveCallback, ts);
                }
            }
            catch (Exception ex)
            {
                Msg(ex);
            }
        }
   
        public void SendPackage(int socketid,string jsonData)
        {
            try
            {
                Socket socket = socketList[socketid];
                if (socket != null)
                {
                    if (socket.Connected)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(jsonData);
                        byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                        socket.Send(send_data_with_length);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }           

        }
        public void SendPackage(int socketid, byte[] data)
        {

            try
            {
                Socket socket = socketList[socketid];
                if (socket != null)
                {
                    if (socket.Connected)
                    {
                        byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                        socket.Send(send_data_with_length);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


        public void BoardcastMessage(byte[] data)
        {

            try
            {
                for (int socketid = 0; socketid < socketList.Count; socketid++)
                {
                    Socket socket = socketList[socketid];
                    if (socket != null)
                    {
                        if (socket.Connected)
                        {
                            byte[] send_data_with_length = BuildPack(BitConverter.GetBytes(data.Length), data);
                            socket.Send(send_data_with_length);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
      
    }
}
