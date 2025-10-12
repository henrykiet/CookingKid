create database cookingKid
GO
use cookingKid
GO
--drop table Menus
--drop table SubMenus
--drop table Users
--drop table Items
--drop table ItemGroups
--drop table UOMs
-- table menus 
create table Menus (
	menu_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	menu_index INT NOT NULL,
	routeLink VARCHAR(20) NOT NULL,
	label NVARCHAR(250) NOT NULL,
	icon VARCHAR(20) NOT NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NOT NULL
)

-- table sub Menus
create table SubMenus(
	menu_id INT NOT NULL,
	line_nbr INT NOT NULL,
	routeLink VARCHAR(20) NOT NULL,
	label NVARCHAR(250) NOT NULL,
	icon VARCHAR(20) NOT NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NOT NULL,
	CONSTRAINT PK_SubMenus PRIMARY KEY (menu_id, line_nbr) -- set cặp kháo chính
)

-- table users
create table Users (
	user_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, -- UNIQUEIDENTIFIER kiểu dữ liệu GUID và tự sinh khi insert
	user_name VARCHAR(250) NOT NULL,
	password VARCHAR(250) NOT NULL,
	gender CHAR(1) NULL,
	first_name NVARCHAR(250) NULL,
	last_name NVARCHAR(250) NOT NULL,
	email VARCHAR(250) NULL,
	phone VARCHAR(20) NULL,
	birth_date DATETIME NULL,
	number_wrong_password INT NOT NULL DEFAULT 0,
	is_lock BIT NOT NULL DEFAULT 0,
	status CHAR(1) NOT NULL
)

-- table item
create table Items (
	item_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, -- UNIQUEIDENTIFIER kiểu dữ liệu GUID và tự sinh khi insert
	item_name NVARCHAR(MAX) NOT NULL,
	price DECIMAL(18,2) NOT NULL, -- đơn giá mua
	purchase DECIMAL(18,2) NOT NULL, -- đơn giá bán
	item_group_id VARCHAR(100) NULL, -- mã nhóm vật tư
	uom_id VARCHAR(100) NULL, -- mã đơn vị tính
	minimum_inventory INT NULL DEFAULT 0,
	maximum_inventory INT NULL,
	manufacturer VARCHAR(100) NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NULL,
)

-- Bảng nhóm vật tư
CREATE TABLE ItemGroups (
    item_group_id VARCHAR(100) NOT NULL PRIMARY KEY, -- mã nhóm vật tư
    group_name NVARCHAR(250) NOT NULL,              -- tên nhóm vật tư
    description NVARCHAR(MAX) NULL,                 -- mô tả nhóm
	status CHAR(1) NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
);

-- Bảng đơn vị tính
CREATE TABLE UOMs (
    uom_id VARCHAR(100) NOT NULL PRIMARY KEY,         -- mã đơn vị tính
    uom_name NVARCHAR(250) NOT NULL,              -- tên đơn vị tính
    description NVARCHAR(MAX) NULL,                -- mô tả
	status CHAR(1) NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
);

GO

--INSERT MENUS
INSERT INTO Menus (menu_index, routeLink, label, icon, create_by, status)
VALUES 
(1, 'dashboard', N'DashBoard', 'fas fa-chart-bar', 'system', 'A'),
(2, '', N'Danh mục', 'fas fa-list', 'system', 'A');

--INSERT SUBMENUS
INSERT INTO SubMenus (menu_id, line_nbr, routeLink, label, icon, create_by, status)
VALUES 
(2, 1, 'users', N'Users', 'fas fa-users', 'system', 'A'),
(2, 2, 'items', N'Items', 'fas fa-list-alt', 'system', 'A'),
(2, 3, 'item-groups', N'Item Groups', 'fas fa-list-alt', 'system', 'A');

-- INSERT USERS
INSERT INTO Users (user_name, password, gender, first_name, last_name, email, phone, birth_date, status)
VALUES
('admin', 'admin123', 'M', 'Nguyen', 'Van A', 'admin@example.com', '0912345678', '1985-01-01', 'A'),
('user1', 'pass1', 'F', 'Le', 'Thi B', 'user1@example.com', '0912345671', '1990-02-12', 'A'),
('user2', 'pass2', 'M', 'Tran', 'Van C', 'user2@example.com', '0912345672', '1988-05-23', 'A'),
('user3', 'pass3', 'F', 'Pham', 'Thi D', 'user3@example.com', '0912345673', '1992-07-15', 'A'),
('user4', 'pass4', 'M', 'Hoang', 'Van E', 'user4@example.com', '0912345674', '1987-09-30', 'A'),
('user5', 'pass5', 'F', 'Do', 'Thi F', 'user5@example.com', '0912345675', '1995-03-20', 'A'),
('user6', 'pass6', 'M', 'Bui', 'Van G', 'user6@example.com', '0912345676', '1986-12-10', 'A'),
('user7', 'pass7', 'F', 'Nguyen', 'Thi H', 'user7@example.com', '0912345677', '1991-08-05', 'A'),
('user8', 'pass8', 'M', 'Le', 'Van I', 'user8@example.com', '0912345678', '1989-11-25', 'A'),
('user9', 'pass9', 'F', 'Tran', 'Thi J', 'user9@example.com', '0912345679', '1993-06-17', 'A');

--INSERT ITEM GROUP
INSERT INTO ItemGroups (item_group_id, group_name, description, status, create_by)
VALUES
('G001', 'Electronics', N'Nhóm đồ điện tử', 'A', 'admin'),
('G002', 'Stationery', N'Nhóm văn phòng phẩm', 'A', 'admin'),
('G003', 'Furniture', N'Nhóm nội thất', 'A', 'admin'),
('G004', 'Food', N'Nhóm thực phẩm', 'A', 'admin'),
('G005', 'Clothing', N'Nhóm quần áo', 'A', 'admin'),
('G006', 'Books', N'Nhóm sách', 'A', 'admin'),
('G007', 'Tools', N'Nhóm dụng cụ', 'A', 'admin'),
('G008', 'Cosmetics', N'Nhóm mỹ phẩm', 'A', 'admin'),
('G009', 'Toys', N'Nhóm đồ chơi', 'A', 'admin'),
('G010', 'Sports', N'Nhóm thể thao', 'A', 'admin');


--INSERT OUMs
INSERT INTO UOMs (uom_id, uom_name, description, status, create_by)
VALUES
('U001', 'Piece', 'Cái', 'A', 'admin'),
('U002', 'Box', 'Hộp', 'A', 'admin'),
('U003', 'Pack', 'Gói', 'A', 'admin'),
('U004', 'Kg', 'Kilogram', 'A', 'admin'),
('U005', 'Litre', 'Lít', 'A', 'admin'),
('U006', 'Meter', 'Mét', 'A', 'admin'),
('U007', 'Set', 'Bộ', 'A', 'admin'),
('U008', 'Dozen', 'Tá', 'A', 'admin'),
('U009', 'Bottle', 'Chai', 'A', 'admin'),
('U010', 'Packet', 'Gói nhỏ', 'A', 'admin');

--INSERT ITEMs
INSERT INTO Items (item_name, price, purchase, item_group_id, uom_id, minimum_inventory, maximum_inventory, manufacturer, create_by, status)
VALUES
('Laptop Dell', 1500.00, 2000.00, 'G001', 'U001', 5, 50, 'Dell', 'admin', 'A'),
('Pen Blue', 0.50, 1.00, 'G002', 'U001', 50, 500, 'Thiên Long', 'admin', 'A'),
('Office Chair', 75.00, 120.00, 'G003', 'U001', 2, 20, 'Ikea', 'admin', 'A'),
('Apple', 1.00, 1.50, 'G004', 'U004', 10, 200, 'VinFruit', 'admin', 'A'),
('T-Shirt', 5.00, 10.00, 'G005', 'U001', 10, 100, 'Nike', 'admin', 'A'),
('Book "SQL Basics"', 10.00, 15.00, 'G006', 'U001', 5, 50, 'NXB Giáo Dục', 'admin', 'A'),
('Hammer', 8.00, 12.00, 'G007', 'U001', 5, 30, 'Stanley', 'admin', 'A'),
('Lipstick', 12.00, 20.00, 'G008', 'U001', 10, 100, 'Maybelline', 'admin', 'A'),
('Toy Car', 7.00, 12.00, 'G009', 'U001', 5, 50, 'Hot Wheels', 'admin', 'A'),
('Football', 20.00, 35.00, 'G010', 'U001', 5, 30, 'Adidas', 'admin', 'A');