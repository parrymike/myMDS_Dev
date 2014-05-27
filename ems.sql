/*
SQLyog Community v11.42 (64 bit)
MySQL - 5.6.17 : Database - emotive.cms
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`emotive.cms` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `emotive.cms`;

/*Table structure for table `applicationcourseyears` */

DROP TABLE IF EXISTS `applicationcourseyears`;

CREATE TABLE `applicationcourseyears` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationID` int(10) unsigned NOT NULL,
  `CourseYearID` int(10) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=latin1;

/*Table structure for table `applicationroles` */

DROP TABLE IF EXISTS `applicationroles`;

CREATE TABLE `applicationroles` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `ApplicationID` int(11) NOT NULL,
  `Name` varchar(60) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;

/*Table structure for table `applications` */

DROP TABLE IF EXISTS `applications`;

CREATE TABLE `applications` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=latin1;

/*Table structure for table `audit` */

DROP TABLE IF EXISTS `audit`;

CREATE TABLE `audit` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Date` datetime NOT NULL,
  `Username` varchar(30) NOT NULL,
  `Action` varchar(20) NOT NULL,
  `Type` varchar(300) NOT NULL,
  `ObjectID` int(20) NOT NULL,
  `Object` text NOT NULL,
  `Details` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=568 DEFAULT CHARSET=latin1;

/*Table structure for table `courses` */

DROP TABLE IF EXISTS `courses`;

CREATE TABLE `courses` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `Abbreviation` varchar(200) NOT NULL,
  `BannerCode` varchar(200) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;

/*Table structure for table `courseyears` */

DROP TABLE IF EXISTS `courseyears`;

CREATE TABLE `courseyears` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(200) NOT NULL,
  `Abbreviation` varchar(200) NOT NULL,
  `BannerCode` varchar(200) NOT NULL,
  `YearStart` datetime NOT NULL,
  `Year` int(11) NOT NULL,
  `CourseID` int(11) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

/*Table structure for table `documentlocations` */

DROP TABLE IF EXISTS `documentlocations`;

CREATE TABLE `documentlocations` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Reference` varchar(20) NOT NULL,
  `Directory` varchar(300) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `documents` */

DROP TABLE IF EXISTS `documents`;

CREATE TABLE `documents` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` text NOT NULL,
  `IdLocation` int(11) NOT NULL,
  `Extension` varchar(10) NOT NULL,
  `ModifiedName` text NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `emailassignedtoevent` */

DROP TABLE IF EXISTS `emailassignedtoevent`;

CREATE TABLE `emailassignedtoevent` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IdEvent` int(11) NOT NULL,
  `IdRole` int(11) NOT NULL,
  `IdEmail` int(11) NOT NULL,
  `Created` datetime NOT NULL,
  `CreatedBy` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `emaileventreplacements` */

DROP TABLE IF EXISTS `emaileventreplacements`;

CREATE TABLE `emaileventreplacements` (
  `id` int(11) DEFAULT NULL,
  `idEvent` int(11) DEFAULT NULL,
  `Name` varchar(30) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `emailevents` */

DROP TABLE IF EXISTS `emailevents`;

CREATE TABLE `emailevents` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(255) NOT NULL,
  `Description` text,
  `Enabled` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `emailhasattachments` */

DROP TABLE IF EXISTS `emailhasattachments`;

CREATE TABLE `emailhasattachments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `IdEmail` int(11) NOT NULL,
  `IdDocument` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `emails` */

DROP TABLE IF EXISTS `emails`;

CREATE TABLE `emails` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `To` varchar(300) DEFAULT NULL,
  `CC` varchar(300) DEFAULT NULL,
  `BCC` varchar(300) DEFAULT NULL,
  `Subject` varchar(300) NOT NULL,
  `Body` text NOT NULL,
  `Priority` varchar(10) DEFAULT 'Normal',
  `IsBodyHtml` tinyint(1) DEFAULT '1',
  `Enabled` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=latin1;

/*Table structure for table `eventreplacementtags` */

DROP TABLE IF EXISTS `eventreplacementtags`;

CREATE TABLE `eventreplacementtags` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `EventID` int(11) NOT NULL,
  `Tag` varchar(30) NOT NULL,
  `Description` varchar(70) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=latin1;

/*Table structure for table `events` */

DROP TABLE IF EXISTS `events`;

CREATE TABLE `events` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApplicationID` int(4) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `NiceName` varchar(255) NOT NULL,
  `Description` text,
  `Enabled` tinyint(4) NOT NULL DEFAULT '1',
  `System` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=68 DEFAULT CHARSET=latin1;

/*Table structure for table `eventsections` */

DROP TABLE IF EXISTS `eventsections`;

CREATE TABLE `eventsections` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(60) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `eventtags` */

DROP TABLE IF EXISTS `eventtags`;

CREATE TABLE `eventtags` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `EventID` int(11) NOT NULL,
  `Tag` varchar(30) NOT NULL,
  `Description` varchar(70) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `nlog` */

DROP TABLE IF EXISTS `nlog`;

CREATE TABLE `nlog` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `TimeStamp` datetime DEFAULT NULL,
  `Logger` text,
  `Origin` text,
  `LogLevel` text,
  `Message` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12165 DEFAULT CHARSET=latin1;

/*Table structure for table `objectassignedtoevent` */

DROP TABLE IF EXISTS `objectassignedtoevent`;

CREATE TABLE `objectassignedtoevent` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `EventID` int(11) NOT NULL,
  `ObjectID` int(11) NOT NULL,
  `Type` varchar(300) DEFAULT NULL,
  `Created` datetime NOT NULL,
  `CreatedBy` varchar(30) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;

/*Table structure for table `pages` */

DROP TABLE IF EXISTS `pages`;

CREATE TABLE `pages` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Title` varchar(200) DEFAULT NULL,
  `Text` text,
  `PageSection` int(5) unsigned NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;

/*Table structure for table `pagesections` */

DROP TABLE IF EXISTS `pagesections`;

CREATE TABLE `pagesections` (
  `ID` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;

/*Table structure for table `roles` */

DROP TABLE IF EXISTS `roles`;

CREATE TABLE `roles` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(20) NOT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

/*Table structure for table `userhasroles` */

DROP TABLE IF EXISTS `userhasroles`;

CREATE TABLE `userhasroles` (
  `userid` int(11) DEFAULT NULL,
  `roleid` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `users` */

DROP TABLE IF EXISTS `users`;

CREATE TABLE `users` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(20) NOT NULL,
  `Forename` varchar(30) NOT NULL,
  `Surname` varchar(30) NOT NULL,
  `Email` varchar(70) NOT NULL,
  `Enabled` tinyint(1) NOT NULL DEFAULT '1',
  `Archived` tinyint(1) NOT NULL DEFAULT '0',
  `Salt` varchar(16) DEFAULT NULL,
  `Password` varchar(40) DEFAULT NULL,
  `UserTypeID` int(11) NOT NULL,
  `RegistrationStatus` varchar(20) DEFAULT NULL,
  `UniversityID` varchar(11) DEFAULT NULL,
  `LastLogin` datetime DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=644 DEFAULT CHARSET=latin1;

/*Table structure for table `usertypes` */

DROP TABLE IF EXISTS `usertypes`;

CREATE TABLE `usertypes` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `Type` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
