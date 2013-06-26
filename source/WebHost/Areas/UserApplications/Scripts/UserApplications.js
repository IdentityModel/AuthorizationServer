var userApps = (function () {
    "use strict";

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
        if (this.path.charAt(this.path.length-1) !== '/') {
            this.path += "/";
        }
        this.settings = settings || {};
    }
    Service.prototype.get = function (id) {
        id = id || "";
        return $.ajax({
            url: userApps.baseUrl + this.path + id,
            type: 'GET'
        }).fail(function (xhr, status, error) {
            showErrorMessage("Error Loading")
            return xhr;
        });
    };
    Service.prototype.put = function (data, id) {
        id = id || "";
        return $.ajax({
            url: userApps.baseUrl + this.path + id,
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
    Service.prototype.delete = function (id) {
        id = id || "";
        return $.ajax({
            url: userApps.baseUrl + this.path + id,
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
    Service.prototype.post = function (data, id) {
        id = id || "";
        return $.ajax({
            url: userApps.baseUrl + this.path + id,
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

    var module = {
        Service: Service,
        util: {
            prompt: function (message) {
                message = message || "Confirm Delete";
                $("#prompt .modal-header :first").text(message);

                var def = $.Deferred();
                $("#prompt").data("deferred", def).modal("show");
                return def.promise();
            },
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
                userApps.util.addValidation(vm, propName, displayName + " is required", ko.computed(function () {
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

    $("#prompt button.cancel").click(function () {
        $("#prompt").modal("hide");
        var def = $("#prompt").data("deferred");
        $("#prompt").removeData("deferred");
        setTimeout(function () {
            def.reject();
        }, 500);
    });

    $("#prompt button.save").click(function () {
        $("#prompt").modal("hide");
        var def = $("#prompt").data("deferred");
        $("#prompt").removeData("deferred");
        setTimeout(function () {
            def.resolve();
        }, 500);
    });

    document.body.addEventListener("click", function (e) {
        var $this = $(e.target);
        if ($this.hasClass("prompt") && !$this.data("in-prompt")) {
            $this.data("in-prompt", true);

            e.stopImmediatePropagation();

            module.util.prompt($this.data("prompt"))
                .done(function () {
                    $this.trigger("click");
                }).always(function () {
                    $this.removeData("in-prompt");
                });
        }
    }, true);

    return module;
})();


$(function () {
    var svc = new userApps.Service("UserApplications/UserApplications");
    svc.get().done(function (data) {
        var vm = new Apps(data);
        ko.applyBindings(vm);
    });

    function Apps(list) {
        this.applications = ko.mapping.fromJS(list);
    }
    Apps.prototype.delete = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.applications.remove(item);
        });
    }
    Apps.prototype.deleteAll = function () {
        var vm = this;
        svc.delete().then(function () {
            vm.applications.removeAll();
        });
    }
});