USE [MHM-WMS-DB]
GO

/****** Object:  StoredProcedure [dbo].[upc_解凍機発振開始終了確認]    Script Date: 2025/08/01 9:59:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[upc_解凍機発振開始終了確認]
    -- IN --
    @MODE INT -- 0:開始、1:終了
    ,@CONFIRM INT-- 0:未確認、1:確認済
    -- OUT --
    ,@O_RET                 INT             OUTPUT
    ,@O_MSG                 VARCHAR(5120)   OUTPUT
AS
BEGIN
    SET NOCOUNT ON
    --SET XACT_ABORT OFF
    BEGIN TRY
        ---------------------------------------------------------------------------
        -- 型宣言部(共通)
        ---------------------------------------------------------------------------
        --*****エラー管理用*****
        DECLARE @SERVERITY NUMERIC(2,0) SET @SERVERITY  = 11--エラー重量度 11?18
        DECLARE @REC_ROWNO NUMERIC(3,0) SET @REC_ROWNO  = 1--エラーブロックNO 1?127
        --*****エラー管理用*****
        DECLARE @P_NAME VARCHAR(128) SELECT @P_NAME = OBJECT_NAME(@@PROCID)
        ---------------------------------------------------------------------------
        -- 型宣言部
        ---------------------------------------------------------------------------
        DECLARE @system_status_type TINYINT;
        ---------------------------------------------------------------------------
        -- 変数初期化
        ---------------------------------------------------------------------------
        SET @O_RET = 0;
        SET @O_MSG = '';
        ---------------------------------------------------------------------------
        -- 以下実行処理
        ---------------------------------------------------------------------------
        -- WMS_STATUSのSYSTEM_STATUS_TYPE=0の場合は確認メッセージをセットしてRETURN
        SELECT TOP 1 @system_status_type = SYSTEM_STATUS_TYPE FROM WMS_STATUS;
        IF @CONFIRM = 0 AND @system_status_type = 0
        BEGIN
            SET @O_RET = 1;
            SET @O_MSG = N'業務終了されていませんが、状態を変更して宜しいですか？';
            RETURN;
        END
        ---- ...既存処理...
        ---- 発振開始
        --IF @MODE = 0
        --BEGIN
        --    -- 発振開始の処理
        --    INSERT INTO [dbo].CP_COMMAND([コマンドID], [サブコマンドID], [処理済]) 
        --    VALUES (1, 2600101, 0);
        --    SET @O_MSG = N'解凍機の発振を開始しました。';
        --END
        ---- 発振終了
        --ELSE IF @MODE = 1
        --BEGIN
        --    -- 発振終了の処理
        --    INSERT INTO [dbo].CP_COMMAND([コマンドID], [サブコマンドID], [処理済]) 
        --    VALUES (1, 2600100, 0);
        --    SET @O_MSG = N'解凍機の発振を終了しました。';
        --END
        --ELSE
        --BEGIN
        --    SET @O_RET = -1;
        --    SET @O_MSG = N'無効なモードです。';
        --END
    END TRY
    BEGIN CATCH
        SET @O_RET = -1;
        SET @O_MSG = ERROR_MESSAGE();
        EXECUTE upc_RAISE_CUSTOM_ERROR
    END CATCH
END


GO


