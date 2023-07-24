using ServioCoffeMakerRobot.CommandResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace ServioCoffeMakerRobot.Controllers
{
    public class RobotController : ApiController
    {
        RobotService _robotService;
        public RobotController()
        {
            _robotService = RobotService.getInstance();
        }
        public CommandResult IndexAsync(int cmdNumber, string value)
        {
            return Task.Run(() => Index(cmdNumber, value)).Result;
        }
        [HttpGet]
        public CommandResult Index(int cmdNumber, string value)
        {
            value = value.ToUpper();
            bool result;
            CommandResult commandResult = new CommandResult();
            commandResult.Success = true;

            switch (cmdNumber)
            {
                case 0:
                    if (value == "STOP")
                    {
                        try
                        {
                            _robotService.Dispose();
                        }
                        catch (Exception ex)
                        {
                            commandResult.Success = false;
                            return commandResult;
                        }
                    }
                    else if (value == "START")
                    {
                        try
                        {
                            _robotService.Start();
                        }
                        catch (Exception ex)
                        {
                            commandResult.Success = false;
                            return commandResult;
                        }
                    }
                    else
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 1:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_START", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    else
                    {
                        while (true)
                        {
                            if (_robotService.ReadProperty("ETH_OUT_CYCLE_ON") == "FALSE")
                                break;
                            Thread.Sleep(3000);
                        }
                        if (_robotService.ReadProperty("ETH_OUT_CYCLE_OK") == "TRUE")
                        {
                            commandResult.Success = true;
                            return commandResult;
                        }
                        else
                        {
                            commandResult.Success = false;
                            return commandResult;
                        }
                    }
                    break;
                case 2:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_STOP", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 3:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_RESET", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 4:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_HOME", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 5:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_1", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 6:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_2", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 7:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_3", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                case 8:
                    if (value != "FALSE" && value != "TRUE")
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    result = _robotService.WriteProperty("ETH_IN_CMD_4", value);
                    if (value == "TRUE" && result == true)
                    {
                        Thread.Sleep(1000);
                        result = _robotService.WriteProperty("ETH_IN_CMD_START", "FALSE");
                    }
                    if (!result)
                    {
                        commandResult.Success = false;
                        return commandResult;
                    }
                    break;
                default:
                    commandResult.Success = false;
                    return commandResult;
                    break;
            }

            return commandResult;
        }
    }
}
