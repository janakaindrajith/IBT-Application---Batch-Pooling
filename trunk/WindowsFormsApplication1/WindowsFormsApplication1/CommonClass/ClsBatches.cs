using System;
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

namespace WindowsFormsApplication1.CommonClass
{
    class ClsBatches
    {
        OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF"].ToString());

        public void GeneralBatchRun()
        {
            try
            {

                string vBatchType = "";
                string vCompleteBatch = "";


                //---------------------------500 Record Trnsfer to TCS batch Table in each time--------------------------
                OracleCommand cmdTrnsfer = new OracleCommand("SP_FAS_IBT_POOLING_PROCESS_GI", con);
                cmdTrnsfer.CommandType = CommandType.StoredProcedure;

                cmdTrnsfer.Parameters.Add("vBatchType", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                cmdTrnsfer.Parameters.Add("vCompleteBatch", OracleType.VarChar, 100).Direction = ParameterDirection.Output;

                con.Open();
                cmdTrnsfer.ExecuteNonQuery();

                vBatchType = cmdTrnsfer.Parameters["vBatchType"].Value.ToString();         // SYSTEM  /  MANUAL
                vCompleteBatch = cmdTrnsfer.Parameters["vCompleteBatch"].Value.ToString(); //  YES   /    NO

                con.Close();
                //=========================================End Of Data Transfer==========================================

                if (vCompleteBatch == "EOF")
                {
                    return;
                } 

                OracleCommand ora_cmd = new OracleCommand("SP_FAS_IBT_BATCHES", con);
                ora_cmd.CommandType = CommandType.StoredProcedure;

                ora_cmd.Parameters.Add("v_user_name", OracleType.VarChar).Value = "UPLD";
                ora_cmd.Parameters.Add("v_request_id", OracleType.VarChar).Value = "01";
                ora_cmd.Parameters.Add("vfile", OracleType.VarChar).Value = "";
                ora_cmd.Parameters.Add("VBatchType", OracleType.VarChar).Value = "General";

                con.Open();
                ora_cmd.ExecuteNonQuery();
                con.Close();

                //Feedback update only for system uploaded not the manual excle uploads
                if (vBatchType == "SYSTEM")
                {
                    using (con)
                    {
                        con.Open();
                        OracleCommand command = con.CreateCommand();
                        OracleTransaction transaction;

                        transaction = con.BeginTransaction(System.Data.IsolationLevel.Serializable);
                        command.Transaction = transaction;


                        //----------------Feed back - Detail Update---------------
                        OracleCommand cmdBulk = new OracleCommand("sp_fas_ibt_rv_feedback_gi", con);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add("StatusRet", OracleType.VarChar, 4000).Direction = ParameterDirection.Output;
                        //conn.Open();

                        cmdBulk.Transaction = transaction;

                        cmdBulk.ExecuteNonQuery();
                        string Status = cmdBulk.Parameters["StatusRet"].Value.ToString();//Get Feedback updated serial no list
                        //conn.Close();
                        //--------------------------------------------------------

                        if (Status == "")
                        {
                            transaction.Commit();
                            con.Close();

                            //Email Send only if the batch completed
                            if (vCompleteBatch == "YES")
                            {
                                CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                                clsEmail.SendEmails("General");

                                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                            }

                            return;
                        }
                        //End of normal receipts

                        Status = Status.Remove(0, 1);
                        Status = Status.Replace(",", "','");
                        Status = "'" + Status + "'";


                        OracleCommand cmdGetBulkDTL = con.CreateCommand();
                        cmdGetBulkDTL.CommandText = " select SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1),dtl.serial_no, bdtl.receipt_status,dtl.id  from fas_ibt_bulk_receipt_dtl bdtl " +
                                                    " inner join fas_ibt_uploaded_dtl dtl on  SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1) = dtl.id and  bdtl.effective_end_date is null and " +
                                                    " dtl.serial_no in (" + Status + ") ";



                        OracleDataAdapter oda = new OracleDataAdapter(cmdGetBulkDTL);
                        DataTable dtTemp = new DataTable();

                        cmdGetBulkDTL.Transaction = transaction;

                        oda.Fill(dtTemp);

                        string[] split = Status.Split(',');

                        split = split.Distinct().ToArray();


                        foreach (string item in split)
                        {
                            Console.WriteLine(item);
                            DataRow[] DrAll = dtTemp.Select("serial_no = " + item + "");
                            DataRow[] DrSuccess = dtTemp.Select("serial_no = " + item + " and (receipt_status = 5 or receipt_status = 9)");

                            if (DrAll.Length == DrSuccess.Length)
                            {
                                //Success
                                OracleCommand oraSuccess = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraSuccess.CommandType = CommandType.StoredProcedure;

                                oraSuccess.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 5;
                                oraSuccess.Parameters.Add("vmatching_status", OracleType.Int16).Value = 3;
                                oraSuccess.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraSuccess.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraSuccess.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //conn.Open();

                                oraSuccess.Transaction = transaction;

                                oraSuccess.ExecuteNonQuery();
                                //conn.Close();

                                continue;
                            }
                            else
                            {
                                //Fail
                                OracleCommand oraFail = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraFail.CommandType = CommandType.StoredProcedure;

                                oraFail.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 7;
                                oraFail.Parameters.Add("vmatching_status", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraFail.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //conn.Open();

                                oraFail.Transaction = transaction;

                                oraFail.ExecuteNonQuery();
                                //conn.Close();

                                continue;

                            }
                        }


                        transaction.Commit();
                        con.Close();
                    }
                }

                //Email Send only if the batch completed
                if (vCompleteBatch == "YES")
                {
                    CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                    clsEmail.SendEmails("General");

                    CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                    clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LifeBatchRun()
        {
            try
            {
                string vBatchType = "";
                string vCompleteBatch = "";


                //---------------------------500 Record Trnsfer to TCS batch Table in each time--------------------------
                OracleCommand cmdTrnsfer = new OracleCommand("SP_FAS_IBT_POOLING_PROCESS_LI", con);
                cmdTrnsfer.CommandType = CommandType.StoredProcedure;

                cmdTrnsfer.Parameters.Add("vBatchType", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                cmdTrnsfer.Parameters.Add("vCompleteBatch", OracleType.VarChar, 100).Direction = ParameterDirection.Output;

                con.Open();
                cmdTrnsfer.ExecuteNonQuery();

                vBatchType = cmdTrnsfer.Parameters["vBatchType"].Value.ToString();         // SYSTEM  /  MANUAL
                vCompleteBatch = cmdTrnsfer.Parameters["vCompleteBatch"].Value.ToString(); //  YES   /    NO

                con.Close();
                //=========================================End Of Data Transfer==========================================

                if (vCompleteBatch == "EOF")
                {
                    return;
                } 

                //---------------------------------------------TCS Batch Run---------------------------------------------
                OracleCommand ora_cmd = new OracleCommand("SP_FAS_IBT_BATCHES", con);
                ora_cmd.CommandType = CommandType.StoredProcedure;

                ora_cmd.Parameters.Add("v_user_name", OracleType.VarChar).Value = "UPLD";
                ora_cmd.Parameters.Add("v_request_id", OracleType.VarChar).Value = "01";
                ora_cmd.Parameters.Add("vfile", OracleType.VarChar).Value = "";
                ora_cmd.Parameters.Add("VBatchType", OracleType.VarChar).Value = "Life";

                con.Open();
                ora_cmd.ExecuteNonQuery();
                con.Close();
                //==========================================End Of TCS Batch Run=========================================


                //Feedback update only for system uploaded not the manual excle uploads
                if (vBatchType == "SYSTEM")
                {
                    using (con)
                    {
                        con.Open();
                        OracleCommand command = con.CreateCommand();
                        OracleTransaction transaction;

                        transaction = con.BeginTransaction(System.Data.IsolationLevel.Serializable);
                        command.Transaction = transaction;


                        //----------------Feed back - Detail Update---------------
                        OracleCommand cmdBulk = new OracleCommand("sp_fas_ibt_rv_feedback_life", con);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add("StatusRet", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                        //conn.Open();

                        cmdBulk.Transaction = transaction;

                        cmdBulk.ExecuteNonQuery();
                        string Status = cmdBulk.Parameters["StatusRet"].Value.ToString();//Get Feedback updated serial no list
                        //conn.Close();
                        //--------------------------------------------------------

                        if (Status == "")
                        {
                            transaction.Commit();
                            con.Close();



                            //Email Send only if the batch completed
                            if (vCompleteBatch == "YES")
                            {
                                CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                                clsEmail.SendEmails("Life");

                                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                            }



                            return;
                        }
                        //End of normal receipts

                        Status = Status.Remove(0, 1);
                        Status = Status.Replace(",", "','");
                        Status = "'" + Status + "'";


                        OracleCommand cmdGetBulkDTL = con.CreateCommand();
                        cmdGetBulkDTL.CommandText = " select SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1),dtl.serial_no, bdtl.receipt_status,dtl.id  from fas_ibt_bulk_receipt_dtl bdtl " +
                                                    " inner join fas_ibt_uploaded_dtl dtl on  SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1) = dtl.id and  bdtl.effective_end_date is null and " +
                                                    " dtl.serial_no in (" + Status + ") ";



                        OracleDataAdapter oda = new OracleDataAdapter(cmdGetBulkDTL);
                        DataTable dtTemp = new DataTable();

                        cmdGetBulkDTL.Transaction = transaction;

                        oda.Fill(dtTemp);

                        string[] split = Status.Split(',');

                        split = split.Distinct().ToArray();


                        foreach (string item in split)
                        {
                            Console.WriteLine(item);
                            DataRow[] DrAll = dtTemp.Select("serial_no = " + item + "");
                            DataRow[] DrSuccess = dtTemp.Select("serial_no = " + item + " and (receipt_status = 5 or receipt_status = 9)");

                            if (DrAll.Length == DrSuccess.Length)
                            {
                                //Success
                                OracleCommand oraSuccess = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraSuccess.CommandType = CommandType.StoredProcedure;

                                oraSuccess.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 5;
                                oraSuccess.Parameters.Add("vmatching_status", OracleType.Int16).Value = 3;
                                oraSuccess.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraSuccess.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraSuccess.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //conn.Open();

                                oraSuccess.Transaction = transaction;

                                oraSuccess.ExecuteNonQuery();
                                //conn.Close();

                                continue;
                            }
                            else
                            {
                                //Fail
                                OracleCommand oraFail = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraFail.CommandType = CommandType.StoredProcedure;

                                oraFail.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 7;
                                oraFail.Parameters.Add("vmatching_status", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraFail.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //conn.Open();

                                oraFail.Transaction = transaction;

                                oraFail.ExecuteNonQuery();
                                //conn.Close();

                                continue;

                            }
                        }


                        transaction.Commit();
                        con.Close();
                    }
                }


                //Email Send only if the batch completed
                if (vCompleteBatch == "YES")
                {
                    CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                    clsEmail.SendEmails("Life");

                    CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                    clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void NonTCSBatchRun()
        {
            try
            {

                
                string vBatchType = "";
                string vCompleteBatch = "";


                //---------------------------500 Record Trnsfer to TCS batch Table in each time--------------------------
                OracleCommand cmdTrnsfer = new OracleCommand("SP_FAS_IBT_POOLING_PROCESS_NT", con);
                cmdTrnsfer.CommandType = CommandType.StoredProcedure;

                cmdTrnsfer.Parameters.Add("vBatchType", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                cmdTrnsfer.Parameters.Add("vCompleteBatch", OracleType.VarChar, 100).Direction = ParameterDirection.Output;

                con.Open();
                cmdTrnsfer.ExecuteNonQuery();

                vBatchType = cmdTrnsfer.Parameters["vBatchType"].Value.ToString();         // SYSTEM  /  MANUAL
                vCompleteBatch = cmdTrnsfer.Parameters["vCompleteBatch"].Value.ToString(); //  YES   /    NO

                con.Close();
                //=========================================End Of Data Transfer==========================================


                if (vCompleteBatch == "EOF")
                {
                    return;
                } 

                OracleCommand ora_cmd = new OracleCommand("SP_FAS_IBT_BATCHES", con);
                ora_cmd.CommandType = CommandType.StoredProcedure;

                ora_cmd.Parameters.Add("v_user_name", OracleType.VarChar).Value = "UPLD";
                ora_cmd.Parameters.Add("v_request_id", OracleType.VarChar).Value = "01";
                ora_cmd.Parameters.Add("vfile", OracleType.VarChar).Value = "";
                ora_cmd.Parameters.Add("VBatchType", OracleType.VarChar).Value = "NonTCS";

                con.Open();
                ora_cmd.ExecuteNonQuery();
                con.Close();
                //==========================================End Of TCS Batch Run=========================================


                //Feedback update only for system uploaded not the manual excle uploads
                if (vBatchType == "SYSTEM")
                {
                    using (con)
                    {
                        con.Open();
                        OracleCommand command = con.CreateCommand();
                        OracleTransaction transaction;

                        transaction = con.BeginTransaction(System.Data.IsolationLevel.Serializable);
                        command.Transaction = transaction;


                        //----------------Feed back - Detail Update---------------
                        OracleCommand cmdBulk = new OracleCommand("sp_fas_ibt_rv_feedback_nontcs", con);
                        cmdBulk.CommandType = CommandType.StoredProcedure;
                        cmdBulk.Parameters.Add("StatusRet", OracleType.VarChar, 100).Direction = ParameterDirection.Output;
                        //con.Open();

                        cmdBulk.Transaction = transaction;

                        cmdBulk.ExecuteNonQuery();
                        string Status = cmdBulk.Parameters["StatusRet"].Value.ToString();
                        //con.Close();


                        if (Status == "")
                        {
                            transaction.Commit();
                            con.Close();

                            //Email Send only if the batch completed
                            if (vCompleteBatch == "YES")
                            {
                                CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                                clsEmail.SendEmails("NonTCS");

                                CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                                clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                            }

                            return;
                        }


                        Status = Status.Remove(0, 1);
                        Status = Status.Replace(",", "','");
                        Status = "'" + Status + "'";

                        OracleCommand cmdGetBulkDTL = con.CreateCommand();
                        cmdGetBulkDTL.CommandText = " select SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1),dtl.serial_no, bdtl.receipt_status,dtl.id  from fas_ibt_bulk_receipt_dtl bdtl " +
                                                    " inner join fas_ibt_uploaded_dtl dtl on  SUBSTR(bdtl.dtl_id,0,INSTR(bdtl.dtl_id,'.')-1) = dtl.id and  bdtl.effective_end_date is null and " +
                                                    " dtl.serial_no in (" + Status + ") ";



                        OracleDataAdapter oda = new OracleDataAdapter(cmdGetBulkDTL);
                        DataTable dtTemp = new DataTable();


                        cmdGetBulkDTL.Transaction = transaction;

                        oda.Fill(dtTemp);

                        //--------------------------------------------------------
                        string[] split = Status.Split(',');

                        split = split.Distinct().ToArray();

                        foreach (string item in split)
                        {
                            Console.WriteLine(item);
                            DataRow[] DrAll = dtTemp.Select("serial_no = " + item + "");
                            DataRow[] DrSuccess = dtTemp.Select("serial_no = " + item + " and (receipt_status = 5 or receipt_status = 9)");

                            if (DrAll.Length == DrSuccess.Length)
                            {
                                //Success
                                OracleCommand oraSuccess = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraSuccess.CommandType = CommandType.StoredProcedure;

                                oraSuccess.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 5;
                                oraSuccess.Parameters.Add("vmatching_status", OracleType.Int16).Value = 3;
                                oraSuccess.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraSuccess.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraSuccess.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //con.Open();
                                oraSuccess.Transaction = transaction;
                                oraSuccess.ExecuteNonQuery();
                                //con.Close();

                                continue;
                            }
                            else
                            {
                                //Fail
                                OracleCommand oraFail = new OracleCommand("SP_FAS_IBT_UPLD_DTL_INSERT", con);
                                oraFail.CommandType = CommandType.StoredProcedure;

                                oraFail.Parameters.Add("vreceipt_status", OracleType.Int16).Value = 7;
                                oraFail.Parameters.Add("vmatching_status", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vBULK_RECEIPT_IND", OracleType.Int16).Value = 1;
                                oraFail.Parameters.Add("vCREATEDBY", OracleType.VarChar).Value = "TCS";
                                oraFail.Parameters.Add("vSerialNo", OracleType.VarChar).Value = item.ToString().Replace("'", "");

                                //con.Open();
                                oraFail.Transaction = transaction;
                                oraFail.ExecuteNonQuery();
                                //con.Close();

                                continue;
                            }
                        }


                        transaction.Commit();
                        con.Close();
                    }
                }


                //Email Send only if the batch completed
                if (vCompleteBatch == "YES")
                {
                    CommonClass.ClsEmail clsEmail = new CommonClass.ClsEmail();
                    clsEmail.SendEmails("NonTCS");

                    CommonClass.CommonFunctions clsCommon = new CommonClass.CommonFunctions();
                    clsCommon.Logger(System.Reflection.MethodBase.GetCurrentMethod().Name, "Batch Process Completed & Sent Emails------------------");
                    
                }


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
