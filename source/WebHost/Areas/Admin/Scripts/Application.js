/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/Applications");

    function Application(keys, data) {
        var vm = this;
        vm.isNew = ko.observable(!data);
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
            signingKeyId: null,
            enabled:true
        };
        ko.mapping.fromJS(data, null, this);
        this.signingKeys = ko.mapping.fromJS(keys);

        as.util.addRequired(this, "name", "Name");
        as.util.addRequired(this, "namespace", "Namespace");
        as.util.addRequired(this, "audience", "Audience");
        as.util.addRequired(this, "signingKeyId", "Signing Key");
        as.util.addAnyErrors(this);

        vm.menusEnabled = ko.computed(function () {
            return !vm.isNew();
        });

        vm.editDescription = ko.computed(function () {
            return vm.isNew() ? "New" : "Manage";
        });

        vm.rememberConsentDecisionEnabled = ko.computed(function () {
            return vm.requireConsent();
        });

        vm.save = function () {
            var data = ko.mapping.toJS(vm);
            if (!data.requireConsent) {
                data.rememberConsentDecision = false;
            }
            if (vm.isNew()) {
                svc.post(data).then(function (data, status, xhr) {
                    window.location = window.location + '#' + data.id;
                    vm.isNew(false);
                    vm.id(data.id);
                });
            }
            else {
                svc.put(data, vm.id());
            }
        };
    }

    var keysSvc = new as.Service("admin/Keys");
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
