-- --------------------------------------------------------
-- Máy chủ:                      localhost
-- Server version:               9.1.0 - MySQL Community Server - GPL
-- Server OS:                    Linux
-- HeidiSQL Phiên bản:           12.6.0.6765
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Dumping database structure for E_commerce
CREATE DATABASE IF NOT EXISTS `E_commerce` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `E_commerce`;

-- Dumping structure for table E_commerce.Bill
CREATE TABLE IF NOT EXISTS `Bill` (
  `bill_id` varchar(15) NOT NULL,
  `invoice_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `shipping_fee` decimal(10,0) DEFAULT '0',
  `note` varchar(255) DEFAULT NULL,
  `user_client` varchar(18) NOT NULL,
  `pmt_id` tinyint NOT NULL,
  `status_id` tinyint NOT NULL,
  `user_emp` varchar(18) DEFAULT NULL,
  PRIMARY KEY (`bill_id`),
  KEY `user_client` (`user_client`),
  KEY `idx_bill_id` (`bill_id`),
  KEY `idx_invoice_date` (`invoice_date`),
  KEY `idxbill_user_emp` (`user_emp`),
  KEY `idxbill_pmt_id` (`pmt_id`),
  KEY `idxbill_status_id` (`status_id`),
  CONSTRAINT `Bill_ibfk_1` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Bill_ibfk_2` FOREIGN KEY (`status_id`) REFERENCES `Status` (`status_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Bill_ibfk_3` FOREIGN KEY (`pmt_id`) REFERENCES `PaymentMethod` (`pmt_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Bill_ibfk_4` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Bill: ~0 rows (approximately)

-- Dumping structure for table E_commerce.BillDetails
CREATE TABLE IF NOT EXISTS `BillDetails` (
  `quantity` int DEFAULT '1',
  `unit_price` decimal(10,0) DEFAULT NULL,
  `bill_id` varchar(15) NOT NULL,
  `product_id` varchar(10) NOT NULL,
  KEY `product_id` (`product_id`),
  KEY `idxBillDetails_bill_id_product_id` (`bill_id`,`product_id`),
  CONSTRAINT `BillDetails_ibfk_1` FOREIGN KEY (`bill_id`) REFERENCES `Bill` (`bill_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `BillDetails_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.BillDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Branch
CREATE TABLE IF NOT EXISTS `Branch` (
  `branch_id` tinyint NOT NULL AUTO_INCREMENT,
  `branch_name` varchar(50) NOT NULL,
  `address` varchar(255) NOT NULL,
  `describe` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`branch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Branch: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Cart
CREATE TABLE IF NOT EXISTS `Cart` (
  `_time` datetime NOT NULL,
  `quantity` int NOT NULL,
  `unit_price` decimal(10,0) NOT NULL,
  `user_client` varchar(18) NOT NULL,
  `product_id` varchar(18) NOT NULL,
  `cart_id` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`cart_id`),
  KEY `product_id` (`product_id`),
  KEY `idx_user_client_product_id` (`user_client`,`product_id`),
  KEY `idx_time` (`_time`),
  CONSTRAINT `Cart_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Cart_ibfk_2` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Cart: ~0 rows (approximately)

-- Dumping structure for table E_commerce.ChatbotInteraction
CREATE TABLE IF NOT EXISTS `ChatbotInteraction` (
  `chat_id` int NOT NULL AUTO_INCREMENT,
  `message` text NOT NULL,
  `response` text NOT NULL,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `user_client` varchar(18) DEFAULT NULL,
  `user_emp` varchar(18) DEFAULT NULL,
  PRIMARY KEY (`chat_id`),
  KEY `user_emp` (`user_emp`),
  KEY `idxChatbotInteraction_user_client_time` (`user_client`,`_time`),
  CONSTRAINT `ChatbotInteraction_ibfk_1` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ChatbotInteraction_ibfk_2` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.ChatbotInteraction: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Collaborate
CREATE TABLE IF NOT EXISTS `Collaborate` (
  `working_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `position_id` int NOT NULL,
  `dep_id` tinyint NOT NULL,
  `user_emp` varchar(18) DEFAULT NULL,
  `branch_id` tinyint NOT NULL,
  KEY `user_emp` (`user_emp`),
  KEY `dep_id` (`dep_id`),
  KEY `position_id` (`position_id`),
  KEY `FK_Branch_staff` (`branch_id`),
  CONSTRAINT `Collaborate_ibfk_1` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Collaborate_ibfk_2` FOREIGN KEY (`dep_id`) REFERENCES `Department` (`dep_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Collaborate_ibfk_3` FOREIGN KEY (`position_id`) REFERENCES `PositionStaff` (`position_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_Branch_staff` FOREIGN KEY (`branch_id`) REFERENCES `Branch` (`branch_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Collaborate: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Comments
CREATE TABLE IF NOT EXISTS `Comments` (
  `comments` varchar(255) DEFAULT NULL,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `user_name` varchar(50) DEFAULT NULL,
  `gmail` varchar(100) NOT NULL,
  `sdt` varchar(100) NOT NULL,
  `topic_id` tinyint DEFAULT NULL,
  KEY `topic_id` (`topic_id`),
  CONSTRAINT `Comments_ibfk_1` FOREIGN KEY (`topic_id`) REFERENCES `Topic` (`topic_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Comments: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Conversation
CREATE TABLE IF NOT EXISTS `Conversation` (
  `conversation_id` int NOT NULL AUTO_INCREMENT,
  `conversation_name` varchar(100) NOT NULL,
  `user_id` varchar(18) NOT NULL,
  PRIMARY KEY (`conversation_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `Conversation_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Conversation: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Customer
CREATE TABLE IF NOT EXISTS `Customer` (
  `user_client` varchar(18) NOT NULL,
  PRIMARY KEY (`user_client`),
  KEY `idx_customer_user_client` (`user_client`),
  CONSTRAINT `Customer_ibfk_1` FOREIGN KEY (`user_client`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Customer: ~2 rows (approximately)
INSERT INTO `Customer` (`user_client`) VALUES
	('2504081426024xJhng'),
	('250408142632ZyzD95');

-- Dumping structure for table E_commerce.CustomerRoleDetails
CREATE TABLE IF NOT EXISTS `CustomerRoleDetails` (
  `user_client` varchar(18) NOT NULL,
  `rank_id` tinyint NOT NULL,
  `role_id` tinyint NOT NULL,
  KEY `rank_id` (`rank_id`),
  KEY `idx_customer_role_user` (`user_client`),
  KEY `idx_customer_role_id` (`role_id`),
  CONSTRAINT `CustomerRoleDetails_ibfk_1` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `CustomerRoleDetails_ibfk_2` FOREIGN KEY (`role_id`) REFERENCES `Role` (`role_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `CustomerRoleDetails_ibfk_3` FOREIGN KEY (`rank_id`) REFERENCES `Rank` (`rank_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.CustomerRoleDetails: ~2 rows (approximately)
INSERT INTO `CustomerRoleDetails` (`user_client`, `rank_id`, `role_id`) VALUES
	('2504081426024xJhng', 7, 6),
	('250408142632ZyzD95', 7, 6);

-- Dumping structure for table E_commerce.Department
CREATE TABLE IF NOT EXISTS `Department` (
  `dep_id` tinyint NOT NULL AUTO_INCREMENT,
  `dep_name` varchar(50) DEFAULT NULL,
  `infor` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`dep_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Department: ~0 rows (approximately)

-- Dumping structure for table E_commerce.DepartmentDetails
CREATE TABLE IF NOT EXISTS `DepartmentDetails` (
  `time_of_create` datetime DEFAULT CURRENT_TIMESTAMP,
  `describe` varchar(255) DEFAULT NULL,
  `branch_id` tinyint NOT NULL,
  `dep_id` tinyint NOT NULL,
  KEY `branch_id` (`branch_id`),
  KEY `dep_id` (`dep_id`),
  CONSTRAINT `DepartmentDetails_ibfk_1` FOREIGN KEY (`branch_id`) REFERENCES `Branch` (`branch_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `DepartmentDetails_ibfk_2` FOREIGN KEY (`dep_id`) REFERENCES `Department` (`dep_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.DepartmentDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Favorite
CREATE TABLE IF NOT EXISTS `Favorite` (
  `user_client` varchar(18) DEFAULT NULL,
  `product_id` varchar(10) DEFAULT NULL,
  UNIQUE KEY `unique_favorite` (`user_client`,`product_id`),
  KEY `product_id` (`product_id`),
  KEY `idxFavorite_user_client_product_id` (`user_client`,`product_id`),
  CONSTRAINT `Favorite_ibfk_1` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Favorite_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Favorite: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Goods
CREATE TABLE IF NOT EXISTS `Goods` (
  `product_id` varchar(10) NOT NULL,
  `product_name` varchar(100) NOT NULL,
  `details` varchar(255) DEFAULT NULL,
  `characteristic` varchar(255) DEFAULT NULL,
  `num_of_view` int DEFAULT '0',
  `protyle_id` tinyint DEFAULT NULL,
  PRIMARY KEY (`product_id`),
  KEY `protyle_id` (`protyle_id`),
  KEY `idx_product_name` (`product_name`),
  KEY `idx_product_id` (`product_id`),
  CONSTRAINT `Goods_ibfk_1` FOREIGN KEY (`protyle_id`) REFERENCES `ProductType` (`protyle_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Goods: ~0 rows (approximately)

-- Dumping structure for table E_commerce.GoodsReceipt
CREATE TABLE IF NOT EXISTS `GoodsReceipt` (
  `pr_id` int NOT NULL AUTO_INCREMENT,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `total_import` decimal(10,0) NOT NULL,
  `total_VAT` decimal(10,0) NOT NULL,
  `total_value_of_receipt` decimal(10,0) NOT NULL,
  `note` varchar(255) NOT NULL,
  `user_emp` varchar(18) NOT NULL,
  `sup_id` tinyint NOT NULL,
  `ps_id` int NOT NULL,
  PRIMARY KEY (`pr_id`),
  KEY `sup_id` (`sup_id`),
  KEY `user_emp` (`user_emp`),
  KEY `ps_id` (`ps_id`),
  CONSTRAINT `GoodsReceipt_ibfk_1` FOREIGN KEY (`sup_id`) REFERENCES `Supplier` (`sup_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `GoodsReceipt_ibfk_2` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `GoodsReceipt_ibfk_3` FOREIGN KEY (`ps_id`) REFERENCES `PaymentSlip` (`ps_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.GoodsReceipt: ~0 rows (approximately)

-- Dumping structure for table E_commerce.GoodsReceiptDetails
CREATE TABLE IF NOT EXISTS `GoodsReceiptDetails` (
  `quantity` int NOT NULL,
  `unit_price` decimal(10,0) NOT NULL,
  `pr_id` int NOT NULL,
  `product_id` varchar(18) NOT NULL,
  KEY `pr_id` (`pr_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `GoodsReceiptDetails_ibfk_1` FOREIGN KEY (`pr_id`) REFERENCES `GoodsReceipt` (`pr_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `GoodsReceiptDetails_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.GoodsReceiptDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.GroupChat
CREATE TABLE IF NOT EXISTS `GroupChat` (
  `group_id` int NOT NULL AUTO_INCREMENT,
  `group_name` varchar(100) NOT NULL,
  `group_type` tinyint(1) DEFAULT '0',
  `join_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `user_id` varchar(18) NOT NULL,
  PRIMARY KEY (`group_id`),
  KEY `user_id` (`user_id`),
  CONSTRAINT `GroupChat_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.GroupChat: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Images
CREATE TABLE IF NOT EXISTS `Images` (
  `img_id` int NOT NULL AUTO_INCREMENT,
  `public_id` varchar(100) NOT NULL,
  `path_img` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`img_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Images: ~0 rows (approximately)

-- Dumping structure for table E_commerce.LoginHistory
CREATE TABLE IF NOT EXISTS `LoginHistory` (
  `login_id` int NOT NULL AUTO_INCREMENT,
  `device_name` varchar(50) NOT NULL,
  `detail` varchar(255) DEFAULT NULL,
  `ipv4` varchar(15) NOT NULL,
  `_time` datetime NOT NULL,
  `user_id` varchar(18) NOT NULL,
  `device_id` varchar(100) DEFAULT NULL,
  `location` varchar(100) DEFAULT NULL,
  `os_name` varchar(50) DEFAULT NULL,
  `browser` varchar(50) DEFAULT NULL,
  `login_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `logout_time` datetime DEFAULT NULL,
  `login_status` varchar(20) NOT NULL,
  `login_method` varchar(20) NOT NULL,
  PRIMARY KEY (`login_id`),
  KEY `index_user_id` (`user_id`),
  CONSTRAINT `FK_LoginHistory_User` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.LoginHistory: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Message
CREATE TABLE IF NOT EXISTS `Message` (
  `mess_id` varchar(18) NOT NULL,
  `text` varchar(255) NOT NULL,
  `send_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `from_number` varchar(18) NOT NULL,
  `conversation_id` int NOT NULL,
  PRIMARY KEY (`mess_id`),
  KEY `conversation_id` (`conversation_id`),
  CONSTRAINT `Message_ibfk_1` FOREIGN KEY (`conversation_id`) REFERENCES `Conversation` (`conversation_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Message: ~0 rows (approximately)

-- Dumping structure for table E_commerce.OAuthProvider
CREATE TABLE IF NOT EXISTS `OAuthProvider` (
  `provider_id` tinyint NOT NULL AUTO_INCREMENT,
  `oauth_name` varchar(20) NOT NULL,
  `client_id` varchar(100) NOT NULL,
  `client_secret` varchar(100) NOT NULL,
  `enabled` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`provider_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.OAuthProvider: ~0 rows (approximately)

-- Dumping structure for table E_commerce.PaymentMethod
CREATE TABLE IF NOT EXISTS `PaymentMethod` (
  `pmt_id` tinyint NOT NULL AUTO_INCREMENT,
  `pmt_name` varchar(50) DEFAULT NULL,
  `account_number` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`pmt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.PaymentMethod: ~0 rows (approximately)

-- Dumping structure for table E_commerce.PaymentSlip
CREATE TABLE IF NOT EXISTS `PaymentSlip` (
  `ps_id` int NOT NULL AUTO_INCREMENT,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `payment_amount` decimal(10,0) NOT NULL,
  `note` varchar(15) NOT NULL,
  `user_emp` varchar(18) NOT NULL,
  `sup_id` tinyint NOT NULL,
  PRIMARY KEY (`ps_id`),
  KEY `sup_id` (`sup_id`),
  KEY `user_emp` (`user_emp`),
  CONSTRAINT `PaymentSlip_ibfk_1` FOREIGN KEY (`sup_id`) REFERENCES `Supplier` (`sup_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `PaymentSlip_ibfk_2` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.PaymentSlip: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Permission
CREATE TABLE IF NOT EXISTS `Permission` (
  `permission_id` tinyint NOT NULL AUTO_INCREMENT,
  `can_add` tinyint(1) NOT NULL,
  `can_modify` tinyint(1) NOT NULL,
  `can_delete` tinyint(1) NOT NULL,
  `can_view` tinyint(1) NOT NULL,
  `web_id` tinyint NOT NULL,
  `dep_id` tinyint NOT NULL,
  `resource` varchar(50) NOT NULL,
  PRIMARY KEY (`permission_id`) USING BTREE,
  KEY `web_id` (`web_id`),
  KEY `dep_id` (`dep_id`),
  CONSTRAINT `Permission_ibfk_1` FOREIGN KEY (`web_id`) REFERENCES `WebSite` (`web_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Permission_ibfk_2` FOREIGN KEY (`dep_id`) REFERENCES `Department` (`dep_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Permission: ~0 rows (approximately)

-- Dumping structure for table E_commerce.PositionStaff
CREATE TABLE IF NOT EXISTS `PositionStaff` (
  `position_id` int NOT NULL AUTO_INCREMENT,
  `position_name` varchar(50) DEFAULT NULL,
  `allowance_coefficient` int DEFAULT '0',
  PRIMARY KEY (`position_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.PositionStaff: ~0 rows (approximately)

-- Dumping structure for table E_commerce.ProductPhoto
CREATE TABLE IF NOT EXISTS `ProductPhoto` (
  `img_id` int NOT NULL,
  `product_id` varchar(10) NOT NULL,
  KEY `img_id` (`img_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `ProductPhoto_ibfk_1` FOREIGN KEY (`img_id`) REFERENCES `Images` (`img_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ProductPhoto_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.ProductPhoto: ~0 rows (approximately)

-- Dumping structure for table E_commerce.ProductType
CREATE TABLE IF NOT EXISTS `ProductType` (
  `protyle_id` tinyint NOT NULL AUTO_INCREMENT,
  `protyle_name` varchar(50) NOT NULL,
  `alias_name` varchar(50) DEFAULT NULL,
  `details` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`protyle_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.ProductType: ~0 rows (approximately)

-- Dumping structure for table E_commerce.ProductTypeDetails
CREATE TABLE IF NOT EXISTS `ProductTypeDetails` (
  `protyle_id` tinyint NOT NULL,
  `sup_id` tinyint NOT NULL,
  KEY `protyle_id` (`protyle_id`),
  KEY `sup_id` (`sup_id`),
  CONSTRAINT `ProductTypeDetails_ibfk_1` FOREIGN KEY (`protyle_id`) REFERENCES `ProductType` (`protyle_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ProductTypeDetails_ibfk_2` FOREIGN KEY (`sup_id`) REFERENCES `Supplier` (`sup_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.ProductTypeDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Promotion
CREATE TABLE IF NOT EXISTS `Promotion` (
  `promo_id` int NOT NULL AUTO_INCREMENT,
  `promo_name` varchar(50) DEFAULT NULL,
  `discount` int DEFAULT '0',
  `start_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `end_time` datetime DEFAULT NULL,
  PRIMARY KEY (`promo_id`),
  CONSTRAINT `Promotion_chk_1` CHECK ((`end_time` > `start_time`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Promotion: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Rank
CREATE TABLE IF NOT EXISTS `Rank` (
  `rank_id` tinyint NOT NULL AUTO_INCREMENT,
  `rank_name` varchar(50) NOT NULL,
  `rating_point` int DEFAULT '0',
  `describe` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`rank_id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Rank: ~7 rows (approximately)
INSERT INTO `Rank` (`rank_id`, `rank_name`, `rating_point`, `describe`) VALUES
	(1, 'Admin', 0, 'Quản trị viên'),
	(2, 'Đồng', 30, 'Ưu đãi lên 5%'),
	(3, 'Bạc', 70, 'Ưu đãi lên 10%'),
	(4, 'Vàng', 100, 'Ưu đãi lên 12%'),
	(5, 'Bạch Kim', 150, 'Ưu đãi lên 15%'),
	(6, 'Kim cương', 200, 'Ưu đãi lên 20%'),
	(7, 'Thành viên', 0, 'Ưu đãi 0%');

-- Dumping structure for table E_commerce.RefreshToken
CREATE TABLE IF NOT EXISTS `RefreshToken` (
  `user_id` varchar(18) NOT NULL,
  `token` varchar(255) DEFAULT NULL,
  `jwt` varchar(255) DEFAULT NULL,
  `is_use` tinyint(1) DEFAULT '1',
  `is_revoked` tinyint(1) DEFAULT '0',
  `is_sure_at` datetime DEFAULT CURRENT_TIMESTAMP,
  `expired` datetime DEFAULT NULL,
  KEY `user_id` (`user_id`),
  CONSTRAINT `RefreshToken_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.RefreshToken: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Reviews
CREATE TABLE IF NOT EXISTS `Reviews` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `user_client` varchar(18) NOT NULL,
  `product_id` varchar(18) NOT NULL,
  `rating` int DEFAULT NULL,
  `note` text,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `Reviews_ibfk_2` (`product_id`),
  KEY `idxReviews_user_client_product_id` (`user_client`,`product_id`),
  CONSTRAINT `Reviews_ibfk_1` FOREIGN KEY (`user_client`) REFERENCES `Customer` (`user_client`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Reviews_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `Reviews_chk_1` CHECK ((`rating` between 1 and 5))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Reviews: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Role
CREATE TABLE IF NOT EXISTS `Role` (
  `role_id` tinyint NOT NULL AUTO_INCREMENT,
  `role_name` varchar(50) DEFAULT NULL,
  `describe` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`role_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Role: ~7 rows (approximately)
INSERT INTO `Role` (`role_id`, `role_name`, `describe`) VALUES
	(1, 'Admin', 'Quản trị viên'),
	(2, 'WarehouseStaff', 'Nhân viên bên kho'),
	(3, 'SalesAgent', 'Nhân viên bán hàng trên khắp cửa hàng'),
	(6, 'Client', 'Người tiêu dùng mua sắm'),
	(7, 'Employee', 'Nhân viên'),
	(9, 'Test_06', 'hehkkke'),
	(10, 'Test_07', 'hehkkke');

-- Dumping structure for table E_commerce.RolePermission
CREATE TABLE IF NOT EXISTS `RolePermission` (
  `role_permission_id` int NOT NULL AUTO_INCREMENT,
  `role_id` tinyint NOT NULL,
  `permission_id` tinyint NOT NULL,
  PRIMARY KEY (`role_permission_id`),
  KEY `idx_role_permission_role_id` (`role_id`),
  KEY `idx_role_permission_permission_id` (`permission_id`),
  CONSTRAINT `RolePermission_ibfk_1` FOREIGN KEY (`role_id`) REFERENCES `Role` (`role_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `RolePermission_ibfk_2` FOREIGN KEY (`permission_id`) REFERENCES `Permission` (`permission_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.RolePermission: ~0 rows (approximately)

-- Dumping structure for table E_commerce.SalesPrice
CREATE TABLE IF NOT EXISTS `SalesPrice` (
  `price` decimal(10,0) NOT NULL,
  `num_of_product` int NOT NULL,
  `_time` datetime DEFAULT CURRENT_TIMESTAMP,
  `product_id` varchar(10) NOT NULL,
  `promo_id` int NOT NULL,
  KEY `product_id` (`product_id`),
  KEY `promo_id` (`promo_id`),
  CONSTRAINT `SalesPrice_ibfk_1` FOREIGN KEY (`product_id`) REFERENCES `Goods` (`product_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `SalesPrice_ibfk_2` FOREIGN KEY (`promo_id`) REFERENCES `Promotion` (`promo_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.SalesPrice: ~0 rows (approximately)

-- Dumping structure for table E_commerce.ShippingBill
CREATE TABLE IF NOT EXISTS `ShippingBill` (
  `_time` datetime NOT NULL,
  `fee` decimal(10,0) DEFAULT NULL,
  `CURRENT_location` varchar(255) NOT NULL,
  `destination` varchar(255) NOT NULL,
  `status_id` tinyint NOT NULL,
  `bill_id` varchar(15) NOT NULL,
  KEY `status_id` (`status_id`),
  KEY `bill_id` (`bill_id`),
  CONSTRAINT `ShippingBill_ibfk_1` FOREIGN KEY (`status_id`) REFERENCES `Status` (`status_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `ShippingBill_ibfk_2` FOREIGN KEY (`bill_id`) REFERENCES `Bill` (`bill_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.ShippingBill: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Staff
CREATE TABLE IF NOT EXISTS `Staff` (
  `user_emp` varchar(18) NOT NULL,
  PRIMARY KEY (`user_emp`),
  KEY `idx_staff_user_emp` (`user_emp`),
  CONSTRAINT `Staff_ibfk_1` FOREIGN KEY (`user_emp`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Staff: ~0 rows (approximately)

-- Dumping structure for table E_commerce.StaffRoleDetails
CREATE TABLE IF NOT EXISTS `StaffRoleDetails` (
  `user_emp` varchar(18) NOT NULL,
  `describe` varchar(255) DEFAULT NULL,
  `role_id` tinyint NOT NULL,
  KEY `idx_staff_role_user` (`user_emp`),
  KEY `idx_staff_role_id` (`role_id`),
  CONSTRAINT `StaffRoleDetails_ibfk_1` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `StaffRoleDetails_ibfk_2` FOREIGN KEY (`role_id`) REFERENCES `Role` (`role_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.StaffRoleDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Status
CREATE TABLE IF NOT EXISTS `Status` (
  `status_id` tinyint NOT NULL AUTO_INCREMENT,
  `status_name` varchar(50) DEFAULT NULL,
  `detail` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`status_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Status: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Supplier
CREATE TABLE IF NOT EXISTS `Supplier` (
  `sup_id` tinyint NOT NULL AUTO_INCREMENT,
  `sup_name` varchar(50) NOT NULL,
  `email` varchar(100) NOT NULL,
  `phone_num` varchar(10) NOT NULL,
  `address` varchar(255) NOT NULL,
  `contact_person` varchar(100) NOT NULL,
  `detail` varchar(255) DEFAULT NULL,
  `tax_code` varchar(20) NOT NULL,
  PRIMARY KEY (`sup_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Supplier: ~0 rows (approximately)

-- Dumping structure for table E_commerce.SupplierLogo
CREATE TABLE IF NOT EXISTS `SupplierLogo` (
  `img_id` int NOT NULL,
  `sup_id` tinyint NOT NULL,
  KEY `img_id` (`img_id`),
  KEY `sup_id` (`sup_id`),
  CONSTRAINT `SupplierLogo_ibfk_1` FOREIGN KEY (`img_id`) REFERENCES `Images` (`img_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `SupplierLogo_ibfk_2` FOREIGN KEY (`sup_id`) REFERENCES `Supplier` (`sup_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.SupplierLogo: ~0 rows (approximately)

-- Dumping structure for table E_commerce.Topic
CREATE TABLE IF NOT EXISTS `Topic` (
  `topic_id` tinyint NOT NULL AUTO_INCREMENT,
  `topic_name` varchar(50) DEFAULT NULL,
  `user_emp` varchar(18) DEFAULT NULL,
  PRIMARY KEY (`topic_id`),
  KEY `user_emp` (`user_emp`),
  CONSTRAINT `Topic_ibfk_1` FOREIGN KEY (`user_emp`) REFERENCES `Staff` (`user_emp`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.Topic: ~0 rows (approximately)

-- Dumping structure for table E_commerce.User
CREATE TABLE IF NOT EXISTS `User` (
  `user_id` varchar(18) NOT NULL,
  `user_name` varchar(100) NOT NULL,
  `date_of_birth` date DEFAULT NULL,
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `phone_num` varchar(10) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `pass_word` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `is_block` tinyint(1) DEFAULT '0',
  `is_delete` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `phone_num` (`phone_num`),
  UNIQUE KEY `email` (`email`),
  KEY `idx_user_name` (`user_name`),
  KEY `idx_phone_num` (`phone_num`),
  KEY `idx_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.User: ~9 rows (approximately)
INSERT INTO `User` (`user_id`, `user_name`, `date_of_birth`, `address`, `phone_num`, `email`, `pass_word`, `is_block`, `is_delete`) VALUES
	('101345978971324416', 'kiem tra', NULL, NULL, NULL, 'baophamgia2002@gmail.com', NULL, 0, 0),
	('101345978971324417', 'kiem tra', NULL, NULL, NULL, 'pgiabao2002@gmail.com', NULL, 0, 0),
	('250406043636JWJ99W', 'Nguyễn Tuấn Đạt 09', '2025-10-15', 'phường 3, Bạc Liêu hóa Cà Mau', '0989337220', 'ntdat123@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$jg0g6LHrZ13XelakUQbSLQ$MLPN5Ia/o/K3Njyd3+9GLUjf2z8bdL6YEA7Uw/nF0G8', 0, 0),
	('250406084713q43urf', 'Nguyễn Tuấn Đạt G', '2025-10-15', 'phường 3, Bạc Liêu', '0989337266', 'ntdat1234@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$xABR1w02kzsMjNxvr/EL6w$zbznHXaEeKxbWCiHYLDLCJmvbumTXilGMC5wtcv2jos', 0, 0),
	('250406093912S40xtY', 'Nguyễn Tuấn Đạt C', '2025-10-15', 'phường 3, Bạc Liêu', '0989337268', 'ntdat1236@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$cQp0/pPDlEaOrfWkxNONUA$tZHkjS33aINDxDG01Or4YY1WznUxjt4iPsrn6jG18fw', 0, 0),
	('250406133245SAEAqD', 'Nguyễn Tuấn Đạt C9', '2025-10-15', 'phường 3, Bạc Liêu', '0989337269', 'ntdat1237@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$nTeis152E9YgItd/ePSZpA$/I1PN3MqODQNLOGKSDEgMc0VgIao64+In8ARAMn6u/E', 0, 0),
	('250407133022ULDDJ2', 'Trần Kiến A', '2025-10-09', 'phường 3, Bạc Liêu', '0989337780', 'ntdat1248@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$FxD7jOmng568jKtblQqcFg$p0KLrkQUEBnMxuSuq3b8e4E8Mc+g9QYb33KNd1fu3mE', 0, 0),
	('250407143917yKNgCm', 'Trần Kiến Quốc', '2025-10-15', 'phường 3, Bạc Liêu', '0989337769', 'ntdat1238@gmail.com', '$argon2id$v=19$m=65536,t=3,p=1$P/TKD0F1IRPpNZJeAtgz9g$LQkTYsw8qHMYKC03cVnGlDb9XQ2JldecaozR+tjeP1U', 0, 0),
	('2504081421144zdkMV', 'Trần Kiến Trung', '2025-10-15', 'phường 3, Bạc Liêu', '0989337760', 'ntdat1231@gmail.com', '$2a$11$T.ryKjJixxdh9bVgkvGwReeGakjJvfxO3bDrU9fOBL1ypoDENusyG', 0, 0),
	('2504081426024xJhng', 'Trần Kiến Trung', '2025-10-15', 'phường 3, Bạc Liêu', '0989337764', 'ntdat1233@gmail.com', '$2a$11$3FgnZCBPVyghqeeDzMq5EuZIcyLMzOszZdbWQ7gLt7SF3Du1zIbw6', 0, 0),
	('250408142632ZyzD95', 'Trần Kiến Đăng', '2025-10-15', 'phường 3, Bạc Liêu', '0989327764', 'ntdat1533@gmail.com', '$2a$11$dfrP5/Z7yBh.0SAbk5nXBuo/GCoTKbouyBA5CrnJO9nwK5zZP/VLq', 0, 0);

-- Dumping structure for table E_commerce.UserOauth
CREATE TABLE IF NOT EXISTS `UserOauth` (
  `provider_id` tinyint NOT NULL,
  `user_id` varchar(18) NOT NULL,
  `external_id` varchar(100) NOT NULL,
  `access_token` varchar(255) NOT NULL,
  `refresh_token` varchar(255) NOT NULL,
  `expiry_date` datetime NOT NULL,
  UNIQUE KEY `external_id` (`external_id`),
  KEY `idx_user_oauth_user_id` (`user_id`),
  KEY `idx_user_oauth_provider_id` (`provider_id`),
  CONSTRAINT `UserOAuth_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `UserOAuth_ibfk_2` FOREIGN KEY (`provider_id`) REFERENCES `OAuthProvider` (`provider_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.UserOauth: ~0 rows (approximately)

-- Dumping structure for table E_commerce.UserPhotoDetails
CREATE TABLE IF NOT EXISTS `UserPhotoDetails` (
  `user_id` varchar(18) NOT NULL,
  `img_id` int NOT NULL,
  KEY `user_id` (`user_id`),
  KEY `img_id` (`img_id`),
  CONSTRAINT `UserPhotoDetails_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `User` (`user_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `UserPhotoDetails_ibfk_2` FOREIGN KEY (`img_id`) REFERENCES `Images` (`img_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.UserPhotoDetails: ~0 rows (approximately)

-- Dumping structure for table E_commerce.WebSite
CREATE TABLE IF NOT EXISTS `WebSite` (
  `web_id` tinyint NOT NULL AUTO_INCREMENT,
  `web_name` varchar(50) DEFAULT NULL,
  `url` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`web_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table E_commerce.WebSite: ~0 rows (approximately)

-- Dumping structure for trigger E_commerce.before_insert_message
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `before_insert_message` BEFORE INSERT ON `Message` FOR EACH ROW BEGIN 
	IF NEW.mess_id IS NULL THEN
		SET NEW.mess_id = UUID_SHORT();
	END iF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
