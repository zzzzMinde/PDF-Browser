using System;
using System.Windows.Forms;
using PDFRenamer;

namespace WinFormsApp1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form1 = new Form1();

            // Check if there is a command-line argument (e.g., a file path) and pass it to the form
            if (args.Length > 0)
            {
                form1.OpenPdfFromArgument(args[0]);
            }

            Application.Run(new Form1());
        }
    }
}
