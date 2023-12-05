﻿using HJV_2ndSemesterProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJV_2ndSemesterProject.ViewModels
{
    public class TaskRepository
    {
        public ObservableCollection <Models.Task> tasks { get; set; }
        public void GetTasks()
        {
            DataAccess.NewConn();
            using (DataAccess.conn)
            using (SqlCommand cmd = new SqlCommand("Select TaskType from Task", DataAccess.conn))
            {
                    DataAccess.conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Models.Task t = new(reader["TaskType"].ToString());
                            tasks.Add(t);
                        }
                    }
            }

            
        }
        public TaskRepository()
        {
            tasks = new ObservableCollection<Models.Task>();
            GetTasks();
    }   }           
}