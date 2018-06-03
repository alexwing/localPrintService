using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Printing;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using WindowsFormsApp1;

namespace LocalPrintService
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (IsAdobeReader())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("Necesita tener instalado Acrobat Reader para funcionar. \nPuede descargarlo de https://get.adobe.com/es/reader/", "No se puede ejecutar");
            }
        }



        static bool IsAdobeReader()
        {
            RegistryKey software = Registry.LocalMachine.OpenSubKey("Software");
            bool status = false;

            if (software != null)
            {
                RegistryKey adobe = null;


                // Try to get 64bit versions of adobe
                if (Environment.Is64BitOperatingSystem)
                {
                    RegistryKey software64 = software.OpenSubKey("Wow6432Node");

                    if (software64 != null)
                        adobe = software64.OpenSubKey("Adobe");
                }

                // If a 64bit version is not installed, try to get a 32bit version
                if (adobe == null)
                    adobe = software.OpenSubKey("Adobe");

                // If no 64bit or 32bit version can be found, chances are adobe reader is not installed.
                if (adobe != null)
                {
                    RegistryKey acroRead = adobe.OpenSubKey("Acrobat Reader");

                    if (acroRead != null)
                    {
                        string[] acroReadVersions = acroRead.GetSubKeyNames();
                        Console.WriteLine("The following version(s) of Acrobat Reader are installed: ");

                        foreach (string versionNumber in acroReadVersions)
                        {
                            Console.WriteLine(versionNumber);
                        }
                        status = true;
                    }
                    else
                    {
                        Console.WriteLine("Adobe reader is not installed!");
                        status = false;
                    }
                }
                else
                {
                    Console.WriteLine("Adobe reader is not installed!");
                    status = false;
                }
            }
            return status;
        }
    }
}
