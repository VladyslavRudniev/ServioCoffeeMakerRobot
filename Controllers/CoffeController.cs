using ServioCoffeMakerRobot;
using ServioCoffeMakerRobot.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebAPI.OWIN.Controllers
{
    public class CoffeController : ApiController
    {
        RobotService _robotService;
        public CoffeController()
        {
            _robotService = RobotService.getInstance();
        }
        public CommandResult IndexAsync(int drink, int addons, int cup)
        {
            return Task.Run(() => Index(drink, addons, cup)).Result;
        }
        [HttpGet]
        public CommandResult Index(int drink, int addons, int cup)
        {
            bool result;
            CommandResult commandResult = new CommandResult();
            commandResult.Success = true;

            if (drink < 1 || drink > 9)
            {
                commandResult.Success = false;
                commandResult.Error += "невірно вибраний напій, діапазон від 1 до 9.\n";
                return commandResult;
            }
            if (cup < 1 || cup > 3)
            {
                commandResult.Success = false;
                commandResult.Error += "невірно вибраний тип стаканчика, діапазон від 1 до 3.\n";
                return commandResult;
            }

            if (!_robotService.WriteProperty("ETH_IN_DRINK_TYPE", drink.ToString()))
            {
                commandResult.Success = false;
                commandResult.Error += "не вдалося встановити тип напою.\n";
                return commandResult;
            }
            if (!_robotService.WriteProperty("ETH_IN_ADDONS_TYPE", addons.ToString()))
            {
                commandResult.Success = false;
                commandResult.Error += "не вдалося встановити тип сиропу.\n";
                return commandResult;
            }
            if (!_robotService.WriteProperty("ETH_IN_CUP_SIZE", cup.ToString()))
            {
                commandResult.Success = false;
                commandResult.Error += "не вдалося встановити тип стаканчика.\n";
                return commandResult;
            }

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

            if (drink != 6 && drink != 7)
            {
                if ((_robotService.ReadProperty("ETH_OUT_4") == "FALSE"))
                {
                    commandResult.Success = false;
                    commandResult.Error += "мельниця офф-лайн.\n";
                }
                if ((_robotService.ReadProperty("ETH_OUT_5") == "FALSE"))
                {
                    commandResult.Success = false;
                    commandResult.Error += "немає зерен для приготування кави.\n";
                }
            }

            if (drink != 1 && drink != 8 && drink != 6)
            {
                if ((_robotService.ReadProperty("ETH_OUT_6") == "FALSE"))
                {
                    commandResult.Success = false;
                    commandResult.Error += "мілкер офф-лайн.\n";
                }
                if ((_robotService.ReadProperty("ETH_OUT_7") == "FALSE"))
                {
                    commandResult.Success = false;
                    commandResult.Error += "немає молока.\n";
                }
            }

            switch (cup)
            {
                case 1:
                    if ((_robotService.ReadProperty("ETH_OUT_8") == "FALSE"))
                    {
                        commandResult.Success = false;
                        commandResult.Error += "немає маленьких стаканчиків.\n";
                    }
                    break;
                case 2:
                    if ((_robotService.ReadProperty("ETH_OUT_9") == "FALSE"))
                    {
                        commandResult.Success = false;
                        commandResult.Error += "немає великіх стаканчиків.\n";
                    }
                    break;
                default:
                    break;
            }

            if ((_robotService.ReadProperty("ETH_OUT_10") == "FALSE"))
            {
                commandResult.Success = false;
                commandResult.Error += "немає тиску в лінії.\n";
            }

            return commandResult;
        }
    }
}
