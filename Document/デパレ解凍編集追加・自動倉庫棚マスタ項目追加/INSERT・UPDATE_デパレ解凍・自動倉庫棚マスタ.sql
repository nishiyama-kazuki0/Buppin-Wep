--1_DEFINE_COMPONENTS_編集ボタン追加
 UPDATE
	[DEFINE_COMPONENTS]
	SET VALUE = '編集[F5]'
		
  WHERE ATTRIBUTES_NAME = 'button5text'
  AND CLASS_NAME = 'DepalletThawingStatus'

--2_DEFINE_COMPONENTS_編集ダイアログ追加
INSERT 
	INTO [DEFINE_COMPONENTS]
	  ([CLASS_NAME]
      ,[COMPONENT_NAME]
      ,[ATTRIBUTES_NAME]
      ,[VALUE]
      ,[VALUE_OBJECT_TYPE]
      ,[COMPONET_DATA_TYPE]
      ,[VALUE_MIN]
      ,[VALUE_MAX]
      ,[METHOD_NAME])
	  
  VALUES 
	('DepalletThawingStatus','AttributesEditDialog','Components','_gridColumns','1','',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','ComponentsInfos','_componentsInfo','1','',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','DialogLabelWidth','150px','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','DialogTitle','デパレ解凍情報メンテナンス','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','DialogType',NULL,'3','ExpressionDBBlazorShared.Shared.DialogDepalletThawingStatusFixContent',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','DialogWidth','500','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','InfoIconName','info','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','InfoTitle','デパレ解凍情報','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','InputIconName','edit','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','InputTitle','入力','0','System.String',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','Mode','Edit','2','ExpressionDBBlazorShared.Shared.enumDialogMode',NULL,NULL,NULL),
	('DepalletThawingStatus','AttributesEditDialog','ProgramName','デパレ解凍情報編集','0','System.String',NULL,NULL,NULL),

	('DepalletThawingStatus','F5CheckSelectRow','MessageContent','デパレ解凍情報が選択されていません。','0','System.String',NULL,NULL,NULL);

--3_DEFINE_COMPONENT_PROGRAMS_編集機能追加
  INSERT 
	INTO [DEFINE_COMPONENT_PROGRAMS]
	  ( [CLASS_NAME]
      ,[CURRENT_METHOD_NAME]
      ,[COMPONENT_NAME]
      ,[CALL_METHOD_NAME]
      ,[EXEC_ORDER_RANK]
      ,[PROCESS_PROGRAM_NAME]
      ,[AUTHORITY_LEVEL_LOWER]
      ,[PRGRAM_CALL_TYPE]
      ,[IS_PROGRAM_RETURN]
      ,[RETRUN_DATA_TYPE]
      ,[TIMEOUT_VALUE]
      ,[RETRY_COUNT]
      ,[ARGUMENT_DATA_SET_NAME]
      ,[IS_ASYNC])
	  
  VALUES 
	('DepalletThawingStatus','OnClickResultF5','AttributesEditDialog','サイドダイアログ表示_データ編集','1','','3','0','1','System.Boolean','-1','0','','0'),
	('DepalletThawingStatus','OnClickResultF5','AttributesGrid','グリッド更新','2','','3','0','0','','-1','0','','0'),
	('DepalletThawingStatus','OnClickResultF5','F5CheckSelectRow','選択行チェック','0','','3','0','1','System.Boolean','-1','0','','0');

	
--4_DEFINE_COMPONENT_COLUMNS_編集項目追加
  UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '5',
	EDIT_TYPE = '2',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompTextBox'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = 'デパレカウントNO'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '1',
	EDIT_TYPE = '2',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompTextBox'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = 'バッチNO'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '2',
	EDIT_TYPE = '2',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompTextBox'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = 'バッチ開始年月日'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '1',
	EDIT_TYPE = '1',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompDateTimeFromTo'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = '解凍開始日時'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '2',
	EDIT_TYPE = '1',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompDateTimeFromTo'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = '解凍完了日時'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '4',
	EDIT_TYPE = '2',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompTextBox'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = '原料登録NO'

    UPDATE
	[DEFINE_COMPONENT_COLUMNS]
	SET 
	EDIT_DIALOG_LAYOUT_DISP_ORDER = '3',
	EDIT_TYPE = '2',
	EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompTextBox'

  WHERE COMPONENT_NAME = 'AttributesGrid'
  AND CLASS_NAME = 'DepalletThawingStatus' 
  AND PROPERTY_KEY = '原料品目コード'

  --5_DEFINE_PROCESS_FUNCTION_デパレ解凍情報編集プログラム追加
 INSERT 
	INTO [DEFINE_PROCESS_FUNCTIONS]
	  ( [PROGRAM_NAME]
      ,[FUNCTION_NAME]
      ,[EXEC_ORDER_RANK]
      ,[ASSEMBLY_NAME]
      ,[CLASS_NAME]
      ,[FUNCTION_TYPE]
      ,[IS_FUNCTION_RETURN]
      ,[RETRUN_DATA_TYPE]
      ,[ARGUMENT_COUNT]
      ,[EXEC_TARGET_PATH]
      ,[IS_TRANSACTION]
      ,[ARGUMENT_NAME1]
      ,[ARGUMENT_TYPE_NAME1]
      ,[ARGUMENT_NAME2]
      ,[ARGUMENT_TYPE_NAME2]
      ,[ARGUMENT_NAME3]
      ,[ARGUMENT_TYPE_NAME3]
      ,[ARGUMENT_NAME4]
      ,[ARGUMENT_TYPE_NAME4]
      ,[ARGUMENT_NAME5]
      ,[ARGUMENT_TYPE_NAME5]
      ,[ARGUMENT_NAME6]
      ,[ARGUMENT_TYPE_NAME6]
      ,[ARGUMENT_NAME7]
      ,[ARGUMENT_TYPE_NAME7]
      ,[ARGUMENT_NAME8]
      ,[ARGUMENT_TYPE_NAME8]
      ,[ARGUMENT_NAME9]
      ,[ARGUMENT_TYPE_NAME9]
      ,[ARGUMENT_NAME10]
      ,[ARGUMENT_TYPE_NAME10]
      ,[ARGUMENT_NAME11]
      ,[ARGUMENT_TYPE_NAME11]
      ,[ARGUMENT_NAME12]
      ,[ARGUMENT_TYPE_NAME12]
      ,[ARGUMENT_NAME13]
      ,[ARGUMENT_TYPE_NAME13]
      ,[ARGUMENT_NAME14]
      ,[ARGUMENT_TYPE_NAME14]
      ,[ARGUMENT_NAME15]
      ,[ARGUMENT_TYPE_NAME15]
      ,[ARGUMENT_NAME16]
      ,[ARGUMENT_TYPE_NAME16]
      ,[ARGUMENT_NAME17]
      ,[ARGUMENT_TYPE_NAME17]
      ,[ARGUMENT_NAME18]
      ,[ARGUMENT_TYPE_NAME18]
      ,[ARGUMENT_NAME19]
      ,[ARGUMENT_TYPE_NAME19]
      ,[ARGUMENT_NAME20]
      ,[ARGUMENT_TYPE_NAME20]
      ,[ARGUMENT_NAME21]
      ,[ARGUMENT_TYPE_NAME21]
      ,[ARGUMENT_NAME22]
      ,[ARGUMENT_TYPE_NAME22]
      ,[ARGUMENT_NAME23]
      ,[ARGUMENT_TYPE_NAME23]
      ,[ARGUMENT_NAME24]
      ,[ARGUMENT_TYPE_NAME24]
      ,[ARGUMENT_NAME25]
      ,[ARGUMENT_TYPE_NAME25]
      ,[ARGUMENT_NAME26]
      ,[ARGUMENT_TYPE_NAME26]
      ,[ARGUMENT_NAME27]
      ,[ARGUMENT_TYPE_NAME27]
      ,[ARGUMENT_NAME28]
      ,[ARGUMENT_TYPE_NAME28]
      ,[ARGUMENT_NAME29]
      ,[ARGUMENT_TYPE_NAME29]
      ,[ARGUMENT_NAME30]
      ,[ARGUMENT_TYPE_NAME30])
  VALUES 
	('デパレ解凍情報編集'
      ,'upc_デパレ解凍情報_編集'
      ,'0'
      ,''
      ,''
      ,'2'
      ,'1'
      ,''
      ,'7'
      ,''
      ,'0'
      ,'DEVICE_ID'
      ,'System.String'
      ,'USER_ID'
      ,'System.String'
      ,'バッチNO'
      ,'System.String'
      ,'原料登録NO'
      ,'System.Int16'
      ,'デパレカウントNO'
      ,'System.Int16'
      ,'解凍開始日時'
      ,'System.String'
      ,'解凍完了日時'
      ,'System.String'
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,''
      ,'');

--6_DEFINE_PROCESS_PROGRAMS_デパレ解凍情報編集プログラム追加
INSERT 
	INTO [DEFINE_PROCESS_PROGRAMS]
	  ( [PROGRAM_NAME]
      ,[PROGRAM_TYPE]
      ,[IS_PROGRAM_RETURN]
      ,[RETRUN_DATA_TYPE]
      ,[IS_TRANSACTION]
      ,[FUNCTION_COUNT]
      ,[TIMEOUT_VALUE]
      ,[RETRY_COUNT]
      ,[SEMAPHORE_LOCK_COUNT]
      ,[SEMAPHORE_MAX_COUNT]
      ,[LOG_TYPE])
  VALUES 
	('デパレ解凍情報編集','0','FALSE','','TRUE','1','30000','0','1','-1','0');

--7_DEFINE_COMPONENTS_デパレ解凍情報_ソート順変更
UPDATE [DEFINE_COMPONENTS]
	SET ATTRIBUTES_NAME = 'バッチNO'
	WHERE CLASS_NAME = 'DepalletThawingStatus'
	AND COMPONENT_NAME = 'OrderByParam'
	AND VALUE = '0:DESC'

UPDATE [DEFINE_COMPONENTS]
	SET ATTRIBUTES_NAME = '原料登録NO',VALUE = '1:ASC'
	WHERE CLASS_NAME = 'DepalletThawingStatus'
	AND COMPONENT_NAME = 'OrderByParam'
	AND VALUE = '1:DESC'

INSERT INTO [DEFINE_COMPONENTS]
( [CLASS_NAME]
      ,[COMPONENT_NAME]
      ,[ATTRIBUTES_NAME]
      ,[VALUE]
      ,[VALUE_OBJECT_TYPE]
      ,[COMPONET_DATA_TYPE]
      ,[VALUE_MIN]
      ,[VALUE_MAX]
      ,[METHOD_NAME])
VALUES
(	   'DepalletThawingStatus'
      ,'OrderByParam'
      ,'デパレカウントNO'
      ,'2:ASC'
      ,'0'
      ,'System.String'
      ,NULL
      ,NULL
      ,'')

--DEFINE_COMPONENTS_デパレ解凍情報_解凍日時入力方法変更
UPDATE [MHM-WMS-DB].[dbo].[DEFINE_COMPONENT_COLUMNS]
SET EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompDateTimePicker'
WHERE CLASS_NAME = 'DepalletThawingStatus'
AND PROPERTY_KEY = '解凍開始日時'

UPDATE [MHM-WMS-DB].[dbo].[DEFINE_COMPONENT_COLUMNS]
SET EDIT_DATA_TYPE_KEY = 'ExpressionDBBlazorShared.Shared.CompDateTimePicker'
WHERE CLASS_NAME = 'DepalletThawingStatus'
AND PROPERTY_KEY = '解凍完了日時'

--1_DEFINE_COMPONENT_COLUMNS_棚状況追加
  INSERT INTO [MHM-WMS-DB].[dbo].[DEFINE_COMPONENT_COLUMNS]
				([CLASS_NAME]
      ,[COMPONENT_NAME]
      ,[VIEW_NAME]
      ,[COMPONET_DATA_TYPE]
      ,[VALUE_MIN]
      ,[VALUE_MAX]
      ,[PROPERTY_KEY]
      ,[WIDTH]
      ,[TEXT_ALIGN]
      ,[IS_RESIZABLE]
      ,[IS_REORDERABLE]
      ,[IS_SORTABLE]
      ,[IS_FILTERABLE]
      ,[FORMAT_STRING]
      ,[IS_EDIT]
      ,[IS_DATA_EXPORT]
      ,[IS_SEARCH_CONDITION]
      ,[IS_DATA_INPUT]
      ,[IS_SUMMARY]
      ,[SEARCH_VALUES_VIEW_NAME]
      ,[SEARCH_DATA_TYPE_KEY]
      ,[SEARCH_INPUT_REQUIRED]
      ,[ORDERBY_RANK]
      ,[SEARCH_LAYOUT_GROUP]
      ,[SEARCH_LAYOUT_DISP_ORDER]
      ,[EDIT_INPUT_REQUIRED]
      ,[REGULAR_EXPRESSION_STRING]
      ,[EDIT_DIALOG_LAYOUT_GROUP]
      ,[EDIT_DIALOG_LAYOUT_DISP_ORDER]
      ,[EDIT_TYPE]
      ,[EDIT_VAUES_VIEW_NAME]
      ,[EDIT_DATA_TYPE_KEY]
      ,[IS_INLINE_EDIT])
	  VALUES
	  ('MstAutomatedStorageRacks'
      ,'AttributesGrid'
      ,'VW_自動倉庫棚マスタ'
      ,'System.String'
      ,NULL
      ,NULL
      ,'棚状況'
      ,'0'
      ,'TextAlign.Left'
      ,'1'
      ,'1'
      ,'1'
      ,'1'
      ,''
      ,'1'
      ,'1'
      ,'0'
      ,'1'
      ,'0'
      ,''
      ,''
      ,'0'
      ,'999'
      ,'0'
      ,'0'
      ,'1'
      ,NULL
      ,'0'
      ,'1'
      ,'1'
      ,'VW_DROPDOWN_棚状況'
      ,'ExpressionDBBlazorShared.Shared.CompDropDown'
      ,'0'),
	  ('MstAutomatedStorageRacks'
      ,'AttributesGrid'
      ,'VW_自動倉庫棚マスタ'
      ,'System.String'
      ,NULL
      ,NULL
      ,'棚状況名'
      ,'150'
      ,'TextAlign.Left'
      ,'1'
      ,'1'
      ,'1'
      ,'1'
      ,''
      ,'1'
      ,'1'
      ,'1'
      ,'1'
      ,'0'
      ,''
      ,'ExpressionDBBlazorShared.Shared.CompTextBox'
      ,'0'
      ,'999'
      ,'2'
      ,'2'
      ,'0'
      ,NULL
      ,'0'
      ,'0'
      ,'0'
      ,''
      ,''
      ,'0')

--3_DEFINE_PROCESS_FUNCTION_自動倉庫棚マスタ編集_棚状況追加
UPDATE
	[MHM-WMS-DB].[dbo].[DEFINE_PROCESS_FUNCTIONS]
	SET [ARGUMENT_COUNT] = '8',
		[ARGUMENT_NAME8] = '棚状況',
		[ARGUMENT_TYPE_NAME8] = 'System.Int32'
	WHERE PROGRAM_NAME = '自動倉庫棚マスタ編集'
	
--6_DEFINE_COMPONENT_COLUMNS_編集不可設定
　UPDATE 
	  DEFINE_COMPONENT_COLUMNS
	SET EDIT_TYPE = '2'
  where CLASS_NAME = 'MstAutomatedStorageRacks'
  AND PROPERTY_KEY = '荷高区分'

    UPDATE 
	  DEFINE_COMPONENT_COLUMNS
	SET EDIT_TYPE = '2'
  where CLASS_NAME = 'MstAutomatedStorageRacks'
  AND PROPERTY_KEY = '禁止区分'

    UPDATE 
	  DEFINE_COMPONENT_COLUMNS
	SET EDIT_TYPE = '2'
  where CLASS_NAME = 'MstAutomatedStorageRacks'
  AND PROPERTY_KEY = '保管区分'

--DEFINE_COMPONENTS_編集時確認追加
insert into DEFINE_COMPONENTS (CLASS_NAME,	COMPONENT_NAME,	ATTRIBUTES_NAME,	VALUE,	VALUE_OBJECT_TYPE,	COMPONET_DATA_TYPE,	VALUE_MIN,	VALUE_MAX,	METHOD_NAME)
  values ('DialogPersonFixContent','F1ConfirmDialog','MessageContent','この操作は重要な変更を伴いますが、本当に実行してもよろしいですか？',0,'System.String',	NULL,	NULL,	NULL)

--DEFINE_LOCATION_STATUS_TYPES_.棚状況追加
INSERT INTO 
  [dbo].[DEFINE_LOCATION_STATUS_TYPES]
  (LOCATION_STATUS_TYPE,LOCATION_STATUS_TYPE_NAME)
  VALUES
  ('0','空棚'),
  ('1','実棚'),
  ('2','入庫予約棚')