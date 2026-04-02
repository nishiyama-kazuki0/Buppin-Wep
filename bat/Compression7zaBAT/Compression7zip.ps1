# 出力をコンソールの文字コードと同じにする(clipコマンドなどへのリダイレクトに必要)
#$OutputEncoding = [Console]::OutputEncoding
# 任意の文字コードにする
# $OutputEncoding = New-Object System.Text.ASCIIEncoding
#$OutputEncoding = [System.Text.Encoding]::GetEncoding('utf-8')
$ErrorActionPreference = "Stop"  # コマンド異常時にスクリプトを終了する

#入力フォルダパスを取得
#$folderpath = "D:\TC\DMP"
$folderpath = "D:\TC\dbbakup"

#入力フォルダ内のファイルを取得
$folder = get-childItem $folderpath -Filter *.bak
#圧縮対象日付を取得
$date = Get-Date -Format yyyyMMdd
#$datetimestamp = Get-Date -Format yyyyMMddHHmmss

## 7-Zipの場所を指定
# スクリプトファイルのパスを取得
$path=Split-Path ( & { $myInvocation.ScriptName } ) -parent
$7zip=$path+"\7za.exe"

foreach($file in $folder){
    #入力ファイルの更新日付を取得
    $timestamp = $(get-itemProperty $folderpath\$file).LastWriteTime.ToString('yyyyMMdd')
    #圧縮対象日付けと入力ファイル更新日時の比較
    if($date -gt $timestamp){

        $targetFile = $folderpath + "\" + $file
	#大文字小文字は区別していることに注意
        $dist = $targetFile.Replace(".bak",".7z")
        # 7-Zipのコマンドライン引数を生成(a 圧縮 x 解凍)
        $Arg="a "+$dist+" "+$targetFile
        # 7z a実行
        $proc = Start-Process -FilePath $7zip -ArgumentList $Arg -Wait -NoNewWindow -PassThru

        if($proc.ExitCode -eq 0) {
            #圧縮が成功した場合
            
            $remotePath = "F:\WMS_HD\dbbackup7z"
            <#
            $remoteSV = "172.17.193.109" #ストレージサーバー
            #$remotePath = "\\172.17.193.109\share2\WMS\DBBK"
            $credential = New-Object System.Management.Automation.PSCredential $user, $pass
            $session = New-PSSession $remoteSV -Credential $credential 
            Copy-Item -Path $dist -Destination $remotePath -ToSession $session -Recurse
            Remove-PSSession $session
            #>
            try {
                #資格情報はOSに登録済みとして扱う
                #ファイルのコピー
                Copy-Item -Path $dist -Destination $remotePath 
                #圧縮後のファイルを削除
                [System.IO.File]::Delete($dist)
            }catch{
                
            }
            #リモートにファイルコピー後に元ファイルを削除
            [System.IO.File]::Delete($targetFile)
        }
    }
}
###
# 古い圧縮ファイル削除処理
###
# 8カ月前の日付を取得
$td = (get-date).addMonths(-8)
$pastDate = get-date($td) -Format yyyyMMdd
$myfolderpath = "F:\WMS_HD\dbbackup7z"
$myfolder = get-childItem $myfolderpath -Filter *.7z
foreach($myfile in $myfolder){
    #入力ファイルの更新日付を取得
    $mytimestamp = $(get-itemProperty $myfolderpath\$myfile).LastWriteTime.ToString('yyyyMMdd')
    #8カ月前と7zファイルの日付を比較
    if($pastDate -gt $mytimestamp){

        $mytargetFile = $myfolderpath + "\" + $myfile
        #古いファイルを削除
        [System.IO.File]::Delete($mytargetFile)
    }
}

exit 