$(function () {
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


/*$( ".droppable" ).droppable({
    drop: function( event, ui ) {
        var cur_left = $(ui.draggable).position().left;
        var ner_left = $(this).position.left;
        $(ui.draggable)
            .css({left: new_left})              
    })
}*/



