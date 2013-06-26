
$(function () {
    var svc = new authz.Service("admin/X509Keys");

    function X509Key(data) {
        var vm = this;
        vm.isNew = ko.observable(!data);
        data = data || {
            id : 0,
            name: "",
            findType: 0,
            value: "",
            thumbprint:""
        };
        ko.mapping.fromJS(data, null, vm);

        authz.util.addRequired(this, "name", "Name");
        authz.util.addRequired(this, "value", "Find Value");
        authz.util.addAnyErrors(this);

        vm.editDescription = ko.computed(function () {
            return vm.isNew() ? "New" : "Manage";
        });

        vm.certUrl = ko.computed(function () {
            return authz.baseUrl + "admin/Certificate/" + vm.id();
        });

        vm.save = function () {
            if (vm.isNew()) {
                svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
                    window.location = window.location + '#' + data.id;
                    vm.isNew(false);
                    vm.id(data.id);
                    vm.thumbprint(data.thumbprint);
                });
            }
            else {
                svc.put(ko.mapping.toJS(vm), vm.id());
            }
        };
    }

    if (window.location.hash) {
        var id = window.location.hash.substring(1);
        svc.get(id).then(function (data) {
            var vm = new X509Key(data);
            ko.applyBindings(vm);
        });
    }
    else {
        var vm = new X509Key();
        ko.applyBindings(vm);
    }
});
