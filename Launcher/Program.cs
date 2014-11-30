using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LOG.Launcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static Main main;
        public static AddServer addserver;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            main = new Main();
            addserver = new AddServer();

            Application.Run(main);
        }
    }
}
