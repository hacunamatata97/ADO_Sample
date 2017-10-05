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
    public partial class Form3 : Form {

        string connectionString = ConfigurationManager.
ConnectionStrings["ProjectDB"].ConnectionString;

        DataTable departmentTBL;
        SqlDataAdapter da;

        public Form3() {
            InitializeComponent();

            //load all department to dataGridView
            string query = "select * from DepartmentTBL";
            da = new SqlDataAdapter(query , connectionString);
            departmentTBL = new DataTable();
            da.Fill(departmentTBL);

            //Bind departmentTBL to dataGridView
            dataGridView1.DataSource = departmentTBL;

            //Users are not allow to update data on dataGridView = readonly
            dataGridView1.Columns[0].ReadOnly = dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[1].Width = dataGridView1.Columns[1].
                GetPreferredWidth(DataGridViewAutoSizeColumnMode.AllCells , true);

            //Primary Key f departmentTBL -> First Column
            departmentTBL.PrimaryKey = new DataColumn[] { departmentTBL.Columns[0] };
        }

        private void dataGridView1_SelectionChanged(object sender , EventArgs e) {

            //output values of selected row to textbox
            int row = dataGridView1.CurrentRow.Index;
            if(row >= 0) {
                string deptID = dataGridView1.Rows[row].Cells[0].Value.ToString();
                string deptName = dataGridView1.Rows[row].Cells[1].Value.ToString();

                depID.Text = deptID;
                depName.Text = deptName;
            }
        }

        private void button3_Click(object sender , EventArgs e) {
            //validation
            string id = depID.Text;
            string name = depName.Text;
            //Id must be a whole number
            int dID;
            if(int.TryParse(id , out dID) == false || string.IsNullOrEmpty(name)) {
                MessageBox.Show("ID must be a number and name cannot be empty");
                return;
            }

            //id cannot be repeated/duplicated -> Find if is any row under
            //departmentTBL contains id
            DataRow dr = departmentTBL.Rows.Find(id);
            if(dr == null) {
                //do insertion

                //1. Create a new row on departmentTBL
                DataRow newRow = departmentTBL.NewRow();

                //2. Update values for new row
                newRow[0] = id;
                newRow[1] = name;

                //3. Insert new row to departmentTBL
                departmentTBL.Rows.Add(newRow);

                //4. Update new row to database: automatically settings
                SqlCommandBuilder cb = new SqlCommandBuilder(da);
                da.Update(departmentTBL);

                DataView dv = new DataView(departmentTBL);
                dataGridView1.DataSource = dv;
                dv.Sort = "Departmentid ascs";
                dv.RowFilter = "Departmentname like '%%'";

                MessageBox.Show("New Department " + name + " has been added");
            } else {
                MessageBox.Show("Department ID cannot be repeated!");
            }
        }

        private void button1_Click(object sender , EventArgs e) {
            //update values of the selected row - except key (department id)
            string id = depID.Text;
            string name = depName.Text;

            //find update row
            DataRow dr = departmentTBL.Rows.Find(id);

            //Update dr on department first
            dr[1] = name;

            //update to database build update command: manually
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("update departmentTBL set"
                + " departmentname = @name where departmentid = @id" , conn);
            cmd.Parameters.AddWithValue("@name" , name);
            cmd.Parameters.AddWithValue("@id" , id);
            da.UpdateCommand = cmd;
            da.Update(departmentTBL);
            MessageBox.Show("Department " + name + " has been updated");
        }
    }
}
