﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Configuration;
using System.Net.Mail;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {

                this.WindowState = FormWindowState.Minimized;

                notifyIcon1.BalloonTipText = "Application Minimized.";
                notifyIcon1.BalloonTipTitle = "IBT-Application Minimized";

                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(10);

                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "*************** Application Started at " + DateTime.Now +  " ***************");
            }
            catch (Exception ex)
            {
                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.InnerException.ToString());
                throw;
            }

        }

        private void BatchProcessLife()
        {
            try
            {
                CommonClass.ClsBatches clsBatch = new CommonClass.ClsBatches();
                clsBatch.LifeBatchRun();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void BatchProcessGeneral()
        {
            try
            {
                CommonClass.ClsBatches clsBatch = new CommonClass.ClsBatches();
                clsBatch.GeneralBatchRun();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BatchProcessNonTCS()
        {
            try
            {
                CommonClass.ClsBatches clsBatch = new CommonClass.ClsBatches();
                clsBatch.NonTCSBatchRun();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {


                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Started--------------------");



                BatchProcessLife();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "-Life Batch Done");

                BatchProcessGeneral();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "-General Batch Done");

                BatchProcessNonTCS();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "-NonTCS Batch Done");


            }
            catch (Exception ex)
            {
                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString());

                CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                clsEmail.ErrorNotifyEmail(ex);
                //Error Email/SMS Auto Generate
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

    }
}
