﻿using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.DAL;
using SQS.nTier.TTM.GenericFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SQS.nTier.TTM.DataImport
{
    public partial class frmTTMDataImport : Form
    {
        #region private variables

        private readonly string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        private readonly string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public frmTTMDataImport()
        {
            InitializeComponent();
        }

        /// <summary>
        /// btnSelectFile_Click
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialogTTM.ShowDialog();

            if (!String.IsNullOrEmpty(openFileDialogTTM.FileName))
            {
                txtSelectFile.Text = openFileDialogTTM.FileName;
            }
        }

        /// <summary>
        /// btnClear_Click
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSelectFile.Text = string.Empty;
            lblTSRCount.Text = "#";
            lblTSOCount.Text = "#";
            lblTaskCount.Text = "#";
            grpReport.Text = "Report (# of #)";
        }

        private void openFileDialogTTM_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(!String.IsNullOrEmpty(openFileDialogTTM.FileName) &&
                (openFileDialogTTM.FileName.IndexOf(".xls") > 0 ||
                openFileDialogTTM.FileName.IndexOf(".xlsx") > 0)))
            {
                MessageBox.Show("Please select a valid excel file", "Select valid excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// btnStart_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(openFileDialogTTM.FileName))
                {
                    string strFileExtention = Path.GetExtension(txtSelectFile.Text);
                    string strExcelConnectionString = String.Empty;

                    strExcelConnectionString = string.Empty;
                    switch (strFileExtention)
                    {

                        case ".xls": //Excel 97-03
                            strExcelConnectionString = string.Format(Excel03ConString, txtSelectFile.Text);
                            break;

                        case ".xlsx": //Excel 07 to later
                            strExcelConnectionString = string.Format(Excel07ConString, txtSelectFile.Text);
                            break;
                    }

                    AppSettingsReader settingsReader = new AppSettingsReader();
                    string strTables = (string)settingsReader.GetValue("tables", typeof(String));
                    String[] strTablesCollection = strTables.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    DataTable dt = null;

                    List<TSR> objTSRList = new List<TSR>();
                    List<TSO> objTSOList = new List<TSO>();

                    //Read Data from the Sheet.
                    using (OleDbConnection con = new OleDbConnection(strExcelConnectionString))
                    {
                        foreach (string strTable in strTablesCollection)
                        {
                            lblStatus.Text = String.Format("Fetching data from {0} sheet.", strTable);
                            Application.DoEvents();
                            using (OleDbCommand cmd = new OleDbCommand())
                            {
                                using (OleDbDataAdapter oda = new OleDbDataAdapter())
                                {
                                    dt = new DataTable();

                                    string strQuery = TableColumns.GetExcelColumns(strTable);

                                    strQuery = String.Format("Select {0} FROM [{1}$]", strQuery, strTable);

                                    cmd.CommandText = strQuery;
                                    cmd.Connection = con;
                                    con.Open();
                                    oda.SelectCommand = cmd;
                                    oda.Fill(dt);
                                    con.Close();
                                }
                            }

                            //Save the data Database
                            if (dt != null)
                            {
                                int iterationCount = 0;
                                if (strTable == "TSR")
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (dr["ID"] != null && dr["ID"] != DBNull.Value)
                                            {
                                                iterationCount++;
                                                grpReport.Text = String.Format("Report (Processing {0} of {1})", iterationCount, dt.Rows.Count.ToString());
                                                Application.DoEvents();

                                                int ClientId = 0;
                                                int TSRStatusID = 0;
                                                int EngagementId = 0;
                                                int PricingModelId = 0;
                                                int ProjectModelID = 0;
                                                int ClientRegionId = 0;
                                                int OperationalRiskId = 0;
                                                int VerticalId = 0;
                                                int PracticeId = 0;
                                                int MarketOfferingId = 0;
                                                int SolutionCentreId = 0;
                                                int DeliveryManagerId = 0;
                                                int TestManagerId = 0;
                                                int AccountManagerId = 0;

                                                int SP_Id = int.Parse(dr["ID"].ToString());
                                                String Title = dr["Title"].ToString();
                                                String strClient = dr["Client"].ToString();
                                                String strTSRStatus = dr["TSR Status"].ToString();
                                                String strEngagementModel = dr["Engagement Model"] == null ? "N.A." : dr["Engagement Model"].ToString();
                                                String strPricingModel = dr["Pricing Model"] == null ? "N.A." : dr["Pricing Model"].ToString();
                                                String strProjectModel = dr["Project Model"] == null ? "N.A." : dr["Project Model"].ToString();
                                                String strClientRegion = dr["Client Region"] == null ? "N.A." : dr["Client Region"].ToString();
                                                String strAccount = dr["Account"] == null ? "N.A." : dr["Account"].ToString();
                                                String ERPordernumber = dr["ERP order number"].ToString();
                                                String strSolutioncentre = dr["Solution centre"] == null ? "N.A." : dr["Solution centre"].ToString();
                                                String strCoreServices = dr["Core Service (s)"].ToString();
                                                float Estimatedeffort = dr["Estimated effort"] == null ? 0f : float.Parse(dr["Estimated effort"].ToString());
                                                float Plannedeffort = dr["Planned effort"] == null ? 0f : float.Parse(dr["Planned effort"].ToString());
                                                float Actualeffort = dr["Actual effort"] == null ? 0f : float.Parse(dr["Planned effort"].ToString());
                                                int Operationalrisk = 0;

                                                if (dr["Operational risk"] == null)
                                                {
                                                    Operationalrisk = 0;
                                                }
                                                else if (dr["Operational risk"] == DBNull.Value)
                                                {
                                                    Operationalrisk = 0;
                                                }
                                                else
                                                {
                                                    Operationalrisk = int.Parse(dr["Operational risk"].ToString());
                                                }

                                                var StartDate = (DateTime)dr["Start Date"];
                                                var TargetCompletionDate = (DateTime)dr["Target Completion Date"];

                                                String strAccountManager = dr["Account Manager"] == null ? "Omkar Damle" : dr["Account Manager"].ToString();
                                                String strDeliverymanager = dr["Delivery manager"] == null ? "Omkar Damle" : dr["Delivery Manager"].ToString();
                                                String strTestManager = dr["Test Manager"] == null ? "Omkar Damle" : dr["Test Manager"].ToString();
                                                String strVertical = dr["Vertical"] == null ? "N.A." : dr["Vertical"].ToString();
                                                String strPractice = dr["Practice"] == null ? "N.A." : dr["Practice"].ToString();
                                                String strMarketOffering = dr["Market Offering"] == null ? "N.A." : dr["Market Offering"].ToString().Trim() == "" ? "N.A." : dr["Market Offering"].ToString();
                                                String strRelevantrepositories = dr["Relevant repositories"] == null ? "N.A." : dr["Relevant repositories"].ToString();

                                                if (strRelevantrepositories == "")
                                                {
                                                    strRelevantrepositories = "N.A.;";
                                                }
                                                if (strEngagementModel == "")
                                                {
                                                    strEngagementModel = "N.A.";
                                                }
                                                if (strPricingModel == "")
                                                {
                                                    strPricingModel = "N.A.";
                                                }
                                                if (strProjectModel == "")
                                                {
                                                    strProjectModel = "N.A.";
                                                }
                                                if (strProjectModel == "")
                                                {
                                                    strProjectModel = "N.A.";
                                                }
                                                if (strClientRegion == "")
                                                {
                                                    strClientRegion = "N.A.";
                                                }
                                                if (strAccount == "")
                                                {
                                                    strAccount = "N.A.";
                                                }
                                                if (strSolutioncentre == "")
                                                {
                                                    strSolutioncentre = "N.A.";
                                                }
                                                if (strCoreServices == "")
                                                {
                                                    strCoreServices = "N.A.";
                                                }
                                                if (strVertical == "")
                                                {
                                                    strVertical = "N.A.";
                                                }
                                                if (strPractice == "")
                                                {
                                                    strPractice = "N.A.";
                                                }
                                                if (strMarketOffering == "")
                                                {
                                                    strMarketOffering = "N.A.";
                                                }

                                                var Modified = (DateTime)dr["Modified"];
                                                String strModifiedBy = dr["Modified By"].ToString();
                                                String strCreatedBy = dr["Created By"].ToString();
                                                var Created = (DateTime)dr["Created"];

                                                String strDescription = dr["Description"] == null ? "" : dr["Description"].ToString();

                                                AccountManagerId = GetUser(strAccountManager);

                                                if (AccountManagerId == 0)
                                                {
                                                    AccountManagerId = GetUser("Omkar Damle");
                                                }

                                                DeliveryManagerId = GetUser(strDeliverymanager);

                                                if (DeliveryManagerId == 0)
                                                {
                                                    DeliveryManagerId = GetUser("Omkar Damle");
                                                }

                                                TestManagerId = GetUser(strTestManager);

                                                if (TestManagerId == 0)
                                                {
                                                    TestManagerId = GetUser("Omkar Damle");
                                                }

                                                LoginSession ls = new LoginSession();
                                                IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                                                Client objClient = objBusinessLayer.ClientRepository.GetSingle(x => x.Name.ToLower() == strClient.ToLower());

                                                if (null != objClient)
                                                {
                                                    ClientId = objClient.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objClient = new Client { Name = strClient, Description = strClient, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.ClientRepository.Add(objClient);
                                                    ClientId = objClient.ID;
                                                }

                                                if (String.IsNullOrWhiteSpace(strTSRStatus))
                                                {
                                                    strTSRStatus = "On Hold";
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                TSRStatus objTSRStatus = objBusinessLayer.TSRStatusRepository.GetSingle(x => x.Name.ToLower() == strTSRStatus.ToLower());

                                                if (null != objTSRStatus)
                                                {
                                                    TSRStatusID = objTSRStatus.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                Engagement objEngagement = objBusinessLayer.EngagementRepository.GetSingle(x => x.Name.ToLower() == strEngagementModel.ToLower());

                                                if (null != objEngagement)
                                                {
                                                    EngagementId = objEngagement.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objEngagement = new Engagement { Name = strEngagementModel, Description = strEngagementModel, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.EngagementRepository.Add(objEngagement);
                                                    EngagementId = objEngagement.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                PricingModel objPricingModel = objBusinessLayer.PricingModelRepository.GetSingle(x => x.Name.ToLower() == strPricingModel.ToLower());

                                                if (null != objPricingModel)
                                                {
                                                    PricingModelId = objPricingModel.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objPricingModel = new PricingModel { Name = strPricingModel, Description = strPricingModel, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.PricingModelRepository.Add(objPricingModel);
                                                    PricingModelId = objPricingModel.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                ProjectModel objProjectModel = objBusinessLayer.ProjectModelRepository.GetSingle(x => x.Name.ToLower() == strProjectModel.ToLower());

                                                if (null != objProjectModel)
                                                {
                                                    ProjectModelID = objProjectModel.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objProjectModel = new ProjectModel { Name = strProjectModel, Description = strProjectModel, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.ProjectModelRepository.Add(objProjectModel);
                                                    ProjectModelID = objProjectModel.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                ClientRegion objClientRegion = objBusinessLayer.ClientRegionRepository.GetSingle(x => x.Name.ToLower() == strClientRegion.ToLower());

                                                if (null != objClientRegion)
                                                {
                                                    ClientRegionId = objClientRegion.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objClientRegion = new ClientRegion { Name = strClientRegion, Description = strClientRegion, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.ClientRegionRepository.Add(objClientRegion);
                                                    ClientRegionId = objClientRegion.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                OperationalRisk objOperationalRisk = objBusinessLayer.OperationalRiskRepository.GetSingle(x => x.RiskNo == Operationalrisk);

                                                if (null != objOperationalRisk)
                                                {
                                                    OperationalRiskId = objOperationalRisk.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objOperationalRisk = new OperationalRisk { RiskNo = Operationalrisk, Description = Operationalrisk.ToString(), CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.OperationalRiskRepository.Add(objOperationalRisk);
                                                    OperationalRiskId = objOperationalRisk.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                Vertical objVertical = objBusinessLayer.VerticalRepository.GetSingle(x => x.Name.ToLower() == strVertical.ToLower());

                                                if (null != objVertical)
                                                {
                                                    VerticalId = objVertical.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objVertical = new Vertical { Name = strVertical, Description = strVertical, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.VerticalRepository.Add(objVertical);
                                                    VerticalId = objVertical.ID;
                                                }
                                                objBusinessLayer = new BusinessLayer(ls);
                                                Practice objPractice = objBusinessLayer.PracticeRepository.GetSingle(x => x.Name.ToLower() == strPractice.ToLower());

                                                if (null != objPractice)
                                                {
                                                    PracticeId = objPractice.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objPractice = new Practice { Name = strPractice, Description = strPractice, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.PracticeRepository.Add(objPractice);
                                                    PracticeId = objPractice.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                MarketOffering objMarketOffering = objBusinessLayer.MarketOfferingRepository.GetSingle(x => x.Name.ToLower() == strMarketOffering.ToLower());

                                                if (null != objMarketOffering)
                                                {
                                                    MarketOfferingId = objMarketOffering.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objMarketOffering = new MarketOffering { Name = strMarketOffering, Description = strMarketOffering, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.MarketOfferingRepository.Add(objMarketOffering);
                                                    MarketOfferingId = objMarketOffering.ID;
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                SolutionCentre objSolutionCentre = objBusinessLayer.SolutionCentreRepository.GetSingle(x => x.Name.ToLower() == strSolutioncentre.ToLower());

                                                if (null != objSolutionCentre)
                                                {
                                                    SolutionCentreId = objSolutionCentre.ID;
                                                }
                                                else
                                                {
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    objSolutionCentre = new SolutionCentre { Name = strSolutioncentre, Description = strSolutioncentre, CreatedBy = strCreatedBy, CreatedOn = Created, UpdatedBy = strCreatedBy, UpdatedOn = Created, Version = 1, ObjectSate = ObjectSate.Manual };
                                                    objBusinessLayer.SolutionCentreRepository.Add(objSolutionCentre);
                                                    SolutionCentreId = objSolutionCentre.ID;
                                                }

                                                ls.LoginName = strModifiedBy;

                                                TSR objTSR = null;

                                                objBusinessLayer = new BusinessLayer(ls);
                                                objTSR = objBusinessLayer.TSRRepository.GetSingle(x => x.SP_Id == SP_Id);

                                                objBusinessLayer = new BusinessLayer(ls);

                                                if (null == objTSR)
                                                {
                                                    objTSR = new TSR
                                                    {
                                                        Title = Title,
                                                        SP_Id = SP_Id,
                                                        DeliveryManagerId = DeliveryManagerId,
                                                        Description = strDescription,
                                                        TestManagerId = TestManagerId,
                                                        VerticalId = VerticalId,
                                                        PracticeId = PracticeId,
                                                        SolutionCentreId = SolutionCentreId,
                                                        ClientRegionId = ClientRegionId,
                                                        ClientId = ClientId,
                                                        Account = strAccount,
                                                        EngagementId = EngagementId,
                                                        AccountManagerId = AccountManagerId,
                                                        ERPordernumber = ERPordernumber,
                                                        MarketOfferingId = MarketOfferingId,
                                                        StartDate = StartDate,
                                                        TargetCompletionDate = TargetCompletionDate,
                                                        Estimatedeffort = Estimatedeffort,
                                                        Plannedeffort = Plannedeffort,
                                                        OperationalRiskId = OperationalRiskId,
                                                        TSRStatusID = TSRStatusID,
                                                        ProjectModelID = ProjectModelID,
                                                        ERPOrderDescription = "",
                                                        PricingModelId = PricingModelId,
                                                        CreatedBy = strCreatedBy,
                                                        CreatedOn = Created,
                                                        UpdatedBy = strModifiedBy,
                                                        UpdatedOn = Modified,
                                                        Version = 1,
                                                        ObjectSate = ObjectSate.Manual
                                                    };

                                                    objBusinessLayer.TSRRepository.Add(objTSR);
                                                }
                                                else
                                                {
                                                    objTSR.Title = Title;
                                                    objTSR.SP_Id = SP_Id;
                                                    objTSR.DeliveryManagerId = DeliveryManagerId;
                                                    objTSR.Description = strDescription;
                                                    objTSR.TestManagerId = TestManagerId;
                                                    objTSR.VerticalId = VerticalId;
                                                    objTSR.PracticeId = PracticeId;
                                                    objTSR.SolutionCentreId = SolutionCentreId;
                                                    objTSR.ClientRegionId = ClientRegionId;
                                                    objTSR.ClientId = ClientId;
                                                    objTSR.Account = strAccount;
                                                    objTSR.EngagementId = EngagementId;
                                                    objTSR.AccountManagerId = AccountManagerId;
                                                    objTSR.ERPordernumber = ERPordernumber;
                                                    objTSR.MarketOfferingId = MarketOfferingId;
                                                    objTSR.StartDate = StartDate;
                                                    objTSR.TargetCompletionDate = TargetCompletionDate;
                                                    objTSR.Estimatedeffort = Estimatedeffort;
                                                    objTSR.Plannedeffort = Plannedeffort;
                                                    objTSR.OperationalRiskId = OperationalRiskId;
                                                    objTSR.TSRStatusID = TSRStatusID;
                                                    objTSR.ProjectModelID = ProjectModelID;
                                                    objTSR.ERPOrderDescription = "";
                                                    objTSR.PricingModelId = PricingModelId;
                                                    objTSR.CreatedBy = strCreatedBy;
                                                    objTSR.CreatedOn = Created;
                                                    objTSR.UpdatedBy = strModifiedBy;
                                                    objTSR.UpdatedOn = Modified;
                                                    objTSR.Version += 1;
                                                    objTSR.ObjectSate = ObjectSate.Manual;

                                                    objBusinessLayer.TSRRepository.Update(objTSR);
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                IList<TSRCoreService> objTSRCoreServiceList = objBusinessLayer.TSRCoreServicesRepository.GetList(x => x.TSRId == objTSR.ID);

                                                //Add Core Services
                                                if (!String.IsNullOrWhiteSpace(strCoreServices))
                                                {
                                                    String[] arrCoreServices = strCoreServices.Split(';');

                                                    if (arrCoreServices.Length > 0)
                                                    {
                                                        objTSR.TSRCoreServices = new List<TSRCoreService>();

                                                        int CoreServiceId = 0;

                                                        for (int i = 0; i < arrCoreServices.Length; i += 2)
                                                        {
                                                            var strCoreServiceName = arrCoreServices[i].ToLower().Replace("#", String.Empty);

                                                            objBusinessLayer = new BusinessLayer(ls);
                                                            CoreService objCoreService = objBusinessLayer.CoreServiceRepository.GetSingle(x => x.Name.ToLower() == strCoreServiceName);

                                                            if (null != objCoreService)
                                                            {
                                                                CoreServiceId = objCoreService.ID;

                                                                TSRCoreService objTSRCoreServiceExisting = null;

                                                                if (objTSRCoreServiceList != null)
                                                                {
                                                                    objTSRCoreServiceExisting = objTSRCoreServiceList.Where(x => x.CoreServiceId == CoreServiceId).FirstOrDefault();
                                                                }

                                                                if (objTSRCoreServiceExisting == null)
                                                                {
                                                                    TSRCoreService objTSRCoreServices = new TSRCoreService
                                                                    {
                                                                        TSRId = objTSR.ID,
                                                                        CoreServiceId = CoreServiceId,
                                                                        CreatedBy = strCreatedBy,
                                                                        CreatedOn = Created,
                                                                        UpdatedBy = strModifiedBy,
                                                                        UpdatedOn = Modified,
                                                                        Version = 1,
                                                                        ObjectSate = ObjectSate.Manual
                                                                    };


                                                                    objBusinessLayer = new BusinessLayer(ls);
                                                                    objBusinessLayer.TSRCoreServicesRepository.Add(objTSRCoreServices);

                                                                    objTSR.TSRCoreServices.Add(objTSRCoreServices);
                                                                }
                                                                else
                                                                {
                                                                    objTSR.TSRCoreServices.Add(objTSRCoreServiceExisting);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                objBusinessLayer = new BusinessLayer(ls);
                                                IList<TSRRelevantRepository> objTSRRelevantRepositoryList = objBusinessLayer.TSRRelevantRepositoriesRepository.GetList(x => x.TSRId == objTSR.ID);

                                                //Add Relevant repositories
                                                if (!String.IsNullOrWhiteSpace(strRelevantrepositories))
                                                {
                                                    if (!strRelevantrepositories.Contains(";"))
                                                    {
                                                        strRelevantrepositories += ";";
                                                    }
                                                    String[] arrRelevantrepositories = strRelevantrepositories.Split(';');

                                                    if (arrRelevantrepositories.Length > 0)
                                                    {
                                                        objTSR.TSRReleventRepositories = new List<TSRRelevantRepository>();
                                                        int RelevantRepositoryId = 0;

                                                        for (int i = 0; i < arrRelevantrepositories.Length; i += 2)
                                                        {
                                                            var strRelevantrepositoryName = arrRelevantrepositories[i].ToLower().Replace("#", String.Empty);

                                                            objBusinessLayer = new BusinessLayer(ls);
                                                            RelevantRepository objRelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetSingle(x => x.Name.ToLower() == strRelevantrepositoryName);

                                                            if (null != objRelevantRepository)
                                                            {
                                                                RelevantRepositoryId = objRelevantRepository.ID;

                                                                TSRRelevantRepository objTSRRelevantRepositoryExisting = null;

                                                                if (objTSRRelevantRepositoryExisting != null)
                                                                {
                                                                    objTSRRelevantRepositoryExisting = objTSRRelevantRepositoryList.Where(x => x.RelevantRepositoryId == RelevantRepositoryId).FirstOrDefault();
                                                                }

                                                                if (objTSRRelevantRepositoryExisting == null)
                                                                {
                                                                    TSRRelevantRepository objTSRRelevantRepository = new TSRRelevantRepository
                                                                    {
                                                                        TSRId = objTSR.ID,
                                                                        RelevantRepositoryId = RelevantRepositoryId,
                                                                        CreatedBy = strCreatedBy,
                                                                        CreatedOn = Created,
                                                                        UpdatedBy = strModifiedBy,
                                                                        UpdatedOn = Modified,
                                                                        Version = 1,
                                                                        ObjectSate = ObjectSate.Manual
                                                                    };


                                                                    objBusinessLayer = new BusinessLayer(ls);
                                                                    objBusinessLayer.TSRRelevantRepositoriesRepository.Add(objTSRRelevantRepository);

                                                                    objTSR.TSRReleventRepositories.Add(objTSRRelevantRepository);
                                                                }
                                                                else
                                                                {
                                                                    objTSR.TSRReleventRepositories.Add(objTSRRelevantRepositoryExisting);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                objTSRList.Add(objTSR);

                                                lblTSRCount.Text = iterationCount.ToString();
                                                Application.DoEvents();
                                            }
                                            else
                                            {
                                                grpReport.Text = String.Format("Report (Processed {0} of {1})", iterationCount, dt.Rows.Count.ToString());

                                                Application.DoEvents();
                                            }
                                        }

                                        lblStatus.Text = String.Format("{0}: {1} records inserted successfully.", strTable, dt.Rows.Count.ToString());
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                                else if (strTable == "TSO")
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (dr["ID"] != null && dr["ID"] != DBNull.Value)
                                            {
                                                iterationCount++;
                                                grpReport.Text = String.Format("Report (Processing {0} of {1})", iterationCount, dt.Rows.Count.ToString());
                                                Application.DoEvents();

                                                int TeamLeadId = 0;
                                                int TSRId = 0;
                                                int TSOStatusID = 0;
                                                int OperationalRiskId = 0;
                                                int CoreServiceId = 0;
                                                int RelevantRepositoryId = 0;

                                                int SP_Id = int.Parse(dr["ID"].ToString());
                                                int SP_TSRId = int.Parse(dr["Related TSR"].ToString());
                                                string Title = dr["Title"].ToString();
                                                string strDescription = dr["Description"] == null ? "" : dr["Description"].ToString();
                                                string strTeamLead = dr["Team Lead"] == null ? "Omkar Damle" : dr["Team Lead"].ToString();
                                                string strCoreService = dr["Core Service"] == null ? "N.A" : dr["Core Service"].ToString();
                                                string strRelevantrepositories = dr["Relevant repositories"] == null ? "N.A." : dr["Relevant repositories"].ToString();
                                                var StartDate = (DateTime)dr["Start Date"];
                                                var TargetCompletionDate = (DateTime)dr["Target Completion Date"];
                                                float Estimatedeffort = dr["Estimated effort"] == null ? 0f : float.Parse(dr["Estimated effort"].ToString());
                                                float Plannedeffort = dr["Planned effort"] == null ? 0f : float.Parse(dr["Planned effort"].ToString());
                                                int Operationalrisk = dr["Operational risk"] == DBNull.Value ? 0 : int.Parse(dr["Operational risk"].ToString());
                                                string strCreatedBy = dr["Created By"].ToString();
                                                var CreatedOn = (DateTime)dr["Created"];
                                                string strModifiedBy = dr["Modified By"].ToString();
                                                var ModifiedOn = (DateTime)dr["Modified"];
                                                string strTSOStatus = dr["TSO Status"].ToString();

                                                if (String.IsNullOrWhiteSpace(strTSOStatus))
                                                {
                                                    strTSOStatus = "On Hold";
                                                }

                                                if (String.IsNullOrWhiteSpace(strCoreService))
                                                {
                                                    strCoreService = "N.A.";
                                                }

                                                if (String.IsNullOrWhiteSpace(strRelevantrepositories))
                                                {
                                                    strRelevantrepositories = "N.A.";
                                                }

                                                //Service Delivery chain
                                                string strServiceDeliveryChain = dr["Service Delivery Chain"].ToString();

                                                if (String.IsNullOrWhiteSpace(strTeamLead))
                                                {
                                                    strTeamLead = "Omkar Damle";
                                                }

                                                TeamLeadId = GetUser(strTeamLead);

                                                if (TeamLeadId == 0)
                                                {
                                                    strTeamLead = "Omkar Damle";
                                                    TeamLeadId = GetUser(strTeamLead);
                                                }

                                                TSR objTSR = objTSRList.Where(x => x.SP_Id == SP_TSRId).FirstOrDefault<TSR>();

                                                if (objTSR != null)
                                                {
                                                    TSRId = objTSR.ID;

                                                    LoginSession ls = new LoginSession();
                                                    ls.LoginName = strModifiedBy;
                                                    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                                                    //Operational Risk
                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    OperationalRisk objOperationalRisk = objBusinessLayer.OperationalRiskRepository.GetSingle(x => x.RiskNo == Operationalrisk);

                                                    if (null != objOperationalRisk)
                                                    {
                                                        OperationalRiskId = objOperationalRisk.ID;
                                                    }

                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    TSOStatus obkTSOStatus = objBusinessLayer.TSOStatusRepository.GetSingle(x => x.Name.ToLower() == strTSOStatus.ToLower());

                                                    if (null != obkTSOStatus)
                                                    {
                                                        TSOStatusID = obkTSOStatus.ID;
                                                    }

                                                    //Core Service
                                                    if (!String.IsNullOrWhiteSpace(strCoreService))
                                                    {
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        CoreService objCoreService = objBusinessLayer.CoreServiceRepository.GetSingle(x => x.Name.ToLower() == strCoreService.ToLower());

                                                        if (null != objCoreService)
                                                        {
                                                            CoreServiceId = objCoreService.ID;

                                                        }
                                                    }

                                                    //Relevant repositories
                                                    if (!String.IsNullOrWhiteSpace(strRelevantrepositories))
                                                    {
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        RelevantRepository objRelevantRepository = objBusinessLayer.RelevantRepositoryRepository.GetSingle(x => x.Name.ToLower() == strRelevantrepositories.ToLower());

                                                        if (null != objRelevantRepository)
                                                        {
                                                            RelevantRepositoryId = objRelevantRepository.ID;
                                                        }
                                                    }

                                                    objBusinessLayer = new BusinessLayer(ls);

                                                    TSO objTSO = objBusinessLayer.TSORepository.GetSingle(x => x.SP_Id == SP_Id);

                                                    objBusinessLayer = new BusinessLayer(ls);

                                                    if (null == objTSO)
                                                    {
                                                        objTSO = new TSO
                                                        {
                                                            Title = Title,
                                                            SP_Id = SP_Id,
                                                            TSRId = TSRId,
                                                            Description = strDescription,
                                                            TeamLeadId = TeamLeadId,
                                                            CoreServiceId = CoreServiceId,
                                                            RelevantRepositoryId = RelevantRepositoryId,
                                                            StartDate = StartDate,
                                                            TargetCompletionDate = TargetCompletionDate,
                                                            EstimatedEffort = Estimatedeffort,
                                                            PlannedEffort = Plannedeffort,
                                                            OperationalRiskId = OperationalRiskId,
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            TSOStatusID = TSOStatusID,
                                                            ObjectSate = ObjectSate.Manual
                                                        };

                                                        objBusinessLayer.TSORepository.Add(objTSO);
                                                    }
                                                    else
                                                    {
                                                        objTSO.Title = Title;
                                                        objTSO.SP_Id = SP_Id;
                                                        objTSO.TSRId = TSRId;
                                                        objTSO.Description = strDescription;
                                                        objTSO.TeamLeadId = TeamLeadId;
                                                        objTSO.CoreServiceId = CoreServiceId;
                                                        objTSO.RelevantRepositoryId = RelevantRepositoryId;
                                                        objTSO.StartDate = StartDate;
                                                        objTSO.TargetCompletionDate = TargetCompletionDate;
                                                        objTSO.EstimatedEffort = Estimatedeffort;
                                                        objTSO.PlannedEffort = Plannedeffort;
                                                        objTSO.OperationalRiskId = OperationalRiskId;
                                                        objTSO.CreatedBy = strCreatedBy;
                                                        objTSO.CreatedOn = CreatedOn;
                                                        objTSO.UpdatedBy = strModifiedBy;
                                                        objTSO.UpdatedOn = ModifiedOn;
                                                        objTSO.Version += 1;
                                                        objTSO.TSOStatusID = TSOStatusID;

                                                        objTSO.ObjectSate = ObjectSate.Manual;

                                                        objBusinessLayer.TSORepository.Update(objTSO);
                                                    }

                                                    //Service Delivery chain
                                                    if (!String.IsNullOrWhiteSpace(strServiceDeliveryChain))
                                                    {
                                                        String[] arrServiceDeliveryChain = strServiceDeliveryChain.Split(';');

                                                        if (arrServiceDeliveryChain.Length > 0)
                                                        {
                                                            objTSO.TSOServiceDeliveryChains = new List<TSOServiceDeliveryChain>();

                                                            int ServiceDeliveryChainId = 0;

                                                            for (int i = 0; i < arrServiceDeliveryChain.Length; i++)
                                                            {
                                                                var strServiceDeliveryName = arrServiceDeliveryChain[i].ToLower().Replace("#", String.Empty);

                                                                objBusinessLayer = new BusinessLayer(ls);
                                                                ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetSingle(x => x.Name.ToLower() == strServiceDeliveryName || x.Description.ToLower() == strServiceDeliveryName);

                                                                objBusinessLayer = new BusinessLayer(ls);
                                                                IList<TSOServiceDeliveryChain> objTSOServiceDeliveryChainList = objBusinessLayer.TSOServiceDeliveryChainRepository.GetList(x => x.TSOId == objTSO.ID);


                                                                if (null != objServiceDeliveryChain)
                                                                {
                                                                    ServiceDeliveryChainId = objServiceDeliveryChain.ID;

                                                                    TSOServiceDeliveryChain objTSOServiceDeliveryChainExisting = null;

                                                                    if (null != objTSOServiceDeliveryChainList)
                                                                    {
                                                                        objTSOServiceDeliveryChainExisting = objTSOServiceDeliveryChainList.Where(x => x.ServiceDeliveryChainId == ServiceDeliveryChainId).FirstOrDefault();
                                                                    }

                                                                    if (null == objTSOServiceDeliveryChainExisting)
                                                                    {
                                                                        TSOServiceDeliveryChain objTSOServiceDeliveryChain = new TSOServiceDeliveryChain
                                                                        {
                                                                            TSOId = objTSO.ID,
                                                                            ServiceDeliveryChainId = ServiceDeliveryChainId,
                                                                            CreatedBy = strCreatedBy,
                                                                            CreatedOn = CreatedOn,
                                                                            UpdatedBy = strModifiedBy,
                                                                            UpdatedOn = ModifiedOn,
                                                                            Version = 1,
                                                                            ObjectSate = ObjectSate.Manual
                                                                        };

                                                                        objBusinessLayer = new BusinessLayer(ls);
                                                                        objBusinessLayer.TSOServiceDeliveryChainRepository.Add(objTSOServiceDeliveryChain);

                                                                        objTSO.TSOServiceDeliveryChains.Add(objTSOServiceDeliveryChain);
                                                                    }
                                                                    else
                                                                    {
                                                                        objTSO.TSOServiceDeliveryChains.Add(objTSOServiceDeliveryChainExisting);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }

                                                    objTSOList.Add(objTSO);

                                                    lblTSOCount.Text = iterationCount.ToString();
                                                    Application.DoEvents();
                                                }
                                            }
                                            else
                                            {
                                                grpReport.Text = String.Format("Report (Processed {0} of {1})", iterationCount, dt.Rows.Count.ToString());

                                                Application.DoEvents();
                                            }
                                        }

                                        lblStatus.Text = String.Format("{0}: {1} records inserted successfully.", strTable, dt.Rows.Count.ToString());
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                                else if (strTable == "Tasks")
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            if (dr["ID"] != null && dr["ID"] != DBNull.Value)
                                            {
                                                iterationCount++;
                                                grpReport.Text = String.Format("Report (Processing {0} of {1})", iterationCount, dt.Rows.Count.ToString());
                                                Application.DoEvents();

                                                int TSOId = 0;
                                                int TaskStatusID = 0;
                                                int? ServiceDeliveryChainMethodId = null;
                                                int TSOServiceDeliveryChainId = 0;
                                                int ServiceDeliveryChainId = 0;

                                                int SP_TSOId = int.Parse(dr["Related TSO"].ToString());
                                                string Title = dr["Title"].ToString();

                                                if (Title.ToLower() == "knowledge management")
                                                {
                                                    Title = "Knowledge Transfer";
                                                }

                                                int SP_TaskId = int.Parse(dr["ID"].ToString());
                                                string strNotes = dr["Notes"] == null ? "" : dr["Notes"] == DBNull.Value ? "" : dr["Notes"].ToString();
                                                var PlannedStartDate = (DateTime)(dr["Start Date"] == null ? DateTime.Now : dr["Start Date"] == DBNull.Value ? DateTime.Now : dr["Start Date"]);
                                                var PlannedCompletionDate = (DateTime)(dr["Target Completion Date"] == null ? DateTime.Now : dr["Target Completion Date"] == DBNull.Value ? DateTime.Now : dr["Target Completion Date"]);
                                                float EffortPlanned = dr["Planned effort"] == null ? 0 : dr["Planned effort"] == DBNull.Value ? 0 : float.Parse(dr["Planned effort"].ToString());
                                                float EffortActual = dr["Actual Effort"] == null ? 0 : dr["Actual Effort"] == DBNull.Value ? 0 : float.Parse(dr["Actual Effort"].ToString());
                                                var Actualstartdate = dr["Actual Start Date"] == null ? PlannedStartDate : dr["Actual Start Date"] == DBNull.Value ? PlannedStartDate : DateTime.Parse(dr["Actual Start Date"].ToString());
                                                var Actualcompletiondate = dr["Actual Completion Date"] == null ? PlannedCompletionDate : dr["Actual Completion Date"] == DBNull.Value ? PlannedCompletionDate : DateTime.Parse(dr["Actual Completion Date"].ToString());
                                                string strTaskStatus = dr["Task Status"] == null ? "Created" : dr["Task Status"] == DBNull.Value ? "Created" : dr["Task Status"].ToString();
                                                int PlannedOutcome = dr["Planned Outcome"] == null ? 0 : dr["Planned Outcome"] == DBNull.Value ? 0 : int.Parse(dr["Planned Outcome"].ToString());
                                                int ActualOutcome = dr["Actual Outcome"] == null ? 0 : dr["Actual Outcome"] == DBNull.Value ? 0 : int.Parse(dr["Actual Outcome"].ToString());
                                                int PlannedReviewRounds = dr["Planned Review Rounds"] == null ? 0 : dr["Planned Review Rounds"] == DBNull.Value ? 0 : int.Parse(dr["Planned Review Rounds"].ToString());
                                                int ActualReviewRounds = dr["Actual Review Rounds"] == null ? 0 : dr["Actual Review Rounds"] == DBNull.Value ? 0 : int.Parse(dr["Actual Review Rounds"].ToString());
                                                string strCreatedBy = dr["Created By"] == null ? "System" : dr["Created By"] == DBNull.Value ? "System" : dr["Created By"].ToString();
                                                var CreatedOn = (DateTime)(dr["Created"] == null ? DateTime.Now : dr["Created"] == DBNull.Value ? DateTime.Now : dr["Created"]);
                                                string strModifiedBy = dr["Modified By"] == null ? "System" : dr["Modified By"] == DBNull.Value ? "System" : dr["Modified By"].ToString();
                                                var ModifiedOn = (DateTime)(dr["Modified"] == null ? DateTime.Now : dr["Modified"] == DBNull.Value ? DateTime.Now : dr["Modified"]);

                                                int DefectRaised = dr["Defects Raised"] == null ? 0 : dr["Defects Raised"] == DBNull.Value ? 0 : int.Parse(dr["Defects Raised"].ToString());
                                                int DefectRejected = dr["Defects Rejected"] == null ? 0 : dr["Defects Rejected"] == DBNull.Value ? 0 : int.Parse(dr["Defects Rejected"].ToString());
                                                int PlannedInput = dr["Actual Outcome"] == null ? 0 : dr["Actual Outcome"] == DBNull.Value ? 0 : int.Parse(dr["Actual Outcome"].ToString());
                                                int ActualInput = dr["Actual Outcome"] == null ? 0 : dr["Actual Outcome"] == DBNull.Value ? 0 : int.Parse(dr["Actual Outcome"].ToString());

                                                float PlannedProcessingTime = dr["Planned Processing time"] == null ? 0 : dr["Planned Processing time"] == DBNull.Value ? 0 : float.Parse(dr["Planned Processing time"].ToString());
                                                float ActualProcessingTime = dr["Actual Processing Time"] == null ? 0 : dr["Actual Processing Time"] == DBNull.Value ? 0 : float.Parse(dr["Actual Processing Time"].ToString());
                                                float IdleTimeEffort = dr["Idle Time (Effort)"] == null ? 0 : dr["Idle Time (Effort)"] == DBNull.Value ? 0 : float.Parse(dr["Idle Time (Effort)"].ToString());
                                                float IdleTimeDuration = dr["Idle Time (Duration)"] == null ? 0 : dr["Idle Time (Duration)"] == DBNull.Value ? 0 : float.Parse(dr["Idle Time (Duration)"].ToString());
                                                float Headcount = dr["Headcount"] == null ? 0 : dr["Headcount"] == DBNull.Value ? 0 : float.Parse(dr["Headcount"].ToString());
                                                string strMethod = dr["Method"] == null ? "" : dr["Method"] == DBNull.Value ? "" : dr["Method"].ToString();
                                                int PlannedOutcomeTestSteps = dr["Planned Outcome (Test Steps)"] == null ? 0 : dr["Planned Outcome (Test Steps)"] == DBNull.Value ? 0 : int.Parse(dr["Planned Outcome (Test Steps)"].ToString());
                                                int ActualOutcomeTestSteps = dr["Actual Outcome (Test Steps)"] == null ? 0 : dr["Actual Outcome (Test Steps)"] == DBNull.Value ? 0 : int.Parse(dr["Actual Outcome (Test Steps)"].ToString());
                                                int WeekNumber = int.Parse(dr["Week Number"].ToString());
                                                TSO objTSO = objTSOList.Where(x => x.SP_Id == SP_TSOId).FirstOrDefault<TSO>();

                                                if (objTSO != null)
                                                {
                                                    TSOId = objTSO.ID;

                                                    LoginSession ls = new LoginSession();
                                                    ls.LoginName = strModifiedBy;
                                                    IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                                                    if (String.IsNullOrWhiteSpace(strTaskStatus))
                                                    {
                                                        strTaskStatus = "On Hold";
                                                    }

                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    TaskStatus objTaskStatus = objBusinessLayer.TaskStatusRepository.GetSingle(x => x.Name.ToLower() == strTaskStatus.ToLower());

                                                    if (null != objTaskStatus)
                                                    {
                                                        TaskStatusID = objTaskStatus.ID;
                                                    }

                                                    objBusinessLayer = new BusinessLayer(ls);
                                                    ServiceDeliveryChain objServiceDeliveryChain = objBusinessLayer.ServiceDeliveryChainRepository.GetSingle(x => x.Name.ToLower() == Title || x.Description.ToLower() == Title);

                                                    if (null != objServiceDeliveryChain)
                                                    {
                                                        ServiceDeliveryChainId = objServiceDeliveryChain.ID;
                                                    }

                                                    TSOServiceDeliveryChain objTSOServiceDeliveryChain = objTSO.TSOServiceDeliveryChains.Where(x => x.ServiceDeliveryChainId == ServiceDeliveryChainId).FirstOrDefault<TSOServiceDeliveryChain>();

                                                    if (objTSOServiceDeliveryChain != null)
                                                    {
                                                        TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID;
                                                    }

                                                    if (strMethod == "")
                                                    {
                                                        ServiceDeliveryChainMethodId = null;
                                                    }
                                                    else
                                                    {
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        ServiceDeliveryChainMethod objServiceDeliveryChainMethod = objBusinessLayer.MethodRepository.GetSingle(x => x.Name.ToLower() == strMethod.ToLower());

                                                        if (objServiceDeliveryChainMethod == null)
                                                        {
                                                            objServiceDeliveryChainMethod = new ServiceDeliveryChainMethod
                                                            {
                                                                Name = strMethod,
                                                                Description = strMethod,
                                                                ServiceDeliveryChainId = ServiceDeliveryChainId,
                                                                CreatedBy = strCreatedBy,
                                                                CreatedOn = CreatedOn,
                                                                UpdatedBy = strModifiedBy,
                                                                UpdatedOn = ModifiedOn,
                                                                Version = 1,
                                                                ObjectSate = ObjectSate.Manual,
                                                            };
                                                            objBusinessLayer = new BusinessLayer(ls);
                                                            objBusinessLayer.MethodRepository.Add(objServiceDeliveryChainMethod);
                                                        }


                                                        ServiceDeliveryChainMethodId = objServiceDeliveryChainMethod.ID;
                                                    }
                                                    if (TSOServiceDeliveryChainId != 0)
                                                    {
                                                        //TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask = new TSOServiceDeliveryChainTask
                                                        //{
                                                        //    //ActualEffort = EffortActual,
                                                        //    //ActualInput = ActualInput,
                                                        //    //ActualOutcome = ActualOutcome,
                                                        //    //ActualOutcomeTestSteps = ActualOutcomeTestSteps,
                                                        //    //ActualProcessingTime = ActualProcessingTime,
                                                        //    //ActualReviewRounds = ActualReviewRounds,
                                                        //    CreatedBy = strCreatedBy,
                                                        //    CreatedOn = CreatedOn,
                                                        //    //DefectRaised = DefectRaised,
                                                        //    //DefectRejected = DefectRejected,
                                                        //    //Headcount = Headcount,
                                                        //    //IdleTimeDuration = IdleTimeDuration,
                                                        //    //IdleTimeEffort = IdleTimeEffort,
                                                        //    Notes = strNotes,
                                                        //    PlannedCompletionDate = PlannedCompletionDate,
                                                        //    //PlannedEffort = EffortPlanned,
                                                        //    //PlannedInput = PlannedInput,
                                                        //    //PlannedOutcome = PlannedOutcome,
                                                        //    //PlannedOutcomeTestSteps = PlannedOutcomeTestSteps,
                                                        //    //PlannedProcessingTime = PlannedProcessingTime,
                                                        //    ObjectSate = ObjectSate.Manual,
                                                        //    //PlannedReviewRounds = PlannedReviewRounds,
                                                        //    PlannedStartDate = PlannedStartDate,
                                                        //    ServiceDeliveryChainMethodId = ServiceDeliveryChainMethodId,
                                                        //    TaskStatusId = TaskStatusID,
                                                        //    TSOServiceDeliveryChainId = TSOServiceDeliveryChainId,
                                                        //    UpdatedBy = strModifiedBy,
                                                        //    UpdatedOn = ModifiedOn,
                                                        //    Version = 1,
                                                        //   /* WeekNumber = WeekNumber*/ //GetIso8601WeekOfYear(PlannedStartDate)
                                                        //};

                                                        //objBusinessLayer = new BusinessLayer(ls);
                                                        //objBusinessLayer.TSOServiceDeliveryChainTaskRepository.Add(objTSOServiceDeliveryChainTask);

                                                        TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainTaskActual = new TSOServiceDeliveryChainTaskActual()
                                                        {
                                                            ActualEffort = EffortActual,
                                                            ActualInput = ActualInput,
                                                            ActualOutcome = ActualOutcome,
                                                            ActualOutcomeTestSteps = ActualOutcomeTestSteps,
                                                            ActualProcessingTime = ActualProcessingTime,
                                                            ActualReviewRounds = ActualReviewRounds,
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,
                                                            DefectRaised = DefectRaised,
                                                            DefectRejected = DefectRejected,
                                                            Headcount = Headcount,
                                                            IdleTimeDuration = IdleTimeDuration,
                                                            IdleTimeEffort = IdleTimeEffort,
                                                            ObjectSate = ObjectSate.Manual,
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            WeekNumber = WeekNumber,
                                                            Year = 2018,
                                                            TSOServiceDeliveryChainTaskId = 1,// objTSOServiceDeliveryChainTask.ID,
                                                        };

                                                        TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainTaskPlanned = new TSOServiceDeliveryChainTaskPlanned()
                                                        {
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,

                                                            PlannedEffort = EffortPlanned,
                                                            PlannedInput = PlannedInput,
                                                            PlannedOutcome = PlannedOutcome,
                                                            PlannedOutcomeTestSteps = PlannedOutcomeTestSteps,
                                                            PlannedProcessingTime = PlannedProcessingTime,
                                                            ObjectSate = ObjectSate.Manual,
                                                            PlannedReviewRounds = PlannedReviewRounds,
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            WeekNumber = WeekNumber,
                                                            Year = 2018,
                                                            TSOServiceDeliveryChainTaskId =1,// objTSOServiceDeliveryChainTask.ID,
                                                        };
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Add(objTSOServiceDeliveryChainTaskActual);
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainTaskPlanned);
                                                        //txtSkipped.Text = String.Format("{0} \r\n {1}", txtSkipped.Text, string.Format("ServiceDeliveryChain ={0} not found in TSO ={1}. SP Task Id={2}", objServiceDeliveryChain.Name, SP_TSOId, dr["ID"].ToString()));
                                                    }
                                                    else
                                                    {
                                                        objTSOServiceDeliveryChain = new TSOServiceDeliveryChain
                                                        {
                                                            TSOId = objTSO.ID,
                                                            ServiceDeliveryChainId = ServiceDeliveryChainId,
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            ObjectSate = ObjectSate.Manual
                                                        };

                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        objBusinessLayer.TSOServiceDeliveryChainRepository.Add(objTSOServiceDeliveryChain);

                                                        objTSO.TSOServiceDeliveryChains.Add(objTSOServiceDeliveryChain);

                                                        TSOServiceDeliveryChainId = objTSOServiceDeliveryChain.ID;

                                                        //TSOServiceDeliveryChainTask objTSOServiceDeliveryChainTask = new TSOServiceDeliveryChainTask
                                                        //{
                                                        //    //ActualEffort = EffortActual,
                                                        //    //ActualInput = ActualInput,
                                                        //    //ActualOutcome = ActualOutcome,
                                                        //    //ActualOutcomeTestSteps = ActualOutcomeTestSteps,
                                                        //    //ActualProcessingTime = ActualProcessingTime,
                                                        //    //ActualReviewRounds = ActualReviewRounds,
                                                        //    CreatedBy = strCreatedBy,
                                                        //    CreatedOn = CreatedOn,
                                                        //    //DefectRaised = DefectRaised,
                                                        //    //DefectRejected = DefectRejected,
                                                        //    //Headcount = Headcount,
                                                        //    //IdleTimeDuration = IdleTimeDuration,
                                                        //    //IdleTimeEffort = IdleTimeEffort,
                                                        //    Notes = strNotes,
                                                        //    PlannedCompletionDate = PlannedCompletionDate,
                                                        //    //PlannedEffort = EffortPlanned,
                                                        //    //PlannedInput = PlannedInput,
                                                        //    //PlannedOutcome = PlannedOutcome,
                                                        //    //PlannedOutcomeTestSteps = PlannedOutcomeTestSteps,
                                                        //    //PlannedProcessingTime = PlannedProcessingTime,
                                                        //    ObjectSate = ObjectSate.Manual,
                                                        //   // PlannedReviewRounds = PlannedReviewRounds,
                                                        //    PlannedStartDate = PlannedStartDate,
                                                        //    ServiceDeliveryChainMethodId = ServiceDeliveryChainMethodId,
                                                        //    TaskStatusId = TaskStatusID,
                                                        //    TSOServiceDeliveryChainId = TSOServiceDeliveryChainId,
                                                        //    UpdatedBy = strModifiedBy,
                                                        //    UpdatedOn = ModifiedOn,
                                                        //    Version = 1,
                                                        //    //WeekNumber = WeekNumber //GetIso8601WeekOfYear(PlannedStartDate)
                                                        //};

                                                        //objBusinessLayer = new BusinessLayer(ls);
                                                        //objBusinessLayer.TSOServiceDeliveryChainTaskRepository.Add(objTSOServiceDeliveryChainTask);

                                                        TSOServiceDeliveryChainTaskActual objTSOServiceDeliveryChainTaskActual = new TSOServiceDeliveryChainTaskActual()
                                                        {
                                                            ActualEffort = EffortActual,
                                                            ActualInput = ActualInput,
                                                            ActualOutcome = ActualOutcome,
                                                            ActualOutcomeTestSteps = ActualOutcomeTestSteps,
                                                            ActualProcessingTime = ActualProcessingTime,
                                                            ActualReviewRounds = ActualReviewRounds,
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,
                                                            DefectRaised = DefectRaised,
                                                            DefectRejected = DefectRejected,
                                                            Headcount = Headcount,
                                                            IdleTimeDuration = IdleTimeDuration,
                                                            IdleTimeEffort = IdleTimeEffort,                                                           
                                                            ObjectSate = ObjectSate.Manual,                                                           
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            WeekNumber = WeekNumber,
                                                            Year=2018,
                                                            TSOServiceDeliveryChainTaskId = 1,//objTSOServiceDeliveryChainTask.ID,
                                                        };

                                                        TSOServiceDeliveryChainTaskPlanned objTSOServiceDeliveryChainTaskPlanned = new TSOServiceDeliveryChainTaskPlanned()
                                                        {
                                                            CreatedBy = strCreatedBy,
                                                            CreatedOn = CreatedOn,                                                         
                                                                                                                  
                                                            PlannedEffort = EffortPlanned,
                                                            PlannedInput = PlannedInput,
                                                            PlannedOutcome = PlannedOutcome,
                                                            PlannedOutcomeTestSteps = PlannedOutcomeTestSteps,
                                                            PlannedProcessingTime = PlannedProcessingTime,
                                                            ObjectSate = ObjectSate.Manual,
                                                            PlannedReviewRounds = PlannedReviewRounds,                                                           
                                                            UpdatedBy = strModifiedBy,
                                                            UpdatedOn = ModifiedOn,
                                                            Version = 1,
                                                            WeekNumber = WeekNumber,
                                                            Year = 2018,
                                                            TSOServiceDeliveryChainTaskId = 1,//objTSOServiceDeliveryChainTask.ID,
                                                        };
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        objBusinessLayer.TSOServiceDeliveryChainTaskActualRepository.Add(objTSOServiceDeliveryChainTaskActual);
                                                        objBusinessLayer = new BusinessLayer(ls);
                                                        objBusinessLayer.TSOServiceDeliveryChainTaskPlannedRepository.Add(objTSOServiceDeliveryChainTaskPlanned);
                                                        //txtSkipped.Text = String.Format("{0} \r\n {1}", txtSkipped.Text, string.Format("ServiceDeliveryChain ={0} not found in TSO ={1}. SP Task Id={2}", objServiceDeliveryChain.Name, SP_TSOId, dr["ID"].ToString()));
                                                    }

                                                    lblTaskCount.Text = iterationCount.ToString();
                                                    Application.DoEvents();
                                                }
                                                //else
                                                //{
                                                //    txtSkipped.Text = String.Format("{0} \r\n {1}", txtSkipped.Text, "TSO ={1}.\r\n", SP_TSOId));
                                                //    Application.DoEvents();
                                                //}
                                            }
                                            else
                                            {
                                                grpReport.Text = String.Format("Report (Processed {0} of {1})", iterationCount, dt.Rows.Count.ToString());
                                                Application.DoEvents();
                                            }
                                        }

                                        lblStatus.Text = String.Format("{0}: {1} records inserted successfully.", strTable, dt.Rows.Count.ToString());
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a valid excel file", "Select valid excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }

        }

        #region Private Functions

        /// <summary>
        /// GetUser
        /// </summary>
        /// <param name="strName">String</param>
        /// <returns>int</returns>
        private int GetUser(String strName)
        {
            int userId = 0;
            string[] adPathArr = { "sqs.group.intl", "en.group.intl", "de.group.intl", "eg.group.intl", "ind.group.intl", "pd.group.intl", "resource.group.intl", "RM.group.intl", "za.group.intl", "sqs-bfsi.com", "secure1.sqs-bfsi.com", "thinksoft.chn.dn" };

            foreach (string adPath in adPathArr)
            {
                System.DirectoryServices.DirectoryEntry entry = new System.DirectoryServices.DirectoryEntry(String.Format("LDAP://{0}", adPath));
                System.DirectoryServices.DirectorySearcher searcher = new System.DirectoryServices.DirectorySearcher(entry);
                searcher.Filter = string.Format("(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2))(|(SAMAccountName={0}*)(DisplayName={0}*)(mail={0}*)))", strName);

                searcher.PropertiesToLoad.Add("mail");
                searcher.PropertiesToLoad.Add("samaccountname");
                searcher.PropertiesToLoad.Add("cn");
                searcher.PropertiesToLoad.Add("userAccountControl");

                System.DirectoryServices.SearchResultCollection results = null;
                int RecordFound = 0;
                try
                {
                    results = searcher.FindAll();
                    RecordFound = results.Count;
                }

                catch (Exception ex)
                {

                }

                if (null != results && RecordFound > 0)
                {
                    foreach (System.DirectoryServices.SearchResult p in results)
                    {
                        var accountId = "";
                        var strUserName = "";
                        var strMail = "";

                        if (p.Properties.Contains("cn"))
                        {
                            strUserName = p.Properties["cn"] == null ? "" : Convert.ToString(p.Properties["cn"][0]);
                        }

                        if (p.Properties.Contains("samaccountname"))
                        {
                            accountId = p.Properties["samaccountname"] == null ? "" : Convert.ToString(p.Properties["samaccountname"][0]);
                        }

                        if (p.Properties.Contains("mail"))
                        {
                            strMail = p.Properties["mail"] == null ? "" : Convert.ToString(p.Properties["mail"][0]);
                        }

                        if (!String.IsNullOrEmpty(strMail))
                        {
                            LoginSession ls = new LoginSession();
                            ls.LoginName = "System";
                            IBusinessLayer objBusinessLayer = new BusinessLayer(ls);

                            User objUser = objBusinessLayer.UserRepository.GetSingle(x => x.EmailID.ToLower() == strMail.ToLower());

                            if (null != objUser)
                            {
                                userId = objUser.ID;
                            }
                            else
                            {
                                objBusinessLayer = new BusinessLayer(ls);

                                objUser = new User { UserId = accountId, Name = strUserName, EmailID = strMail, Locked = false, Activated = true, Password = "Password@123" };

                                objBusinessLayer.UserRepository.Add(objUser);

                                userId = objUser.ID;
                            }
                        }
                    }
                }

                if (userId != 0)
                {
                    break;
                }
            }

            return userId;
        }

        /// <summary>
        /// This presumes that weeks start with Monday. Week 1 is the 1st week of the year with a Thursday in it.
        /// </summary>
        /// <param name="time">DateTime</param>
        /// <returns>int</returns>
        private int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        #endregion

        ///// <summary>
        ///// lblTaskSkipped_DoubleClick
        ///// </summary>
        ///// <param name="sender">object</param>
        ///// <param name="e">e</param>
        //private void lblTaskSkipped_DoubleClick(object sender, EventArgs e)
        //{
        //    if (!String.IsNullOrWhiteSpace(lblTaskSkipped.Text))
        //    {
        //        MessageBox.Show(lblTaskSkipped.Text, "Skipped Tasks", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        //    }
        //}
    }
}