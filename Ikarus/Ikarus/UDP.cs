﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Threading;


namespace Ikarus
{
    static class UDP
    {
        private static bool closeListener = true;
        public static bool CloseListener
        {
            get { return closeListener; }
        }
        private static UdpClient listener = null;
        private static IPAddress sendToAddress = null;
        private static IPEndPoint sendingEndPoint = null;
        private static IPEndPoint listenerEndPoint = null;
        private static Socket sendingSocket = null;
        public static string receivedData = "";
        private static string newline = Environment.NewLine;
        public static List<string> receivedDataStack = new List<string> { };

        #region comments
        // Create a socket object. This is the fundamental device used to network
        // communications. When creating this object we specify:
        //
        // Internetwork:    We use the internet communications protocol
        // Dgram:           We use datagrams or broadcast to everyone rather than send to
        //                  a specific listener
        // UDP:             the messages are to be formated as user datagram protocal.
        //                  The last two seem to be a bit redundant.
        #endregion

        public static void UDPSender(string ipAdress, int port, string textToSend)
        {
            #region comments
            // create an address object and populate it with the IP address that we will use
            // in sending at data to. This particular address ends in 255 meaning we will send
            // to all devices whose address begins with 192.168.2.
            #endregion

            ipAdress = ipAdress.Substring(0, ipAdress.LastIndexOf(".") + 1) + "255"; // 127.0.0.1 -> 127.0.0.255

            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendToAddress = IPAddress.Parse(ipAdress);
            sendingEndPoint = new IPEndPoint(sendToAddress, port);

            byte[] send_buffer = Encoding.ASCII.GetBytes(textToSend);

            try
            {
                sendingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0); // ???
                sendingSocket.SendTo(send_buffer, sendingEndPoint);
                sendingSocket.Close();
            }
            catch (Exception e)
            {
                ImportExport.LogMessage("Send Exception: " + e.Message + "...", true);
            }
        }

        public static void StartListener(int listenPort)
        {
            try
            {
                listener = new UdpClient(listenPort);
            }
            catch (Exception e)
            {
                ImportExport.LogMessage("Listener Exception: " + e.Message, true);
            }

            listenerEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

            byte[] receiveByteArray;

            try
            {
                ImportExport.LogMessage("Listener started on Port " + listenPort + "...", true);
                closeListener = false;

                while (!closeListener)
                {
                    receiveByteArray = listener.Receive(ref listenerEndPoint);
                    receivedData = Encoding.ASCII.GetString(receiveByteArray, 0, receiveByteArray.Length);

                    receivedData = receivedData.Replace("*", ":").Trim();
                    receivedDataStack.Add(receivedData);

                    if (MainWindow.detailLog) { ImportExport.LogMessage("--- Received package: " + receivedData, true); }

                    //ImportExport.LogMessage("--- Received package count: " + receivedDataStack.Count, true);

                    MainWindow.mainWindow[0].GrabValues();
                }
            }
            catch (Exception e)
            {
                ImportExport.LogMessage("Listener Exception: " + e.Message, true);
            }
            finally
            {
                ListenerClose();
            }
        }

        public static void ListenerClose()
        {
            try
            {
                closeListener = true;
                listener.Close();
                ImportExport.LogMessage("Listener closed ... " + receivedData, true);
            }
            catch (Exception e)
            {
                ImportExport.LogMessage("Listener closed ... " + e.ToString(), true);
            }
        }
    }
}
