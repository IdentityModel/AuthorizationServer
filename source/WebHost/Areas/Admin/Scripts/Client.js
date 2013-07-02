/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/Clients");
    var secretSvc = new as.Service("admin/SymmetricKeys");

    function Client(data) {
        var vm = this;
        vm.isNew = ko.observable(!data);

        if (data && !data.clientSecret) {
            data.clientSecret = "";
        }
        data = data || {
            clientId: "",
            clientSecret:"",
            name: "",
            flow: "Code",
            allowRefreshToken: false,
            requireConsent: true,
            enabled:true
        };
        ko.mapping.fromJS(data, null, this);

        as.util.addRequired(this, "clientId", "Client ID");
        as.util.addValidation(this, "clientSecret", "Client Secret is required", ko.computed(function () {
            if (!vm.isNew()) return true;
            return !!vm.clientSecret();
        }));
        as.util.addRequired(this, "name", "Name");
        as.util.addAnyErrors(this);

        vm.allowRefreshTokenEnabled = ko.computed(function () {
            return vm.flow() === "Code" || vm.flow() === "ResourceOwner";
        });
        vm.requireConsentEnabled = ko.computed(function () {
            return vm.flow() === "Code" || vm.flow() === "Implicit";
        });
        vm.clientIdEnabled = ko.computed(function () {
            return vm.isNew();
        });
        vm.redirectsEnabled = ko.computed(function () {
            return !vm.isNew();
        });
        vm.editDescription = ko.computed(function () {
            return vm.isNew() ? "New" : "Manage";
        });
        vm.generateSecret = function () {
            secretSvc.get().done(function (data) {
                vm.clientSecret(data.value.replace(/[+/=]/gi, ""));
            });
        };
        vm.save = function () {
            if (vm.isNew()) {
                svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
                    window.location = window.location + '#' + vm.clientId();
                    vm.isNew(false);
                });
            }
            else {
                svc.put(ko.mapping.toJS(vm), vm.clientId());
            }
        };
    }

    if (window.location.hash) {
        var id = window.location.hash.substring(1);
        svc.get(id).then(function (data) {
            var vm = new Client(data);
            ko.applyBindings(vm);
        });
    }
    else {
        var vm = new Client();
        ko.applyBindings(vm);
    }
});
