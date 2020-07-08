using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCV
{
    public partial class DataForm : Form
    {
        public DataForm()
        {
            InitializeComponent();
        }
        DataTable batteryDataTable = null;
        DataTable logDataTable = null;

        private void DataForm_Load(object sender, EventArgs e)
        {
            //自动默认加载最近一天的数据，加载后不会自动刷新，
            batteryDataTable = DataManager.GetBatteriesInfoIn24Hours();
            logDataTable = DataManager.GetLogInfoIn24Hours();
            batteryDataGridView.DataSource = batteryDataTable;
            logDataGridView.DataSource = logDataTable;
        }

        private void log_Enter(object sender, EventArgs e)
        {
            //显示的控件
            logTypeLabel.Visible = true;
            logTypeComboBox.Visible = true;
            logSearchBtn.Visible = true;
            logRefresh.Visible = true;
            //隐藏的控件
            batterySearchBtn.Visible = false;
            batteryRefresh.Visible = false;
            verifiedLabel.Visible = false;
            verifiedComboBox.Visible = false;
            sendResultLabel.Visible = false;
            sendResultComboBox.Visible = false;
            errtypeLabel.Visible = false;
            errtypeComboBox.Visible = false;
            snLabel.Visible = false;
            snTextBox.Visible = false;
        }

        private void battery_Enter(object sender, EventArgs e)
        {
            //隐藏的控件
            logTypeLabel.Visible = false;
            logTypeComboBox.Visible = false;
            logSearchBtn.Visible = false;
            logRefresh.Visible = false;
            //显示的控件
            batterySearchBtn.Visible = true;
            batteryRefresh.Visible = true;
            verifiedLabel.Visible = true;
            verifiedComboBox.Visible = true;
            sendResultLabel.Visible = true;
            sendResultComboBox.Visible = true;
            errtypeLabel.Visible = true;
            errtypeComboBox.Visible = true;
            snLabel.Visible = true;
            snTextBox.Visible = true;
        }

        private void batterySearchBtn_Click(object sender, EventArgs e)
        {
            string userIdQuery = (userTextBox.Text==string.Empty)?"":("user==" +"'"+ userTextBox.Text+"'");
            string resultQuery = (errtypeComboBox.Text == "不限") ? "" : ("errtype==" + "'"+errtypeComboBox.Text+"'");
            string verifiedQuery = (verifiedComboBox.Text == "不限") ? "" : ("verifyflag==" + "'" + verifiedComboBox.Text + "'");
            string savedQuery = (sendResultComboBox.Text == "不限") ? "" : ("savedFlag==" + "'" + sendResultComboBox.Text + "'");
            //日期处理
            string dateQuery = "";
            if (startDateTimePicker.Value.ToShortDateString() == endDateTimePicker.Value.ToShortDateString())
            {
                //选择的是某一天的数据
                dateQuery = "date_format(opertime,'%Y/%m/%d')='" +
                    startDateTimePicker.Value.ToShortDateString() + "'";
            }
            else
            //选择的是一段时间的数据
            {
                dateQuery = "date_fromat(opertime,'%Y/%m/%d') between '" +
                    startDateTimePicker.Value.ToShortDateString() + "' and '" +
                    endDateTimePicker.Value.ToShortDateString() + "'";
            }
            string sqlQuery = "select * from log where " + userIdQuery + " and " + resultQuery + " and " +
                verifiedQuery + " and " + savedQuery + " and "+dateQuery;
            batteryDataTable = DataManager.GetBatteryInfoByCondition(sqlQuery);
            batteryDataGridView.DataSource = batteryDataTable;
        }

        private void logSearchBtn_Click(object sender, EventArgs e)
        {
            string useIdQuery = (userTextBox.Text==string.Empty)?"":("user=="+"'"+userTextBox.Text+"'");
            string logTypeQuery = (logTypeComboBox.Text == "不限") ? "" : ("opertype==" + "'"+logTypeComboBox.Text+"'");
            string dateQuery = "";
            //日期处理
            if(startDateTimePicker.Value.ToShortDateString()==endDateTimePicker.Value.ToShortDateString())
            {
                //选择的是某一天的数据
                dateQuery = "date_format(opertime,'%Y/%m/%d')='" + 
                    startDateTimePicker.Value.ToShortDateString() + "'";
            }
            else
            //选择的是一段时间的数据
            {
                dateQuery = "date_fromat(opertime,'%Y/%m/%d') between '" +
                    startDateTimePicker.Value.ToShortDateString() + "' and '" +
                    endDateTimePicker.Value.ToShortDateString() + "'";
            }
            string sqlQuery = "select * from log where " + useIdQuery + " and " + logTypeQuery + " and " + dateQuery;
            logDataTable=DataManager.GetLogInfoByCondition(sqlQuery);
            logDataGridView.DataSource = logDataTable;
        }

        private void batteryRefresh_Click(object sender, EventArgs e)
        {
            batteryDataTable = DataManager.GetBatteriesInfoIn24Hours();
            batteryDataGridView.DataSource = batteryDataTable;
        }

        private void logRefresh_Click(object sender, EventArgs e)
        {
            logDataTable = DataManager.GetLogInfoIn24Hours();
            logDataGridView.DataSource = logDataTable;
        }

    }
}
