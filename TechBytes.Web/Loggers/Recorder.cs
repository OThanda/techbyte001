using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace TechBytes.Web.Loggers
{
    public class Recorder : IRecorder
    {
        public void Write(string text)
        {
            Debug.WriteLine(text);
        }
    }
}