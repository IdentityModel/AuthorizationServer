
$(function () {
    var svc = new authz.Service("admin/Applications");

    function Application(keys, data) {
        var isNew = !data;
        data = data || {
            id:0,
            name: "",
            description: "",
            logoUrl: "",
            namespace: "",
            audience: "",
            tokenLifetime: 0,
            allowRefreshToken: false,
            requireConsent: false,
            rememberConsentDecision: false,
            signingKeyId: null
        };
        ko.mapping.fromJS(data, null, this);
        this.signingKeys = ko.mapping.fromJS(keys);

        var vm = this;

        authz.util.addRequired(this, "name", "Name");
        authz.util.addRequired(this, "namespace", "Namespace");
        authz.util.addRequired(this, "audience", "Audience");
        authz.util.addRequired(this, "signingKeyId", "Signing Key");
        authz.util.addAnyErrors(this);

        vm.menusEnabled = ko.computed(function () {
            return !isNew;
        });

        vm.editDescription = ko.computed(function () {
            return isNew ? "New" : "Manage";
        });

        vm.save = function () {
            if (isNew) {
                svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
                    window.location = window.location + '#' + data.id;
                });
            }
            else {
                svc.put(ko.mapping.toJS(vm), vm.id());
            }
        };
    }

    var keysSvc = new authz.Service("admin/Keys");
    var kd = keysSvc.get();
    var ad = null;

    if (window.location.hash) {
        var id = window.location.hash.substring(1);
        ad = svc.get(id);
    }

    $.when(kd, ad).then(function (keyData, appData) {
        var vm = new Application(keyData[0], appData && appData[0]);
        ko.applyBindings(vm);
    });
});
