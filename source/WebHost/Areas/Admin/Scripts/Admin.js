
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
                        console.log("found", prop);
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
        console.log(this, JSON.stringify(this));
        authz.services.global.put(ko.mapping.toJS(this));
    };

    function Service(path) {
        this.path = path;
    }
    Service.prototype.get = function () {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'GET'
        });
    };
    Service.prototype.put = function (data) {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    };
    Service.prototype.delete = function () {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'DELETE'
        });
    };
    Service.prototype.post = function (data) {
        return $.ajax({
            url: authz.baseUrl + this.path,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    };

    return {
        services: {
            global : new Service("admin/global")
        },
        models : models
    };
})();
