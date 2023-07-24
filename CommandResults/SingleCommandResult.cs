using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServioCoffeMakerRobot.CommandResults
{
    [DataContract]
    public class SingleCommandResult : CommandResult
    {
        [DataMember(Order = 3, Name = "value")]
        public int Value { get; set; }
    }
}
