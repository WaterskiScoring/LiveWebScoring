Use LiveWebScoreboard
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID ('dbo.PublishReport') IS NOT NULL
	DROP TABLE dbo.PublishReport
GO
CREATE TABLE PublishReport (
  PK bigint IDENTITY (1,1) NOT NULL,
  SanctionId char(6) NOT NULL,
  ReportType varchar(12) NOT NULL,
  Event varchar(12) NOT NULL,
  ReportTitle varchar(256) NOT NULL,
  ReportFilePath varchar(1024) NOT NULL,
  LastUpdateDate datetime NOT NULL
, CONSTRAINT [PublishReport_PKIndex] PRIMARY KEY NONCLUSTERED (	SanctionId ASC, ReportType ASC, Event ASC, ReportTitle ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX PublishReport_PK ON dbo.PublishReport (PK ASC);
GO


IF OBJECT_ID ('dbo.Tournament') IS NOT NULL
	DROP TABLE dbo.Tournament
GO
CREATE TABLE [Tournament] (
  [SanctionId] nchar(6) NOT NULL
, [Name] nvarchar(128) NULL
, [Class] nchar(1) NULL
, [Federation] nvarchar(12) NULL
, [SlalomRounds] tinyint NULL
, [TrickRounds] tinyint NULL
, [JumpRounds] tinyint NULL
, [Rules] nvarchar(16) NULL
, [EventDates] nvarchar(128) NULL
, [TourDataLoc] nvarchar(256) NULL
, [EventLocation] nvarchar(1024) NULL
, [HcapSlalomBase] numeric(9,3) NULL
, [HcapTrickBase] numeric(9,3) NULL
, [HcapJumpBase] numeric(9,3) NULL
, [HcapSlalomPct] numeric(9,3) NULL
, [HcapTrickPct] numeric(9,3) NULL
, [HcapJumpPct] numeric(9,3) NULL
, [ContactMemberId] nchar(9) NULL
, [ContactPhone] nvarchar(128) NULL
, [ContactEmail] nvarchar(128) NULL
, [RuleExceptions] nvarchar(1024) NULL
, [RuleInterpretations] nvarchar(1024) NULL
, [SafetyDirPerfReport] nvarchar(1024) NULL
, [RopeHandlesSpecs] nvarchar(1024) NULL
, [SlalomRopesSpecs] nvarchar(1024) NULL
, [JumpRopesSpecs] nvarchar(1024) NULL
, [SlalomCourseSpecs] nvarchar(1024) NULL
, [JumpCourseSpecs] nvarchar(1024) NULL
, [TrickCourseSpecs] nvarchar(1024) NULL
, [BuoySpecs] nvarchar(1024) NULL
, [RuleExceptQ1] nvarchar(128) NULL
, [RuleExceptQ2] nvarchar(128) NULL
, [RuleExceptQ3] nchar(1) NULL
, [RuleExceptQ4] nchar(1) NULL
, [RuleInterQ1] nvarchar(128) NULL
, [RuleInterQ2] nvarchar(128) NULL
, [RuleInterQ3] nchar(1) NULL
, [RuleInterQ4] nchar(1) NULL
, [ContactAddress] nvarchar(128) NULL
, [ChiefJudgeMemberId] nchar(9) NULL
, [ChiefJudgeAddress] nvarchar(128) NULL
, [ChiefJudgePhone] nvarchar(128) NULL
, [ChiefJudgeEmail] nvarchar(128) NULL
, [ChiefDriverMemberId] nchar(9) NULL
, [ChiefDriverAddress] nvarchar(128) NULL
, [ChiefDriverPhone] nvarchar(128) NULL
, [ChiefDriverEmail] nvarchar(128) NULL
, [SafetyDirMemberId] nchar(9) NULL
, [SafetyDirAddress] nvarchar(128) NULL
, [SafetyDirPhone] nvarchar(128) NULL
, [SafetyDirEmail] nvarchar(128) NULL
, [LastUpdateDate] datetime NULL
, [PlcmtMethod] nvarchar(256) NULL
, [OverallMethod] nvarchar(256) NULL
, [ChiefScorerMemberId] nchar(9) NULL
, [ChiefScorerAddress] nvarchar(128) NULL
, [ChiefScorerPhone] nvarchar(128) NULL
, [ChiefScorerEmail] nvarchar(128) NULL
, [SanctionEditCode] nvarchar(32) NULL
, CONSTRAINT [Tournament_PK] PRIMARY KEY NONCLUSTERED (	[SanctionId] ASC) 
);
GO

IF OBJECT_ID ('dbo.TourReg') IS NOT NULL
	DROP TABLE dbo.TourReg
GO
CREATE TABLE [TourReg] (
  [PK] bigint IDENTITY (4762,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [SkierName] nvarchar(128) NULL
, [EntryDue] money NULL
, [EntryPaid] money NULL
, [PaymentMethod] nvarchar(64) NULL
, [ReadyToSki] nchar(1) NULL
, [ReadyForPlcmt] nchar(1) NULL
, [AwsaMbrshpPaymt] money NULL
, [AwsaMbrshpComment] nvarchar(256) NULL
, [Weight] smallint NULL
, [TrickBoat] nvarchar(64) NULL
, [JumpHeight] numeric(3,1) NULL
, [Federation] nvarchar(12) NULL
, [ForeignFederationID] nchar(12) NULL
, [Gender] nchar(1) NULL
, [SkiYearAge] tinyint NULL
, [State] nchar(2) NULL
, [LastUpdateDate] datetime NULL
, [Notes] nvarchar(1024) NULL
, [City] nvarchar(128) NULL
, [Withdrawn] nchar(1) NULL
, [IwwfLicense] nchar(1) NULL
, [SlalomClassReg] nvarchar(16) NULL
, [TrickClassReg] nvarchar(16) NULL
, [JumpClassReg] nvarchar(16) NULL
, [Team] nvarchar(16) NULL
, CONSTRAINT [TourRegPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TourReg_PK ON dbo.TourReg ([PK] DESC);
GO

IF OBJECT_ID ('dbo.EventReg') IS NOT NULL
	DROP TABLE dbo.EventReg
GO
CREATE TABLE [EventReg] (
  [PK] bigint IDENTITY (6706,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [EventGroup] nvarchar(12) NULL
, [RunOrder] smallint NULL
, [TeamCode] nvarchar(12) NULL
, [EventClass] nchar(1) NULL
, [RankingScore] numeric(9,3) NULL
, [RankingRating] nvarchar(12) NULL
, [HCapBase] numeric(9,3) NULL
, [HCapScore] numeric(9,3) NULL
, [Rotation] smallint NULL
, [ReadyForPlcmt] nchar(1) NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [EventRegPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Event] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX EventReg_PK ON dbo.EventReg ([PK] ASC);
GO

IF OBJECT_ID ('dbo.SlalomScore') IS NOT NULL
	DROP TABLE dbo.SlalomScore
GO
CREATE TABLE [SlalomScore] (
  [PK] bigint IDENTITY (1556,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [MaxSpeed] tinyint NULL
, [StartSpeed] tinyint NULL
, [StartLen] nvarchar(6) NULL
, [Score] numeric(5,2) NULL
, [NopsScore] numeric(7,2) NULL
, [Rating] nvarchar(16) NULL
, [Status] nvarchar(16) NULL
, [FinalSpeedMph] tinyint NULL
, [FinalSpeedKph] tinyint NULL
, [FinalLen] nvarchar(16) NULL
, [FinalLenOff] nvarchar(16) NULL
, [FinalPassScore] numeric(5,2) NULL
, [Boat] nvarchar(32) NULL
, [EventClass] nchar(1) NULL
, [CompletedSpeedMph] tinyint NULL
, [CompletedSpeedKph] tinyint NULL
, [Note] nvarchar(1024) NULL
, [InsertDate] datetime NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [SlalomScorePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX SlalomScore_PK ON dbo.SlalomScore ([PK] ASC);
GO

IF OBJECT_ID ('dbo.SlalomRecap') IS NOT NULL
	DROP TABLE dbo.SlalomRecap
GO
CREATE TABLE [SlalomRecap] (
  [PK] bigint IDENTITY (4562,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [SkierRunNum] smallint NOT NULL
, [Judge1Score] numeric(5,2) NULL
, [Judge2Score] numeric(5,2) NULL
, [Judge3Score] numeric(5,2) NULL
, [Judge4Score] numeric(5,2) NULL
, [Judge5Score] numeric(5,2) NULL
, [EntryGate1] bit NULL
, [EntryGate2] bit NULL
, [EntryGate3] bit NULL
, [ExitGate1] bit NULL
, [ExitGate2] bit NULL
, [ExitGate3] bit NULL
, [BoatTime] numeric(5,2) NULL
, [Score] numeric(5,2) NULL
, [TimeInTol] nchar(1) NULL
, [ScoreProt] nchar(1) NULL
, [Reride] nchar(1) NULL
, [RerideReason] nvarchar(256) NULL
, [LastUpdateDate] datetime NULL
, [Note] nvarchar(1024) NULL
, [PassLineLength] numeric(5,2) NULL
, [PassSpeedKph] tinyint NULL
, [InsertDate] datetime NULL
, [ProtectedScore] numeric(5,2) NULL
, [BoatPathGood] nchar(1) NULL
, CONSTRAINT [SlalomRecapPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC, [SkierRunNum] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX SlalomRecap_PK ON dbo.SlalomRecap ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TrickScore') IS NOT NULL
	DROP TABLE dbo.TrickScore
GO
CREATE TABLE [TrickScore] (
  [PK] bigint IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [Score] smallint NULL
, [ScorePass1] smallint NULL
, [ScorePass2] smallint NULL
, [NopsScore] numeric(7,2) NULL
, [Rating] nvarchar(16) NULL
, [Status] nvarchar(16) NULL
, [Boat] nvarchar(32) NULL
, [Note] nvarchar(1024) NULL
, [EventClass] nchar(1) NULL
, [InsertDate] datetime NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [TrickScorePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TrickScore_PK ON dbo.TrickScore ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TrickPass') IS NOT NULL
	DROP TABLE dbo.TrickPass
GO
CREATE TABLE [TrickPass] (
  [PK] bigint IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [PassNum] tinyint NOT NULL
, [Skis] tinyint NOT NULL
, [Seq] tinyint NOT NULL
, [Score] smallint NULL
, [Code] nvarchar(16) NULL
, [Results] nvarchar(12) NULL
, [Note] nvarchar(1024) NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [TrickPassPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC, [PassNum] ASC, [Seq] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TrickPass_PK ON dbo.TrickPass ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TrickVideo') IS NOT NULL
	DROP TABLE dbo.TrickVideo
GO
CREATE TABLE [TrickVideo] (
  [PK] bigint IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [Pass1VideoUrl] nvarchar(256) NULL
, [Pass2VideoUrl] nvarchar(256) NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [TrickVideoPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TrickVideo_PK ON dbo.TrickVideo ([PK] ASC);
GO

IF OBJECT_ID ('dbo.JumpScore') IS NOT NULL
	DROP TABLE dbo.JumpScore
GO
CREATE TABLE [JumpScore] (
  [PK] bigint IDENTITY (241,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [BoatSpeed] tinyint NULL
, [RampHeight] numeric(3,1) NULL
, [ScoreFeet] numeric(4,1) NULL
, [ScoreMeters] numeric(5,2) NULL
, [NopsScore] numeric(7,2) NULL
, [Rating] nvarchar(16) NULL
, [Status] nvarchar(16) NULL
, [Boat] nvarchar(32) NULL
, [Note] nvarchar(1024) NULL
, [EventClass] nchar(1) NULL
, [LastUpdateDate] datetime NULL
, [InsertDate] datetime NULL
, CONSTRAINT [JumpScorePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX JumpScore_PK ON dbo.JumpScore ([PK] ASC);
GO

IF OBJECT_ID ('dbo.JumpRecap') IS NOT NULL
	DROP TABLE dbo.JumpRecap
GO
CREATE TABLE [JumpRecap] (
  [PK] bigint IDENTITY (713,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [PassNum] tinyint NOT NULL
, [Reride] nchar(1) NULL
, [BoatSpeed] tinyint NULL
, [RampHeight] numeric(3,1) NULL
, [Meter1] numeric(5,2) NULL
, [Meter2] numeric(5,2) NULL
, [Meter3] numeric(5,2) NULL
, [Meter4] numeric(5,2) NULL
, [Meter5] numeric(5,2) NULL
, [Meter6] numeric(5,2) NULL
, [ScoreFeet] numeric(5,2) NULL
, [ScoreMeters] numeric(5,2) NULL
, [ScoreTriangle] numeric(5,2) NULL
, [BoatSplitTime] numeric(6,3) NULL
, [BoatSplitTime2] numeric(6,3) NULL
, [BoatEndTime] numeric(6,3) NULL
, [TimeInTol] nchar(1) NULL
, [ScoreProt] nchar(1) NULL
, [ReturnToBase] nchar(1) NULL
, [Results] nvarchar(12) NULL
, [RerideReason] nvarchar(256) NULL
, [Split52TimeTol] smallint NULL
, [Split82TimeTol] smallint NULL
, [Split41TimeTol] smallint NULL
, [SkierBoatPath] nchar(6) NULL
, [RerideIfBest] nchar(1) NULL
, [RerideCanImprove] nchar(1) NULL
, [Note] nvarchar(1024) NULL
, [LastUpdateDate] datetime NULL
, [InsertDate] datetime NULL
, CONSTRAINT [JumpRecapPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Round] ASC, [PassNum] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX JumpRecap_PK ON dbo.JumpRecap ([PK] ASC);
GO

IF OBJECT_ID ('dbo.JumpVideoSetup') IS NOT NULL
	DROP TABLE dbo.JumpVideoSetup
GO
CREATE TABLE [JumpVideoSetup] (
  [PK] bigint IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [XCoordScrnUpperLeft] numeric(7,3) NULL
, [YCoordScrnUpperLeft] numeric(7,3) NULL
, [XCoordScrnUpperRight] numeric(7,3) NULL
, [YCoordScrnUpperRight] numeric(7,3) NULL
, [XCoordScrnLowerLeft] numeric(7,3) NULL
, [YCoordScrnLowerLeft] numeric(7,3) NULL
, [XCoordScrnLowerRight] numeric(7,3) NULL
, [YCoordScrnLowerRight] numeric(7,3) NULL
, [XCoordScrnCheckBuoy] numeric(7,3) NULL
, [YCoordScrnCheckBuoy] numeric(7,3) NULL
, [XCoordSurveyUpperLeft] numeric(7,3) NULL
, [YCoordSurveyUpperLeft] numeric(7,3) NULL
, [XCoordSurveyUpperRight] numeric(7,3) NULL
, [YCoordSurveyUpperRight] numeric(7,3) NULL
, [XCoordSurveyLowerLeft] numeric(7,3) NULL
, [YCoordSurveyLowerLeft] numeric(7,3) NULL
, [XCoordSurveyLowerRight] numeric(7,3) NULL
, [YCoordSurveyLowerRight] numeric(7,3) NULL
, [XCoordSurveyCheckBuoy] numeric(7,3) NULL
, [YCoordSurveyCheckBuoy] numeric(7,3) NULL
, CONSTRAINT [JumpVideoSetupPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX JumpVideoSetup_PK ON dbo.JumpVideoSetup ([PK] ASC);
GO

IF OBJECT_ID ('dbo.JumpMeterSetup') IS NOT NULL
	DROP TABLE dbo.JumpMeterSetup
GO
CREATE TABLE [JumpMeterSetup] (
  [PK] bigint IDENTITY (5,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [AngleAtoB] numeric(5,2) NULL
, [AngleBtoA] numeric(5,2) NULL
, [AngleAtoC] numeric(5,2) NULL
, [AngleCtoA] numeric(5,2) NULL
, [AngleBtoC] numeric(5,2) NULL
, [AngleCtoB] numeric(5,2) NULL
, [AngleAtoZ] numeric(5,2) NULL
, [AngleBtoZ] numeric(5,2) NULL
, [AngleCtoZ] numeric(5,2) NULL
, [DistAtoB] numeric(5,2) NULL
, [DistBtoC] numeric(5,2) NULL
, [DistAtoC] numeric(5,2) NULL
, [DistTo15ET] numeric(5,2) NULL
, [AngleAto15ET] numeric(5,2) NULL
, [AngleBto15ET] numeric(5,2) NULL
, [AngleCto15ET] numeric(5,2) NULL
, [XCoordZero] numeric(5,2) NULL
, [YCoordZero] numeric(5,2) NULL
, [XCoord15ET] numeric(5,2) NULL
, [YCoord15ET] numeric(5,2) NULL
, [TriangleZero] numeric(7,3) NULL
, [Triangle15ET] numeric(7,3) NULL
, [TriangleMax] numeric(7,3) NULL
, [TriangleMaxZero] numeric(7,3) NULL
, [JumpDirection] tinyint NULL
, CONSTRAINT [JumpMeterSetupPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX JumpMeterSetup_PK ON dbo.JumpMeterSetup ([PK] ASC);
GO

IF OBJECT_ID ('dbo.WscMonitor') IS NOT NULL
	DROP TABLE dbo.WscMonitor
GO
CREATE TABLE [WscMonitor] (
  [SanctionId] nchar(6) NOT NULL
, [MonitorName] nvarchar(32) NOT NULL
, [HeartBeat] datetime NULL
, CONSTRAINT [WscMonitorPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC) 
)
GO

IF OBJECT_ID ('dbo.WscMonitorMsg') IS NOT NULL
	DROP TABLE dbo.WscMonitorMsg
GO
CREATE TABLE [WscMonitorMsg] (
  [PK] int IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MsgAction] nvarchar(16) NOT NULL
, [MsgType] nvarchar(128) NOT NULL
, [MsgData] nvarchar(128) NOT NULL
, [InsertDate] datetime NULL
, CONSTRAINT [WscMonitorMsgPKIndex] PRIMARY KEY NONCLUSTERED ([PK] ASC) 
)
GO

IF OBJECT_ID ('dbo.WscMsgSend') IS NOT NULL
	DROP TABLE dbo.WscMsgSend
GO
CREATE TABLE [WscMsgSend] (
  [PK] int IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MsgType] nvarchar(128) NOT NULL
, [MsgData] ntext NOT NULL
, [CreateDate] datetime NULL
, CONSTRAINT [WscMsgSendPKIndex] PRIMARY KEY NONCLUSTERED ([PK] ASC) 
)
GO

IF OBJECT_ID ('dbo.WscMsgListen') IS NOT NULL
	DROP TABLE dbo.WscMsgListen
GO
CREATE TABLE [WscMsgListen] (
  [PK] int IDENTITY (1,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MsgType] nvarchar(128) NOT NULL
, [MsgData] ntext NOT NULL
, [CreateDate] datetime NULL
, CONSTRAINT [WscMsgListenPKIndex] PRIMARY KEY NONCLUSTERED ([PK] ASC) 
)
GO

IF OBJECT_ID ('dbo.JumpMeasurement') IS NOT NULL
	DROP TABLE dbo.JumpMeasurement
GO
CREATE TABLE [JumpMeasurement] (
  [PK] bigint IDENTITY (109,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [PassNumber] tinyint NOT NULL
, [ScoreFeet] numeric(5,2) NULL
, [ScoreMeters] numeric(5,2) NULL
, [InsertDate] datetime NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [JumpMeasurementPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [Event] ASC, [Round] ASC, [PassNumber] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX JumpMeasurement_PK ON dbo.JumpMeasurement ([PK] ASC);
GO

IF OBJECT_ID ('dbo.BoatTime') IS NOT NULL
	DROP TABLE dbo.BoatTime
GO
CREATE TABLE [BoatTime] (
  [PK] bigint IDENTITY (2214,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [PassNumber] tinyint NOT NULL
, [PassLineLength] numeric(5,2) NULL
, [PassSpeedKph] tinyint NULL
, [BoatTimeBuoy1] numeric(5,2) NULL
, [BoatTimeBuoy2] numeric(5,2) NULL
, [BoatTimeBuoy3] numeric(5,2) NULL
, [BoatTimeBuoy4] numeric(5,2) NULL
, [BoatTimeBuoy5] numeric(5,2) NULL
, [BoatTimeBuoy6] numeric(5,2) NULL
, [BoatTimeBuoy7] numeric(5,2) NULL
, [InsertDate] datetime NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [BoatTimePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [Event] ASC, [Round] ASC, [PassNumber] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX BoatTime_PK ON dbo.BoatTime ([PK] ASC);
GO

IF OBJECT_ID ('dbo.BoatPath') IS NOT NULL
	DROP TABLE dbo.BoatPath
GO
CREATE TABLE [BoatPath] (
  [PK] bigint IDENTITY (2199,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [PassNumber] tinyint NOT NULL
, [PassLineLength] numeric(5,2) NULL
, [PassSpeedKph] tinyint NULL
, [PathDevBuoy0] numeric(5,2) NULL
, [PathDevBuoy1] numeric(5,2) NULL
, [PathDevBuoy2] numeric(5,2) NULL
, [PathDevBuoy3] numeric(5,2) NULL
, [PathDevBuoy4] numeric(5,2) NULL
, [PathDevBuoy5] numeric(5,2) NULL
, [PathDevBuoy6] numeric(5,2) NULL
, [PathDevCum0] numeric(5,2) NULL
, [PathDevCum1] numeric(5,2) NULL
, [PathDevCum2] numeric(5,2) NULL
, [PathDevCum3] numeric(5,2) NULL
, [PathDevCum4] numeric(5,2) NULL
, [PathDevCum5] numeric(5,2) NULL
, [PathDevCum6] numeric(5,2) NULL
, [PathDevZone0] numeric(5,2) NULL
, [PathDevZone1] numeric(5,2) NULL
, [PathDevZone2] numeric(5,2) NULL
, [PathDevZone3] numeric(5,2) NULL
, [PathDevZone4] numeric(5,2) NULL
, [PathDevZone5] numeric(5,2) NULL
, [PathDevZone6] numeric(5,2) NULL
, [RerideNote] nvarchar(64) NULL
, [InsertDate] datetime NULL
, [LastUpdateDate] datetime NULL
, [DriverMemberId] nchar(9) NULL
, [DriverName] nchar(64) NULL
, [BoatDescription] nchar(256) NULL
, [homologation] nchar(1) NULL
, CONSTRAINT [BoatPathPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [Event] ASC, [Round] ASC, [PassNumber] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX BoatPath_PK ON dbo.BoatPath ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TourProperties') IS NOT NULL
	DROP TABLE TourProperties
GO
CREATE TABLE [TourProperties] (
  [PK] bigint IDENTITY (1432,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [PropKey] nvarchar(1024) NOT NULL
, [PropOrder] smallint NULL
, [PropValue] nvarchar(1024) NULL
, CONSTRAINT [TourPropertiesPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [PropKey] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TourProperties_PK ON dbo.TourProperties ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TourBoatUse') IS NOT NULL
	DROP TABLE TourBoatUse
GO
CREATE TABLE [TourBoatUse] (
  [PK] bigint IDENTITY (47,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [TourBoatSeq] smallint NOT NULL
, [HullId] nvarchar(16) NULL
, [SlalomUsed] nchar(1) NULL
, [TrickUsed] nchar(1) NULL
, [JumpUsed] nchar(1) NULL
, [SlalomCredit] nchar(1) NULL
, [TrickCredit] nchar(1) NULL
, [JumpCredit] nchar(1) NULL
, [CertOfInsurance] nchar(1) NULL
, [ModelYear] smallint NULL
, [BoatModel] nvarchar(128) NULL
, [SpeedControlVersion] nvarchar(128) NULL
, [Owner] nvarchar(128) NULL
, [InsuranceCompany] nvarchar(128) NULL
, [LastUpdateDate] datetime NULL
, [PreEventNotes] nvarchar(1024) NULL
, [PostEventNotes] nvarchar(1024) NULL
, [Notes] nvarchar(1024) NULL
, CONSTRAINT [TourBoatUsePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [TourBoatSeq] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TourBoatUse_PK ON dbo.TourBoatUse ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TeamList') IS NOT NULL
	DROP TABLE TeamList
GO
CREATE TABLE [TeamList] (
  [PK] bigint IDENTITY (79,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [TeamCode] nvarchar(12) NOT NULL
, [Name] nvarchar(64) NULL
, [ContactName] nvarchar(128) NULL
, [ContactInfo] nvarchar(128) NULL
, [LastUpdateDate] datetime NULL
, [Notes] nvarchar(1024) NULL
, CONSTRAINT [TeamListPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [TeamCode] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TeamList_PK ON dbo.TeamList ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TeamOrder') IS NOT NULL
	DROP TABLE TeamOrder
GO
CREATE TABLE [TeamOrder] (
  [PK] bigint IDENTITY (161,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [TeamCode] nvarchar(12) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [EventGroup] nvarchar(12) NULL
, [SlalomRunOrder] smallint NULL
, [TrickRunOrder] smallint NULL
, [JumpRunOrder] smallint NULL
, [SeedingScore] numeric(7,1) NULL
, [LastUpdateDate] datetime NULL
, [Notes] nvarchar(1024) NULL
, CONSTRAINT [TeamOrderPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [TeamCode] ASC, [AgeGroup] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TeamOrder_PK ON dbo.TeamOrder ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TeamScore') IS NOT NULL
	DROP TABLE TeamScore
GO
CREATE TABLE [TeamScore] (
  [PK] bigint IDENTITY (79,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [TeamCode] nvarchar(12) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Name] nvarchar(64) NULL
, [ReportFormat] nvarchar(16) NOT NULL
, [OverallPlcmt] nvarchar(8) DEFAULT NULL
, [SlalomPlcmt] nvarchar(8) DEFAULT NULL
, [TrickPlcmt] nvarchar(8) DEFAULT NULL
, [JumpPlcmt] nvarchar(8) DEFAULT NULL
, [OverallScore] numeric(7,2) DEFAULT NULL
, [SlalomScore] numeric(7,2) DEFAULT NULL
, [TrickScore] numeric(7,2) DEFAULT NULL
, [JumpScore] numeric(7,2) DEFAULT NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [TeamScorePKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [TeamCode] ASC, [AgeGroup] ASC, [ReportFormat] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TeamScore_PK ON dbo.TeamScore ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TeamScoreDetail') IS NOT NULL
	DROP TABLE TeamScoreDetail
GO
CREATE TABLE [TeamScoreDetail] (
  [PK] bigint IDENTITY (79,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [TeamCode] nvarchar(12) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [SkierCategory] nvarchar(12) DEFAULT NULL
, [LineNum] smallint NOT NULL
, [SlalomSkierName] nvarchar(64) DEFAULT NULL
, [SlalomPlcmt] nvarchar(8) DEFAULT NULL
, [SlalomScore] numeric(7,2) DEFAULT NULL
, [SlalomNops] numeric(7,2) DEFAULT NULL
, [SlalomPoints] numeric(7,2) DEFAULT NULL
, [TrickSkierName] nvarchar(64) DEFAULT NULL
, [TrickPlcmt] nvarchar(8) DEFAULT NULL
, [TrickScore] int DEFAULT NULL
, [TrickNops] numeric(7,2) DEFAULT NULL
, [TrickPoints] numeric(7,2) DEFAULT NULL
, [JumpSkierName] nvarchar(64) DEFAULT NULL
, [JumpPlcmt] nvarchar(8) DEFAULT NULL
, [JumpScore] numeric(7,2) DEFAULT NULL
, [JumpNops] numeric(7,2) DEFAULT NULL
, [JumpPoints] numeric(7,2) DEFAULT NULL
, CONSTRAINT [TeamScoreDetailPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [TeamCode] ASC, [AgeGroup] ASC, [LineNum] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TeamScoreDetail_PK ON dbo.TeamScoreDetail ([PK] ASC);
GO



IF OBJECT_ID ('dbo.SafetyCheckList') IS NOT NULL
	DROP TABLE SafetyCheckList
GO
CREATE TABLE [SafetyCheckList] (
  [PK] bigint IDENTITY (17,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [SponsorClubName] nvarchar(128) NULL
, [NumInjuries] tinyint NULL
, [MedAccptCheck] nchar(1) NULL
, [CommAvail] nchar(1) NULL
, [MedAvail] nchar(1) NULL
, [PostedMedRoute] nchar(1) NULL
, [HazFree] nchar(1) NULL
, [ObstructionsMarked] nchar(1) NULL
, [LandingClear] nchar(1) NULL
, [DockChecked] nchar(1) NULL
, [JumpInspect] nchar(1) NULL
, [JumpSecure] nchar(1) NULL
, [JumpSurfaceSafe] nchar(1) NULL
, [JumpColor] nchar(1) NULL
, [JumpAlgaeRemoved] nchar(1) NULL
, [CourseSafeDist] nchar(1) NULL
, [TowerStable] nchar(1) NULL
, [TowerLadderSafe] nchar(1) NULL
, [TowerFloorSafe] nchar(1) NULL
, [RefuelFireExtn] nchar(1) NULL
, [RefuelSignsPosted] nchar(1) NULL
, [RefuelGrounded] nchar(1) NULL
, [SafetyPfd] nchar(1) NULL
, [SafetyRadio] nchar(1) NULL
, [SafetyVolunteers] nchar(1) NULL
, [SafetyBoats] nchar(1) NULL
, [FirstAidArea] nchar(1) NULL
, [SpineBoard] nchar(1) NULL
, [SafetyCid] nchar(1) NULL
, [FirstAidKit] nchar(1) NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [SafetyCheckListPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX SafetyCheckList_PK ON dbo.SafetyCheckList ([PK] ASC);
GO

IF OBJECT_ID ('dbo.OfficialWorkAsgmt') IS NOT NULL
	DROP TABLE OfficialWorkAsgmt
GO
CREATE TABLE [OfficialWorkAsgmt] (
  [PK] bigint IDENTITY (1707,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [Round] tinyint NOT NULL
, [Event] nvarchar(12) NOT NULL
, [EventGroup] nvarchar(12) NOT NULL
, [WorkAsgmt] nvarchar(32) NOT NULL
, [StartTime] datetime NOT NULL
, [EndTime] datetime NULL
, [Notes] nvarchar(1024) NULL
, CONSTRAINT [OfficialWorkAsgmtPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC,[MemberId] ASC, [Round] ASC,[Event] ASC, [EventGroup] ASC, [WorkAsgmt] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX OfficialWorkAsgmt_PK ON dbo.OfficialWorkAsgmt ([PK] ASC);
GO

IF OBJECT_ID ('dbo.OfficialWork') IS NOT NULL
	DROP TABLE OfficialWork
GO
CREATE TABLE [OfficialWork] (
  [PK] bigint IDENTITY (4163,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [JudgeChief] nchar(1) NULL
, [JudgeAsstChief] nchar(1) NULL
, [JudgeAppointed] nchar(1) NULL
, [DriverChief] nchar(1) NULL
, [DriverAsstChief] nchar(1) NULL
, [DriverAppointed] nchar(1) NULL
, [ScoreChief] nchar(1) NULL
, [ScoreAsstChief] nchar(1) NULL
, [ScoreAppointed] nchar(1) NULL
, [SafetyChief] nchar(1) NULL
, [SafetyAsstChief] nchar(1) NULL
, [SafetyAppointed] nchar(1) NULL
, [AnncrChief] nchar(1) NULL
, [JudgeSlalomCredit] nchar(1) NULL
, [JudgeTrickCredit] nchar(1) NULL
, [JudgeJumpCredit] nchar(1) NULL
, [DriverSlalomCredit] nchar(1) NULL
, [DriverTrickCredit] nchar(1) NULL
, [DriverJumpCredit] nchar(1) NULL
, [ScoreSlalomCredit] nchar(1) NULL
, [ScoreTrickCredit] nchar(1) NULL
, [ScoreJumpCredit] nchar(1) NULL
, [SafetySlalomCredit] nchar(1) NULL
, [SafetyTrickCredit] nchar(1) NULL
, [SafetyJumpCredit] nchar(1) NULL
, [TechSlalomCredit] nchar(1) NULL
, [TechTrickCredit] nchar(1) NULL
, [TechJumpCredit] nchar(1) NULL
, [TechChief] nchar(1) NULL
, [TechAsstChief] nchar(1) NULL
, [AnncrSlalomCredit] nchar(1) NULL
, [AnncrTrickCredit] nchar(1) NULL
, [AnncrJumpCredit] nchar(1) NULL
, [AnncrAsstChief] nchar(1) NULL
, [SafetyOfficialRating] nvarchar(32) NULL
, [TechOfficialRating] nvarchar(32) NULL
, [AnncrOfficialRating] nvarchar(32) NULL
, [LastUpdateDate] datetime NULL
, [Note] nvarchar(1024) NULL
, [JudgeSlalomRating] nvarchar(32) NULL
, [JudgeTrickRating] nvarchar(32) NULL
, [JudgeJumpRating] nvarchar(32) NULL
, [DriverSlalomRating] nvarchar(32) NULL
, [DriverTrickRating] nvarchar(32) NULL
, [DriverJumpRating] nvarchar(32) NULL
, [ScorerSlalomRating] nvarchar(32) NULL
, [ScorerTrickRating] nvarchar(32) NULL
, [ScorerJumpRating] nvarchar(32) NULL
, CONSTRAINT [OfficialWorkPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC,[MemberId] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX OfficialWork_PK ON dbo.OfficialWork ([PK] ASC);
GO

IF OBJECT_ID ('dbo.LiveWebMsgSend') IS NOT NULL
	DROP TABLE LiveWebMsgSend
GO
CREATE TABLE [LiveWebMsgSend] (
  [PK] int IDENTITY (8,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MsgType] nvarchar(128) NOT NULL
, [MsgDataHash] int NOT NULL
, [MsgData] ntext NOT NULL
, [CreateDate] datetime NULL
, CONSTRAINT [LiveWebMsgSendPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC,[MsgDataHash] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX LiveWebMsgSend_PK ON dbo.LiveWebMsgSend ([PK] ASC);
GO

IF OBJECT_ID ('dbo.EventRunOrderFilters') IS NOT NULL
	DROP TABLE EventRunOrderFilters
GO
CREATE TABLE [EventRunOrderFilters] (
  [SanctionId] nchar(6) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [FilterName] nvarchar(128) NOT NULL
, [PrintTitle] nvarchar(256) NULL
, [GroupFilterCriteria] nvarchar(1024) NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [EventRunOrderFiltersPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [Event] ASC, [FilterName] ASC) 
)
GO

IF OBJECT_ID ('dbo.EventRunOrder') IS NOT NULL
	DROP TABLE EventRunOrder
GO
CREATE TABLE [EventRunOrder] (
  [PK] bigint IDENTITY (419,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [EventGroup] nvarchar(12) NOT NULL
, [RunOrderGroup] nvarchar(12) NULL
, [Event] nvarchar(12) NOT NULL
, [Round] tinyint NOT NULL
, [RunOrder] smallint NULL
, [RankingScore] numeric(9,3) NULL
, [LastUpdateDate] datetime NULL
, [Notes] nvarchar(1024) NULL
, CONSTRAINT [EventRunOrderPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [MemberId] ASC, [AgeGroup] ASC, [Event] ASC, [Round] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX EventRunOrder_PK ON dbo.EventRunOrder ([PK] ASC);
GO

IF OBJECT_ID ('dbo.DivOrder') IS NOT NULL
	DROP TABLE DivOrder
GO
CREATE TABLE [DivOrder] (
  [PK] bigint IDENTITY (511,1) NOT NULL
, [SanctionId] nchar(6) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [RunOrder] smallint NULL
, [LastUpdateDate] datetime NULL
, CONSTRAINT [DivOrderPKIndex] PRIMARY KEY NONCLUSTERED ([SanctionId] ASC, [Event] ASC, [AgeGroup] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX DivOrder_PK ON dbo.DivOrder ([PK] ASC);
GO

IF OBJECT_ID ('dbo.MemberList') IS NOT NULL
	DROP TABLE MemberList
GO
CREATE TABLE [MemberList] (
  [MemberId] nchar(9) NOT NULL
, [LastName] nvarchar(128) NOT NULL
, [FirstName] nvarchar(128) NOT NULL
, [Address1] nvarchar(128) NULL
, [Address2] nvarchar(128) NULL
, [City] nvarchar(128) NULL
, [State] nchar(2) NULL
, [Federation] nvarchar(12) NULL
, [ForeignFederationID] nchar(12) NULL
, [Country] nvarchar(64) NULL
, [Postalcode] nvarchar(16) NULL
, [SkiYearAge] tinyint NULL
, [DateOfBirth] datetime NULL
, [Gender] nchar(1) NULL
, [MemberStatus] nvarchar(256) NULL
, [MemberExpireDate] datetime NULL
, [SafetyOfficialRating] nvarchar(32) NULL
, [TechOfficialRating] nvarchar(32) NULL
, [AnncrOfficialRating] nvarchar(32) NULL
, [InsertDate] datetime NULL
, [UpdateDate] datetime NULL
, [Note] nvarchar(1024) NULL
, [JudgeSlalomRating] nvarchar(32) NULL
, [JudgeTrickRating] nvarchar(32) NULL
, [JudgeJumpRating] nvarchar(32) NULL
, [DriverSlalomRating] nvarchar(32) NULL
, [DriverTrickRating] nvarchar(32) NULL
, [DriverJumpRating] nvarchar(32) NULL
, [ScorerSlalomRating] nvarchar(32) NULL
, [ScorerTrickRating] nvarchar(32) NULL
, [ScorerJumpRating] nvarchar(32) NULL
, CONSTRAINT [MemberListPKIndex] PRIMARY KEY NONCLUSTERED ([MemberId] ASC) 
)
GO

IF OBJECT_ID ('dbo.NopsData') IS NOT NULL
	DROP TABLE NopsData
GO
CREATE TABLE [NopsData] (
  [PK] bigint IDENTITY (26299,1) NOT NULL
, [SkiYear] tinyint NOT NULL
, [Event] nvarchar(12) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [SortSeq] tinyint NULL
, [Base] numeric(9,3) NULL
, [Adj] numeric(9,3) NULL
, [RatingOpen] numeric(9,3) NULL
, [RatingRec] numeric(9,3) NULL
, [RatingMedian] numeric(9,3) NULL
, [OverallBase] numeric(9,3) NULL
, [OverallExp] numeric(9,3) NULL
, [EventsReqd] tinyint NULL
, CONSTRAINT [NopsDataPKIndex] PRIMARY KEY NONCLUSTERED ([SkiYear] ASC, [Event] ASC, [AgeGroup] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX NopsData_PK ON dbo.NopsData ([PK] ASC);
GO

IF OBJECT_ID ('dbo.CodeValueList') IS NOT NULL
	DROP TABLE CodeValueList
GO
CREATE TABLE [CodeValueList] (
  [PK] bigint IDENTITY (77524,1) NOT NULL
, [ListName] nvarchar(128) NOT NULL
, [ListCode] nvarchar(16) NOT NULL
, [ListCodeNum] numeric(9,3) NULL
, [SortSeq] int NULL
, [CodeValue] nvarchar(64) NULL
, [MinValue] numeric(9,3) NULL
, [MaxValue] numeric(9,3) NULL
, [CodeDesc] nvarchar(1024) NULL
, CONSTRAINT [CodeValueListPKIndex] PRIMARY KEY NONCLUSTERED ([ListName] ASC, [ListCode] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX CodeValueList_PK ON dbo.CodeValueList ([PK] ASC);
GO

IF OBJECT_ID ('dbo.TrickList') IS NOT NULL
	DROP TABLE dbo.TrickList
GO
CREATE TABLE [TrickList] (
  [PK] int IDENTITY (1,1) NOT NULL
, [RuleCode] nvarchar(16) NOT NULL
, [TrickCode] nvarchar(16) NOT NULL
, [NumSkis] tinyint NOT NULL
, [StartPos] tinyint NOT NULL
, [NumTurns] tinyint NOT NULL
, [RuleNum] smallint NULL
, [TypeCode] tinyint NOT NULL
, [Points] smallint NOT NULL
, [Description] nvarchar(128) NULL
, CONSTRAINT [TrickListPKIndex] PRIMARY KEY NONCLUSTERED ([RuleCode] ASC, [TrickCode] ASC, [NumSkis] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX TrickList_PK ON dbo.CodeValueList ([PK] ASC);
GO

IF OBJECT_ID ('dbo.SkierRanking') IS NOT NULL
	DROP TABLE SkierRanking
GO
CREATE TABLE [SkierRanking] (
  [PK] bigint IDENTITY (1168,1) NOT NULL
, [MemberId] nchar(9) NOT NULL
, [AgeGroup] nvarchar(12) NOT NULL
, [Event] nvarchar(12) NOT NULL
, [Score] numeric(9,3) NULL
, [Rating] nvarchar(12) NULL
, [HCapBase] numeric(9,3) NULL
, [HCapScore] numeric(9,3) NULL
, [Notes] nvarchar(1024) NULL
, [SeqNum] tinyint NULL
, CONSTRAINT [SkierRankingPKIndex] PRIMARY KEY NONCLUSTERED ([MemberId] ASC, [AgeGroup] ASC, [Event] ASC) 
)
GO
CREATE UNIQUE NONCLUSTERED INDEX SkierRanking_PK ON dbo.SkierRanking ([PK] ASC);
GO