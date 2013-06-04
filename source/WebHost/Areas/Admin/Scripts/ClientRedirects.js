
$(function () {
    var svc = new authz.Service("admin/ClientRedirects");

    function ClientRedirects(list) {
        this.uris = ko.mapping.fromJS(list);
    }
    ClientRedirects.prototype.deleteUri = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.uris.remove(item);
        });
    }

    var id = window.location.hash.substring(1);
    svc.get(id).then(function (data) {
        var vm = new ClientRedirects(data);
        ko.applyBindings(vm);
    });
});
