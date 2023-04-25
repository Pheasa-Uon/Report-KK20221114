using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Operation
{
    public partial class OneYearRenewal : System.Web.UI.Page
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
                dtpFromDate.Text = DataHelper.getSystemDateTextbox();
                dtpToDate.Text = DataHelper.getSystemDateTextbox();
            }
        }

        private void GenerateReport(DataTable DT)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection();
            reportParameters.Add(new ReportParameter("FromDate", DateTime.ParseExact(dtpFromDate.Text, format, null).ToString("dd-MMM-yyyy")));
            reportParameters.Add(new ReportParameter("ToDate", DateTime.ParseExact(dtpToDate.Text, format, null).ToString("dd-MMM-yyyy")));

            var ds = new ReportDataSource("OneYearRenewalDS", DT);
            DataHelper.generateOperationReport(ReportViewer1, "OneYearRenewal", reportParameters, ds);
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            try
            {
                fromDate = DateTime.ParseExact(dtpFromDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateFromError = "* Date wrong format";
                return;
            }
            try
            {
                toDate = DateTime.ParseExact(dtpToDate.Text.Trim(), format, null).ToString("yyyy-MM-dd");
            }
            catch (Exception)
            {
                dateToError = "* Date wrong format";
                return;
            }

            var sql = "ONE_YEAR_RENEWAL";
            List<Procedure> procedureList = new List<Procedure>();

            procedureList.Add(item: new Procedure() { field_name = "@pFRDT", sql_db_type = MySqlDbType.Date, value_name = fromDate });
            procedureList.Add(item: new Procedure() { field_name = "@pTODT", sql_db_type = MySqlDbType.Date, value_name = toDate });

            DataTable dt = db.getProcedureDataTable(sql, procedureList);
            GenerateReport(dt);
        }
    }
}