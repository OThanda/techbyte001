using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBytes.Web.Loggers
{
    interface IRecorder
    {
        void Write(string text);
    }
}
