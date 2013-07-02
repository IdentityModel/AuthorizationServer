/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var hash = window.location.hash.substring(1);
    var scopeID = parseInt(hash);
    var idx = hash.indexOf("a");
    var appID = hash.substring(idx + 1);

    if (scopeID) {
        var svc = new as.Service("admin/Scopes/" + scopeID);
        svc.get().then(function (data) {
            ko.applyBindings(new Scope(data), document.getElementById("scopes"));
        });

        var scopeClientSvc = new as.Service("admin/ScopeClients/" + scopeID);
        scopeClientSvc.get().then(function (data) {
            ko.applyBindings(new ScopeClients(data), document.getElementById("scopeClients"));
        });
    }
    else {
        var svc = new as.Service("admin/ApplicationScopes/" + appID);
        ko.applyBindings(new Scope(), document.getElementById("scopes"));
    }

    function Scope(data) {
        var vm = this;
        vm.isNew = ko.observable(!data);

        data = data || {
            id:0,
            name: "",
            displayName:"",
            description: "",
            emphasize: false
        };
        ko.mapping.fromJS(data, null, this);

        vm.appID = ko.computed(function () {
            return appID;
        });
        vm.nameEnabled = ko.computed(function () {
            return vm.isNew();
        });
        vm.editDescription = ko.computed(function () {
            return vm.isNew() ? "New" : "Manage";
        });
        vm.menusEnabled = ko.computed(function () {
            return !vm.isNew();
        });

        as.util.addRequired(this, "name", "Name");
        as.util.addRequired(this, "displayName", "Display Name");
        as.util.addAnyErrors(this);

        vm.save = function () {
            if (vm.isNew()) {
                svc.post(ko.mapping.toJS(vm)).then(function (data, status, xhr) {
                    scopeID = data.id;
                    var url = window.location.toString();
                    url = url.substring(0, url.indexOf('#'));
                    url += "#" + scopeID;
                    window.location = url;

                    vm.id(data.id);
                    vm.isNew(false);

                    svc = new as.Service("admin/Scopes/" + scopeID);
                    scopeClientSvc = new as.Service("admin/ScopeClients/" + scopeID);
                    scopeClientSvc.get().then(function (data) {
                        ko.applyBindings(new ScopeClients(data), document.getElementById("scopeClients"));
                    });
                });
            }
            else {
                svc.put(ko.mapping.toJS(vm));
            }
        };
    }

    function ScopeClients(list) {
        ko.mapping.fromJS(list, null, this);
        this.appID = ko.computed(function () {
            return appID;
        });
        this.scopeID = ko.computed(function () {
            return scopeID;
        });
    }

    ScopeClients.prototype.addClient = function (item) {
        var vm = this;
        scopeClientSvc.put(null, item.clientId()).then(function () {
            vm.otherClients.remove(item);
            vm.allowedClients.push(item);
        });
    };

    ScopeClients.prototype.deleteClient = function (item) {
        var vm = this;
        scopeClientSvc.delete(item.clientId()).then(function () {
            vm.allowedClients.remove(item);
            vm.otherClients.push(item);
        });
    }
});
