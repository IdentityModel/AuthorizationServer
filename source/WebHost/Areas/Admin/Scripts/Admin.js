var authz = (function () {
    "use strict";

    function showMessage(msg, css, detail) {
        var elem = $("#message");
        if (elem.is(":visible")) {
            elem.clearQueue().delay(1000).fadeOut(function () {
                showMessage(msg, css, detail);
            });
        }
        else {
            if (detail) {
                msg += "<br><br>" + detail;
            }
            elem
                .addClass(css)
                .html(msg)
                .fadeIn()
                .delay(3000)
                .fadeOut(function () {
                    $(this).html("").removeClass(css);
                });
        }
    }
    function showSuccessMessage(msg) {
        showMessage(msg, "alert-success");
    }
    function showErrorMessage(msg, detail) {
        showMessage(msg, "alert-error", detail);
    }

    function getErrorDetail(xhr) {
        if (xhr.responseJSON && xhr.responseJSON.error) {
            return xhr.responseJSON.error;
        }
    }

    function Service(path, settings) {
        this.path = path;
        if (this.path.charAt(this.path.length-1) !== '/') {
            this.path += "/";
        }
        this.settings = settings || {};
    }
    Service.prototype.get = function (id) {
        id = id || "";
        return $.ajax({
            url: authz.baseUrl + this.path + id,
            type: 'GET'
        }).fail(function (xhr, status, error) {
            showErrorMessage("Error Loading", getErrorDetail(xhr))
            return xhr;
        });
    };
    Service.prototype.put = function (data, id) {
        id = id || "";
        return $.ajax({
            url: authz.baseUrl + this.path + id,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).then(
        function(data, status, xhr){
            showSuccessMessage("Update Successful");
            return xhr;
        },
        function(xhr, status, error){
            showErrorMessage("Error Updating", getErrorDetail(xhr));
            return xhr;
        });
    };
    Service.prototype.delete = function (id) {
        id = id || "";
        return $.ajax({
            url: authz.baseUrl + this.path + id,
            type: 'DELETE'
        }).then(
        function(data, status, xhr){
            showSuccessMessage("Delete Successful");
            return xhr;
        },
        function(xhr, status, error){
            showErrorMessage("Error Deleting", getErrorDetail(xhr));
            return xhr;
        });
    };
    Service.prototype.post = function (data, id) {
        id = id || "";
        return $.ajax({
            url: authz.baseUrl + this.path + id,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        }).then(
        function (data, status, xhr) {
            showSuccessMessage("Create Successful");
            return xhr;
        },
        function (xhr, status, error) {
            showErrorMessage("Error Creating", getErrorDetail(xhr));
            return xhr;
        });
    };

    var module = {
        Service: Service,
        util: {
            addValidation: function (vm, propName, message, valFunc) {
                var dirty = ko.observable(false);
                vm[propName].subscribe(function () {
                    dirty(true);
                });
                vm[propName + "InError"] = ko.computed(function () {
                    return !valFunc();
                });
                vm[propName + "Error"] = ko.computed(function () {
                    if (dirty() && !valFunc()) {
                        return message;
                    }
                });
            },
            addRequired: function (vm, propName, displayName) {
                authz.util.addValidation(vm, propName, displayName + " is required", ko.computed(function() {
                    return !!vm[propName]();
                }));
            },
            addAnyErrors: function (vm) {
                vm.anyErrors = ko.computed(function () {
                    for (var prop in vm) {
                        if (prop.indexOf("InError") >= 0) {
                            if (vm[prop]()) {
                                return true;
                            }
                        }
                    }
                    return false;
                });
            }
        }
    };

    return module;
})();
