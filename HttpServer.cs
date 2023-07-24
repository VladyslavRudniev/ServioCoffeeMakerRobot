using Newtonsoft.Json;
using ServioCoffeMakerRobot.CommandResults;
using ServioCoffeMakerRobot.Commands;
using ServioCoffeMakerRobot.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPI.OWIN.Controllers;

namespace ServioCoffeMakerRobot
{
    public class HttpServer : IDisposable
    {
        private HttpListener v_Listener;
        private Thread v_ListenerThread;
        private string localPort;
        private string ipAddress;
        private ManualResetEvent v_Stop, v_Ready;

        public HttpServer(string ipAddress, string localPort)
        {
            this.localPort = localPort;
            this.ipAddress = ipAddress;
            ServicePointManager.Expect100Continue = false;
        }

        public void StopListening()
        {
            v_Listener.Close();
            try
            {
                v_ListenerThread.Abort();
            }
            catch (Exception ex)
            {
                LoggerService.Write("HttpServer ERROR", $"Помилка в методі Stop: {ex.Message}");
            }
        }

        public void StartListening()
        {
            try
            {
                v_Stop = new ManualResetEvent(false);
                v_Ready = new ManualResetEvent(false);
                v_Listener = new HttpListener();
                v_ListenerThread = new Thread(HandleRequests);
                v_Listener.Prefixes.Add(string.Format("http://*:{0}/", localPort));
                v_Listener.Prefixes.ToList().ForEach(p => { LoggerService.Write("Prefixes", $"P: {p}"); });
                v_Listener.Start();
                v_ListenerThread.Start();
            }
            catch (Exception ex)
            {
                LoggerService.Write("HttpServer ERROR", $"Помилка в методі Start: {ex.Message}");
            }
            
        }
        private void HandleRequests()
        {
            try
            {
                while (v_Listener.IsListening)
                {
                    var context = v_Listener.BeginGetContext(ContextReady, null);

                    if (0 == WaitHandle.WaitAny(new[] { v_Stop, context.AsyncWaitHandle }))
                        return;
                }
            }
            catch (Exception e)
            {
            }
        }

        private void ContextReady(IAsyncResult ar)
        {
            if (v_Listener == null || !v_Listener.IsListening) return;
            HttpListenerContext context = v_Listener.EndGetContext(ar);
            HttpListenerRequest request = null;
            HttpListenerResponse response = null;
            CommandResult res;
            try
            {
                request = context.Request;
                response = context.Response;
                var body = request.InputStream;
                var reader = new System.IO.StreamReader(body, Encoding.UTF8);
                var requestString = reader.ReadToEnd();
                byte[] responseBuffer;
                string json = String.Empty;
                var httpmethod = request.HttpMethod;
                var url = request.Url;
                body.Close();
                reader.Close();
                
                int len = request.Url.Segments.Length;
                var segments = request.Url.Segments.ToList().Select(s => s.Replace("/", "").ToLower()).ToList();

                if (request.HttpMethod == "HEAD")
                {
                    SetErrorResponse(response, 200, null);
                    return;
                }
                var command = segments[1];
                SingleCommandResult result = new SingleCommandResult();
                RobotStateCommandResult robotState = new RobotStateCommandResult();
                CommandResult commandResult = new CommandResult();
                commandResult.Success = true;
                result.Success = false;
                robotState.Success = false;
                LoggerService.Write("HttpServer INFO", $"ОТРИМАН ЗАПИТ ДО /{command}/");
                if (request.HttpMethod == "POST") 
                {
                    ApiController controller;
                    switch (command)
                    {
                        case "robot":
                            var robotCommand = RobotCommand.DeserializeJson(requestString);
                            controller = new RobotController();
                            commandResult = (controller as RobotController).IndexAsync(robotCommand.CmdNumber, robotCommand.Value);
                            json = JsonConvert.SerializeObject(commandResult, Formatting.Indented);
                            break;
                        case "robotstate":
                            controller = new RobotStateController();
                            commandResult = (controller as RobotStateController).IndexAsync();
                            json = JsonConvert.SerializeObject(commandResult, Formatting.Indented);
                            break;
                        case "coffee":
                            var coffeeCommand = CoffeeCommand.DeserializeJson(requestString);
                            controller = new CoffeController();
                            commandResult = (controller as CoffeController).IndexAsync(coffeeCommand.Drink, coffeeCommand.Addons, coffeeCommand.Cup);
                            json = JsonConvert.SerializeObject(commandResult, Formatting.Indented);
                            break;
                        default:
                            res = new CommandResult() { Success = false };
                            json = JsonConvert.SerializeObject(res, Formatting.Indented);
                            break;
                    }
                    
                    responseBuffer = Encoding.UTF8.GetBytes(json);
                    response.StatusCode = 200;
                    response.ContentType = "application/json; charset=utf-8";
                    response.ContentLength64 = responseBuffer.Length;

                    response.AddHeader("Connection", "close");
                    response.AddHeader("Access-Control-Allow-Origin", "*");
                    response.AddHeader("Access-Control-Allow-Headers", "*");
                    response.AddHeader("Service", "Servio Coffee Maker Robot");

                    var output = response.OutputStream;
                    output.Write(responseBuffer, 0, responseBuffer.Length);
                    LoggerService.Write("HttpServer INFO", $"ВІДПРАВЛЕНА ВІДПОВІДЬ: /{json}/");
                    output.Close();

                    int bufLength = responseBuffer.Length;
                    if (responseBuffer.Length > 2048)
                        bufLength = 2048;
                    byte[] responseBufferLog = new byte[bufLength];
                    Buffer.BlockCopy(responseBuffer, 0, responseBufferLog, 0, bufLength);
                }
            }
            catch (Exception ex) 
            {
                LoggerService.Write("HttpServer ERROR", $"Помилка в методі ContextReady: {ex.Message}");
                res = new CommandResult() { Error = ex.Message };
            }
        }
        public static void SetErrorResponse(HttpListenerResponse _response, int _code, string _text)
        {
            _response.StatusCode = _code;
            var output = _response.OutputStream;
            if (!string.IsNullOrEmpty(_text))
            {
                var responseBuffer = Encoding.UTF8.GetBytes(_text);
                output.Write(responseBuffer, 0, responseBuffer.Length);
            }
            // You must close the output stream.
            output.Close();
        }

        public void Dispose()
        {
            StopListening();
        }
    }
}
