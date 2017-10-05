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
    public partial class Form2 : Form {

        string connectionString = ConfigurationManager.
    ConnectionStrings["ProjectDB"].ConnectionString;

        DataSet ds;

        public Form2() {
            InitializeComponent();
            //load all departments to dropdownlist
            string query = "select * from DepartmentTBL;"
                + "select * from EmployeeTBL";
            SqlDataAdapter da = new SqlDataAdapter(query , connectionString);
            ds = new DataSet();
            da.Fill(ds);
            //ref to table DepartmentTBL
            DataTable departmentTBL = ds.Tables[0];
            DataTable employeeTBL = ds.Tables[1];
            //bind departmentTBL to dropdownlist and employeeTBL to datagridview
            comboBox1.DataSource = departmentTBL;
            comboBox1.DisplayMember = "DepartmentName";
            comboBox1.ValueMember = "DepartmentID";
            dataGridView1.DataSource = employeeTBL;
        }

        private void comboBox1_SelectedIndexChanged(object sender , EventArgs e) {

            // output information of all employees of selected department
            // get selected department id
            string deptID = comboBox1.SelectedValue.ToString();
            DataTable employeeTBL = ds.Tables[1];
            DataRow[] dr = employeeTBL.Select("DepartmentId = " + deptID);
            //if department id has at least an employee
            if(dr != null && dr.Length > 0) {
                dataGridView1.DataSource = dr.CopyToDataTable();
            } else {
                MessageBox.Show("No employee in the department " + deptID);
            }
        }
    }
}
