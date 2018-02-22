EXEC sp_rename N'Posts.Content', N'Title', N'COLUMN';

GO

ALTER TABLE [Posts] ADD [CategoryId] int NOT NULL DEFAULT 0;

GO

CREATE TABLE [Category] (
    [Id] int NOT NULL IDENTITY,
    [CreatedBy] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [Deleted] datetime2 NULL,
    [ModifiedBy] nvarchar(max) NULL,
    [ModifiedDate] datetime2 NULL,
    [Name] nvarchar(max) NULL,
    [Version] rowversion NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
);

GO

CREATE INDEX [IX_Posts_CategoryId] ON [Posts] ([CategoryId]);

GO

ALTER TABLE [Posts] ADD CONSTRAINT [FK_Posts_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE CASCADE;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20180222141649_added category table', N'2.0.1-rtm-125');

GO

