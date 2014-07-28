CREATE DATABASE MovieTrack;
GO
--------------------------------------------------------------------------------------------------
USE MovieTrack;
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Movie 
( 
	movieID INT IDENTITY PRIMARY KEY,
	imdb_id VARCHAR(100) UNIQUE NOT NULL,
	name VARCHAR(100) NOT NULL,
	year VARCHAR(4),
	thumbnail VARCHAR(200),
	video VARCHAR(200),
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Actor
( 
	actorID INT IDENTITY PRIMARY KEY,
	name VARCHAR(100) UNIQUE NOT NULL,
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Genre
( 
	genreID INT IDENTITY PRIMARY KEY,
	name VARCHAR(100) UNIQUE NOT NULL,
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Lang
( 
	langID INT IDENTITY PRIMARY KEY,
	name VARCHAR(100) UNIQUE NOT NULL,
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Subtitle 
( 
	subtitleID INT IDENTITY PRIMARY KEY,
	status INT NOT NULL,
	movieID INT NOT NULL FOREIGN KEY REFERENCES Movie(movieID) ON DELETE CASCADE,
	langID INT NOT NULL FOREIGN KEY REFERENCES Lang(langID) ON DELETE CASCADE,
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Movie_Actor
( 
	id INT IDENTITY PRIMARY KEY,
	movieID INT NOT NULL FOREIGN KEY REFERENCES Movie(movieID) ON DELETE CASCADE,
	actorID INT NOT NULL FOREIGN KEY REFERENCES Actor(actorID) ON DELETE CASCADE,
	UNIQUE (movieID, actorID)
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Movie_Genre
( 
	id INT IDENTITY PRIMARY KEY,
	movieID INT NOT NULL FOREIGN KEY REFERENCES Movie(movieID) ON DELETE CASCADE,
	genreID INT NOT NULL FOREIGN KEY REFERENCES Genre(genreID) ON DELETE CASCADE,
	UNIQUE (movieID, genreID)
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Movie_Lang
( 
	id INT IDENTITY PRIMARY KEY,
	movieID INT NOT NULL FOREIGN KEY REFERENCES Movie(movieID) ON DELETE CASCADE,
	langID INT NOT NULL FOREIGN KEY REFERENCES Lang(langID) ON DELETE CASCADE,
	UNIQUE (movieID, langID)
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE Domain
( 
	id INT IDENTITY PRIMARY KEY,
	name VARCHAR(100) UNIQUE NOT NULL,
	frequency INT NOT NULL,
); 
GO
--------------------------------------------------------------------------------------------------
CREATE TABLE DbPediaType
( 
	id INT IDENTITY PRIMARY KEY,
	name VARCHAR(100) UNIQUE NOT NULL,
); 
GO
--------------------------------------------------------------------------------------------------