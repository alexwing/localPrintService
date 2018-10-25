using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Printing;
using Microsoft.Win32;
using System.IO;
using System.Net;
using System.Diagnostics;
using LocalPrintService;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        // Or specify a specific name in the current dir
        IniFile MyIni = new IniFile("config.ini");
        public Form1()
        {

            InitializeComponent();

            LocalPrintServer ps = new LocalPrintServer();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                listBox1.Items.Add(printer);
            }
            // Get the default print queue
            // Create a new instance of the PrinterSettings class, which 
            // we will only use to fetch the default printer name
            System.Drawing.Printing.PrinterSettings newSettings = new System.Drawing.Printing.PrinterSettings();

            //get printer default from config.ini
            String defaultPrinterINI = MyIni.Read("Default", "printer");
            int index = -1;
            if (defaultPrinterINI != "")
            {
                 index = listBox1.FindString(defaultPrinterINI);
            }
       
            //MessageBox.Show("impresora: " + defaultPrinter +" index:" + index, "ERROR");

            //if no ini, get printer default system
            if (index <0)
                index = listBox1.FindString(newSettings.PrinterName);

            if (index > -1)
                listBox1.SetSelected(index, true);

            //    var result = MessageBox.Show(pq.ToString(), pq.ToString(),
            //                            MessageBoxButtons.YesNo,
            //                            MessageBoxIcon.Question);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            WebClient myWebClient = new WebClient();
            String fileName = "downloads/invoice.pdf";
            String myStringWebResource = textBox1.Text.ToString();

            myWebClient.DownloadFile(myStringWebResource, fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
            Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + Application.StartupPath);

            //Print(textBox1.Text.ToString(), listBox1.Items[listBox1.SelectedIndex].ToString());
            Print(fileName, listBox1.Items[listBox1.SelectedIndex].ToString());
        }

        public bool Print(string file, string printer)
        {
            if (!File.Exists(file))
            {
                MessageBox.Show("No se encuentra el fichero: " + file, "ERROR");
            }
            try
            {
                System.Diagnostics.Process.Start(
                   Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows\CurrentVersion" +
                        @"\App Paths\AcroRd32.exe").GetValue("").ToString(),
                   string.Format("/h /t \"{0}\" \"{1}\"", file, printer));
                //MessageBox.Show("matar acrobat: " + FindAndKillProcess("AcroRd32"), "ERROR");
                if (ChkCloseReader.Checked)
                {
                    StopAdobeReader();
                }
                return true;
            }
            catch
            {
                MessageBox.Show("Hubo un error al mandar el fichero PDF: " + file, "ERROR");
                return false;
            }
        }

        public void StopAdobeReader()
        {
            int sleepTime = (int) InputSleepTime.Value * 1000;

            Stopwatch sw = Stopwatch.StartNew();
            var delay = Task.Delay(sleepTime).ContinueWith(_ =>
            {
                FindAndKillProcess("AcroRd32");
                sw.Stop();
                return sw.ElapsedMilliseconds;
            });
        }

        public  bool FindAndKillProcess(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
               // Console.WriteLine(clsProcess.ProcessName.ToString());
                if (clsProcess.ProcessName.Contains(name))
                {
                  
                    //To know if it works
                    //MessageBox.Show(clsProcess);
                    clsProcess.Kill();

                    return true;
                }
            }
            //process not found, return false
            return false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MyIni.Write("Default", listBox1.Items[listBox1.SelectedIndex].ToString(), "printer");
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
               // notifyIcon1.ShowBalloonTip(3000);
                this.ShowInTaskbar = false;

            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = true;
        }

    }
}
