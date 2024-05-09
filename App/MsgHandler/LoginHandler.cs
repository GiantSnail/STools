using lightarc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MsgHandler
{
    public class LoginHandler : IMsgHandler
    {
        public object HandleMessage(byte[] data)
        {
            object obj = new object();
            return obj;
        }
    }
}
