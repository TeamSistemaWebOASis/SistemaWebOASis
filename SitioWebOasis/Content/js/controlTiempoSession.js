$(document).ready(function () {
    time = ( $('#sessionTime').val() != undefined ) ? $('#sessionTime').val() 
                                                    : 20;

    sessionTime = parseInt(time) * 60;    
    startTimer(sessionTime);
    idleTime = 0;
    var timer = sessionTime, minutes, seconds;

    function startTimer(sessionTime){
        setInterval(function () {
            //  Incrementa en un milisegundo el contador 
            idleTime = idleTime + 1000;
            minutes = parseInt(timer / 60, 10)
            seconds = parseInt(timer % 60, 10);

            minutes = minutes < 10 ? "0" + minutes : minutes;
            seconds = seconds < 10 ? "0" + seconds : seconds;

            $('#strSessionTime').html('<i class="glyphicon glyphicon-time fa-2x"></i> Tiempo de sesión: <strong>' + minutes + ":" + seconds + '</strong>');
            
            if( --timer < 0 ){
                location.href = "/Account/SignOut"
            }

        }, 1000);
    }


    //  Por cada movimiento del mouse el contador de inactividad vuelve a cero
    //  Y PASADO 3 MINUTOS (180000 milisegundos)
    $(this).mousemove(function (e) {
        if (idleTime >= 180000) {
            //  actualizo el tiempo de session
            resetSessionTime();
        }
    })

    //  Cada vez que se presiona una tecla click el contador de inactividad vuelve a cero
    //  Y PASADO 3 MINUTOS (180000 milisegundos)
    $(this).keypress(function (e) {
        if (idleTime >= 180000) {
            //  actualizo el tiempo de session
            resetSessionTime();
        }
    })


    function resetSessionTime() {
        idleTime = 0;
        timer = sessionTime;
    }
})