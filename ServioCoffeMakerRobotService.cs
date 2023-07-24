using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using WebAPI.OWIN;

namespace ServioCoffeMakerRobot
{
    public partial class ServioCoffeMakerRobotService : ServiceBase
    {
        private HttpServer _terminalService;
        private RobotService _robotService;
        public ServioCoffeMakerRobotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _robotService = RobotService.getInstance();
            var RobotHost = ConfigurationManager.AppSettings.Get("RobotIPAdress");
            var RobotPort = ConfigurationManager.AppSettings.Get("RobotPort");
            _robotService.ipAddress = RobotHost;
            _robotService.port = RobotPort;
            _robotService.Start();

            var ConnectionHost = ConfigurationManager.AppSettings.Get("ConnectionHost");
            var ConnectionPort = ConfigurationManager.AppSettings.Get("ConnectionPort");
            _terminalService = new HttpServer(ConnectionHost, ConnectionPort);
            _terminalService.StartListening();
        }

        protected override void OnStop()
        {
            _terminalService.Dispose();
            _robotService.Dispose();
            _terminalService = null;
            _robotService = null;
            Thread.Sleep(2000);
        }

    }
}
