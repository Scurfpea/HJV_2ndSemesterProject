﻿using HJV_2ndSemesterProject.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJV_2ndSemesterProject.Data;

namespace HJV_2ndSemesterProject.ViewModels
{
    //This class handles CRUD-functions for the LogEntry class.
    public class LogEntryRepository
    {

        public LogEntryRepository() { }

        public void CreateLogEntry(LogEntry entry)
        {
            int newID;
            DataAccess.NewConn();
            using (DataAccess.conn)
            {
                using (SqlCommand cmd = new SqlCommand("sp_CreateLogEntry", DataAccess.conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Role", (int)entry.Role);
                    cmd.Parameters.AddWithValue("@NumberOfHours", entry.NumberofHours);
                    cmd.Parameters.AddWithValue("@Comment", entry.Comment);
                    cmd.Parameters.AddWithValue("@MA_Number", entry.MA_Number);
                    cmd.Parameters.AddWithValue("@SailingID", entry.SailingID);

                    DataAccess.conn.Open();
                    newID= Convert.ToInt32(cmd.ExecuteScalar());
                }

                foreach (Models.Task t in entry.Tasks)
                {
                    LinkTaskToLogEntry(newID, t.TaskID, DataAccess.conn);
                }
            }
        }

        public void UpdateLogEntry( LogEntry updated )
        {
            DataAccess.NewConn();
            using (DataAccess.conn)
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateLogEntry", DataAccess.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LogID", updated.Id);
                    cmd.Parameters.AddWithValue("@Role", (int)updated.Role);
                    cmd.Parameters.AddWithValue("@NumberOfHours", updated.NumberofHours);
                    cmd.Parameters.AddWithValue("@Comment", updated.Comment);
                    cmd.Parameters.AddWithValue("@MA_Number", updated.MA_Number);
                    cmd.Parameters.AddWithValue("@SailingID", updated.SailingID);
                    DataAccess.conn.Open();
                    cmd.ExecuteNonQuery();
                }
                foreach (Models.Task t in updated.Tasks)
                {
                    LinkTaskToLogEntry(updated.Id, t.TaskID, DataAccess.conn);
                }
            }
        
        }

        public void DeleteLogEntry(int id)
        {
            DataAccess.NewConn();
            using (DataAccess.conn)
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteLogEntry", DataAccess.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LogID", id);
                    DataAccess.conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Gets all logs for one volunteer.
        public List<LogEntry> GetLogsByMA (string MA_NUmber)
        {
            List<LogEntry> result= new();
            DataAccess.NewConn();
            using (DataAccess.conn)
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetLogEntriesByMa_Number", DataAccess.conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MA_NUmber", MA_NUmber);
                    DataAccess.conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            LogEntry entry = new((Role)(int)reader["Role"], (double)reader["NumberOfHours"],
                                reader["Comment"].ToString(), MA_NUmber, (int)reader["SailingID"], null,
                                (int)reader["LogID"]);

                            result.Add(entry);
                        }
                    }
                }
                foreach (LogEntry log in result)
                {
                    log.Tasks.AddRange(GetLogTasks(log.Id, DataAccess.conn));
                }
                return result;
            }
        }
        // Adds rows to the  LOG_ENTRY_TASK linking table in the database.
        private void LinkTaskToLogEntry(int logId, int taskId,SqlConnection connection) 
        {
            using (SqlCommand cmd1= new("sp_CreateLogEntryTask",connection))
            {
                cmd1.CommandType=CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@LogID",logId);
                cmd1.Parameters.AddWithValue("@TaskID", taskId);
                cmd1.ExecuteNonQuery();
            }      
        }

        //Gets all tasks associated with a certain log entry.
        private List<Models.Task> GetLogTasks(int LogID, SqlConnection connection)
        {
            List<Models.Task> result = new();
            using (SqlCommand cmd1 = new("sp_GetTasksByLog", connection))
            {
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@LogID", LogID);
                using (SqlDataReader reader1 = cmd1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        Models.Task t = new(reader1["TaskType"].ToString(), (int)reader1["TaskID"]);
                        result.Add(t);
                    }
                    return result;
                }
            }

        }
    }
}
