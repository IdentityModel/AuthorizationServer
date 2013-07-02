/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/Clients");

    function Clients(list) {
        this.clients = ko.mapping.fromJS(list);
    }
    Clients.prototype.deleteClient = function (item) {
        var vm = this;
        svc.delete(item.clientId()).then(function () {
            vm.clients.remove(item);
        });
    }

    svc.get().then(function (data) {
        var vm = new Clients(data);
        ko.applyBindings(vm);
    });
});
