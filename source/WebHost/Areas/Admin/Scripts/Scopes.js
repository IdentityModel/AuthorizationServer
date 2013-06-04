
$(function () {
    var svc = new authz.Service("admin/Scopes");

    var appID = window.location.hash.substring(1);
    svc.get(appID).then(function (data) {
        var vm = new Scopes(data);
        ko.applyBindings(vm);
    });
    
    function Scopes(list) {
        this.scopes = ko.mapping.fromJS(list);
        this.newScope = ko.mapping.fromJS({
            name:"", description:"", emphasize:false
        });
        authz.util.addRequired(this.newScope, "name", "Name");
        authz.util.addRequired(this.newScope, "description", "Description");
        authz.util.addAnyErrors(this.newScope);
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
        svc.delete(item.id()).then(function () {
            vm.scopes.remove(item);
        });
    }
});
