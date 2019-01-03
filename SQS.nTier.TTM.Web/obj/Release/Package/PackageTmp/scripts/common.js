﻿var urlPrefix = "http://localhost:10048/api/v1/";

var clientId = 'sqsAuth';
var userId = '';
var userName = '';
var roleId = '';
var AllInfoUser = '';

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.replace("#", "").slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        if (hash[0].indexOf(".html") < 0) {
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
    }
    return vars;
}

function getLocalStorage(verName) {
    return localStorage.getItem(verName);
}

function bindUserPopUp() {
    $("#userName").text("Name : " + AllInfoUser.name);
    $("#userRole").text("Role : " + AllInfoUser.role);
}

$.fn.slideFadeToggle = function (easing, callback) {
    return this.animate({ opacity: 'toggle', height: 'toggle' }, 'fast', easing, callback);
};


function showMessageBox(message, color, url, isReload, controlToFocus, isUser) {
    hideLoader();
    if ($("#modal-wait").length < 1) {
        var dig = $('<div id="modal-wait" style="display:none" title="TTM"></div>');
        $(dig).html('<div style="display:table-cell;padding-left:20px;color:black;text-align:justify;"><p><b>' + message + '<b></p></div>').dialog({
            dialogClass: 'ui-dialog-notitlebar',
            height: "auto",
            width: "500",
            modal: true,
            closeOnEscape: false,
            draggable: false,
            resizable: false,
            open: $.proxy(function () {
                $(".ui-dialog-titlebar-close").remove();
                $(".ui-dialog-titlebar").css("width", "93%");
                if (typeof isUser != "undefined" && isUser != "" && isUser != null) {
                    $(".ui-dialog-buttonpane").css("width", "86%");
                }
                else {
                    $(".ui-dialog-buttonpane").css("width", "96%");
                }
                $(".ui-widget-overlay").css("opacity", "1");
                $(this).find(".ui-button").blur();
            }, this),
            position: { my: "center top", at: "center top+100" },
            buttons: [{
                text: "Ok",
                click: function (event) {
                    if (url != "" && url != null && typeof url != "undefined" && !isReload) {
                        window.location = url;
                    }
                    else if (isReload) {
                        location.reload();
                    }
                    else if (typeof controlToFocus != "undefined" && controlToFocus != "" && controlToFocus != null) {
                        $("#" + controlToFocus).focus();
                    }
                    $(this).dialog("destroy");
                }
            }]
        });
    }
}

function CheckUser() {
    var url = urlPrefix + "User/GetUser";

    var objUserJSON = JSON.stringify({ userName: userId, password: '', adUser: true });

    $.ajax({
        type: "POST",
        url: url,
        contentType: 'application/json; charset=UTF-8',
        data: objUserJSON,
        async: false,
        success: function (result) {
            if (result != null && parseInt(result.id) > 0) {
                userName = result.name;
                roleId = result.role;
                AllInfoUser = result;
                bindUserPopUp();

                if (roleId.toLowerCase() == "account manager") {
                    $("#trNewTSR").hide();
                }
                else {
                    $("#trNewTSR").show();
                }
                return userName;
            }
            else {
                //showMessageBox("Invalid Login", "red", "index.html", false, null, false);
                //userName = "";
                //roleId = "";
                window.location = "index.html";
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
        }
    });
}

var idleTimer = null;
var idleWait = 900000; // 10 minutes


function idleLogout() {
    window.onload = idleLogout;
    window.onmousemove = idleLogout;
    window.onmousedown = idleLogout;  // catches touchscreen presses as well      
    window.ontouchstart = idleLogout; // catches touchscreen swipes as well 
    window.onclick = idleLogout;      // catches touchpad clicks as well
    window.onkeypress = idleLogout;
    window.addEventListener('scroll', idleLogout, true);

    clearTimeout(idleTimer);
    var path = window.location.href;
    path = path.substring(path.lastIndexOf("/") + 1);
    if (path != "index.html") {
        idleTimer = setTimeout(function () {
            localStorage.clear();
            localStorage.setItem("signOn", false);
            window.location = "index.html";
        }, idleWait);
    }
}

function validation(val, validateType) {
    var isValide = true;
    if (validateType.toLowerCase() === "length") {
        if (val.length > 30) {
            isValide = false;
        }
    }
    else if (validateType.toLowerCase() === "digit") {
        if (val.length > 10) {
            isValide = false;
        }
    }
    else if (validateType.toLowerCase() === "date") {
        var re = new RegExp("^(([0-9])|([0-2][0-9])|([3][0-1]))\-(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\-\d{4}$");
        if (val.match(re)) {
            isValide = false;
        }
    }
    return isValide;
}


function showDeleteConfirm(message, ID) {
    if ($("#modal-wait").length < 1) {
        var dig = $('<div id="modal-wait" style="display:none" title="TTM"></div>');
        $(dig).html('<div style="display:table-cell;padding-left:20px;color:black;text-align:justify;"><p><b>' + message + '<b></p></div>').dialog({
            dialogClass: 'ui-dialog-notitlebar',
            height: "auto",
            width: "500",
            modal: true,
            closeOnEscape: false,
            draggable: false,
            resizable: false,
            open: $.proxy(function () {
                $(".ui-dialog-titlebar-close").remove();
                $(".ui-dialog-titlebar").css("width", "93%");
                $(".ui-dialog-buttonpane").css("width", "96%");
                $(".ui-widget-overlay").css("opacity", "1");
            }, this),
            position: { my: "center top", at: "center top+100" },
            buttons: [{
                text: "Ok",
                click: function () {
                    doDelete(ID);
                    $(this).dialog("destroy");
                }
            },
            {
                text: "Cancel",
                icon: "ui-icon-heart",
                click: function () {
                    $(this).dialog("destroy");
                }

            }]
        });
    }
}

function onlyten(e, field) {
    var val = field.value;
    if (val.length > 10) {
        field.value = val.substring(0, 10);
    }
}

function showLoader() {
    setTimeout(function () {
        $("#WaitCursor").html('<p>Loading....</p><div class="loader"></div>').dialog({
            dialogClass: 'ui-dialog-notitlebar',
            height: "300",
            width: "190",
            modal: true,
            closeOnEscape: false,
            draggable: false,
            resizable: false,
            open: $.proxy(function () {
                $(".ui-dialog-content").css("overflow-x", "hidden");
                //$(".ui-dialog-content").css("margin-left", "25px");
                $(".ui-dialog-titlebar-close").remove();
                $(".ui-dialog-titlebar").remove();
                $(".ui-widget-overlay").css("opacity", "1");
            }, this),
            position: { my: "center top", at: "center top+100" }
        });
    }, 0);
}

function hideLoader() {
    setTimeout(function () {
        try {
            $("#WaitCursor").dialog("close");
            $("#WaitCursor").dialog("destroy");
        } catch (e) {

        }
    }, 200);
}
//debugger;
var tsrId = getLocalStorage("tsrid"); //getUrlVars()["tsrid"];

var tsoId = getLocalStorage("tsoid");  //getUrlVars()["tsoid"];


function removerowToselect() {
    var windowURL = window.location.href;
    windowURL = windowURL.substring(0, windowURL.indexOf("rowToselect") - 1);
    window.history.pushState(null, null, windowURL);
}

function GotoTSODashboard() {
    localStorage.setItem("tsrid", "undefined");
    window.location = "TSODashboard.html";
}

function GotoTASKDashboard() {
    localStorage.setItem("tsoid", "undefined");
    window.location = "TASKDashboard.html";
}

function GotoRISKDashboard() {
    localStorage.setItem("taskid", "undefined");
    window.location = "RISKDashboard.html";
}


function CreateNewTSR(tsrId) {
    if (tsrId == '') {
        localStorage.removeItem("tsrid");
        window.location = "TSR.html";
    }
    else {
        localStorage.setItem("tsrid", tsrId);
        window.location = "TSR.html";
    }
}


function ViewTASK(tsoId) {
    localStorage.setItem("tsoid", tsoId);
    window.location = "TASKDashboard.html";
}

function ViewRisk(taskid) {
    localStorage.setItem("taskid", taskid);   
    window.location = "RISKDashboard.html";
}



function ViewTSO(tsrId) {
    localStorage.setItem("tsrid", tsrId);
    window.location = "TSODashboard.html";
}

function Home() {
    window.location = "Home.html";
}

function Wiki() {
    window.location.href = "Wiki.html";
}


function CreateNewTSO(tsoId) {
    if (tsoId == '') {
        localStorage.removeItem("tsoid");
        localStorage.setItem("tsrid", tsrId);
        window.location = "TSO.html";
    }
    else {
        localStorage.setItem("tsrid", tsrId);
        localStorage.setItem("tsoid", tsoId);
        window.location = "TSO.html";
    }
}

function CreateNewRISK(RiskID) {
    if (RiskID == '') {
        localStorage.removeItem("RiskID"); //Risk ID for Actual
        localStorage.setItem("tsrid", tsrId);
        localStorage.setItem("tsoid", tsoId);       
        window.location = "TaskRisk.html";
    }
    else {
        localStorage.setItem("tsrid", tsrId);
        localStorage.setItem("tsoid", tsoId);
        localStorage.setItem("RiskID", RiskID);//Risk ID for Actual
        window.location = "TaskRisk.html";
    }
}

function CreateNewRISKForEmpty(RiskID, taskid) {
    if (RiskID == '' && taskid != '') {
        localStorage.removeItem("RiskID"); //Risk ID for Actual
        localStorage.setItem("tsrid", tsrId);
        localStorage.setItem("tsoid", tsoId);
        localStorage.setItem("tsoServiceDeliveryChainId", taskid);
        window.location = "TaskRisk.html";
    }
}

function ViewTasks(tsoId, serviceDeliveryChainId, tsoServiceDeliveryChainId, tsrId) {
    localStorage.setItem("tsrid", tsrId);
    localStorage.setItem("tsoid", tsoId);
    localStorage.setItem("serviceDeliveryChainId", serviceDeliveryChainId);
    localStorage.setItem("tsoServiceDeliveryChainId", tsoServiceDeliveryChainId);
    if (serviceDeliveryChainId == 42) {
        window.location = "ManagementTasks.html";
    }
    else {
        window.location = "Tasks.html";
    }
}

function GotoTSRDashboard() {
    window.location = "TSRDashboard.html";
}

function sendEmail() {
    window.location = "mailto:ttm-support@sqs.com";
}

function showHelp() {
    window.location = "Help.html";
}

var breadcrumb = [];
function addToBreadcrumbArray(name, id, type, isLInk) {
    var none = true;
    for (i = 0; i < breadcrumb.length; i++) {
        if (breadcrumb[i].Name == name) {
            none = false;
        }
    }
    if (none) {
        breadcrumb.push({ "Name": name, "id": id, "Type": type, "isLink": isLInk });
    }
}

function CreateBreadcrumb() {
    $("#breadCrumb").show();
    $("#breadCrumb").empty();
    var width = $("#breadCrumb").width().toFixed(0);

    var totallength = 0;
    var maxlength = (width - breadcrumb.length * 50);

    var total = 0;
    var maxWidth = [];
    var i;
    for (i = 0; i < breadcrumb.length; i++) {
        if (typeof breadcrumb[i].Name !== "undefined") {
            total += breadcrumb[i].Name;
        }
    }
    var lengthPerCharacter = (maxlength / total.length);
    for (i = 0; i < breadcrumb.length; i++) {
        if (typeof breadcrumb[i].Name !== "undefined") {
            var l = Math.round(lengthPerCharacter * breadcrumb[i].Name.length);
            maxWidth.push(l);
        }
    }


    //namelength = span[0].offsetWidth + (breadcrumb.length * 20).toFixed(0);
    //var perelement = ((width / breadcrumb.length) - (breadcrumb.length * 20)).toFixed(0);
    //if (namelength < width) {
    //    perelement = 'auto';
    //}
    //else {
    //    perelement = perelement + "px";
    //}
    //var perelement = ((width / breadcrumb.length) - (breadcrumb.length * 20)).toFixed(0);
    for (var i = 0; i < breadcrumb.length; i++) {
        if (typeof breadcrumb[i].Name !== "undefined") {
            var linkClass = "bTitleLink";
            if (!breadcrumb[i].isLink) {
                linkClass = "bTitle";
            }

            var newLink = $("<div class='ellipsis " + linkClass + "' style='max-width:" + maxWidth[i] + "px !important' data-type='" + breadcrumb[i].Type + "' data-id='" + breadcrumb[i].id + "' title='" + breadcrumb[i].Name + "'>" + breadcrumb[i].Name + "</div>").on("click",
                        function () {
                            var objID = $(this).data("id");
                            if ($(this).data("type") == "TSR") {
                                CreateNewTSR(objID);
                            }
                            else if ($(this).data("type") == "TSO") {
                                CreateNewTSO(objID);
                            }
                        });

            $("#breadCrumb").append(newLink);
            var dd = newLink[0].offsetWidth;
            $("#breadCrumb").append("<div class='arrow-right'></div>");
        }
    }
}

function MasterData() {
    window.location = "../Masters/Index.html'target': '_blank'";
}

var isFilterApply = false;
function loadDataByRecord() {
    showLoader();
    var newstartingRecordNumber = 0;
    var pageSize = $("#pagesize option:selected").text();
    if (!isFilterApply) {
        loadPagingData(newstartingRecordNumber, pageSize);
    }
    else {
        callFilter(false, newstartingRecordNumber, pageSize);
    }
    if (newstartingRecordNumber == 0) {
        $("#pagedisplay").val(1);
    }
}

function GoPrevPage() {
    var pageDisplay = parseInt($("#pagedisplay").val());
    if (pageDisplay == 1) {
        //showMessageBox("Max limit reached !", "red");
        $('#first').attr('disabled', true);
        $('#prev').attr('disabled', true);

        $('#last').attr('disabled', false);
        $('#next').attr('disabled', false);
        return false;
    }
    else if (pageDisplay <= allpages) {
        $('#last').attr('disabled', false);
        $('#next').attr('disabled', false);

        showLoader();
        var pageSize = $("#pagesize option:selected").text();
        // var startingRecordNumber = parseInt(pageSize) * parseInt($("#pagedisplay").val() - 2) + 1;
        var newstartingRecordNumber = parseInt(pageSize) * (pageDisplay - 2);
        $("#pagedisplay").val(pageDisplay - 1);
        if (!isFilterApply) {
            onLoadClear();
            loadPagingData(newstartingRecordNumber, pageSize);
        }
        else {
            callFilter(false, newstartingRecordNumber, pageSize);
        }
    }
    else {
        $('#last').attr('disabled', false);
        $('#next').attr('disabled', false);

        showMessageBox("Page number is Invalid !", "red");
        $("#pagedisplay").val($("#pagedisplay").oldvalue());
        return false;
    }

}

/* Pagination textBox validation for ristrict String & Negetive Value */

$(function () {
    $("#pagedisplay").keyup(function () {
        var value = $(this).val();
        value = value.replace(/^(0*)/, "");
        $(this).val(value);
    });

    $("#pagedisplay").keydown(function (e) {
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            return;
        }
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $('*').bind('mousemove keydown scroll', function () {
        idleLogout();
    });

    $("body").trigger("mousemove");
});

function GoNextPage() {
    if (parseInt($("#pagedisplay").val()) == allpages) {
        $('#first').attr('disabled', false);
        $('#prev').attr('disabled', false);

        $('#last').attr('disabled', true);
        $('#next').attr('disabled', true);
        //showMessageBox("Max limit reached !", "red");
        return false;
    }
    else if (parseInt($("#pagedisplay").val()) <= allpages) {
        $('#first').attr('disabled', false);
        $('#prev').attr('disabled', false);

        showLoader();
        var pageSize = $("#pagesize option:selected").text();
        ////var startingRecordNumber = parseInt(pageSize) * parseInt($("#pagedisplay").val()) + 1;
        var pageDisplay = parseInt($("#pagedisplay").val());
        var newstartingRecordNumber = parseInt(pageSize) * pageDisplay;
        $("#pagedisplay").val(pageDisplay + 1);
        if (!isFilterApply) {
            onLoadClear();
            loadPagingData(newstartingRecordNumber, pageSize);
        }
        else {
            callFilter(false, newstartingRecordNumber, pageSize);
        }
    }
    else {
        $('#first').attr('disabled', true);
        $('#prev').attr('disabled', true);

        showMessageBox("Page number is Invalid !", "red");
        $("#pagedisplay").val($("#pagedisplay").oldvalue());
        return false;
    }
}

function GoFristPage() {
    if (parseInt($("#pagedisplay").val()) == 1) {
        $('#first').attr('disabled', true);
        $('#prev').attr('disabled', true);

        $('#last').attr('disabled', false);
        $('#next').attr('disabled', false);
        //showMessageBox("Max limit reached !", "red");
        return false;
    }
    else {
        $('#last').attr('disabled', false);
        $('#next').attr('disabled', false);

        showLoader();
        var pageSize = $("#pagesize option:selected").text();
        var newstartingRecordNumber = 0;
        $("#pagedisplay").val(1);
        if (!isFilterApply) {
            onLoadClear();
            loadPagingData(newstartingRecordNumber, pageSize);
        }
        else {
            callFilter(false, newstartingRecordNumber, pageSize);
        }
    }
}

function GoLastPage() {
    if (parseInt($("#pagedisplay").val()) == allpages) {
        $('#first').attr('disabled', false);
        $('#prev').attr('disabled', false);

        $('#last').attr('disabled', true);
        $('#next').attr('disabled', true);
        //showMessageBox("Max limit reached !", "red");
        return false;
    }
    else if (allpages == 0) {
        $('#first').attr('disabled', true);
        $('#prev').attr('disabled', true);

        $('#last').attr('disabled', true);
        $('#next').attr('disabled', true);
        //showMessageBox("Max limit reached !", "red");
        return false;
    }
    else {
        $('#first').attr('disabled', false);
        $('#prev').attr('disabled', false);

        showLoader();
        var pageSize = $("#pagesize option:selected").text();
        var newstartingRecordNumber = parseInt(pageSize) * parseInt(allpages - 1);
        $("#pagedisplay").val(allpages);
        if (!isFilterApply) {
            onLoadClear();
            loadPagingData(newstartingRecordNumber, pageSize);
        }
        else {
            callFilter(false, newstartingRecordNumber, pageSize);
        }
    }
}

var totalRecords;
var oldPages;
var pages;
var isSearch = false;
function showPages(url) {
    $.ajax({
        type: "GET",
        url: url,
        dataType: "json",
        beforeSend: function (xhr) {
            xhr.setRequestHeader('userid', userId);
        },
        async: false,
        success: function (result) {
            totalRecords = parseInt(result);
            if (isNaN(totalRecords))
                totalRecords = 0;


            oldPages = totalRecords;
            var selectedSize = $("#pagesize option:selected").text();
            if (selectedSize != "All") {
                totalRecords = totalRecords / parseInt(selectedSize);
                totalRecords = totalRecords.toString();

                if (totalRecords.indexOf('.') > -1) {
                    pages = totalRecords.substring(0, totalRecords.indexOf('.'));
                }
                else {
                    pages = totalRecords;
                }

                if (totalRecords > pages) {
                    pages = parseInt(pages) + 1;
                }

                allpages = pages;
                if (isNaN(pages) || pages == 0) {
                    pages = 1;
                }

                var message;

                if (pages == 1 || pages == 0) {
                    $('#first').attr('disabled', true);
                    $('#prev').attr('disabled', true);
                    $('#last').attr('disabled', true);
                    $('#next').attr('disabled', true);
                    message = "page of 1 page.";
                }
                else {
                    $('#first').attr('disabled', false);
                    $('#prev').attr('disabled', false);
                    $('#last').attr('disabled', false);
                    $('#next').attr('disabled', false);
                    message = "page of " + pages + " pages.";
                }
                $("#totalpage").text(message);
            }
            else {
                $('#first').attr('disabled', true);
                $('#prev').attr('disabled', true);
                $('#last').attr('disabled', true);
                $('#next').attr('disabled', true);
                $("#totalpage").text("page of 1 page.");

            }
            //hideLoader();
        },
        error: function (jqXHR, textStatus, errorThrown) {

            hideLoader();
            if (errorThrown == " Unauthorized") {
                textStatus = "You do not have permission to made any changes in TSR";
            }
            showMessageBox(errorThrown, "red");
            //showMessageBox(jqXHR.responseText + "--" + textStatupo's + "--" + errorThrown, "red");
        }
    });
}

function showTotalpages(totalRecords) {
    var selectedSize = $("#pagesize option:selected").text();
    totalRecords = totalRecords / parseInt(selectedSize);
    totalRecords = totalRecords.toString();

    if (totalRecords.indexOf('.') > -1) {
        pages = totalRecords.substring(0, totalRecords.indexOf('.'));
    }
    else {
        pages = totalRecords;
    }

    if (totalRecords > pages) {
        pages = parseInt(pages) + 1;
    }

    allpages = pages;
    if (isNaN(pages) || pages == 0) {
        pages = 1;
    }

    var message;

    if (pages == 1 || pages == 0) {
        message = "page of 1 page.";
    }
    else {
        message = "page of " + pages + " pages.";
    }
    $("#totalpage").text(message);
}

function getAllPageSize() {
    return parseInt(oldPages) * 10;
}

function getStartId() {
    var rowId = 0;
    if (totalRecords.indexOf('.') > -1) {
        rowId = totalRecords.substring(0, totalRecords.indexOf('.'));
    }

    return rowId = (rowId * $("#pagesize option:selected").text()) + 1;
}


function notZero(e, field) {
    var val = field.value;
    if (val == 0) {
        showMessageBox("Week must be greater than 0 !", "red");
        return false;
    }
}

function varFormat(string, arrayofStrings) {
    for (var k = 0; k < arrayofStrings.length; k++) {
        string = string.replace("{" + k + "}", arrayofStrings[k])
    }
    return string
}

function checkClose(e, field) {

    if (roleId.toLowerCase() != "admin") {
        //On-hold ,  In progress ,  Created  ,   Closed  ,  Cancelled
        var defalutMsg = "You can not change the status from {0} to {1} !";
        var newval = $("#" + field.id + " option:selected").text().toLowerCase();
        var newValId = $("#" + field.id + " option:selected").val();
        var oldVal = $("#" + field.id + " option[value='" + field.aOldValue + "']").text().toLowerCase();
        var oldValId = $("#" + field.id + " option[value='" + field.aOldValue + "']").val();
        if (newval == "closed" && oldVal == "created") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(oldValId);
            return false;
        }
        else if ((oldVal == "in progress" || oldVal == "on-hold" || oldVal == "closed" || oldVal == "cancelled") && newval == "created") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(oldValId);
            return false;
        }
        else if ((oldVal == "closed" || oldVal == "cancelled") && newval == "in progress") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(oldValId);
            return false;
        }
        else if ((oldVal == "closed" || oldVal == "cancelled") && newval == "on-hold") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(oldValId);
            return false;
        }
        else {
            $("#" + field.id).val(newValId);
            return true;
        }
    }
    else {
        return true;
    }
}

function checkCloseOnSubmit(field, aOldValue) {

    //On-hold ,  In progress ,  Created  ,   Closed  ,  Cancelled
    if (roleId.toLowerCase() != "admin") {
        var defalutMsg = "You can not change the status from {0} to {1} !";
        var newval = $("#" + field + " option:selected").text().toLowerCase();
        var oldVal = $("#" + field + " option[value='" + aOldValue + "']").text().toLowerCase();
        if (newval == "closed" && oldVal == "created") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(aOldValue);
            return false;
        }
        else if ((oldVal == "in progress" || oldVal == "on-hold" || oldVal == "closed" || oldVal == "cancelled") && newval == "created") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(aOldValue);
            return false;
        }
        else if ((oldVal == "closed" || oldVal == "cancelled") && newval == "in progress") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(aOldValue);
            return false;
        }
        else if ((oldVal == "closed" || oldVal == "cancelled") && newval == "on-hold") {
            showMessageBox(varFormat(defalutMsg, [oldVal, newval]), "red");
            $("#" + field.id).val(aOldValue);
            return false;
        }
    }

}

function highLightrow(objrowId, tableID) {
    if (typeof objrowId != "undefined" && objrowId != "" && objrowId != null) {
        $('#' + tableID + ' > tbody  > tr').each(function () {
            var id = $(this).find("a.accordion-section-title").attr("id");
            if (typeof id != "undefined") {
                if (id.indexOf(objrowId) >= 0) {
                    $(this).addClass("rowSelect");
                }
            }
        });
    }
}

Date.prototype.getWeek = function () {
    var onejan = new Date(this.getFullYear(), 0, 1);
    var today = new Date(this.getFullYear(), this.getMonth(), this.getDate());
    var dayOfYear = ((today - onejan + 86400000) / 86400000);
    return Math.ceil(dayOfYear / 7)
};


function ExportExcelDump() {
    $("#ExportExcel").dialog({
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
                    text: "Export to csv",
                    click: function () {
                        var url;
                        showLoader();
                        var elt = document.getElementById("TypeOfExport");
                        var today = new Date();
                        var todaysdate = today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear();
                        if (elt.value === "TSR") {
                            var url = urlPrefix + "TSR/GetTSRDump/";
                            $.ajax({
                                method: 'GET',
                                url: url,
                                beforeSend: function (request) {
                                    request.setRequestHeader("userid", userId);
                                },
                                responseType: 'json',
                                success: function (result) {
                                    var filename = "iMS TSR Dashboard " + todaysdate + ".csv";
                                    var contentType = "application/octet-stream;charset=utf-8;";
                                    try {

                                        var blob = new Blob(["\ufeff", result], { type: contentType }); //new Blob(["\ufeff", csv_content]);
                                        //Check if user is using IE
                                        var ua = window.navigator.userAgent;
                                        var msie = ua.indexOf("MSIE ");

                                        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                                            window.navigator.msSaveBlob(blob, filename);
                                            //alert('Please click on save to save the file');                                            
                                        }
                                        else  // If another browser, return 0
                                        {
                                            //Create a url to the blob
                                            var url = window.URL.createObjectURL(blob);
                                            var linkElement = document.createElement('a');
                                            linkElement.setAttribute('href', url);
                                            linkElement.setAttribute("download", filename);

                                            //Force a download
                                            var clickEvent = new MouseEvent("click", {
                                                "view": window,
                                                "bubbles": true,
                                                "cancelable": false
                                            });
                                            linkElement.dispatchEvent(clickEvent);
                                        }
                                    } catch (ex) {
                                        console.log(ex);
                                    }
                                    $(".ui-dialog-content").dialog('destroy');
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    hideLoader();
                                    if (jqXHR != null && jqXHR.responseJSON != null && jqXHR.responseJSON.message != "" && jqXHR.responseJSON.message == "The request date is incorrect - check your system clock.") {
                                        showMessageBox("Session timed out.\nRedirecting to login page...", "red", "Index.html", false);
                                    }
                                    else {
                                        showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
                                    }
                                }
                            });
                        }
                        else if (elt.value === "TSO") {
                            url = urlPrefix + "TSO/GetTSODump/";
                            $.ajax({
                                method: 'GET',
                                url: url,
                                beforeSend: function (request) {
                                    request.setRequestHeader("userid", userId);
                                },
                                responseType: 'json',
                                success: function (result) {
                                    var today = new Date();
                                    var filename = "iMS TSO Dashboard " + todaysdate + ".csv";
                                    var contentType = "application/octet-stream;charset=utf-8;";
                                    try {
                                        var blob = new Blob(["\ufeff", result], { type: contentType });
                                        //Check if user is using IE
                                        var ua = window.navigator.userAgent;
                                        var msie = ua.indexOf("MSIE ");

                                        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                                            window.navigator.msSaveBlob(blob, filename);
                                            //alert('Please click on save to save the file');                                            
                                        }
                                        else  // If another browser, return 0
                                        {
                                            //Create a url to the blob
                                            var url = window.URL.createObjectURL(blob);
                                            var linkElement = document.createElement('a');
                                            linkElement.setAttribute('href', url);
                                            linkElement.setAttribute("download", filename);

                                            //Force a download
                                            var clickEvent = new MouseEvent("click", {
                                                "view": window,
                                                "bubbles": true,
                                                "cancelable": false
                                            });
                                            linkElement.dispatchEvent(clickEvent);
                                        }
                                    } catch (ex) {
                                        console.log(ex);
                                    }
                                    $(".ui-dialog-content").dialog('destroy');
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    hideLoader();
                                    if (jqXHR != null && jqXHR.responseJSON != null && jqXHR.responseJSON.message != "" && jqXHR.responseJSON.message == "The request date is incorrect - check your system clock.") {
                                        showMessageBox("Session timed out.\nRedirecting to login page...", "red", "Index.html", false);
                                    }
                                    else {
                                        showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
                                    }
                                }
                            });
                        }
                        else if (elt.value === "Tasks") {
                            var txtStartDate = $('#txtStartDate').val()
                            var txtEndDate = $('#txtEndDate').val()
                            url = urlPrefix + "TSO/GetServiceDeliveryChainTasksDump/";
                            tdata = { startDate: txtStartDate, endDate: txtEndDate };
                            $.ajax({
                                method: 'GET',
                                url: url,
                                beforeSend: function (request) {
                                    request.setRequestHeader("userid", userId);
                                },
                                responseType: 'json',
                                data: tdata,
                                success: function (result) {
                                    var filename = "iMS TASK Dashboard " + todaysdate + ".csv";
                                    var contentType = "application/octet-stream;charset=utf-8;";
                                    try {
                                        var blob = new Blob(["\ufeff", result], { type: contentType });
                                        //Check if user is using IE
                                        var ua = window.navigator.userAgent;
                                        var msie = ua.indexOf("MSIE ");

                                        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                                            window.navigator.msSaveBlob(blob, filename);
                                            //alert('Please click on save to save the file');                                            
                                        }
                                        else  // If another browser, return 0
                                        {
                                            //Create a url to the blob
                                            var url = window.URL.createObjectURL(blob);
                                            var linkElement = document.createElement('a');
                                            linkElement.setAttribute('href', url);
                                            linkElement.setAttribute("download", filename);

                                            //Force a download
                                            var clickEvent = new MouseEvent("click", {
                                                "view": window,
                                                "bubbles": true,
                                                "cancelable": false
                                            });
                                            linkElement.dispatchEvent(clickEvent);
                                        }
                                    } catch (ex) {
                                        console.log(ex);
                                    }
                                    $(".ui-dialog-content").dialog('destroy');
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    hideLoader();
                                    if (jqXHR != null && jqXHR.responseJSON != null && jqXHR.responseJSON.message != "" && jqXHR.responseJSON.message == "The request date is incorrect - check your system clock.") {
                                        showMessageBox("Session timed out.\nRedirecting to login page...", "red", "Index.html", false);
                                    }
                                    else {
                                        showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
                                    }


                                }
                            });
                        }
                        else if (elt.value === "Risks") {
                            var txtStartDate = $('#txtStartDate').val()
                            var txtEndDate = $('#txtEndDate').val()
                            url = urlPrefix + "TSOServiceDeliveryChainRisk/GetRisksDump/";
                            tdata = { startDate: txtStartDate, endDate: txtEndDate };
                            $.ajax({
                                method: 'GET',
                                url: url,
                                beforeSend: function (request) {
                                    request.setRequestHeader("userid", userId);
                                },
                                responseType: 'json',
                                data: tdata,
                                success: function (result) {
                                    var filename = "iMS Risk Dashboard " + todaysdate + ".csv";
                                    var contentType = "application/octet-stream;charset=utf-8;";
                                    try {
                                        var blob = new Blob(["\ufeff", result], { type: contentType });
                                        //Check if user is using IE
                                        var ua = window.navigator.userAgent;
                                        var msie = ua.indexOf("MSIE ");

                                        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
                                            window.navigator.msSaveBlob(blob, filename);
                                            //alert('Please click on save to save the file');                                            
                                        }
                                        else  // If another browser, return 0
                                        {
                                            //Create a url to the blob
                                            var url = window.URL.createObjectURL(blob);
                                            var linkElement = document.createElement('a');
                                            linkElement.setAttribute('href', url);
                                            linkElement.setAttribute("download", filename);

                                            //Force a download
                                            var clickEvent = new MouseEvent("click", {
                                                "view": window,
                                                "bubbles": true,
                                                "cancelable": false
                                            });
                                            linkElement.dispatchEvent(clickEvent);
                                        }
                                    } catch (ex) {
                                        console.log(ex);
                                    }
                                    $(".ui-dialog-content").dialog('destroy');
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    hideLoader();
                                    if (jqXHR != null && jqXHR.responseJSON != null && jqXHR.responseJSON.message != "" && jqXHR.responseJSON.message == "The request date is incorrect - check your system clock.") {
                                        showMessageBox("Session timed out.\nRedirecting to login page...", "red", "Index.html", false);
                                    }
                                    else {
                                        showMessageBox(jqXHR.responseText + "--" + textStatus + "--" + errorThrown, "red");
                                    }


                                }
                            });
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
}

function menuhighlight(middle) {
    var last = middle.substring(middle.indexOf('-'), middle.length);
    //$('#top' + last).css('background-color', '#F7B36F');
    $('#' + middle).css('background-color', '#fb8c20');
    //$('#bottom' + last).css('background-color', '#F7B36F');
}

function menuRemove(middle) {
    var last = middle.substring(middle.indexOf('-'), middle.length);

    if (middle == "middel-8") {
        //$('#top' + last).css('background-color', '#F7B36F');
        $('#' + middle).css('background-color', '#fb8c20');
        //$('#bottom' + last).css('background-color', '#F7B36F');
    }
    else {
        //$('#top' + last).css('background-color', '');
        $('#' + middle).css('background-color', '');
        // $('#bottom' + last).css('background-color', '');

        //$('#top-2').css('background-color', '');
        $('#middel-2').css('background-color', '');
        // $('#bottom-2').css('background-color', '');

        //$('#top-8').css('background-color', '');
        $('#middel-8').css('background-color', '');
        //$('#bottom-8').css('background-color', '');

        //if ($('.child_list:visible').length > 0) {
        //    $(".child_list").css("display", "none");
        //}

        if ($('.user_list:visible').length > 0) {
            // $(".user_list").css("display", "none");
        }
    }
}

function hideAllMenu() {
    if ($('.user_list:visible').length > 0) {
        $(".user_list").css("display", "none");
    }

    //$('#top-2').css('background-color', '');
    $('#middel-2').css('background-color', '');
    //$('#bottom-2').css('background-color', '');

    // $('#top-8').css('background-color', '');
    $('#middel-8').css('background-color', '');
    //$('#bottom-8').css('background-color', '');
}

var fileds = [];
var dataString = { "strid": 0, "strtitle": "none", "strclient": "none", "strcenter": "none", "strpractice": "none", "strstatus": -1 };//, "strweek": 0 
function filterTable(tDeatilbody) {

    $('[id^=filter_]').each(function () {
        var index = $(this).attr('id');
        index = index.substring(index.indexOf("_") + 1, index.length);
        var dd = $(this).val();
        if ($(this).val() != null) {
            if ($(this).val().trim() != "") {
                if ($(this).val().trim())
                    var updateRow = null;
                var isadd = true;
                for (var i = 0; i < fileds.length; i++) {
                    if (fileds[i][0] == index) {
                        updateRow = i;
                        isadd = false;
                        break;
                    }
                }

                if (isadd) {
                    if ($(this).val().trim() != ".") {
                        var value = singleDot($(this).val().trim());
                        fileds.push([index, value]);
                    }
                    else {
                        $(this).val("");
                    }
                }

                if (updateRow != null) {
                    if ($(this).val().trim() != ".") {
                        var value = singleDot($(this).val().trim());
                        fileds[updateRow][1] = value;
                    }
                    else {
                        $(this).val("");
                    }
                }
            }
            else {
                if (fileds.length > 0) {
                    for (var i = 0; i < fileds.length; i++) {
                        if (fileds[i][0] == index) {
                            fileds.splice(i, 1);
                            if ("str" + index.toLowerCase().trim() == "strid" || "str" + index.toLowerCase().trim() == "strstatus") {
                                dataString["str" + index.toLowerCase().trim()] = 0;
                            }
                            else {
                                dataString["str" + index.toLowerCase().trim()] = "none";
                            }
                            break;
                        }
                    }
                }
            }
        }
    });
}


function Filter(url) {

    if (fileds.length > 0) {
        showLoader();
        var fullurl = url;
        for (var p = 0; p < fileds.length; p++) {
            if (fileds[p][1].toLowerCase().trim() != "select status" && fileds[p][1].toLowerCase().trim() != "select risk") {
                dataString["str" + fileds[p][0].toLowerCase().trim()] = fileds[p][1].toLowerCase().trim();
            }
            else {
                dataString["str" + fileds[p][0].toLowerCase().trim()] = -1;
            }
        }

        var isApply = false;

        if (dataString["strcenter"] != "none") {
            isApply = true;
            dataString["strcenter"] = getSpecialChar(dataString["strcenter"]);
        }

        if (dataString["strclient"] != "none") {
            isApply = true;
            dataString["strclient"] = getSpecialChar(dataString["strclient"]);
        }

        if (parseInt(dataString["strid"]) > 0) {
            isApply = true;
            dataString["strid"] = getSpecialChar(dataString["strid"]);
        }

        if (dataString["strtitle"] != "none") {
            isApply = true;
            dataString["strtitle"] = getSpecialChar(dataString["strtitle"]);
        }

        if (dataString["strpractice"] != "none") {
            isApply = true;
            dataString["strpractice"] = getSpecialChar(dataString["strpractice"]);
        }

        if (dataString["strstatus"] > 0) {
            isApply = true;
        }

        //if (parseInt(dataString["strweek"]) > 0) {
        //    isApply = true;
        //    dataString["strweek"] = getSpecialChar(dataString["strweek"]);
        //}

        if (isApply) {
            fullurl = fullurl + "/" + dataString["strcenter"];
            fullurl = fullurl + "/" + dataString["strclient"];
            fullurl = fullurl + "/" + dataString["strid"];
            fullurl = fullurl + "/" + dataString["strtitle"];
            fullurl = fullurl + "/" + dataString["strstatus"];
            fullurl = fullurl + "/" + dataString["strpractice"];
            //fullurl = fullurl + "/" + dataString["strweek"];

            LoadSearchData(fullurl);
            isFilterApply = true;
        }
        else {
            hideLoader();
            showMessageBox("Invalid filter !", "red", "", false, "", false);
        }
    }
}

function showFilterPages(url) {
    var fullurl = url;
    var isApply = false;

    if (dataString["strcenter"] != "none") {
        isApply = true;
    }

    if (dataString["strclient"] != "none") {
        isApply = true;
    }

    if (parseInt(dataString["strid"]) > 0) {
        isApply = true;
    }

    if (dataString["strtitle"] != "none") {
        isApply = true;
    }

    if (dataString["strstatus"] > 0) {
        isApply = true;
    }

    if (dataString["strpractice"] != "none") {
        isApply = true;
    }

    if (isApply) {
        fullurl = fullurl + "/" + dataString["strcenter"];
        fullurl = fullurl + "/" + dataString["strclient"];
        fullurl = fullurl + "/" + dataString["strid"];
        fullurl = fullurl + "/" + dataString["strtitle"];
        fullurl = fullurl + "/" + dataString["strstatus"];
        fullurl = fullurl + "/" + dataString["strpractice"];

        showPages(fullurl);
    }
}


function clearFilter(tDeatilbody) {
    if (checkFilter()) {
        showLoader();
        $('input[id^=filter_]').val("");
        $('#filter_Status').val("Select Status");
        $('#filter_Status').val("Select Risk");
        $('#filter_RiskStatus').val("Select Status");
        filterTable(tDeatilbody, "", 0);
        riskfilterTable(tDeatilbody, "", 0);
        fileds = [];
        riskfileds = [];
        isFilterApply = false;
    }
    var pageSize = $("#pagesize option:selected").text();
    loadPagingData(0, pageSize);
}

function onLoadClear() {
    $('input[id^=filter_]').val("");
    $('#filter_Status').val("Select Status");
}


function gotoMaster() {
    var win = window.open('../Masters/Client.html?user=' + encodeURI(userId), '_blank');
    win.focus();
    //location.href('../Masters/Index.html?user=' + encodeURI(userId));
}

function gotoWiki() {
    var win = window.open('../Portal/Wiki.html', '_blank');
    win.focus();
    //location.href('../Portal/Wiki.html?user=' + encodeURI(userId));
}

function checkFilter() {
    var checkUpdate = false;
    $('[id^=filter_]').each(function () {
        if ($(this).val() != null) {
            var dd = $(this).val().trim();
            if ($(this).val().trim() != "" && $(this).val().trim() != "Select Status") {
                checkUpdate = true;
            }
        }
    });

    if ($("#filter_Status option:selected").text() != "Select Status") {
        checkUpdate = true;
    }
    return checkUpdate;
}


var riskfileds = [];
var riskdataString = { "strid": 0, "strtitle": "none", "strclient": "none", "strcenter": "none", "strpractice": "none", "strstatus": -1, "strweek": -1, "strriskstatus":-1 };
function riskfilterTable(tDeatilbody) {

    $('[id^=filter_]').each(function () {
        var index = $(this).attr('id');
        index = index.substring(index.indexOf("_") + 1, index.length);
        var dd = $(this).val();
        if ($(this).val() != null) {
            if ($(this).val().trim() != "") {
                if ($(this).val().trim())
                    var updateRow = null;
                var isadd = true;
                for (var i = 0; i < riskfileds.length; i++) {
                    if (riskfileds[i][0] == index) {
                        updateRow = i;
                        isadd = false;
                        break;
                    }
                }

                if (isadd) {
                    if ($(this).val().trim() != ".") {
                        var value = singleDot($(this).val().trim());
                        riskfileds.push([index, value]);
                    }
                    else {
                        $(this).val("");
                    }
                }

                if (updateRow != null) {
                    if ($(this).val().trim() != ".") {
                        var value = singleDot($(this).val().trim());
                        riskfileds[updateRow][1] = value;
                    }
                    else {
                        $(this).val("");
                    }
                }
            }
            else {
                if (riskfileds.length > 0) {
                    for (var i = 0; i < riskfileds.length; i++) {
                        if (riskfileds[i][0] == index) {
                            riskfileds.splice(i, 1);
                            if ("str" + index.toLowerCase().trim() == "strid" || "str" + index.toLowerCase().trim() == "strstatus") {
                                riskdataString["str" + index.toLowerCase().trim()] = 0;
                            }
                            else {
                                riskdataString["str" + index.toLowerCase().trim()] = "none";
                            }
                            break;
                        }
                    }
                }
            }
        }
    });
}


function RiskFilter(url) {

    if (riskfileds.length > 0) {
        showLoader();
        var fullurl = url;
        for (var p = 0; p < riskfileds.length; p++) {
            if (riskfileds[p][1].toLowerCase().trim() != "select status" && riskfileds[p][1].toLowerCase().trim() != "select risk") {
                riskdataString["str" + riskfileds[p][0].toLowerCase().trim()] = riskfileds[p][1].toLowerCase().trim();
            }
            else {
                riskdataString["str" + riskfileds[p][0].toLowerCase().trim()] = -1;
            }
        }

        var isApply = false;

        if (riskdataString["strcenter"] != "none") {
            isApply = true;
            riskdataString["strcenter"] = getSpecialChar(riskdataString["strcenter"]);
        }

        if (riskdataString["strclient"] != "none") {
            isApply = true;
            riskdataString["strclient"] = getSpecialChar(riskdataString["strclient"]);
        }

        if (parseInt(riskdataString["strid"]) > 0) {
            isApply = true;
            riskdataString["strid"] = getSpecialChar(riskdataString["strid"]);
        }

        if (riskdataString["strtitle"] != "none") {
            isApply = true;
            riskdataString["strtitle"] = getSpecialChar(riskdataString["strtitle"]);
        }

        if (riskdataString["strpractice"] != "none") {
            isApply = true;
            riskdataString["strpractice"] = getSpecialChar(riskdataString["strpractice"]);
        }

        if (riskdataString["strstatus"] > 0) {
            isApply = true;
        }
        if (riskdataString["strriskstatus"] > 0) {
            isApply = true;
        }

        if (parseInt(riskdataString["strweek"]) > 0) {
            isApply = true;
            riskdataString["strweek"] = getSpecialChar(riskdataString["strweek"]);
        }
        if (riskdataString["strweek"] == "none") {
            isApply = true;
            riskdataString["strweek"] = -1;//getSpecialChar(riskdataString["strweek"]);
        }

        if (isApply) {
            fullurl = fullurl + "/" + riskdataString["strcenter"];
            fullurl = fullurl + "/" + riskdataString["strclient"];
            fullurl = fullurl + "/" + riskdataString["strid"];
            fullurl = fullurl + "/" + riskdataString["strtitle"];
            fullurl = fullurl + "/" + riskdataString["strstatus"];
            fullurl = fullurl + "/" + riskdataString["strpractice"];
            fullurl = fullurl + "/" + riskdataString["strweek"];
            fullurl = fullurl + "/" + riskdataString["strriskstatus"];

            LoadSearchData(fullurl);
            isFilterApply = true;
        }
        else {
            hideLoader();
            showMessageBox("Invalid filter !", "red", "", false, "", false);
        }
    }
}

function showRiskFilterPages(url) {
    var fullurl = url;
    var isApply = false;

    if (riskdataString["strcenter"] != "none") {
        isApply = true;
    }

    if (riskdataString["strclient"] != "none") {
        isApply = true;
    }

    if (parseInt(riskdataString["strid"]) > 0) {
        isApply = true;
    }

    if (riskdataString["strtitle"] != "none") {
        isApply = true;
    }

    if (riskdataString["strstatus"] > 0) {
        isApply = true;
    }

    if (riskdataString["strriskstatus"] > 0) {
        isApply = true;
    }
    if (riskdataString["strpractice"] != "none") {
        isApply = true;
    }

    if (riskdataString["strweek"] > 0 || riskdataString["strweek"] !="none") {
        isApply = true;
    }
  

    if (isApply) {
        fullurl = fullurl + "/" + riskdataString["strcenter"];
        fullurl = fullurl + "/" + riskdataString["strclient"];
        fullurl = fullurl + "/" + riskdataString["strid"];
        fullurl = fullurl + "/" + riskdataString["strtitle"];
        fullurl = fullurl + "/" + riskdataString["strstatus"];
        fullurl = fullurl + "/" + riskdataString["strpractice"];
        fullurl = fullurl + "/" + riskdataString["strweek"];
        fullurl = fullurl + "/" + riskdataString["strriskstatus"];

        showPages(fullurl);
    }
}


function clearRiskFilter(tDeatilbody) {
    if (checkFilter()) {
        showLoader();
        $('input[id^=filter_]').val("");       
        $('#filter_Status').val("Select Risk");
        $('#filter_RiskStatus').val("Select Status");      
        riskfilterTable(tDeatilbody, "", 0);        
        riskfileds = [];
        isFilterApply = false;
    }
    var pageSize = $("#pagesize option:selected").text();
    loadPagingData(0, pageSize);
}
function showRecord() {
    showLoader();
    var pageSize = $("#pagesize option:selected").text();
    ////var startingRecordNumber = parseInt(pageSize) * parseInt($("#pagedisplay").val()) + 1;
    var pageDisplay = parseInt($("#pagedisplay").val());
    var newstartingRecordNumber = parseInt(pageSize) * (pageDisplay - 1);
    if (!isFilterApply) {
        onLoadClear();
        loadPagingData(newstartingRecordNumber, pageSize);
    }
    else {
        callFilter(false, newstartingRecordNumber, pageSize);
    }
}

function onlyDigit(controll) {
    var digit = $(controll).val().replace(/[^0-9]/g, '');
    digit = digit.substring(0, 8);
    $(controll).val(digit);
}

function onlyString(controll) {
    var digit = $(controll).val();
    digit = digit.substring(0, 50);
    digit = singleDot(digit);
    $(controll).val(digit);
}

function singleDot(val) {
    var dots = 0;
    var length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '.') {
            dots++;
        }
        if (dots > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var slash = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '/') {
            slash++;
        }
        if (slash > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var star = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '*') {
            star++;
        }
        if (star > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var and = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '&') {
            and++;
        }
        if (and > 2) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var upArrow = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '^') {
            upArrow++;
        }
        if (upArrow > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var rightArrow = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '<') {
            rightArrow++;
        }
        if (rightArrow > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var leftArrow = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '>') {
            leftArrow++;
        }
        if (leftArrow > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var question = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '?') {
            question++;
        }
        if (question > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var colun = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == ':') {
            colun++;
        }
        if (colun > 1) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }

    var backSlash = 0;
    length = val.length;
    for (var i = 0; i < length; i++) {
        if (val[i] == '\\') {
            backSlash++;
        }
        if (backSlash > 0) {
            val = val.substring(0, i) + "" + val.substring(i + 1);
        }
    }
    return val;
}

function getSpecialChar(strTitleOrClient) {

    if (strTitleOrClient.indexOf("&") > -1)
        strTitleOrClient = strTitleOrClient.replace(/&/g, "_undefindAmpersand_");
    if (strTitleOrClient.indexOf("*") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\*/g, "_undefindAsterisk_");
    if (strTitleOrClient.indexOf("^") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\^/g, "_undefindCaret_");
    if (strTitleOrClient.indexOf("<") > -1)
        strTitleOrClient = strTitleOrClient.replace(/</g, "_undefindLessthan_");
    if (strTitleOrClient.indexOf(">") > -1)
        strTitleOrClient = strTitleOrClient.replace(/>/g, "_undefindGreaterthan_");
    if (strTitleOrClient.indexOf("?") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\?/g, "_undefindQuestionmark_");
    if (strTitleOrClient.indexOf(".") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\./g, "_undefindFullstop_");
    if (strTitleOrClient.indexOf("/") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\//g, "_undefindSlash_");
    if (strTitleOrClient.indexOf(":") > -1)
        strTitleOrClient = strTitleOrClient.replace(/:/g, "_undefindColon_");
    if (strTitleOrClient.indexOf("\\") > -1)
        strTitleOrClient = strTitleOrClient.replace(/\\/, "_undefindBackslash_");
    else
        strTitleOrClient = strTitleOrClient.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, '');

    return strTitleOrClient;
}

function LogOff(on) {
    localStorage.clear();
    localStorage.setItem("signOn", on);
    if (on) {
        window.location = "index.html";
    } else {
        window.location = "index.html";
    }
}

function removeSpecialCharacters(control) {
    var val = $(control).val();
    $(control).val(val.replace(/[`~!@#$%^&*()_|+\-=?;:'",.<>\{\}\[\]\\\/]/gi, ''));
}