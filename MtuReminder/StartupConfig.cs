using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MtuReminder.Configuration
{
    public static class StartupConfig
    {


        public static void DateTimeConfiguration()
        {
            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en");
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en");
        }


        /// <summary>
        /// Add app in startup in registery
        /// </summary>
        public static void SetStartupApp()
        {
            RegistryKey CurrentUserKey = Registry.CurrentUser;
            RegistryKey StartupKey;

            StartupKey=CurrentUserKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run",true);
            if (StartupKey.GetValue("MtuReminder") == null)
            {
#if DEBUG
                var ProgDir = Environment.CurrentDirectory+ @"\MtuReminder.exe";
                StartupKey.SetValue("MtuReminder", ProgDir);
#endif
            }
        }
    }
}
