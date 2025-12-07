create database cookingKid
GO
use cookingKid
GO
--drop table Items
-- table Menus 
create table Menus (
	menu_id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
	menu_index INT NOT NULL,
	routeLink VARCHAR(50) NULL,
	controller VARCHAR(50) NULL,
	label NVARCHAR(250) NOT NULL,
	icon VARCHAR(50) NOT NULL,
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
	routeLink VARCHAR(50) NOT NULL,
	controller VARCHAR(50) NOT NULL,
	label NVARCHAR(250) NOT NULL,
	icon VARCHAR(50) NOT NULL,
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
	phone VARCHAR(12) NULL,
	birth_date DATE NULL,
	number_wrong_password INT NOT NULL DEFAULT 0,
	is_lock BIT NOT NULL DEFAULT 0,
	company_id VARCHAR(100) NULL, -- id nếu là công ty
	role_id VARCHAR(100) NOT NULL, -- id chức vụ (customer, employee, manager) 
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NOT NULL
)

-- table companys
create table Companys (
	company_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY, -- UNIQUEIDENTIFIER kiểu dữ liệu GUID và tự sinh khi insert
	company_name VARCHAR(250) NOT NULL,
	acronym VARCHAR(20), -- tên viết tắt công ty
	email VARCHAR(250) NULL,
	hostline VARCHAR(12) NULL,
	fax VARCHAR(20) NULL,
	birth_date DATE NULL, -- ngày thành lập
	description NVARCHAR(MAX), -- mô tả công ty
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NOT NULL
)


--table Role
create table Roles (
	role_id CHAR(100) NOT NULL PRIMARY KEY,
	role_name VARCHAR(250) NOT NULL, --customer, employee
	description NVARCHAR(MAX) NULL,
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
	update_by VARCHAR(100) NULL,
	update_at DATE NULL,
	status CHAR(1) NOT NULL --A: available, U: unAvailable 
)

-- table item
create table Items (
	item_id VARCHAR(100) NOT NULL PRIMARY KEY,
	item_name NVARCHAR(MAX) NOT NULL,
	cost_price DECIMAL(19,4) NOT NULL, -- đơn giá mua
	sale_price DECIMAL(19,4) NOT NULL, -- đơn giá bán
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

-- Bảng Thuế
CREATE TABLE Taxs (
	tax_id VARCHAR(100) NOT NULL PRIMARY KEY, -- mã thuế, ví dụ: VAT10, EXEMPT
	tax_name NVARCHAR(250) NOT NULL, -- tên thuế, ví dụ: Thuế GTGT 10%
	tax_rate DECIMAL(5, 2) NOT NULL, -- tỷ lệ thuế (ví dụ: 0.10 cho 10%)
	description NVARCHAR(MAX) NULL, -- mô tả
	status CHAR(1) NOT NULL, -- A: active, I: inactive
	create_by VARCHAR(100) NOT NULL,
	create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
	update_by VARCHAR(100) NULL,
	update_at DATE NULL
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

--Bảng Quotations
-- Khai báo biến
DECLARE @sql NVARCHAR(MAX) = N'';
DECLARE @start INT = 202501;
DECLARE @end INT = 202612;

-- Tạo cấu trúc bảng SQL động
SELECT @sql += N'
IF OBJECT_ID(''[dbo].[Quotations$' + CAST(p AS NVARCHAR(6)) + ']'') IS NULL
BEGIN
    CREATE TABLE [dbo].[Quotations$' + CAST(p AS NVARCHAR(6)) + '] (
        quotation_id VARCHAR(100) NOT NULL PRIMARY KEY, 
        quotation_code VARCHAR(100) NULL, -- số báo giá, ví dụ: BG0001
        quotation_date DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- lấy ngày tháng năm
        customer_id VARCHAR(100) NOT NULL, -- thông tin id khách (lấy từ user_id)
        employee_id VARCHAR(100) NULL, -- thông tin nhân viên tạo báo giá
        --phần thông tin khách
        email_customer VARCHAR(250) NULL,
        phone_customer VARCHAR(12) NULL,
        address_customer VARCHAR(500) NULL,
        fax_customer VARCHAR(20) NULL,
        -- phần thông tin nhân viên
        email_employee VARCHAR(250) NULL,
        phone_employee VARCHAR(12) NULL,
        fax_employee VARCHAR(20) NULL,
        --phần tổng hợp báo giá chi tiết
        total_quantity DECIMAL(19,4) NULL, --tổng số lượng
        total_amount DECIMAL(19,4) NULL, -- tổng tiền
        total_vat DECIMAL(19,4) NULL, -- tổng tiền thuế
        total_payment DECIMAL(19,4) NULL, -- tổng tiền sau thuế 
        note NVARCHAR(MAX) NULL,
        status CHAR(1) NOT NULL,
        create_by VARCHAR(100) NOT NULL,
        create_at DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE), -- chỉ lưu ngày và lấy ngày hiện tại khi insert
        update_by VARCHAR(100) NULL,
        update_at DATE NULL
    );
    -- PRINT N''Đã tạo bảng: Quotations$' + CAST(p AS NVARCHAR(6)) + '''
END
'
-- Tạo danh sách các tháng YYYYMM cần tạo bảng
FROM (
    SELECT 
        p = @start - 1 + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM sys.all_objects -- Dùng sys.all_objects thay vì sys.objects để đảm bảo đủ số lượng hàng
) AS t
-- Điều kiện lọc chỉ lấy các tháng YYYYMM hợp lệ trong phạm vi 202501 - 202612
-- và kiểm tra tính hợp lệ của tháng (MM <= 12)
WHERE p BETWEEN @start AND @end
  AND (p % 100) <= 12
  AND (p % 100) >= 1;

-- Xem trước câu lệnh SQL động sẽ được thực thi
PRINT @sql;

-- Thực thi câu lệnh SQL động để tạo các bảng
-- BỎ DÒNG COMMENT DƯỚI ĐÂY NẾU BẠN CHẮC CHẮN MUỐN CHẠY LỆNH TẠO BẢNG
EXEC sp_executesql @sql;

GO

--bảng QuotaionDetails
-- Khai báo biến
DECLARE @sql NVARCHAR(MAX) = N'';
DECLARE @start INT = 202501;
DECLARE @end INT = 202612;

-- Tạo cấu trúc bảng SQL động
SELECT @sql += N'
IF OBJECT_ID(''[dbo].[QuotationDetails$' + CAST(p AS NVARCHAR(6)) + ']'') IS NULL
BEGIN
    CREATE TABLE [dbo].[QuotationDetails$' + CAST(p AS NVARCHAR(6)) + '] (
        quotation_id VARCHAR(100) NOT NULL, 
		line_nbr INT NOT NULL, -- thứ tự dòng
		item_id VARCHAR(100),
		item_name NVARCHAR(500) NULL, --têm sản phẩm
        uom_id VARCHAR(100) NOT NULL, 
        tax_id VARCHAR(100) NOT NULL, -- bảng mã thuế 
		quantity INT NOT NULL,
		price DECIMAL(19,4) NULL, -- giá mua
		amount DECIMAL(19,4) NULL, -- tổng giá
        vat DECIMAL(19,4) NULL, -- tổng thuế 
        payment DECIMAL(19,4) NULL, -- tiền sau thuế
        note NVARCHAR(200) NULL,
		CONSTRAINT PK_QuotationDetails$' + CAST(p AS NVARCHAR(6)) + ' PRIMARY KEY (quotation_id, line_nbr)
    );
    -- PRINT N''Đã tạo bảng: QuotaionDetails$' + CAST(p AS NVARCHAR(6)) + '''
END
'
-- Tạo danh sách các tháng YYYYMM cần tạo bảng
FROM (
    SELECT 
        p = @start - 1 + ROW_NUMBER() OVER (ORDER BY (SELECT NULL))
    FROM sys.all_objects -- Dùng sys.all_objects thay vì sys.objects để đảm bảo đủ số lượng hàng
) AS t
-- Điều kiện lọc chỉ lấy các tháng YYYYMM hợp lệ trong phạm vi 202501 - 202612
-- và kiểm tra tính hợp lệ của tháng (MM <= 12)
WHERE p BETWEEN @start AND @end
  AND (p % 100) <= 12
  AND (p % 100) >= 1;

-- Xem trước câu lệnh SQL động sẽ được thực thi
PRINT @sql;

-- Thực thi câu lệnh SQL động để tạo các bảng
-- BỎ DÒNG COMMENT DƯỚI ĐÂY NẾU BẠN CHẮC CHẮN MUỐN CHẠY LỆNH TẠO BẢNG
EXEC sp_executesql @sql;


GO

--INSERT MENUS
INSERT INTO Menus (menu_index, routeLink, controller, label, icon, create_by, status)
VALUES 
(1, 'dashboard', 'dashboard', N'DashBoard', 'fas fa-chart-bar', 'system', 'A'),
(2, '', '', N'Catalog', 'fas fa-list', 'system', 'A'),
(3, '', '', N'Report', 'fas fa-file-invoice', 'system', 'A');
--INSERT SUBMENUS
INSERT INTO SubMenus (menu_id, line_nbr, routeLink, controller, label, icon, create_by, status)
VALUES 
(2, 1, 'grid','user', N'Users', 'fas fa-users', 'system', 'A'),
(2, 2, 'grid','item', N'Items', 'fas fa-list-alt', 'system', 'A'),
(2, 3, 'grid','item-group', N'Item Groups', 'fas fa-tags', 'system', 'A'),
(3, 1, 'grid','quotation', N'Quotations', 'fa-solid fa-money-bill-alt', 'system', 'A');

-- INSERT ROLES (Thêm 5 dòng)
INSERT INTO Roles (role_id, role_name, description, create_by, status)
VALUES
('CUSTOMER', 'Customer', N'Khách hàng mua hàng', 'system', 'A'),
('EMPLOYEE', 'Employee', N'Nhân viên bán hàng/tạo báo giá', 'system', 'A'),
('MANAGER', 'Manager', N'Quản lý hệ thống/bộ phận', 'system', 'A'),
('ADMIN', 'Administrator', N'Quản trị viên hệ thống', 'system', 'A'),
('GUEST', 'Guest', N'Người dùng vãng lai (chỉ xem)', 'system', 'A');


-- INSERT USERS
-- Lấy IDs cho việc liên kết
DECLARE @AdminRoleID CHAR(100) = 'ADMIN';
DECLARE @EmployeeRoleID CHAR(100) = 'EMPLOYEE';
DECLARE @CustomerRoleID CHAR(100) = 'CUSTOMER';
DECLARE @CompanyID UNIQUEIDENTIFIER;
SELECT @CompanyID = company_id FROM Companys WHERE acronym = 'GLOBALTECH'; -- Lấy ID của công ty mẫu

-- INSERT USERS
INSERT INTO Users (user_name, password, gender, first_name, last_name, email, phone, birth_date, role_id, company_id, create_by, status)
VALUES
('admin', 'admin123', 'M', 'Nguyen', 'Van A', 'admin@example.com', '0912345678', '1985-01-01', @AdminRoleID, NULL, 'system', 'A'), -- Admin
('user1', 'pass1', 'F', 'Le', 'Thi B', 'user1@example.com', '0912345671', '1990-02-12', @CustomerRoleID, NULL, 'system', 'A'), -- Customer
('user2', 'pass2', 'M', 'Tran', 'Van C', 'user2@example.com', '0912345672', '1988-05-23', @EmployeeRoleID, @CompanyID, 'system', 'A'), -- Employee
('user3', 'pass3', 'F', 'Pham', 'Thi D', 'user3@example.com', '0912345673', '1992-07-15', @CustomerRoleID, NULL, 'system', 'A'), -- Customer
('user4', 'pass4', 'M', 'Hoang', 'Van E', 'user4@example.com', '0912345674', '1987-09-30', @EmployeeRoleID, @CompanyID, 'system', 'A'), -- Employee
('user5', 'pass5', 'F', 'Do', 'Thi F', 'user5@example.com', '0912345675', '1995-03-20', @CustomerRoleID, NULL, 'system', 'A'), -- Customer
('user6', 'pass6', 'M', 'Bui', 'Van G', 'user6@example.com', '0912345676', '1986-12-10', @CustomerRoleID, NULL, 'system', 'A'), -- Customer
('user7', 'pass7', 'F', 'Nguyen', 'Thi H', 'user7@example.com', '0912345677', '1991-08-05', @EmployeeRoleID, @CompanyID, 'system', 'A'), -- Employee
('user8', 'pass8', 'M', 'Le', 'Van I', 'user8@example.com', '0912345678', '1989-11-25', @CustomerRoleID, NULL, 'system', 'A'), -- Customer
('user9', 'pass9', 'F', 'Tran', 'Thi J', 'user9@example.com', '0912345679', '1993-06-17', @CustomerRoleID, NULL, 'system', 'A'); -- Customer

--INERT COMPANIES 
-- Cần dùng user_id hợp lệ từ bảng Users để làm create_by
DECLARE @AdminUserID UNIQUEIDENTIFIER;
SELECT @AdminUserID = user_id FROM Users WHERE user_name = 'admin';

INSERT INTO Companys (company_name, acronym, email, hostline, fax, birth_date, description, create_by, status)
VALUES
(N'Công Ty TNHH Giải Pháp Công Nghệ Toàn Cầu', 'GLOBALTECH', 'info@globaltech.vn', '02812345678', '02812345679', '2015-05-20', N'Cung cấp các giải pháp công nghệ thông tin.', CAST(@AdminUserID AS VARCHAR(100)), 'A'),
(N'Công Ty Cổ Phần Thương Mại và Dịch Vụ ABC', 'ABC CORP', 'contact@abccorp.com', '02498765432', '02498765433', '2008-11-15', N'Thương mại và phân phối hàng tiêu dùng.', CAST(@AdminUserID AS VARCHAR(100)), 'A'),
(N'Công Ty Sản Xuất và Xây Dựng Phát Triển', 'PDC', 'sales@pdc.com.vn', '02365558888', NULL, '1999-01-01', N'Sản xuất vật liệu xây dựng.', CAST(@AdminUserID AS VARCHAR(100)), 'A'),
(N'Cửa Hàng Sách Trực Tuyến TBT', 'TBT BOOKS', 'bookstore@tbt.vn', '0901000111', NULL, '2020-03-10', N'Cửa hàng sách và văn phòng phẩm online.', CAST(@AdminUserID AS VARCHAR(100)), 'A'),
(N'Doanh Nghiệp Tư Nhân Vận Tải Nhanh', 'FASTSHIP', 'support@fastship.com', '0987654321', '0987654320', '2018-07-25', N'Dịch vụ vận chuyển và logistics.', CAST(@AdminUserID AS VARCHAR(100)), 'A');

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
INSERT INTO Items (item_id, item_name, cost_price, sale_price, item_group_id, uom_id, minimum_inventory, maximum_inventory, manufacturer, create_by, status)
VALUES
('IT001', N'Laptop Dell XPS 13', 1500.00, 2000.00, 'G001', 'U001', 5, 50, 'Dell', 'admin', 'A'),
('IT002', N'Bút Bi Thiên Long Xanh', 0.50, 1.00, 'G002', 'U001', 50, 500, 'Thiên Long', 'admin', 'A'),
('IT003', N'Ghế Văn Phòng Cao Cấp', 75.00, 120.00, 'G003', 'U001', 2, 20, 'Ikea', 'admin', 'A'),
('IT004', N'Táo Mỹ Organic', 1.00, 1.50, 'G004', 'U004', 10, 200, 'VinFruit', 'admin', 'A'),
('IT005', N'Áo Thun Cổ Tròn Trắng', 5.00, 10.00, 'G005', 'U001', 10, 100, 'Nike', 'admin', 'A'),
('IT006', N'Sách "SQL Cơ Bản"', 10.00, 15.00, 'G006', 'U001', 5, 50, 'NXB Giáo Dục', 'admin', 'A'),
('IT007', N'Búa Đinh Stanley', 8.00, 12.00, 'G007', 'U001', 5, 30, 'Stanley', 'admin', 'A'),
('IT008', N'Son Môi Matte Đỏ', 12.00, 20.00, 'G008', 'U001', 10, 100, 'Maybelline', 'admin', 'A'),
('IT009', N'Bộ Đồ Chơi Xe Hơi', 7.00, 12.00, 'G009', 'U007', 5, 50, 'Hot Wheels', 'admin', 'A'),
('IT010', N'Bóng Đá Adidas Size 5', 20.00, 35.00, 'G010', 'U001', 5, 30, 'Adidas', 'admin', 'A');

--INSET TAX
INSERT INTO Taxs (tax_id, tax_name, tax_rate, description, status, create_by)
VALUES
('VAT10', N'Thuế Giá trị gia tăng 10%', 0.10, N'Thuế GTGT tiêu chuẩn 10%', 'A', 'system'),
('VAT8', N'Thuế Giá trị gia tăng 8%', 0.08, N'Thuế GTGT giảm 8% (áp dụng tạm thời)', 'A', 'system'),
('EXEMPT', N'Không chịu thuế (Miễn thuế)', 0.00, N'Các mặt hàng/dịch vụ thuộc diện miễn thuế', 'A', 'system'),
('VAT5', N'Thuế Giá trị gia tăng 5%', 0.05, N'Thuế GTGT cho một số mặt hàng thiết yếu', 'A', 'system'),
('NONVAT', N'Không phải đối tượng chịu thuế', 0.00, N'Hàng hóa/dịch vụ không thuộc đối tượng chịu GTGT', 'A', 'system');



-- Lấy thông tin cần thiết
DECLARE @QDate DATE = '2025-10-18';
DECLARE @EmployeeID VARCHAR(100);
DECLARE @CustomerID1 VARCHAR(100), @CustomerID2 VARCHAR(100), @CustomerID3 VARCHAR(100);

SELECT TOP 1 @EmployeeID = CAST(user_id AS VARCHAR(100)) FROM Users WHERE role_id = 'EMPLOYEE' ORDER BY create_at;
SELECT @CustomerID1 = CAST(user_id AS VARCHAR(100)) FROM Users WHERE user_name = 'user1';
SELECT @CustomerID2 = CAST(user_id AS VARCHAR(100)) FROM Users WHERE user_name = 'user3';
SELECT @CustomerID3 = CAST(user_id AS VARCHAR(100)) FROM Users WHERE user_name = 'user5';

-- Tạo dữ liệu mẫu cho Quotations$202510 (5 dòng)
INSERT INTO Quotations$202510 (
    quotation_id, quotation_code, quotation_date, customer_id, employee_id, 
    email_customer, phone_customer, total_quantity, total_amount, total_vat, total_payment, 
    note, status, create_by
)
VALUES
('BG2510-001', 'BG2510001', @QDate, @CustomerID1, @EmployeeID, 
'user1@example.com', '0912345671', 55.00, 1400.00, 140.00, 1540.00, 
N'Đã gửi báo giá lần 1', 'A', @EmployeeID),

('BG2510-002', 'BG2510002', @QDate, @CustomerID2, @EmployeeID, 
'user3@example.com', '0912345673', 10.00, 200.00, 20.00, 220.00, 
N'Khách hàng yêu cầu giao sớm', 'A', @EmployeeID),

('BG2510-003', 'BG2510003', @QDate, @CustomerID3, @EmployeeID, 
'user5@example.com', '0912345675', 105.00, 100.00, 10.00, 110.00, 
N'Báo giá văn phòng phẩm', 'A', @EmployeeID),

('BG2510-004', 'BG2510004', @QDate, @CustomerID1, @EmployeeID, 
'user1@example.com', '0912345671', 1.00, 2000.00, 200.00, 2200.00, 
N'Báo giá máy tính xách tay', 'A', @EmployeeID),

('BG2510-005', 'BG2510005', @QDate, @CustomerID2, @EmployeeID, 
'user3@example.com', '0912345673', 20.00, 240.00, 24.00, 264.00, 
N'Đã chốt đơn hàng', 'A', @EmployeeID);



-- Dữ liệu này giả định Taxs đã có 'VAT10'
INSERT INTO QuotationDetails$202510 (
    quotation_id, line_nbr, item_id, item_name, uom_id, tax_id, quantity, price, amount, vat, payment, note
)
VALUES
-- BG2510-001
('BG2510-001', 1, 'IT003', N'Ghế Văn Phòng Cao Cấp', 'U001', 'VAT10', 10, 120.00, 1200.00, 120.00, 1320.00, NULL),
('BG2510-001', 2, 'IT007', N'Búa Đinh Stanley', 'U001', 'VAT10', 45, 12.00, 540.00, 54.00, 594.00, NULL),

-- BG2510-002
('BG2510-002', 1, 'IT005', N'Áo Thun Cổ Tròn Trắng', 'U001', 'VAT10', 10, 10.00, 100.00, 10.00, 110.00, N'Size L'),

-- BG2510-003
('BG2510-003', 1, 'IT002', N'Bút Bi Thiên Long Xanh', 'U001', 'VAT10', 100, 1.00, 100.00, 10.00, 110.00, NULL),
('BG2510-003', 2, 'IT006', N'Sách "SQL Cơ Bản"', 'U001', 'VAT10', 5, 15.00, 75.00, 7.50, 82.50, NULL),

-- BG2510-004
('BG2510-004', 1, 'IT001', N'Laptop Dell XPS 13', 'U001', 'VAT10', 1, 2000.00, 2000.00, 200.00, 2200.00, NULL),

-- BG2510-005
('BG2510-005', 1, 'IT003', N'Ghế Văn Phòng Cao Cấp', 'U001', 'VAT10', 2, 120.00, 240.00, 24.00, 264.00, NULL);