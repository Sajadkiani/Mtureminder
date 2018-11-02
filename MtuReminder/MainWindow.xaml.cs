using MD.PersianDateTime;
using System;
using System.Windows;
using System.Windows.Input;
using MtuReminder.Model;
using MtuReminder.DataAccess;
using MtuReminder.Util;
using System.Diagnostics;
using MtuReminder.Properties;

namespace MtuReminder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }


        private void MainFormLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DataRepo ObjRepo = new DataRepo();
            ObjRepo.SetHiddingTask(this.TxtTaskId.Text);
            this.Close();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            //Task Tsk = Task.Run(()=> { MainWindow MnWin = new MainWindow(); });
            MainWindow MnWin = new MainWindow();
            MnWin.Show();
        }



        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            DataRepo ObjRepo = new DataRepo();
            try
            {

                if (dtPicker.Text == null  || dtPicker.Text==string.Empty || TaskBodyTxt.Text == string.Empty)
                {
                    WpfMessageBox message = new WpfMessageBox();
                    message.Top = this.Top;
                    message.Left = this.Left;
                    message.MessageTextLbl.Content = ".لطفا فیلد های اطلاعات  را به صورت کامل وارد کنید";
                    message.Show();
                }
                else
                {
                    if (DateTimeConfig.Isdate(dtPicker.Text))
                    {
                        string UtcDateTimeDone = DateTimeConfig.ConvertPersianToUtc(dtPicker.Text);
                        TaskModel ObjTaskModel = new TaskModel()
                        {
                            TimeDone = DateTimeConfig.ConvertPersianToUtc(dtPicker.Text),
                            TaskId = TxtTaskId.Text,
                            Content = TaskBodyTxt.Text,
                            CreatedOn = DateTimeConfig.ConvertPersianToUtc(new PersianDateTime(DateTime.Now).ToLongDateTimeString()),
                            Leftposition = this.Left.ToString(),
                            TopPosition = this.Top.ToString()
                        };

                        bool resault;
                        if (ObjTaskModel.TaskId == "")
                        {
                            resault = ObjRepo.InsertRecord(ObjTaskModel, Settings.Default.DbPath);
                        }
                        else
                        {
                            resault = ObjRepo.UpdateRecord(ObjTaskModel, Settings.Default.DbPath);
                        }
                        if (resault)
                        {
                            this.Close();
                        }
                    }
                    else {
                        MessageBox.Show("فرمت تاریخ صحیح نمی باشد");
                    }
               
                }
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in Apply :"+ee.Message, EventLogEntryType.Error);
                }
                MessageBox.Show(this,"Error In Apply :" + ee.Message);
            }


        }



        private void onMouseDown(object sender, DragEventArgs e)
        {
       
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            DataRepo ObjRepo = new DataRepo();
            try
            {
                this.DragMove();
                double leftPosition = this.Left;
                double TopPosition = this.Top;
                if (TxtTaskId.Text != "")
                {
                    ObjRepo.SaveWindowsPosition(leftPosition, TopPosition, TxtTaskId.Text);
                }

            }
            catch (Exception ee)
            {
                ObjRepo.CloseDatabase();
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in OnMouseDown :" + ee.Message, EventLogEntryType.Error);
                    MessageBox.Show(this,"Error In onMouseDown :" + ee.Message);
                }
            }
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRepo ObjRepo = new DataRepo();
                TaskModel task = new TaskModel()
                {
                    TimeDone = DateTimeConfig.ConvertPersianToUtc(dtPicker.Text),
                    TaskId = TxtTaskId.Text,
                    Content = TaskBodyTxt.Text,
                    CreatedOn = DateTimeConfig.ConvertPersianToUtc(new PersianDateTime(DateTime.Now).ToLongDateTimeString()),
                    Leftposition = this.Left.ToString(),
                    TopPosition = this.Top.ToString()
                };

                if (ObjRepo.SetComplete(task))
                {
                    this.Close();
                }
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in Complete :" + ee.Message, EventLogEntryType.Error);
                    MessageBox.Show(this,"Error In Complete :" + ee.Message);
                }
            }
     
           
        }
    }
}
