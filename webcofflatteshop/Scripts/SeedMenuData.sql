-- Run this script in SQL Server Management Studio after selecting your shop database.
-- It is idempotent: existing rows are updated, missing rows are inserted.
-- Use it when you want to refresh sample menu/category data quickly without creating a new EF migration.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.Categories ON;

MERGE dbo.Categories AS Target
USING (VALUES
    (1, N'Coffee'),
    (2, N'Matcha'),
    (3, N'Chocolate'),
    (4, N'Bakery')
) AS Source (Id, Name)
ON Target.Id = Source.Id
WHEN MATCHED THEN
    UPDATE SET Name = Source.Name
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Name) VALUES (Source.Id, Source.Name);

SET IDENTITY_INSERT dbo.Categories OFF;

SET IDENTITY_INSERT dbo.Products ON;

MERGE dbo.Products AS Target
USING (VALUES
    (1, N'Espresso', CAST(2.50 AS decimal(18,2)), N'Đậm đà vị cà phê Ý nguyên bản.', 1, CAST(NULL AS nvarchar(300))),
    (2, N'Americano', CAST(3.00 AS decimal(18,2)), N'Espresso pha loãng, nhẹ nhàng và thơm.', 1, CAST(NULL AS nvarchar(300))),
    (3, N'Cappuccino', CAST(3.80 AS decimal(18,2)), N'Bọt sữa mịn, cân bằng giữa sữa và cà phê.', 1, CAST(NULL AS nvarchar(300))),
    (4, N'Latte', CAST(4.20 AS decimal(18,2)), N'Sữa béo mượt cùng espresso dịu êm.', 1, CAST(NULL AS nvarchar(300))),
    (5, N'Mocha', CAST(4.50 AS decimal(18,2)), N'Hòa quyện cà phê và chocolate ngọt ngào.', 1, CAST(NULL AS nvarchar(300))),
    (6, N'Caramel Macchiato', CAST(4.90 AS decimal(18,2)), N'Vị caramel thơm, hậu vị espresso mạnh.', 1, CAST(NULL AS nvarchar(300))),
    (7, N'Cold Brew', CAST(4.30 AS decimal(18,2)), N'Ủ lạnh 18 tiếng, mượt và ít chua.', 1, CAST(NULL AS nvarchar(300))),
    (8, N'Vietnamese Iced Coffee', CAST(3.70 AS decimal(18,2)), N'Cà phê sữa đá đậm vị Việt Nam.', 1, CAST(NULL AS nvarchar(300))),
    (9, N'Matcha Latte', CAST(4.60 AS decimal(18,2)), N'Trà xanh Nhật kết hợp sữa thanh dịu.', 2, CAST(NULL AS nvarchar(300))),
    (10, N'Chocolate Frappe', CAST(5.20 AS decimal(18,2)), N'Đá xay chocolate mát lạnh cho ngày hè.', 3, CAST(NULL AS nvarchar(300))),
    (11, N'Croissant Butter', CAST(2.90 AS decimal(18,2)), N'Bánh sừng bò bơ giòn tan mỗi sáng.', 4, CAST(NULL AS nvarchar(300))),
    (12, N'Tiramisu', CAST(4.10 AS decimal(18,2)), N'Bánh tiramisu mềm mịn thơm cà phê.', 4, CAST(NULL AS nvarchar(300))),
    (13, N'Blueberry Cheesecake', CAST(4.80 AS decimal(18,2)), N'Cheesecake béo nhẹ phủ mứt việt quất.', 4, CAST(NULL AS nvarchar(300)))
) AS Source (Id, Name, Price, Description, CategoryId, ImageUrl)
ON Target.Id = Source.Id
WHEN MATCHED THEN
    UPDATE SET
        Name = Source.Name,
        Price = Source.Price,
        Description = Source.Description,
        CategoryId = Source.CategoryId,
        ImageUrl = COALESCE(Target.ImageUrl, Source.ImageUrl)
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Name, Price, Description, CategoryId, ImageUrl)
    VALUES (Source.Id, Source.Name, Source.Price, Source.Description, Source.CategoryId, Source.ImageUrl);

SET IDENTITY_INSERT dbo.Products OFF;

COMMIT TRANSACTION;
