﻿using HJV_2ndSemesterProject.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJV_2ndSemesterProject.Data;

namespace HJV_2ndSemesterProject.ViewModels
{

    //This class supplies a list of all the waters from the database. Used by LogEntryViewModel for databinding a listbox.
    public class WatersRepository
    {
        public ObservableCollection<Waters> Waters { get;}
        public WatersRepository() 
        {
            Waters = new();
            GetAllWaters();
        }

        private void GetAllWaters()
        {
            DataAccess.NewConn();
            using (DataAccess.conn)
            {
                DataAccess.conn.Open();
                using (SqlCommand cmd = new SqlCommand("Select * from WATERS", DataAccess.conn))
                {
                    using SqlDataReader reader = cmd.ExecuteReader();
                    {
                        while (reader.Read())
                        {
                            Waters w = new(reader["WatersName"].ToString(),(int) reader["WatersType"]);
                            Waters.Add(w);
                        }
                    }
                }

            }
        }
    }
}
