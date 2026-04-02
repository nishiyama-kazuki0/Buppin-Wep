USE [MHM-WMS-DB]
GO

/****** Object:  View [dbo].[VW_デパレ解凍情報]    Script Date: 2025/09/16 17:24:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[VW_デパレ解凍情報]
AS
SELECT                      TOP (100) PERCENT バッチNO, dbo.FC_GET_DATE_FORM(バッチ開始年月日) AS バッチ開始年月日, 原料品目コード, 原料登録NO, デパレカウントNO, 
                                      dbo.FC_GET_DATE_FORM(デパレ完了日時) AS デパレ完了日時, 到着位置, dbo.FC_GET_DATE_FORM(解凍開始日時) AS 解凍開始日時, 
                                      dbo.FC_GET_DATE_FORM(解凍完了日時) AS 解凍完了日時, dbo.FC_GET_DATE_FORM(開梱受渡日時) AS 開梱受渡日時, dbo.FC_GET_DATE_FORM(実容器受取日時) 
                                      AS 実容器受取日時, 開梱受渡回数, 実容器受取回数, 専用容器QR1, dbo.FC_GET_DATE_FORM(投入セル受渡日時1) AS 投入セル受渡日時1, 専用容器QR2, 
                                      dbo.FC_GET_DATE_FORM(投入セル受渡日時2) AS 投入セル受渡日時2, バッチ管理ID, 生産タスク管理ID, 引当結果管理ID, 出庫予約管理ID
FROM                         dbo.DEPALLET_THAWING_STATUS AS D1
ORDER BY               バッチNO DESC, デパレカウントNO
GO

--EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
--Begin DesignProperties = 
--   Begin PaneConfigurations = 
--      Begin PaneConfiguration = 0
--         NumPanes = 4
--         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
--      End
--      Begin PaneConfiguration = 1
--         NumPanes = 3
--         Configuration = "(H (1 [50] 4 [25] 3))"
--      End
--      Begin PaneConfiguration = 2
--         NumPanes = 3
--         Configuration = "(H (1 [50] 2 [25] 3))"
--      End
--      Begin PaneConfiguration = 3
--         NumPanes = 3
--         Configuration = "(H (4 [30] 2 [40] 3))"
--      End
--      Begin PaneConfiguration = 4
--         NumPanes = 2
--         Configuration = "(H (1 [56] 3))"
--      End
--      Begin PaneConfiguration = 5
--         NumPanes = 2
--         Configuration = "(H (2 [66] 3))"
--      End
--      Begin PaneConfiguration = 6
--         NumPanes = 2
--         Configuration = "(H (4 [50] 3))"
--      End
--      Begin PaneConfiguration = 7
--         NumPanes = 1
--         Configuration = "(V (3))"
--      End
--      Begin PaneConfiguration = 8
--         NumPanes = 3
--         Configuration = "(H (1[56] 4[18] 2) )"
--      End
--      Begin PaneConfiguration = 9
--         NumPanes = 2
--         Configuration = "(H (1 [75] 4))"
--      End
--      Begin PaneConfiguration = 10
--         NumPanes = 2
--         Configuration = "(H (1[66] 2) )"
--      End
--      Begin PaneConfiguration = 11
--         NumPanes = 2
--         Configuration = "(H (4 [60] 2))"
--      End
--      Begin PaneConfiguration = 12
--         NumPanes = 1
--         Configuration = "(H (1) )"
--      End
--      Begin PaneConfiguration = 13
--         NumPanes = 1
--         Configuration = "(V (4))"
--      End
--      Begin PaneConfiguration = 14
--         NumPanes = 1
--         Configuration = "(V (2))"
--      End
--      ActivePaneConfig = 0
--   End
--   Begin DiagramPane = 
--      Begin Origin = 
--         Top = 0
--         Left = 0
--      End
--      Begin Tables = 
--         Begin Table = "D1"
--            Begin Extent = 
--               Top = 6
--               Left = 38
--               Bottom = 136
--               Right = 229
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--      End
--   End
--   Begin SQLPane = 
--   End
--   Begin DataPane = 
--      Begin ParameterDefaults = ""
--      End
--   End
--   Begin CriteriaPane = 
--      Begin ColumnWidths = 11
--         Column = 1440
--         Alias = 900
--         Table = 1170
--         Output = 720
--         Append = 1400
--         NewValue = 1170
--         SortType = 1350
--         SortOrder = 1410
--         GroupBy = 1350
--         Filter = 1350
--         Or = 1350
--         Or = 1350
--         Or = 1350
--      End
--   End
--End
--' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_デパレ解凍情報'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_デパレ解凍情報'
--GO


