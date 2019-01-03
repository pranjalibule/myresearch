﻿function LoadOperationalRiskIndicatorWith(contId) {
    var url = urlPrefix + "OperatonalRiskIndicator/GetAllIDName";
    LoadData(url, contId);
}

function LoadOperationalRiskWith(contId) {
    var url = urlPrefix + "OperationalRisk/GetAllIDNameForRisk";
    LoadData(url, contId);
}

function LoadRiskStatusWith(contId) {
    var url = urlPrefix + "RiskStatus/GetAllIDName";
    LoadData(url, contId);
}

//function AddNewRISK(tableId) {
//    var lasttr = $('#' + tableId).find('tr[id^="Risk_"]');
//    lasttr = lasttr[lasttr.length - 1].id;
//    var newtr = lasttr.substring(lasttr.indexOf('_') + 1, lasttr.length);
//    newtr = parseInt(newtr) + 1;

//    $('#' + lasttr).after('<tr id="Risk_' + newtr + '" ><td style="width:99%;" class="bottomBorder"><table cellpadding="0" cellspacing="0" class="tsotop">' +
//						   '<tr><td class="titletd width13imp">Type of Risk</td><td class="seprator">:</td><td class="field" style="width:41% !important;">' +
//						   '<select id="OpRiskPresent_' + newtr + '" style="width:207px;float:left;"></select><div class="waterInfo" title="a. 0- No Risk &#013;b. 1-3 – low risk &#013;c. 4-6 medium risk &#013;d. 7-10 high risk">Hint</div></td>' +
//						   '<td class="field" style="width:42% !important;"><select id="OpRiskFuture_' + newtr + '" style="width:207px;float:left;"></select>' +
//						   '<div class="waterInfo" title="a. 0- No Risk &#013;b. 1-3 – low risk &#013;c. 4-6 medium risk &#013;d. 7-10 high risk">Hint</div></td></tr>' +
//						   '<tr><td class="titletd width13imp" style="text-align:left;">Indicator</td><td class="seprator">:</td><td class="field" style="width:41% !important;">' +
//						   '<select id="IndicatorPresent_' + newtr + '" style="width:207px;"></select></td><td class="field" style="width:42% !important;">' +
//						   '<select id="IndicatorFuture_' + newtr + '" style="width:207px;"></select></td></tr><tr><td class="titletd width13imp" style="text-align:left;">' +
//						   'Description</td><td class="seprator">:</td><td class="field" style="text-align:center;width:41% !important;">' +
//						   '<textarea id="riskDescriptionPresent_' + newtr + '" rows="3" cols="58" maxlength="500"></textarea></td> <td class="field" style="text-align:center;width:42% !important;">' +
//						   '<textarea id="riskDescriptionFuture_' + newtr + '" rows="3" cols="58" maxlength="500"></textarea></td></tr><tr><td class="titletd width13imp" style="text-align:left;">' +
//						   'Mitigation Description</td><td class="seprator">:</td><td class="field" style="text-align:center;width:41% !important;">' +
//						   '<textarea id="riskMitiDescriptionPresent_' + newtr + '" rows="3" cols="58" maxlength="500"></textarea></td><td class="field" style="text-align:center;width:42% !important;">' +
//						   '<textarea id="riskMitiDescriptionFuture_' + newtr + '" rows="3" cols="58" maxlength="500"></textarea></td></tr><tr><td class="titletd width13imp" style="text-align:left;">' +
//						   'Responsible Person</td><td class="seprator">:</td><td class="field" style="width:41% !important;">' +
//						   '<input id="txtResponsiblePerson_' + newtr + '" type="text" size="30" maxlength="100" style="float:none;" />' +
//						   '<input id="lblResponsiblePersonName_' + newtr + '" type="hidden" /><input id="lblResponsiblePersonMail_' + newtr + '" type="hidden" />' +
//						   '<input id="lblResponsiblePersonId_' + newtr + '" type="hidden" /></td><td class="field" style="width:42% !important;">' +
//						   'Due Date : <input id="txtDueDate_' + newtr + '" type="text" size="30" maxlength="100" style="float:none;" /></td></tr></table></td>' +
//						   '</tr>');

//    LoadOperationalRiskIndicatorWith('IndicatorPresent_' + newtr);
//    LoadOperationalRiskIndicatorWith('IndicatorFuture_' + newtr);
//    LoadOperationalRiskWith('OpRiskPresent_' + newtr);
//    LoadOperationalRiskWith('OpRiskFuture_' + newtr);
//    LoadRiskStatusWith('ddlStatusPresent' + newtr);

//    SetAutoCompleteForAD("txtResponsiblePerson_" + newtr, "lblResponsiblePersonMail_" + newtr);
//    SetAutoCompleteForAD("txtRaisedByPerson_" + newtr, "lblRaisedByPersonMail_" + newtr);

//    $('#txtDueDate_' + newtr).datepicker({
//        dateFormat: 'dd-M-yy', showButtonPanel: true, changeMonth: true, changeYear: true, firstDay: 1,
//        buttonImage: "/images/calendar.gif", buttonImageOnly: true, inline: true, showWeek: true, weekHeader: "W"
//    });
//}

function SaveRisk() {
    // returns array of values enter in risk discription
    var data1 = getMultiData('OpRiskPresent_');
   // var data2 = getMultiData('OpRiskFuture_');
    var data3 = getMultiData('IndicatorPresent_');
   // var data4 = getMultiData('IndicatorFuture_');
    var data5 = getMultiData('riskDescriptionPresent_');
   // var data6 = getMultiData('riskDescriptionFuture_');
    var data7 = getMultiData('riskMitiDescriptionPresent_');
  //  var data8 = getMultiData('riskMitiDescriptionFuture_');
    var data9 = getMultiData('txtResponsiblePerson_');
    var data11 = $("#lblResponsiblePersonId_0").val();
    var data10 = getMultiData('txtDueDate_');
    var txtTaskActualOid = $("#txtTaskActualOid").val();
    // var txtTaskPlannedOid = $("#txtTaskPlannedOid").val();

    var data12 = getMultiData('txtRaisedByPerson_');
    var data13 = $("#lblRaisedByPersonId_0").val();
    var data14 = getMultiData('riskImpactPresent_');
    var data15 = getMultiData('riskResolutionPresent_');
    var data16 = $("#ddlStatusPresent_0").val();

    var txtTSOServiceDelivierychainID=$("#txtTaskServiceChainIdOid").val();    
    if(txtTSOServiceDelivierychainID=="" ||txtTSOServiceDelivierychainID=="undefined")
    {
        txtTSOServiceDelivierychainID = getLocalStorage("taskid");
    }
    //if (data3[0] != data4[0])
    //{
    //    showMessageBox("Operational Indicator can not be different.", "red", "", false, "");
    //    return false;
    //}

    var duedate = jQuery.datepicker.parseDate("dd-M-yy", $("#txtDueDate_0").val());  
    var due= dateFormat(duedate, "isoDateddMonyyyy");
    var currentdate = new Date();
    var curr = dateFormat(currentdate, "isoDateddMonyyyy");

    if (data10[0] =='') {
        showMessageBox("Due Date can not blank", "red", "", false, "");
        return false;
    }
    else if (data1[0] == "3") {
        showMessageBox("Please select severity greater than zero", "red", "", false, "");
        return false;
    }
    else if (data9[0]=='')
    {
        showMessageBox("Responsible person can not blank", "red", "", false, "");
        return false;
    }
    else if (data12[0] == '') {
        showMessageBox("Raised By can not blank", "red", "", false, "");
        return false;
    }
    else if (data5[0] == '' || data7[0] == '')
    {
        showMessageBox("Please fill manadatory fields", "red", "", false, "");
        return false;
    }
   else if (data14[0] == '' || data15[0] == '')
    {
        showMessageBox("Please fill manadatory fields", "red", "", false, "");
        return false;
   }
   else if (due < curr) {
       showMessageBox("You are not allowed to select older week’s date.", "red", "", false, "txtDueDate_0");
       return false;

   }
    else if ($("#txtDueDate_0").val() != "") {
        try {
            jQuery.datepicker.parseDate("dd-M-yy", $("#txtDueDate_0").val());
        }
        catch (e) {
            showMessageBox("Please select valid Due date.", "red", "", false, "txtDueDate_0");
            return false;
        }
    }
    

    var myDate = new Date();
    var currentweek = myDate.getWeek();

    var data = JSON.stringify({
        WeekNumber: currentweek,
        ResponsiblePersonId: data11,
        DueDate: data10[0],
        ActualOperationalRiskId: data1[0],
        ActualOperationalRiskIndicatorId: data3[0],
        ActualOperationalRiskDescription: data5[0],
        ActualOperationalRiskMitigation: data7[0],
        //PlannedOperationalRiskId: data2[0],
        //PlannedOperationalRiskIndicatorId: data4[0],
        //PlannedOperationalRiskDescription: data6[0],
        //PlannedOperationalRiskMitigation: data8[0],
        CreatedBy: userName,
       // TSOServiceDeliveryChainId: getLocalStorage("taskid"),
        ID: txtTaskActualOid,
        //PlannedRiskId: txtTaskPlannedOid,        
        TSOServiceDeliveryChainId:txtTSOServiceDelivierychainID,
        Impact: data14[0],
        Resolution: data15[0],
        RaisedById: data13,
        StatusId: data16[0],
     
    });

    var urlAddUpdate = urlPrefix + "TSOServiceDeliveryChainRisk";
    var method = "POST";
    var message = "Risk saved successfully.";
    urlAddUpdate = urlAddUpdate + "/CreateRiskForTask";

    
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
              // localStorage.setItem("taskid", taskid);
                showMessageBox(response, "green", "RISKDashboard.html");
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

function getMultiData(controlID) {
    var valArray = [];
    $("[id^='" + controlID + "']").each(function () {
        var $this = $(this);
        var typeOfControl = $this.prop('type');
        if (typeOfControl == 'text' || typeOfControl == 'textarea') {
            var valu = $this[0].value;
            valArray.push(valu);
        }
        else if (typeOfControl == 'select-one') {
            var valu = $this.val();
            valArray.push(valu);
        }
    });
    return valArray;
}

$(function () {
    $("#txtResponsiblePerson_0").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtAccountManager", "#lblAccountManagerId");
            GetDBId("#lblResponsiblePersonMail_0", "#lblResponsiblePersonId_0");
        }
    });
});

$(function () {
    $("#txtRaisedByPerson_0").focusout(function () {
        if ($(this).val() != "") {
            //GetDBId("#txtAccountManager", "#lblAccountManagerId");
            GetDBId("#lblRaisedByPersonMail_0", "#lblRaisedByPersonId_0");
        }
    });
});

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

function getDateRangeOfWeek(weekNo) {
    var d1 = new Date();
    numOfdaysPastSinceLastMonday = eval(d1.getDay() - 1);
    d1.setDate(d1.getDate() - numOfdaysPastSinceLastMonday);
    var weekNoToday = d1.getWeek();
    var weeksInTheFuture = eval(weekNo - weekNoToday);
    d1.setDate(d1.getDate() + eval(7 * weeksInTheFuture));
    var rangeIsFrom = d1.getDate() + "/" + eval(d1.getMonth() + 1) + "/" + d1.getFullYear();
    d1.setDate(d1.getDate() + 4);
    var rangeIsTo = d1.getDate() +  "/" + eval(d1.getMonth() +1) + "/" +d1.getFullYear();
    return rangeIsFrom + " to " + rangeIsTo;
};

function LoadLastTSOServiceDeliveryChainTaskRisk() {
    var url = urlPrefix + "TSOServiceDeliveryChainRisk/GetLastTaskRisk/" + tsoServiceDeliveryChainActualRiskId;
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
                addToBreadcrumbArray(result.TSOServiceDeliveryChain.TSO.TSR.Title, result.TSOServiceDeliveryChain.TSO.TSR.ID, "TSR", true);
                addToBreadcrumbArray(result.TSOServiceDeliveryChain.TSO.Title, result.TSOServiceDeliveryChain.TSO.ID, "TSO", true);
                addToBreadcrumbArray(result.TSOServiceDeliveryChain.ServiceDeliveryChain.Name + "(" + result.TSOServiceDeliveryChain.ServiceDeliveryChain.Description + ")", result.TSOServiceDeliveryChainId, "TASK", true);
                CreateBreadcrumb();
                $("#OpRiskPresent_0").val(result.ActualOperationalRisk.ID);
               // $("#OpRiskFuture_0").val(result.PlannedOperationalRisk.ID);
                $("#IndicatorPresent_0").val(result.ActualOperationalRiskIndicatorId);
              //  $("#IndicatorFuture_0").val(result.PlannedOperationalRiskIndicator.ID);
                $("#riskDescriptionPresent_0").text(result.ActualOperationalRiskDescription);
               // $("#riskDescriptionFuture_0").text(result.PlannedOperationalRiskDescription);
                $("#riskMitiDescriptionPresent_0").text(result.ActualOperationalRiskMitigation);
               // $("#riskMitiDescriptionFuture_0").text(result.PlannedOperationalRiskMitigation);              
                
                $('#txtResponsiblePerson_0').val(result.ResponsiblePerson.Name);
                $("#lblResponsiblePersonName_0").text(result.ResponsiblePerson.Name);
                $("#lblResponsiblePersonMail_0").text(result.ResponsiblePerson.EmailID);
                $("#lblResponsiblePersonId_0").val(result.ResponsiblePerson.ID);
                $("#txtDueDate_0").val(dateFormat(result.DueDate.replace("T00:00:00", ""), "isoDateddMonyyyy"))
                $("#txtTaskActualOid").val(result.ID);
                //$("#txtTaskPlannedOid").val(result.PlannedRiskId);
                $("#txtTaskServiceChainIdOid").val(result.TSOServiceDeliveryChainId);

                $('#txtRaisedByPerson_0').val(result.RaisedBy.Name);
                $("#lblRaisedByPersonName_0").text(result.RaisedBy.Name);
                $("#lblRaisedByPersonMail_0").text(result.RaisedBy.EmailID);
                $("#lblRaisedByPersonId_0").val(result.RaisedBy.ID);
                
                $("#riskImpactPresent_0").text(result.Impact);
                $("#riskResolutionPresent_0").text(result.Resolution);
                $("#lastUpdatedOnPresent_0").val(dateFormat(result.CreatedOn.replace("T00:00:00", ""), "isoDateddMonyyyy"))
                $("#ddlStatusPresent_0").val(result.StatusId);
                $("#IndicatorPresent_0").attr("disabled", "disabled");
                //if (result.ActualOperationalRisk.RiskNo > 4)
                //{
                //    $("#riskDescriptionPresent_0").attr("disabled", false);                   
                //    $("#riskMitiDescriptionPresent_0").attr("disabled", false);
                    
                //}
                // if (result.PlannedOperationalRisk.RiskNo > 4)
                //{
                //    $("#riskDescriptionFuture_0").attr("disabled", false);
                //    $("#riskMitiDescriptionFuture_0").attr("disabled", false);
                //}

                if (roleId.toLowerCase() != "guest") {                   
                            if ($("#ddlStatusPresent_0 option:selected").text() == "Closed") {
                                $("#TaskInfo, #SubmitTask").attr("disabled", "disabled");
                                $("#ddlStatusPresent_0").attr("disabled", "disabled");
                                $("#riskDescriptionPresent_0").attr("disabled", "disabled");
                                $("#riskImpactPresent_0").prop("disabled", true);
                                $("#riskResolutionPresent_0").prop("disabled", true);
                                $("#lastUpdatedOnPresent_0").prop("disabled", true);                                                              
                                $("#txtDueDate_0").datepicker({ minDate: -1, maxDate: -2 }).attr('readonly', 'readonly');                              
                               
                            }
                        }
                    }                    
                     
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

function LoadTSR(tsoServiceDeliveryChainId) {
    var url = urlPrefix + "ServiceDeliveryChainTask/GetTSR/" + tsoServiceDeliveryChainId;

    $.ajax({
        type: "GET",
        url: url,
        beforeSend: function (request) {
            request.setRequestHeader("userid", userId);        },
        dataType: "json",
        success: function (result) {
            if (null != result) {
                //addToBreadcrumbArray(result.Title, result.ID, "TSR", true);               
                //CreateBreadcrumb();
                addToBreadcrumbArray(result.TSO.TSR.Title, result.TSO.TSR.ID, "TSR", true);
                addToBreadcrumbArray(result.TSO.Title, result.TSO.ID, "TSO", true);
                addToBreadcrumbArray(result.ServiceDeliveryChain.Name + "(" + result.ServiceDeliveryChain.Description + ")", result.ID, "TASK", true);
                CreateBreadcrumb();
                $('#txtResponsiblePerson_0').val(result.TSO.TSR.DeliveryManager.Name);
                $("#lblResponsiblePersonName_0").text(result.TSO.TSR.DeliveryManager.Name);
                $("#lblResponsiblePersonMail_0").text(result.TSO.TSR.DeliveryManager.EmailID);
                $("#lblResponsiblePersonId_0").val(result.TSO.TSR.DeliveryManager.ID);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

//function LoadTSOServiceDeliveryChainObj(tsoServiceDeliveryChainId) {
//    var url = urlPrefix + "ServiceDeliveryChainTask/GetServiceDeliveryChainTask/" + tsoServiceDeliveryChainId;

//    $.ajax({
//        type: "GET",
//        url: url,
//        beforeSend: function (request) {
//            request.setRequestHeader("userid", userId);
//        },
//        dataType: "json",
//        success: function (result) {
//            if (null != result) {
//                addToBreadcrumbArray(result.TSO.TSR.Title, result.TSO.TSR.ID, "TSR", true);
//                addToBreadcrumbArray(result.TSO.Title, result.TSOServiceDeliveryChain.TSO.ID, "TSO", true);
//                addToBreadcrumbArray(result.ServiceDeliveryChain.Name + "(" + result.ServiceDeliveryChain.Description + ")", result.ID, "TASK", true);
//                CreateBreadcrumb();               
//            }
//        },
//        error: function (jqXHR, textStatus, errorThrown) {
//            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
//        }
//    });
//}
function CancelRisk() {

   // localStorage.setItem("taskid", taskid);
    window.location = "RISKDashboard.html";
}
function callSuccess(message) {
    localStorage.setItem("taskid", taskid);
    window.location = "RiskDashboard.html";
}