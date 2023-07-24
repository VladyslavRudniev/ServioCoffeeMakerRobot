//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;

//namespace ServioCoffeMakerRobot
//{
//    public class DeviceClient
//    {
//        private static DeviceClient instance;
//        private Dictionary<string, byte> methodIds = new Dictionary<string, byte>();
//        private readonly object locker;
//        private string server;
//        private int port;
//        private Stopwatch watchdogWatch = new Stopwatch();
//        private DateTime lastReceived = DateTime.MinValue;
//        private byte[] dataBuffer = new byte[65536];
//        /// <summary>
//        /// Установлена ли связь с устройством
//        /// </summary>
//        private bool v_isConnectToDevice = false;
//        private Socket clientSocket;
//        private Thread v_watchdogWorker;
//        private Thread v_receivedWorker;

//        public static DeviceClient getInstance()
//        {
//            if (instance == null)
//                instance = new DeviceClient();
//            return instance;
//        }
//        private DeviceClient()
//        {
//            locker = new object();
//            v_receivedWorker = new Thread(m_ReadReceiveBuffer);
//            v_watchdogWorker = new Thread(WatchdogHandler);

//            methodIds.Add("ETH_IN_CMD_START", 99);
//            methodIds.Add("ETH_IN_CMD_STOP", 98);
//            methodIds.Add("ETH_IN_CMD_RESET", 97);
//            methodIds.Add("ETH_IN_CMD_HOME", 96);
//            methodIds.Add("ETH_IN_CMD_1", 95);
//            methodIds.Add("ETH_IN_CMD_2", 94);
//            methodIds.Add("ETH_IN_CMD_3", 93);
//            methodIds.Add("ETH_IN_CMD_4", 92);
//            methodIds.Add("ETH_IN_WATCHDOG", 91);
//            methodIds.Add("ETH_IN_DRINK_TYPE", 90);
//            methodIds.Add("ETH_IN_ADDONS_TYPE", 89);
//            methodIds.Add("ETH_IN_CUP_SIZE", 88);

//            methodIds.Add("ETH_OUT_ROBOT_STATUS", 101);
//            methodIds.Add("ETH_OUT_CYCLE_ON", 102);
//            methodIds.Add("ETH_OUT_CYCLE_OK", 103);
//            methodIds.Add("ETH_OUT_CYCLE_FAULT", 104);
//            methodIds.Add("ETH_OUT_HOME_CYCLE_ON", 105);
//            methodIds.Add("ETH_OUT_IN_HOME", 106);
//            methodIds.Add("ETH_OUT_1", 107);
//            methodIds.Add("ETH_OUT_2", 108);
//            methodIds.Add("ETH_OUT_3", 109);
//            methodIds.Add("ETH_OUT_4", 110);
//            methodIds.Add("ETH_OUT_5", 111);
//            methodIds.Add("ETH_OUT_6", 112);
//            methodIds.Add("ETH_OUT_7", 113);
//            methodIds.Add("ETH_OUT_8", 114);
//            methodIds.Add("ETH_OUT_9", 115);
//            methodIds.Add("ETH_OUT_10", 116);
//            methodIds.Add("ETH_OUT_11", 117);
//            methodIds.Add("ETH_OUT_12", 118);
//            methodIds.Add("ETH_OUT_13", 119);
//            methodIds.Add("ETH_OUT_14", 120);
//            methodIds.Add("ETH_OUT_15", 121);
//            methodIds.Add("ETH_OUT_16", 122);
//            methodIds.Add("ETH_OUT_17", 123);
//            methodIds.Add("ETH_OUT_18", 124);
//            methodIds.Add("ETH_OUT_WATCHDOG", 125);
//            methodIds.Add("ETH_OUT_ERROR_CODE", 126);
//            methodIds.Add("ETH_TOTAL_CYCLE_COUNTER", 127);
//            methodIds.Add("ETH_TOTAL_CUPS_SMALL", 128);
//            methodIds.Add("ETH_TOTAL_CUPS_MEDIUM", 129);
//            methodIds.Add("ETH_TOTAL_CUPS_BIG", 130);
//            methodIds.Add("ETH_CYCLE_TIME_COUNT", 131);
//        }
//        public void m_Connect()
//        {
//            lock (locker)
//            {
//                //Подключаемся по Tcp/Ip
//                LoggerService.Write("RobotService INFO", string.Format("Устанавливается подключение к {0}:{1}.", server, port));

//                if (clientSocket == null || !clientSocket.Connected)
//                {
//                    clientSocket = null;
//                    v_isConnectToDevice = false;
//                    if (clientSocket == null)
//                    {
//                        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                    }
//                    //clientSocket.Connect(endPoint);
//                    //SocketExtensions.Connect(clientSocket, server, port, new TimeSpan(0, 0, 1));
//                    watchdogWatch.Stop();
//                    try
//                    {
//                        SocketExtensions.Connect2(clientSocket, server, port, new TimeSpan(0, 0, 3));
//                    }
//                    catch (Exception ex)
//                    {
//                        LoggerService.Write("RobotService ERROR", $"Помилка в методі m_Connect: {ex.Message}");
//                    }
//                    finally
//                    {
//                        watchdogWatch.Restart();
//                    }
//                    v_isConnectToDevice = clientSocket.Connected; //true;
//                    if (v_isConnectToDevice)
//                    {
//                        lastReceived = DateTime.Now;

//                        clientSocket.BeginReceive(dataBuffer, 0, 65536, SocketFlags.None,
//                            new AsyncCallback(GettedData), clientSocket);

//                        //m_SendCommand((byte)ECMD_CODE.CMD_GETCOUNTER, EMPTY_DATA, true);
//                    }
//                }
//                //v_isConnectToDevice = clientSocket.Connected;
//                v_isConnectToDevice = clientSocket.Connected; //true;

//                if (v_isConnectToDevice)
//                {
//                    LoggerService.Write("RobotService INFO", string.Format("{0}:{1} порт занят. Связь с устройством установлена.", server, port));
//                }
//                else
//                    LoggerService.Write("RobotService INFO", string.Format("{0}:{1} порт занят. Связь с устройством установить не удалось.", server, port));
//            }
//        }

//        private void GettedData(IAsyncResult ar)
//        {
//            try
//            {
//                Socket curSock = (Socket)ar.AsyncState;
//                int bytesRead = curSock.EndReceive(ar);
//                IPEndPoint rep = (IPEndPoint)curSock.RemoteEndPoint;

//                if (bytesRead > 0)
//                {
//                    OnRecieveData(dataBuffer, bytesRead);
//                    curSock.BeginReceive(dataBuffer, 0, 65536, SocketFlags.None,
//                        new AsyncCallback(GettedData), clientSocket);
//                    lastReceived = DateTime.Now;
//                }
//                else
//                    OnServerDisconnect();
//            }
//            catch
//            {
//            }
//        }
//        public void SendToServer(byte[] sendData)
//        {
//            clientSocket.Send(sendData);
//            clientSocket.Send(new byte[0] { });
//        }
//        protected virtual void OnServerDisconnect()
//        {
//            v_isConnectToDevice = false;
//            if (clientSocket != null && clientSocket.Connected)
//                clientSocket.Disconnect(false);
//        }
//        public void m_Disconnect()
//        {
//            try
//            {
//                if (v_receivedWorker != null)
//                    v_receivedWorker.Abort();

//                if (v_watchdogWorker != null)
//                    v_watchdogWorker.Abort();

//                clientSocket.Shutdown(SocketShutdown.Both);
//            }
//            catch (Exception ex)
//            {
//                LoggerService.Write("RobotService ERROR", $"Не удалось разорвать соединение с устройством по Net-протоколу: {ex.Message}");
//            }
//        }
//        private void m_ReadReceiveBuffer()
//        {
//            var indexOfBegin = -1;
//            var indexOfEnd = -1;
//            var packetLen = -1;
//            var cmdB = 0x00;
//            while (true)
//            {
//                //Читаем все что есть на текущий момент в буфере
//                lock (locker)
//                {
//                    try
//                    {
//                        indexOfBegin = v_receiveBuffer.IndexOf(BYTE_MSG_BEGIN);
//                        if (indexOfBegin >= 0 || indexOfBegin + 2 < v_receiveBuffer.Count)
//                        {
//                            packetLen = v_receiveBuffer[indexOfBegin + 2];
//                            if (indexOfBegin + 2 + packetLen <= v_receiveBuffer.Count)
//                            {
//                                indexOfEnd = indexOfBegin + 2 + packetLen;
//                                cmdB = v_receiveBuffer[indexOfBegin + 3];
//                                //if (Enum.IsDefined(typeof(ECMD_CODE), cmdB))
//                                if (cmdB >= (byte)ECMD_CODE.CMD_ECHO && cmdB <= (byte)ECMD_CODE.CMD_EV_DATA_TRANSFER)
//                                {
//                                    byte[] data = v_receiveBuffer.Skip(indexOfBegin + 3).Take(packetLen - 1).ToArray();
//                                    var cmdResponse = new CommandResponse()
//                                    {
//                                        Command = (ECMD_CODE)cmdB,
//                                        NumChannel = v_receiveBuffer[indexOfBegin + 1],
//                                        Data = data,
//                                        Received = DateTime.Now
//                                    };
//                                    //Добавляем в кэш принятый пакет-команду
//                                    Answers.Add(cmdResponse);
//                                    //Удаляем из буфера принятый пакет (вместе с принятым раннее "мусором")
//                                    v_receiveBuffer.RemoveRange(0, indexOfEnd);
//                                    //Сообщаем что получен новый ответ
//                                    DoDeviceEvent?.Invoke();
//                                }
//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        WriteDeviceLog(ex.Message);
//                    }
//                }
//                waitReceiveHandler.WaitOne();
//            }
//        }
//    }
//}