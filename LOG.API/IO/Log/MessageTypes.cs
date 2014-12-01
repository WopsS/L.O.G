using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOG.API.IO.Log
{
    /// <summary>
    /// Type for the log message.
    /// </summary>
    public enum LOGMessageTypes : int
    {
        Info = 0,
        Debug = 1,
        Warning = 2,
        Error = 3
    }
}
