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
$td = (get-date).addMonths(-8)#削除対象の日数分減算
$pastDate = get-date($td) -Format yyyyMMdd
$myfolderpath = "F:\WMS_HD\CSV"
#対象のファイルを再帰的に取得
$myfolder = get-childItem $myfolderpath -Recurse -Filter *.csv #csvファイルに絞っておく
foreach($myfile in $myfolder){
    #入力ファイルの更新日付を取得
    $mytimestamp = $(get-itemProperty $myfile.FullName).LastWriteTime.ToString('yyyyMMdd')
    #ファイルの日付を比較
    if($pastDate -gt $mytimestamp){

        #古いファイルを削除
        Remove-Item -LiteralPath $myfile.FullName -Force
    }
}
# 空のサブフォルダを再帰的に削除
Get-ChildItem -Path $myfolderpath -Directory -Recurse | Where-Object {
    (Get-ChildItem $_.FullName -File).Count -eq 0
} | ForEach-Object {
    Remove-Item $_.FullName -Force
}

exit 