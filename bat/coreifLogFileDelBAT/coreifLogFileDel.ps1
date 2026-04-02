# 出力をコンソールの文字コードと同じにする(clipコマンドなどへのリダイレクトに必要)
#$OutputEncoding = [Console]::OutputEncoding
# 任意の文字コードにする
# $OutputEncoding = New-Object System.Text.ASCIIEncoding
#$OutputEncoding = [System.Text.Encoding]::GetEncoding('utf-8')
$ErrorActionPreference = "Stop"  # コマンド異常時にスクリプトを終了する

## 7-Zipの場所を指定
# スクリプトファイルのパスを取得
$path=Split-Path ( & { $myInvocation.ScriptName } ) -parent

###
# 古いファイルの削除処理
###
# 削除閾値となる日付を取得
$td = (get-date).addDays(-120)#削除対象の日数分減算
$pastDate = get-date($td) -Format yyyyMMdd
$myfolderpath = "C:\appWMS\coreif\Release\Log"
$myfolder = get-childItem $myfolderpath -Filter *.log #logファイルに絞っておく
foreach($myfile in $myfolder){
    #入力ファイルの更新日付を取得
    $mytimestamp = $(get-itemProperty $myfolderpath\$myfile).LastWriteTime.ToString('yyyyMMdd')
    #ファイルの日付を比較
    if($pastDate -gt $mytimestamp){

        $mytargetFile = $myfolderpath + "\" + $myfile
        #古いファイルを削除
        [System.IO.File]::Delete($mytargetFile)
    }
}

exit 