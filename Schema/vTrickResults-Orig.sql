USE [LiveWebScoreboard]
GO

/****** Object:  View [dbo].[vTrickResults]    Script Date: 2/5/2024 12:11:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[vTrickResults]
AS
SELECT TR.SanctionId, TR.SkierName, TR.MemberId, TR.AgeGroup, TR.Gender, TS.NopsScore, TS.Score AS EventScore, ER.TeamCode, TS.Round, 'Trick' AS Event, TS.EventClass, TS.InsertDate, TS.LastUpdateDate, 
                  TS.LastUpdateDate AS SortLastUpdateDate, CAST(TS.Score AS CHAR) + ' POINTS (P1:' + CAST(TS.ScorePass1 AS CHAR) + ' P2:' + CAST(TS.ScorePass2 AS CHAR) + ')' AS EventScoreDesc, TV.Pass1VideoUrl, TV.Pass2VideoUrl
FROM     dbo.TourReg AS TR INNER JOIN
                  dbo.TrickScore AS TS ON TS.MemberId = TR.MemberId AND TS.SanctionId = TR.SanctionId AND TS.AgeGroup = TR.AgeGroup INNER JOIN
                  dbo.EventReg AS ER ON ER.MemberId = TR.MemberId AND ER.SanctionId = TR.SanctionId AND ER.AgeGroup = TR.AgeGroup AND ER.Event = 'Trick' LEFT OUTER JOIN
                  dbo.TrickVideo AS TV ON TV.MemberId = TR.MemberId AND TV.SanctionId = TR.SanctionId AND TV.AgeGroup = TR.AgeGroup AND TV.Round = TS.Round
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "TR"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 293
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TS"
            Begin Extent = 
               Top = 7
               Left = 341
               Bottom = 170
               Right = 543
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "ER"
            Begin Extent = 
               Top = 7
               Left = 591
               Bottom = 170
               Right = 793
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "TV"
            Begin Extent = 
               Top = 7
               Left = 841
               Bottom = 170
               Right = 1043
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vTrickResults'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vTrickResults'
GO


