using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dargdropberkaratcom
{
    class Db
    {

        public static string ConnectionString()
        {
            string connectionString = "";
            try
            {
                connectionString = "Server=" + Properties.Settings.Default.DBServer +
                ";Database=" + Properties.Settings.Default.DBName +
                ";User Id=" + Properties.Settings.Default.DBUsername +
                ";Password=" + Properties.Settings.Default.DBPassword +
                ";Persist Security Info = true";
            }
            catch (Exception ex)
            {

                connectionString = ex.Message;
            }


            return connectionString;
        }



        public static string secilensatir(DataGridView dtw, int colomname)
        {
            string mmid = "0";
            if (dtw.Rows.Count > 0)
            {
                int mmsatir = dtw.CurrentCellAddress.Y;
                if (mmsatir < 0) mmsatir = 0;
                if (mmsatir >= 0) mmid = dtw.Rows[mmsatir].Cells[colomname].Value.ToString();
            }
            return mmid;

        }


    }
}
