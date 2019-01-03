﻿var TSROld = '';
function LoadVertical() {
    var url = urlPrefix + "Vertical/GetAllIDName";
    LoadData(url, 'ddlVertical');
}
function LoadPractice() {
    var url = urlPrefix + "Practice/GetAllIDName";
    LoadData(url, 'ddlPractice');
}
function LoadSolutionCentre() {
    var url = urlPrefix + "SolutionCentre/GetAllIDName";
    LoadData(url, 'ddlSolutioncentre');
}
function LoadClientRegion() {
    var url = urlPrefix + "ClientRegion/GetAllIDName";
    LoadData(url, 'ddlClientRegion');
}
function LoadMarketOffering() {
    var url = urlPrefix + "MarketOffering/GetAllIDName";
    LoadData(url, 'ddlMarketOffering');
}
function LoadOperationalRisk() {
    var url = urlPrefix + "OperationalRisk/GetAllIDName";
    LoadData(url, 'ddlOperationalrisk');
}
function LoadCoreServices() {
    var url = urlPrefix + "CoreService/GetAllIDName";
    LoadData(url, 'listCoreServices');
}
function LoadReliventRepositories() {
    var url = urlPrefix + "RelevantRepository/GetAllIDName";
    LoadData(url, 'listRelevantRepositories');
}

function LoadProjectModels() {
    var url = urlPrefix + "ProjectModel/GetAllIDName";
    LoadData(url, 'ddlProjectModel');
}

function LoadPricingModel() {
    var url = urlPrefix + "PricingModel/GetAllIDName";
    LoadData(url, 'ddlPricing');
}
function LoadEngagement() {
    var url = urlPrefix + "Engagement/GetAllIDName";
    LoadData(url, 'ddlEngagement');
}

function LoadClient() {
    var url = urlPrefix + "Client/GetAllIDName";
    LoadData(url, 'ddlClient');
}


function LoadTSRStatus() {
    var url = urlPrefix + "TSRStatus/GetAllIDName";
    LoadData(url, 'ddlTSRStatus');
}

function LoadData(url, dropdown) {
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        async: false,
        success: function (result) {
            var totalRecords = result.length;

            if (!isNaN(totalRecords) && parseInt(totalRecords) > 0) {
                PopulateDropdown(result, dropdown);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (errorThrown == "Unauthorized") {
                textStatus = "You'r not authorised to do the current operation."
            }
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

function LoadTSR(tsrId) {
    var url = urlPrefix + "TSR/GetById/" + tsrId;
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('userid', userId);
        },
        success: function (result) {
            if (null !== result) {
                PopulateTSR(result);
            }
            hideLoader();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoader();
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

function disableAllTSR() {
    if (roleId.toLowerCase() != "admin") {
        if ($('#ddlTSRStatus').val() > 1 && $('#ddlTSRStatus').val() < 5) {
            if ($('#ddlTSRStatus').val() == 2 || $('#ddlTSRStatus').val() == 4) {
                $("#ddlTSRStatus").attr("disabled", "disabled");
                $("#TSRSubmit").attr("disabled", "disabled");
                $("#ddlOperationalrisk").attr("disabled", "disabled");
            }
            $("#txtDeliveryManager,#txtDescription, #txtERPOrderDescription").attr("disabled", "disabled");
            //$("#ddlVertical,#ddlPractice,#ddlSolutioncentre,#ddlClientRegion,#ddlClient").attr("disabled", "disabled");
            $("#txtTestManager,#txtAccount,#txtAccountManager").attr("disabled", "disabled");
            //$("#ddlProjectModel,#ddlMarketOffering,#ddlEngagement,#ddlPricing,#ddlProjectModel,#ddlOperationalrisk").attr("disabled", "disabled");
            //$("#listRelevantRepositories,#listRelevantRepositoriesSelected,#listCoreServices,#listCoreServicesSelected").attr("disabled", "disabled");
            //$("#AddCoreServices,#RemoveCoreServies,#AddRelevantRepositories,#RemoveRelevantRepositories").attr("disabled", "disabled");

            $(".remove").hide();
            $('.remove').attr("disabled", true);
        }
        else {
            $("#txtDeliveryManager,#txtDescription, #txtERPOrderDescription").removeAttr("disabled");
            //$("#ddlVertical,#ddlPractice,#ddlSolutioncentre,#ddlClientRegion,#ddlClient").removeAttr("disabled");
            $("#txtTestManager,#txtAccount,#txtAccountManager").removeAttr("disabled");
            //$("#ddlProjectModel,#ddlMarketOffering,#ddlEngagement,#ddlPricing,#ddlProjectModel,#ddlOperationalrisk").removeAttr("disabled");
            //$("#listRelevantRepositories,#listRelevantRepositoriesSelected,#listCoreServices,#listCoreServicesSelected").removeAttr("disabled");
            //$("#AddCoreServices,#RemoveCoreServies,#AddRelevantRepositories,#RemoveRelevantRepositories").removeAttr("disabled");
            $(".remove").show();
            $('.remove').attr("disabled", false);
        }
        //,#txtDeliveryManager,#txtAccountManager,#txtTestManager,#ddlOperationalrisk ,#ddlTSRStatus , #txtActualCompletionDate
        $("#txtTitle,#txtERPOrderNumber").attr("disabled", "disabled");
        $("#txtStartDate,#txtTargetCompletionDate, #txtSpId,#txtEstimatedEffort,#txtPlannedEffort,#txtActualEffort").attr("disabled", "disabled");//#txtActualStartDate,
        $("#DeliveryInfo").prop('disabled', 'disabled');
        $("#txtAccount").prop('disabled', 'disabled');
        //$("#upload").prop('disabled', 'disabled');
    }

    if ($("#txtTMO2").val() == "") {
        $("#trTMO2").hide();
    }
    else {
        $("#trTMO2").show();
    }
}

function PopulateTSR(result) {
    TSROld = result;  
   
    addToBreadcrumbArray(result.Title, result.ID, "TSR", false);
    CreateBreadcrumb();

    $("#txtID").val(result.ID);
    $("#txtTitle").val(result.Title);
    $("#txtDeliveryManager").val(result.DeliveryManager.Name)
    $("#lblDeliveryManagerMail").val(result.DeliveryManager.EmailID)
    $("#lblDeliveryManagerId").val(result.DeliveryManagerId);
    if (result.TSRTMOUsers.length > 0) {
        $("#txtTMO1").val(result.TSRTMOUsers[0].User.Name)
        $("#lblTMO1Mail").val(result.TSRTMOUsers[0].User.EmailID)
        $("#lblTMO1Id").val(result.TSRTMOUsers[0].UserId);

        if (result.TSRTMOUsers.length > 1) {
            $("#txtTMO2").val(result.TSRTMOUsers[1].User.Name)
            $("#lblTMO2Mail").val(result.TSRTMOUsers[1].User.EmailID)
            $("#lblTMO2Id").val(result.TSRTMOUsers[1].UserId);
        }
    }

    $("#txtDescription").val(result.Description);
    $("#txtERPOrderDescription").val(result.ERPOrderDescription);
    $("#txtTestManager").val(result.TestManager.Name)
    $("#lblTestManagerMail").val(result.TestManager.EmailID)
    $("#lblTestManagerId").val(result.TestManagerId);
    $("#ddlVertical").val(result.VerticalId);
    $("#ddlPractice").val(result.PracticeId);
    $("#ddlSolutioncentre").val(result.SolutionCentreId);
    $("#ddlClientRegion").val(result.ClientRegionId);
    $("#ddlClient").val(result.ClientId);
    $("#ddlTSRStatus").val(result.TSRStatusID);
    $("#txtAccount").val(result.Account);
    $("#ddlEngagement").val(result.EngagementId);
    $("#ddlPricing").val(result.PricingModelId);
    $("#txtAccountManager").val(result.AccountManager.Name)
    $("#lblAccountManagerMail").val(result.AccountManager.EmailID)
    $("#lblAccountManagerId").val(result.AccountManagerId);
    $("#txtERPOrderNumber").val(result.ERPordernumber);
    $("#ddlMarketOffering").val(result.MarketOfferingId);
    $("#ddlProjectModel").val(result.ProjectModelID);
    $("#txtActualEffort").val(result.ActualEffort);
    $("#txtSpId").val(result.SP_Id);

    $("#txttimestamp").val(dateFormat(result.UpdatedOn, "dd-mmm-yyyy HH:MM:ss TT"));

    $("#txtCreatedBy").val(result.UpdatedBy);

    var startDate = $.datepicker.parseDate('yy-m-dd', result.StartDate.replace("T00:00:00", ""));
    $("#txtStartDate").val(dateFormat(startDate, "isoDateddMonyyyy"));
    var targetCompletionDate = $.datepicker.parseDate('yy-m-dd', result.TargetCompletionDate.replace("T00:00:00", ""));
    $("#txtTargetCompletionDate").val(dateFormat(targetCompletionDate, "isoDateddMonyyyy"));

    $("#txtEstimatedEffort").val(result.Estimatedeffort);

    $("#txtPlannedEffort").val(result.Plannedeffort);
    $("#ddlOperationalrisk").val(result.OperationalRiskId);

    if (null !== result.TSRReleventRepositories && result.TSRCoreServices.length > 0) {
        for (var iReleventRepositoriesCount = 0; iReleventRepositoriesCount < result.TSRCoreServices.length; iReleventRepositoriesCount++) {
            $('#listCoreServices option').each(function () {
                if (parseInt(this.value) == parseInt(result.TSRCoreServices[iReleventRepositoriesCount].CoreServiceId)) {
                    $("#listCoreServices option[value='" + result.TSRCoreServices[iReleventRepositoriesCount].CoreServiceId + "']").prop("selected", true);
                    return;
                }
            });
        }
    }

    //var actualStartDate = $.datepicker.parseDate('yy-m-dd', result.ActualStartDate.replace("T00:00:00", ""));
    //$("#txtActualStartDate").val(dateFormat(actualStartDate, "isoDateddMonyyyy"));
    //var actualCompletionDate = $.datepicker.parseDate('yy-m-dd', result.ActualCompletionDate.replace("T00:00:00", ""));
    //$("#txtActualCompletionDate").val(dateFormat(actualCompletionDate, "isoDateddMonyyyy"));


    moveItems('#listCoreServices', '#listCoreServicesSelected');

    if (null !== result.TSRReleventRepositories && result.TSRReleventRepositories.length > 0) {
        for (var iCoreServices = 0; iCoreServices < result.TSRReleventRepositories.length; iCoreServices++) {
            $('#listRelevantRepositories option').each(function () {
                if (parseInt(this.value) == parseInt(result.TSRReleventRepositories[iCoreServices].RelevantRepositoryId)) {
                    $("#listRelevantRepositories option[value='" + result.TSRReleventRepositories[iCoreServices].RelevantRepositoryId + "']").prop("selected", true);
                    return;
                }
            });
        }
    }

    moveItems('#listRelevantRepositories', '#listRelevantRepositoriesSelected');
    $("#TSRStartDate").val(dateFormat(startDate, "isoDateddMonyyyy"));
    var tbody = $('#tsrfiledetails');
    var number = 0;
    for (var totalrecords = 0; totalrecords < result.TSRFiles.length; totalrecords++) {
        number++;
        var FileName = result.TSRFiles[totalrecords]["Path"];
        var extention = FileName.substr(FileName.indexOf("."), FileName.length);
        var guid = result.TSRFiles[totalrecords]["GUID"];
        var url = result.FilePath[totalrecords];
        var className = "";
        if (number == result.TSRFiles.length) {
            className = "class=lastRow";
        }

        tbody.append("<tr id='tr_" + number + "' style='min-height:20px;min-height:20px !important; padding:5px 0px !important;' " + className + " data-name='" + guid + "' data-path=>" +
            "<td align='center' style='width:13%;min-height:20px !important; padding:2px 0px !important;' id='tdIndex'>" + number + "</td>" +
            "<td  class='leftalign' style='width:76%;min-height:20px !important; padding:2px 0px !important;'>" + FileName + "</td>" +
            "<td align='center' style='width:6%;min-height:20px !important; padding:2px 0px !important;'><a class='download' href='" + url + "' target='_blank' download='" + FileName + "'>Show</a></td>" +
            "<td align='center' style='width:5%;min-height:20px !important; padding:2px 0px !important;'><a class='remove' onclick='RemoveFile(" + number + ")'><img style='width:12px;' src='/images/cross.png'/></a></td></tr>");
    }
    disableAllTSR();
}


function PopulateDropdown(result, dropdown) {
   
    if (dropdown != 'listCoreServices' && dropdown != 'listRelevantRepositories' && dropdown != 'ddlOperationalrisk') {
        $("#" + dropdown).prepend("<option value='Please Select' selected='selected'>-- Select --</option>");
    }
    
    $.each(result, function (i, item) {
        $('#' + dropdown).append($('<option>', {
            value: item.ID,
            text: item.Name
        }));
    });
}
function Show()
{
    $("#TMOAdd").attr("disabled", "disabled");
    $("#trTMO2").show();
    
}
function Save() {
    showLoader();
    if (CheckTSRData()) {

        var isChange = false;
        var txtID = $("#txtID").val();

        var txtTitle = $("#txtTitle").val();
        var txtSp_Id = $("#txtSpId").val();
        var txtDeliveryManager = $("#lblDeliveryManagerId").val();
        var txtDescription = $("#txtDescription").val();
        var txtERPOrderDescription = $("#txtERPOrderDescription").val();


        var txtTestManager = $("#lblTestManagerId").val();
        var ddlVertical = $("#ddlVertical").val();
        var ddlPractice = $("#ddlPractice").val();
        var ddlSolutioncentre = $("#ddlSolutioncentre").val();


        var ddlProjectModel = $("#ddlProjectModel").val();
        var ddlClientRegion = $("#ddlClientRegion").val();
        var txtClient = $("#ddlClient").val();
        var txtAccount = $("#txtAccount").val();


        var txtEngagement = $("#ddlEngagement").val();
        var txtPricingModel = $("#ddlPricing").val();
        var txtAccountManager = $("#lblAccountManagerId").val();
        var txtERPOrderNumber = $("#txtERPOrderNumber").val();
        var ddlMarketOffering = $("#ddlMarketOffering").val();


        var txtStartDate = $("#txtStartDate").val();
        var txtTargetCompletionDate = $("#txtTargetCompletionDate").val();
        //var txtActualStartDate = $("#txtActualStartDate").val();
        //var txtActualCompletionDate = $("#txtActualCompletionDate").val();


        var txtEstimatedEffort = $("#txtEstimatedEffort").val();
        var txtPlannedEffort = $("#txtPlannedEffort").val();
        var ddlOperationalrisk = $("#ddlOperationalrisk").val();
        var ddlTSRStatus = $("#ddlTSRStatus").val();


        //Get all core services
        var listCoreServicesSelected = "";
        $('#listCoreServicesSelected').find('option').each(function () {
            listCoreServicesSelected += $(this).val();
            listCoreServicesSelected += ",";
        });       
      
        listCoreServicesSelected = listCoreServicesSelected.substring(0, listCoreServicesSelected.length - 1);

        var listRelevantRepositoriesSelected = "";

        $('#listRelevantRepositoriesSelected').find('option').each(function () {
            listRelevantRepositoriesSelected += $(this).val();
            listRelevantRepositoriesSelected += ",";
        });

        listRelevantRepositoriesSelected = listRelevantRepositoriesSelected.substring(0, listRelevantRepositoriesSelected.length - 1);

        //TMONames
        var listOfTMOSelected = "";
        if ($("#txtTMO1").val() != "") {
            listOfTMOSelected += $("#lblTMO1Id").val();
        }
        listOfTMOSelected += ",";
        if ($("#txtTMO2").val() != "") {
            listOfTMOSelected += $("#lblTMO2Id").val();
            listOfTMOSelected += ",";
        }

        listOfTMOSelected = listOfTMOSelected.substring(0, listOfTMOSelected.length - 1);

        if (TSROld != null && TSROld != "") {
            if (txtTitle != TSROld.Title || txtDeliveryManager != TSROld.DeliveryManagerId ||
            txtDescription != TSROld.Description || txtERPOrderDescription != TSROld.ERPOrderDescription) {
                isChange = true;
            }

            if (txtTestManager != TSROld.TestManagerId || ddlVertical != TSROld.VerticalId ||
            ddlPractice != TSROld.PracticeId || ddlSolutioncentre != TSROld.SolutionCentreId) {
                isChange = true;
            }
           
            if (ddlProjectModel != TSROld.ProjectModelID || ddlClientRegion != TSROld.ClientRegionId ||
            txtClient != TSROld.ClientId || txtAccount != TSROld.Account) {
                isChange = true;
            }

            if (txtEngagement != TSROld.EngagementId || txtPricingModel != TSROld.PricingModelId || txtAccountManager != TSROld.AccountManagerId ||
            txtERPOrderNumber != TSROld.ERPordernumber || ddlMarketOffering != TSROld.MarketOfferingId) {
                isChange = true;
            }

            if (txtStartDate != dateFormat(TSROld.StartDate, "isoDateddMonyyyy") || txtTargetCompletionDate != dateFormat(TSROld.TargetCompletionDate, "isoDateddMonyyyy") //||
                //  txtActualStartDate != dateFormat(TSROld.ActualStartDate, "isoDateddMonyyyy") || txtActualCompletionDate != dateFormat(TSROld.ActualCompletionDate, "isoDateddMonyyyy"
                ) {
                isChange = true;
            }

            if (txtEstimatedEffort != TSROld.Estimatedeffort || txtPlannedEffort != TSROld.Plannedeffort ||
            ddlOperationalrisk != TSROld.OperationalRiskId || ddlTSRStatus != TSROld.TSRStatusID) {
                isChange = true;
            }

            if (userName != TSROld.UpdatedBy) {
                isChange = true;
            }

            var chianArr = listRelevantRepositoriesSelected.split(',');
            if (TSROld.TSRReleventRepositories.length < chianArr.length) {
                isChange = true;
            }

            chianArr = listCoreServicesSelected.split(',');
            if (TSROld.TSRCoreServices.length < chianArr.length) {
                isChange = true;
            }

            chianArr = listOfTMOSelected.split(',')
            if (TSROld.TSRTMOUsers.length > chianArr.length) {
                isChange = true;
            }
            else if (TSROld.TSRTMOUsers.length < chianArr.length)
            {
                isChange = true;
            }          

          
            var files = $("#fileUpload").get(0).files;
            var rows = $('#tsrfiledetails > tr');
            if (rows.length != TSROld.TSRFiles.length) {
                isChange = true;
            }

            if (files.length > 0) {
                isChange = true;
            }

            checkCloseOnSubmit('ddlTSRStatus', TSROld.TSRStatusID);
        }



        if (!isChange && (typeof txtID != "undefined" && txtID != "" && txtID != null)) {
            hideLoader();
            showMessageBox("No updation seen !", "red");
            return false;
        }


        //debugger;
        var varddlTSRStatus = $("#ddlTSRStatus option:selected").text();
        if (varddlTSRStatus == "-- Select --" && $('#ddlTSRStatus').css('display') != 'none') {
            showMessageBox("Please select the status of TSR !", "red");
            return false;
        }


        if (isChange && !TSROld.CanClose && (varddlTSRStatus == "Closed" || varddlTSRStatus == "Cancelled")) {
            hideLoader();
            showMessageBox("The TSO / Tasks needs to be closed first before TSR can  be closed !", "red");
            return false;
        }

        //Get all core services
      
        var data = JSON.stringify({
            Title: txtTitle, DeliveryManagerId: txtDeliveryManager, Description: txtDescription,
            ERPOrderDescription: txtERPOrderDescription, TestManagerId: txtTestManager, VerticalId: ddlVertical,
            PracticeId: ddlPractice, SolutionCentreId: ddlSolutioncentre, ProjectModelID: ddlProjectModel, ClientRegionId: ddlClientRegion,
            ClientId: txtClient, Account: txtAccount, EngagementId: txtEngagement, PricingModelId: txtPricingModel, AccountManagerId: txtAccountManager,
            ERPordernumber: txtERPOrderNumber, MarketOfferingId: ddlMarketOffering, StartDate: txtStartDate,
            TargetCompletionDate: txtTargetCompletionDate, //ActualStartDate: txtActualStartDate,
            // ActualCompletionDate: txtActualCompletionDate,
            Estimatedeffort: txtEstimatedEffort, Plannedeffort: txtPlannedEffort,
            OperationalRiskId: ddlOperationalrisk, TSRCoreServicesArr: listCoreServicesSelected,
            TSTRelevantRepositoriesArr: listRelevantRepositoriesSelected, CreatedBy: userName, UpdatedBy: userName,
            TSRStatusID: ddlTSRStatus, TSRTMOArr: listOfTMOSelected
        });

        var urlAddUpdate = urlPrefix + "TSR";

        var method = "POST";
        var message = "";
        var tsrId = getLocalStorage("tsrid");
        if (tsrId != "" && tsrId != null) {
            if (typeof tsrId != "undefined") {
                method = "PUT";
                urlAddUpdate = urlAddUpdate + "/UpdateTSR/" + tsrId;
                message = "TSR updated successfully.";
            }
            else {
                urlAddUpdate = urlAddUpdate + "/CreateTSR";
                message = "TSR created successfully.";
            }
        }
        else {
            urlAddUpdate = urlAddUpdate + "/CreateTSR";
            message = "TSR created successfully.";
        }

        $.ajax({
            url: urlAddUpdate,
            data: data,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('userid', userId);
            },
            type: method,
            contentType: 'application/json; charset=UTF-8',
            success: function (response) {
                //debugger;
                hideLoader();
                //$("#txtOutput").html("<b>" + response + "</b>");
                if (response.indexOf("Error") == 0) {
                    showMessageBox(response, "red");
                    // $("#txtOutput").css('color', 'red');
                }
                else {
                    var returnMsg;

                    var dd = 0;
                    for (var rowindex = 0; rowindex < $("#fileUpload").get(0).files.length; rowindex++) {
                        dd = dd + $("#fileUpload").get(0).files[rowindex].size;
                    }
                    if ($("#fileUpload").get(0).files.length > 0) {
                        returnMsg = sendFile(response, message);
                    }
                    if (typeof returnMsg == "undefined") {
                        //localStorage.setItem("user", userId);
                        showMessageBox(message, "green", "TSRDashboard.html");// + "&rowToselect=" + response);
                    }
                    else if (returnMsg == "Success") {
                        //localStorage.setItem("user", userId);
                        showMessageBox(message, "green", "TSRDashboard.html");// + "&rowToselect=" + response);
                    }
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoader();
                if (errorThrown == "Unauthorized") {
                    textStatus = "You'r not authorised to do the current operation."
                }
                // showMessageBox(textStatus + "--" + errorThrown, "red");
                showMessageBox(textStatus, "red");

            }
        });
    }
}

function Download(fileSequence) {
    var rows = $('#tsrfiledetails > tr');
    var Number = 0;
    for (var rowindex = 0; rowindex < rows.length; rowindex++) {
        var td = rows[rowindex].children(0);
        if (typeof td != "undefined") {
            if (rows[rowindex].id == "tr_" + fileSequence) {
                var guid = rows[rowindex].attributes["data-name"].value;
                if (guid.length > 5) {
                    var url = urlPrefix + "/TSR/DownloadFile/" + guid;
                    $.ajax({
                        url: url,
                        type: 'POST',
                        contentType: 'application/json; charset=UTF-8',
                        success: function (response) {
                        },
                        error: function (er) { showMessageBox(er.responseText, "red"); return false; }
                    });
                }
            }
            else {
                Number++;
                $(td).text(Number);
            }
        }
    }
}

function Cancel() {
    window.location = "TSRDashboard.html";
}

function callSuccess(message) {
    window.location = "TSRDashboard.html";
}

function CheckTSRData() {

    var startdate = jQuery.datepicker.parseDate("dd-M-yy", $("#txtStartDate").val());
    var startdue = dateFormat(startdate, "isoDateddMonyyyy");
    var currentdate = new Date();
    var curr = dateFormat(currentdate, "isoDateddMonyyyy");

    var enddate = jQuery.datepicker.parseDate("dd-M-yy", $("#txtTargetCompletionDate").val());
    var enddue = dateFormat(enddate, "isoDateddMonyyyy");
  

if ($("#txtStartDate").val() != "") {
        try {
            jQuery.datepicker.parseDate("dd-M-yy", $("#txtStartDate").val());
        }
        catch (e) {
            showMessageBox("Please select valid planned start date.", "red", "", false, "txtStartDate");
            return false;
        }
    }

 if ($("#txtTargetCompletionDate").val() != "") {
        try {
            jQuery.datepicker.parseDate("dd-M-yy", $("#txtTargetCompletionDate").val());
        }
        catch (e) {
            showMessageBox("Please select valid planned completion date.", "red", "", false, "txtTargetCompletionDate");
            return false;
        }
    }
   

    if ($("#txtTitle").val() == "") {
        showMessageBox("Please enter TSR title.", "red", "", false, "txtTitle");
        return false;
    }
    else if ($("#txtSpId").val() == "") {
        showMessageBox("Please enter SP_Id.", "red", "", false, "txtSpId");
        return false;
    }   
    else if (new Date(startdue) < new Date(curr)) {
        showMessageBox("You are not allowed to select older week’s  for planned start date.", "red", "", false, "txtStartDate");
        return false;
    }
    else if (new Date(enddue) < new Date(curr)) {
        showMessageBox("You are not allowed to select older week’s  for planned completion date.", "red", "", false, "txtTargetCompletionDate");
        return false;
    }  
    else if ($("#txtDescription").val() == "") {
        showMessageBox("Please enter TSR description.", "red", "", false, "txtDescription");
        return false;
    }
    else if ($("#lblDeliveryManagerId").val() == "" || $("#txtDeliveryManager").val() == "") {
        showMessageBox("Please enter valid Delivery Manager's name.", "red", "", false, "txtDeliveryManager");
        return false;
    }
    else if ($("#lblTestManagerId").val() == "" || $("#txtTestManager").val() == "") {
        showMessageBox("Please enter valid Test Manager's name.", "red", "", false, "txtTestManager");
        return false;
    }
    else if ($("#ddlVertical").val() == "" || $("#ddlVertical").val() == "Please Select") {
        showMessageBox("Please select vertical.", "red", "", false, "ddlVertical");
        return false;
    }
    else if ($("#ddlPractice").val() == "" || $("#ddlPractice").val() == "Please Select") {
        showMessageBox("Please select practice.", "red", "", false, "ddlPractice");
        return false;
    }
    else if ($("#ddlSolutioncentre").val() == "" || $("#ddlSolutioncentre").val() == "Please Select") {
        showMessageBox("Please select solution centre.", "red", "", false, "ddlSolutioncentre");
        return false;
    }
    else if ($("#ddlClientRegion").val() == "" || $("#ddlClientRegion").val() == "Please Select") {
        showMessageBox("Please select client region.", "red", "", false, "ddlClientRegion");
        return false;
    }
    else if ($("#ddlClient").val() == "" || $("#ddlClient").val() == "Please Select") {
        showMessageBox("Please select client.", "red", "", false, "ddlClient");
        return false;
    }
    else if ($("#txtAccount").val() == "") {
        showMessageBox("Please enter account name.", "red", "", false, "txtAccount");
        return false;
    }
    else if ($("#ddlEngagement").val() == "" || $("#ddlEngagement").val() == "Please Select") {

        showMessageBox("Please select engagement.", "red", "", false, "ddlEngagement");
        return false;
    }
    else if ($("#ddlPricing").val() == "" || $("#ddlPricing").val() == "Please Select") {
        showMessageBox("Please select Pricng Model.", "red", "", false, "ddlPricing");
        return false;
    }
    else if ($("#lblAccountManagerId").val() == "") {
        showMessageBox("Please enter valid Account Manager's name.", "red", "", false, "txtAccountManager");
        return false;
    }
    else if ($("#txtERPOrderNumber").val() == "") {
        showMessageBox("Please enter ERP order number.", "red", "", false, "txtERPOrderNumber");
        return false;
    }
    else if ($("#txtERPOrderDescription").val() == "") {
        showMessageBox("Please enter ERP order description.", "red", "", false, "txtERPOrderDescription");
        return false;
    }
    else if ($("#txtStartDate").val() == "") {
        showMessageBox("Please select planned start date.", "red", "", false, "txtStartDate");
        return false;
    }
    else if ($("#txtTargetCompletionDate").val() == "") {
        showMessageBox("Please select planned completion date.", "red", "", false, "txtTargetCompletionDate");
        return false;
    }
    else if ($.datepicker.parseDate('dd-M-yy', $("#txtTargetCompletionDate").val()) < $.datepicker.parseDate('dd-M-yy', $("#txtStartDate").val())) {
        showMessageBox("Planned completion date can not be less than planned start date.", "red", "", false, "txtTargetCompletionDate");
        return false;
    }
    else if ($.datepicker.parseDate('dd-M-yy', $("#TSRStartDate").val()) > $.datepicker.parseDate('dd-M-yy', $("#txtStartDate").val())) {
        showMessageBox("TSR planned start date can not be less than previous TSR planned start date.", "red", "", false, "txtStartDate");
        return false;
    }
    else if ($("#txtEstimatedEffort").val() == "") {
        showMessageBox("Please enter estimated efforts.", "red", "", false, "txtEstimatedEffort");
        return false;
    }
    else if ($("#txtPlannedEffort").val() == "") {
        showMessageBox("Please enter planned efforts.", "red", "", false, "txtPlannedEffort");
        return false;
    }
    else if ($("#ddlProjectModel").val() == "" || $("#ddlProjectModel").val() == "Please Select") {
        showMessageBox("Please select project model.", "red", "", false, "ddlProjectModel");
        return false;
    }
    else if ($('#listCoreServicesSelected').has('option').length == 0) {
        showMessageBox("Please select core services.", "red", "", false, "listCoreServicesSelected");
        return false;
    }  
    else {
        return true;
    }
}

function NumAndTwoDecimals(e, field) {
    var val = field.value;
    var re = /^([0-9]+[\.]?[0-9]?[0-9]?|[0-9]+)$/g;
    var re1 = /^([0-9]+[\.]?[0-9]?[0-9]?|[0-9]+)/g;
    if (re.test(val)) {
        //do something here

    } else {
        val = re1.exec(val);
        if (val) {
            field.value = val[0];
        } else {
            field.value = "";
        }
    }

    var max_chars = 8;
    if (field.value.length > max_chars) {
        field.value = field.value.substr(0, max_chars);
    }
}

function moveItems(origin, dest) {
    $(origin).find(':selected').appendTo(dest);

    //$(dest).append($(dest + " option").remove().sort(function (a, b) {
    //    var at = $(a).text(), bt = $(b).text();
    //    return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
    //}));

    $(dest).append($(dest + " option").prop('selected', false));
}

var fis = 0;
//var formdata = new FormData();
var filesToUpload = [];
$(function () {

    //--Core services
    $('#AddCoreServices').click(function () {
        moveItems('#listCoreServices', '#listCoreServicesSelected');
    });

    //$('#listCoreServices').dblclick(function () {       
    //    moveItems('#listCoreServices', '#listCoreServicesSelected');       
    //});

    $('#RemoveCoreServies').on('click', function () {
        moveItems('#listCoreServicesSelected', '#listCoreServices');
    });

    //$('#listCoreServicesSelected').dblclick(function () {
    //    moveItems('#listCoreServicesSelected', '#listCoreServices');
    //});

    //--Relevant repositories --

    $('#AddRelevantRepositories').click(function () {
        moveItems('#listRelevantRepositories', '#listRelevantRepositoriesSelected');
    });

    //$('#listRelevantRepositories').dblclick(function () {
    //    moveItems('#listRelevantRepositories', '#listRelevantRepositoriesSelected');
    //});

    $('#RemoveRelevantRepositories').on('click', function () {
        moveItems('#listRelevantRepositoriesSelected', '#listRelevantRepositories');
    });

    //$('#listRelevantRepositoriesSelected').dblclick(function () {
    //    moveItems('#listRelevantRepositoriesSelected', '#listRelevantRepositories');
    //});

    $("#txtDeliveryManager").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtDeliveryManager", "#lblDeliveryManagerId");
            GetDBId("#lblDeliveryManagerMail", "#lblDeliveryManagerId");
        }
    });

    $("#txtTestManager").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtTestManager", "#lblTestManagerId");
            GetDBId("#lblTestManagerMail", "#lblTestManagerId");
        }
    });

    $("#txtAccountManager").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtAccountManager", "#lblAccountManagerId");
            GetDBId("#lblAccountManagerMail", "#lblAccountManagerId");
        }
    });

    $("#txtTMO1").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtDeliveryManager", "#lblDeliveryManagerId");
            GetDBId("#lblTMO1Mail", "#lblTMO1Id");
        }
    });


    $("#txtTMO2").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtDeliveryManager", "#lblDeliveryManagerId");
            GetDBId("#lblTMO2Mail", "#lblTMO2Id");
        }
    });


    $('input[type=file]').change(function () {
        var varddlTSRStatus = $("#ddlTSRStatus option:selected").text();
        if (varddlTSRStatus == "Closed" || varddlTSRStatus == "Cancelled") {
            showMessageBox("You can not upload file in closed or cancelled TSR !", "red");
            return false;
        }
        else {
            var filePath = $('#fileUpload').val();

            var files = $("#fileUpload").get(0).files;
            var temp = 0;
            if (files.length > 0) {
                for (var f = 0; f < files.length; f++) {
                    filesToUpload.push({ id: "files" + temp, file: files[f] });
                    temp++;
                }
            }

            if (filePath != "") {
                showUpload(filePath);
            }
        }
    });
});

function SelectFile() {
    var varddlTSRStatus = $("#ddlTSRStatus option:selected").text();
    if (varddlTSRStatus == "Closed" || varddlTSRStatus == "Cancelled") {
        showMessageBox("You can not upload file in closed or cancelled TSR !", "red");
        return false;
    }
    else {
        $('#fileUpload').trigger('click');
    }
}

function sendFile(tsrID, message) {
    var formData = new FormData();
    var tsrId = getLocalStorage("tsrid");
    if (tsrID != "") {
        tsrId = tsrID;
    }


    var files = $("#fileUpload").get(0).files;
    var temp = 0;
    if (files.length > 0) {
        for (var f = 0; f < files.length; f++) {
            for (var i = 0; i < filesToUpload.length; i++) {
                if (filesToUpload[i].file.name == files[f].name) {
                    formData.append("files" + temp, files[f]);
                    temp++;
                }
            }
        }
    }

    if (formData == null) {
        showMessageBox(tsrID, "green", "TSRDashboard.html");
    }

    formData.append("id", tsrId);
    var mes = "Success";
    $.ajax({
        url: urlPrefix + "TSR/TSRFileUpload/", type: "POST", processData: false,
        data: formData, dataType: 'json',
        contentType: false,
        async: false,
        success: function (response) {
            $("#fileUpload").val('');
            mes = "Success";
            showMessageBox(message, "green", "TSRDashboard.html");
        },
        error: function (er) {
            mes = er.statusText;
            showMessageBox(mes, "red");
        }

    });
    return mes;
}

var fileSequence = 0;
function showUpload(filePath) {
    //showLoader();
    var tbody = $('#tsrfiledetails');
    var array = filePath.split(",");
    var rows = $('#tsrfiledetails > tr');
    if (rows.length > 0) {
        fileSequence = rows.length;
    }
    else {
        fileSequence = 0;
    }
    for (var totalrecords = 0; totalrecords < array.length; totalrecords++) {
        fileSequence++;
        var FileName = array[totalrecords];
        FileName = FileName.substr(FileName.lastIndexOf("\\") + 1, FileName.length);

        var rows = $('#tsrfiledetails > tr');
        for (var rowindex = 0; rowindex < rows.length; rowindex++) {
            var td = rows[rowindex].children(0);
            if (typeof td != "undefined") {
                if (rows[rowindex].outerHTML.indexOf(FileName) > 0) {
                    showMessageBox("File with same name exist !", "red"); return false;
                }
            }
        }

        var className = "";
        if (fileSequence == array.length) {
            className = "class=lastRow";
        }
        tbody.append("<tr id='tr_" + fileSequence + "' style='min-height:20px !important; padding:5px 0px !important;' " + className + " data-name='" + FileName + "'>" +
            "<td align='center' style='width:13%;min-height:20px !important; padding:2px 0px !important;' id='tdIndex'>" + fileSequence + "</td>" +
            "<td  class='leftalign' style='width:76%;min-height:20px !important; padding:2px 0px !important;'>" + FileName + "</td>" +
            "<td align='center' style='width:6%;min-height:20px !important; padding:2px 0px !important;'>New</td>" +
            "<td align='center' style='width:5%;min-height:20px !important; padding:2px 0px !important;'><a href='#' class='remove' onclick='RemoveLocalFile(" + fileSequence + ")'><img style='width:12px;' src='/images/cross.png'/></a></td></tr>");
    }
    //hideLoader();
}

function RemoveFile(fileSequence) {
    var rows = $('#tsrfiledetails > tr');
    var Number = 0;
    for (var rowindex = 0; rowindex < rows.length; rowindex++) {
        var td = rows[rowindex].children(0);
        if (typeof td != "undefined") {
            if (rows[rowindex].id == "tr_" + fileSequence) {
                var guid = rows[rowindex].attributes["data-name"].value;
                if (guid.length > 5) {
                    var url = urlPrefix + "/TSR/RemoveTSRFile/" + guid;
                    $.ajax({
                        url: url,
                        type: 'POST',
                        contentType: 'application/json; charset=UTF-8',
                        beforeSend: function (request) {
                            request.setRequestHeader('userid', userId);
                        },
                        success: function (response) {
                            $('#tr_' + fileSequence).remove();
                            var rows = $('#tsrfiledetails > tr');
                            if (rows.length > 0) {
                                fileSequence = rows.length;
                            }
                        },
                        error: function (er) {
                            showMessageBox(er.responseText, "red"); return false;
                        }
                        //error: function (jqXHR, textStatus, errorThrown)
                        //{
                        //    if (errorThrown == "Unauthorized") {
                        //        textStatus = "You'r not authorised to do the current operation."
                        //    }
                        //    showMessageBox(er.responseText, "red"); return false;
                        //}
                    });
                }
            }
            else {
                Number++;
                $(td).text(Number);
            }
        }
    }
    $("#fileUpload").val("");
}

function RemoveLocalFile(fileSequence) {
    var rows = $('#tsrfiledetails > tr');
    var Number = 0;
    //formdata.reset();
    for (var rowindex = 0; rowindex < rows.length; rowindex++) {
        var td = rows[rowindex].children(0);
        if (typeof td != "undefined") {
            if (rows[rowindex].id == "tr_" + fileSequence) {
                var guid = rows[rowindex].attributes["data-name"].value;
                if (guid.length > 5) {
                    $('#tr_' + fileSequence).remove();
                    for (var i = 0; i < filesToUpload.length; ++i) {
                        if (filesToUpload[i].id === "files" + rowindex)
                            filesToUpload.splice(i, 1);
                    }
                }
            }
            else {
                Number++;
                $(td).text(Number);
            }
        }
    }
}

function SetAutoCompleteForAD(textboxId, lblId) {

  
    var url = urlPrefix + "User/GetUsersByNameOrEmail";

    $("#" + textboxId).autocomplete({
        minLength: 3,
        source: function (request, response) {
            var objLoginJSON = JSON.stringify({ userName: $("#" + textboxId).val(), password: '', adUser: false });
           
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: url,
                data: objLoginJSON,
                dataType: "json",
                success: function (userData) {
                    // Populate the array that will be passed to the response callback.
                    var autocompleteObjects = [];
                    if (userData != null) {
                        for (var i = 0; i < userData.length; i++) {
                            var object = {
                                value: userData[i].EmailID,
                                label: userData[i].Name,
                            };
                            console.log(object);
                            autocompleteObjects.push(object);
                        }
                    }
                    // Invoke the response callback.
                    response(autocompleteObjects);
                  
                },
                error: function (errormsg) {
                    showMessageBox(errormsg.responseText, "red");
                }
            });
        },
        select: function (event, ui) {
            if (ui.item.label.toLowerCase().indexOf("error") < 0) {
                $("#" + textboxId).val(ui.item.label); // display the selected text
                $("#" + lblId).val(ui.item.value); // save selected email to hidden input
            }
            else {
                $("#" + textboxId).val(""); // display the selected text
                $("#" + lblId).val("");
            }

            return false;
        },
    });
}

function GetDBId(label, labelID) {
    var lblLabel = $(label);
    var lbllabelID = $(labelID);
    var dd = lbllabelID.val();

    if (null != lblLabel && lblLabel.val() != "") {
        showLoader();
        var url = urlPrefix + "User/GetUserByEmail/";

        var adUser = true;

        var objLoginJSON = JSON.stringify({ userName: lblLabel.val(), password: '', adUser: adUser });
        $.ajax({
            type: "POST",
            url: url,
            contentType: 'application/json; charset=UTF-8',
            data: objLoginJSON,
            async: false,
            success: function (result) {
                if (null !== result) {
                    $(labelID).val(result.ID);
                }
                hideLoader();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoader();
                showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
            }
        });
    }
}

$('.SearchManager_link').click(function () {

    //$("#UserSearchdiv").css("visibility", "visible");
    //$('#UserSearchdiv').css("display", "block");

    var txtname = $(this).attr('name');
    var NameVal = $("#" + txtname).val();

    var txtid = $(this).attr('id');
    var idVal = $("#" + txtid).val();

    if (NameVal == "") {
        //$('#UserSearchdiv').html("");
        alert("Please enter valid name for search.");
        return false;
    }
    else {
        showLoader();
        
        var adUser = false;
        var url = urlPrefix + "User/GetUsersByNameOrEmail/";
        var objLoginJSON = JSON.stringify({ userName: NameVal, password: '', adUser: adUser });

        $.ajax({
            type: "POST",
            url: url,
            contentType: 'application/json; charset=UTF-8',
            data: objLoginJSON,
            async: false,
            success: function (result) {

                if (result != null) {
                    ////Table Header
                    //var tableheaders = new Array();
                    //tableheaders.push(["Name", "User ID"]);

                    ////Create a HTML Table element.
                    //var table = document.createElement("TABLE");

                    ////Get the count of columns.
                    //var columnCount = 2;

                    ////Add the header row.
                    //var row = table.insertRow(-1);
                    //for (var i = 0; i < columnCount; i++) {
                    //    var headerCell = document.createElement("TH");
                    //    headerCell.innerHTML = tableheaders[0][i];
                    //    row.appendChild(headerCell);
                    //}
                    $('#userdetails').empty();
                    var tbody = $('#userdetails');

                    for (var key in result) {
                        var checkboxName = result[key].Name;
                        var UserId = result[key].UserId;
                        tbody.append("<tr><td align='center'><input type='radio' title='" + checkboxName + "' name ='html_radio' value='" + UserId + "'></td>" +
                            "<td align='left'><label style='font-size:14px;' for='" + checkboxName + "'>" + checkboxName + "</label></td>" +
                            "<td align='left'>" + UserId + "</td></tr>");

                        //var checkboxName = result[key].Name;
                        //var UserId = result[key].UserId;

                        //var FirstCol = '<input type="radio" title="' + checkboxName + '" name ="html_radio" value="' + UserId + '">';
                        //FirstCol += '<label style="font-size:14px;padding-right:30px;padding-left:8px" for="' + checkboxName + '">' + checkboxName + '</label>';

                        //var SecondCol = '<label style="font-size:14px;padding-left:30px">' + UserId + '</label>';

                        //row = table.insertRow(-1);

                        ////First Cell
                        //var cell = row.insertCell(-1);
                        //cell.innerHTML = FirstCol;

                        ////Second Cell
                        //var cell = row.insertCell(-1);
                        //cell.innerHTML = SecondCol;

                    }


                    //var dvTable = document.getElementById("tabs-2");
                    //dvTable.innerHTML = "";
                    //dvTable.appendChild(table);

                }
                
                $("#UserSearchdiv").dialog({
                    height: "600",
                    width: "640",
                    modal: true,
                    closeOnEscape: false,
                    draggable: false,
                    resizable: false,
                    open: $.proxy(function () {                        
                        hideLoader();
                        $(".ui-dialog-titlebar-close").remove();
                        $(".ui-dialog-titlebar").css("margin-bottom", "5px");
                        $(".ui-dialog-titlebar").css("width", "94%");
                        $(".ui-dialog-buttonpane").css("width", "96%");
                        $(".ui-widget-overlay").css("opacity", "1");
                    }),
                    buttons: [
                            {
                                text: "Select",
                                click: function () {

                                    if (!$("input[name='html_radio']:checked").val()) {
                                        alert('Please select employee record.');
                                    }
                                    else {
                                        var data = $("input[name='html_radio']:checked").val();
                                        var txttitle = $("input[name='html_radio']:checked").attr('title');

                                        $("#" + txtname).val(txttitle);

                                        //Create New User Record.
                                        var url = urlPrefix + "User/GetUserByEmail/";
                                        var adUser = true;
                                        var objLoginJSON = JSON.stringify({ userName: data, password: '', adUser: adUser });
                                        $.ajax({
                                            type: "POST",
                                            url: url,
                                            contentType: 'application/json; charset=UTF-8',
                                            data: objLoginJSON,
                                            async: false,
                                            success: function (result) {
                                                if (null !== result) {
                                                    $("#" + txtid).val(result.ID);
                                                }
                                                hideLoader();
                                            },
                                            error: function (jqXHR, textStatus, errorThrown) {
                                                hideLoader();
                                                showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
                                            }
                                        });

                                        $(this).dialog("close");
                                    }
                                }
                            },
                            {
                                text: "Cancel",
                                click: function () {
                                    $(this).dialog("close");
                                }
                            }
                    ]
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoader();
                showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
            }
        });

        hideLoader();
    }
});

