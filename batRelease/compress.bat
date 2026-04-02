@echo off

rem 引数として圧縮する元のフォルダ名(フルパス)を指定する
rem フォルダ構成に注意。[app]を[history]に置き換える関係でレベルが同じ必要がある
rem 7zipを使用するため、事前にインストールすること

rem 引数がない場合は最後に飛んで終了する
if %1=="" (
  goto :end
)

set bin="C:\Program Files\7-Zip\7z.exe"

rem 日付「YYYYMMDD」形式
set str_date=%date:~0,4%%date:~5,2%%date:~8%

rem 時刻「HHmmss」形式
set str_time=%time: =0%
set str_time=%str_time:~0,2%%str_time:~3,2%%str_time:~6,2%

rem 圧縮前フォルダ(フルパス)
set str_folder=%1

echo %str_folder%

rem 圧縮後ファイル
mkdir %str_folder:app=history%
set str_zipfile=%str_folder:app=history%%str_folder:*app=%_%str_date%_%str_time%.zip

echo %str_zipfile%

if not "%str_folder%"=="" (
  rem zip圧縮実行
  %bin% a %str_zipfile% %str_folder%
)

:end

echo 処理を終了しました

rem pause
