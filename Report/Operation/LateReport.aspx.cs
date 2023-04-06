using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using Report.Utils;
using Report.Models;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace Report.Operation
{
    public partial class LateReport : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                DataHelper.populateBranchDDL(ddBranchName, DataHelper.getUserId());
                populateOfficer();
            }
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

        private void GenerateReport(DataTable lateReportDT, DataTable summaryDT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("Branch", ddBranchName.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("PawnOfficer", ddOfficer.SelectedItem.Text));
            reportParameters.Add(new ReportParameter("SystemDate", DataHelper.getSystemDateStr()));

            var lateDS = new ReportDataSource("LateReportDS", lateReportDT);
            var summaryDS = new ReportDataSource("SummaryDS", summaryDT);

            DataHelper.generateOperationReport(ReportViewer1, "LateReport", reportParameters, lateDS, summaryDS);

        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            var dayFormat = DataHelper.getSystemDate().ToString("yyyy-MM-dd");

            var lateReportSql = "PS_LATE_REPORT";
            
            var spd = "PS_BRANCH_PROD_TEST";

            List<Procedure> parameters = new List<Procedure>();
            parameters.Add(item: new Procedure() { field_name = "@pBranch", sql_db_type = MySqlDbType.VarChar, value_name = ddBranchName.SelectedItem.Value });
            parameters.Add(item: new Procedure() { field_name = "@pSystem_date", sql_db_type = MySqlDbType.VarChar, value_name = dayFormat });
            parameters.Add(item: new Procedure() { field_name = "@pCurrency", sql_db_type = MySqlDbType.VarChar, value_name = "2" });
            parameters.Add(item: new Procedure() { field_name = "@PawnOfficer", sql_db_type = MySqlDbType.VarChar, value_name = ddOfficer.SelectedItem.Value });

            DataTable lateReportDT = db.getProcedureDataTable(lateReportSql, parameters);
            DataTable summaryDT = db.getProcedureDataTable(spd, parameters);
            
            GenerateReport(lateReportDT, summaryDT);
        }
    }
}