echo off
rem targetPathは他で変更されるので、絶対パスで指定する

rem webAPIの起動batを実行する
CALL C:\appWMS\bat\WEBAPI_KESTREL_START.bat 

rem webAPIが起動しきる必要があるので、5秒待たせる
timeout /t 5

rem CycleProcを起動する(webAPIが起動していないとエラーとなる)
CALL C:\appWMS\bat\HOSTED_WORKER_START.bat 

rem HULFT連携アプリの起動
CALL C:\appWMS\bat\HULFTIF_START.bat 

