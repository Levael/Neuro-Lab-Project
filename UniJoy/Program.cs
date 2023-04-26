using System;
using System.Windows.Forms;

namespace UniJoy
{
    static class Program
    {
        /// THE MAIN ENTRY POINT FOR THE APPLICATION
        
        // as I understand it's for Windows forms support (yeh, legacy from MSDOS), otherwise doesn't work
        [STAThread]
        static void Main()
        {
            ExcelProtocolConfigFileLoader excelLoader = new ExcelProtocolConfigFileLoader();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            WinApi.TimeBeginPeriod(1);
            Application.Run(new GuiInterface(ref excelLoader));
            WinApi.TimeEndPeriod(1);
        }
    }
}
