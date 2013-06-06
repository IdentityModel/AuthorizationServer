
$(function () {
    var svc = new authz.Service("admin/Tokens");

    function Tokens(list) {
        this.tokens = ko.mapping.fromJS(list);
    }
    Tokens.prototype.deleteToken = function (item) {
        var vm = this;
        svc.delete(item.id()).then(function () {
            vm.tokens.remove(item);
        });
    }

    svc.get().then(function (data) {
        var vm = new Tokens(data);
        ko.applyBindings(vm);
    });
});
