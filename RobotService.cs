using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace ServioCoffeMakerRobot
{
    public class RobotService : IDisposable
    {
        private const byte PACKET_READ_ID = 133;
        private const byte PACKET_WRITE_ID = 134;

        private const byte PACKET_READ_WATCHDOG_ID2 = 198;
        private const byte PACKET_WRITE_WATCHDOG_ID2 = 199;

        public string ipAddress;
        public string port;

        private TcpClient tcpClient;
        private TcpListener tcpListener;
        private NetworkStream stream;
        private object locker;

        private Dictionary<string, byte> methodIds = new Dictionary<string, byte>();
        private Dictionary<int, string> responses = new Dictionary<int, string>();

        private List<byte> receiveBuffer;
        private int bytesRead;
        private string response;

        private static RobotService instance;
        static Thread watchdogThread;
        static Thread streamReaderThread;
        public static RobotService getInstance()
        {
            if (instance == null)
                instance = new RobotService();
            return instance;
        }
        private RobotService()
        {
            receiveBuffer = new List<byte>();
            bytesRead = 56653;

            methodIds.Add("ETH_IN_CMD_START", 99);
            methodIds.Add("ETH_IN_CMD_STOP", 98);
            methodIds.Add("ETH_IN_CMD_RESET", 97);
            methodIds.Add("ETH_IN_CMD_HOME", 96);
            methodIds.Add("ETH_IN_CMD_1", 95);
            methodIds.Add("ETH_IN_CMD_2", 94);
            methodIds.Add("ETH_IN_CMD_3", 93);
            methodIds.Add("ETH_IN_CMD_4", 92);
            methodIds.Add("ETH_IN_WATCHDOG", 91);
            methodIds.Add("ETH_IN_DRINK_TYPE", 90);
            methodIds.Add("ETH_IN_ADDONS_TYPE", 89);
            methodIds.Add("ETH_IN_CUP_SIZE", 88);

            methodIds.Add("ETH_OUT_ROBOT_STATUS", 101);
            methodIds.Add("ETH_OUT_CYCLE_ON", 102);
            methodIds.Add("ETH_OUT_CYCLE_OK", 103);
            methodIds.Add("ETH_OUT_CYCLE_FAULT", 104);
            methodIds.Add("ETH_OUT_HOME_CYCLE_ON", 105);
            methodIds.Add("ETH_OUT_IN_HOME", 106);
            methodIds.Add("ETH_OUT_1", 107);
            methodIds.Add("ETH_OUT_2", 108);
            methodIds.Add("ETH_OUT_3", 109);
            methodIds.Add("ETH_OUT_4", 110);
            methodIds.Add("ETH_OUT_5", 111);
            methodIds.Add("ETH_OUT_6", 112);
            methodIds.Add("ETH_OUT_7", 113);
            methodIds.Add("ETH_OUT_8", 114);
            methodIds.Add("ETH_OUT_9", 115);
            methodIds.Add("ETH_OUT_10", 116);
            methodIds.Add("ETH_OUT_11", 117);
            methodIds.Add("ETH_OUT_12", 118);
            methodIds.Add("ETH_OUT_13", 119);
            methodIds.Add("ETH_OUT_14", 120);
            methodIds.Add("ETH_OUT_15", 121);
            methodIds.Add("ETH_OUT_16", 122);
            methodIds.Add("ETH_OUT_17", 123);
            methodIds.Add("ETH_OUT_18", 124);
            methodIds.Add("ETH_OUT_WATCHDOG", 125);
            methodIds.Add("ETH_OUT_ERROR_CODE", 126);
            methodIds.Add("ETH_TOTAL_CYCLE_COUNTER", 127);
            methodIds.Add("ETH_TOTAL_CUPS_SMALL", 128);
            methodIds.Add("ETH_TOTAL_CUPS_MEDIUM", 129);
            methodIds.Add("ETH_TOTAL_CUPS_BIG", 130);
            methodIds.Add("ETH_CYCLE_TIME_COUNT", 131);
        }
        public void Start()
        {
            try
            {
                LoggerService.Write("RobotService INFO", "метод Start почав роботу");
                locker = new object();
                tcpClient = new TcpClient();
#if DEBUG
                LoggerService.Write("RobotService INFO", "Відбувається коннект TCP клієнта");
#endif
                tcpClient.Connect(ipAddress, Int32.Parse(port));
#if DEBUG
                LoggerService.Write("RobotService INFO", "Відбувся коннект TCP клієнта");
#endif
                stream = tcpClient.GetStream();
#if DEBUG
                LoggerService.Write("RobotService INFO", "Відкрився stream TCP клієнта");
#endif
                streamReaderThread = new Thread(StreamReader);
                streamReaderThread.Start();
                watchdogThread = new Thread(WatchdogHandler);
                watchdogThread.Start();
#if DEBUG
                LoggerService.Write("RobotService INFO", "Запустився WatchdogHandler в окремому потоці");
#endif
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі Start: {ex.Message}");
            }
        }
        public void Dispose()
        {
            try
            {
                LoggerService.Write("RobotService INFO", "Визивається метод Dispose TCP клієнта");
                try
                {
                    if (watchdogThread != null)
                        watchdogThread.Abort();
                }
                catch { }

                if (tcpClient != null)
                    tcpClient.Dispose();
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі Dispose: {ex.Message}");
            }
        }

        private List<byte> ConvertAsciiToHex(string str)
        {
            try
            {
                char[] charValues = str.ToCharArray();
                List<byte> hexOutput = new List<byte>();
                foreach (char _eachChar in charValues)
                {
                    hexOutput.Add((byte)_eachChar);
                }
                return hexOutput;
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі ConvertAsciiToHex: {ex.Message}");
            }
            return new List<byte>();
        }
        private byte[] СreateReadPacket(string command, byte ID1, byte ID2)
        {
            try
            {
#if DEBUG
                if (command != "ETH_IN_WATCHDOG" && command != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", "метод СreateReadPacket почав роботу");
#endif

                List<byte> bytes = new List<byte>();
                var cmd = ConvertAsciiToHex(command);
                var packetLenght = 3 + command.Length;
                byte ss;
                ss = (byte)packetLenght;
                byte sd;
                sd = (byte)command.Length;
                bytes.AddRange(new byte[] { ID1, ID2, 0x00, ss, 0X00, 0x00, sd });
                bytes.AddRange(cmd);
#if DEBUG
                if (command != "ETH_IN_WATCHDOG" && command != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", "Пакет на запит для читання змінної сформовано");
#endif
                return bytes.ToArray();
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі СreateReadPacket: {ex.Message}");
            }
            return new byte[0];
        }
        public byte[] CreateWritePacket(string value, string command, byte ID1, byte ID2)
        {
            try
            {
#if DEBUG
                if (command != "ETH_IN_WATCHDOG" && command != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", "метод CreateWritePacket почав роботу");
#endif
                List<byte> bytes = new List<byte>();
                var valueLenght = value.Length;
                var cmd = ConvertAsciiToHex(command);
                var packetLenght = 3 + command.Length + 2 + valueLenght;
                byte ss;
                ss = (byte)packetLenght;
                byte sd;
                sd = (byte)command.Length;
                bytes.AddRange(new byte[] { ID1, ID2, 0x00, ss, 0X01, 0x00, sd });
                bytes.AddRange(cmd);
                byte se;
                se = (byte)valueLenght;
                bytes.AddRange(new byte[] { 0x00, se });
                var sdf = ConvertAsciiToHex(value);
                bytes.AddRange(sdf);
#if DEBUG
                if (command != "ETH_IN_WATCHDOG" && command != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", "Пакет запиту для запису змінної сформовано");
#endif
                return bytes.ToArray();

            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі CreateWritePacket: {ex.Message}");
            }
            return new byte[0];
        }
        public void WatchdogHandler()
        {
            try
            {

                LoggerService.Write("RobotService INFO", "метод WatchdogHandler почав роботу");
                string watchdog = String.Empty;
                while (true)
                {
                    ReadOutWatchdog();
                    while (true)
                    {
                        if (responses.TryGetValue(PACKET_READ_WATCHDOG_ID2, out watchdog))
                        {
                            responses.Remove(PACKET_READ_WATCHDOG_ID2);
                            break;
                        }
                        Thread.Sleep(10);
                    }
                    WriteInWatchdog(watchdog);
                    Thread.Sleep(500);
                }
                LoggerService.Write("RobotService INFO", "метод WatchdogHandler закінчив роботу");
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі WatchdogHandler: {ex.Message}");
            }
        }

        private void StreamReader()
        {
            try
            {
                LoggerService.Write("RobotService INFO", "метод StreamReader почав роботу");
                lock (locker)
                {
                    int currentResponseID;
                    Stopwatch sw = Stopwatch.StartNew();
                    while (true)
                    {
                        if (sw.ElapsedMilliseconds > 3000)
                            throw new Exception("Waiting time for a response has passed");
                        int packetSize;
                        int responseSize;
                        if (stream.CanRead)
                        {
                            bytesRead = stream.ReadByte();
                            if ((byte)bytesRead == PACKET_READ_ID || (byte)bytesRead == PACKET_WRITE_ID)
                            {
                                currentResponseID = stream.ReadByte();
#if DEBUG
                                LoggerService.Write("RobotService INFO", $"Знайдено відповідний ID у пакеті відповіді {currentAnswerID}");
#endif
                                bytesRead = stream.ReadByte();
                                packetSize = stream.ReadByte();
                                bytesRead = stream.ReadByte();
                                bytesRead = stream.ReadByte();
                                responseSize = stream.ReadByte();

                                receiveBuffer.Clear();
                                //LoggerService.Write("RobotService INFO", "Початок зчитування відповіді");
                                for (int i = 0; i < responseSize; i++)
                                {
                                    bytesRead = stream.ReadByte();
                                    receiveBuffer.Add((byte)bytesRead);
                                }

                                response = String.Empty;
                                receiveBuffer.ForEach(x => response += (char)x);
                                if (currentResponseID != PACKET_READ_WATCHDOG_ID2 && currentResponseID != PACKET_WRITE_WATCHDOG_ID2)
                                    LoggerService.Write("RobotService INFO", $"Відповідь записана як массив символів >>>>>>>>>>>>>>> {response}");
                                responses.Remove(currentResponseID);
                                responses.Add(currentResponseID, response);

                                sw.Restart();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі StreamReader: ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ {ex.Message}");
                Dispose();
                Start();
            }
        }

        #region ETH_OUT
        public void ReadOutWatchdog()
        {
            var data = СreateReadPacket("ETH_OUT_WATCHDOG", PACKET_READ_ID, PACKET_READ_WATCHDOG_ID2);
            sendCommand(data);
        }
        public async Task<string> ReadPropertyAsync(string methodName)
        {
            return await Task.Run(() => ReadProperty(methodName));
        }
        public string ReadProperty(string methodName)
        {
            try
            {
                if (methodName != "ETH_IN_WATCHDOG" && methodName != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", $"метод ReadProperty('{methodName}') почав роботу");
                string result;
                byte packetReadId2;
                methodIds.TryGetValue(methodName, out packetReadId2);
                var data = СreateReadPacket(methodName, PACKET_READ_ID, packetReadId2);
                sendCommand(data);
                while (true)
                {
                    if (responses.TryGetValue(packetReadId2, out result))
                    {
                        responses.Remove(packetReadId2);
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (result == "RECONNECT" || string.IsNullOrEmpty(result))
                    return ReadProperty(methodName);
                return result;
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі ReadProperty: {ex.Message}");
                return $"ReadProperty error: {ex.Message}.";
            }
        }

        #endregion
        #region ETH_IN

        public void WriteInWatchdog(string value)
        {
            var data = CreateWritePacket(value, "ETH_IN_WATCHDOG", PACKET_WRITE_ID, PACKET_WRITE_WATCHDOG_ID2);
            sendCommand(data);
        }

        public bool WriteProperty(string methodName, string value)
        {
            try
            {
                if (methodName != "ETH_IN_WATCHDOG" && methodName != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", $"метод WriteProperty('{methodName}') почав роботу");
                string result;
                byte packetWriteId2;
                methodIds.TryGetValue(methodName, out packetWriteId2);
                var data = CreateWritePacket(value, methodName, PACKET_READ_ID, packetWriteId2);
#if DEBUG
                if (methodName != "ETH_IN_WATCHDOG" && methodName != "ETH_OUT_WATCHDOG")
                    LoggerService.Write("RobotService INFO", "Відправляється пакет даних");
#endif
                sendCommand(data);
                while (true)
                {
                    if (responses.TryGetValue(packetWriteId2, out result))
                    {
                        responses.Remove(packetWriteId2);
                        break;
                    }
                    Thread.Sleep(10);
                }
                if (result == "RECONNECT")
                    return WriteProperty(methodName, value);
                return result == value ? true : false;
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі WriteProperty: {ex.Message}");
                return false;
            }
        }

#endregion

        private void sendCommand(byte[] data)
        {
            sendToServer(data);
        }

        private void sendToServer(byte[] sendData)
        {
            try
            {
                lock (this)
                {
                    //LoggerService.Write("RobotService INFO", "Початок запису в потік");
                    if (stream != null)
                        stream.Write(sendData, 0, sendData.Length);
                    //LoggerService.Write("RobotService INFO", "Запис в потік завершено");
                }
            }
            catch (Exception ex)
            {
                LoggerService.Write("RobotService ERROR", $"Помилка в методі sendToServer: {ex.Message}");
            }
        }
    }
}
