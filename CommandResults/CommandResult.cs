using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServioCoffeMakerRobot.CommandResults
{
    public interface ICommandResult
    {
    }

    [Serializable]
    [DataContract]
    public class CommandResultBasic : ICommandResult
    {
    }

    [Serializable]
    [DataContract]
    public class CommandResult : CommandResultBasic
    {

        [DataMember(Order = 1)]
        public string Error = "";

        private Boolean success;

        [DataMember(Order = 2)]
        public Boolean Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
            }
        }

        public byte[] Serialize()
        {
            byte[] Result = null;
            try
            {
                var stream = new MemoryStream();
                var formatter = new BinaryFormatter();

                formatter.Serialize(stream, this);
                Result = stream.ToArray();
                stream.Close();
            }
            catch (Exception ex)
            {
            }
            return Result;
        }

        internal byte[] SerializeJson()
        {
            throw new NotImplementedException();
        }
    }
}
