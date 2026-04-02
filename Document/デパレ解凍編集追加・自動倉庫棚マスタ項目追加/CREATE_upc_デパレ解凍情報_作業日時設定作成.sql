USE [MHM-WMS-DB]
GO
/****** Object:  StoredProcedure [dbo].[upc_デパレ解凍情報_編集]    Script Date: 2025/09/12 16:25:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[upc_デパレ解凍情報_編集]
    -- IN --
     @DEVICE_ID             VARCHAR(128)
    ,@USER_ID               VARCHAR(10)
    ,@バッチNO          VARCHAR(16)
    ,@原料登録NO              INT
    ,@デパレカウントNO        INT
    ,@解凍開始日時          VARCHAR(19)
    ,@解凍完了日時          VARCHAR(19)
    -- OUT --
    ,@O_RET                 INT             OUTPUT
    ,@O_MSG                 VARCHAR(5120)   OUTPUT
AS
BEGIN
    SET NOCOUNT ON
    
    BEGIN TRY
        ---------------------------------------------------------------------------
        -- 型宣言部(共通)
        ---------------------------------------------------------------------------
        --*****エラー管理用*****
        DECLARE @SERVERITY NUMERIC(2,0) SET @SERVERITY  = 11--エラー重量度 11?18
        DECLARE @REC_ROWNO NUMERIC(3,0) SET @REC_ROWNO  = 1--エラーブロックNO 1?127
        --*****エラー管理用*****
        DECLARE @P_NAME VARCHAR(128)    SELECT @P_NAME = OBJECT_NAME(@@PROCID)
        
        ---------------------------------------------------------------------------
        -- 型宣言部
        ---------------------------------------------------------------------------
        DECLARE @CNT                    INT;
        DECLARE @開始                   BIGINT;
		DECLARE @完了                   BIGINT;
        ---------------------------------------------------------------------------
        -- 変数初期化
        ---------------------------------------------------------------------------
        SET @O_RET = 0;--負数:エラー,0:正常,正数:警告
        SET @O_MSG = '';--クライアント側で表示させるメッセージ
        
        ---------------------------------------------------------------------------
        -- 以下実行処理
        ---------------------------------------------------------------------------
        -- ロックなど必要か？

		
		------------------------------------楠井-------------------------------------
		-- 空チェックと変換　
		IF @解凍開始日時 IS NULL OR @解凍開始日時 = ''　BEGIN
			IF @解凍完了日時 IS NOT NULL AND @解凍完了日時 <> ''　BEGIN
				SET @O_RET = -1;
				SET @O_MSG = '解凍完了日時のみが入力されています。解凍開始日時も入力してください。';
				RETURN
			END
			--両方NULL
			ELSE BEGIN
				SET @解凍開始日時 = ' '
				SET @解凍完了日時 = ' '
				-- バッチデータ(原料投入指図)の更新
					UPDATE DEPALLET_THAWING_STATUS
					SET
						 -- ヘッダ
						解凍開始日時       = @解凍開始日時,
						解凍完了日時       = @解凍完了日時,
						 -- 汎用
						UPDATE_DATETIME    = dbo.ufc_get_datetime_to_char(getdate()),
						UPDATE_DEVICE_ID   = @DEVICE_ID,
						UPDATE_PNAME       = @P_NAME,
						UPDATE_USER_ID     = @USER_ID
					WHERE
						バッチNO           = @バッチNO
						AND 原料登録NO     = @原料登録NO
						AND デパレカウントNO = @デパレカウントNO

						IF @@ROWCOUNT = 0 BEGIN
							SET @O_RET = -1;
							SET @O_MSG = '更新対象のデータが存在しません。';
							RETURN;
						END
			END

		END
		-- @解凍開始日時NULLでないとき
		ELSE BEGIN
			IF @解凍完了日時 IS NOT NULL AND @解凍完了日時 <> ''　BEGIN
				SET @開始 = REPLACE(REPLACE(REPLACE(@解凍開始日時, '/', ''), ':', ''), ' ', '')
				SET @完了 = REPLACE(REPLACE(REPLACE(@解凍完了日時, '/', ''), ':', ''), ' ', '')

				-- 比較して条件を満たす場合のみ UPDATE 実行
				IF @開始 < @完了　BEGIN
				  -- バッチデータ(原料投入指図)の更新
					UPDATE DEPALLET_THAWING_STATUS
					SET
						 -- ヘッダ
						解凍開始日時       = @解凍開始日時,
						解凍完了日時       = @解凍完了日時,
						 -- 汎用
						UPDATE_DATETIME    = dbo.ufc_get_datetime_to_char(getdate()),
						UPDATE_DEVICE_ID   = @DEVICE_ID,
						UPDATE_PNAME       = @P_NAME,
						UPDATE_USER_ID     = @USER_ID
					WHERE
						バッチNO           = @バッチNO
						AND 原料登録NO     = @原料登録NO
						AND デパレカウントNO = @デパレカウントNO

						IF @@ROWCOUNT = 0 BEGIN
							SET @O_RET = -1;
							SET @O_MSG = '更新対象のデータが存在しません。';
							RETURN;
						END
				END

				ELSE
				BEGIN
					SET @O_RET = -1;
					SET @O_MSG = '解凍開始日時は解凍完了日時より前である必要があります。';
					RETURN
				END
			END
			--@解凍完了日時がNULLのとき
			ELSE BEGIN 
					
						SET @開始 = REPLACE(REPLACE(REPLACE(@解凍開始日時, '/', ''), ':', ''), ' ', '')
						SET @解凍完了日時 = ' '
					  -- バッチデータ(原料投入指図)の更新
						UPDATE DEPALLET_THAWING_STATUS
						SET
							 -- ヘッダ
							解凍開始日時       = @解凍開始日時,
							解凍完了日時       = @解凍完了日時,
							 -- 汎用
							UPDATE_DATETIME    = dbo.ufc_get_datetime_to_char(getdate()),
							UPDATE_DEVICE_ID   = @DEVICE_ID,
							UPDATE_PNAME       = @P_NAME,
							UPDATE_USER_ID     = @USER_ID
						WHERE
							バッチNO           = @バッチNO
							AND 原料登録NO     = @原料登録NO
							AND デパレカウントNO = @デパレカウントNO

							IF @@ROWCOUNT = 0 BEGIN
								SET @O_RET = -1;
								SET @O_MSG = '更新対象のデータが存在しません。';
								RETURN;
							END

				END
			END
		

		-- 正常終了時のメッセージを追加しておく
        SET @O_RET = 0;
    END TRY
    BEGIN CATCH
        SET @O_RET = -1
        EXECUTE upc_RAISE_CUSTOM_ERROR
    END CATCH
END