
$(function () {
    var svc = new authz.Service("admin/global");

    function Global(data) {
        ko.mapping.fromJS(data, null, this);

        authz.util.addRequired(this, "name", "Name");
        authz.util.addRequired(this, "issuer", "Issuer");
        authz.util.addAnyErrors(this);
    }
    Global.prototype.save = function () {
        svc.put(ko.mapping.toJS(this));
    };

    svc.get().then(function (data) {
        var vm = new Global(data);
        ko.applyBindings(vm);
    });
});
