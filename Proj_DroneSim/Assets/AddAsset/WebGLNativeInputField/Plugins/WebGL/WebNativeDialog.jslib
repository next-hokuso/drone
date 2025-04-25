﻿var WebNativeDialog = {
  NativeDialogPrompt:function (title , defaultValue){
    //defaultValue = Pointer_stringify(defaultValue);
    //title = Pointer_stringify(title);
    defaultValue = UTF8ToString(defaultValue);
    title = UTF8ToString(title);
    var result = window.prompt( title , defaultValue );
    //if( !result ){
    //  result = defaultValue;
    //}
    //var buffer = _malloc(lengthBytesUTF8(result) + 1);
    //writeStringToMemory(result, buffer);

    if (result == null)
    {
        result = defaultValue;
    }
    var bufferSize = lengthBytesUTF8(result) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(result, buffer, bufferSize);
    return buffer;
  },
  SetupOverlayDialogHtml:function(title,defaultValue,okBtnText,cancelBtnText){
    //title = Pointer_stringify(title);
    //defaultValue = Pointer_stringify(defaultValue);
    //okBtnText = Pointer_stringify(okBtnText);
    //cancelBtnText = Pointer_stringify(cancelBtnText);
    title = UTF8ToString(title);
    defaultValue = UTF8ToString(defaultValue);
    okBtnText = UTF8ToString(okBtnText);
    cancelBtnText = UTF8ToString(cancelBtnText);
    if( !document.getElementById("nativeInputDialogInput" ) ){
      // setup css
      var style = document.createElement( 'style' );
      style.setAttribute('id' , 'inputDialogTextSelect');
      style.appendChild( document.createTextNode( '#nativeInputDialogInput::-moz-selection { background-color:#00ffff;}' ) );
      style.appendChild( document.createTextNode( '#nativeInputDialogInput::selection { background-color:#00ffff;}' ) );
      document.head.appendChild( style );
    }
    if( !document.getElementById("nativeInputDialog" ) ){
      // setup html
      var html = '<div id="nativeInputDialog" style="background:#000000;opacity:0.9;width:100%;height:100%;position:fixed;top:0%;z-index:2147483647;">' + 
               '  <div style="position:relative;top:30%;" align="center" vertical-align="middle">' + 
               '    <div id="nativeInputDialogTitle" style="color:#ffffff;">Here is title</div>' + 
               '    <div>' + 
               '      <input id="nativeInputDialogInput" type="text" size="20" onsubmit="">' + 
               '    </div>' + 
               '    <div style="margin-top:10px">' + 
               '      <input id="nativeInputDialogOkBtn" type="button" /*type="image" alt="OK" src="StreamingAssets/BTN_back.png" width="44" height="44"*/ value="OK" onclick=""*/ >' + 
               '      <input id="nativeInputDialogCancelBtn" type="button" /*type="image" alt="Cancel" src="StreamingAssets/BTN_close.png" width="44" height="44"*/ value="Cancel" onclick ="">' + 
               '      <input id="nativeInputDialogCheck" type="checkBox" style="display:none;">' + 
               '    </div>' + 
               '  </div>' + 
               '</div>';
      var element = document.createElement('div');
      element.innerHTML = html;
      // write to html
      document.body.appendChild( element );

      // set Event
      // document.getElementById("canvas").style == null, comment out
      var okFunction = 
        'document.getElementById("nativeInputDialog" ).style.display = "none";' + 
        'document.getElementById("nativeInputDialogCheck").checked = false;'/* +
        'document.getElementById("canvas").style.display="";'*/;
      var cancelFunction = 
        'document.getElementById("nativeInputDialog" ).style.display = "none";'+ 
        'document.getElementById("nativeInputDialogCheck").checked = true;'/* +
        'document.getElementById("canvas").style.display="";'*/;

      var inputField = document.getElementById("nativeInputDialogInput");
      inputField.setAttribute( "onsubmit" , okFunction );
      var okBtn = document.getElementById("nativeInputDialogOkBtn");
      okBtn.setAttribute( "onclick" , okFunction );
      var cancelBtn = document.getElementById("nativeInputDialogCancelBtn");
      cancelBtn.setAttribute( "onclick" , cancelFunction );
    }
    document.getElementById("nativeInputDialogTitle").innerText = title;
    document.getElementById("nativeInputDialogInput").value= defaultValue;

    document.getElementById("nativeInputDialogOkBtn").value = okBtnText;
    document.getElementById("nativeInputDialogCancelBtn").value = cancelBtnText;
    document.getElementById("nativeInputDialog" ).style.display = "";
  },
  HideUnityScreenIfHtmlOverlayCant:function(){
    if( navigator.userAgent.indexOf("Chrome/") < 0 ){
      /*document.getElementById("canvas").style.display="none";*/
    }
  },
  IsRunningOnEdgeBrowser:function(){
    if( navigator.userAgent.indexOf("Edge/") < 0 ){
      return false;
    }
    return true;
  },
  IsOverlayDialogHtmlActive:function(){
    var nativeDialog = document.getElementById("nativeInputDialog" );
    if( !nativeDialog ){
      return false;
    }
    return ( nativeDialog.style.display != 'none' );
  },
  IsOverlayDialogHtmlCanceled:function(){
    var check = document.getElementById("nativeInputDialogCheck");
    if( !check ){ return false; }
    return check.checked;
  },
  GetOverlayHtmlInputFieldValue:function(){
    var inputField = document.getElementById("nativeInputDialogInput");
    var result = "";
    if( inputField && inputField.value ){
      result = inputField.value;
    }
    //var buffer = _malloc(lengthBytesUTF8(result) + 1);
    //writeStringToMemory(result, buffer);
    var bufferSize = lengthBytesUTF8(result) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(result, buffer, bufferSize);
    return buffer;
  }

};
mergeInto( LibraryManager.library , WebNativeDialog );

