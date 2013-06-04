
$(function () {
    var svc = new authz.Service("admin/Clients");

    function NewClient() {
        ko.mapping.fromJS({
            clientID: "",
            clientSecret: "",
            name: "",
            flow: "Code",
            allowRefreshTokens: false,
            requireConsent:true
        }, null, this);

        authz.util.addRequired(this, "clientID", "Client ID");
        authz.util.addRequired(this, "clientSecret", "Client Secret");
        authz.util.addRequired(this, "name", "Name");
        authz.util.addAnyErrors(this);

        var vm = this;
        vm.flow.subscribe(function (val) {
            vm.allowRefreshTokens(val === "Code" || val === "ResourceOwner");
        });
        vm.allowRefreshTokensEnabled = ko.computed(function () {
            return vm.flow() === "Code" || vm.flow() === "ResourceOwner";
        });
    }

    NewClient.prototype.save = function () {
        var vm = this;
        svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
            var url = xhr.getResponseHeader("Location");
            //window.location = url;
        });
    }

    var vm = new NewClient();
    ko.applyBindings(vm);
});
