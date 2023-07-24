using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServioCoffeMakerRobot.Commands
{
    public class CoffeeCommand
    {
        [JsonProperty(PropertyName = "Drink")]
        public int Drink { get; set; }
        [JsonProperty(PropertyName = "Addons")]
        public int Addons { get; set; }
        [JsonProperty(PropertyName = "Cup")]
        public int Cup { get; set; }

        public static CoffeeCommand DeserializeJson(string jsonString)
        {
            CoffeeCommand command = null;
            command = JsonConvert.DeserializeObject<CoffeeCommand>(jsonString);

            if (command == null)
                command = new CoffeeCommand();

            return command;
        }
    }
}
