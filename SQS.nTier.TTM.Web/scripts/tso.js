﻿function LoadOperationalRisk() {
    var url = urlPrefix + "OperationalRisk/GetAllIDName";
    LoadData(url, 'ddlOperationalrisk');
}

function LoadCoreServices() {
    var url = urlPrefix + "CoreService/GetAllIDNameByTSRId/" + tsrId;
    LoadData(url, 'ddlCoreServices');
}

function LoadReliventRepositories() {
    var url = urlPrefix + "RelevantRepository/GetAllIDNameByTSRId/" + tsrId;
    LoadData(url, 'ddlRelevantRepositories');
}

function LoadServiceDeliveryChain() {
    var url = urlPrefix + "ServiceDeliveryChain/GetAllIDName";
    LoadData(url, 'listServiceDeliveryChain');
}

function LoadInputType() {
    var url = urlPrefix + "ProductivityInput/GetAllIDName";
    LoadData(url, 'listInputType');
}

function LoadOutputType() {
    var url = urlPrefix + "ProductivityOutcome/GetAllIDName";
    LoadData(url, 'listOutputType');
}

function LoadTSOStatus() {
    var url = urlPrefix + "TSOStatus/GetAllIDName";
    LoadData(url, 'ddlTSOStatus');
}

function LoadData(url, dropdown) {
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader("userid", userId);
        },
        dataType: "json",
        async: false,
        success: function (result) {
            var totalRecords = result.length;
            if (!isNaN(totalRecords) && parseInt(totalRecords) > 0) {
                PopulateDropdown(result, dropdown);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

var objTSO = null;
var objTSR = null;
function LoadTSR(tsrId) {
    var url = urlPrefix + "TSR/GetById/" + tsrId;

    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader("userid", userId);
        },
        dataType: "json",
        success: function (result) {
            if (null != result) {
                //debugger;
                objTSR = result;
                addToBreadcrumbArray(result.Title, result.ID, "TSR", true);
                if (objTSO != null && objTSO != "") {
                    addToBreadcrumbArray(objTSO.Title, objTSO.ID, "TSO", false);
                }
                CreateBreadcrumb();
                $("#lblTSR").html(result.Title);
                if (roleId.toLowerCase() != "admin") {
                    if (result.TSRStatus.Name == "Closed" || result.TSRStatus.Name == "Cancelled") {
                        $("#CreateNewTSO").hide();
                        $("#Submit").hide();
                        $("#txtTitle").attr('disabled', true);
                        $("#txtDescription").attr('disabled', true);
                        $("#ddlTSOStatus").attr('disabled', true);
                        $("#ddlOperationalrisk").attr('disabled', true);

                        $("#txtTeamLead").attr('disabled', true);
                        $("#ddlCoreServices").attr('disabled', true);
                        //$("#ddlRelevantRepositories").attr('disabled', true);
                        $("#ddlRelevantRepositories").prop('disabled', true);
                        $("#AddServiceDeliveryChain").attr('disabled', true);
                        $("#RemoveServiceDeliveryChain").attr('disabled', true);


                        $("#txtStartDate").attr('disabled', true);
                        //$("#txtActualStartDate").attr('disabled', true);
                        $("#txtTargetCompletionDate").attr('disabled', true);
                        //$("#txtActualCompletionDate").attr('disabled', true);
                        $("#txtEstimatedEffort").attr('disabled', true);
                        $("#txtPlannedEffort").attr('disabled', true);
                        $("#txtSpId").attr('disabled', true);
                        $("#SubmitTSO").hide();
                    }
                        //else if (result.TSRStatus.Name == "On Hold" || result.TSRStatus.Name == "In Progress") {
                        //    $("#ddlRelevantRepositories").attr('disabled', true);
                        //    $("#AddServiceDeliveryChain").attr('disabled', true);
                        //    $("#RemoveServiceDeliveryChain").attr('disabled', true);
                        //}               
                    else {
                        if (result.TSRReleventRepositories.length == 0) {
                            $("#ddlRelevantRepositories").attr('disabled', true);
                        }
                        else {
                            $("#ddlRelevantRepositories").attr('disabled', false);
                        }
                        $("#AddServiceDeliveryChain").attr('disabled', false);
                        $("#RemoveServiceDeliveryChain").attr('disabled', false);

                        $("#CreateNewTSO").show();
                        $("#Submit").show();
                        $("#SubmitTSO").show();
                    }
                }
                else {
                    $("#CreateNewTSO").show();
                    $("#Submit").show();
                    $("#SubmitTSO").show();
                }
                $("#lblTSR").prop("title", "Delivery Manager: " + result.DeliveryManager.Name +
                    "\nTest Manager: " + result.TestManager.Name +
                    "\nAccount Manager: " + result.AccountManager.Name +
                    "\nVertical: " + result.Vertical.Name +
                    "\nPractice: " + result.Practice.Name +
                    "\nSolution Centre: " + result.SolutionCentre.Name +
                    "\nClient Region: " + result.ClientRegion.Name +
                    "\nClient: " + result.Client +
                    "\nAccount: " + result.Account +
                    "\nEngagement: " + result.Engagement +
                    "\nERPordernumber: " + result.ERPordernumber +
                    "\nStartDate: " + dateFormat(result.StartDate.replace("T00:00:00", ""), "isoDateddMonyyyy") +
                    "\nTargetCompletionDate: " + dateFormat(result.TargetCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy") +
                    "\nEstimatedeffort: " + result.Estimatedeffort + " days" +
                    "\nPlannedeffort: " + result.Plannedeffort + " days"
                );

                //set the start and completion dates in hidden fields for validation
                $("#TSRStartDate").val(dateFormat(result.StartDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                $("#TSRTargetCompletionDate").val(dateFormat(result.TargetCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                $("#TSRPlannedEffort").val(result.Plannedeffort);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

function LoadTSO(tsoId) {
    var url = urlPrefix + "TSO/GetById/" + tsoId;

    $.ajax({
        type: "GET",
        beforeSend: function (request) {
            request.setRequestHeader("userid", userId);
        },
        url: url,
        dataType: "json",
        success: function (result) {
            if (null != result) {
                if (tsrId == 'undefined' || tsrId == "null") {
                    LoadFromTSOCoreServices(result.TSRId);
                    LoadFromTSOReliventRepositories(result.TSRId);
                }
                PopulateTSO(result);
            }
            hideLoader();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (errorThrown == "Unauthorized") {
                textStatus = "You'r not authorised to do the current operation."
            }
            localStorage.setItem("tsrid", tsrId);
            showMessageBox(textStatus, "red", "TSODashboard.html");

        }
    });
}


function LoadFromTSOCoreServices(objtsrId) {
    //debugger;
    var url = urlPrefix + "CoreService/GetAllIDNameByTSRId/" + objtsrId;
    LoadData(url, 'ddlCoreServices');
}

function LoadFromTSOReliventRepositories(objtsrId) {
    var url = urlPrefix + "RelevantRepository/GetAllIDNameByTSRId/" + objtsrId;
    LoadData(url, 'ddlRelevantRepositories');
}

function PopulateTSO(result) {
    objTSO = result;
    LoadTSR(result.TSRId);
    $("#txtID").val(result.ID);
    $("#txtTitle").val(result.Title);
    $("#txtTitle").prop('disabled', 'disabled');
    $("#txtDescription").val(result.Description);
    $("#txtTeamLead").val(result.TeamLead.Name)
    $("#lblTeamLeadMail").val(result.TeamLead.EmailID)
    $("#lblTeamLeadId").val(result.TeamLeadId);
    $("#txtActualEffort").val(result.ActualEffort);
    $("#txtSpId").val(result.SP_Id);
    $("#txtEffortConsumption").val(((result.ActualEffort / result.PlannedEffort) * 100).toFixed(2));
    if (null != result.ActualOutcome) {
        $("#txtActualOutcome").val((result.ActualOutcome).toFixed(2));
    }
    if (null != result.OutomeCompletion) {
        $("#txtDegreeOfCompletion").val(result.OutomeCompletion.toFixed(2));
    }

    $("#ddlCoreServices").val(result.CoreServiceId);
    $("#ddlRelevantRepositories").val(result.RelevantRepositoryId);

    startDate = $.datepicker.parseDate('yy-m-dd', result.StartDate.replace("T00:00:00", ""));

    $("#txtStartDate").val(dateFormat(startDate, "isoDateddMonyyyy"));

    targetCompletionDate = $.datepicker.parseDate('yy-m-dd', result.TargetCompletionDate.replace("T00:00:00", ""));

    $("#txtTargetCompletionDate").val(dateFormat(targetCompletionDate, "isoDateddMonyyyy"));
    $("#txtEstimatedEffort").val(result.EstimatedEffort);
    $("#txtPlannedOutcome").val(result.PlannedOutcome);
    $("#txtPlannedEffort").val(result.PlannedEffort);
    $("#ddlOperationalrisk").val(result.OperationalRiskId);
    $("#ddlTSOStatus").val(result.TSOStatusID);
    $("#lastUpdatedOn").val(dateFormat(result.UpdatedOn, "dd-mmm-yyyy HH:MM:ss TT"));
    $("#lastUpdatedBy").val(result.UpdatedBy);

    if (null !== result.TSRReleventRepositories && result.TSOServiceDeliveryChains.length > 0) {
        for (var iTSOServiceDeliveryChainsCount = 0; iTSOServiceDeliveryChainsCount < result.TSOServiceDeliveryChains.length; iTSOServiceDeliveryChainsCount++) {
            $('#listServiceDeliveryChain option').each(function () {
                if (parseInt(this.value) == parseInt(result.TSOServiceDeliveryChains[iTSOServiceDeliveryChainsCount].ServiceDeliveryChainId)) {
                    $("#listServiceDeliveryChain option[value='" + result.TSOServiceDeliveryChains[iTSOServiceDeliveryChainsCount].ServiceDeliveryChainId + "']").prop("selected", true);
                    return;
                }
            });
        }
    }

    moveItems('#listServiceDeliveryChain', '#listServiceDeliveryChainSelected');

    //Input Type
    if (result.TSOProductivityInputs.length > 0) {
        for (var iInputTypesCount = 0; iInputTypesCount < result.TSOProductivityInputs.length; iInputTypesCount++) {
            $('#listInputType option').each(function () {
                if (parseInt(this.value) == parseInt(result.TSOProductivityInputs[iInputTypesCount].ProductivityInputId)) {
                    $("#listInputType option[value='" + result.TSOProductivityInputs[iInputTypesCount].ProductivityInputId + "']").prop("selected", true);
                    return;
                }
            });
        }
    }

    moveItems('#listInputType', '#listInputTypeSelected');

    //Output Type
    if (result.TSOProductivityOutcomes.length > 0) {
        for (var iOutcomeTypeCount = 0; iOutcomeTypeCount < result.TSOProductivityOutcomes.length; iOutcomeTypeCount++) {
            $('#listOutputType option').each(function () {
                if (parseInt(this.value) == parseInt(result.TSOProductivityOutcomes[iOutcomeTypeCount].ProductivityOutcomeId)) {
                    $("#listOutputType option[value='" + result.TSOProductivityOutcomes[iOutcomeTypeCount].ProductivityOutcomeId + "']").prop("selected", true);
                    return;
                }
            });
        }
    }

    moveItems('#listOutputType', '#listOutputTypeSelected');

    if (roleId.toLowerCase() != "admin") {
        $("#txtEstimatedEffort").prop('disabled', 'disabled');
        $("#ddlCoreServices").attr('disabled', true);
        $("#ddlRelevantRepositories").prop('disabled', true);     
        $("#txtStartDate").attr('disabled', true);
        $("#txtTargetCompletionDate").attr('disabled', true);
        $("#txtPlannedEffort").attr('disabled', true);

        disableTSO(result.TSOStatus);
    }

    disableLists(true);
    $("#ddlOperationalrisk").prop('disabled', 'disabled');
}

function disableLists(isDis) {
    $("#listInputType").attr('disabled', isDis);
    $("#AddInputType").attr('disabled', isDis);
    $("#RemoveInputType").attr('disabled', isDis);
    $("#listInputTypeSelected").attr('disabled', isDis);

    $("#listOutputType").attr('disabled', isDis);
    $("#AddOutputType").attr('disabled', isDis);
    $("#RemoveOutputType").attr('disabled', isDis);
    $("#listOutputTypeSelected").attr('disabled', isDis);

    $("#listServiceDeliveryChain").attr('disabled', isDis);
    $("#AddServiceDeliveryChain").attr('disabled', isDis);
    $("#RemoveServiceDeliveryChain").attr('disabled', isDis);
    $("#listServiceDeliveryChainSelected").attr('disabled', isDis);
}


function disableTSO(result) {

    if (result.Name == "Closed" || result.Name == "Cancelled") {

        $("#txtTitle").attr('disabled', true);
        $("#txtDescription").attr('disabled', true);
        $("#ddlTSOStatus").attr('disabled', true);
        $("#ddlOperationalrisk").attr('disabled', true);

        $("#txtTeamLead").attr('disabled', true);
        $("#ddlCoreServices").attr('disabled', true);
        $("#ddlRelevantRepositories").prop('disabled', true);
        $("#AddServiceDeliveryChain").attr('disabled', true);
        $("#RemoveServiceDeliveryChain").attr('disabled', true);

        $("#txtStartDate").attr('disabled', true);
        $("#txtTargetCompletionDate").attr('disabled', true);
        $("#txtEstimatedEffort").attr('disabled', true);
        $("#txtPlannedEffort").attr('disabled', true);
        $("#txtSpId").attr('disabled', true);
        $("#CreateNewTSO").hide();
        $("#SubmitTSO").attr('disabled', true);
    }
    else if (result == "On Hold" || result == "In Progress") {
        $("#ddlRelevantRepositories").attr('disabled', true);
        $("#AddServiceDeliveryChain").attr('disabled', true);
        $("#RemoveServiceDeliveryChain").attr('disabled', true);
    }


}
function PopulateDropdown(result, dropdown) {
    $.each(result, function (i, item) {
        $('#' + dropdown).append($('<option>', {
            value: item.ID,
            text: item.Name
        }));
    });
}

function Save() {
    showLoader();
    if (CheckTSOkData()) {
        var isChange = false;
        var txtID = $("#txtID").val();
        var txtTitle = $("#txtTitle").val().trim();
        var txtDescription = $("#txtDescription").val().trim();
        var txtTeamLead = $("#lblTeamLeadId").val();
        var txtSp_Id = $("#txtSpId").val();
        var ddlCoreServices = $("#ddlCoreServices").val();
        var ddlRelevantRepositories = $("#ddlRelevantRepositories").val();
        var txtStartDate = $("#txtStartDate").val();
        var txtTargetCompletionDate = $("#txtTargetCompletionDate").val();
        var txtEstimatedEffort = $("#txtEstimatedEffort").val();
        var txtPlannedOutcome = $("#txtPlannedOutcome").val();
        var txtPlannedEffort = $("#txtPlannedEffort").val();
        var ddlOperationalrisk = $("#ddlOperationalrisk").val();
        var ddlTSOStatus = $("#ddlTSOStatus").val();

        var listServiceDeliveryChainArr = '';


        $('#listServiceDeliveryChainSelected').find('option').each(function () {
            listServiceDeliveryChainArr += $(this).val();
            listServiceDeliveryChainArr += ",";
        });
        listServiceDeliveryChainArr = listServiceDeliveryChainArr.substring(0, listServiceDeliveryChainArr.length - 1);


        var listInputTypeArr = '';

        $('#listInputTypeSelected').find('option').each(function () {
            listInputTypeArr += $(this).val();
            listInputTypeArr += ",";
        });
        listInputTypeArr = listInputTypeArr.substring(0, listInputTypeArr.length - 1);

        var listOutputTypeArr = '';

        $('#listOutputTypeSelected').find('option').each(function () {
            listOutputTypeArr += $(this).val();
            listOutputTypeArr += ",";
        });
        listOutputTypeArr = listOutputTypeArr.substring(0, listOutputTypeArr.length - 1);

        var CanClose = true;
        if (objTSO != null) {
            if (txtDescription != objTSO.Description || txtTeamLead != objTSO.TeamLead.ID) {
                isChange = true;
            }

            var start = $.datepicker.parseDate('yy-m-dd', objTSO.StartDate.replace("T00:00:00", ""));
            start = dateFormat(startDate, "isoDateddMonyyyy");

            var TargetCompletionDate = $.datepicker.parseDate('yy-m-dd', objTSO.TargetCompletionDate.replace("T00:00:00", ""));
            TargetCompletionDate = dateFormat(TargetCompletionDate, "isoDateddMonyyyy");

            if (txtStartDate != start || txtTargetCompletionDate != TargetCompletionDate) {
                isChange = true;
            }


            if (txtEstimatedEffort != objTSO.EstimatedEffort || txtPlannedEffort != objTSO.PlannedEffort || txtPlannedOutcome != objTSO.PlannedOutcome) { //
                isChange = true;
            }

            if (ddlOperationalrisk != objTSO.OperationalRiskId ||
                       ddlTSOStatus != objTSO.TSOStatusID) {
                isChange = true;
            }

            if (ddlRelevantRepositories != objTSO.RelevantRepositoryId || ddlTSOStatus != objTSO.TSOStatusID) {
                isChange = true;
            }
            CanClose = objTSO.CanClose;

            var OldChains = "";
            for (var i = 0; i < objTSO.TSOServiceDeliveryChains.length; i++) {
                OldChains = OldChains + objTSO.TSOServiceDeliveryChains[i].ServiceDeliveryChainId + ",";
            }
            if (OldChains != listServiceDeliveryChainArr) {
                isChange = true;
            }

            var OldInput = "";
            for (var i = 0; i < objTSO.TSOProductivityInputs.length; i++) {
                OldInput = OldInput + objTSO.TSOProductivityInputs[i].ProductivityInputId + ",";
            }
            if (OldInput != listInputTypeArr) {
                isChange = true;
            }


            var OldOutcome = "";
            for (var i = 0; i < objTSO.TSOProductivityOutcomes.length; i++) {
                OldOutcome = OldOutcome + objTSO.TSOProductivityOutcomes[i].ProductivityOutcomeId + ",";
            }
            if (OldOutcome != listOutputTypeArr) {
                isChange = true;
            }

            checkCloseOnSubmit('ddlTSOStatus', objTSO.TSOStatusID);
        }
        else {
            isChange = true;
        }

        if (!isChange && (typeof txtID != "undefined" && txtID != "" && txtID != null)) {
            hideLoader();
            showMessageBox("No updation seen !", "red");
            return false;
        }



        var varddlTSOStatus = $("#ddlTSOStatus option:selected").text();
        if (varddlTSOStatus == "-- Select --") {
            showMessageBox("Please select the status of TSO !", "red");
            return false;
        }

        if (isChange && !CanClose && (varddlTSOStatus == "Closed" || varddlTSOStatus == "Cancelled")) {
            hideLoader();
            showMessageBox("The Tasks needs to be closed first before TSO can  be closed !", "red");
            return false;
        }


        if (listServiceDeliveryChainArr.trim() == "") {
            hideLoader();
            showMessageBox("Please select at least one service delivery chain.", "red");
            return false;
        }

        if (listInputTypeArr.trim() == "") {
            hideLoader();
            showMessageBox("Please select at least one input type.", "red");
            return false;
        }

        if (listOutputTypeArr.trim() == "") {
            hideLoader();
            showMessageBox("Please select at least one outcome type.", "red");
            return false;
        }



        var data = JSON.stringify({
            TSRId: tsrId, Title: txtTitle, Description: txtDescription, TeamLeadId: txtTeamLead, PlannedOutcome: txtPlannedOutcome,
            TSOStatusID: ddlTSOStatus, CoreServiceId: ddlCoreServices, RelevantRepositoryId: ddlRelevantRepositories, StartDate: txtStartDate,
            TargetCompletionDate: txtTargetCompletionDate, Estimatedeffort: txtEstimatedEffort, Plannedeffort: txtPlannedEffort,
            OperationalRiskId: ddlOperationalrisk, ServiceDeliveryChainArr: listServiceDeliveryChainArr, CreatedBy: userName, SP_Id: txtSp_Id,
            UpdatedBy: userName, ProductivityInputsArr: listInputTypeArr, ProductivityOutcomesArr: listOutputTypeArr // ActualStartDate: txtActualStartDate, ActualCompletionDate: txtActualCompletionDate,
        });

        var urlAddUpdate = urlPrefix + "TSO";

        var method = "POST";
        var message = "";
        if (!isNaN(txtID) && txtID != "") {
            method = "PUT";
            urlAddUpdate = urlAddUpdate + "/UpdateTSO/" + txtID;
            message = "TSO updated successfully.";
        }
        else {
            urlAddUpdate = urlAddUpdate + "/CreateTSO";
            message = "TSO created successfully.";
        }

        $.ajax({
            url: urlAddUpdate,
            data: data,
            type: method,
            beforeSend: function (request) {
                request.setRequestHeader("userid", userId);
            },
            contentType: 'application/json; charset=UTF-8',
            success: function (response) {
                hideLoader();
                //$("#txtOutput").html("<b>" + response + "</b>");
                if (response.indexOf("Error") == 0) {
                    showMessageBox(response, "red");
                    //$("#txtOutput").css('color', 'red');
                }
                else {
                    localStorage.setItem("tsrid", tsrId);
                    showMessageBox(message, "green", "TSODashboard.html");// + "&rowToselect=" + response);
                    //callSuccess(message);
                    //$("#txtOutput").css('color', 'green');
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                hideLoader();
                if (errorThrown == "Unauthorized") {
                    textStatus = "You are not authorized to do the current operation."
                }
                localStorage.setItem("tsrid", tsrId);
                showMessageBox(textStatus, "red", "TSODashboard.html");
            }
        });
    }
}

function Cancel() {
    localStorage.setItem("tsrid", tsrId);
    window.location = "TSODashboard.html";
}

function callSuccess(message) {
    localStorage.setItem("tsrid", tsrId);
    window.location = "TSODashboard.html";
    //alert(message);
}

function CheckTSOkData() {

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


    if ($("#txtTitle").val().trim() == "") {
        showMessageBox("Please enter TSO title.", "red", "", false, "txtTitle");
        $("#txtTitle").val().trim();
        return false;
    }
    else if ($("#txtSpId").val() == "") {
        showMessageBox("Please enter SP_Id.", "red", "", false, "txtSpId");
        return false;
    }
    else if ($("#txtDescription").val().trim() == "") {
        showMessageBox("Please enter description.", "red", "", false, "txtDescription");
        return false;
    }
    else if ($("#txtDescription").val() == "") {
        showMessageBox("Please enter description.", "red", "", false, "txtDescription");
        return false;
    }
    else if ($("#ddlCoreServices").val() == "") {
        showMessageBox("Please select core services.", "red", "", false, "ddlCoreServices");
        return false;
    }
    else if ($("#lblTeamLeadId").val() == "") {
        showMessageBox("Please enter Team lead's name or mail id.", "red", "", false, "txtTeamLead");
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
        showMessageBox("Planned completion date can not be less than or equal to  planned start date.", "red", "", false, "txtTargetCompletionDate");
        return false;
    }
    else if ($.datepicker.parseDate('dd-M-yy', $("#TSRStartDate").val()) > $.datepicker.parseDate('dd-M-yy', $("#txtStartDate").val())) {
        showMessageBox("TSO planned start date can not be less than TSR planned start date.", "red", "", false, "txtStartDate");
        return false;
    }
    else if ($.datepicker.parseDate('dd-M-yy', $("#txtTargetCompletionDate").val()) > $.datepicker.parseDate('dd-M-yy', $("#TSRTargetCompletionDate").val())) {
        showMessageBox("Planned completion date can not be greater than TSR planned completion date date.", "red", "", false, "txtTargetCompletionDate");
        return false;
    }
    //else if (parseFloat($("#txtPlannedEffort").val()) > parseFloat($("#TSRPlannedEffort").val())) {
    //    showMessageBox("TSO Planned effort can not be greater than TSR planned effort .", "red", "", false, "txtPlannedEffort");
    //    return false;
    //}
    else if ($("#txtPlannedOutcome").val() == "") {
        showMessageBox("Please enter planned outcome.", "red", "", false, "txtPlannedOutcome");
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
    else if ($("#ddlOperationalrisk").val() == "") {
        showMessageBox("Please select operational risk.", "red", "", false, "ddlOperationalrisk");
        return false;
    }
    else if ($("#ddlRelevantRepositories").val() == "") {
        showMessageBox("Please select relevant repositories.", "red", "", false, "ddlRelevantRepositories");
        return false;
    }
    else if ($("#ddlCoreServices").val() == "" || $("#ddlCoreServices").val() == null) {
        showMessageBox("Please select core service.", "red", "", false, "ddlCoreServices");
        return false;
    }
    else if ($("#listServiceDeliveryChainSelected").has('option').length == "") {
        showMessageBox("Please select service delivery chain.", "red", "", false, "listServiceDeliveryChainSelected");
        return false;
    }
    else if ($("#listInputTypeSelected").has('option').length == "") {
        showMessageBox("Please select input type.", "red", "", false, "listInputTypeSelected");
        return false;
    }
    else if ($("#listOutputTypeSelected").has('option').length == "") {
        showMessageBox("Please select output type.", "red", "", false, "listOutputTypeSelected");
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
    var goAhead = true;
    if (objTSO != null) {
        if (origin == "#listServiceDeliveryChainSelected") {
            var selected = $(origin).find(':selected');
            $(selected).each(function (index) {
                var dd = this.value;
                var ff = jQuery.inArray(this.value, objTSO.CloseTaskIds);
                if (jQuery.inArray(this.value, objTSO.CloseTaskIds) > -1) {
                    showMessageBox("You can not remove closed task.", "red", "", false, "listServiceDeliveryChainSelected");
                    goAhead = false;
                }
            });
        }
        else if (origin == "#listInputTypeSelected") {
            var selected = $(origin).find(':selected');
            $(selected).each(function (index) {
                var dd = this.value;
                var ff = jQuery.inArray(this.value, objTSO.CloseTaskIds);
                if (jQuery.inArray(this.value, objTSO.CloseTaskIds) > -1) {
                    showMessageBox("You can not remove closed task.", "red", "", false, "listInputTypeSelected");
                    goAhead = false;
                }
            });
        }
        else if (origin == "#listOutputTypeSelected") {
            var selected = $(origin).find(':selected');
            $(selected).each(function (index) {
                var dd = this.value;
                var ff = jQuery.inArray(this.value, objTSO.CloseTaskIds);
                if (jQuery.inArray(this.value, objTSO.CloseTaskIds) > -1) {
                    showMessageBox("You can not remove closed task.", "red", "", false, "listOutputTypeSelected");
                    goAhead = false;
                }
            });
        }
    }

    //if (origin == "#listInputTypeSelected") {
    //    var selected = $(origin).find(':selected');
    //    if (selected.length == 1) {
    //        showMessageBox("You can not remove all input type.", "red", "", false, "listOutputTypeSelected");
    //        goAhead = false;
    //    }
    //}
    //else if (origin == "#listOutputTypeSelected") {
    //    var selected = $(origin).find(':selected');
    //    if (selected.length == 1) {
    //        showMessageBox("You can not remove all outcome type.", "red", "", false, "listOutputTypeSelected");
    //        goAhead = false;
    //    }
    //}

    if (goAhead) {
        $(origin).find(':selected').appendTo(dest);
    }

    $(dest).append($(dest + " option").prop('selected', false));
}

$(function () {

    //--Service delivery chain
    $('#AddServiceDeliveryChain').click(function () {
        moveItems('#listServiceDeliveryChain', '#listServiceDeliveryChainSelected');
    });

    $('#RemoveServiceDeliveryChain').on('click', function () {
        moveItems('#listServiceDeliveryChainSelected', '#listServiceDeliveryChain');
    });

    // Input
    $('#AddInputType').click(function () {
        moveItems('#listInputType', '#listInputTypeSelected');
    });

    $('#RemoveInputType').on('click', function () {
        moveItems('#listInputTypeSelected', '#listInputType');
    });

    // Output
    $('#AddOutputType').click(function () {
        moveItems('#listOutputType', '#listOutputTypeSelected');
    });

    $('#RemoveOutputType').on('click', function () {
        moveItems('#listOutputTypeSelected', '#listOutputType');
    });

    $("#txtTeamLead").focusout(function () {
        if ($(this).val() != "") {
            GetDBId("#lblTeamLeadMail", "#lblTeamLeadId");
        }
    });
});

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
                beforeSend: function (request) {
                    request.setRequestHeader("userid", userId);
                },
                dataType: "json",
                success: function (userData) {
                    // Populate the array that will be passed to the response callback.
                    var autocompleteObjects = [];
                    for (var i = 0; i < userData.length; i++) {
                        var object = {
                            value: userData[i].EmailID,
                            label: userData[i].Name,
                        };

                        autocompleteObjects.push(object);
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
            beforeSend: function (request) {
                request.setRequestHeader("userid", userId);
            },
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

    $("#UserSearchdiv").css("visibility", "visible");
    $('#UserSearchdiv').css("display", "block");

    var txtname = $(this).attr('name');
    var NameVal = $("#" + txtname).val();

    var txtid = $(this).attr('id');
    var idVal = $("#" + txtid).val();

    if (NameVal == "") {
        $('#UserSearchdiv').html("");
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
                showLoader();
                if (result != null) {

                    //Table Header
                    var tableheaders = new Array();
                    tableheaders.push(["Name", "User ID"]);

                    //Create a HTML Table element.
                    var table = document.createElement("TABLE");

                    //Get the count of columns.
                    var columnCount = 2;

                    //Add the header row.
                    var row = table.insertRow(-1);
                    for (var i = 0; i < columnCount; i++) {
                        var headerCell = document.createElement("TH");
                        headerCell.innerHTML = tableheaders[0][i];
                        row.appendChild(headerCell);
                    }

                    for (var key in result) {

                        var checkboxName = result[key].Name;
                        var UserId = result[key].UserId;

                        var FirstCol = '<input type="radio" title="' + checkboxName + '" name ="html_radio" value="' + UserId + '">';
                        FirstCol += '<label style="font-size:14px;padding-right:30px;padding-left:8px" for="' + checkboxName + '">' + checkboxName + '</label>';

                        var SecondCol = '<label style="font-size:14px;padding-left:30px">' + UserId + '</label>';

                        row = table.insertRow(-1);

                        //First Cell
                        var cell = row.insertCell(-1);
                        cell.innerHTML = FirstCol;

                        //Second Cell
                        var cell = row.insertCell(-1);
                        cell.innerHTML = SecondCol;

                    }

                    var dvTable = document.getElementById("tabs-2");
                    dvTable.innerHTML = "";
                    dvTable.appendChild(table);

                    hideLoader();
                }

                $("#UserSearchdiv").dialog({
                    dialogClass: "no-close block",
                    width: "500px",
                    hight: "250px",
                    modal: true,
                    open: $.proxy(function () {
                        $(".ui-dialog-titlebar-close").remove();
                        $(".ui-dialog-titlebar").css("width", "93%");
                        $(".ui-dialog-buttonpane").css("width", "86%");
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
                                        hideLoader();
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
