﻿/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/Keys");

    function Keys(list) {
        this.keys = ko.mapping.fromJS(list);
    }
    Keys.prototype.deleteKey = function (item) {
        var vm = this;
        svc.delete(item.iD()).then(function () {
            vm.keys.remove(item);
        });
    }

    svc.get().then(function (data) {
        var vm = new Keys(data);
        ko.applyBindings(vm);
    });
});
