CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(95) NOT NULL,
    `ProductVersion` varchar(32) NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
);


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Owners` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(255) NOT NULL,
        CONSTRAINT `PK_Owners` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Regions` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(255) NOT NULL,
        `NestingBoxIdPrefix` varchar(10) NULL,
        CONSTRAINT `PK_Regions` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Roles` (
        `Id` varchar(255) NOT NULL,
        `Name` varchar(256) NULL,
        `NormalizedName` varchar(256) NULL,
        `ConcurrencyStamp` longtext NULL,
        CONSTRAINT `PK_Roles` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Species` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Name` varchar(255) NOT NULL,
        CONSTRAINT `PK_Species` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Users` (
        `Id` varchar(255) NOT NULL,
        `UserName` varchar(256) NULL,
        `NormalizedUserName` varchar(256) NULL,
        `Email` varchar(256) NULL,
        `NormalizedEmail` varchar(256) NULL,
        `EmailConfirmed` bit NOT NULL,
        `PasswordHash` longtext NULL,
        `SecurityStamp` longtext NULL,
        `ConcurrencyStamp` longtext NULL,
        `PhoneNumber` longtext NULL,
        `PhoneNumberConfirmed` bit NOT NULL,
        `TwoFactorEnabled` bit NOT NULL,
        `LockoutEnd` datetime(6) NULL,
        `LockoutEnabled` bit NOT NULL,
        `AccessFailedCount` int NOT NULL,
        `FirstName` longtext NOT NULL,
        `LastName` longtext NOT NULL,
        CONSTRAINT `PK_Users` PRIMARY KEY (`Id`)
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `RoleClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RoleId` varchar(255) NOT NULL,
        `ClaimType` longtext NULL,
        `ClaimValue` longtext NULL,
        CONSTRAINT `PK_RoleClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_RoleClaims_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `NestingBoxes` (
        `Id` varchar(6) NOT NULL,
        `RegionId` int NOT NULL,
        `OldId` varchar(100) NOT NULL,
        `ForeignId` varchar(100) NOT NULL,
        `CoordinateLongitude` double NULL,
        `CoordinateLatitude` double NULL,
        `HangUpDate` datetime(6) NULL,
        `HangUpUserId` varchar(255) NULL,
        `OwnerId` int NOT NULL,
        `Material` int NOT NULL,
        `HoleSize` int NOT NULL,
        `ImageFileName` varchar(100) NULL,
        `Comment` longtext NULL,
        `LastUpdated` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `PK_NestingBoxes` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_NestingBoxes_Users_HangUpUserId` FOREIGN KEY (`HangUpUserId`) REFERENCES `Users` (`Id`) ON DELETE RESTRICT,
        CONSTRAINT `FK_NestingBoxes_Owners_OwnerId` FOREIGN KEY (`OwnerId`) REFERENCES `Owners` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_NestingBoxes_Regions_RegionId` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `ReservedIdSpaces` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `RegionId` int NOT NULL,
        `OwnerId` varchar(255) NOT NULL,
        `ReservationDate` datetime(6) NOT NULL,
        `FirstNestingBoxIdWithoutPrefix` int NOT NULL,
        `LastNestingBoxIdWithoutPrefix` int NOT NULL,
        CONSTRAINT `PK_ReservedIdSpaces` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ReservedIdSpaces_Users_OwnerId` FOREIGN KEY (`OwnerId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_ReservedIdSpaces_Regions_RegionId` FOREIGN KEY (`RegionId`) REFERENCES `Regions` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `UserClaims` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `UserId` varchar(255) NOT NULL,
        `ClaimType` longtext NULL,
        `ClaimValue` longtext NULL,
        CONSTRAINT `PK_UserClaims` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_UserClaims_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `UserLogins` (
        `LoginProvider` varchar(255) NOT NULL,
        `ProviderKey` varchar(255) NOT NULL,
        `ProviderDisplayName` longtext NULL,
        `UserId` varchar(255) NOT NULL,
        CONSTRAINT `PK_UserLogins` PRIMARY KEY (`LoginProvider`, `ProviderKey`),
        CONSTRAINT `FK_UserLogins_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `UserRoles` (
        `UserId` varchar(255) NOT NULL,
        `RoleId` varchar(255) NOT NULL,
        CONSTRAINT `PK_UserRoles` PRIMARY KEY (`UserId`, `RoleId`),
        CONSTRAINT `FK_UserRoles_Roles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `Roles` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_UserRoles_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `UserTokens` (
        `UserId` varchar(255) NOT NULL,
        `LoginProvider` varchar(255) NOT NULL,
        `Name` varchar(255) NOT NULL,
        `Value` longtext NULL,
        CONSTRAINT `PK_UserTokens` PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
        CONSTRAINT `FK_UserTokens_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE TABLE `Inspections` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `NestingBoxId` varchar(6) NOT NULL,
        `InspectionDate` datetime(6) NOT NULL,
        `InspectedByUserId` varchar(255) NOT NULL,
        `HasBeenCleaned` bit NOT NULL,
        `Condition` int NOT NULL,
        `JustRepaired` bit NOT NULL,
        `Occupied` bit NOT NULL,
        `ContainsEggs` bit NOT NULL,
        `EggCount` int NULL,
        `ChickCount` int NOT NULL,
        `RingedChickCount` int NOT NULL,
        `AgeInDays` int NULL,
        `FemaleParentBirdDiscovery` int NOT NULL,
        `MaleParentBirdDiscovery` int NOT NULL,
        `SpeciesId` int NULL,
        `ImageFileName` varchar(100) NULL,
        `Comment` longtext NULL,
        `LastUpdated` datetime(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
        CONSTRAINT `PK_Inspections` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Inspections_Users_InspectedByUserId` FOREIGN KEY (`InspectedByUserId`) REFERENCES `Users` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Inspections_NestingBoxes_NestingBoxId` FOREIGN KEY (`NestingBoxId`) REFERENCES `NestingBoxes` (`Id`) ON DELETE CASCADE,
        CONSTRAINT `FK_Inspections_Species_SpeciesId` FOREIGN KEY (`SpeciesId`) REFERENCES `Species` (`Id`) ON DELETE RESTRICT
    );

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_Inspections_InspectedByUserId` ON `Inspections` (`InspectedByUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_Inspections_NestingBoxId` ON `Inspections` (`NestingBoxId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_Inspections_SpeciesId` ON `Inspections` (`SpeciesId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_NestingBoxes_HangUpUserId` ON `NestingBoxes` (`HangUpUserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_NestingBoxes_OwnerId` ON `NestingBoxes` (`OwnerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_NestingBoxes_RegionId` ON `NestingBoxes` (`RegionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_ReservedIdSpaces_OwnerId` ON `ReservedIdSpaces` (`OwnerId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_ReservedIdSpaces_RegionId` ON `ReservedIdSpaces` (`RegionId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_RoleClaims_RoleId` ON `RoleClaims` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE UNIQUE INDEX `RoleNameIndex` ON `Roles` (`NormalizedName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_UserClaims_UserId` ON `UserClaims` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_UserLogins_UserId` ON `UserLogins` (`UserId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `IX_UserRoles_RoleId` ON `UserRoles` (`RoleId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE INDEX `EmailIndex` ON `Users` (`NormalizedEmail`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    CREATE UNIQUE INDEX `UserNameIndex` ON `Users` (`NormalizedUserName`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20190920015448_InitialCreate') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20190920015448_InitialCreate', '3.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191014235003_NestingBoxRequiredIdsChanges') THEN

    ALTER TABLE `NestingBoxes` MODIFY COLUMN `OldId` varchar(100) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191014235003_NestingBoxRequiredIdsChanges') THEN

    ALTER TABLE `NestingBoxes` MODIFY COLUMN `ForeignId` varchar(100) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;


DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20191014235003_NestingBoxRequiredIdsChanges') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20191014235003_NestingBoxRequiredIdsChanges', '3.0.0');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

