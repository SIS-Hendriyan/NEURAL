var APPURL = window.location.href;
var PathName = window.location.pathname;
var SplitAppurl = APPURL.split('/');
var SplitPathName = PathName.split('/');

var menu = getPathNameUrl();

function getBasicUrl() {
    var url = "";
    SplitAppurl.splice((SplitAppurl.length - 1), 1);
    for (var i in SplitAppurl) {
        if (url == "") {
            url = SplitAppurl[i];
        } else {
            url = url + "/" + SplitAppurl[i];
        }
    }
    //alert(url);
    return url;
};

function getPathNameUrl() {
    var url = "";
    SplitPathName.splice((SplitPathName.length - 1), 1);
    for (var i in SplitPathName) {
        //alert(SplitPathName[i]);
        //if (url == "") {
        //    url = SplitPathName[i];
        //} else {
        //    url = url + "/" + SplitPathName[i];
        //}
        url = SplitPathName[i];
    }
    //alert(url);
    return url;
};

var showNotification = function (title, text, status) {
    notification.show({
        title: title,
        message: text
    }, status);
    notification.setOptions({ autoHideAfter: 5000 });
}

var showAlert = function (title, text) {
    $("#dialog").kendoDialog({
        width: "500px",
        title: title,
        closable: true,
        modal: true,
        content: text
    }).data("kendoDialog").open();
    $(".k-window-titlebar").css('background-color', 'red');
    $(".k-window-titlebar").css('color', 'white');
}

function getMenuIsReadOnly() {
    var dataMenu = $.ajax({
        url: "../Menu/getMenuAccess",
        type: "GET",
        data: {
            menuName: menu,
            restrictionType: "Read Only"
        },
        success: function (result) {
            data = result.data;
            //console.log(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('error: ' + jqXHR.status);
        },
        statusCode: {

        }
    });
    return dataMenu;
}

function getMenuAccess() {
    var dataMenu = $.ajax({
        url: "../Menu/getMenuAccess",
        type: "GET",
        data: {
            menuName: menu,
            restrictionType: "No Access"
        },
        success: function (result) {
            data = result.data;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('error: ' + jqXHR.status);
        },
        statusCode: {

        }
    });
    return dataMenu;
}

function getControlRestriction() {
    var dataMenu = $.ajax({
        url: "../ControlRestriction/getDataList2",
        type: "GET",
        data: {
            control: menu
        },
        success: function (result) {
            data = result.data;
            //console.log(data);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('error: ' + jqXHR.status);
        },
        statusCode: {

        }
    });
    return dataMenu;
}

var fromPost = function (data, url, callback) {

    var result = true;
    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(data),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false
    }).done(function () {
        callback(result);
        //return result;
    });
    //console.log(result);
}

var displayLoading = function (target) {
    console.log("TargetLoadscreen", target);
    var element = $(target);

    kendo.ui.progress(element, true);
}
var hideLoading = function (target) {
    console.log("hideLoadscreen", target);
    var element = $(target);
    kendo.ui.progress(element, false);
}