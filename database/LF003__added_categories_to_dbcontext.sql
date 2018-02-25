ALTER TABLE [Posts] DROP CONSTRAINT [FK_Posts_Category_CategoryId];

GO

ALTER TABLE [Category] DROP CONSTRAINT [PK_Category];

GO

EXEC sp_rename N'Category', N'Categories';

GO

ALTER TABLE [Categories] ADD CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]);

GO

ALTER TABLE [Posts] ADD CONSTRAINT [FK_Posts_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180223141507_added categories to dbcontext', N'2.0.1-rtm-125');

GO

