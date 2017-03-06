// JavaScript Document
$("a[data-ajax=false]").click(handleClick);

/* ******************************************************************** */
function handleClick(e) { // AJAX SPECIFIC
    var target = $(e.target).closest('a');
    if( target ) {
        e.preventDefault();
        window.location = target.attr('href');
    }
}

/* ******************************************************************** */
// USED FOR SENDING SANCTION INFO FROM TOURNY LIST PAGE TO GETSCORES
function getRecordDataByType(inRecordType, inRecordSubType) { 
	document.forms['selectRecordList'].RecordType.value=inRecordType;
	document.forms['selectRecordList'].RecordSubType.value=inRecordSubType;
	document.forms['selectRecordList'].submit();	
}

/* ******************************************************************** */
// AJAX SPECIFIC RELOAD FEATURE
function reloadPage() { 
  $.mobile.changePage(
    document.location.reload(true),
    {
      allowSamePageTransition : true,
      transition              : 'none',
      showLoadMsg             : false,
      reloadPage              : true
    }
  );
}

/* ******************************************************************** */
// SET COOKIES...CURRENTLY FOR RECENT PASSES REFRESH RATE
function setCookie(c_name,value,exdays) { 
	var exdate=new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value=escape(value) + ((exdays==null) ? "" : "; expires="+exdate.toUTCString());
	document.cookie=c_name + "=" + c_value;
}

/* ******************************************************************** */
// RETRIEVE AND RETURN COOKIE VALUES...CURRENTLY FOR RECENT PASSES REFRESH RATE
function getCookie(c_name) { 
	var c_value = document.cookie;
	var c_start = c_value.indexOf(" " + c_name + "=");
	if (c_start == -1)
	  {
	  c_start = c_value.indexOf(c_name + "=");
	  }
	if (c_start == -1)
	  {
	  c_value = null;
	  }
	else
	  {
	  c_start = c_value.indexOf("=", c_start) + 1;
	  var c_end = c_value.indexOf(";", c_start);
	  if (c_end == -1)
	  {
	c_end = c_value.length;
	}
	c_value = unescape(c_value.substring(c_start,c_end));
	}
	return c_value;
}