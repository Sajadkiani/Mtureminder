using MtuReminder.Configuration;
using MtuReminder.DataAccess;
using MtuReminder.Model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace MtuReminder
{
    /// <summary>
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public string  DbPath { get; set; }
        public TaskWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            try
            {
                //Create tray icon
                System.Windows.Forms.NotifyIcon Icn = new System.Windows.Forms.NotifyIcon();

                string RootPath = System.AppDomain.CurrentDomain.BaseDirectory;
                //using (EventLog eventLog = new EventLog("Application"))
                //{
                //    eventLog.Source = "Application";
                //    eventLog.WriteEntry(RootPath, EventLogEntryType.Error);
                //}
                string IcnPath = RootPath + "/Icons/if_2_300981.ico";
                Icn.Icon = new System.Drawing.Icon(IcnPath);
                Icn.Visible = true;
                Icn.MouseClick += new System.Windows.Forms.MouseEventHandler(TrayIcon_Click);
                Icn.ShowBalloonTip(1000, "Mtu Remeinder", "Mtu Reminder is active and added icon to system tray", System.Windows.Forms.ToolTipIcon.Info);

                //Create trayicon context menu 
                System.Windows.Forms.ContextMenu Cm = new System.Windows.Forms.ContextMenu();
                Cm.MenuItems.Add("New Task", new EventHandler(TrayIconContextMenu_NewTask_Click));
                Cm.MenuItems.Add("Exit", new EventHandler(TrayIconContextMenu_Exit_Click));

                //Paste context menu to tray icon
                Icn.ContextMenu = Cm;


                //Hide first form that app run in background 
                WindowState = WindowState.Minimized;
                Hide();

  
                //Configuration that moust be 
                StartupConfig.DateTimeConfiguration();
                StartupConfig.SetStartupApp();


                //Add DataBase Url To Setting File
                string StrPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+@"\MtuReminder\db/";
                if (!Directory.Exists(StrPath))
                {
                    Directory.CreateDirectory(StrPath);
                }
                //string StrPath = Directory.GetCurrentDirectory();
                //string FullPath = StrPath + @"\db";
                string FullPath = StrPath + @"\DB.xml";
                Properties.Settings.Default["DbPath"] = FullPath;
                Properties.Settings.Default.Save();
                DbPath = FullPath;

                //Timer Configuration 
                DispatcherTimer Timer = new DispatcherTimer();
                Timer.Tick += TimerHandler;
                Timer.Interval = new TimeSpan(0, 0, 5);
                Timer.Start();
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ee.Message, EventLogEntryType.Error);
                }
                MessageBox.Show("Error In Proram load :"+ee.Message);
            }
          
        }


        private void TrayIcon_Click(object sender,System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void TrayIconContextMenu_NewTask_Click(object sender,EventArgs e)
        {
            MainWindow MnWin = new MainWindow();
            MnWin.Show();
        }

        private void TrayIconContextMenu_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                DataRepo ObjRepo = new DataRepo();
                ObjRepo.CloseDatabase();
                System.Windows.Application.Current.Shutdown();
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry(ee.Message, EventLogEntryType.Error);
                }
                MessageBox.Show("Error In Proram Exit :" + ee.Message);

            }

        }

        public void ShowNewWindows(object sender, EventArgs e)
        {
            MainWindow MnWin = new MainWindow();
            MnWin.Show();
        }

        private void TimerHandler(object sender, EventArgs e)
        {
            try
            {
                DataRepo ObjRepo = new DataRepo();
                // var DbPath=Properties.Settings.Default["DbPath"].ToString();
                //search for exist task for done 
                List<TaskModel> ObjTaskModelList = ObjRepo.SearchForToDo(this.DbPath);

                foreach (var item in ObjTaskModelList)
                {
                    MainWindow ObjMainWindows = new MainWindow();
                    ObjMainWindows.TaskBodyTxt.Text = item.Content;
                    ObjMainWindows.TxtTaskId.Text = item.TaskId;

                    //ObjMainWindows.ShowDoneTimeLbl.Content= item.TimeDone;
                    DateTime dt = new DateTime();
                    DateTime.TryParse(item.TimeDone, out dt);
                    ObjMainWindows.dtPicker.Value = dt;
                    //ObjMainWindows.Test.Visibility = Visibility.Hidden;
                    //ObjMainWindows.ShowDoneTimeLbl.Visibility = Visibility.Visible;
                    ObjMainWindows.Left = Convert.ToDouble(item.Leftposition);
                    ObjMainWindows.Top = Convert.ToDouble(item.TopPosition);
                    ObjMainWindows.Show();
                }
            }
            catch (Exception ee)
            {
                EventLog windwosEventLog=new EventLog("Application");
                windwosEventLog.WriteEntry(ee.Message,EventLogEntryType.Error);
                MessageBox.Show("Error In Proram load :" + ee.Message);
            }


        }
    }
}
