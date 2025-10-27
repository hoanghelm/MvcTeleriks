using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
//using WIRS.BusinessComponents;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using WIRS.DAL;


namespace WIRS.CommonUtilities
{
    public class CommonFun
    {

        public static DataSet GetLookUpType(string lookup_type, string parent_id)
        {
            return CommonFunDAL.GetLookUpType(lookup_type, parent_id);
        }

        public static DataSet GetAllLookUpType(string lookup_type, string parent_id)
        {
            return CommonFunDAL.GetAllLookUpType(lookup_type, parent_id);
        }
        public DataSet Get_sbu_by_uid(string sba_code, string sbu_code)
        {
            return CommonFunDAL.Get_sbu_by_uid(sba_code, sbu_code);
        }

        public DataSet Setup_all_sbus(string sba_code)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.Setup_all_sbus(sba_code);
            return ds;
        }
        public void Save_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by, out string error_Code)
        {
            DataSet ds = new DataSet();
            ds = Get_sbu_by_uid(sba_code, sbu_code);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Update_sbu(sba_code, sbu_code, sbu_name, inactive_date, modified_by, out error_Code);
            }
            else
            {
                Insert_sbu(sba_code, sbu_code, sbu_name, inactive_date, modified_by, out error_Code);
            }
        }

        public void Insert_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by, out string error_Code)
        {
            error_Code = string.Empty;
            CommonFunDAL.Insert_sbu(sba_code, sbu_code, sbu_name, inactive_date, modified_by, out error_Code);
        }

        public void Update_sbu(string sba_code, string sbu_code, string sbu_name, string inactive_date, string modified_by, out string error_Code)
        {
            error_Code = string.Empty;
            CommonFunDAL.Update_sbu(sba_code, sbu_code, sbu_name, inactive_date, modified_by, out error_Code);
        }


        public DataSet Get_location_by_uid(string sba_code, string sbu_code, string department_code, string location_code)
        {
            return CommonFunDAL.Get_location_by_uid(sba_code, sbu_code, department_code, location_code);
        }

        public DataSet get_all_locations(string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.get_all_locations(sba_code, sbu_code, department_code);
            return ds;
        }
        public DataSet get_active_locations(string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.get_active_locations(sba_code, sbu_code, department_code);
            return ds;
        }

        public DataSet search_departments(string code_type, string sba_code, string sbu_code, string department_name)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.search_departments(code_type, sba_code, sbu_code, department_name);
            return ds;
        }
        public DataSet get_all_departments(string code_type, string sba_code, string sbu_code)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.get_all_departments(code_type, sba_code, sbu_code);
            return ds;
        }
        public DataSet get_active_departments(string code_type, string sba_code, string sbu_code)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.get_active_departments(code_type, sba_code, sbu_code);
            return ds;
        }
        public DataSet search_locations(string sba_code, string sbu_code, string department_code, string location_name)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.search_locations(sba_code, sbu_code, department_code, location_name);
            return ds;
        }
        public DataSet Setup_active_locations(string sba_code, string sbu_code, string department_code)
        {
            DataSet ds = new DataSet();

            try
            {
                //                dropDownListName.Items.Clear();
                ds = CommonFunDAL.get_active_locations(sba_code, sbu_code, department_code);

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Any " + "Location" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }
        public void Save_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by, out string error_Code)
        {
            DataSet ds = new DataSet();
            ds = Get_location_by_uid(sba_code, sbu_code, department_code, location_code);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Update_location(sba_code, sbu_code, department_code, location_code, location_name, inactive_date, modified_by, out error_Code);
            }
            else
            {
                Insert_location(sba_code, sbu_code, department_code, location_code, location_name, inactive_date, modified_by, out error_Code);
            }
        }

        public void save_department(string code_type, string sba_code, string sbu_code, string department_code, string department_name, string inactive_date, string modified_by, out string error_Code)
        {
            DataSet ds = new DataSet();
            ds = get_department_by_uid(code_type, sba_code, sbu_code, department_code);
            if (ds.Tables[0].Rows.Count > 0)
            {
                CommonFunDAL.update_department(code_type, sba_code, sbu_code, department_code, department_name, inactive_date, modified_by, out error_Code);
            }
            else
            {
                CommonFunDAL.insert_department(code_type, sba_code, sbu_code, department_code, department_name, inactive_date, modified_by, out error_Code);
            }
        }
        public DataSet get_department_by_uid(string code_type, string sba_code, string sbu_code, string department_code)
        {
            return CommonFunDAL.get_department_by_uid(code_type, sba_code, sbu_code, department_code);
        }
        public void Insert_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by, out string error_Code)
        {
            error_Code = string.Empty;
            CommonFunDAL.Insert_location(sba_code, sbu_code, department_code, location_code, location_name, inactive_date, modified_by, out error_Code);
        }

        public void Update_location(string sba_code, string sbu_code, string department_code, string location_code, string location_name, string inactive_date, string modified_by, out string error_Code)
        {
            error_Code = string.Empty;
            CommonFunDAL.Update_location(sba_code, sbu_code, department_code, location_code, location_name, inactive_date, modified_by, out error_Code);
        }


        public void Setup_all_sbus(DropDownList dropDownListName, string sba_code)
        {
            DataSet ds = new DataSet();
            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.Setup_all_sbus(sba_code);

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Select " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Setup_active_sbus(DropDownList dropDownListName, string sba_code)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.Setup_active_sbus(sba_code);

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Any " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetupAllsbas(DropDownList dropDownListName)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAllSbus();

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Select " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetupAllSbus(DropDownList dropDownListName)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAllSbus();

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Select " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetupAnySbus(DropDownList dropDownListName)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAllSbus();

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Any " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetSbusWithAll(out DataSet sbuList)
        {
            sbuList = new DataSet();
            try
            {

                sbuList = CommonFunDAL.GetSbusWithAll();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetLookUpType(string lookuptype)
        {
            return CommonFunDAL.GetLookUpType(lookuptype);
        }

        public void SetupAccessSbubyUser(DropDownList dropDownListName, string userId, string userRole, string txt, string sba_code)
        {
            DataSet ds = new DataSet();
            string errorCode = string.Empty;

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAccessSbubyUser(userId, userRole, sba_code, errorCode);

                DataRow row = ds.Tables[0].NewRow();
                row["sbu_code"] = "";
                row["sbu_name"] = txt;//"--Any " + "SBU" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetupAccessSbabyUser(DropDownList dropDownListName, string userId, string userRole, string txt)
        {
            DataSet ds = new DataSet();
            string errorCode = string.Empty;
            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAccessSbabyUser(userId, userRole, out errorCode);

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = txt;//"--Any " + "SBU" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sba_name";
                dropDownListName.DataValueField = "sba_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SetupLookUpCheckboxlist(CheckBoxList chkboxlistName, string lookuptype)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetLookUpType(lookuptype);
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = "lookup_value";
            chkboxlistName.DataValueField = "lookup_code";
            chkboxlistName.DataBind();
        }

        public void DisplayLookUpCheckboxlist(CheckBoxList chkboxlistName, DataTable dt, string _value, string _text)
        {
            chkboxlistName.DataSource = dt;
            chkboxlistName.DataTextField = _text;
            chkboxlistName.DataValueField = _value;
            chkboxlistName.DataBind();
        }

        public void SetupNameofSubmission(Label l1, Label l2, Label l3, Label l4, DataTable dt)
        {
            l1.Text = dt.Rows[0]["from_name"].ToString();
            l2.Text = dt.Rows[0]["from"].ToString();
            l3.Text = dt.Rows[0]["Date"].ToString();
            l4.Text = dt.Rows[0]["from_designation"].ToString();
        }

        public void SetupLookUpCheckboxlist(CheckBoxList chkboxlistName, string lookuptype, string ParentID)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetLookUpType(lookuptype, ParentID);
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = "lookup_value";
            chkboxlistName.DataValueField = "lookup_code";
            chkboxlistName.DataBind();
        }

        public void SetupLookUpRadiolist(RadioButtonList rdoName, string lookuptype)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetLookUpType(lookuptype);
            rdoName.DataSource = ds;
            rdoName.DataTextField = "lookup_value";
            rdoName.DataValueField = "lookup_code";
            rdoName.DataBind();
        }

        public void BindCodeToListControl(ListControl lc, DataSet ds, string textField, string valueField, string emptyText)
        {
            lc.DataTextField = textField;
            lc.DataValueField = valueField;
            lc.DataSource = ds.Tables[0];
            lc.DataBind();

            if (emptyText != null && emptyText != string.Empty)
            {
                lc.Items.Insert(0, new ListItem(emptyText, string.Empty));
            }
        }

        public void SetupLookUpDDList(DropDownList dropDownListName, string lookuptype)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetLookUpType(lookuptype);
            dropDownListName.DataSource = ds;
            dropDownListName.DataTextField = "lookup_value";
            dropDownListName.DataValueField = "lookup_code";
            dropDownListName.DataBind();
        }

        public string GetLookUpTypebyValue(string lookuptype, string lookup_value)
        {
            return CommonFunDAL.GetLookUpTypebyValue(lookuptype, lookup_value);
        }

        public string GetLookUpTypebyCode(string lookuptype, string lookup_code)
        {
            return CommonFunDAL.GetLookUpTypebyCode(lookuptype, lookup_code);
        }

        //public void SetupLookUpDDList(DropDownList dropDownListName, string lookuptype, string firstText)
        //{
        //    DataSet ds = new DataSet();
        //    ds = CommonFunDAL.GetLookUpType(lookuptype);
        //    DataRow row = ds.Tables[0].NewRow();
        //    row[0] = "";
        //    row[1] = firstText;
        //    ds.Tables[0].Rows.InsertAt(row, 0);

        //    dropDownListName.DataSource = ds;
        //    dropDownListName.DataTextField = "lookup_value";
        //    dropDownListName.DataValueField = "lookup_code";
        //    dropDownListName.DataBind();
        //}

        public string generate_sbu_code(string sba_code)
        {
            string sbu_code = string.Empty;
            DataSet ds = new DataSet();
            ds = CommonFunDAL.generate_sbu_code(sba_code);
            if (ds.Tables[0].Rows.Count > 0)
            {
                sbu_code = ds.Tables[0].Rows[0][0].ToString();
            }

            return sbu_code;
        }

        public string generate_lookup_code(string lookup_type)
        {
            string str_code = string.Empty;
            DataSet ds = new DataSet();
            ds = CommonFunDAL.generate_lookup_code(lookup_type);
            if (ds.Tables[0].Rows.Count > 0)
            {
                str_code = ds.Tables[0].Rows[0][0].ToString();
            }

            return str_code;
        }
        public string generate_department_code(string lookup_type)
        {
            string str_code = string.Empty;
            DataSet ds = new DataSet();
            ds = CommonFunDAL.generate_department_code(lookup_type);
            if (ds.Tables[0].Rows.Count > 0)
            {
                str_code = ds.Tables[0].Rows[0][0].ToString();
            }

            return str_code;
        }
        public string generate_location_code(string lookup_type)
        {
            string str_code = string.Empty;
            DataSet ds = new DataSet();
            ds = CommonFunDAL.generate_location_code(lookup_type);
            if (ds.Tables[0].Rows.Count > 0)
            {
                str_code = ds.Tables[0].Rows[0][0].ToString();
            }

            return str_code;
        }

        public void SetupLookUpDDList(ListControl dropDownListName, string lookuptype, string firstText)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetLookUpType(lookuptype);
            DataRow row = ds.Tables[0].NewRow();
            row[0] = "";
            row[1] = firstText;
            ds.Tables[0].Rows.InsertAt(row, 0);

            dropDownListName.DataTextField = "lookup_value";
            dropDownListName.DataValueField = "lookup_code";
            dropDownListName.DataSource = ds.Tables[0];
            dropDownListName.DataBind();

            //dropDownListName.DataSource = ds;
            //dropDownListName.DataTextField = "lookup_value";
            //dropDownListName.DataValueField = "lookup_code";
            //dropDownListName.DataBind();
        }
        //public void SetupLookUpDDListDesigantion(DropDownList dropDownListName, string lookuptype, string firstText)
        //{
        //    DataSet ds = new DataSet();
        //    ds = CommonFunDAL.GetLookUpType(lookuptype);
        //    DataRow row = ds.Tables[0].NewRow();
        //    row[0] = "";
        //    row[1] = firstText;
        //    ds.Tables[0].Rows.InsertAt(row, 0);

        //    dropDownListName.DataSource = ds;
        //    dropDownListName.DataTextField = "lookup_value";
        //    dropDownListName.DataValueField = "lookup_code";
        //    dropDownListName.DataBind();
        //}

        public void SetupEmailDistributionList(DropDownList dropDownListName)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetAllSbus();

                DataRow row = ds.Tables[0].NewRow();
                row[0] = "";
                row[1] = "--Select " + "LOB" + "--";
                ds.Tables[0].Rows.InsertAt(row, 0);
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "sbu_name";
                dropDownListName.DataValueField = "sbu_code";
                dropDownListName.DataBind();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GetCCEmailDistribution(CheckBoxList chkboxlistName)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetCCEmailDistribution();
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = "appointment";
            chkboxlistName.DataValueField = "emp_no";
            chkboxlistName.DataBind();
        }

        public void GetToEmailDistribution(CheckBoxList chkboxlistName, DataSet ds)
        {
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = "WSHOfficer";
            chkboxlistName.DataValueField = "WSHOfficer";
            chkboxlistName.DataBind();

        }

        public void BindCheckBoxList(CheckBoxList chkboxlistName, DataSet ds, string text, string value)
        {
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = text;
            chkboxlistName.DataValueField = value;
            chkboxlistName.DataBind();
        }

        public void AddListViewItem(ListView listViewControl, Dictionary<string, string> colItems, string sessionKey)
        {
            // Declarations
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (AppSession.GetSession(sessionKey) != null)
                {
                    ds = (DataSet)AppSession.GetSession(sessionKey);
                    dt = ds.Tables[0];

                    if (dt.Columns.Count.Equals(0))
                    {
                        // Add columns
                        foreach (KeyValuePair<string, string> pair in colItems)
                        {
                            // Add columns to the datatable through given dictionary
                            dt.Columns.Add(pair.Key);
                        }
                    }
                    // Add new items
                    DataRow row = dt.NewRow();
                    foreach (KeyValuePair<string, string> pair in colItems)
                    {
                        row[pair.Key] = pair.Value;
                    }

                    dt.Rows.Add(row);
                }
                else
                {
                    dt = new DataTable();
                    foreach (KeyValuePair<string, string> pair in colItems)
                    {
                        // Add columns to the datatable through given dictionary
                        dt.Columns.Add(pair.Key);
                    }

                    DataRow row = dt.NewRow();
                    // Loop again to the dictionary to add row
                    foreach (KeyValuePair<string, string> pair in colItems)
                    {
                        row[pair.Key] = pair.Value;
                    }
                    dt.Rows.Add(row);

                    // Add tables to the dataset to rebind to gridview
                    ds.Tables.Add(dt);
                }

                AppSession.SetSession(sessionKey, ds);
                listViewControl.DataSource = ds;
                listViewControl.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteListViewItem(ListView listViewControl, int itemIndex, string sessionKey)
        {
            // Declarations
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                if (AppSession.GetSession(sessionKey) != null)
                {
                    ds = (DataSet)AppSession.GetSession(sessionKey);
                    dt = ds.Tables[0];

                    if (dt.Rows.Count > 0)
                    {
                        dt.Rows.RemoveAt(itemIndex);
                    }

                    AppSession.SetSession(sessionKey, ds);
                    listViewControl.DataSource = ds;
                    listViewControl.DataBind();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GenerateBlankListview(ListView listViewName, string sessionKey)
        {
            // Declarations
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                // Add a blank data table
                ds.Tables.Add(dt);

                // Clear all rows inside a data table
                ds.Clear();

                // Store value to session
                AppSession.SetSession(sessionKey, ds);

                // Set the listview datasource with a blank data source and bind it
                listViewName.DataSource = ds;
                listViewName.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsNumeric(string sText)
        {
            bool bIsNum = false;
            if (long.TryParse(sText, out long iTemp))
            {
                bIsNum = true;
            }

            return bIsNum;
        }

        public bool IsPhoneNo(string sText)
        {
            bool bIsPhone = false;
            if (int.TryParse(sText, out int iTemp))
            {
                bIsPhone = true;
            }

            return bIsPhone;
        }

        public bool IsNRICFIN(string sText)
        {
            bool bIsNum = false;

            bool validate = false;
            string firstChar = string.Empty;
            string lastChar = string.Empty;
            string middleChar = string.Empty;
            string identity = ConfigurationManager.AppSettings["Indentity"].ToString();
            string[] identitystr = identity.Split(',');
            string chkdigit = string.Empty;

            int num1 = 2;
            int num2 = 7;
            int num3 = 6;
            int num4 = 5;
            int num5 = 4;
            int num6 = 3;
            int num7 = 2;
            int addnum = 4;
            int sum = 0;
            int result;


            sText = sText.ToUpper();
            firstChar = sText[0].ToString();
            lastChar = sText[(sText.Length - 1)].ToString();
            middleChar = sText.Substring(1, sText.Length - 2);




            foreach (string i in identitystr)
            {
                if (i == firstChar)
                {
                    validate = true;
                    break;
                }
            }

            if (validate == true)
            {
                if (IsNumeric(middleChar) == true)
                {
                    sum = (Convert.ToInt32(middleChar[0].ToString()) * num1) + (Convert.ToInt32(middleChar[1].ToString()) * num2) + (Convert.ToInt32(middleChar[2].ToString()) * num3) +
                        (Convert.ToInt32(middleChar[3].ToString()) * num4) + (Convert.ToInt32(middleChar[4].ToString()) * num5) + (Convert.ToInt32(middleChar[5].ToString()) * num6) +
                        (Convert.ToInt32(middleChar[6].ToString()) * num7);
                    if (firstChar == "T" || firstChar == "G")
                    {
                        sum = sum + addnum;

                    }
                    result = sum % 11;
                    result = 11 - result;


                    if (firstChar == "S" || firstChar == "T")
                    {
                        switch (result)
                        {
                            case 1:
                                chkdigit = "A";
                                break;
                            case 2:
                                chkdigit = "B";
                                break;
                            case 3:
                                chkdigit = "C";
                                break;
                            case 4:
                                chkdigit = "D";
                                break;
                            case 5:
                                chkdigit = "E";
                                break;
                            case 6:
                                chkdigit = "F";
                                break;
                            case 7:
                                chkdigit = "G";
                                break;
                            case 8:
                                chkdigit = "H";
                                break;
                            case 9:
                                chkdigit = "I";
                                break;
                            case 10:
                                chkdigit = "Z";
                                break;
                            case 11:
                                chkdigit = "J";
                                break;
                        }
                    }
                    else if (firstChar == "F" || firstChar == "G")
                    {
                        switch (result)
                        {
                            case 1:
                                chkdigit = "K";
                                break;
                            case 2:
                                chkdigit = "L";
                                break;
                            case 3:
                                chkdigit = "M";
                                break;
                            case 4:
                                chkdigit = "N";
                                break;
                            case 5:
                                chkdigit = "P";
                                break;
                            case 6:
                                chkdigit = "Q";
                                break;
                            case 7:
                                chkdigit = "R";
                                break;
                            case 8:
                                chkdigit = "T";
                                break;
                            case 9:
                                chkdigit = "U";
                                break;
                            case 10:
                                chkdigit = "W";
                                break;
                            case 11:
                                chkdigit = "X";
                                break;
                        }

                    }
                    if (chkdigit == lastChar)
                    {
                        bIsNum = true;
                    }
                }

            }




            return bIsNum;
        }

        public bool IsNRIC(string sText)
        {
            bool bIsNum = false;

            bool validate = false;
            string firstChar = string.Empty;
            string lastChar = string.Empty;
            string middleChar = string.Empty;
            string identity = ConfigurationManager.AppSettings["NRIC"].ToString();
            string[] identitystr = identity.Split(',');
            string chkdigit = string.Empty;

            int num1 = 2;
            int num2 = 7;
            int num3 = 6;
            int num4 = 5;
            int num5 = 4;
            int num6 = 3;
            int num7 = 2;
            int sum = 0;
            int addnum = 4;
            int result;


            sText = sText.ToUpper();
            firstChar = sText[0].ToString();
            lastChar = sText[(sText.Length - 1)].ToString();
            middleChar = sText.Substring(1, sText.Length - 2);

            foreach (string i in identitystr)
            {
                if (i == firstChar)
                {
                    validate = true;
                    break;
                }
            }

            if (validate == true)
            {
                if (IsNumeric(middleChar) == true)
                {
                    sum = (Convert.ToInt32(middleChar[0].ToString()) * num1) + (Convert.ToInt32(middleChar[1].ToString()) * num2) + (Convert.ToInt32(middleChar[2].ToString()) * num3) +
                        (Convert.ToInt32(middleChar[3].ToString()) * num4) + (Convert.ToInt32(middleChar[4].ToString()) * num5) + (Convert.ToInt32(middleChar[5].ToString()) * num6) +
                        (Convert.ToInt32(middleChar[6].ToString()) * num7);

                    if (firstChar == "T")
                    {
                        sum = sum + addnum;

                    }
                    result = sum % 11;
                    result = 11 - result;


                    if (firstChar == "S" || firstChar == "T")
                    {
                        switch (result)
                        {
                            case 1:
                                chkdigit = "A";
                                break;
                            case 2:
                                chkdigit = "B";
                                break;
                            case 3:
                                chkdigit = "C";
                                break;
                            case 4:
                                chkdigit = "D";
                                break;
                            case 5:
                                chkdigit = "E";
                                break;
                            case 6:
                                chkdigit = "F";
                                break;
                            case 7:
                                chkdigit = "G";
                                break;
                            case 8:
                                chkdigit = "H";
                                break;
                            case 9:
                                chkdigit = "I";
                                break;
                            case 10:
                                chkdigit = "Z";
                                break;
                            case 11:
                                chkdigit = "J";
                                break;
                        }
                    }
                    if (chkdigit == lastChar)
                    {
                        bIsNum = true;
                    }
                }

            }




            return bIsNum;
        }

        public bool IsFIN(string sText)
        {
            bool bIsNum = false;

            bool validate = false;
            string firstChar = string.Empty;
            string lastChar = string.Empty;
            string middleChar = string.Empty;
            string identity = ConfigurationManager.AppSettings["FIN"].ToString();
            string[] identitystr = identity.Split(',');
            string chkdigit = string.Empty;

            int num1 = 2;
            int num2 = 7;
            int num3 = 6;
            int num4 = 5;
            int num5 = 4;
            int num6 = 3;
            int num7 = 2;
            int addnum = 4;
            int sum = 0;
            int result;


            sText = sText.ToUpper();
            firstChar = sText[0].ToString();
            lastChar = sText[(sText.Length - 1)].ToString();
            middleChar = sText.Substring(1, sText.Length - 2);




            foreach (string i in identitystr)
            {
                if (i == firstChar)
                {
                    validate = true;
                    break;
                }
            }

            if (validate == true)
            {
                if (IsNumeric(middleChar) == true)
                {
                    sum = (Convert.ToInt32(middleChar[0].ToString()) * num1) + (Convert.ToInt32(middleChar[1].ToString()) * num2) + (Convert.ToInt32(middleChar[2].ToString()) * num3) +
                        (Convert.ToInt32(middleChar[3].ToString()) * num4) + (Convert.ToInt32(middleChar[4].ToString()) * num5) + (Convert.ToInt32(middleChar[5].ToString()) * num6) +
                        (Convert.ToInt32(middleChar[6].ToString()) * num7);
                    if (firstChar == "G")
                    {
                        sum = sum + addnum;

                    }
                    result = sum % 11;
                    result = 11 - result;

                    if (firstChar == "F" || firstChar == "G")
                    {
                        switch (result)
                        {
                            case 1:
                                chkdigit = "K";
                                break;
                            case 2:
                                chkdigit = "L";
                                break;
                            case 3:
                                chkdigit = "M";
                                break;
                            case 4:
                                chkdigit = "N";
                                break;
                            case 5:
                                chkdigit = "P";
                                break;
                            case 6:
                                chkdigit = "Q";
                                break;
                            case 7:
                                chkdigit = "R";
                                break;
                            case 8:
                                chkdigit = "T";
                                break;
                            case 9:
                                chkdigit = "U";
                                break;
                            case 10:
                                chkdigit = "W";
                                break;
                            case 11:
                                chkdigit = "X";
                                break;
                        }

                    }
                    if (chkdigit == lastChar)
                    {
                        bIsNum = true;
                    }
                }

            }
            return bIsNum;
        }

        public bool IsDecimal(string sText)
        {
            bool bIsNum = false;
            if (double.TryParse(sText, out double dTemp))
            {
                bIsNum = true;
            }

            return bIsNum;
        }

        public bool IsValidFileExtension(string fileExtension)
        {
            bool bIsValid = false;

            if (fileExtension.ToUpper().Equals("JPG") || fileExtension.ToUpper().Equals("PDF"))
            {
                bIsValid = true;
            }
            else
            {
                bIsValid = false;
            }

            return bIsValid;
        }

        public void BindRecordToListView(ListView listViewControl, DataSet dataSet, string sessionKey)
        {
            try
            {
                AppSession.SetSession(sessionKey, dataSet);
                listViewControl.DataSource = dataSet;
                listViewControl.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetIncidentTypeListByID(string incidentId)
        {
            string incidentTypeList = string.Empty;
            try
            {
                incidentTypeList = CommonFunDAL.GetIncidentTypeListByID(incidentId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return incidentTypeList;
        }






        public double MegabytesToBytes(long byteCount)
        {
            double byteReturn = 0;
            if (byteCount == 0)
            {
                byteReturn = 0;
            }
            else
            {
                byteReturn = byteCount * 1048576;
            }
            return byteReturn;
        }

        public string RemoveSpecialCharacters(string str)
        {
            string replacement = Regex.Replace(str, "/[\x00-\x1F\x7F]/u", "", RegexOptions.Compiled);

            replacement = Regex.Replace(str, @"[\n\u000B\u000C\r\u0085\u2028\u2029]", "");
            return replacement;
        }
        public bool sso_sunburstconnect(string value, string key, string digest_value)
        {
            bool isvalid = false;
            try
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = Encoding.UTF8.GetBytes(value);

                byte[] keyByte = Encoding.UTF8.GetBytes(key);

                // Initialize a SHA512 hash object. with the key
                HMACSHA512 hmac = new HMACSHA512(keyByte);
                byte[] computedHash = hmac.ComputeHash(data);
                string returnValue = Convert.ToBase64String(computedHash);

                if (returnValue.Equals(digest_value))
                {
                    isvalid = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isvalid;
        }
        public string Encrypt(string value, string passwordHash, string key, string viKey)
        {
            string returnValue = string.Empty;
            try
            {
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(value);

                byte[] keyBytes = new Rfc2898DeriveBytes(passwordHash, Encoding.ASCII.GetBytes(key)).GetBytes(256 / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(viKey));

                byte[] cipherTextBytes;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        cipherTextBytes = memoryStream.ToArray();
                        cryptoStream.Close();
                    }
                    memoryStream.Close();
                }

                returnValue = Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        public string Decrypt(string value, string passwordHash, string key, string viKey)
        {
            string returnValue = string.Empty;
            try
            {
                byte[] cipherTextBytes = Convert.FromBase64String(value.Replace(' ', '+'));
                byte[] keyBytes = new Rfc2898DeriveBytes(passwordHash, Encoding.ASCII.GetBytes(key)).GetBytes(256 / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(viKey));
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();

                returnValue = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        public void BindRecordToLiteral(Literal LiteralControl, string tc, string sessionKey)
        {
            try
            {
                AppSession.SetSession(sessionKey, tc);
                LiteralControl.Text = tc;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SendToMailMessage(string from, string to, string subject, string body, bool isHtmlBody, MailPriority priority, out string errorCode)
        {
            errorCode = string.Empty;
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(from)
                };
                mailMessage.To.Add(new MailAddress(to));

                string emailITsupport = ConfigurationManager.AppSettings["ITSupportEmail"].ToString();
                if (!string.IsNullOrEmpty(emailITsupport))
                {
                    mailMessage.Bcc.Add(emailITsupport);
                }

                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isHtmlBody;
                mailMessage.Priority = priority;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch
            {
                errorCode = "ERR-111";
            }
            finally
            {
                // This is to let proceed sending email    
            }
        }

        public void SendMailMessageSBU(string from, string to, DataSet cc, string bcc, string subject, string body, bool isHtmlBody, MailPriority priority, out string errorCode)
        {
            errorCode = string.Empty;
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(from)
                };
                //mailMessage.To.Add(new MailAddress(to));

                if (!string.IsNullOrEmpty(to))
                {
                    mailMessage.To.Add(new MailAddress(to));
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    mailMessage.Bcc.Add(new MailAddress(bcc));
                }
                string emailITsupport = ConfigurationManager.AppSettings["ITSupportEmail"].ToString();
                if (!string.IsNullOrEmpty(emailITsupport))
                {
                    mailMessage.Bcc.Add(emailITsupport);
                }

                if (cc != null)
                {
                    if (cc.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow email in cc.Tables[0].Rows)
                        {
                            mailMessage.CC.Add(new MailAddress(email["email_address"].ToString()));
                        }
                    }
                }

                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isHtmlBody;
                mailMessage.Priority = priority;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch
            {
                errorCode = "ERR-111";
            }
            finally
            {
                // This is to let proceed sending email    
            }
        }


        public void SendMailMessage(string from, DataSet to, DataSet cc, string bcc, string subject, string body, bool isHtmlBody, MailPriority priority, out string errorCode)
        {
            errorCode = string.Empty;
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(from)
                };

                if (to != null)
                {
                    if (to.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow email in to.Tables[0].Rows)
                        {
                            mailMessage.To.Add(new MailAddress(email["email_address"].ToString()));
                        }
                    }
                }

                if (cc != null)
                {
                    if (cc.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow email in cc.Tables[0].Rows)
                        {
                            mailMessage.CC.Add(new MailAddress(email["email_address"].ToString()));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(bcc))
                {
                    mailMessage.Bcc.Add(new MailAddress(bcc));
                }
                string emailITsupport = ConfigurationManager.AppSettings["ITSupportEmail"].ToString();
                if (!string.IsNullOrEmpty(emailITsupport))
                {
                    mailMessage.Bcc.Add(emailITsupport);
                }

                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isHtmlBody;
                mailMessage.Priority = priority;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch
            {
                errorCode = "ERR-111";
            }
            finally
            {
                // This is to let proceed sending email    
            }
        }

        public void SendToListMailMessage(string from, DataSet to, string subject, string body, bool isHtmlBody, MailPriority priority, out string errorCode)
        {
            errorCode = string.Empty;
            try
            {
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress(from)
                };

                if (to != null)
                {
                    if (to.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow email in to.Tables[0].Rows)
                        {
                            mailMessage.To.Add(new MailAddress(email["email_address"].ToString()));
                        }
                    }
                }

                string emailITsupport = ConfigurationManager.AppSettings["ITSupportEmail"].ToString();
                if (!string.IsNullOrEmpty(emailITsupport))
                {
                    mailMessage.Bcc.Add(emailITsupport);
                }
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isHtmlBody;
                mailMessage.Priority = priority;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Send(mailMessage);
            }
            catch
            {
                errorCode = "ERR-111";
            }
            finally
            {
                // This is to let proceed sending email    
            }
        }


        public void GetYears(DropDownList dropDownListName)
        {
            DataSet ds = new DataSet();

            try
            {
                dropDownListName.Items.Clear();
                ds = CommonFunDAL.GetYears();
                dropDownListName.DataSource = ds;
                dropDownListName.DataTextField = "Year";
                dropDownListName.DataValueField = "Year";
                dropDownListName.DataBind();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        //10 March 2015
        public void GetCCEmailDistributionByEmailGroup(CheckBoxList chkboxlistName, string emailgroup)
        {
            DataSet ds = new DataSet();
            ds = CommonFunDAL.GetCCEmailDistributionByEmailGroup(emailgroup);
            chkboxlistName.DataSource = ds;
            chkboxlistName.DataTextField = "appointment";
            chkboxlistName.DataValueField = "emp_no";
            chkboxlistName.DataBind();
        }
        public DataSet get_config_data(string config_type)
        {
            return CommonFunDAL.get_config_data(config_type);
        }

        public DataSet get_UserInfo_by_userID(string userID)
        {
            return CommonFunDAL.get_UserInfo_by_userID(userID);
        }

        public static string GetHtmlContent(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result; // Blocking call to wait for the response.
                response.EnsureSuccessStatusCode();
                string htmlContent = response.Content.ReadAsStringAsync().Result; // Blocking call to get the content.
                return htmlContent;
            }
        }
    }
}