
var authz = (function () {
    "use strict";

    var models = {
        Global: function (data) {
            ko.mapping.fromJS(data, null, this);

            var item = this;
            models.Global.prototype.nameInError = ko.computed(function () {
                return !item.name();
            });
            models.Global.prototype.nameError = ko.computed(function () {
                if (!item.name()) {
                    return "Name is required.";
                }
            });
            models.Global.prototype.issuerInError = ko.computed(function () {
                return !item.issuer();
            });
            models.Global.prototype.issuerError = ko.computed(function () {
                if (!item.issuer()) {
                    return "Issuer is required.";
                }
            });
            models.Global.prototype.anyErrors = ko.computed(function () {
                for (var prop in item) {
                    if (prop.indexOf("InError") >= 0) {
                        if (item[prop]()) {
                            return true;
                        }
                    }
                }
                return false;
            });
        }
    };
    models.Global.prototype.save = function () {
        authz.services.global.put(ko.mapping.toJS(this));
    };

    function showMessage(msg, css) {
        var elem = $("#message");
        if (elem.is(":visible")) {
            elem.clearQueue().fadeOut(function () {
                showMessage(msg, css);
            });
        }
        else {
            elem
                .addClass(css)
                .text(msg)
                .fadeIn()
                .delay(2000)
                .fadeOut(function () {
                    $(this).text("").removeClass(css);
                });
        }
    }
    function showSuccessMessage(msg) {
        showMessage(msg, "alert-success");
    }
    function showErrorMessage(msg) {
        showMessage(msg, "alert-error");
    }

    function Service(path, settings) {
        this.path = path;
        this.settings = settings || {};
    }
    Service.prototype.get = function () {
        var request =  $.ajax({
            url: authz.baseUrl + this.path,
            type: 'GET'
        });
        return request;
    };
    Service.prototype.put = function (data) {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).then(
        function(data, status, xhr){
            showSuccessMessage("Update Successful");
            return xhr;
        },
        function(xhr, status, error){
            showErrorMessage("Error Updating");
            return xhr;
        });
    };
    Service.prototype.delete = function () {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'DELETE'
        }).then(
        function(data, status, xhr){
            showSuccessMessage("Delete Successful");
            return xhr;
        },
        function(xhr, status, error){
            showErrorMessage("Error Deleting");
            return xhr;
        });
    };
    Service.prototype.post = function (data) {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).then(
        function (data, status, xhr) {
            showSuccessMessage("Create Successful");
            return xhr;
        },
        function (xhr, status, error) {
            showErrorMessage("Error Creating");
            return xhr;
        });
    };

    return {
        services: {
            global : new Service("admin/global")
        },
        models : models
    };
})();
