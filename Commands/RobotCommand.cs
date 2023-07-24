using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace ServioCoffeMakerRobot.Commands
{
    public class RobotCommand
    {
        [JsonProperty(PropertyName = "CmdNumber")]
        public int CmdNumber { get; set; }
        [JsonProperty(PropertyName = "Value")]
        public string Value { get; set; }

        public static RobotCommand DeserializeJson(string jsonString) 
        {
            RobotCommand command = null;
            command = JsonConvert.DeserializeObject<RobotCommand>(jsonString);

            if (command == null)
                command = new RobotCommand();

            return command;
        }
    }
}
