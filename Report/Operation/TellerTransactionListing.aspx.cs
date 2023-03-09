using Microsoft.Reporting.WebForms;
using Report.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Report.Utils;
using MySql.Data.MySqlClient;

namespace Report.Operation
{
    public partial class TellerTransactionListing : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        static List<Currency> currencyList;
        public static string fromDate, systemDateStr;
        public string format = "dd/MM/yyyy";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                systemDateStr = DataHelper.getSystemDate().ToString("dd/MM/yyyy");
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
                dtpFromDate.Text = systemDateStr;
                dtpToDate.Text = systemDateStr;
                
                currencyList = DataHelper.populateCurrencyDDL(ddCurrency);
              
            }
        }

        private void GenerateReport(DataTable dt)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("PawnOfficer", ddOfficer.SelectedItem.Text));
            if (dtpFromDate.Text != "")
            {
                reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            }
            else
            {
                reportParameters.Add(new ReportParameter("FromDate", " "));
            }
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("Currency", ddCurrency.SelectedItem.Text));

            var ds = new ReportDataSource("TellerTransactionListing", dt);

            DataHelper.generateOperationReport(ReportViewer1, "TellerTransactionListing", reportParameters, ds);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            
            var fromDate = DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("yyyy-MM-dd");
            var toDate = DateTime.ParseExact(dtpToDate.Text, format, null).ToString("yyyy-MM-dd");

            string officer = null;
            if (ddOfficer.SelectedItem.Value != "0")
            {
                officer = ddOfficer.SelectedItem.Value;
            }

            var spd = "Teller_Transaction_Listing";
            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@currencyId", sql_db_type = MySqlDbType.VarChar, value_name = ddCurrency.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@pFromDate", sql_db_type = MySqlDbType.Date, value_name = fromDate });
            parameters.Add(item: new Procedure() { field_name = "@pToDate", sql_db_type = MySqlDbType.Date, value_name = toDate });
            parameters.Add(item: new Procedure() { field_name = "@pOfficer", sql_db_type = MySqlDbType.VarChar, value_name = officer });

            DataTable dt = db.getProcedureDataTable(spd, parameters);
            GenerateReport(dt);
        }

        protected void ddBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateOfficer();
        }


        private void populateOfficer()
        {
            if (ddBranchName.SelectedItem.Value != "")
            {
                if (ddBranchName.SelectedItem.Value == "ALL")
                {
                    ddOfficer.Enabled = true;
                    DataHelper.populateOfficerDDLAll(ddOfficer);
                }
                else
                {
                    ddOfficer.Enabled = true;
                    DataHelper.populateOfficerDDL(ddOfficer, Convert.ToInt32(ddBranchName.SelectedItem.Value));
                }

            }
            else
            {
                ddOfficer.Enabled = false;
                ddOfficer.Items.Clear();
            }
        }
    }
}