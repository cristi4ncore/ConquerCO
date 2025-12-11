/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 50717
Source Host           : localhost:3306
Source Database       : prueba

Target Server Type    : MYSQL
Target Server Version : 50717
File Encoding         : 65001

Date: 2025-11-06 18:52:06
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for accounts
-- ----------------------------
DROP TABLE IF EXISTS `accounts`;
CREATE TABLE `accounts` (
  `Username` char(25) NOT NULL DEFAULT '',
  `Password` char(16) DEFAULT '',
  `IP` char(15) DEFAULT '',
  `LastCheck` bigint(255) unsigned DEFAULT '0',
  `State` tinyint(5) unsigned DEFAULT '0',
  `EntityID` bigint(18) unsigned DEFAULT '0',
  `Email` char(100) DEFAULT '',
  `Question` char(100) DEFAULT NULL,
  `answer` char(30) DEFAULT NULL,
  `Country` char(110) DEFAULT '',
  `City` char(100) DEFAULT '',
  `secretquestion` char(45) DEFAULT '',
  `realname` char(25) DEFAULT '',
  `machine` char(50) DEFAULT '',
  `lastvote` char(50) DEFAULT '',
  `mobilenumber` bigint(18) DEFAULT '0',
  `securitycode` varchar(100) DEFAULT '',
  `date` varchar(0) DEFAULT '',
  `joined` varchar(220) DEFAULT NULL,
  `Online` tinyint(2) DEFAULT NULL,
  `UID` bigint(18) DEFAULT NULL,
  `Class` tinyint(5) DEFAULT NULL,
  `hash` varchar(250) DEFAULT NULL,
  `active` int(11) DEFAULT '0',
  `new` int(11) DEFAULT '0',
  `newmail` varchar(250) DEFAULT NULL,
  `remember_token` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Username`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of accounts
-- ----------------------------
INSERT INTO `accounts` VALUES ('1', '1', '26.147.180.40', '0', '4', '1000002', '', null, null, '', '', '', '', '', '', '0', '', '', null, '1', null, null, null, '0', '0', null, null);
INSERT INTO `accounts` VALUES ('2', '1', '', '0', '4', '1000003', '', null, null, '', '', '', '', '', '', '0', '', '', null, '1', null, null, null, '0', '0', null, null);

-- ----------------------------
-- Table structure for banned
-- ----------------------------
DROP TABLE IF EXISTS `banned`;
CREATE TABLE `banned` (
  `UID` varchar(16) NOT NULL,
  `username` varchar(16) NOT NULL DEFAULT '',
  `Hours` bigint(18) unsigned NOT NULL DEFAULT '0',
  `StartBan` bigint(255) unsigned NOT NULL DEFAULT '0',
  `Reason` varchar(255) NOT NULL DEFAULT '',
  PRIMARY KEY (`UID`) USING BTREE
) ENGINE=MyISAM DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of banned
-- ----------------------------

-- ----------------------------
-- Table structure for configuration
-- ----------------------------
DROP TABLE IF EXISTS `configuration`;
CREATE TABLE `configuration` (
  `EntityID` bigint(10) unsigned DEFAULT '1000000',
  `Server` varchar(30) CHARACTER SET utf8 NOT NULL DEFAULT '',
  PRIMARY KEY (`Server`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of configuration
-- ----------------------------
INSERT INTO `configuration` VALUES ('1000003', 'AsGard');

-- ----------------------------
-- Table structure for macs
-- ----------------------------
DROP TABLE IF EXISTS `macs`;
CREATE TABLE `macs` (
  `mac` varchar(255) DEFAULT NULL,
  `id` bigint(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of macs
-- ----------------------------

-- ----------------------------
-- Table structure for online
-- ----------------------------
DROP TABLE IF EXISTS `online`;
CREATE TABLE `online` (
  `Name` varchar(255) NOT NULL,
  `OnlineCount` int(11) DEFAULT '0',
  `MaxOnline` int(11) DEFAULT NULL,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of online
-- ----------------------------
INSERT INTO `online` VALUES ('AsGard', '0', '0');

-- ----------------------------
-- Table structure for servers
-- ----------------------------
DROP TABLE IF EXISTS `servers`;
CREATE TABLE `servers` (
  `Name` varchar(16) CHARACTER SET utf8 NOT NULL DEFAULT '',
  `IP` varchar(16) CHARACTER SET utf8 DEFAULT NULL,
  `Port` int(16) unsigned DEFAULT NULL,
  `TransferKey` varchar(64) CHARACTER SET latin1 COLLATE latin1_general_cs NOT NULL,
  `TransferSalt` varchar(64) CHARACTER SET latin1 COLLATE latin1_general_cs NOT NULL,
  PRIMARY KEY (`Name`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 ROW_FORMAT=DYNAMIC;

-- ----------------------------
-- Records of servers
-- ----------------------------
INSERT INTO `servers` VALUES ('DeathConquer', '26.147.180.40', '5816', 'EypKhLvYJ3zdLCTyz9Ak8RAgM78tY5F32b7CUXDuLDJDFBH8H67BWy9QThmaN5VS', 'MyqVgBf3ytALHWLXbJxSUX4uFEu3Xmz2UAY9sTTm8AScB7Kk2uwqDSnuNJske4BJ');

-- ----------------------------
-- Procedure structure for iuprofs
-- ----------------------------
DROP PROCEDURE IF EXISTS `iuprofs`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `iuprofs`(IN `EntityID` bigint(18),IN `ID`  mediumint(10),IN `Level` smallint(5),IN `Experience` bigint(10),IN `PreviousLevel` smallint(5),IN type smallint(1))
BEGIN
IF type =0 THEN
	INSERT INTO profs (`EntityID` , `ID` ,`Level` ,`Experience` ,`PreviousLevel` )
	VALUE
	(`EntityID` , `ID` ,`Level` ,`Experience` ,`PreviousLevel` );
ELSE
	UPDATE profs SET 
	`profs`.`Level` =`Level`,`profs`.`Experience`=`Experience` ,`profs`.`PreviousLevel` =`PreviousLevel`
	WHERE (
	(`profs`.`EntityID` =`EntityID` AND `profs`.`ID`=`ID`)
);
END IF;
END
;;
DELIMITER ;

-- ----------------------------
-- Procedure structure for iuskills
-- ----------------------------
DROP PROCEDURE IF EXISTS `iuskills`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `iuskills`(IN `EntityID` bigint(18),IN `ID`  mediumint(10),IN `Level` smallint(5),IN `Experience` bigint(10),IN `PreviousLevel` smallint(5),IN type smallint(1))
BEGIN
IF type =0 THEN
	INSERT INTO skills (`EntityID` , `ID` ,`Level` ,`Experience` ,`PreviousLevel` )
	VALUE
	(`EntityID` , `ID` ,`Level` ,`Experience` ,`PreviousLevel` );
ELSE
	UPDATE skills SET 
	`skills`.`Level` =`Level`,`skills`.`Experience`=`Experience` ,`skills`.`PreviousLevel` =`PreviousLevel`
	WHERE (
	(`skills`.`EntityID` =`EntityID` AND `skills`.`ID`=`ID`)
);
END IF;
END
;;
DELIMITER ;

-- ----------------------------
-- Procedure structure for test_multi_sets
-- ----------------------------
DROP PROCEDURE IF EXISTS `test_multi_sets`;
DELIMITER ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `test_multi_sets`()
    DETERMINISTIC
begin
        select user() as first_col;
        select user() as first_col, now() as second_col;
        select user() as first_col, now() as second_col, now() as third_col;
        end
;;
DELIMITER ;
