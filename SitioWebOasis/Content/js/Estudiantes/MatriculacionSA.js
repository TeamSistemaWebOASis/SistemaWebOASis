$(document).ready(function () {
    var popOverSettings = { container: 'body',
                            placement: 'left',
                            toggle: 'popover', 
                            html: true }

    $("button[rel=popover]").popover(popOverSettings);
})