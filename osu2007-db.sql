SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
CREATE DATABASE IF NOT EXISTS `osu2007`;
USE `osu2007`;

CREATE TABLE IF NOT EXISTS `rankedmaps` (
  `mapID` int(11) NOT NULL,
  `mapsetID` int(11) NOT NULL,
  `mapHash` varchar(256) NOT NULL,
  `artist` varchar(80) NOT NULL,
  `song` varchar(80) NOT NULL,
  `mapper` varchar(80) NOT NULL,
  `difficulty` varchar(80) NOT NULL,
  `circleSize` tinyint(4) NOT NULL,
  `hpDrain` tinyint(4) NOT NULL,
  `overallDifficulty` tinyint(4) NOT NULL,
  `circleCount` smallint(6) NOT NULL,
  `sliderCount` smallint(6) NOT NULL,
  `spinnerCount` smallint(6) NOT NULL,
  `mapTime` smallint(6) NOT NULL,
  `drainTime` smallint(6) NOT NULL,
  `pStars` float NOT NULL COMMENT 'Star calculation algorithm by peppy',
  `eStars` float NOT NULL COMMENT 'Star calculation algorithm by eyup'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

ALTER TABLE `rankedmaps` 
  ADD PRIMARY KEY (`mapID`),
  ADD UNIQUE KEY `mapHash` (`mapHash`);
  
ALTER TABLE `rankedmaps`
  MODIFY `mapID` int(11) NOT NULL AUTO_INCREMENT;

CREATE TABLE IF NOT EXISTS `scores` (
  `scoreID` int(11) NOT NULL,
  `userID` int(11) NOT NULL,
  `mapID` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `combo` int(11) NOT NULL,
  `count300` int(11) NOT NULL,
  `count100` int(11) NOT NULL,
  `count50` int(11) NOT NULL,
  `countEliteBeat` int(11) NOT NULL,
  `countBeat` int(11) NOT NULL,
  `countMiss` int(11) NOT NULL,
  `perfect` tinyint(1) NOT NULL,
  `mods` int(1) NOT NULL DEFAULT 0
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

ALTER TABLE `scores`
  ADD PRIMARY KEY (`scoreID`);

ALTER TABLE `scores`
  MODIFY `scoreID` int(11) NOT NULL AUTO_INCREMENT;

CREATE TABLE IF NOT EXISTS `users` (
  `userID` int(11) NOT NULL,
  `userName` varchar(16) NOT NULL,
  `password` varchar(256) NOT NULL,
  `rankedScore` bigint(21) UNSIGNED NOT NULL,
  `totalScore` bigint(21) UNSIGNED NOT NULL,
  `playCount` int(11) UNSIGNED NOT NULL,
  `accuracy` float NOT NULL COMMENT '1.00 = 100% accuracy'
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

ALTER TABLE `users`
  ADD PRIMARY KEY (`userID`),
  ADD UNIQUE KEY `userName` (`userName`);
  
ALTER TABLE `users`
  MODIFY `userID` int(11) NOT NULL AUTO_INCREMENT;