//Quaggaの初期処理と読込開始処理（ブラウザにカメラ機能の許可をする必要がある）
function QuaggaStartRead()
{
    var modal = document.getElementById('modal');
    modal.style.display = 'block';
    document.body.style.overflow = 'hidden';
    Quagga.init(
        {
            inputStream: {
                name: "Live",
                type: 'LiveStream',
                target:document.getElementById("modal"),
                constraints: {
                    width: window.innerWidth
                },
            },
            decoder: {
                //読み込むコードのフォーマット指定はここで。（対応するコードに関しては」QuaggaのReadmeを参照）※QuaggaはQRコード未対応のためQRは別ライブラリを用いて実装する必要あり。
                readers: [
                    {
                        format: 'ean_reader',
                        config: {},
                    },
                    {
                        format: 'code_39_reader',
                        config: {},
                    },
                ],
            },
        },
        (err) => {
            if (!err) {
                Quagga.start();
                timeoutId = setTimeout(QuagaStopRead, 15000);
            } else {
                modal.style.display = 'none';
                document.body.style.overflow = '';
                Quagga.stop();
                alert(
                    'この機能を利用するには\nブラウザのカメラ利用を許可してください。'
                );
            }
        }
    );
}
//読取の終了処理読み込み画面を閉じる
function QuagaStopRead()
{
    var modal = document.getElementById('modal');
    modal.style.display = 'none';
    document.body.style.overflow = '';
    clearTimeout(timeoutId); // 成功したらタイムアウトをクリア
    Quagga.stop();
}

//タイムアウトID
var timeoutId ;

var DetectedCount = 0;
var DetectedCode = "";
//読取成功後の処理。この処理で取得したコードの詳細を取得する。
Quagga.onDetected((result) => {
    var modal = document.getElementById('modal');
    
    //読み取り誤差が多いため、3回連続で同じ値だった場合に成功とする
    if (DetectedCode == result.codeResult.code) {
        DetectedCount++;
    } else {
        DetectedCount = 0;
        DetectedCode = result.codeResult.code;
    }
    if (DetectedCount >= 3) {
        clearTimeout(timeoutId); // 成功したらタイムアウトをクリア
        var decodeResult = "SUCCESS";//Quagga.onDetectedは読取(デコード)成功した時発生するので、成功値固定としておく
        var codeType = result.codeResult.format;
        var stringData = result.codeResult.code;
        modal.style.display = 'none';
        document.body.style.overflow = '';
        Quagga.stop();
        DetectedCode = '';
        DetectedCount = 0;
        DotNet.invokeMethodAsync('ExpressionDBBlazorShared', 'CallScanFunction', decodeResult, codeType, stringData);//modalを閉じた後に呼び出しを行う。
    }
});

//OnDetectedで取得できるデコード情報の例
// #region
// "codeResult": 
//  {
//      "code": "FANAVF1461710",  // デコードされたコード（文字列）
//      "format": "code_128",     // バーコードの種類（例：code_39, codabar, ean_13, ean_8, upc_a, upc_eなど）
//      "start": 355,             // バーコードの開始位置
//      "end": 26,                // バーコードの終了位置
//      "codeset": 100,           // コードセット
//      "decodedCodes": [         // デコードされたコードの詳細
//          {
//              "code": 104,
//              "start": 21,
//              "end": 41
//          },
//          {
//              "error": 0.8888888888888893,
//              "code": 106,
//              "start": 328,
//              "end": 350
//          }],
//          "startInfo": {
//          "error": 1.0000000000000002,
//      ...
//        }
//    },
//  ...
//}
// #endregion


//下記はコードを認識した際の処理となる。デバッグ用にコードとして認識できているかを可視化する処理なので実装しなくても問題はない。
Quagga.onProcessed((data) => {
    const ctx = Quagga.canvas.ctx.overlay;
    const canvas = Quagga.canvas.dom.overlay;

    if (!data) return;

    // 認識したバーコードを緑の枠で囲む
    if (data.boxes) {
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        const hasNotRead = box => box !== data.box;
        data.boxes.filter(hasNotRead).forEach(box => {
            Quagga.ImageDebug.drawPath(box, { x: 0, y: 1 }, ctx, { color: "green", lineWidth: 2 });
        });
    }

    // 読み取ったバーコードを青の枠で囲む
    if (data.box) {
        Quagga.ImageDebug.drawPath(data.box, { x: 0, y: 1 }, ctx, { color: "blue", lineWidth: 2 });
    }

    // 読み取ったバーコードに赤い線を引く
    if (data.codeResult && data.codeResult.code) {
        Quagga.ImageDebug.drawPath(data.line, { x: "x", y: "y" }, ctx, { color: "red", lineWidth: 3 });
    }
});

