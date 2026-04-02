@echo off
rem sqlcmdで実行するファイルの文字コード 65001:UTF-8 , 932:Shift-JIS
set CodePage=65001
rem set CodePageR=932
chcp %CodePage%

rem 発行に使用するプロファイル名
set profileName=FolderProfile.pubxml
rem set profileName=IISProfile.pubxml

rem 発行を行うプロジェクト名
set projectfileName=ExpressionDBBlazorWasmApp

rem ログファイル名
 set LogFileName=PublishExecute_Log
rem ログファイルを削除
 del %~dp0\%LogFileName%.log

rem ビルドの前にプロジェクトクリーンを行っておく
rem dotnet clean ..\ExpressionDBBlazorShared\ExpressionDBBlazorShared.csproj
rem dotnet clean %targetPath%\%projectfileName%\%projectfileName%.csproj

rem プロジェクトをリリースビルド
dotnet build %~dp0\%projectfileName%\%projectfileName%.csproj --configuration Release
rem プロジェクトを発行
dotnet publish %~dp0\%projectfileName%\%projectfileName%.csproj -c Release -p:PublishProfile=%~dp0\%projectfileName%\Properties\PublishProfiles\%profileName% >> %~dp0\%LogFileName%.log
IF %ERRORLEVEL% EQU 0 (
  rem プロジェクトを発行
  rem   dotnet publish .\%projectfileName%\%projectfileName%.csproj -c Release -p:PublishProfile=.\%projectfileName%\Properties\PublishProfiles\FolderProfile.pubxml -o ./publish >> %~dp0\%LogFileName%.log
  echo プロジェクトの発行処理に成功しました。
) ELSE (
  echo プロジェクトの発行に失敗しました。
)

echo プロジェクトの発行処理を完了しました。
pause