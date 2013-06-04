
$(function () {
    var svc = new authz.Service("admin/Scopes");

    function Scopes(list) {
        this.scopes = ko.mapping.fromJS(list);
        //var vm = this;
    }

    Scopes.prototype.deleteScope = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.scopes.remove(item);
        });
    }

    var id = window.location.hash.substring(1);
    svc.get(id).then(function (data) {
        var vm = new Scopes(data);
        ko.applyBindings(vm);
    });
});
