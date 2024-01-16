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



CREATE TABLE Habits (
    ID INT IDENTITY PRIMARY KEY,
    Name VARCHAR(255),
    Frequency INT,
    Repeat INT,
    StartDate DATE
);

CREATE TABLE Progresses (
    ID INT IDENTITY PRIMARY KEY, 
    HabitID INT,
    HabitProgress INT,
    IsCompleted BIT,
    Note TEXT,
    EndDate DATE,
    FOREIGN KEY (HabitID) REFERENCES Habits (ID)
);



/*******************************************************************************
   Populate Tables
********************************************************************************/


INSERT INTO [Habits] ([Name], [Frequency], [Repeat], [StartDate])
VALUES 
('Exercise', 5, 1, '2023-10-25'),
('Reading', 3, 2, '2023-10-26'),
('Running', 4, 1, '2023-10-25')


INSERT INTO [Progresses] ([HabitID], [HabitProgress], [IsCompleted], [Note], [EndDate])
VALUES 
(1,  100, 1, 'Feeling great!', '2023-10-29'),
(2, 50, 1, 'Finished a book!', '2023-10-27'),
(3, 80, 1, 'Improved my pace', '2023-10-28')


/*******************************************************************************
   Drop Tables
********************************************************************************/

DROP TABLE Users

DROP TABLE Skills

drop table Images

