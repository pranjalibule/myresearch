using SQS.nTier.TTM.BAL;
using SQS.nTier.TTM.DAL;
using SQS.nTier.TTM.DTO;
using SQS.nTier.TTM.Encryption;
using SQS.nTier.TTM.GenericFramework;
using SQS.nTier.TTM.GenericFramework.Utility;
using SQS.nTier.TTM.WebAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;


namespace SQS.nTier.TTM.UserRoleDataImport
{
    public partial class frmUserRoleDataImport : Form
    {

        #region private variables

        private readonly string Excel03ConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        private readonly string Excel07ConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
        Utility objUtility = new Utility();
        #endregion


        #region Constructor

        public frmUserRoleDataImport()
        {
            InitializeComponent();
        }

        #endregion

        private void openFileDialogTTM_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!(!String.IsNullOrEmpty(openFileDialogTTM.FileName) && (openFileDialogTTM.FileName.IndexOf(".xls") > 0 || openFileDialogTTM.FileName.IndexOf(".xlsx") > 0 || openFileDialogTTM.FileName.IndexOf(".csv") > 0)))
            {
                MessageBox.Show("Please select a valid excel file", "Select valid excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialogTTM.ShowDialog();

            if (!String.IsNullOrEmpty(openFileDialogTTM.FileName))
            {
                txtSelectFile.Text = openFileDialogTTM.FileName;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSelectFile.Text = string.Empty;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(openFileDialogTTM.FileName))
                {

                    string FilePathName = openFileDialogTTM.FileName;
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

                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    UserDTO objUserDTO = null;
                    CryptorEngine objCryptorEngine = new CryptorEngine();

                    using (OleDbConnection conStr = new OleDbConnection(strExcelConnectionString))
                    {
                        Application.DoEvents();
                        conStr.Open();
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", conStr);
                        OleDbDataAdapter da = new OleDbDataAdapter();
                        da.SelectCommand = cmd;
                        da.Fill(ds);
                        conStr.Close();
                    }

                    dt = ds.Tables[0];
                    int cols = dt.Columns.Count;
                    int totalrecords = 0;
                    // int userId = 0;

                    int Totalcount = dt.Rows.Count;
                    lblNumOfRows.Text = Totalcount.ToString();
                    int RecordProcessed = 0;

                    LoginSession ls = new LoginSession();
                    ls.LoginName = "Admin";
                    BusinessLayer objBusinessLayer = new BusinessLayer(ls);
                    UserType objUserType = objBusinessLayer.UserTypeRepository.GetSingle(x => x.Name.ToLower() == "internal");

                    //Get the User & Role

                    foreach (DataRow dr in dt.Rows)
                    {
                        string strUser = dr.ItemArray[0].ToString();
                        string UserRole = dr.ItemArray[1].ToString() == null ? "Guest" : dr.ItemArray[1].ToString();
                        IList<User> objUserList = objUtility.GetUsersFromADOnly(strUser, out totalrecords);
                        try
                        {
                            if (null != objUserList && objUserList.Count > 0)
                            {
                                for (int j = 0; j < objUserList.Count; j++)
                                {
                                    objUserDTO = new UserDTO();
                                    objUserDTO = Conversions.ToDTO<UserDTO, User>(objUserList[j]);


                                    objBusinessLayer = new BusinessLayer(ls);
                                    Roles objUserrole = objBusinessLayer.RoleRepository.GetSingle(x => x.Name.ToLower() == UserRole.ToLower());//UserRole Provide By Excel Sheet But Role is already Present In DB.

                                    string ExistADUserName = objUserDTO.UserId.ToString();
                                    objBusinessLayer = new BusinessLayer(ls);
                                    User UserObj = objBusinessLayer.UserRepository.GetSingle(x => x.UserId.ToLower() == ExistADUserName.ToLower());
                                    if (UserObj != null)
                                    {
                                        TTMLogger.Logger.LogError("Record already Present In DB:" + UserObj.Name);
                                        if (UserObj.RoleId == null && objUserrole != null)
                                        {
                                            UserObj.RoleId = objUserrole.ID;
                                            objBusinessLayer = new BusinessLayer(ls);
                                            objBusinessLayer.UserRepository.Update(UserObj);
                                            RecordProcessed++;
                                            lblRecordProcessed.Text = RecordProcessed.ToString();
                                            Application.DoEvents();
                                        }
                                    }
                                    else
                                    {
                                        if (objUserType != null)
                                        {

                                            //Create a new user


                                            string strPassword = objCryptorEngine.Encrypt(PasswordGenerator(), true);
                                            User objUser = null;

                                            if (objUserrole != null)
                                            {
                                                objUser = new User
                                                {
                                                    UserId = objUserDTO.UserId,
                                                    UserTypeID = objUserType.ID,
                                                    EmailID = objUserDTO.EmailID,
                                                    Name = objUserDTO.Name,
                                                    Password = strPassword,
                                                    Activated = true,
                                                    RoleId = objUserrole.ID
                                                };

                                                objBusinessLayer = new BusinessLayer(ls);
                                                objBusinessLayer.UserRepository.Add(objUser);
                                                RecordProcessed++;
                                                lblRecordProcessed.Text = RecordProcessed.ToString();
                                                Application.DoEvents();

                                            }
                                        }
                                    }
                                }
                            }
                        }

                        catch (Exception ex)
                        {
                            TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TTMLogger.Logger.LogError(String.Format("Error - {0}", ex.Message));
            }

        }
        private string PasswordGenerator()
        {
            Random RandomGen = new Random();
            int seed = RandomGen.Next(1, int.MaxValue);
            const string specialCharacters = @"abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";

            var chars = new char[12];
            var rd = new Random(seed);

            for (var i = 0; i < 12; i++)
            {
                chars[i] = specialCharacters[rd.Next(0, specialCharacters.Length)];
            }

            return new string(chars);
        }

        private int GetUserFromAD(String strName)
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
                    throw ex;


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


    }
}
