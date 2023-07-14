using Microsoft.Reporting.WebForms;
using MySql.Data.MySqlClient;
using Report.Models;
using Report.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Report.Operation
{
    public partial class ContractandCollateralDetailsofCustomer : System.Web.UI.Page
    {
        private DBConnect db = new DBConnect();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataHelper.checkLoginSession();
                txtCustomer.Text = "";
            }
        }
        private void GenerateReport(DataTable ps_cus_dt, DataTable ps_crd_dt, DataTable ps_col_dt, DataTable ps_dpt_dt)
        {
            ReportParameterCollection reportParameters = new ReportParameterCollection
            {
                new ReportParameter("txtCustomer", txtCustomer.Text)
            };

            var ps_cus_ds = new ReportDataSource("CustomerDS", ps_cus_dt);
            var ps_crd_ds = new ReportDataSource("LoanDS", ps_crd_dt);
            var ps_col_ds = new ReportDataSource("CollateralDS", ps_col_dt);
            var ps_dpt_ds = new ReportDataSource("DepositDS", ps_dpt_dt);

            DataHelper.generateOperationReport(ReportViewer1, "ContractandCollateralDetailsofCustomer", reportParameters, ps_cus_ds, ps_crd_ds, ps_col_ds, ps_dpt_ds);
        }
        protected void btnView_Click(object sender, EventArgs e)
        {

            var ps_cus = "CUSTOMER_DETAILS";
            var ps_crd = "CUSTOMER_DETAILS_LOAN";
            var ps_col = "CUSTOMER_DETAILS_COLLATERAL";
            var ps_dpt = "CUSTOMER_DETAILS_DEPOSIT";


            List<Procedure> procedureList = new List<Procedure>();
            procedureList.Add(item: new Procedure() { field_name = "@pCUSNO", sql_db_type = MySqlDbType.VarChar, value_name = txtCustomer.Text });

            DataTable ps_cus_dt = db.getProcedureDataTable(ps_cus, procedureList);
            DataTable ps_crd_dt = db.getProcedureDataTable(ps_crd, procedureList);
            DataTable ps_col_dt = db.getProcedureDataTable(ps_col, procedureList);
            DataTable ps_dpt_dt = db.getProcedureDataTable(ps_dpt, procedureList);


            GenerateReport(ps_cus_dt, ps_crd_dt, ps_col_dt, ps_dpt_dt);
        }
    }
}