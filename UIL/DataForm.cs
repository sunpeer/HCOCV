using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL;
using BLL;

namespace UIL
{
    public partial class DataForm : Form
    {
        public DataForm(App myapp)
        {
            InitializeComponent();
            app = myapp;
        }
        DataTable batteryDataTable;
        DataTable logDataTable;
        App app;
        int curLogD = 0;
        int curBatteryD = 0;
        int sendFalseCount = 0;
        private void logDataTableInfoUpdate(int day)
        {
            DateTime curdate = DateTime.Now;
            DateTime date = curdate.AddDays(-day);
            logCurDate.Text = "数据生成日期：" + date.ToString("MM-dd");
            logCurDataCount.Text = "当前数据量：" + logDataTable.Rows.Count.ToString();
            logProcessStatusLabel.Text = "日志数据库加载完毕";
        }
        private void batteryDatatableInfoUpdate(int day)
        {
            DateTime curdate = DateTime.Now;
            DateTime date=curdate.AddDays(-day);
            batteryCurDate.Text = "数据生成日期：" + date.ToString("MM-dd");
            batteryCurDataCount.Text = "当前数据量：" + batteryDataTable.Rows.Count.ToString();
            foreach (DataRow row in batteryDataTable.Rows)
            {
                string savedflag = row["savedflag"] as string;
                if(savedflag!=null)
                {
                    if (savedflag== "上传失败")
                        sendFalseCount++;
                }
            }
            batterySendDescInfoLabel.Text = "当前数据未成功上传量：" + sendFalseCount.ToString();
            batteryOperationStatusLabel.Text = "电池数据库加载完毕";
        }
        private void DataForm_Load(object sender, EventArgs e)
        {

            //自动默认加载当天的数据
            logDataTable = DataManager.GetLogInfobyDay(0, "");
            //batteryDataTable = DataManager.GetBatteriesInfoIn24Hours();
            //logDataTable = DataManager.GetLogInfoIn24Hours();
            batteryDataTable = DataManager.GetBatteryInfobyDay(0);
            batteryDataGridView.DataSource = batteryDataTable;
            logDataGridView.DataSource = logDataTable;
            logDataTableInfoUpdate(0);
            batteryDatatableInfoUpdate(0);
            //changeDataGridViewColor();
        }

        private void outputLogBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog savFileDialog = new SaveFileDialog();
            savFileDialog.Filter = "(*.csv)|*.csv";
            savFileDialog.FilterIndex = 0;
            savFileDialog.RestoreDirectory = true;
            //savFileDialog.CreatePrompt = true;
            savFileDialog.Title = "导出";
            savFileDialog.ShowDialog();
            string strName = savFileDialog.FileName;
            int day = timeSwtich(log2ecelTimeOption.Text);
            string fName = fNameSwitch(log2excelTypeOption.Text);
            bool result = DataManager.LoadLog2CSV(strName, day, fName);
            if (result)
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("导出失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private string fNameSwitch(string fNameDesc)
        {
            string fName = "";
            switch (fNameDesc)
            {
                case "不限":
                    fName = "";
                    break;
                case "员工登录":
                    fName = "Login";
                    break;
                case "设置工序":
                    fName = "SetupOperation";
                    break;
                case "获取设备资源编号":
                    fName = "GetResource";
                    break;
                case "获取来料信息":
                    fName = "GetData";
                    break;
                case "获取工艺标准":
                    fName = "GetTechnologyStandard";
                    break;
                case "上传采集数据":
                    fName = "UploadBatteryData";
                    break;
                case "上传设备状态":
                    fName = "UploadDeviceStatus";
                    break;
                case "上传报警":
                    fName = "UploadError";
                    break;
            }
            return fName;
        }
        private int timeSwtich(string time_desc)
        {
            int day = 30;
            switch (time_desc)
            {
                case "最近一天":
                    day = 1;
                    break;
                case "最近三天":
                    day = 3;
                    break;
                case "最近一个星期":
                    day = 7;
                    break;
                case "最近半个月":
                    day = 15;
                    break;
                case "最近一个月":
                    day = 30;
                    break;
            }
            return day;
        }

        private void battery2excelBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog savFileDialog = new SaveFileDialog();
            savFileDialog.Filter = "(*.csv)|*.csv";
            savFileDialog.FilterIndex = 0;
            savFileDialog.RestoreDirectory = true;
            //savFileDialog.CreatePrompt = true;
            savFileDialog.Title = "导出";
            savFileDialog.ShowDialog();
            string strName = savFileDialog.FileName;
            int day = timeSwtich(batterytimeOption.Text);
            bool result = DataManager.LoadBattery2CSV(strName, day);
            if (result)
                MessageBox.Show("导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("导出失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void logPreBtn_Click(object sender, EventArgs e)
        {
            if (curLogD == 30)
                return;
            else
            {
                curLogD++;
                string fName = fNameSwitch(logTypeComboBox.Text);
                logDataTable = DataManager.GetLogInfobyDay(curLogD, fName);
                logDataGridView.DataSource = logDataTable;
                logDataTableInfoUpdate(curLogD);
            }
        }

        private void logNextBtn_Click(object sender, EventArgs e)
        {
            if (curLogD == 0)
                return;
            else
            {
                curLogD--;
                string fName = fNameSwitch(logTypeComboBox.Text);
                logDataTable = DataManager.GetLogInfobyDay(curLogD, fName);
                logDataGridView.DataSource = logDataTable;
                logDataTableInfoUpdate(curLogD);
            }
        }

        private void batteryPreBtn_Click(object sender, EventArgs e)
        {
            if (curBatteryD == 30)
                return;
            else
            {
                curBatteryD++;
                sendFalseCount = 0;
                batteryDataTable = DataManager.GetBatteryInfobyDay(curBatteryD);
                batteryDataGridView.DataSource = batteryDataTable;
                batteryDatatableInfoUpdate(curBatteryD);
            }
        }

        private void batteryNextBtn_Click(object sender, EventArgs e)
        {
            if (curBatteryD == 0)
                return;
            else
            {
                curBatteryD--;
                sendFalseCount = 0;
                batteryDataTable = DataManager.GetBatteryInfobyDay(curBatteryD);
                batteryDataGridView.DataSource = batteryDataTable;
                batteryDatatableInfoUpdate(curBatteryD);
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            //从datatable处上传，上传成功一条，
            //更新一条数据库的上传字段，全部结束后，重新向数据库读取
            sendBattery();
        }
        private void sendBattery()
        {
            if (sendFalseCount == 0)
                return;
            baterySendProgressBar.Value = 0;
            baterySendProgressBar.Step = baterySendProgressBar.Maximum / sendFalseCount;
            string msg = "";
            foreach (DataRow row in batteryDataTable.Rows)
            {
                string savedflag = row["savedflag"] as string;
                if (savedflag != null)
                {
                    if (savedflag == "上传失败")
                    {
                        bool result = sendBatteryRow(row, ref msg);   //上传该条数据，如果上传成功则更新数据库
                        if (result)
                        {
                            //更新显示
                            baterySendProgressBar.PerformStep();
                            row["savedflag"] = "上传成功";
                            batterySendDescInfoLabel.Text = "当前数据未成功上传量：" + --sendFalseCount;
                        }
                        else
                        {
                            batteryOperationStatusLabel.Text = "上传失败：" + msg;
                        }
                    }
                }
            }
        }
        //通过row向MES传数据，成功，则更新数据库哦   
        private bool sendBatteryRow(DataRow row,ref string msg)
        {
            return app.localDataUpload(row, ref msg);
        }

        private void DataForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }
    }
}
