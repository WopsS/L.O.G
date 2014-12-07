﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LOG.Launcher
{
    static class Program
    {
        internal static Main main;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main = new Main();

            Application.Run(main);
        }
    }
}
