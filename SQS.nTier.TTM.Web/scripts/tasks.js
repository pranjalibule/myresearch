﻿var taskName;
var oldTask = null;
var currentweek;
function LoadServiceDeliveryChain() {
    var url = urlPrefix + "ServiceDeliveryChain/GetById/" + serviceDeliveryChainId;
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader('userid', userId);
        },
        dataType: "json",
        async: false,
        success: function (result) {
            if (null != result) {
                taskName = result.Name + "(" + result.Description + ")";
                $("#lblTask").val(taskName);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (errorThrown == "Unauthorized") {
                textStatus = "You are not authorized to do the current operation."
            }
            showMessageBox(textStatus, "red");
        }
    });
}

function LoadServiceDeliveryChainMethod(ServiceDeliveryChainId) {

    var url = urlPrefix + "Method/GetAllMethodIDName/" + ServiceDeliveryChainId;
    LoadData(url, 'ddlMethod');
}
function LoadLastTSOServiceDeliveryChainTask() {
    var url = urlPrefix + "TSO/GetLastTask/" + tsoServiceDeliveryChainId;
    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader('userid', userId);
        },
        dataType: "json",
        async: false,
        success: function (result) {
            if (null != result && result.ID > 0) {
                oldTask = result;
                if (result.CurrentWeek > result.WeekNumber) {
                    if (result.TaskStatusId == 0) {
                        $("#ddlTaskStatus option:contains('Created')").attr('selected', 'selected');
                    }
                    else {
                        $("#ddlTaskStatus").val(result.TaskStatusId);
                        $("#ddlMethod").val(result.ServiceDeliveryChainMethodId);
                        $("#txtStartDate").val(dateFormat(result.PlannedStartDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                        $("#txtTargetCompletionDate").val(dateFormat(result.PlannedCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                        $("#lbleffort").text(result.ActualEffortCumulative);
                        $("#lblActualOutcome").text(result.ActualOutcomeCumulative);
                        $("#lblActualOutcomeTestSteps").text(result.ActualOutcomeTestStepsCumulative);
                        $("#lblActualProcessingTime").text(result.ActualProcessingTimeCumulative);
                        $("#lblActualInputCumulative").text(result.ActualInputCumulative);

                        $("#lblPlannedeffort").text(result.PlannedEffortCumulative);
                        $("#lblPlannedActualOutcome").text(result.PlannedOutcomeCumulative);
                        $("#lblPlannedActualOutcomeTestSteps").text(result.PlannedOutcomeTestStepsCumulative);
                        $("#lblPlannedActualProcessingTime").text(result.PlannedProcessingTimeCumulative);
                        $("#lblPlannedInputCumulative").text(result.PlannedInputCumulative);
                        $("#txtTaskActualOid").val(result.TaskActualID);
                        $("#txtTaskPlannedOid").val(result.TaskPlannedId);
                        $("#txtTSOOid").val(result.TSOId);
                        $("#lastUpdatedOn").val(dateFormat(result.UpdatedOn, "dd-mmm-yyyy HH:MM:ss TT"));
                        $("#lastUpdatedBy").val(result.UpdatedBy);
                        $("#txtoperationalRisk").val(result.OperationalRisk);
                        
                        //if (result.ServiceDeliveryChainId == 42 || result.ServiceDeliveryChainId == 46) {
                        //    $('#txtResponsiblePerson').val(result.ResponsiblePerson.Name);
                        //    $("#lblResponsiblePersonMail").val(result.ResponsiblePerson.EmailID)
                        //    $("#lblResponsiblePersonId").val(result.ResponsiblePersonId);
                        //}

                        $("#txtTaskServiceChainIdOid").val(result.ServiceDeliveryChainId);

                        if (roleId.toLowerCase() != "admin") {
                            if ($("#ddlTaskStatus option:selected").text() == "Cancelled" || $("#ddlTaskStatus option:selected").text() == "Closed") {
                                $("#TaskInfo, #SubmitTask").attr("disabled", "disabled");
                                $("#ddlTaskStatus").attr("disabled", "disabled");
                                $("#ddlMethod").attr("disabled", "disabled");
                                $("#txtNotes").prop("disabled", true);
                                $("#txtHeadCount").prop("disabled", true);
                                $("#txtIdleTimeDu").prop("disabled", true);
                                $("#txtActualEffort").prop("disabled", true);
                                disablesActualPlanned();

                                $("#txtActualOutcomeTestSteps").prop("disabled", true);
                                $("#txtActualProcessingTime").prop("disabled", true);
                                $("#txtActualProductivityEB").prop("disabled", true);
                                $("#txtActualProductivityTB").prop("disabled", true);
                                $("#txtPlannedProcessingTime").prop("disabled", true);
                                $("#txtPlannedOutcomeTestSteps").prop("disabled", true);


                                $("#txtPlannedEffort").prop("disabled", true);
                                $("#txtPlannedServiceQuality").prop("disabled", true);
                                $("#txtDefectsRaised").prop("disabled", true);
                                $("#txtDefectDensity").prop("disabled", true);
                                $("#txtDefectsRejected").prop("disabled", true);
                                $("#txtActualServiceQuality").prop("disabled", true);
                                $("#txtStartDate").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
                                $("#txtTargetCompletionDate").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
                                $("#txtEffortsEnteredUntil").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
                                $("#ActualReset, #PlannedReset").attr("disabled", "disabled");
                            }
                        }
                    }
                    $("#lblTSO").attr("disabled", "disabled");
                    callInputOutcomeFill();
                }
                else {
                    $("#lblTask").prop('disabled', 'disabled');
                    PopulateTask(result);
                }
            }
            else {
                $("#ddlTaskStatus option:contains('Created')").attr('selected', 'selected');
                if (roleId.toLowerCase() != "admin") {
                    $("#lblTSO").attr("disabled", "disabled");
                    $("#ddlTaskStatus").attr("disabled", "disabled");
                }
            }
            currentweek = result.CurrentWeek;
            if (currentweek > result.WeekNumber && result.WeekNumber != 0) {               
                $("#txtEffortsEnteredUntil").val(currentweek);
                $("#hiddenEffortsEnteredUntil").val(currentweek);
                $("#foractualweek").text("fill in actual data for week " + currentweek);
                $("#forplannedweek").text("fill in planned data for week " + (parseInt(currentweek) + 1));
                $("#txtNotes").val(result.Notes);
                $("#lblTask").val(result.ServiceDeliveryChain.Name + " (" + result.ServiceDeliveryChain.Description + ")");
                $("#txtStartDate").val(dateFormat(result.PlannedStartDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                $("#txtTargetCompletionDate").val(dateFormat(result.PlannedCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
                if (roleId.toLowerCase() != "admin") {
                    $("#txtStartDate,#txtTargetCompletionDate").attr("disabled", "disabled");
                }
            }
            else if (currentweek == result.WeekNumber && result.ID > 0) {
                if (roleId.toLowerCase() != "admin") {
                    $("#txtStartDate").prop("disabled", true);
                    $("#txtTargetCompletionDate").prop("disabled", true);
                }
            }
            else if (currentweek == result.WeekNumber) {
                $("#txtEffortsEnteredUntil").val(currentweek);
                $("#hiddenEffortsEnteredUntil").val(currentweek);
                $("#foractualweek").text("fill in actual data for week " + currentweek);
                $("#forplannedweek").text("fill in planned data for week " + (parseInt(currentweek) + 1));
                if (roleId.toLowerCase() != "admin") {
                    $("#txtStartDate").prop("disabled", false);
                    $("#txtTargetCompletionDate").prop("disabled", false);
                }
            }
            else if (result.WeekNumber == 0) {
                $("#txtEffortsEnteredUntil").val(currentweek);
                $("#hiddenEffortsEnteredUntil").val(currentweek);
                $("#foractualweek").text("fill in actual data for week " + currentweek);
                $("#forplannedweek").text("fill in planned data for week " + (parseInt(currentweek) + 1));
            }
            var future = dateFormat(result.PlannedStartDate.replace("T00:00:00", ""), "isoDateddMonyyyy");
            var CurrentDate = dateFormat(result.CurrentDate.replace("T00:00:00", ""), "isoDateddMonyyyy");
            var futuredate = future;

            future = future.replace(/-/g, " ");
            var futureWeek = jQuery.datepicker.iso8601Week(new Date(result.PlannedStartDate));
            if (roleId.toLowerCase() != "admin") {
                if (futureWeek > currentweek && Date.parse(futuredate) > Date.parse(CurrentDate)) {
                    $("#txtDefectsRaised").attr("disabled", "disabled");
                    $("#txtDefectsRejected").attr("disabled", "disabled");
                    $("#txtActualEffort").attr("disabled", "disabled");
                    $("input[id^='txtActualOutcome_'").attr("disabled", "disabled");
                    $("#txtActualOutcomeTestSteps").attr("disabled", "disabled");
                    $("#txtActualProcessingTime").attr("disabled", "disabled");
                    $("input[id^='txtActualInput_'").attr("disabled", "disabled");
                    $("#txtActualServiceQuality").val(0);
                    $("#txtActualServiceQuality").attr("disabled", "disabled");
                }
            }
            $("#lastUpdatedOn").val(dateFormat(result.UpdatedOn, "dd-mmm-yyyy HH:MM:ss TT"));
            $("#lastUpdatedBy").val(result.UpdatedBy);
            hideLoader();
        },
        error: function (jqXHR, textStatus, errorThrown) {           
            if (errorThrown == "Unauthorized") {
                textStatus = "You are not authorized to do the current operation."
            }
            showMessageBox(textStatus, "red");
        }
    });
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

function LoadTSO(tsoId) {
    var url = urlPrefix + "TSO/GetById/" + tsoId;

    $.ajax({
        type: "GET",
        beforeSend: function (request) {
            request.setRequestHeader('userid', userId);
        },
        url: url,
        dataType: "json",
        success: function (result) {
            if (null != result) {
                PopulateTSO(result);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
            if (errorThrown == "Unauthorized") {
                textStatus = "You are not authorized to do the current operation."
            }
            showMessageBox(textStatus, "red");
        }
    });
}

//function LoadOperationalRiskIndicator() {
//    var url = urlPrefix + "OperatonalRiskIndicator/GetAllIDName";
//    LoadData(url, 'IndicatorPresent');
//    LoadData(url, 'IndicatorFuture');
//}

//function LoadOperationalRisk() {
//    var url = urlPrefix + "OperationalRisk/GetAllIDName";
//    LoadData(url, 'OpRiskPresent');
//    LoadData(url, 'OpRiskFuture');
//}

function LoadTaskStatus() {
    var url = urlPrefix + "TaskStatus/GetAllIDName";
    LoadData(url, 'ddlTaskStatus');
}
function LoadData(url, dropdown) {
    $.ajax({
        type: "GET",
        beforeSend: function (request) {
            request.setRequestHeader('userid', userId);
        },
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
            // showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
            if (errorThrown == "Unauthorized") {
                textStatus = "You are not authorized to do the current operation."
            }
            showMessageBox(textStatus, "red");
        }
    });
}

function PopulateDropdown(result, dropdown) {
    $.each(result, function (i, item) {
        $('#' + dropdown).append($('<option>', {
            value: item.ID,
            text: item.Name
        }));
    });
}
var objTSO = null;
function PopulateTSO(result) {
    for (var i = 0; i < result.TSOProductivityInputs.length; i++) {
        if (result.TSOProductivityInputs[i].ProductivityInput != null) {
            addInputOutcomeRow(result.TSOProductivityInputs[i].ProductivityInput, result.TSOProductivityInputs[i].ID, 'txtActualInput_Row_0', 'ActualInput');
            addInputOutcomeRow(result.TSOProductivityInputs[i].ProductivityInput, result.TSOProductivityInputs[i].ID, 'txtPlannedInput_Row_0', 'PlannedInput');
        }
    }

    for (var i = 0; i < result.TSOProductivityOutcomes.length; i++) {
        if (result.TSOProductivityOutcomes[i].ProductivityOutcome != null) {
            addInputOutcomeRow(result.TSOProductivityOutcomes[i].ProductivityOutcome, result.TSOProductivityOutcomes[i].ID, 'txtActualOutcome_Row_0', 'ActualOutcome');
            addInputOutcomeRow(result.TSOProductivityOutcomes[i].ProductivityOutcome, result.TSOProductivityOutcomes[i].ID, 'txtPlannedOutcome_Row_0', 'PlannedOutcome');
        }
    }


    objTSO = result;
    addToBreadcrumbArray(result.TSR.Title, result.TSR.ID, "TSR", true);
    addToBreadcrumbArray(result.Title, result.ID, "TSO", true);
    //addToBreadcrumbArray(taskName, "", "task", false);
    CreateBreadcrumb();

    //$("#lblTSO").html(result.Title);
    $("#lblTSO").val(result.Title);
    $("#lblTSO").prop("title", "Description: " + result.Description +
        "\nTSR: " + result.TSR.Title +
        "\nTeam Lead: " + result.TeamLead.Name +
        "\nStartDate: " + dateFormat(result.StartDate.replace("T00:00:00", ""), "isoDateddMonyyyy") +
        "\nTargetCompletionDate: " + dateFormat(result.TargetCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy") +
        "\nEstimated Effort: " + result.EstimatedEffort + " days" +
        "\nPlanned Effort: " + result.PlannedEffort + " days"
    );
    $("#lblProjectModel").html(result.TSR.ProjectModelName);
    $("#hiddennEstimatedEffort").val(result.EstimatedEffort);
    $("#hiddentsoPlannedEffort").val(result.PlannedEffort);

    startDate = $.datepicker.parseDate('yy-m-dd', result.StartDate.replace("T00:00:00", ""));

    $("#TSOStartDate").val(dateFormat(startDate, "isoDateddMonyyyy"));

    startDate = $.datepicker.parseDate('yy-m-dd', result.TargetCompletionDate.replace("T00:00:00", ""));

    $("#TSOTargetCompletionDate").val(dateFormat(startDate, "isoDateddMonyyyy"));

    callInputOutcomeFill();
}

function fillInput(inputs, id, type) {
    for (var g = 0; g < inputs.length; g++) {
        var week = inputs[g].WeekNumber;
        if (oldTask != null) {
            week = oldTask.CurrentWeek;
        }
        if (type == "PlannedInput") {
            week = week + 1;
        }
        if (week == inputs[g].WeekNumber) {
            //$("#txt" + type + "_" + id).val(inputs[g].InputValue);
        }
    }
}

function fillOutcome(inputs, id, type) {
    for (var g = 0; g < inputs.length; g++) {
        var week = inputs[g].WeekNumber;
        if (oldTask != null) {
            week = oldTask.CurrentWeek;
        }
        if (type == "PlannedOutcome") {
            week = week + 1;
        }
        if (week == inputs[g].WeekNumber) {
            //$("#txt" + type + "_" + id).val(inputs[g].OutcomeValue);
        }
    }
}


function addInputOutcomeRow(Productivity, id, rowID, type) {
    showLoader();
    var name = Productivity.Name;

    var newrowID = rowID.substr(0, rowID.lastIndexOf("_"));
    var presenttrRow = $('[id^=' + newrowID + ']');
    var newIdSting = rowID.substr(0, rowID.indexOf("_"));
    var cumulativeValue = 0;

    var inputVar = '<input id="' + newIdSting + '_' + Productivity.ID + '" data-tableId="' + type + "_" + id + '" onkeyup="intNumber(event, this); ActualProductivityIB();" type="text" size="18" maxlength="100" />';
    if (type == 'PlannedInput') {
        inputVar = '<input id="' + newIdSting + '_' + Productivity.ID + '" data-tableId="' + type + "_" + id + '"  onkeyup="intNumber(event, this); PlannedProductivityIB();" type="text" size="18" maxlength="100" />';
    }
    else if (type == 'ActualOutcome') {
        inputVar = '<input id="' + newIdSting + '_' + Productivity.ID + '" data-tableId="' + type + "_" + id + '"  onkeyup="intNumber(event, this); ActualProductivity(); ActualProductivityTB(); ActualProductivityIB(); defectDensity();" type="text" size="18" maxlength="100" />';
    }
    else if (type == 'PlannedOutcome') {
        inputVar = '<input id="' + newIdSting + '_' + Productivity.ID + '" data-tableId="' + type + "_" + id + '"  onkeyup="intNumber(event, this); PlannedProductivity(); PlannedProductivityIB(); PlannedProductivityTB();" type="text" size="18" maxlength="100" />';
    }

    var newTr = $('<tr id="' + newIdSting + '_Row_' + Productivity.ID + '"><td class="titletd width35imp required inOut">' + name + '</td><td class="field width15imp"id="lableText' + type + '_cumulative_' + Productivity.ID + '" style="text-align:center;color:#696969;">' + cumulativeValue + '</td>' + // 
                 '<td class="seprator">:</td><td class="field width29imp">' + inputVar + '<div class="waterInfo"># of items</div></td></tr>');

    $('#' + rowID).after(newTr);

    hideLoader();
}

function getAllInputOutcomeData() {
    var ActualIntrRow = $('[id*=_Row_]');
    var valArry = [];
    var Year = new Date(jQuery.datepicker.parseDate("dd-M-yy", $("#txtStartDate").val()));
    Year = Year.getFullYear();
    for (var i = 0; i < ActualIntrRow.length; i++) {
        var inputVal = $(ActualIntrRow[i]).find("input");
        if (inputVal.length > 0) {
            var tableId = $(inputVal[0]).data("tableid");
            var id = tableId.substr(tableId.indexOf("_") + 1);
            tableId = tableId.substr(0, tableId.indexOf("_"));
            valArry.push({ "Type": tableId, "ID": id, "Value": $(inputVal[0]).val(), "Week": $("#txtEffortsEnteredUntil").val(), "Year": Year });
        }
    }

    return valArry;
}

function fillInputOutcome() {
    if (oldTask != null) {
        if (oldTask.InputOutcomeCumulative.length > 0) {
            for (var i = 0; i < oldTask.InputOutcomeCumulative.length; i++) {
                var lable = $("#lableText" + oldTask.InputOutcomeCumulative[i].Type + '_cumulative_' + oldTask.InputOutcomeCumulative[i].ID);
                if (lable.length > 0) {
                    $("#lableText" + oldTask.InputOutcomeCumulative[i].Type + '_cumulative_' + oldTask.InputOutcomeCumulative[i].ID).text(oldTask.InputOutcomeCumulative[i].Value);
                }
            }
        }

        if (oldTask.InputOutcome.length > 0) {
            for (var i = 0; i < oldTask.InputOutcome.length; i++) {
                var text = $('*[data-tableId="' + oldTask.InputOutcome[i].Type + '_' + oldTask.InputOutcome[i].TSOPID + '"]');
                if (text.length > 0) {
                    $('*[data-tableId="' + oldTask.InputOutcome[i].Type + '_' + oldTask.InputOutcome[i].TSOPID + '"]').val(oldTask.InputOutcome[i].Value);
                    //$("#txt" + oldTask.InputOutcome[i].Type + '_' + oldTask.InputOutcome[i].TSOID).val(oldTask.InputOutcome[i].Value);
                }
            }
        }
    }
}

function PopulateTask(result) {
    addToBreadcrumbArray(result.Title, result.ID, "TASK", true);
    CreateBreadcrumb();
    $("#ddlTaskStatus").val(result.TaskStatusId);
    $("#txtOid").val(result.ID);
    $("#txtTaskActualOid").val(result.TaskActualID);
    $("#txtTaskPlannedOid").val(result.TaskPlannedId);
    $("#txtTaskServiceChainIdOid").val(result.ServiceDeliveryChainId);
    $("#txtTSOOid").val(result.TSOId);

    $("#ddlMethod").val(result.ServiceDeliveryChainMethodId);
    $("#txtStartDate").val(dateFormat(result.PlannedStartDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
    $("#txtTargetCompletionDate").val(dateFormat(result.PlannedCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy"));
    $("#lastUpdatedOn").val(dateFormat(result.UpdatedOn, "dd-mmm-yyyy HH:MM:ss TT"));
    $("#lastUpdatedBy").val(result.UpdatedBy);
    $("#lblDefectsRaised").text(result.DefectRaisedCumulative);
    $("#lblDefectsRejected").text(result.DefectRejectedCumulative);
    $("#txtoperationalRisk").val(result.OperationalRisk);
    $("#txtNotes").val(result.Notes);
    if (result != null) {
        $("#lblTask").val(result.ServiceDeliveryChain.Name + " (" + result.ServiceDeliveryChain.Description + ")");
    }

    if (null != result.IdleTimeDuration) {
        $("#txtIdleTimeDu").val(result.IdleTimeDuration);
    }

    if (null != result.ActualOutcome) {
        $("#txtIdleTimeEf").val(result.IdleTimeEffort);
    }

    if (null != result.PlannedReviewRounds) {
        $("#txtPlannedServiceQuality").val(result.PlannedReviewRounds);
    }

    if (null != result.ActualReviewRounds) {
        $("#txtActualServiceQuality").val(result.ActualReviewRounds);
    }
    
    if (null != result.DefectRaised) {
        $("#txtDefectsRaised").val(result.DefectRaised);
    }

    if (null != result.DefectRejected) {
        $("#txtDefectsRejected").val(result.DefectRejected);
    }

    
    //if (result.ServiceDeliveryChainId == 42 || result.ServiceDeliveryChainId == 46) {
    //    $('#txtResponsiblePerson').val(result.ResponsiblePerson.Name);
    //    $("#lblResponsiblePersonMail").val(result.ResponsiblePerson.EmailID)
    //    $("#lblResponsiblePersonId").val(result.ResponsiblePersonId);
    //}

    var currentweek = result.CurrentWeek;
    if (roleId.toLowerCase() != "admin") {
        if (result.WeekNumber == currentweek || result.WeekNumber == currentweek + 1 || result.WeekNumber == currentweek - 1) {
            fillPlannedActual(result);
            currentweek = result.WeekNumber;
        }
        else {
            showMessageBox("No record found for selected week !", "red");
            $("#txtActualEffort").val("");
            $("#txtActualOutcomeTestSteps").val("");
            $("#txtActualProcessingTime").val("");
            $("#txtPlannedEffort").val("");
            $("#txtPlannedOutcomeTestSteps").val("");
            $("#txtPlannedProcessingTime").val("");
            ActualReset();
            PlannedReset();
        }
    }
    else {
        if (result.WeekNumber > 0) {
            currentweek = result.WeekNumber;
        }
    }
    fillPlannedActual(result);
    $("#txtEffortsEnteredUntil").val(currentweek);

    $("#txtHeadCount").val(result.Headcount);

    $("#hiddenEffortsEnteredUntil").val(currentweek);
    $("#foractualweek").text("fill in actual data for week " + currentweek);
    $("#forplannedweek").text("fill in planned data for week " + (parseInt(currentweek) + 1));

    if (roleId.toLowerCase() != "admin") {
        $("#lblDefectRejectionRatio").attr("disabled", "disabled");
        $("#lblTSO").attr("disabled", "disabled");
        $("#lblTask").prop('disabled', 'disabled');
        $("#txtStartDate,#txtTargetCompletionDate").attr("disabled", "disabled");

        if ($("#ddlTaskStatus option:selected").text() == "Cancelled" || $("#ddlTaskStatus option:selected").text() == "Closed") {
            $("#TaskInfo, #SubmitTask").attr("disabled", "disabled");
            $("#ddlTaskStatus").attr("disabled", "disabled");
            $("#ddlMethod").attr("disabled", "disabled");
            $("#txtNotes").prop("disabled", true);
            $("#txtHeadCount").prop("disabled", true);
            $("#txtIdleTimeDu").prop("disabled", true);
            $("#txtActualEffort").prop("disabled", true);

            $("#txtActualOutcomeTestSteps").prop("disabled", true);
            $("#txtActualProcessingTime").prop("disabled", true);
            $("#txtActualProductivityEB").prop("disabled", true);
            $("#txtActualProductivityTB").prop("disabled", true);
            $("#txtPlannedProcessingTime").prop("disabled", true);
            $("#txtPlannedOutcomeTestSteps").prop("disabled", true);

            $("#txtPlannedEffort").prop("disabled", true);
            $("#txtPlannedServiceQuality").prop("disabled", true);
            $("#txtDefectsRaised").prop("disabled", true);
            $("#txtDefectDensity").prop("disabled", true);
            $("#txtDefectsRejected").prop("disabled", true);
            $("#txtActualServiceQuality").prop("disabled", true);
            $("#txtStartDate").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
            $("#txtTargetCompletionDate").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
            $("#txtEffortsEnteredUntil").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');
            $("#ActualReset, #PlannedReset").attr("disabled", "disabled");

            disablesActualPlanned();
        }

        $("#ddlMethod").attr("disabled", "disabled");
        $("#txtPlannedServiceQuality").prop("disabled", false);
        $("#txtDefectsRaised").prop("disabled", false);
        $("#txtDefectsRejected").prop("disabled", false);
        $("#txtActualServiceQuality").prop("disabled", false);
    }
    callInputOutcomeFill();
}

function callInputOutcomeFill() {
    $("input[id^='txtActualInput_']").val("0");
    $("input[id^='txtActualOutcome_']").val("0");
    $("input[id^='txtPlannedInput_']").val("0");
    $("input[id^='txtPlannedOutcome_']").val("0");
    $("td[id^='lableText']").text(0);
    if (oldTask != null) {
        fillInputOutcome();
    }
    calculation();
}

function checkWithOld() {
    var isChange = false;

    var txtNotes = $("#txtNotes").val();
    var txtStartDate = $("#txtStartDate").val();
    var txtTargetCompletionDate = $("#txtTargetCompletionDate").val();
    var txtPlannedEffort = $("#txtPlannedEffort").val();

    if (txtPlannedEffort != oldTask.PlannedEffort || txtNotes != oldTask.Notes
        || txtStartDate != dateFormat(oldTask.PlannedStartDate.replace("T00:00:00", ""), "isoDateddMonyyyy") ||
        txtTargetCompletionDate != dateFormat(oldTask.PlannedCompletionDate.replace("T00:00:00", ""), "isoDateddMonyyyy")) {
        isChange = true;
    }


    var txtActualEffort = $("#txtActualEffort").val();
    var txtPlannedOutcome = $("#txtPlannedOutcome_0").val() == null ? 0 : $("#txtPlannedOutcome_0").val();
    var txtPlannedOutcomeTestSteps = $("#txtPlannedOutcomeTestSteps").val();
    var txtActualOutcome = $("#txtActualOutcome_0").val() == null ? 0 : $("#txtActualOutcome_0").val();

    if (txtActualEffort != oldTask.ActualEffort || txtPlannedOutcome != oldTask.PlannedOutcome
        || txtPlannedOutcomeTestSteps != oldTask.PlannedOutcomeTestSteps ||
        txtActualOutcome != oldTask.ActualOutcome) {
        isChange = true;
    }

    var txtActualOutcomeTestSteps = $("#txtActualOutcomeTestSteps").val();
    var txtPlannedServiceQuality = $("#txtPlannedServiceQuality").val() == null ? 0 : $("#txtPlannedServiceQuality").val();
    var txtActualServiceQuality = $("#txtActualServiceQuality").val() == null ? 0 : $("#txtActualServiceQuality").val();
    var txtDefectsRaised = $("#txtDefectsRaised").val();

    if (txtActualOutcomeTestSteps != oldTask.ActualOutcomeTestSteps || txtPlannedServiceQuality != oldTask.PlannedReviewRounds
        || txtActualServiceQuality != oldTask.ActualReviewRounds || txtDefectsRaised != oldTask.DefectRaised) {
        isChange = true;
    }

    var txtDefectsRejected = $("#txtDefectsRejected").val();
    var txtPlannedProcessingTime = $("#txtPlannedProcessingTime").val();
    var txtActualProcessingTime = $("#txtActualProcessingTime").val();
    var txtPlannedInput = $("#txtPlannedInput_0").val();

    if (txtDefectsRejected != oldTask.DefectRejected || txtPlannedProcessingTime != oldTask.PlannedProcessingTime
        || txtActualProcessingTime != oldTask.ActualProcessingTime || txtPlannedInput != oldTask.PlannedInput) {
        isChange = true;
    }


    var txtActualInput = $("#txtActualInput_0").val();
    var ddlTaskStatus = $("#ddlTaskStatus").val();
    var txtIdleTimeDu = $("#txtIdleTimeDu").val();

    if (txtActualInput != oldTask.ActualInput || ddlTaskStatus != oldTask.TaskStatusId
        || txtIdleTimeDu != oldTask.IdleTimeDuration) {
        isChange = true;
    }

    checkCloseOnSubmit('ddlTaskStatus', oldTask.TaskStatusId);

    var txtHeadCount = $("#txtHeadCount").val();
    var ddlMethod = $("#ddlMethod").val();
    if (parseFloat(txtHeadCount).toFixed(2) != parseFloat(oldTask.Headcount).toFixed(2) || ddlMethod != oldTask.ServiceDeliveryChainMethodId) {
        isChange = true;
    }

    return isChange;
}

function fillPlannedActual(result) {
    oldTask = result;

    if (null != result.ActualOutcomeTestSteps) {
        $("#txtActualOutcomeTestSteps").val(result.ActualOutcomeTestSteps);
    }

    if (null != result.PlannedOutcomeTestSteps) {
        $("#txtPlannedOutcomeTestSteps").val(result.PlannedOutcomeTestSteps);
    }

    if (null != result.ActualEffort ) {
        $("#txtActualEffort").val(result.ActualEffort);
    }

    
    if (null != result.ActualProcessingTime ) {
        $("#txtActualProcessingTime").val(result.ActualProcessingTime);
    }
    
    $("#txtActualInput_0").val(result.ActualInput);

    if (null != result.PlannedEffort) {
        $("#txtPlannedEffort").val(result.PlannedEffort);
    }
    
    if (null != result.PlannedProcessingTime ) {
        $("#txtPlannedProcessingTime").val(result.PlannedProcessingTime);
    }
    
    $("#txtPlannedInput_0").val(result.PlannedInput);

    $("#lbleffort").text(result.ActualEffortCumulative);

    $("#lblActualOutcome").text(result.ActualOutcomeCumulative);
    $("#lblActualOutcomeTestSteps").text(result.ActualOutcomeTestStepsCumulative);
    $("#lblActualProcessingTime").text(result.ActualProcessingTimeCumulative);
    $("#lblActualInputCumulative").text(result.ActualInputCumulative);

    $("#lblPlannedeffort").text(result.PlannedEffortCumulative);
    $("#lblPlannedActualOutcome").text(result.PlannedOutcomeCumulative);
    $("#lblPlannedActualOutcomeTestSteps").text(result.PlannedOutcomeTestStepsCumulative);
    $("#lblPlannedActualProcessingTime").text(result.PlannedProcessingTimeCumulative);
    $("#lblPlannedInputCumulative").text(result.PlannedInputCumulative);


    PlannedProductivity();
    PlannedProductivityTB();
    PlannedProductivityIB();
    ActualProductivity();
    ActualProductivityTB();
    ActualProductivityIB();
}


function togglePlannedActualselection(bool) {
    if (roleId.toLowerCase() != "admin") {
        $("#txtActualEffort").prop("disabled", bool);
        $("#txtActualOutcomeTestSteps").prop("disabled", bool);
        $("#txtActualProcessingTime").prop("disabled", bool);

        $("#txtPlannedEffort").prop("disabled", bool);
        $("#txtPlannedOutcomeTestSteps").prop("disabled", bool);
        $("#txtPlannedProcessingTime").prop("disabled", bool);

        $("#ActualReset").prop("disabled", bool);
        $("#PlannedReset").prop("disabled", bool);

        $("input[id^='txtActualInput_']").prop("disabled", bool);
        $("input[id^='txtActualOutcome_']").prop("disabled", bool);
        $("input[id^='txtPlannedInput_']").prop("disabled", bool);
        $("input[id^='txtPlannedOutcome_']").prop("disabled", bool);

    }
}

function toggleActualselection(bool) {
    if (roleId.toLowerCase() != "admin") {
        $("#txtActualEffort").prop("disabled", bool);
        $("#txtActualOutcomeTestSteps").prop("disabled", bool);
        $("#txtActualProcessingTime").prop("disabled", bool);
        $("input[id^='txtActualInput_']").prop("disabled", bool);
        $("input[id^='txtActualOutcome_']").prop("disabled", bool);

        $("#ActualReset").prop("disabled", bool);
    }
}

function calculation() {
    PlannedProductivity();
    ActualProductivity();
    PlannedProductivityTB();
    ActualProductivityTB();
    PlannedProductivityIB();
    ActualProductivityIB();
    defectDensity();
    Headcount();
    IdleEffort();

}

function Save() {
    showLoader();
    if(CheckTaskData()) {
        var txtTSOServiceDeliveryChainId = $("#txtTSOServiceDeliveryChainId").val();
        var txtNotes = $("#txtNotes").val();
        var txtStartDate = $("#txtStartDate").val();
        var txtTargetCompletionDate = $("#txtTargetCompletionDate").val();
        var txtPlannedEffort = $("#txtPlannedEffort").val();

        var txtActualEffort = $("#txtActualEffort").val();

        var txtEffortsEnteredUntil = $("#txtEffortsEnteredUntil").val();
        var txtPlannedProductivity = $("#txtPlannedProductivityTB").val();

        var txtActualProductivity = $("#txtActualProductivityTB").val();
        var txtPlannedOutcomeTestSteps = $("#txtPlannedOutcomeTestSteps").val();
        var txtActualOutcomeTestSteps = $("#txtActualOutcomeTestSteps").val();
        var txtPlannedServiceQuality = $("#txtPlannedServiceQuality").val() == null ? 0 : $("#txtPlannedServiceQuality").val();
        var txtActualServiceQuality = $("#txtActualServiceQuality").val() == null ? 0 : $("#txtActualServiceQuality").val();
        var txtDefectsRaised = $("#txtDefectsRaised").val();
        var txtDefectsRejected = $("#txtDefectsRejected").val();
        var txtPlannedProductivityIB = $("#txtPlannedProductivityIB").val();
        var txtActualProductivityIB = $("#txtActualProductivityIB").val();
        var txtPlannedProcessingTime = $("#txtPlannedProcessingTime").val();
        var txtActualProcessingTime = $("#txtActualProcessingTime").val();
        var txtPlannedInput = $("#txtPlannedInput_0").val();
        var txtActualInput = $("#txtActualInput_0").val();
        var ddlTaskStatus = $("#ddlTaskStatus").val();
        var txtIdleTimeDu = $("#txtIdleTimeDu").val();
        var txtIdleTimeEf = $("#txtIdleTimeEf").val();
        var txtHeadCount = $("#txtHeadCount").val();
        var ddlMethod = $("#ddlMethod").val();


        var objInputOutcome = getAllInputOutcomeData();
        var txtPlannedOutcome = $("#txtPlannedOutcome_0").val() == null ? 0 : $("#txtPlannedOutcome_0").val();
        var txtActualOutcome = $("#txtActualOutcome_0").val() == null ? 0 : $("#txtActualOutcome_0").val();

        //var txtResponsiblePerson;
        if (oldTask != null && oldTask != "") {
            if (!checkWithOld()) {
                hideLoader();
                showMessageBox("No updation seen !", "red");
                return false;
            }

            //if (oldTask.ServiceDeliveryChainId == 42 || oldTask.ServiceDeliveryChainId == 46) {
            //    txtResponsiblePerson = $("#lblResponsiblePersonId").val();
            //    var ResponsiblePersonId = $("#lblResponsiblePersonId").val();
            //    var DueDate = $("#txtDueDate").val();
            //    var ActualOperationalRiskId = $("#OpRiskPresent").val();
            //    var ActualOperationalRiskIndicatorId = $("#IndicatorPresent").val();
            //    var ActualOperationalRiskDescription = $("#riskDescriptionPresent").val();
            //    var ActualOperationalRiskMitigation = $("#riskMitiDescriptionPresent").val();
            //    var PlannedOperationalRiskId = $("#IndicatorPresent").val();
            //    var PlannedOperationalRiskIndicatorId = $("#IndicatorFuture").val();
            //    var PlannedOperationalRiskDescription = $("#riskDescriptionFuture").val();
            //    var PlannedOperationalRiskMitigation = $("#riskMitiDescriptionFuture").val();
            //}
        }       


        var txtOid = $("#txtOid").val();
        var txtTaskActualOid = $("#txtTaskActualOid").val();
        var txtTaskPlannedOid = $("#txtTaskPlannedOid").val();
        var txtServiceChainId = $("#txtTaskServiceChainIdOid").val();
        var txtTSOOid = getLocalStorage("tsoid");
        var data = JSON.stringify({
            TSOServiceDeliveryChainId: txtTSOServiceDeliveryChainId,
            Notes: txtNotes,
            PlannedStartDate: txtStartDate,
            PlannedCompletionDate: txtTargetCompletionDate,
            PlannedEffort: txtPlannedEffort,
            ActualEffort: txtActualEffort,
            WeekNumber: txtEffortsEnteredUntil,
            PlannedProductivity: txtPlannedProductivity,
            TaskStatusId: ddlTaskStatus,
            ActualProductivity: txtActualProductivity,
            PlannedOutcome: txtPlannedOutcome,
            PlannedOutcomeTestSteps: txtPlannedOutcomeTestSteps,
            ActualOutcome: txtActualOutcome,
            ActualOutcomeTestSteps: txtActualOutcomeTestSteps,
            PlannedReviewRounds: txtPlannedServiceQuality,
            ActualReviewRounds: txtActualServiceQuality,
            CreatedBy: userName,
            DefectRaised: txtDefectsRaised,
            DefectRejected: txtDefectsRejected,
            PlannedProductivityIB: txtPlannedProductivityIB,
            ActualProductivityIB: txtActualProductivityIB,
            PlannedProcessingTime: txtPlannedProcessingTime,
            ActualProcessingTime: txtActualProcessingTime,
            PlannedInput: txtPlannedInput,
            ActualInput: txtActualInput,
            IdleTimeEffort: txtIdleTimeEf,
            IdleTimeDuration: txtIdleTimeDu,
            Headcount: txtHeadCount,
            ServiceDeliveryChainMethodId: ddlMethod,
            ID: txtTSOServiceDeliveryChainId,
            InputOutcome: objInputOutcome,
            TaskActualID: txtTaskActualOid,
            TaskPlannedId: txtTaskPlannedOid,
            ServiceDeliveryChainId: txtServiceChainId,
            TSOId: txtTSOOid,
        //    ResponsiblePersonId : ResponsiblePersonId,
        //  DueDate : DueDate,
        //ActualOperationalRiskId : ActualOperationalRiskId,
        //ActualOperationalRiskIndicatorId : ActualOperationalRiskIndicatorId,
        //ActualOperationalRiskDescription : ActualOperationalRiskDescription,
        //ActualOperationalRiskMitigation :ActualOperationalRiskMitigation,
        //PlannedOperationalRiskId : PlannedOperationalRiskId,
        //PlannedOperationalRiskIndicatorId :PlannedOperationalRiskIndicatorId,
        //PlannedOperationalRiskDescription : PlannedOperationalRiskDescription,
        //PlannedOperationalRiskMitigation : PlannedOperationalRiskMitigation
        });
        var urlAddUpdate = urlPrefix + "TSO";
        var method = "POST";
        var message = "";
        urlAddUpdate = urlAddUpdate + "/CreateTask";
        message = "Task saved successfully.";
        
        $.ajax({
            url: urlAddUpdate,
            data: data,
            type: method,
            beforeSend: function (request) {
                request.setRequestHeader('userid', userId);
            },
            contentType: 'application/json; charset=UTF-8',
            success: function (response) {
                hideLoader();
                if (response.indexOf("Error") == 0) {
                    showMessageBox(response, "red");
                }
                else {
                    localStorage.setItem("tsrid", tsrId);
                    showMessageBox(response, "green", "TSODashboard.html");
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (errorThrown == "Unauthorized") {
                    textStatus = "You are not authorized to do the current operation."
                }
                showMessageBox(textStatus, "red");
            }
        });
    };
}



function Cancel() {
    window.location = "TASKDashboard.html";
}

function callSuccess(message) {
    localStorage.setItem("tsrid", tsrId);
    window.location = "TSODashboard.html";
}

function CheckTaskData() {
    if ($("#txtNotes").val() == "") {
        showMessageBox("Please enter description.", "red", "", false, "txtNotes");
        return false;
    }

    if ($("#txtStartDate").val() != "") {
        try {
            jQuery.datepicker.parseDate("dd-M-yy", $("#txtStartDate").val());
        }
        catch (e) {
            showMessageBox("Please select valid planned start date.", "red", "", false, "txtStartDate");
            return false;
        }
    }

    var myDate = new Date(jQuery.datepicker.parseDate("dd-M-yy", $("#txtStartDate").val()));
    var selectedstartweek = myDate.getWeek();

    var manualWeekNumber = $("#txtEffortsEnteredUntil").val();

    if (oldTask == null) {
        var today = new Date();
        var myDate = myDate.toLocaleDateString();
        var today = today.toLocaleDateString();
        if (selectedstartweek < currentweek && manualWeekNumber < currentweek) {
            showMessageBox("You are not allowed to select older week’s date. Kindly select current week’s or last week date.", "red", "", false, "txtStartDate");
            return false;
        }

        if (selectedstartweek > currentweek + 1 && Date.parse(myDate) < Date.parse(today)) {
            showMessageBox("You are not allowed to select a date from immediate future week only", "red", "", false, "txtStartDate");
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
    if ($("#txtStartDate").val() == "") {
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
    else if ($.datepicker.parseDate('dd-M-yy', $("#TSOStartDate").val()) > $.datepicker.parseDate('dd-M-yy', $("#txtStartDate").val())) {
        showMessageBox("Planned start date can not be less than TSO start date.", "red", "", false, "txtStartDate");
        return false;
    }
    else if ($("#txtPlannedEffort").val() == "") {
        showMessageBox("Please enter planned efforts.", "red", "", false, "txtPlannedEffort");
        return false;
    }
    else if (parseInt($("#hiddentsoPlannedEffort").val()) < parseInt($("#txtPlannedEffort").val())) {
        showMessageBox("Planned efforts can not be more than TSO planned efforts.", "red", "", false, "txtPlannedEffort");
        return false;
    }
    else if ($("#txtActualEffort").val() == "") {
        showMessageBox("Please enter actual efforts.", "red", "", false, "txtActualEffort");
        return false;
    }
    else if ($("#txtPlannedEffort").val() == "") {
        showMessageBox("Please enter planned efforts.", "red", "", false, "txtPlannedEffort");
        return false;
    }
    else if (checkInputOutcome()) {
        showMessageBox("Please enter input or outcome types.", "red", "", false, "");
        return false;
    }
    else if ($("#txtEffortsEnteredUntil").val() == "" || $("#txtEffortsEnteredUntil").val() == 0) {
        showMessageBox("Please select week.", "red", "", false, "txtEffortsEnteredUntil");
        return false;
    }
    else if ($("#txtPlannedProductivityIB").val() == "") {
        showMessageBox("Please enter planned productivity.", "red", "", false, "txtPlannedProductivityIB");
        return false;
    }
    else if ($("#txtPlannedProductivityTB").val() == "") {
        showMessageBox("Please enter planned productivity.", "red", "", false, "txtPlannedProductivityTB");
        return false;
    }
    else if ($("#txtActualProductivityIB").val() == "") {
        showMessageBox("Please enter actual productivity.", "red", "", false, "txtActualProductivityIB");
        return false;
    }
    else if ($("#txtActualProductivityTB").val() == "") {
        showMessageBox("Please enter actual productivity.", "red", "", false, "txtStartDatetxtActualProductivityTB");
        return false;
    }
    else if ($("#txtDefectsRaised").val() == "") {
        showMessageBox("Please enter defect raised.", "red", "", false, "txtDefectsRaised");
        return false;
    }
    else if ($("#txtDefectsRejected").val() == "") {
        showMessageBox("Please enter defect rejected.", "red", "", false, "txtDefectsRejected");
        return false;
    }
    else if (parseInt($("#txtDefectsRaised").val()) < parseInt($("#txtDefectsRejected").val())) {
        showMessageBox("Defect rejected should not be greater than defect raised.", "red", "", false, "txtDefectsRejected");
        return false;
    }
    else if ($("#txtPlannedProcessingTime").val() == "") {
        showMessageBox("Please enter planned processing time.", "red", "", false, "txtPlannedProcessingTime");
        return false;
    }
    else if ($("#txtActualProcessingTime").val() == "") {
        showMessageBox("Please enter actual processing time.", "red", "", false, "txtActualProcessingTime");
        return false;
    }
    else if ($("#txtActualServiceQuality").val() == "") {
        showMessageBox("Please enter actual review rounds.", "red", "", false, "txtActualServiceQuality");
        return false;
    }
    else if ($("#txtEffortsEnteredUntil").val() == 0) {
        showMessageBox("Week must be greater than 0 !", "red", "", false, "txtEffortsEnteredUntil");
        return false;
    }
    else if ($("#txtEffortsEnteredUntil").val() >= 53) {
        showMessageBox("Week should be less than or equal to 52 ", "red", "", false, "txtEffortsEnteredUntil");
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

function intNumber(e, field) {
    var val = field.value;
    var re = /^([0-9]|[0-9]+)$/g;
    var re1 = /^([0-9]|[0-9]+)/g;
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

function defectDensity() {
    var val = $("#txtDefectsRaised").val();
    var DefectDensity = (val || 0) / (SumOfActualOutcome() || 0);
    if (DefectDensity == "Infinity" || isNaN(DefectDensity)) {
        DefectDensity = 0;
    }
    
    $("#txtDefectDensity").val(DefectDensity.toFixed(2));
    defectRatio();
}

function defectRatio() {
    var val = $("#txtDefectsRejected").val();
    var DefectRejectionRatio = ((val || 0) / ($("#txtDefectsRaised").val() || 0)) * 100;
    if (DefectRejectionRatio == "Infinity" || isNaN(DefectRejectionRatio)) {
        DefectRejectionRatio = 0;
    }
    $("#lblDefectRejectionRatio").val(DefectRejectionRatio.toFixed(2));
}

function PlannedProductivity() {
    var PlannedProductivity = (SumOfPlannedOutcome() || 0) / ($("#txtPlannedEffort").val() || 0);
    if (PlannedProductivity == "Infinity" || isNaN(PlannedProductivity)) {
        PlannedProductivity = 0;
    }
    $("#txtPlannedProductivityEB").val(PlannedProductivity.toFixed(2));
}

function ActualProductivity() {
    var ActualProductivity = (SumOfActualOutcome() || 0) / ($("#txtActualEffort").val() || 0);
    if (ActualProductivity == "Infinity" || isNaN(ActualProductivity)) {
        ActualProductivity = 0;
    }
    $("#txtActualProductivityEB").val(ActualProductivity.toFixed(2));
}

function PlannedProductivityTB() {
    var PlannedProductivityTB = ((SumOfPlannedOutcome() || 0) / ($("#txtPlannedProcessingTime").val() || 0));
    if (PlannedProductivityTB == "Infinity" || isNaN(PlannedProductivityTB)) {
        PlannedProductivityTB = 0;
    }
    $("#txtPlannedProductivityTB").val(PlannedProductivityTB.toFixed(2));
}

function ActualProductivityTB() {
    var ActualProductivityTB = ((SumOfActualOutcome() || 0) / ($("#txtActualProcessingTime").val() || 0));
    if (ActualProductivityTB == "Infinity" || isNaN(ActualProductivityTB)) {
        ActualProductivityTB = 0;
    }
    $("#txtActualProductivityTB").val(ActualProductivityTB.toFixed(2));
}

function PlannedProductivityIB() {
    var PlannedProductivityIB = ((SumOfPlannedOutcome() || 0) / (SumOfPlannedInput() || 0));
    if (PlannedProductivityIB == "Infinity" || isNaN(PlannedProductivityIB)) {
        PlannedProductivityIB = 0;
    }
    $("#txtPlannedProductivityIB").val(PlannedProductivityIB.toFixed(2));
}

function ActualProductivityIB() {
    var ActualProductivityIB = ((SumOfActualOutcome() || 0) / (SumOfActualInput() || 0));
    if (ActualProductivityIB == "Infinity" || isNaN(ActualProductivityIB)) {
        ActualProductivityIB = 0;
    }
    $("#txtActualProductivityIB").val(ActualProductivityIB.toFixed(2));
}

function Headcount() {
    var Headcount = ($("#txtActualEffort").val() || 0) / ($("#txtActualProcessingTime").val() || 0);
    if (Headcount == "Infinity" || isNaN(Headcount)) {
        Headcount = 0;
    }
    $("#txtHeadCount").val(Headcount.toFixed(2));
    IdleEffort();
}

function IdleEffort() {
    var IdleEffort = ($("#txtIdleTimeDu").val() || 0) * ($("#txtHeadCount").val() || 0);
    if (IdleEffort == "Infinity" || isNaN(IdleEffort)) {
        IdleEffort = 0;
    }
    $("#txtIdleTimeEf").val(IdleEffort.toFixed(2));
}

function SumOfActualOutcome() {
    var sum = 0;
    var ff = $("input[id^='txtActualOutcome_']");
    $("input[id^='txtActualOutcome_']").each(function () {
        sum += Number($(this).val());
    });
    return sum;
}

function SumOfActualInput() {
    var sum = 0;
    $("input[id^='txtActualInput_']").each(function () {
        sum += Number($(this).val());
    });
    return sum;
}

function SumOfPlannedOutcome() {
    var sum = 0;
    $("input[id^='txtPlannedOutcome_']").each(function () {
        sum += Number($(this).val());
    });
    return sum;
}

function SumOfPlannedInput() {
    var sum = 0;
    $("input[id^='txtPlannedInput_']").each(function () {
        sum += Number($(this).val());
    });
    return sum;
}

function checkInputOutcome() {
    var isBlank = false;

    $("input[id^='txtActualInput_']").each(function () {
        if ($(this).val() == "") {
            isBlank = true;
        }
    });

    $("input[id^='txtPlannedInput_']").each(function () {
        if ($(this).val() == "") {
            isBlank = true;
        }
    });

    $("input[id^='txtActualOutcome_']").each(function () {
        if ($(this).val() == "") {
            isBlank = true;
        }
    });
    $("input[id^='txtPlannedOutcome_']").each(function () {
        if ($(this).val() == "") {
            isBlank = true;
        }
    });

    return isBlank;
}

function disablesActualPlanned() {
    $("input[id^='txtActualInput_']").prop("disabled", true);
    $("input[id^='txtActualOutcome_']").prop("disabled", true);
    $("input[id^='txtPlannedInput_']").prop("disabled", true);
    $("input[id^='txtPlannedOutcome_']").prop("disabled", true);
}

function ActualReset() {
    $("#txtActualEffort").val(0);
    $("#txtActualOutcomeTestSteps").val(0);
    $("#txtActualProcessingTime").val(0.00);
    $("input[id^='txtActualInput_']").val(0);
    $("input[id^='txtActualOutcome_']").val(0);

    ActualProductivity();
    ActualProductivityTB();
    ActualProductivityIB();
}

function PlannedReset() {
    $("#txtPlannedEffort").val(0);
    $("#txtPlannedOutcomeTestSteps").val(0);
    $("#txtPlannedProcessingTime").val(0.00);
    $("input[id^='txtPlannedInput_']").val(0);
    $("input[id^='txtPlannedOutcome_']").val(0);

    PlannedProductivity();
    PlannedProductivityTB();
    PlannedProductivityIB();
}

function removeInputRow(rowID) {
    showLoader();
    $('#' + rowID).remove();
    hideLoader();
}

function getActualInOutVal() {
    //debugger;
    var ActualIntrRow = $('[id^=txtActualInput_Row_]');
    var valArry = [];
    for (var i = 0; i < ActualIntrRow.length; i++) {
        var type = $(ActualIntrRow[i]).find("select");
        var inputVal = $(ActualIntrRow[i]).find("input");
        valArry.push({ "Type": $('#' + type[0].id + ' :selected').val(), "Value": $(inputVal[0]).val() });
    }
}
