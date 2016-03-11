/*$(function () {
    $(".draggable").draggable();

});

$(function () {
    $(".droppable").droppable({
        drop: function (event, ui) {            
            $(ui.draggable)                
                .detach().css({left: 0}).appendTo(this)                
        }
    })
});
*/
$(document).ready(function () {
    $('#spinner').hide();
    console.log("111");
    $.ajaxSetup({
        beforeSend: function () {
            $('#spinner').show()
        },
        complete: function () {
            $('#spinner').hide()
        },
        error: function () {
            $('#spinner').hide()
        }
    });
});




/*$( ".droppable" ).droppable({
    drop: function( event, ui ) {
        var cur_left = $(ui.draggable).position().left;
        var ner_left = $(this).position.left;
        $(ui.draggable)
            .css({left: new_left})              
    })
}*/



