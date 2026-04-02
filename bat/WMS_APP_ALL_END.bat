echo off
rem targetPathは他で変更されるので、絶対パスで指定する

rem CycleProcのタスクをKILLする
CALL C:\appWMS\bat\HOSTED_WORKER_END.bat 

rem webAPIのタスクをKILLする
CALL C:\appWMS\bat\WEBAPI_KESTREL_END.bat 

rem FTP連携アプリのタスクをKILLする
CALL C:\appWMS\bat\HULFTIF_END.bat 

