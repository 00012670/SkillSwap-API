USE [BISP]
GO


/*******************************************************************************
   Create Tables
********************************************************************************/

CREATE TABLE [dbo].[Users](
	[AuthId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[Role] [nvarchar](max) NULL,
	[Token] [nvarchar](max) NULL,
 CONSTRAINT [PK_authentications] PRIMARY KEY CLUSTERED 
(
	[AuthId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

CREATE TABLE [dbo].[Skills](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NULL,
    [Email] [nvarchar](max) NULL,
    [Description] [nvarchar](max) NULL,
    [Category] [nvarchar](max) NULL,
    [Level] int,
    [Prerequisity] [nvarchar](max) NULL,
    [Picture] [nvarchar](max) NULL,
    CONSTRAINT [PK_skills] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO




/*******************************************************************************
   Populate Tables
********************************************************************************/


-- Insert into User table
--INSERT INTO Users (Username, Email, Password, FullName, Bio, SkillInterested, Token, Role)
--VALUES ('user1', 'user1@example.com', '12345678!', 'User One', 'Bio for user1', 'Skill1', '', 'Admin'),
--       ('user2', 'user2@example.com', '12345678!', 'User Two', 'Bio for user2', 'Skill2', '', 'User'),
--       ('user3', 'user3@example.com', '12345678!', 'User Three', 'Bio for user3', 'Skill3', '', 'User');


---- Insert into Skill table
--INSERT INTO Skills (Name, Description, Category, Level, Prerequisity, UserId)
--VALUES ('skill1', 'Description for Skill1', 'Category1', 0, 'Prerequisity for Skill1', 1),
--       ('skill2', 'Description for Skill2', 'Category2', 1, 'Prerequisity for Skill2', 2),
--       ('skill3', 'Description for Skill3', 'Category3', 2, 'Prerequisity for Skill3', 3);




/*******************************************************************************
   Drop Tables
********************************************************************************/

DROP TABLE Users

DROP TABLE Skills

drop table Images

drop table Messages

SELECT DB_NAME() AS CurrentDatabase;

SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users';