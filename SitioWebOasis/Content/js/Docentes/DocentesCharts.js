$(document).ready(function () {
    //*******************************************
    /*	MINI BAR CHART
	/********************************************/

    if ($('.mini-bar-chart').length > 0) {
        var values = getRandomValues();
        var params = {  type: 'bar',
                        barWidth: 5,
                        height: 25
        }

        params.barColor = '#CE7B11';
        $('#mini-bar-chart1').sparkline(values[0], params);

        params.barColor = '#1D92AF';
        $('#mini-bar-chart2').sparkline(values[1], params);

        params.barColor = '#3F7577';
        $('#mini-bar-chart3').sparkline(values[2], params);
    }



    function getRandomValues() {
        // data setup
        var values = new Array(20);

        for (var i = 0; i < values.length; i++) {
            values[i] = [5 + randomVal(), 10 + randomVal(), 15 + randomVal(), 20 + randomVal(), 30 + randomVal(),
				35 + randomVal(), 40 + randomVal(), 45 + randomVal(), 50 + randomVal()]
        }

        return values;
    }


    function randomVal() {
        return Math.floor(Math.random() * 80);
    }


})