
$(function () {

    var scopeID = window.location.hash.substring(1);
    if (scopeID.charAt(0) === "a") {
        var appID = scopeID.substring(1);
        var svc = new authz.Service("admin/ApplicationScopes/" + appID);
        var vm = new Scope();
        ko.applyBindings(vm);
    }
    else {
        var svc = new authz.Service("admin/Scopes/" + scopeID);
        svc.get().then(function (data) {
            var vm = new Scope(data);
            ko.applyBindings(vm);
        });
    }

    function Scope(data) {
        var isNew = !data;
        data = data || {
            id:0,
            name: "",
            description: "",
            emphasize: false
        };
        ko.mapping.fromJS(data, null, this);

        var vm = this;
        vm.nameEnabled = ko.computed(function () {
            return isNew;
        });
        vm.editDescription = ko.computed(function () {
            return isNew ? "New" : "Manage";
        });
        vm.menusEnabled = ko.computed(function () {
            return !isNew;
        });

        authz.util.addRequired(this, "name", "Name");
        authz.util.addRequired(this, "description", "Description");
        authz.util.addAnyErrors(this);

        vm.save = function () {
            if (isNew) {
                svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
                    scopeID = data.id;
                    var url = window.location.toString();
                    url = url.substring(0, url.indexOf('#'));
                    url += "#" + scopeID;
                    window.location = url;

                    svc = new authz.Service("admin/Scopes/" + scopeID);
                    var vm = new Scope(data);
                    ko.applyBindings(vm);
                });
            }
            else {
                svc.put(ko.mapping.toJS(vm));
            }
        };
    }
});
