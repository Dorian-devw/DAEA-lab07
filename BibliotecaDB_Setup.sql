
USE master;
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'BibliotecaDB')
BEGIN
    ALTER DATABASE BibliotecaDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE BibliotecaDB;
END
GO

CREATE DATABASE BibliotecaDB;
GO

USE BibliotecaDB;
GO

CREATE TABLE Autores (
    AutorId INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Nacionalidad NVARCHAR(50) NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Libros (
    LibroId INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(200) NOT NULL,
    ISBN NVARCHAR(50) NOT NULL UNIQUE,
    AutorId INT NOT NULL,
    Ejemplares INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Libros_Autores FOREIGN KEY (AutorId) REFERENCES Autores(AutorId)
);
GO

CREATE TABLE Socios (
    SocioId INT IDENTITY(1,1) PRIMARY KEY,
    DNI NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Prestamos (
    PrestamoId INT IDENTITY(1,1) PRIMARY KEY,
    SocioId INT NOT NULL,
    FechaPrestamo DATETIME NOT NULL DEFAULT GETDATE(),
    FechaLimite DATETIME NOT NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente', -- 'Pendiente', 'Devuelto'
    CONSTRAINT FK_Prestamos_Socios FOREIGN KEY (SocioId) REFERENCES Socios(SocioId)
);
GO

CREATE TABLE DetallePrestamo (
    PrestamoId INT NOT NULL,
    LibroId INT NOT NULL,
    FechaDevolucion DATETIME NULL,
    PRIMARY KEY (PrestamoId, LibroId),
    CONSTRAINT FK_DetallePrestamo_Prestamos FOREIGN KEY (PrestamoId) REFERENCES Prestamos(PrestamoId),
    CONSTRAINT FK_DetallePrestamo_Libros FOREIGN KEY (LibroId) REFERENCES Libros(LibroId)
);
GO


INSERT INTO Autores (Nombre, Nacionalidad) VALUES 
('Gabriel García Márquez', 'Colombiana'),
('Mario Vargas Llosa', 'Peruana'),
('Isabel Allende', 'Chilena'),
('Julio Cortázar', 'Argentina'),
('J.R.R. Tolkien', 'Británica'),
('George R.R. Martin', 'Estadounidense'),
('J.K. Rowling', 'Británica'),
('Isaac Asimov', 'Estadounidense');

INSERT INTO Libros (Titulo, ISBN, AutorId, Ejemplares) VALUES 
('Cien años de soledad', '978-0307474728', 1, 5),
('El amor en los tiempos del cólera', '978-0307389732', 1, 3),
('La ciudad y los perros', '978-8420452331', 2, 4),
('La fiesta del chivo', '978-8466331904', 2, 2),
('La casa de los espíritus', '978-0553383805', 3, 6),
('Eva Luna', '978-0553280586', 3, 3),
('Rayuela', '978-8420423645', 4, 4),
('Bestiario', '978-8447330759', 4, 2),
('El hobbit', '978-8445071410', 5, 10),
('La comunidad del anillo', '978-8445071755', 5, 8),
('Las dos torres', '978-8445071762', 5, 7),
('El retorno del rey', '978-8445071779', 5, 8),
('Juego de tronos', '978-8496208919', 6, 12),
('Choque de reyes', '978-8496208964', 6, 9),
('Harry Potter y la piedra filosofal', '978-8478884452', 7, 15),
('Harry Potter y la cámara secreta', '978-8478884957', 7, 12),
('Harry Potter y el prisionero de Azkaban', '978-8478885190', 7, 10),
('Fundación', '978-8498890479', 8, 5),
('Fundación e Imperio', '978-8498890486', 8, 4),
('Segunda Fundación', '978-8498890493', 8, 4);

INSERT INTO Socios (DNI, Nombre, Email) VALUES 
('11111111', 'Juan Perez', 'juan@email.com'),
('22222222', 'Maria Garcia', 'maria@email.com'),
('33333333', 'Carlos Lopez', 'carlos@email.com'),
('44444444', 'Ana Martinez', 'ana@email.com'),
('55555555', 'Luis Rodriguez', 'luis@email.com'),
('66666666', 'Elena Gomez', 'elena@email.com'),
('77777777', 'Jorge Diaz', 'jorge@email.com'),
('88888888', 'Rosa Ruiz', 'rosa@email.com'),
('99999999', 'Pedro Sanchez', 'pedro@email.com'),
('10101010', 'Carmen Torres', 'carmen@email.com');

INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) 
VALUES (1, GETDATE() - 5, GETDATE() + 2, 'Pendiente');
DECLARE @P1 INT = SCOPE_IDENTITY();
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P1, 1);
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P1, 2);
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P1, 3);
UPDATE Libros SET Ejemplares = Ejemplares - 1 WHERE LibroId IN (1, 2, 3);

INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) 
VALUES (2, GETDATE() - 10, GETDATE() - 3, 'Pendiente');
DECLARE @P2 INT = SCOPE_IDENTITY();
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P2, 9);
UPDATE Libros SET Ejemplares = Ejemplares - 1 WHERE LibroId IN (9);

INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) 
VALUES (3, GETDATE() - 2, GETDATE() + 5, 'Pendiente');
DECLARE @P3 INT = SCOPE_IDENTITY();
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P3, 15);
UPDATE Libros SET Ejemplares = Ejemplares - 1 WHERE LibroId IN (15);

INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) 
VALUES (4, GETDATE() - 15, GETDATE() - 8, 'Devuelto');
DECLARE @P4 INT = SCOPE_IDENTITY();
INSERT INTO DetallePrestamo (PrestamoId, LibroId, FechaDevolucion) VALUES (@P4, 5, GETDATE() - 10);
INSERT INTO DetallePrestamo (PrestamoId, LibroId, FechaDevolucion) VALUES (@P4, 6, GETDATE() - 10);

INSERT INTO Prestamos (SocioId, FechaPrestamo, FechaLimite, Estado) 
VALUES (5, GETDATE() - 1, GETDATE() + 6, 'Pendiente');
DECLARE @P5 INT = SCOPE_IDENTITY();
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P5, 18);
INSERT INTO DetallePrestamo (PrestamoId, LibroId) VALUES (@P5, 19);
UPDATE Libros SET Ejemplares = Ejemplares - 1 WHERE LibroId IN (18, 19);

GO
