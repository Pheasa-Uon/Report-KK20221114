using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Operation
{
    public partial class ContractExpectedRedeem : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        public string format = "dd/MM/yyyy";
        public string fromDate, toDate;
        public string dateFromError = "", dateToError = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                dtpToDate.Text = DataHelper.getSystemDateTextbox();
                txtContract.Text = "";
            }
        }
        private void GenerateReport(DataTable DT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ContractNo", txtContract.Text.Trim()));

            var ds = new ReportDataSource("ContractExpectedRedeemDS", DT);
            DataHelper.generateOperationReport(ReportViewer1, "ContractExpectedRedeem", reportParameters, ds);
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                toDate = DateTime.ParseExact(dtpToDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateToError = "* Date wrong format";
                return;
            }

            var sql = "Contract_Expected_Redeem";
            List<Procedure> procedureList = new List<Procedure>();

            // procedureList.Add(item: new Procedure() { field_name = "@pFRDT", sql_db_type = MySqlDbType.Date, value_name = fromDate });
            procedureList.Add(item: new Procedure() { field_name = "@pTODT", sql_db_type = MySqlDbType.Date, value_name = toDate });
            procedureList.Add(item: new Procedure() { field_name = "@pACNO", sql_db_type = MySqlDbType.VarChar, value_name = txtContract.Text });

            DataTable dt = db.getProcedureDataTable(sql, procedureList);
            GenerateReport(dt);
        }
    }
}