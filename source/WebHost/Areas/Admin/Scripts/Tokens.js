/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/Tokens");

    function Tokens(list) {
        this.tokens = ko.mapping.fromJS(list);
    }
    Tokens.prototype.deleteToken = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.tokens.remove(item);
        });
    }
    Tokens.prototype.deleteAll = function () {
        var vm = this;
        svc.delete().then(function () {
            vm.tokens.removeAll();
        });
    }

    svc.get().then(function (data) {
        var vm = new Tokens(data);
        ko.applyBindings(vm);
    });
});
