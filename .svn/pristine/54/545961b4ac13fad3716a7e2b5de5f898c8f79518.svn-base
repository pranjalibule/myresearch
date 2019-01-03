using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQS.nTier.TTM.DTO;
using SQS.nTier.TTM.WebAPI.Controllers;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;


namespace TTM.Tests
{
    [TestClass()]
    public class TSRTests
    {
        public string ForLoggingUser;

        [TestInitialize]
        public void TestInitialize()
        {
            LoginDTO logDTO = new LoginDTO();
            logDTO.UserName = "admin";
            logDTO.Password = "admin";
            
            LoginController login = new LoginController();
            var result = login.ValidateLogin(logDTO);
            ForLoggingUser = result.Content.Name;

        }

        [TestMethod()]
        public void GetTSRById()
        {
            int tsrId = 5;
            TSRController tsrcontroller = new TSRController();
            var result = tsrcontroller.GetById(tsrId);
            //parse result           
            Assert.AreEqual(5, result.Content.ID);           
        }

        [TestMethod()]
        public void GetAllPaged()
        {
               
            TSRController tsrcontroller = new TSRController();

            tsrcontroller.Request = new HttpRequestMessage();
            var a = ForLoggingUser.ToString();            
            tsrcontroller.Request.Headers.TryAddWithoutValidation("userid", a);

            IHttpActionResult actionResult = tsrcontroller.GetAllPaged(0, 10);
            var result = tsrcontroller.GetAllPaged(0, 10);
            Assert.AreEqual(10, result.Content.EntitySummary.Count);
        }

        [TestMethod()]
        public void CreateTSR()
        {           
            TSRDTO tr = new TSRDTO();
            tr.Title = "NewTSRCreated";
            tr.Account = "123H";
            tr.ERPOrderDescription = "";
            tr.ERPordernumber = "123";
            tr.AccountManagerId = 46;
            tr.TestManagerId = 46;
            tr.DeliveryManagerId = 46;
            tr.ClientId = 1;
            tr.ClientRegionId = 1;
            tr.CreatedBy = "admin";
            tr.UpdatedBy = "admin";
            tr.UpdatedOn = System.DateTime.Now;
            tr.VerticalId = 6;
            tr.Version = 1;
            tr.EngagementId = "1";
            tr.Description = "New TSR";
            tr.Estimatedeffort = 20;
            tr.OperationalRiskId = 4;
            tr.Plannedeffort = 20;
            tr.PracticeId = 2;
            tr.PricingModelId = "1";
            tr.ProjectModelID = 1;
            tr.SolutionCentreId = 2;
            tr.ServiceDeliveryChainArr = "1,3";
            tr.StartDate = System.DateTime.Now;
            tr.TargetCompletionDate = System.DateTime.Now.AddDays(7);
            tr.TSRCoreServicesArr = "1";
            tr.TSRStatusID = 1;

            TSRController tsrcontroller = new TSRController();
            tsrcontroller.Request = new HttpRequestMessage();
            var a = ForLoggingUser.ToString();
            tsrcontroller.Request.Headers.TryAddWithoutValidation("userid", a);
            var result = tsrcontroller.CreateTSR(tr);
            int tsrId = Convert.ToInt32(result.Content); 
            var getByIDResult= tsrcontroller.GetById(tsrId);
            Assert.AreEqual(tr.Title, getByIDResult.Content.Title);
        }

        [TestMethod()]
        public void UpdateTSR()
        {
            TSRController tsrcontroller = new TSRController();
            tsrcontroller.Request = new HttpRequestMessage();
            var a = ForLoggingUser.ToString();
            tsrcontroller.Request.Headers.TryAddWithoutValidation("userid", a);
            var result = tsrcontroller.GetAllPaged(0, 10);           
            var getByIDResult = tsrcontroller.GetById(result.Content.EntitySummary[1].ID);
            TSRDTO updateNewDto = getByIDResult.Content;
            updateNewDto.Description = "Updated";
            var updated=tsrcontroller.UpdateTSR(getByIDResult.Content.ID, updateNewDto);
            var UpdatedDTO = tsrcontroller.GetById(Convert.ToInt32(updated.Content));
            Assert.AreNotEqual(getByIDResult.Content.Description, UpdatedDTO.Content.Description);
        }
       
    }
}