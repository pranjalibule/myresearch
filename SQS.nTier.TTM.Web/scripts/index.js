
$(document).ready(function () {
    $("#divProcessing").hide();
});

function ActivationMail() {
    $("#modal-wait").dialog("close");

    showLoader();

    var url = urlPrefix + "Login/SendActivationMail";
    var method = "POST";

    var txtUsername = $("#username").val();
    var txtPassword = $("#password").val();
    var adUser = true;

    var objLoginJSON = JSON.stringify({ userName: txtUsername, password: txtPassword, adUser: adUser });

    $.ajax({
        url: url,
        type: method,
        beforeSend: function (request) {
            request.setRequestHeader("User", userId);
        },
        contentType: 'application/json; charset=UTF-8',
        data: objLoginJSON,
        success: function (response) {
            hideLoader();
            location.reload();
        },
        error: function (jqXHR, textStatus, errorThrown) {            
            hideLoader();
            showMessageBox(textStatus + "--" + errorThrown, "red");
        }
    });
}

function DoLogin(event) {
    if (event.keyCode === 13) {
        Login();
        return true;
    }
}

function singleSingON() {
    showLoader();
    $.ajax({
        url: urlPrefix + "Login/singleSingON",
        type: "POST",
        beforeSend: function (request) {
            request.setRequestHeader("User", userId);
        },
        contentType: 'application/json; charset=UTF-8',
        success: function (response) {
            hideLoader();
            if (response === null) {
                showMessageBox("Invalid Login.", "red");
                $('#password').val('');
                return false;
            }
            else {
                if (response.Name !== null && response.Name.indexOf("Error") >= 0) {
                    showMessageBox(response.Name, "red");
                    $('#password').val('');
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("PendingActivation") >= 0) {
                    var message1 = response.Name;
                    //  var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    var message1 = response.Name.replace("PendingActivation: ", "");
                    //   var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";
                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage = message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("AddFromLogin") >= 0) {
                    var message1 = response.Name;
                    //var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    var message1 = response.Name.replace("AddFromLogin: ", "");
                    // var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage = message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else {
                    localStorage.setItem("user", encodeURI(response.Name));
                    window.location = "Home.html";
                    return true;
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoader();
            $("#divProcessing").hide();
            showMessageBox(textStatus + "--" + errorThrown, "red");
        }
    });
}

function displayLoggedIn() {
    showLoader();
    $.ajax({
        url: urlPrefix + "Login/singleSingON",
        type: "POST",
        beforeSend: function (request) {
            request.setRequestHeader("User", userId);
        },
        contentType: 'application/json; charset=UTF-8',
        success: function (response) {
            hideLoader();
            if (response === null) {
                showMessageBox("Invalid Login.", "red");
                $('#password').val('');
                return false;
            }
            else {
                if (response.Name !== null && response.Name.indexOf("Error") >= 0) {
                    showMessageBox(response.Name, "red");
                    $('#password').val('');
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("PendingActivation") >= 0) {
                    var message1 = response.Name;
                    //  var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    var message1 = response.Name.replace("PendingActivation: ", "");
                    //   var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";
                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage = message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("AddFromLogin") >= 0) {
                    var message1 = response.Name;
                    //var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    var message1 = response.Name.replace("AddFromLogin: ", "");
                    // var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage = message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else {
                    $("#SSON").text("Login using windows ( " + response.Other + " )");
                    return true;
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoader();
            $("#divProcessing").hide();
            showMessageBox(textStatus + "--" + errorThrown, "red");
        }
    });
}

function Login() {
    showLoader();   

    $("#txtOutput").html("");
    var txtUsername = $("#username").val();
    var txtPassword = $("#password").val();

    if (txtUsername === "") {
        hideLoader();
        showMessageBox("Enter username.", "red");
        //$("#username").focus();
        return false;
    }
    else if (txtPassword === "") {
        hideLoader();
        showMessageBox("Enter password.", "red");
        //$("#password").focus();
        return false;
    }


    if (txtUsername != "admin") {
        var valide = validation(txtUsername, "length");
        if (!valide) {
            hideLoader();
            showMessageBox("Username should not be greater than 30 characters.", "red");
            //$("#username").focus();
            return false;
        }
    }

    if (txtPassword != "admin") {
        var valide = validation(txtPassword, "length");
        if (!valide) {
            hideLoader();
            showMessageBox("Password should not be greater than 30 characters.", "red");
            //$("#password").focus();
            return false;
        }
    }
    

    var url = urlPrefix + "Login/ValidateLogin";

    var method = "POST";

    var adUser = true;

    if (txtUsername.indexOf("@") > 0) {
        adUser = false;
    }
    $("#divProcessing").show();
    var objLoginJSON = JSON.stringify({ userName: txtUsername, password: txtPassword, adUser: adUser });

    $.ajax({
        url: url,
        type: method,
        beforeSend: function (request) {
            request.setRequestHeader("User", userId);          
        },
        contentType: 'application/json; charset=UTF-8',
        data: objLoginJSON,
        success: function (response) {
            hideLoader();           
            if (response === null) {
                showMessageBox("Invalid Login.", "red");
                $('#password').val('');
                return false;
            }
            else {
                if (response.Name !== null && response.Name.indexOf("Error") >= 0) {
                    showMessageBox(response.Name, "red");                  
                    $('#password').val('');
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("PendingActivation") >= 0) {
                    var message1 = response.Name;
                  //  var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    var message1 = response.Name.replace("PendingActivation: ", "");
                 //   var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";
                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage =message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else if (response.Name !== null && response.Name.indexOf("AddFromLogin") >= 0) {
                    var message1 = response.Name;
                    //var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";
                   
                    var message1 = response.Name.replace("AddFromLogin: ", "");
                   // var message2 = "User is registered in TTM portal but not activated. Please click on <a onclick='ActivationMail();' href ='#''>Activation Link</a> to get TTM portal access.";
                    var message2 = "Invalid User, Please contact TTM support <a onclick='ActivationMail();' href ='#''>(ttm-support@sqs.com)</a>for access request";

                    //var fullmessage = message1 + "<br/>" + message2;
                    var fullmessage = message2;
                    showMessageBox(fullmessage, "green");
                    return false;
                }
                else {
                    localStorage.setItem("user", encodeURI(response.Name));
                    window.location ="Home.html";                    
                    return true;
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoader();
            $("#divProcessing").hide();
            showMessageBox(textStatus + "--" + errorThrown, "red");
        }
    });
}

function RequestAccess() {
    showLoader();

    $("#txtOutput").html("");
    var txtUsername = $("#username").val();
    var txtPassword = $("#password").val();

    if (txtUsername === "") {
        hideLoader();
        showMessageBox("Enter username.", "red");    
        return false;
    }
    else if (txtPassword === "") {
        hideLoader();
        showMessageBox("Enter password.", "red");       
        return false;
    }

    var url = urlPrefix + "Login/RequestAccess";

    var method = "POST";

    var adUser = true;

    if (txtUsername.indexOf("@") > 0) {
        adUser = false;
    }

    var objLoginJSON = JSON.stringify({ userName: txtUsername, password: txtPassword, adUser: adUser });

    $.ajax({
        url: url,
        type: method,
        contentType: 'application/json; charset=UTF-8',
        data: objLoginJSON,
        success: function (response) {
            hideLoader();
            if (response === null) {
                showMessageBox("Invalid Login.", "red");
                return false;
            }
            else {
                if (response.Name !== null && response.Name.indexOf("Error") >= 0) {
                    showMessageBox(response.Name, "red");
                    return false;
                }
                else {
                    showMessageBox("Please check email regarding access request.", "green", "Index.html");
                    return true;
                }
            }
           
        },
        error: function (jqXHR, textStatus, errorThrown) {
            hideLoader();
            $("#divProcessing").hide();
            showMessageBox(textStatus + "--" + errorThrown, "red");
        }
    });
}