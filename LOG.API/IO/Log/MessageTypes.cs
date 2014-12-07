using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.IO.Log
{
    /// <summary>
    /// Types of log messages.
    /// </summary>
    public enum LOGMessageTypes : int
    {
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3
    }
}
