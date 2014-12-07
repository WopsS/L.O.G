using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LOG.API.IO
{
    public class Directories
    {
        /// <summary>
        /// Check if a specific directory exist and create it if it doesn't exist.
        /// </summary>
        /// <param name="Path">Path to directory.</param>
        public static void CheckDirectory(string Path)
        {
            if (Directory.Exists(Path) == false)
                Directory.CreateDirectory(Path);
        }

        /// <summary>
        /// Create a specific directory.
        /// </summary>
        /// <param name="Path">Path to directory.</param>
        public static void CreateDirectory(string Path)
        {
            Directory.CreateDirectory(Path);
        }
    }
}
