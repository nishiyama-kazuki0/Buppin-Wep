rem スクリプトが配置されているパス(カレントディレクトリ以外で指定したい場合は編集必要)
set currentPath=%cd%

rem 共有フォルダの資格情報
set remoteUser=Administrator
set remmotePassword=zenn0h-wm$

rem コピーしたいファイルが配置されているパス
set sourcePath=ExpressionDBBlazorWasmApp\bin\Release\net7.0\publish

rem コピー先のパス
set targetPath=\\ExpressionDB-WMS-TEST\wwwroot


rem 共有フォルダのアクセスを行う
net use %targetPath% /user:%remoteUser% %remmotePassword%

rem 指定するフォルダのすべてのファイルをコピーする
rem xcopy /Y /EXCLUDE:C:\src\JA_ZENNOH_WMS\JA_ZENNOH_BLAZOR_APP\ZennohWebAPI\bin\Release\net7.0\exclude.txt C:\src\JA_ZENNOH_WMS\JA_ZENNOH_BLAZOR_APP\ZennohWebAPI\bin\Release\net7.0\*.* \\Zennoh-wms-test\tc\WebAPI\net7.0\
xcopy /Y /E %currentPath%\%sourcePath%\*.* %targetPath%

pause