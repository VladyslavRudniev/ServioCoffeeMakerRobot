using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServioCoffeMakerRobot
{
    public partial class Form1 : Form
    {
        private HttpServer _terminalService;
        private RobotService _robotService;
        public Form1()
        {
            InitializeComponent();
            _robotService = RobotService.getInstance();
            var RobotHost = ConfigurationManager.AppSettings.Get("RobotIPAdress");
            var RobotPort = ConfigurationManager.AppSettings.Get("RobotPort");
            _robotService.ipAddress = RobotHost;
            _robotService.port = RobotPort;
            _robotService.Start();

            var ConnectionHost = ConfigurationManager.AppSettings.Get("ConnectionHost");
            var ConnectionPort = ConfigurationManager.AppSettings.Get("ConnectionPort");
            //_terminalService = WebApp.Start<Startup>($"{ConnectionHost}:{ConnectionPort}");
            _terminalService = new HttpServer(ConnectionHost, ConnectionPort);
            _terminalService.StartListening();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _terminalService.Dispose();
            _robotService.Dispose();
            Thread.Sleep(2000);
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            _terminalService.Dispose();
            _robotService.Dispose();
            _terminalService = null;
            _robotService = null;
            Thread.Sleep(2000);
        }
    }
}
