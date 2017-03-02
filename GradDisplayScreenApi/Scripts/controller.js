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
        .controller('controller', controller);

    controller.$inject = ['$scope', '$interval', '$http', '$timeout', 'factory'];

    function controller($scope, $interval, $http, $timeout, factory) {

        $scope.title = 'controller';

        $scope.Graduate = null;

        factory.Graduate().success(function (data) {

            $scope.Graduate = data;


        });


        $interval(function () {
            $http({
                url: '/api/graduate',
                method: 'GET',
                isArray: true,
                // params: { voiceperson: params.voiceperson, voiceextra: params.voiceextra }
                params: {}
            }).success(function (Graduate) {

                if (Graduate != null) {

                    if ($scope.Graduate != null) {

                        if ($scope.Graduate.GraduateId != Graduate.GraduateId) {
                            $scope.Graduate = Graduate;
                        }

                    } else {
                        $scope.Graduate = Graduate;
                    }

                } else {
                    $scope.Graduate = null;
                }

            });
        }, 500);


        $timeout(function () {

            function textfill(el, options) {
                var fontSize = options.maxFontPixels;
                var ourText = $('#arabicFullname');
                var maxHeight = el.height();
                var maxWidth = el.width();
                var textHeight;
                var textWidth;

                do {
                    console.log("fontsize: " + fontSize);
                    console.log("maxheight: " + maxHeight);
                    console.log("maxweigth: " + maxWidth);

                    ourText.css('font-size', fontSize);
                    textHeight = ourText.height();
                    textWidth = ourText.width();

                    console.log("textHeight: " + textHeight);
                    console.log("textWidth: " + textWidth);

                    console.log("=======================================================");
                    fontSize = fontSize - 1;


                } while ((textHeight > maxHeight || textWidth > maxWidth) && fontSize > 3);
            }

            textfill($('#fullname'), { maxFontPixels: 200 });

        }, 500, false);
    }

})();