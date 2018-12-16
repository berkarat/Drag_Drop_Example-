using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dargdropberkaratcom
{
// =============================================
// Author:		<Berk Arat>
// Create date: <10/12/2018>
// Description:	<Computer&Hardware Create. DRAG&DROP using. Example App berkarat.com>
// =============================================
    public partial class Form1 : Form
    {
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
        public class Message
        {
            public string selected_computer_ID { get; set; }
            public string selected_computer_Name { get; set; }
            public string selected_hardware_id { get; set; }
            public string selected_hardware_name { get; set; }
            public string selected_device_description { get; set; }
            public bool update = false;
            public bool changed = false;


        }
        #region DECLARES
        Message msj = new Message();
        DataTable dt_gethardware;
        DataTable dt_getcomputerhardware;
        string err = String.Empty;
        public string[,] computer_type;
        public string[,] operating_system;
        #endregion
        public Form1()
        {
            InitializeComponent();
            button3.Enabled = false;
            Get_Operating_System(out err);
            Get_Computer_Type(out err);
            Get_Computer_Details("99", out err);
            msj.changed = false ;
        }
        #region METHODS
        public DataTable get_image(out string err, string ID)
        {

            try
            {
                err = string.Empty;
                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                string command = @"SELECT  b.NAME,a.[IMG_PATH] FROM [test].[dbo].[tbl_hardware_image] a, tbl_hardware b where a.TYPE=b.TYPE and a.TYPE=" + ID;
                using (SqlCommand _cmd = new SqlCommand(command, sqlCon))
                {
                    sqlCon.Open();
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    DataTable dt = new DataTable();
                    _dap.Fill(dt);
                    return dt;
                }

            }
            catch (Exception ex)
            {
                err = ex.Message;
                return null;
            }

        }
        public bool Get_Computer_Details(string computerid, out string err)
        {
            msj.changed = true;
            #region  Get_Computer_Details

            err = string.Empty;
            try
            {

                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                using (SqlCommand _cmd = new SqlCommand("sp_Get_Computer_Details", sqlCon))
                {
                    sqlCon.Open();
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("@computerid", Convert.ToInt32(computerid));
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    DataTable dt = new DataTable();
                    _dap.Fill(dt);
                    dataGridView2.DataSource = dt;
                    if (dt.Rows.Count == 0)
                    {
                        return false;

                    }
                    else
                    {
                        return true;

                    }

                }
            }


            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion

        }
        public bool Get_Computer_Type(out string err)
        {
            #region Get_Computer_Type
            err = string.Empty;
            try
            {
                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                using (SqlCommand _cmd = new SqlCommand(@"SELECT TOP 1000 [Type]    ,[Name] FROM[test].[dbo].[tbl_computer_type]", sqlCon))
                {
                    sqlCon.Open();
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    DataTable dt = new DataTable();
                    _dap.Fill(dt);

                    computer_type = new string[dt.Rows.Count, dt.Rows.Count + 1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        computer_type[i, 0] = dt.Rows[i]["NAME"].ToString();
                        computer_type[i, 1] = dt.Rows[i]["TYPE"].ToString();
                        ComboboxItem item = new ComboboxItem();
                        item.Text = dt.Rows[i]["NAME"].ToString();
                        item.Value = dt.Rows[i]["TYPE"].ToString();
                        comboBox1.Items.Add(item);
                    }
                    sqlCon.Close();

                    _cmd.Connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion
        }
        public bool Get_Operating_System(out string err)
        {
            #region Get_Operating_System
            err = string.Empty;
            try
            {
                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                using (SqlCommand _cmd = new SqlCommand(@"  SELECT       [OS_ID]      ,[OS_Name]  FROM [test].[dbo].[tbl_oprating_system]", sqlCon))
                {
                    sqlCon.Open();
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    DataTable dt = new DataTable();
                    _dap.Fill(dt);

                    operating_system = new string[dt.Rows.Count, dt.Rows.Count + 1];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        operating_system[i, 0] = dt.Rows[i]["OS_Name"].ToString();
                        operating_system[i, 1] = dt.Rows[i]["OS_ID"].ToString();
                        ComboboxItem item = new ComboboxItem();
                        item.Text = dt.Rows[i]["OS_Name"].ToString();
                        item.Value = dt.Rows[i]["OS_ID"].ToString();
                        comboBox2.Items.Add(item);
                    }
                    sqlCon.Close();

                    _cmd.Connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion
        }
        public bool UPDATE_HARWARE(string computer_id, out string err)
        {
            err = string.Empty;
            try
            {

                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                string query = @"Delete FROM[dbo].[tlb_computer_hardware] WHERE[ComputerID] =" + computer_id;
                using (SqlCommand _cmd = new SqlCommand(query, sqlCon))
                {
                    sqlCon.Open(); _cmd.ExecuteNonQuery(); sqlCon.Close();
                }

                for (int j = 0; j < listView2.Items.Count; j++)
                {

                    string query2 = @"INSERT INTO [dbo].[tlb_computer_hardware]
           ([ComputerID]
           ,[Hardware_ID])

VALUES ( "
               + computer_id + ", "
               + listView1.Items[j].Tag + ")";
                    using (SqlCommand _cmd = new SqlCommand(query2, sqlCon))
                    {
                        sqlCon.Open(); _cmd.ExecuteNonQuery(); sqlCon.Close();
                    }
                }

                MessageBox.Show("UPDATE SUCCESSFULLY !!!");

                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }

        }
        public bool Insert_Computer(out string err)
        {
            #region  INSERT COMPANY
            err = string.Empty;
            try
            {
                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                using (SqlCommand _cmd = new SqlCommand("sp_ComputerCreate", sqlCon))
                {

                    sqlCon.Open();
                    _cmd.CommandType = CommandType.StoredProcedure;
                    _cmd.Parameters.AddWithValue("@ComputerID", textBox1.Text);
                    _cmd.Parameters.AddWithValue("@ComputerName", textBox2.Text);
                    _cmd.Parameters.AddWithValue("@ComputerType", (comboBox1.SelectedItem as ComboboxItem).Value.ToString());
                    _cmd.Parameters.AddWithValue("@Operating_System", (comboBox2.SelectedItem as ComboboxItem).Value.ToString());

                    _cmd.Parameters.Add(new SqlParameter("@OUT_DESCRIPTION", SqlDbType.NVarChar, 1024));
                    _cmd.Parameters["@OUT_DESCRIPTION"].Direction = ParameterDirection.Output;
                    _cmd.Parameters.Add(new SqlParameter("@OUT_RET", SqlDbType.Int));
                    _cmd.Parameters["@OUT_RET"].Direction = ParameterDirection.Output;
                    _cmd.ExecuteNonQuery();
                    sqlCon.Close();
                    _cmd.Connection.Close();

                }
                return true;
            }


            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion
        }
        public bool GET_HARDWARE(out string err)
        {
            #region GET_APP

            err = string.Empty;
            try
            {

                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                string query = @"SELECT [ID]      ,[NAME]      ,[TYPE]  FROM [test].[dbo].[tbl_hardware] ORDER BY ID ";
                using (SqlCommand _cmd = new SqlCommand(query, sqlCon))
                {
                    sqlCon.Open();
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    dt_gethardware = new DataTable();
                    _dap.Fill(dt_gethardware);
                    sqlCon.Close();
                }
                foreach (DataRow row in dt_gethardware.Rows)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = row["NAME"].ToString();
                    item.Tag = row["ID"].ToString();
                    listView1.Items.Add(item);
                }
                return true;
            }
            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion
        }
        public bool GET_HARDWARE(string computer_id, out string err)
        {
            #region GET_APP

            err = string.Empty;
            try
            {

                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                string query = @"SELECT 
b.ComputerName,
c.NAME,
c.ID
  FROM [test].[dbo].[tlb_computer_hardware]  a,
  test.dbo.tbl_computer b,
  test.dbo.tbl_hardware c where
   a.ComputerID=b.ComputerID  and
   a.[Hardware_ID]=c.ID and b.ComputerID=" + computer_id;
                using (SqlCommand _cmd = new SqlCommand(query, sqlCon))
                {
                    sqlCon.Open();
                    SqlDataAdapter _dap = new SqlDataAdapter(_cmd);
                    dt_getcomputerhardware = new DataTable();
                    _dap.Fill(dt_getcomputerhardware);
                    sqlCon.Close();
                }
                foreach (DataRow row in dt_getcomputerhardware.Rows)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = row["NAME"].ToString();
                    item.Tag = row["ID"].ToString();
                    listView2.Items.Add(item);
                }
                return true;
            }
            catch (Exception ex)
            {

                err = ex.Message;
                return false;
            }
            #endregion
        }
        public void select()
        {

            SqlConnection sqlcon = new SqlConnection("Server=DESKTOP-U34S2C6; Database=test; User Id=berk;Password = 123456; ");
            sqlcon.Open();


            SqlCommand _cmd = new SqlCommand(@"SELECT TOP 1000 [ID] ,[NAME] ,[TYPE]  FROM [test].[dbo].[tbl_hardware]", sqlcon);
            SqlDataAdapter dap = new SqlDataAdapter(_cmd);
            DataTable dt = new DataTable();
            dap.Fill(dt);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                ListViewItem lts = new ListViewItem();
                lts.Text = dt.Rows[i]["NAME"].ToString();
                lts.Tag = dt.Rows[i]["TYPE"].ToString();
                listView1.Items.Add(lts);
            }

        }
        #endregion
        #region EVENTS
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            msj.changed = true;
            button3.Enabled = false;
            button2.Enabled = false;
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = false;



            int count = 0;
            SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
            string query_2 = @"SELECT MAX(ID)+1 FROM [test].[dbo].[tbl_computer] ";
            using (SqlCommand _cmd = new SqlCommand(query_2, sqlCon))
            {
                sqlCon.Open();
                count = (int)_cmd.ExecuteScalar();
                sqlCon.Close();
            }
            textBox1.Clear();
            textBox3.Clear();
            textBox2.Clear();

            textBox1.Text = count.ToString();
            textBox1.Enabled = false;
            groupBox2.Visible = true;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            msj.changed = true;

            if (!Get_Computer_Details(textBox3.Text, out err))
            {
                label1.Visible = true;
                pictureBox2.Visible = true;

            }
            else
            {
                label1.Visible = false;
                pictureBox2.Visible = false;
                dataGridView2.Rows[0].Selected = false;
                textBox1.Clear();
                textBox2.Clear();


            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            msj.changed = true;

        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            msj.changed = true;
            button2.Enabled = true;
            button3.Enabled = true;

            //Db.secilensatir(dataGridView2, 3);
            toolStripButton1.Enabled = false;
            toolStripButton2.Enabled = true;
            msj.selected_computer_ID = textBox1.Text = dataGridView2.SelectedCells[1].Value.ToString();
            msj.selected_computer_Name = textBox2.Text = dataGridView2.SelectedCells[0].Value.ToString();
            comboBox1.Text = dataGridView2.SelectedCells[2].Value.ToString();
            comboBox2.Text = dataGridView2.SelectedCells[3].Value.ToString();
            textBox1.Enabled = false;
            //string x  =  dataGridView2.SelectedCells[].Value.ToString();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            msj.changed = true;
            #region COMPUTER UPDATE
            try
            {
                SqlConnection sqlCon = new SqlConnection(Db.ConnectionString());
                string query = @"DELETE FROM [dbo].[tbl_computer] WHERE ComputerID =" + textBox1.Text;
                using (SqlCommand _cmd = new SqlCommand(query, sqlCon))
                {
                    sqlCon.Open(); _cmd.ExecuteNonQuery(); sqlCon.Close();
                }


                Insert_Computer(out err);
                Get_Computer_Details("99", out err);
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
            catch (Exception ex)
            {

            }

            #endregion
        }
        private void tabControl1_Click(object sender, EventArgs e)
        {


            if (tabControl1.SelectedTab.Tag == "2")
            {// INSERT UPDATE 


                ((Control)tabPage2).Enabled = false;
                ((Control)tabPage1).Enabled = false;
            }
            else
            {
                ((Control)tabPage1).Enabled = true;
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            msj.changed = true;
            switch (tabControl1.SelectedTab.Tag)
            {
                case "1":
                    if (Insert_Computer(out err))
                    {
                        MessageBox.Show("INSERT SUCCESFULY! ");

                    }
                    else
                    {
                        MessageBox.Show(err);
                    }
                    break;
                case "2":
                    UPDATE_HARWARE(msj.selected_computer_ID, out err);
                    listView2.Clear();
                    GET_HARDWARE(msj.selected_computer_ID, out err);

                    break;
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            msj.changed = true;
            toolStripButton1.Enabled = true;
            toolStripButton2.Enabled = false;
            tabControl1.SelectedIndex = 1;
            ((Control)tabPage2).Enabled = true;
            listView1.Clear();
            listView2.Clear();
            GET_HARDWARE(out err);
            GET_HARDWARE(msj.selected_computer_ID, out err);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            msj.changed = true;
            tabControl1.SelectedIndex = 0;
            ((Control)tabPage2).Enabled = false;
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (msj.changed)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit without saving ?", "Information ", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    this.Close();
                }
                else if (dialogResult == DialogResult.No)
                {

                }

            }
            else
            {
                this.Close();

            }
        }

        #region List1->List2


        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            msj.changed = true;
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item2 = listView1.SelectedItems[0];
                string path = get_image(out err, item2.Tag.ToString()).Rows[0]["IMG_PATH"].ToString();
                pictureBox1.BackgroundImage = Image.FromFile(path);
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            msj.changed = true;
            if (listView1.SelectedItems.Count > 0)
            {
                listView2.AllowDrop = true;
                panel1.AllowDrop = false;
                //ListViewItem lt = new ListViewItem();
                //lt.Text = listView1.SelectedItems[0].Text;
                //listView1.DoDragDrop(lt, DragDropEffects.Copy | DragDropEffects.Move);


                ListViewItem item2 = listView1.SelectedItems[0];
                //string selected_device_id = item2.Tag.ToString();
                msj.selected_hardware_name = item2.Text;
                msj.selected_hardware_id = item2.Tag.ToString();
                listView1.DoDragDrop(item2, DragDropEffects.Copy | DragDropEffects.Move);

            }

        }
        private void listView2_DragEnter_1(object sender, DragEventArgs e)
        {
            msj.changed = true;
            var obj = e.Data.GetData(e.Data.GetFormats()[0]);
            if (typeof(ListViewItem).IsAssignableFrom(obj.GetType()))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }
        private void listView2_DragDrop_1(object sender, DragEventArgs e)
        {
            msj.changed = true;
            bool check = false;


            ListViewItem lt = new ListViewItem();

            lt.Tag = msj.selected_hardware_id;
            lt.Text = msj.selected_hardware_name;


            for (int i = 0; i < listView2.Items.Count; i++)
            {
                if (listView2.Items[i].Tag.ToString() == lt.Tag.ToString())
                {
                    MessageBox.Show(listView2.Items[i].Text + " Still Exist ! ");
                    check = true;
                    break;
                }
            }

            if (!check)
            {

                listView2.Items.Insert(0, lt);

            }
        }

        #endregion

        #region List2->DELETE

        private void listView2_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {

                panel1.AllowDrop = true;
                listView2.AllowDrop = false;
                ListViewItem item2 = listView2.SelectedItems[0];
                //string selected_device_id = item2.Tag.ToString();
                msj.selected_hardware_name = item2.Text;
                msj.selected_hardware_id = item2.Tag.ToString();
                panel1.DoDragDrop(item2, DragDropEffects.Copy | DragDropEffects.Move);

            }
        }
        private void panel1_DragDrop_1(object sender, DragEventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                listView2.Items.Remove(listView2.SelectedItems[0]);
            }
        }

        private void panel1_DragEnter_1(object sender, DragEventArgs e)
        {
            var obj = e.Data.GetData(e.Data.GetFormats()[0]);
            if (typeof(ListViewItem).IsAssignableFrom(obj.GetType()))
            {
                e.Effect = DragDropEffects.Copy;
            }

        }




        #endregion

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            Get_Computer_Details("99", out err);
        }
    }
}
