// CustomLoginForm用
var loginKey;
function initializeLoginKeyListener(dotnetHelper) {
    loginKey = (e) => {
        if (e.keyCode == 13 || e.keyCode == 112) {
            // Enter, F1
            e.preventDefault();
            // Enterキーによるフォーカス制御のため、activeElementのIDを渡す
            dotnetHelper.invokeMethod('OnKeyDown', e.keyCode, document.activeElement.id);
        }
    };
    window.addEventListener('keydown', loginKey);
}
function removeLoginKeyListener() {
    window.removeEventListener('keydown', loginKey);
}

// ButtonFuncRadzen用
var funcKey = [];
function initializeFuncKeyListener(dotnetHelper, id) {
    funcKey[id] = (e) => {
        if (e.keyCode == 33 || e.keyCode == 34 || e.keyCode >= 112 && e.keyCode <= 123) {
            if(e.keyCode == 33 || e.keyCode == 34 || e.keyCode >= 112 && e.keyCode <= 122) {
                // F1～F11,PageUp,PageDown
                e.preventDefault();
            }
            else if (e.keyCode == 123) {
                // F12
                //ファンクションキーのみ元のイベントをキャンセルする
                DotNet.invokeMethodAsync('ExpressionDBBlazorShared', 'DebugMethod').then(data => {
                    if (true == data) {

                    }
                    else {
                        e.preventDefault();
                    }
                });
            }
            dotnetHelper.invokeMethod('OnKeyDown', e.keyCode);
        }
    };
    window.addEventListener('keydown', funcKey[id]);
}
function removeFuncKeyListener(id) {
    window.removeEventListener('keydown', funcKey[id]);
    if (funcKey.hasOwnProperty(id)) {
        delete funcKey[id];
    }
}

// ButtonFuncRadzen用(HT)
var funcKeyHT=[];
function initializeFuncKeyHTListener(dotnetHelper, id) {
    funcKeyHT[id] = (e) => {
        if ((e.keyCode == 33 || e.keyCode == 34 || e.keyCode >= 112 && e.keyCode <= 123) || e.keyCode == 189 || e.keyCode == 190) {
            if ((e.keyCode == 33 || e.keyCode == 34 || e.keyCode >= 112 && e.keyCode <= 122) || e.keyCode == 189 || e.keyCode == 190) {
                // F1～F11,Pageup,PageDown
                e.preventDefault();
            }
            else if (e.keyCode == 123) {
                // F12
                //ファンクションキーのみ元のイベントをキャンセルする
                DotNet.invokeMethodAsync('ExpressionDBBlazorShared', 'DebugMethod').then(data => {
                    if (true == data) {

                    }
                    else {
                        e.preventDefault();
                    }
                });
            }
            dotnetHelper.invokeMethod('OnKeyDown', e.keyCode);
        }
    };
    window.addEventListener('keydown', funcKeyHT[id]);
}
function removeFuncKeyHTListener(id) {
    window.removeEventListener('keydown', funcKeyHT[id]);
    if (funcKeyHT.hasOwnProperty(id)) {
        delete funcKeyHT[id];
    }
}

// ButtonFuncRadzen用
var htStepKey;
function initializeHtStepKeyListener(dotnetHelper) {
    htStepKey = (e) => {
        if (e.keyCode >= 37 && e.keyCode <= 40) {
            // 上下左右
            e.preventDefault();
            dotnetHelper.invokeMethod('OnKeyDownDataSelect', e.keyCode);
        }
    };
    window.addEventListener('keydown', htStepKey);
}
function removeHtStepKeyListener() {
    window.removeEventListener('keydown', htStepKey);
}

// ButtonFuncRadzen用
var htMenuKey;
function initializeTtMenuKeyListener(dotnetHelper) {
    htMenuKey = (e) => {
        if (e.keyCode >= 48 && e.keyCode <= 57) {
            dotnetHelper.invokeMethod('OnKeyDown', e.keyCode);
        }
    };
    window.addEventListener('keydown', htMenuKey);
}
function removeTtMenuKeyListener() {
    window.removeEventListener('keydown', htMenuKey);
}

// Enterキーのkeydownイベントで、フォーカスが当たっているElementのIDを返す
var enterKeyPress;
function initializeEnterKeyPressListener(dotnetHelper) {
    enterKeyPress = (e) => {
        if (e.keyCode == 13) {
            dotnetHelper.invokeMethod('OnEnterKeyPress', document.activeElement.id);
        }
    };
    window.addEventListener('keypress', enterKeyPress);
}
function removeEnterKeyPressListener() {
    window.removeEventListener('keypress', enterKeyPress);
}
// Scrollイベントで、ボタン表示を制御するメソッドを呼び出す
var scrolle = [];
function initializeScrollListener(dotnetHelper, id, bid) {
    scrolle[id] = (e) => {
        dotnetHelper.invokeMethod('OnScroll');
    };
    window.document.getElementById(bid).addEventListener('scroll', scrolle[id]);
}
function removeScrollListener(id, bid) {
    window.document.getElementById(bid).removeEventListener('scroll', scrolle[id]);
    if (scrolle.hasOwnProperty(id)) {
        delete scrolle[id];
    }
}
// RadzenTextBoxフォーカス時に全選択用
function selectText(inputId) {
    var inp = document.querySelector("#" + inputId);
    if (inp != null && inp.select) {
        inp.select();
    }
}

