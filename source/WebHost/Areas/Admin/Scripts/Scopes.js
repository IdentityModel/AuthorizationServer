/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */


$(function () {
    var svc = new as.Service("admin/ApplicationScopes");
    var scopesSvc = new as.Service("admin/Scopes");

    var appID = window.location.hash.substring(1);
    svc.get(appID).then(function (data) {
        var vm = new Scopes(data);
        ko.applyBindings(vm);
    });
    
    function Scopes(list) {
        this.appID = ko.observable(appID);
        this.scopes = ko.mapping.fromJS(list);
        this.newScope = ko.mapping.fromJS({
            name:"", description:"", emphasize:false
        });
        as.util.addRequired(this.newScope, "name", "Name");
        as.util.addRequired(this.newScope, "description", "Description");
        as.util.addAnyErrors(this.newScope);
    }
    Scopes.prototype.addScope = function () {
        var vm = this;
        svc.post(ko.mapping.toJS(vm.newScope), appID).then(function (data) {
            vm.scopes.push(ko.mapping.fromJS(data));
            vm.newScope.name("");
            vm.newScope.description("");
            vm.newScope.emphasize(false);
        });
    };

    Scopes.prototype.deleteScope = function (item) {
        var vm = this;
        scopesSvc.delete(item.id()).then(function () {
            vm.scopes.remove(item);
        });
    }
});
