(function () {
    'use strict';

    function getSearchParameters() {
        var prmstr = window.location.search.substr(1);
        return prmstr != null && prmstr != "" ? transformToAssocArray(prmstr) : {};
    }

    function transformToAssocArray(prmstr) {
        var params = {};
        var prmarr = prmstr.split("&");
        for (var i = 0; i < prmarr.length; i++) {
            var tmparr = prmarr[i].split("=");
            params[tmparr[0]] = tmparr[1];
        }
        return params;
    }

    var params = getSearchParameters();


    angular
        .module('app')
        .factory('factory', factory);

    factory.$inject = ['$http', '$location'];

    function factory($http, $location) {

        var service = {
            Graduate: function(){

                return $http({
                    url: '/api/graduate',
                    method: 'GET',
                    isArray: true,
                    // params: { voiceperson: params.voiceperson, voiceextra: params.voiceextra }
                    params: { }
                });
            }
        };

        return service;


    }
})();