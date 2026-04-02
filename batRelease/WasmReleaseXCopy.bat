echo off 
rem スクリプトが配置されているパスの一つ上のディレクトリ(カレントディレクトリ以外で指定したい場合は編集必要)
set currentPath=%cd%
set currentUpPath=%cd%\..

rem コピーしたいファイルが配置されているパス
set sourcePath=app\wasm

rem コピー先のパス
set targetPath=C:\appWMS\wasm

rem 指定するフォルダのすべてのファイルをコピーする。%currentPath%\exclude.txtに記載されているファイルは除く。
xcopy /Y /E /EXCLUDE:%currentPath%\exclude.txt %currentUpPath%\%sourcePath%\*.* %targetPath%

rem リリースしたファイルを圧縮してバックアップ
call compress.bat %currentUpPath%\%sourcePath%

pause