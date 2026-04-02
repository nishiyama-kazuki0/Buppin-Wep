USE [MHM-WMS-DB]
GO

/****** Object:  View [dbo].[VW_自動倉庫棚マスタ]    Script Date: 2025/09/16 11:20:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*----------------------------------------------------------------------------- 
----------------------------------------------------------------------------- 
  VW_自動倉庫棚マスタ
    [作成] 
        2023/05/02 5:00:00  
 
    [更新] 
        2024/11/11 15:00:00  (テーブルから値を取得するように変更、WHERE句に自動倉庫の条件を追加)
    [備考] 
     
----------------------------------------------------------------------------- 
-----------------------------------------------------------------------------
 自動倉庫*/
ALTER VIEW [dbo].[VW_自動倉庫棚マスタ]
AS
SELECT                      M2.保管区分, D1.保管区分名 AS 倉庫, dbo.FC_GET_LOCATION_FORM(M1.LOCATION_ID) AS 棚番号, M1.LOAD_HIGHT_TYPE AS 荷高区分, 
                                      D2.LOAD_HIGHT_TYPE_NAME AS 荷高区分名, M1.LOCATION_TYPE AS 禁止区分, D3.LOCATION_TYPE_NAME AS 禁止区分名, M1.優先保管種別, D4.優先保管種別名, 
                                      D5.LOCATION_STATUS_TYPE AS 棚状況, D5.LOCATION_STATUS_TYPE_NAME AS 棚状況名
FROM                         dbo.MST_LOCATIONS AS M1 LEFT OUTER JOIN
                                      dbo.MST_ZONE AS M2 ON M1.AREA_ID = M2.AREA_ID AND M1.ZONE_ID = M2.ZONE_ID LEFT OUTER JOIN
                                      dbo.DEFINE_STORAGE_TYPES AS D1 ON M2.保管区分 = D1.保管区分 LEFT OUTER JOIN
                                      dbo.DEFINE_LOAD_HIGHT_TYPES AS D2 ON M1.LOAD_HIGHT_TYPE = D2.LOAD_HIGHT_TYPE LEFT OUTER JOIN
                                      dbo.DEFINE_LOCATION_TYPES AS D3 ON M1.LOCATION_TYPE = D3.LOCATION_TYPE LEFT OUTER JOIN
                                      dbo.DEFINE_PREFERRED_STORAGE_TYPES AS D4 ON M1.優先保管種別 = D4.優先保管種別 LEFT OUTER JOIN
                                      dbo.DEFINE_LOCATION_STATUS_TYPES AS D5 ON M1.LOCATION_STATUS = D5.LOCATION_STATUS_TYPE
WHERE                       (M1.ZONE_ID = 'DA') OR
                                      (M1.ZONE_ID = 'FA')
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
--         Begin Table = "M1"
--            Begin Extent = 
--               Top = 6
--               Left = 38
--               Bottom = 136
--               Right = 229
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "M2"
--            Begin Extent = 
--               Top = 6
--               Left = 267
--               Bottom = 136
--               Right = 465
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "D1"
--            Begin Extent = 
--               Top = 138
--               Left = 38
--               Bottom = 234
--               Right = 194
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "D2"
--            Begin Extent = 
--               Top = 138
--               Left = 232
--               Bottom = 234
--               Right = 462
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "D3"
--            Begin Extent = 
--               Top = 234
--               Left = 38
--               Bottom = 364
--               Right = 254
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "D4"
--            Begin Extent = 
--               Top = 234
--               Left = 292
--               Bottom = 330
--               Right = 465
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         End
--         Begin Table = "D5"
--            Begin Extent = 
--               Top = 6
--               Left = 503
--               Bottom = 102
--               Right = 764
--            End
--            DisplayFlags = 280
--            TopColumn = 0
--         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_自動倉庫棚マスタ'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'End
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
--' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_自動倉庫棚マスタ'
--GO

--EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'VW_自動倉庫棚マスタ'
--GO


