/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */
$(function () {
    var svc = new as.Service("UserApplications/UserApplications");
    svc.get().done(function (data) {
        var vm = new Apps(data);
        ko.applyBindings(vm);
    });

    function Apps(list) {
        this.applications = ko.mapping.fromJS(list);
    }
    Apps.prototype.delete = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.applications.remove(item);
        });
    }
    Apps.prototype.deleteAll = function () {
        var vm = this;
        svc.delete().then(function () {
            vm.applications.removeAll();
        });
    }
});