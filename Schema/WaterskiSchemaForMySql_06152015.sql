--
-- Table structure for table `EventReg`
--

CREATE TABLE IF NOT EXISTS `EventReg` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Event` varchar(12) NOT NULL,
  `EventGroup` varchar(12) DEFAULT NULL,
  `RunOrder` smallint(6) DEFAULT NULL,
  `TeamCode` varchar(12) DEFAULT NULL,
  `EventClass` char(1) DEFAULT NULL,
  `RankingScore` decimal(9,3) DEFAULT NULL,
  `RankingRating` varchar(12) DEFAULT NULL,
  `HCapBase` decimal(9,3) DEFAULT NULL,
  `HCapScore` decimal(9,3) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=14972 ;

--
-- Table structure for table `EventRunOrder`
--

CREATE TABLE IF NOT EXISTS `EventRunOrder` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `EventGroup` varchar(12) NOT NULL,
  `Event` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `RunOrder` smallint(6) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Notes` varchar(1024) DEFAULT NULL,
  `RankingScore` decimal(9,3) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=4034 ;

--
-- Table structure for table `JumpRecap`
--

CREATE TABLE IF NOT EXISTS `JumpRecap` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `PassNum` tinyint(4) NOT NULL,
  `Reride` char(1) DEFAULT NULL,
  `BoatSpeed` tinyint(4) DEFAULT NULL,
  `RampHeight` decimal(3,1) DEFAULT NULL,
  `Meter1` decimal(5,2) DEFAULT NULL,
  `Meter2` decimal(5,2) DEFAULT NULL,
  `Meter3` decimal(5,2) DEFAULT NULL,
  `Meter4` decimal(5,2) DEFAULT NULL,
  `Meter5` decimal(5,2) DEFAULT NULL,
  `Meter6` decimal(5,2) DEFAULT NULL,
  `ScoreFeet` decimal(5,2) DEFAULT NULL,
  `ScoreMeters` decimal(5,2) DEFAULT NULL,
  `ScoreTriangle` decimal(5,2) DEFAULT NULL,
  `BoatSplitTime` decimal(6,3) DEFAULT NULL,
  `BoatSplitTime2` decimal(6,3) DEFAULT NULL,
  `BoatEndTime` decimal(6,3) DEFAULT NULL,
  `TimeInTol` char(1) DEFAULT NULL,
  `ScoreProt` char(1) DEFAULT NULL,
  `ReturnToBase` char(1) DEFAULT NULL,
  `Results` varchar(12) DEFAULT NULL,
  `RerideReason` varchar(256) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Note` varchar(1024) DEFAULT NULL,
  `Split52TimeTol` smallint(6) DEFAULT NULL,
  `Split82TimeTol` smallint(6) DEFAULT NULL,
  `Split41TimeTol` smallint(6) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=27412 ;

--
-- Table structure for table `SlalomRecap`
--

CREATE TABLE IF NOT EXISTS `SlalomRecap` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `PassNum` tinyint(4) NOT NULL,
  `SkierRunNum` smallint(6) NOT NULL,
  `Judge1Score` decimal(5,2) DEFAULT NULL,
  `Judge2Score` decimal(5,2) DEFAULT NULL,
  `Judge3Score` decimal(5,2) DEFAULT NULL,
  `Judge4Score` decimal(5,2) DEFAULT NULL,
  `Judge5Score` decimal(5,2) DEFAULT NULL,
  `EntryGate1` bit(1) DEFAULT NULL,
  `EntryGate2` bit(1) DEFAULT NULL,
  `EntryGate3` bit(1) DEFAULT NULL,
  `ExitGate1` bit(1) DEFAULT NULL,
  `ExitGate2` bit(1) DEFAULT NULL,
  `ExitGate3` bit(1) DEFAULT NULL,
  `BoatTime` decimal(5,2) DEFAULT NULL,
  `Score` decimal(5,2) DEFAULT NULL,
  `TimeInTol` char(1) DEFAULT NULL,
  `ScoreProt` char(1) DEFAULT NULL,
  `Reride` char(1) DEFAULT NULL,
  `RerideReason` varchar(256) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Note` varchar(1024) DEFAULT NULL,
  `PassLineLength` decimal(5,2) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=52976 ;

--
-- Table structure for table `SlalomScore`
--

CREATE TABLE IF NOT EXISTS `SlalomScore` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `MaxSpeed` tinyint(4) DEFAULT NULL,
  `StartSpeed` tinyint(4) DEFAULT NULL,
  `StartLen` varchar(6) DEFAULT NULL,
  `Score` decimal(5,2) DEFAULT NULL,
  `NopsScore` decimal(7,2) DEFAULT NULL,
  `Rating` varchar(16) DEFAULT NULL,
  `Status` varchar(16) DEFAULT NULL,
  `FinalPassNum` tinyint(4) DEFAULT NULL,
  `FinalSpeedMph` tinyint(4) DEFAULT NULL,
  `FinalSpeedKph` tinyint(4) DEFAULT NULL,
  `FinalLen` varchar(16) DEFAULT NULL,
  `FinalLenOff` varchar(16) DEFAULT NULL,
  `FinalPassScore` decimal(5,2) DEFAULT NULL,
  `Boat` varchar(32) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Note` varchar(1024) DEFAULT NULL,
  `EventClass` char(1) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=17366 ;

--
-- Table structure for table `Tournament`
--

CREATE TABLE IF NOT EXISTS `Tournament` (
  `SanctionId` char(6) NOT NULL,
  `Name` varchar(128) DEFAULT NULL,
  `Class` char(1) DEFAULT NULL,
  `Federation` varchar(12) DEFAULT NULL,
  `SlalomRounds` tinyint(4) DEFAULT NULL,
  `TrickRounds` tinyint(4) DEFAULT NULL,
  `JumpRounds` tinyint(4) DEFAULT NULL,
  `Rules` varchar(16) DEFAULT NULL,
  `EventDates` varchar(128) DEFAULT NULL,
  `TourDataLoc` varchar(256) DEFAULT NULL,
  `EventLocation` varchar(1024) DEFAULT NULL,
  `HcapSlalomBase` decimal(9,3) DEFAULT NULL,
  `HcapTrickBase` decimal(9,3) DEFAULT NULL,
  `HcapJumpBase` decimal(9,3) DEFAULT NULL,
  `HcapSlalomPct` decimal(9,3) DEFAULT NULL,
  `HcapTrickPct` decimal(9,3) DEFAULT NULL,
  `HcapJumpPct` decimal(9,3) DEFAULT NULL,
  `ContactMemberId` char(9) DEFAULT NULL,
  `ContactPhone` varchar(128) DEFAULT NULL,
  `ContactEmail` varchar(128) DEFAULT NULL,
  `RuleExceptions` varchar(1024) DEFAULT NULL,
  `RuleInterpretations` varchar(1024) DEFAULT NULL,
  `SafetyDirPerfReport` varchar(1024) DEFAULT NULL,
  `RopeHandlesSpecs` varchar(1024) DEFAULT NULL,
  `SlalomRopesSpecs` varchar(1024) DEFAULT NULL,
  `JumpRopesSpecs` varchar(1024) DEFAULT NULL,
  `SlalomCourseSpecs` varchar(1024) DEFAULT NULL,
  `JumpCourseSpecs` varchar(1024) DEFAULT NULL,
  `TrickCourseSpecs` varchar(1024) DEFAULT NULL,
  `BuoySpecs` varchar(1024) DEFAULT NULL,
  `RuleExceptQ1` varchar(128) DEFAULT NULL,
  `RuleExceptQ2` varchar(128) DEFAULT NULL,
  `RuleExceptQ3` char(1) DEFAULT NULL,
  `RuleExceptQ4` char(1) DEFAULT NULL,
  `RuleInterQ1` varchar(128) DEFAULT NULL,
  `RuleInterQ2` varchar(128) DEFAULT NULL,
  `RuleInterQ3` char(1) DEFAULT NULL,
  `RuleInterQ4` char(1) DEFAULT NULL,
  `ContactAddress` varchar(128) DEFAULT NULL,
  `ChiefJudgeMemberId` char(9) DEFAULT NULL,
  `ChiefJudgeAddress` varchar(128) DEFAULT NULL,
  `ChiefJudgePhone` varchar(128) DEFAULT NULL,
  `ChiefJudgeEmail` varchar(128) DEFAULT NULL,
  `ChiefDriverMemberId` char(9) DEFAULT NULL,
  `ChiefDriverAddress` varchar(128) DEFAULT NULL,
  `ChiefDriverPhone` varchar(128) DEFAULT NULL,
  `ChiefDriverEmail` varchar(128) DEFAULT NULL,
  `SafetyDirMemberId` char(9) DEFAULT NULL,
  `SafetyDirAddress` varchar(128) DEFAULT NULL,
  `SafetyDirPhone` varchar(128) DEFAULT NULL,
  `SafetyDirEmail` varchar(128) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `PlcmtMethod` varchar(256) DEFAULT NULL,
  `OverallMethod` varchar(256) DEFAULT NULL,
  `ChiefScorerMemberId` char(9) DEFAULT NULL,
  `ChiefScorerAddress` varchar(128) DEFAULT NULL,
  `ChiefScorerPhone` varchar(128) DEFAULT NULL,
  `ChiefScorerEmail` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`SanctionId`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- Table structure for table `TourReg`
--

CREATE TABLE IF NOT EXISTS `TourReg` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `MemberId` char(9) NOT NULL,
  `SanctionId` char(6) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `SkierName` varchar(128) DEFAULT NULL,
  `EntryDue` decimal(7,2) DEFAULT NULL,
  `EntryPaid` decimal(7,2) DEFAULT NULL,
  `PaymentMethod` varchar(64) DEFAULT NULL,
  `ReadyToSki` char(1) DEFAULT NULL,
  `ReadyForPlcmt` char(1) DEFAULT NULL,
  `AwsaMbrshpPaymt` decimal(7,2) DEFAULT NULL,
  `AwsaMbrshpComment` varchar(256) DEFAULT NULL,
  `Weight` smallint(6) DEFAULT NULL,
  `TrickBoat` varchar(64) DEFAULT NULL,
  `JumpHeight` decimal(3,1) DEFAULT NULL,
  `Federation` varchar(12) DEFAULT NULL,
  `Gender` char(1) DEFAULT NULL,
  `SkiYearAge` tinyint(4) DEFAULT NULL,
  `City` varchar(128) DEFAULT NULL,
  `State` char(2) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Notes` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=8465 ;

--
-- Table structure for table `TrickPass`
--

CREATE TABLE IF NOT EXISTS `TrickPass` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `PassNum` tinyint(4) NOT NULL,
  `Skis` tinyint(4) NOT NULL,
  `Seq` tinyint(4) NOT NULL,
  `Score` smallint(6) DEFAULT NULL,
  `Code` varchar(16) DEFAULT NULL,
  `Results` varchar(12) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Note` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=281246 ;

--
-- Table structure for table `TrickScore`
--

CREATE TABLE IF NOT EXISTS `TrickScore` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `Score` smallint(6) DEFAULT NULL,
  `ScorePass1` smallint(6) DEFAULT NULL,
  `ScorePass2` smallint(6) DEFAULT NULL,
  `NopsScore` decimal(7,2) DEFAULT NULL,
  `Rating` varchar(16) DEFAULT NULL,
  `Status` varchar(16) DEFAULT NULL,
  `Boat` varchar(32) DEFAULT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  `Note` varchar(1024) DEFAULT NULL,
  `EventClass` char(1) DEFAULT NULL,
  `Pass1VideoUrl` varchar(256) DEFAULT NULL,
  `Pass2VideoUrl` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=15143 ;

--
-- Table structure for table `TrickVideo`
--

CREATE TABLE IF NOT EXISTS `TrickVideo` (
  `PK` bigint(20) NOT NULL AUTO_INCREMENT,
  `SanctionId` char(6) NOT NULL,
  `MemberId` char(9) NOT NULL,
  `AgeGroup` varchar(12) NOT NULL,
  `Round` tinyint(4) NOT NULL,
  `Pass1VideoUrl` varchar(256) NOT NULL,
  `Pass2VideoUrl` varchar(256) NOT NULL,
  `LastUpdateDate` datetime DEFAULT NULL,
  PRIMARY KEY (`PK`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=1135 ;

