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
using System.Net.Mail;
using System.Collections;
using System.Data.SqlClient;

namespace WindowsFormsApplication1.CommonClass
{
    class ClsEmail
    {
        public void EmailAlertCommon(Int16 EmailID, ArrayList List)
        {
            try
            {
                //return;

                String To = "";
                String Cc = "";
                String DisplayName = "";
                String Subject = "";
                String Body = "";

                OracleConnection conn_GetUserRole = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF"].ToString());
                conn_GetUserRole.Open();

                OracleCommand cmd = conn_GetUserRole.CreateCommand();

                cmd.CommandText = " select t.email_to_list,t.email_cc_list,t.email_display_name,t.email_subject,t.email_body from " +
                                  " fas_ibt_email_alerts t where t.Email_id = '" + EmailID + "' and t.effective_end_date is null";

                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();

                oda.Fill(dt);

                //Database having values 
                To = dt.Rows[0]["email_to_list"].ToString().Replace(";",",") + ",";
                Cc = dt.Rows[0]["email_cc_list"].ToString().Replace(";", ",") + ",";
                //DisplayName = dt.Rows[0]["email_display_name"].ToString() + "Testing Only";
                //Subject = dt.Rows[0]["email_subject"].ToString() + "Testing Only";

                DisplayName = dt.Rows[0]["email_display_name"].ToString();
                Subject = dt.Rows[0]["email_subject"].ToString();

                //Body = dt.Rows[0]["email_body"].ToString();





                //-------------MRP ERROR LIST--------------
                if (List != null)
                {
                    Body = "<html><body><table>";
                    Body += "<tr bgcolor=" + "#7C9CB6" + ">" +
                            "<td><font face=" + "Tahoma" + " size=" + "-1>Serial Number</font></td></tr>";

                    foreach (var item in List)
                    {
                        Body += "<tr bgcolor=" + "#CCCCCC" + ">" +
                                    "<td><font face=" + "Tahoma" + " size=" + "-1>" + item + "</font></td></tr>";
                    }
                    Body += "</table></html></body>";
                }
                else
                {
                    Body = dt.Rows[0]["email_body"].ToString();
                }
                //-----------------------------------------






                MailMessage message = new MailMessage();
                MailAddress from = new MailAddress("misreports@hnbassurance.com", DisplayName);


                String OriginTO = To;
                for (int i = 0; i < OriginTO.Length; i++)
                {
                    String Temp = OriginTO.Substring(0, OriginTO.IndexOf(",") + 1);

                    if (Temp == "")
                    {
                        break;
                    }

                    OriginTO = OriginTO.Replace(Temp, "");

                    MailAddress to = new MailAddress(Temp.Replace(",", ""));
                    message.To.Add(to);
                }


                String OriginCC = Cc;
                for (int i = 0; i < OriginCC.Length; i++)
                {
                    String Temp = OriginCC.Substring(0, OriginCC.IndexOf(",") + 1);

                    if (Temp == "")
                    {
                        break;
                    }

                    OriginCC = OriginCC.Replace(Temp, "");

                    MailAddress cc = new MailAddress(Temp.Replace(",", ""));
                    message.CC.Add(cc);
                }



                message.From = from;
                message.Subject = Subject;

                if (List != null)
                {
                    message.IsBodyHtml = true;
                }

                message.Body = @Body;
                SmtpClient client = new SmtpClient("smtp.hnbassurance.com");

                client.Credentials = new System.Net.NetworkCredential("misreports", "Water@1234");

                client.Send(message);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ErrorNotifyEmail(Exception Err)
        {
            try
            {
                MailMessage message = new MailMessage();
                MailAddress from = new MailAddress("misreports@hnbassurance.com", "IBT Receipt Automated Process Error........!");

                //MailAddress to = new MailAddress("janaka.indrajith@hnbassurance.com", "deshapriya.sooriya@hnbassurance.com");
                MailAddress to = new MailAddress("janaka.indrajith@hnbassurance.com");
                message.To.Add(to);

                message.From = from;
                message.Subject = "IBT Receipt Automated Process Error.......!";

                message.Body = Err.Message.ToString();
                SmtpClient client = new SmtpClient("smtp2.hnbassurance.com");

                //Origin
                //client.Credentials = new System.Net.NetworkCredential("smtp2.hnbassurance.com", "Water@1234");
                client.Credentials = new System.Net.NetworkCredential("misreports", "Water@1234");


                client.Send(message);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void SendEmails(String Type)
        {
            try
            {

                OracleConnection connORAGetData = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF_ACC"].ToString());
                SqlConnection connEmail = new SqlConnection(ConfigurationManager.ConnectionStrings["sqlconn"].ToString());


                SqlCommand cmdEmail = new SqlCommand();
                SqlDataReader drEmail;
                OracleCommand cmdORADetails = new OracleCommand();
                OracleDataReader drORADetails;
                DataTable Details = new DataTable();

                String MailHeader = "";
                String MailSubject = "";
                String RefNo = "";
                String AgentCode = "";

                cmdEmail.CommandType = CommandType.Text;
                //cmdEmail.CommandText = "SELECT * FROM BRANCH_E_MAIL WHERE BRANCH_CODE='" + Convert.ToString(Dts.Variables["Branch_code_name"].Value) + "'";
                cmdEmail.CommandText = "SELECT * FROM BRANCH_E_MAIL WHERE BRANCH_CODE = 'HDO'";
                cmdEmail.Connection = connEmail;
                connEmail.Open();
                drEmail = cmdEmail.ExecuteReader();

                cmdORADetails.CommandType = CommandType.Text;


                if (Type == "General")
                {
                    MailHeader = "GI";
                    MailSubject = "GI";
                    RefNo = "Debit No.";
                    AgentCode = "Agent Code";


                    //cmdORADetails.CommandText = "SELECT NVL(crc_get_agent_code(to_char(T1.POL_POLICY_ID)),'D') as PPA_PTY_PARTY_CODE,T.DEBIT_NO,SUBSTR(T.STATUS,-59,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T4.CREATEDBY " +
                    //                            "FROM iims_acc.TEMP_GI_RECEIPT_BATCH T,T_POLICY T1, T_DEBIT_CREDIT_NOTE T3,iims_acc.TEMP_GI_RECEIPT_BATCH_ORG T4 " +
                    //                            "WHERE T.SERIAL = T4.SERIAL " +
                    //                            "AND T.DEBIT_NO=T3.DCN_DEBIT_CREDIT_NOTE_NO " +
                    //                            "AND T1.POL_POLICY_ID=T3.DCN_POL_POLICY_ID " +
                    //                            "AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-59,20),PPA_PTY_PARTY_CODE";


                    //15-08-2017
                    cmdORADetails.CommandText = " SELECT T2.PPA_PTY_PARTY_CODE,T.DEBIT_NO,SUBSTR(T.STATUS,-59,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T4.CREATEDBY " +
                                                " FROM TEMP_GI_RECEIPT_BATCH T,T_POLICY T1, T_POLICY_PARTY T2, T_DEBIT_CREDIT_NOTE T3,TEMP_GI_RECEIPT_BATCH_ORG T4 " +
                                                " WHERE T.SERIAL = T4.SERIAL " +
                                                " AND T.DEBIT_NO=T3.DCN_DEBIT_CREDIT_NOTE_NO " +
                                                " AND T1.POL_POLICY_ID=T3.DCN_POL_POLICY_ID " +
                                                " AND T1.POL_POLICY_ID=T2.PPA_POL_POLICY_ID " +
                                                " AND T2.PPA_SHR_STAKE_HOLDER_FN_CODE IN ('AGENT','HNB_BANK','DIRECT','DIR_SP','DIR_SPIND','BROKER','ASSU_STAFF','HNB_STAFF') " +
                                                " AND T2.PPA_EFFECTIVE_END_DATE IS NULL " +
                                                " AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-59,20),T2.PPA_PTY_PARTY_CODE";
                    //" AND T.POLICY_BRANCH = '" + Convert.ToString(Dts.Variables["Branch_code_name"].Value) + "' ORDER BY SUBSTR(T.STATUS,-59,20),T2.PPA_PTY_PARTY_CODE";
                }

                if (Type == "Life")
                {
                    MailHeader = "LIFE";
                    MailSubject = "LIFE";
                    RefNo = "Policy No.";
                    AgentCode = "Agent Code";


                    
                    //cmdORADetails.CommandText = "SELECT NVL(crc_get_agent_code(to_char(T1.POL_POLICY_ID)),'D') as PPA_PTY_PARTY_CODE,T.POLICY_NUMBER,SUBSTR(T.STATUS,-20,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T3.CREATEDBY " +
                    //                            "FROM iims_acc.TEMP_LIFE_RECEIPT_BATCH T,T_POLICY T1,iims_acc.TEMP_LIFE_RECEIPT_BATCH_ORG T3 WHERE T.SERIAL = T3.SERIAL " +
                    //                            "AND (T.POLICY_NUMBER=T1.POL_POLICY_NUMBER OR T.POLICY_NUMBER=T1.POL_PROPOSAL_NUMBER)  " +
                    //                            "AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-20,20),PPA_PTY_PARTY_CODE";


                    //15-08-2017
                    cmdORADetails.CommandText = "SELECT T2.PPA_PTY_PARTY_CODE,T.POLICY_NUMBER,SUBSTR(T.STATUS,-20,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T3.CREATEDBY " +
                                                "FROM TEMP_LIFE_RECEIPT_BATCH T,T_POLICY T1, T_POLICY_PARTY T2,TEMP_LIFE_RECEIPT_BATCH_ORG T3 " +
                                                "WHERE T.SERIAL = T3.SERIAL AND (T.POLICY_NUMBER=T1.POL_POLICY_NUMBER OR T.POLICY_NUMBER=T1.POL_PROPOSAL_NUMBER) AND T1.POL_POLICY_ID=T2.PPA_POL_POLICY_ID " +
                                                "AND T2.PPA_SHR_STAKE_HOLDER_FN_CODE IN ('AGENT','HNB_BANK','DIRECT','DIR_SP','DIR_SPIND','BROKER','ASSU_STAFF','HNB_STAFF') " +
                                                "AND T2.PPA_EFFECTIVE_END_DATE IS NULL AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-20,20),T2.PPA_PTY_PARTY_CODE";

                    //12-07-2016
                    //cmdORADetails.CommandText = "SELECT T2.PPA_PTY_PARTY_CODE,T.POLICY_NUMBER,SUBSTR(T.STATUS,-20,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE " +
                    //         "FROM TEMP_LIFE_RECEIPT_BATCH T,T_POLICY T1, T_POLICY_PARTY T2,TEMP_LIFE_RECEIPT_BATCH_ORG T3 " +
                    //         "WHERE T.SERIAL = T3.SERIAL " +
                    //         "AND T.POLICY_NUMBER=T1.POL_POLICY_NUMBER " +
                    //         "AND T1.POL_POLICY_ID=T2.PPA_POL_POLICY_ID " +
                    //         "AND T2.PPA_SHR_STAKE_HOLDER_FN_CODE IN ('AGENT','HNB_BANK','DIRECT','DIR_SP','DIR_SPIND','BROKER','ASSU_STAFF','HNB_STAFF') " +
                    //         "AND T2.PPA_EFFECTIVE_END_DATE IS NULL " +
                    //         "AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-20,20),T2.PPA_PTY_PARTY_CODE"; 

                    /////////////"AND T.POLICY_BRANCH = '" + Convert.ToString(Dts.Variables["Branch_code_name"].Value) + "' ORDER BY SUBSTR(T.STATUS,-20,20),T2.PPA_PTY_PARTY_CODE"; 
                }

                if (Type == "NonTCS")
                {
                    MailHeader = "NonTCS";
                    MailSubject = "NonTCS";
                    RefNo = "Policy No.";
                    AgentCode = "Instrument No.";

                    //cmdORADetails.CommandText = " SELECT substr(T.NARRATION,0,instr(T.NARRATION,'-')-1 ) Narration,T.POLICY_NUMBER,SUBSTR(T.STATUS,-20,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T.TRANSACTION_CODE " +
                    //                            " FROM TEMP_LIFE_RECEIPT_BATCH_NONTCS T,temp_receipt_batch_nontcs_org T4  " + //TEMP_LIFE_RECEIPT_BATCH_NT_ORG T4 " +
                    //                            " WHERE T.SERIAL = T4.SERIAL " +
                    //                            " AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-20,20)";
                    //        //" AND T.POLICY_BRANCH = '" + Convert.ToString(Dts.Variables["Branch_code_name"].Value) + "' ORDER BY SUBSTR(T.STATUS,-20,20)";



                    cmdORADetails.CommandText = " SELECT T.Transaction_Code, T.POLICY_NUMBER,SUBSTR(T.STATUS,-20,20) AS RV,T.AMOUNT,T.STATUS,T.CASH_ACCOUNT,T.INSTRUMENT_DATE,T4.CREATEDBY " +
                                                " FROM iims_acc.TEMP_LIFE_RECEIPT_BATCH_NONTCS T,iims_acc.temp_receipt_batch_nontcs_org T4   " +
                                                " WHERE T.SERIAL = T4.SERIAL " +
                                                " AND T.POLICY_BRANCH = 'HDO' ORDER BY SUBSTR(T.STATUS,-20,20) ";

                }

                //connEmail.Close();

                cmdORADetails.Connection = connORAGetData;
                connORAGetData.Open();
                drORADetails = cmdORADetails.ExecuteReader();
                Details.Load(drORADetails);

                //
                if (Details.Rows.Count == 0)
                {
                    ClearTable(Type);
                    return;
                }

                //if (drORADetails.HasRows)
                //{
                string test;
                String UploadedUser = "";
                // TODO: Add your code here
                test = "<html><body><table>";
                test += "<tr bgcolor=" + "#7C9CB6" + ">" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>" + AgentCode + "</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>" + RefNo + "</font></td>" +  //"<td><font face=" + "Tahoma" + " size=" + "-1>Debit No.</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>Receipt No.</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>Amount</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>Status</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>Cash Acc.</font></td>" +
                        "<td><font face=" + "Tahoma" + " size=" + "-1>IBT Date</font></td></tr>";
                for (int i = 0; i < Details.Rows.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        test += "<tr bgcolor=" + "#FFFFFF" + ">" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][0] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][1] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][2] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Convert.ToDouble(Details.Rows[i][3]).ToString("#,##0.00") + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][4] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][5] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][6] + "</font></td></tr>";
                    }
                    else
                    {
                        test += "<tr bgcolor=" + "#CCCCCC" + ">" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][0] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][1] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][2] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Convert.ToDouble(Details.Rows[i][3]).ToString("#,##0.00") + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][4] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][5] + "</font></td>" +
                                "<td><font face=" + "Tahoma" + " size=" + "-1>" + Details.Rows[i][6] + "</font></td></tr>";
                    }
                }


                UploadedUser = Details.Rows[0]["CREATEDBY"].ToString();


                test += "</table></html></body>";
                //Dts.Variables["Email"].Value = test.ToString();
                MailMessage message = new MailMessage();
                //MailAddress from = new MailAddress("misreports@hnbassurance.com", "TCS Receipt Upload - " + MailHeader + "Testing Only");
                MailAddress from = new MailAddress("misreports@hnbassurance.com", "TCS Receipt Upload - " + MailHeader);
                while (drEmail.Read())
                {
                    MailAddress to = new MailAddress(drEmail["EMAIL"].ToString());
                    message.To.Add(to);
                }

                message.From = from;
                //message.Subject = "TCS Receipt Upload - " + MailSubject + " Uploaded User - " + UploadedUser + "Testing Only";
                message.Subject = "TCS Receipt Upload - " + MailSubject + " Uploaded User - " + UploadedUser;
                message.IsBodyHtml = true;
                //string BodyText;
                message.Body = @test;
                SmtpClient client = new SmtpClient("smtp2.hnbassurance.com");

                client.Credentials = new System.Net.NetworkCredential("smtp2.hnbassurance.com", "Water@1234");

                client.Send(message);

                client.Dispose();





                ClearTable(Type);


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static void ClearTable(String Type)
        {

            try
            {
                if (Type == "General")
                {
                    OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF"].ToString());

                    OracleCommand cmd = new OracleCommand();//conn.CreateCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "sp_fas_ibt_clear_Tables_ORG";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vType", OracleType.VarChar).Value = "General";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                if (Type == "Life")
                {
                    OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF"].ToString());

                    OracleCommand cmd = new OracleCommand();//conn.CreateCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "sp_fas_ibt_clear_Tables_ORG";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vType", OracleType.VarChar).Value = "Life";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                if (Type == "NonTCS")
                {
                    OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["ORAWF"].ToString());

                    OracleCommand cmd = new OracleCommand();//conn.CreateCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "sp_fas_ibt_clear_Tables_ORG";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("vType", OracleType.VarChar).Value = "NonTCS";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                
                throw ex;
            }

        }
    }
}
