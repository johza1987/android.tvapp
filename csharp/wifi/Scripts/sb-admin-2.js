$(function () {
    Global = function (option) {
        this.Global.Version = "1.0.0";
        this.Global.ClassBtnOk = 'btn-primary btn-sm btn-msg-box btn-msg-ok';
        this.Global.ClassBtnCancel = 'btn-default btn-sm btn-msg-box btn-msg-cancel';
    };

    Global();

    $('#side-menu').metisMenu();
    $('.datepicker').datepicker({
        language: 'th-th',
        autoclose: true
    });

    $('[data-toggle="tooltip"]').tooltip();
    $('[data-tooltip="true"]').tooltip();
});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function() {
    $(window).bind("load resize", function() {
        topOffset = 50;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    //var url = window.location;
    //var element = $('ul.nav a').filter(function() {
    //    return this.href == url || url.href.indexOf(this.href) == 0;
    //}).addClass('active').parent().parent().addClass('in').parent();
    //if (element.is('li')) {
    //    element.addClass('active');
    //}
	
	$('.btn-sidebar-toggle').on('click', function (e) {
		if ($('.sidebar-nav').width() > 0) {
			$('#page-wrapper').addClass("wrapper-fullwidth");
			$('.sidebar').hide();
		}
		else
		{
			$('#page-wrapper').removeAttr("class");
			$('.sidebar').show();
		}
	});
});
