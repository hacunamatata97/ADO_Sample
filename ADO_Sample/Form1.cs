using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration; // Reading, connectionString from app.config
using System.Data.SqlClient; // connection, Command, DataReader

namespace ADO_Sample {
    public partial class Form1 : Form {

        string connectionString = ConfigurationManager.
            ConnectionStrings["ProjectDB"].ConnectionString;
        public Form1() {
            InitializeComponent();
        }

        private void button2_Click(object sender , EventArgs e) {
            //output data from DepartmentTBL to listview
            try {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "select * from DepartmentTBL";
                SqlCommand cmd = new SqlCommand(query , conn);
                SqlDataReader reader = cmd.ExecuteReader();
                //ready to output
                listView1.Items.Clear(); // clear old data
                int row = 0;
                while(reader.Read()) {
                    int id = reader.GetInt32(0);
                    string name = reader["DepartmentName"].ToString();
                    listView1.Items.Add(id.ToString());
                    listView1.Items[row].SubItems.Add(name);
                    row++;
                }
                conn.Close();
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender , EventArgs e) {
            //output data from DepartmentTBL to listview
            try {
                SqlConnection conn = new SqlConnection(connectionString);
                conn.Open();
                string query = "select count(*) from DepartmentTBL";
                SqlCommand cmd = new SqlCommand(query , conn);
                int count = (int)cmd.ExecuteScalar();
                conn.Close();
                MessageBox.Show("Number of Department(s): " + count);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender , EventArgs e) {

            //validation
            string id = depID.Text;
            string name = depName.Text;
            //Id must be a whole number
            int deptID;
            if(int.TryParse(id , out deptID) == false || string.IsNullOrEmpty(name)) {
                MessageBox.Show("ID must be a number and name cannot be empty");
                return;
            }
            //Id cannot be repeated
            string query = "select * from DepartmentTBL where DepartmentID = @DepartmentID";
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand(query , conn);
            conn.Open();
            cmd.Parameters.AddWithValue("@DepartmentID" , id);
            SqlDataReader reader = cmd.ExecuteReader();
            if(reader.Read()) {
                MessageBox.Show("ID cannot be repeated");
                conn.Close();
                return;
            }
            //insertion
            query = "Insert into DepartmentTBL values(@DeptID,@DeptName)";
            cmd = new SqlCommand(query , conn);
            // close the previous cmd and conn
            conn.Close();
            if(conn.State == ConnectionState.Closed) conn.Open();
            cmd.Parameters.AddWithValue("@DeptID" , id);
            cmd.Parameters.AddWithValue("@DeptName" , name);
            int r = cmd.ExecuteNonQuery();
            if(r != 0) {
                MessageBox.Show("Department " + name + " has been added");
            }
            conn.Close();

            //reload Departments to listview
            button2_Click(null , null);
        }
    }
}
