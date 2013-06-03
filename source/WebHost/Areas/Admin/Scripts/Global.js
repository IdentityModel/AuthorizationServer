
$(function () {

    authz.services.global.get().then(function (data) {

        var container = document.getElementById("global");
        var global = new authz.models.Global(data);
        ko.applyBindings(global, container);

    });

});
