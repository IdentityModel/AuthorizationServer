/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/global");

    function Global(data) {
        ko.mapping.fromJS(data, null, this);

        as.util.addRequired(this, "name", "Name");
        as.util.addRequired(this, "issuer", "Issuer");
        as.util.addAnyErrors(this);
    }
    Global.prototype.save = function () {
        svc.put(ko.mapping.toJS(this));
    };

    svc.get().then(function (data) {
        var vm = new Global(data);
        ko.applyBindings(vm);
    });
});
