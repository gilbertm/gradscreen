(function ($) {

    function imageExists(image_url) {

        var http = new XMLHttpRequest();

        http.open('HEAD', image_url, false);
        http.send();

        return http.status != 404;

    }

    $(window).load(function () {

        /*
           * falling items
           */

        /**
        * Instant Snowstorm, rev 2
        * (c) Copyright 2012 DWUser.com.  All rights reserved.
        * by Nathan Rohler
        * Released under MIT License
        */

        // -------------------------------
        // --- Configuration variables ---
        // -------------------------------

        // How many frames per second?  More => smoother but more processor intensive
        var fps = 30;

        // How often should flakes be added?  Every 10 frames by default.  Greater => fewer flakes
        var addNewFlakesEveryNFrames = 30;

        // How many flakes should be added each time?  Greater => more flakes, more processor intensive
        var newFlakesToAdd = 3;

        // Controls the speed; 0.7 provides a nice speed
        var speedControl = 0.7;

        // -------------------------------
        // -------------------------------
        // -------------------------------

        // Holder variables
        var flakes = [];
        var additionCounter = 0;

        // The flake creator function
        function createFlake(curX, curY) {

            // How unique should each flake be?  These values specify max unique speed and wiggle/drift
            var maxSpeedOffset = 15;
            var maxWiggleOffset = 25;

            // How big should the flakes be?
            var minSize = 10;
            var maxSize = 25;

            // How much drifting/wiggling should be allowed in the downward path?
            var minWiggle = 10;
            var maxWiggle = 40;

            var sizePercent = Math.random();
            var size = Math.floor(sizePercent * (maxSize - minSize) + minSize);
            var opacity = 0.3 + Math.random() * 0.7;

            var color = "rgba(" + Math.floor((Math.random() * 255)) + ", " + Math.floor((Math.random() * 255)) + ", " + Math.floor((Math.random() * 255)) + ", 0.7)";

            // 'none';
            // '#9CF';

            // Create a unique speed offset, so each flake falls at a unique rate
            var speedOffset = Math.floor(Math.random() * maxSpeedOffset);

            // Create a unique wiggle amount based on size (bigger = more wiggle/drift)
            var wiggle = minWiggle + (maxWiggleOffset * Math.random()) + (maxWiggle - minWiggle) * sizePercent;

            var images = [];

            if (document.getElementById("date")) {
                images.push(document.getElementById("date").innerHTML);
            }

            if (document.getElementById("maple")) {
                images.push(document.getElementById("maple").innerHTML);
            }

            var starImage = new Image(size, size);
            starImage.src = images[Math.floor((Math.random() * images.length))];
            // starImage.style = "transform : rotate(" + Math.floor((Math.random() * 10)) + "deg);";
            var imageRotation = "rotate(" + Math.floor((Math.random() * 180)) + "deg)";

            var flake = $('<div>').text(' ').html(starImage).css({
                position: 'fixed',
                left: curX,
                top: curY,
                zIndex: 5000,
                width: size,
                height: size,
                opacity: opacity,
                transform: imageRotation,
                // '-webkit-transform': 'rotate(' + Math.floor((Math.random() * 10)) + 'deg)',
                // borderRadius: size / 2,
                // '-moz-border-radius': size / 2,
                // '-webkit-border-radius': size / 2,
                // backgroundColor: color
            }).appendTo('.main-wrapper');

            var flakeObj = {
                size: size,
                sizePercent: sizePercent,
                homeX: curX,
                curX: curX,
                curY: curY,
                flake: flake,
                uniqueSpeedOffset: speedOffset,
                wiggle: wiggle,
                wiggleCounter: Math.floor(100 * Math.random())
            };
            flakes.push(flakeObj);

            return flakeObj;
        }

        // Create the sin table to save processing power later
        var sinTable = [];
        for (var i = 0; i < 100; i++) {
            var sin = Math.sin((i / 100) * Math.PI * 2);
            sinTable.push(sin);
        }

        // Track where the mouse is, so we can move flakes away from it
        var mouseX = -200;
        var mouseY = -200;
        var $w = $(window);
        $(document).mousemove(function (e) {
            mouseX = e.pageX - $w.scrollLeft();
            mouseY = e.pageY - $w.scrollTop();
        });

        function onEnterFrame() {
            // Update existing flakes
            var winH = $w.height();
            for (var i = flakes.length - 1; i > -1; i--) {
                var flakeObj = flakes[i];
                var flake = flakeObj.flake;
                var speed = 2 + (1 - flakeObj.sizePercent) * 5 + flakeObj.uniqueSpeedOffset; // bigger = slower to fall
                speed *= speedControl; // apply the speed control
                var curY = flakeObj.curY;
                var newY = curY + speed;

                var wiggleCounter = flakeObj.wiggleCounter = (++flakeObj.wiggleCounter % 100);
                var sin = sinTable[wiggleCounter];
                var wiggle = flakeObj.wiggle * sin;
                var newX = flakeObj.homeX + wiggle;

                // If we're close to the mouse, force out of the way
                var mouseXDist = Math.abs(mouseX - newX);
                var mouseYDist = Math.abs(mouseY - newY);
                var influenceArea = 150;
                if (mouseXDist < influenceArea && mouseYDist < influenceArea) {
                    var maxForce = 10;
                    var dist = Math.sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
                    if (dist < influenceArea) {
                        var influence = maxForce * (1 - (dist / influenceArea));
                        if (mouseY > newY) {
                            newY -= influence;
                            if (mouseX < newX) flakeObj.homeX += influence;
                            else flakeObj.homeX -= influence;
                        }
                        else newY += influence;
                    }
                }

                flakeObj.curY = newY;
                flakeObj.curX = newX;
                flake.css({
                    top: newY,
                    left: newX
                });

                // Destroy flake if it has gone out of view by 100
                if (newY > winH + 100) {
                    flake.remove();
                    flakeObj.flake = null;
                    flakes.splice(i, 1);
                }
            }

            // Add any new flakes
            if (++additionCounter % addNewFlakesEveryNFrames == 0) {
                additionCounter = 0;

                var minX = -100;
                var maxX = $(window).width() + 100;
                var homeY = -100;
                for (var i = 0; i < newFlakesToAdd; i++) {
                    var curX = minX + Math.floor(Math.random() * (maxX - minX));
                    var curY = homeY + Math.floor(100 * Math.random());
                    createFlake(curX, curY);
                }
            }
        }

        // Start the animation based on the requested frames per second
        setInterval(onEnterFrame, 1500 / fps);
    });


})(jQuery);