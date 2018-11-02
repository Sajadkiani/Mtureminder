using MtuReminder.Model;
using MtuReminder.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace MtuReminder.DataAccess
{
    public class DataRepo
    {
        public XmlDocument db;
        public DataRepo()
        {
            db = LoadDb();
        }
        public  bool InsertRecord(TaskModel TaskRecord, string FullPath)
        {
            try
            {
                //create Db If not Exist
                if (!File.Exists(FullPath))
                {
                    XDocument ObjXDoc = new XDocument();
                    File.Create(FullPath);
                    var ObjAccessControll = File.GetAccessControl(FullPath);
                    ObjXDoc.Add(new XComment("Database File For Reminder"));
                    ObjXDoc.Add(new XElement("Tasks"));
                    ObjXDoc.Save(FullPath);
                }
                    XmlNode TasksNode = db.SelectSingleNode("/Tasks");
                    XmlElement ObjNewTask = db.CreateElement("Task");
                    ObjNewTask.SetAttribute("CreatedOn", TaskRecord.CreatedOn);
                    ObjNewTask.SetAttribute("IsComplete", false.ToString());
                    ObjNewTask.SetAttribute("DoneTime", TaskRecord.TimeDone);
                    ObjNewTask.SetAttribute("InShow", "false");
                    var TaskId = Guid.NewGuid();
                    ObjNewTask.SetAttribute("TaskId", TaskId.ToString());
                    ObjNewTask.InnerText=TaskRecord.Content;
                    XmlElement ObjPosition = db.CreateElement("Position");
                    ObjPosition.SetAttribute("Left", TaskRecord.Leftposition);
                    ObjPosition.SetAttribute("Top", TaskRecord.TopPosition);
                    ObjNewTask.AppendChild(ObjPosition);
                    TasksNode.AppendChild(ObjNewTask);
                    db.Save(FullPath);
            }
            catch (Exception e)
            {
                CloseDatabase();
                throw new Exception("Error In InsertRecord :"+e.Message);
            }

            return true;
        }


        public bool UpdateRecord(TaskModel TaskRecord, string FullPath)
        {
            try
            {

                    XmlNode TasksNode = db.SelectSingleNode("/Tasks");

                    foreach (XmlNode Tasknode in TasksNode.ChildNodes)
                    {

                        if (Guid.Parse(Tasknode.Attributes["TaskId"].Value)==Guid.Parse(TaskRecord.TaskId))
                        {
                            XmlElement ObjNewTask = db.CreateElement("Task");
                            ObjNewTask.SetAttribute("CreatedOn", TaskRecord.CreatedOn);
                            ObjNewTask.SetAttribute("DoneTime", TaskRecord.TimeDone);
                            ObjNewTask.SetAttribute("InShow", "false");
                            var TaskId = Guid.NewGuid();
                            ObjNewTask.SetAttribute("TaskId", TaskId.ToString());
                        ObjNewTask.SetAttribute("IsComplete", false.ToString());
                        ObjNewTask.InnerText = TaskRecord.Content;
                            XmlElement ObjPosition = db.CreateElement("Position");
                            ObjPosition.SetAttribute("Left", TaskRecord.Leftposition);
                            ObjPosition.SetAttribute("Top", TaskRecord.TopPosition);
                            ObjNewTask.AppendChild(ObjPosition);
                            TasksNode.ReplaceChild(ObjNewTask, Tasknode);
                        }
                    }

                    db.Save(FullPath);
            }
            catch (Exception e)
            {
                CloseDatabase();
                throw new Exception("Error in UpdateRecord :"+e.Message);
            }

            return true;
        }

        internal bool SetComplete(TaskModel TaskRecord)
        {
            XmlNode TasksNode = db.SelectSingleNode("/Tasks");
            try
            {
                foreach (XmlNode Tasknode in TasksNode.ChildNodes)
                {
                    if (Guid.Parse(Tasknode.Attributes["TaskId"].Value) == Guid.Parse(TaskRecord.TaskId))
                    {
                        TasksNode.RemoveChild(Tasknode);
                    }
                }

                db.Save(Settings.Default.DbPath);
                return true;
            }
            catch (Exception ee)
            {
                throw new Exception("Error in SetForRunning :"+ee.Message);
            }
   
        }

        internal  void SetHiddingTask(string TaskId)
        {
            try
            {
                XmlNode TasksNode = db.SelectSingleNode("/Tasks");

                foreach (XmlNode item in TasksNode.ChildNodes)
                {
                    if (item.Attributes["TaskId"].Value == TaskId)
                    {
                        item.Attributes["InShow"].Value = "false";
                        db.Save(Settings.Default.DbPath);
                    }
                }
            }
            catch (Exception ee)
            {
                throw new Exception("Error in SetHiddingTask :"+ee.Message);
            }
     
        }



        internal  void SetShowingTask(string TaskId)
        {
            try
            {
                XmlNode TasksNode = db.SelectSingleNode("/Tasks");

                foreach (XmlNode item in TasksNode.ChildNodes)
                {
                    if (item.Attributes["TaskId"].Value == TaskId)
                    {
                        item.Attributes["InShow"].Value = "true";
                        db.Save(Settings.Default.DbPath);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in SetShowingTask :"+e.Message);
            }

        }

        internal void SaveWindowsPosition(double leftPosition, double topPosition,string TaskId)
        {
            try
            {
                XmlNode ObjTask=db.SelectSingleNode("//Task[@TaskId='" + TaskId + "']");
                    XmlNode OldPositionChild = ObjTask.LastChild;
                    XmlElement ObjNewPosition = db.CreateElement("Position");
                    ObjNewPosition.SetAttribute("Left", leftPosition.ToString());
                    ObjNewPosition.SetAttribute("Top", topPosition.ToString());
                    ObjTask.ReplaceChild(ObjNewPosition, OldPositionChild);
                    db.Save(Settings.Default.DbPath);
              
            }
            catch (Exception ee)
            {
                CloseDatabase();
                throw new Exception("Error in SaveWindowsPosition :"+ee.Message);
            }
        }

        internal  List<TaskModel> SearchForToDo(string DbPath)
        {
            try
            {

                List<TaskModel> ObjTaskModelList = new List<TaskModel>();
                TaskModel ObjTaskModel;
                XmlNode TasksNode = db.SelectSingleNode("/Tasks");
                if (TasksNode !=null)
                {
                    foreach (XmlNode Tasknode in TasksNode.ChildNodes)
                    {
                        if (Convert.ToBoolean(Tasknode.Attributes["InShow"].Value) == false)
                        {
                            ObjTaskModel = new TaskModel();
                            ObjTaskModel.TimeDone = Tasknode.Attributes["DoneTime"].Value;
                            ObjTaskModel.Content = Tasknode.InnerText;
                            ObjTaskModel.TaskId = Tasknode.Attributes["TaskId"].Value;

                            XmlNode ObjPosition = Tasknode.LastChild;
                            ObjTaskModel.Leftposition = ObjPosition.Attributes["Left"].Value;
                            ObjTaskModel.TopPosition = ObjPosition.Attributes["Top"].Value;
                            if (CheckForRunning(ObjTaskModel))
                            {
                                ObjTaskModelList.Add(ObjTaskModel);
                                SetShowingTask(ObjTaskModel.TaskId);
                            }
                        }
                    }
                    db.Save(DbPath);
                }
        
                return ObjTaskModelList;
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in searchfortodo :"+ ee.Message, EventLogEntryType.Error);
                }
                CloseDatabase();
                return null;
            }
        }

        internal void CloseDatabase()
        {
            try
            {
                XmlNode TasksNode = db.SelectSingleNode("/Tasks");
                if (TasksNode != null)
                {
                    foreach (XmlNode Tasknode in TasksNode.ChildNodes)
                    {
                        if (Convert.ToBoolean(Tasknode.Attributes["InShow"].Value) == true)
                        {
                            SetHiddingTask(Tasknode.Attributes["TaskId"].Value);
                        }
                    }
                    db.Save(Settings.Default.DbPath);
                }
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in Closedatabase :" + ee.Message, EventLogEntryType.Error);
                }
                throw new Exception(ee.Message);
            }
        }


        /// <summary>
        /// Check task is in running Time
        /// </summary>
        /// <param name="objTaskModel"></param>
        /// <returns></returns>
        internal  bool CheckForRunning(TaskModel objTaskModel)
        {

            DateTime DoneTimeDt = new DateTime();
            try
            {
                DateTime.TryParse(objTaskModel.TimeDone, out DoneTimeDt);
                var ObjNow =DateTime.UtcNow;
                TimeSpan ObjTs = new TimeSpan(0, Settings.Default.Timelimit, 0);
                ObjNow = ObjNow.Add(ObjTs);
                if (DateTime.Compare(DoneTimeDt, ObjNow) < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                CloseDatabase();
                throw new Exception("Error in CheckForRunning :"+" "+objTaskModel.Content);
            }

        }


        internal  XmlDocument LoadDb()
        {
            XmlDocument database = new XmlDocument();
            try
            {
                XDocument ObjXDoc = new XDocument();
                string StrPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\MtuReminder\db\";
                if (!Directory.Exists(StrPath))
                {
                    Directory.CreateDirectory(StrPath);
                }
                
                if (!File.Exists(Settings.Default.DbPath))
                {
                    using (File.Create(Settings.Default.DbPath))
                    {
                    }
                    ObjXDoc.Add(new XComment("Database File For Reminder"));
                    ObjXDoc.Add(new XElement("Tasks"));
                    ObjXDoc.Save(Settings.Default.DbPath);
                }
          
                database.Load(Settings.Default.DbPath);
            }
            catch (Exception ee)
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "Application";
                    eventLog.WriteEntry("Error in Closedatabase" + ee.Message, EventLogEntryType.Error);
                }
                throw new Exception("Error in Load Database :"+ee.Message);
            }
      
            return database;
        }

    }
}
