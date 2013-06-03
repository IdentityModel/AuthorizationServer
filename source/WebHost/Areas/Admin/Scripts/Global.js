
$(function () {
    var svc = new authz.Service("admin/global");

    function Global(data) {
        ko.mapping.fromJS(data, null, this);

        var item = this;
        item.nameInError = ko.computed(function () {
            return !item.name();
        });
        item.nameError = ko.computed(function () {
            if (!item.name()) {
                return "Name is required.";
            }
        });
        item.issuerInError = ko.computed(function () {
            return !item.issuer();
        });
        item.issuerError = ko.computed(function () {
            if (!item.issuer()) {
                return "Issuer is required.";
            }
        });
        item.anyErrors = ko.computed(function () {
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
    Global.prototype.save = function () {
        svc.put(ko.mapping.toJS(this));
    };

    svc.get().then(function (data) {
        var vm = new Global(data);
        ko.applyBindings(vm);
    });
});
