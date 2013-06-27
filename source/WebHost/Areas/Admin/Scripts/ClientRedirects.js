/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new authz.Service("admin/ClientRedirects");

    var clientId = window.location.hash.substring(1);
    svc.get(clientId).then(function (data) {
        var vm = new ClientRedirects(data);
        ko.applyBindings(vm);
    });

    function ClientRedirects(list) {
        this.uris = ko.mapping.fromJS(list);
        this.newUri = {
            uri : ko.observable(""),
            description : ko.observable("")
        };
        this.backId = ko.computed(function () {
            return clientId;
        });
        authz.util.addRequired(this.newUri, "uri", "Uri");
        authz.util.addAnyErrors(this.newUri);
    }
    ClientRedirects.prototype.addUri = function () {
        var vm = this;
        svc.post(ko.mapping.toJS(vm.newUri), clientId).then(function (data) {
            vm.uris.push(ko.mapping.fromJS(data));
            vm.newUri.uri("");
            vm.newUri.description("");
        });
    }
    ClientRedirects.prototype.deleteUri = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.uris.remove(item);
        });
    }
});
