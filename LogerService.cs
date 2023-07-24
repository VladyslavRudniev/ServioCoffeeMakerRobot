using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServioCoffeMakerRobot
{
    internal class LogerService : IDisposable
    {
        private static LogerService  instance;

        private LogerService()
        { }
        public static LogerService getInstance()
        {
            if (instance == null) 
            {

            }
            return instance;
        }
    }
}
