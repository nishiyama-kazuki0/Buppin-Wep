\@echo off
setlocal enabledelayedexpansion

REM ============================================
REM 設定
REM ============================================

REM ダウンロード先（OneDrive リダイレクト配慮）
set "DL1=%USERPROFILE%\Downloads"
set "DL2=%USERPROFILE%\OneDrive\Downloads"

if exist "%DL1%" (
    set "DEST_DIR=%DL1%"
) else if exist "%DL2%" (
    set "DEST_DIR=%DL2%"
) else (
    set "DEST_DIR=%DL1%"
    mkdir "%DEST_DIR%" 2>nul
)

REM 監視間隔（秒）…大きなファイル対策で 2～5 秒が無難
set "INTERVAL=2"

REM コピーではなく移動（move）する
REM もし元フォルダーに残したいなら下の MOVE_CMD を copy に変更してください
set "MOVE_CMD=move /Y"

REM 移動直後に開く前の待機（ミリ秒）…ファイルロック対策（0～2000 くらい）
set "OPEN_DELAY_MS=300"

REM 対象拡張子（必要に応じて追加/削除）
set "EXT_LIST=xlsx xls xlsm xlsb xltm xltx"
REM ============================================


REM ダウンロード先が無ければ作成（念のため）
if not exist "%DEST_DIR%" (
    mkdir "%DEST_DIR%" 2>nul
)

echo 監視開始: %~dp0
echo Excel ファイルが見つかると "%DEST_DIR%" に移動し、自動で開きます。
echo 中止するには Ctrl + C を押してください。
echo.

:LOOP
for %%E in (%EXT_LIST%) do (
    for %%F in ("%~dp0*%%E") do (
        if exist "%%~fF" (
            REM 競合回避（同名があれば _1, _2… を付与）
            set "SRC=%%~fF"
            set "BASENAME=%%~nF"
            set "EXT=%%~xF"
            set "TARGET=%DEST_DIR%\!BASENAME!!EXT!"

            set /a N=1
            :RENAMECHECK
            if exist "!TARGET!" (
                set "TARGET=%DEST_DIR%\!BASENAME!_!N!!EXT!"
                set /a N+=1
                goto RENAMECHECK
            )

            echo 移動: "!SRC!"  ->  "!TARGET!"
            %MOVE_CMD% "!SRC!" "!TARGET!" >nul 2>&1
            if errorlevel 1 (
                echo [警告] 移動に失敗しました: "!SRC!"
            ) else (
                REM 移動成功 → 少し待ってからｓ既定アプリ（Excel）で開く
                if %OPEN_DELAY_MS% gtr 0 (
                    powershell -NoProfile -Command "Start-Sleep -Milliseconds %OPEN_DELAY_MS%"
                )
                REM 既定の関連付けで開く（Excel が関連付けられている想定）
                start "" "!TARGET!"
            )
        )
    )
)

REM 次のスキャンまで待機
timeout /t %INTERVAL% /nobreak >nul
goto LOOP