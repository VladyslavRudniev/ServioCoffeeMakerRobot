using ServioCoffeMakerRobot;
using ServioCoffeMakerRobot.CommandResults;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using static System.Windows.Forms.LinkLabel;

namespace WebAPI.OWIN.Controllers
{
    public class RobotStateController : ApiController
    {
        RobotService _robotService;
        public RobotStateController()
        {
            _robotService = RobotService.getInstance();
        }
        public CommandResult IndexAsync()
        {
            return Task.Run(() => Index()).Result;
        }
        [HttpGet] 
        public CommandResult Index()
        {
            List<int> response = new List<int>();
            CommandResult commandResult = new CommandResult();
            commandResult.Success = true;

            if ((_robotService.ReadProperty("ETH_OUT_ROBOT_STATUS") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "робот офф-лайн.\n";
            }
            if (_robotService.ReadProperty("ETH_OUT_CYCLE_ON") == "TRUE")
            {
                commandResult.Success = false;
                commandResult.Error += "робот ще не закінчив цикл.\n";
            }
            if (_robotService.ReadProperty("ETH_OUT_HOME_CYCLE_ON") == "TRUE")
            {
                commandResult.Success = false;
                commandResult.Error += "робот ще виконує переміщення в домошнє положення.\n";
            }
            if ((_robotService.ReadProperty("ETH_OUT_IN_HOME") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "робот не в домошньому положенні.\n";
            }

            if ((_robotService.ReadProperty("ETH_OUT_2") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "кава машина офф-лайн.\n";
            }
            if ((_robotService.ReadProperty("ETH_OUT_1") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "не вистачає води.\n";
            }
            if ((_robotService.ReadProperty("ETH_OUT_3") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "не вистачає горячої води.\n";
            }
            if ((_robotService.ReadProperty("ETH_OUT_10") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "немає тиску в лінії.\n";
            }
            //response.Add(_robotService.ReadProperty("ETH_OUT_ROBOT_STATUS") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_CYCLE_ON") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_CYCLE_OK") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_CYCLE_FAULT") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_HOME_CYCLE_ON") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_IN_HOME") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_1") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_2") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_3") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_4") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_5") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_6") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_7") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_8") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_9") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_10") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_11") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_12") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_13") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_14") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_15") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_16") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_17") == "TRUE" ? 1 : 0);
            //response.Add(_robotService.ReadProperty("ETH_OUT_18") == "TRUE" ? 1 : 0);

            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_OUT_WATCHDOG")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_OUT_ERROR_CODE")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_TOTAL_CYCLE_COUNTER")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_TOTAL_CUPS_SMALL")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_TOTAL_CUPS_MEDIUM")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_TOTAL_CUPS_BIG")));
            //response.Add(Int32.Parse(_robotService.ReadProperty("ETH_CYCLE_TIME_COUNT")));

            return commandResult;
        }
    }
}
