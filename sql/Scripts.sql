CREATE DATABASE BibliotecaDb

USE BibliotecaDb

CREATE TABLE TipoUsuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(50) NOT NULL
);

CREATE TABLE Usuario (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Dni NVARCHAR(20) NOT NULL UNIQUE,
    Nombres NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100),
    Telefono NVARCHAR(20),
    Estado BIT NOT NULL,
    TipoUsuarioId INT NOT NULL,
    FOREIGN KEY (TipoUsuarioId) REFERENCES TipoUsuario(Id)
);

CREATE TABLE Libro (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ISBN NVARCHAR(20) NOT NULL UNIQUE,
    Nombre NVARCHAR(200) NOT NULL,
    Categoria NVARCHAR(100),
    Autor NVARCHAR(100),
    Pais NVARCHAR(50),
    FechaPublicacion DATE,
    Editorial NVARCHAR(100)
);

CREATE TABLE Prestamo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    FechaPrestamo DATETIME NOT NULL,
    FechaDevolucion DATETIME NULL,
    TipoLectura NVARCHAR(20) NOT NULL,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id)
);

CREATE TABLE DetallePrestamo (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PrestamoId INT NOT NULL,
    LibroId INT NOT NULL,
    FOREIGN KEY (PrestamoId) REFERENCES Prestamo(Id),
    FOREIGN KEY (LibroId) REFERENCES Libro(Id)
);

-- Insertar datos en TipoUsuario

INSERT INTO TipoUsuario (Descripcion) VALUES
('Alumno'),
('Docente');

-- Insertar Usuarios (2 alumnos, 2 docentes)

INSERT INTO Usuario (Dni, Nombres, Apellidos, Email, Telefono, Estado, TipoUsuarioId) VALUES
('12345678', 'Juan', 'Perez', 'juan.perez@mail.com', '999111222', 1, 1),
('87654321', 'Maria', 'Lopez', 'maria.lopez@mail.com', '999333444', 1, 1),
('11223344', 'Carlos', 'Ramirez', 'carlos.ramirez@mail.com', '999555666', 1, 2),
('44332211', 'Ana', 'Gomez', 'ana.gomez@mail.com', '999777888', 1, 2);

-- Insertar Libros (6)

INSERT INTO Libro (ISBN, Nombre, Categoria, Autor, Pais, FechaPublicacion, Editorial) VALUES
('978-3-16-148410-0', 'C# Avanzado', 'Programación', 'Juan Dev', 'Perú', '2020-01-10', 'Editorial A'),
('978-1-40-289462-3', 'Bases de Datos', 'Tecnología', 'Ana Smith', 'Chile', '2018-07-23', 'Editorial B'),
('978-0-12-345678-9', 'Diseño Web', 'Diseño', 'Luis Perez', 'Argentina', '2019-11-05', 'Editorial C'),
('978-9-87-654321-0', 'Redes de Computadoras', 'Redes', 'Laura Ruiz', 'México', '2017-03-15', 'Editorial D'),
('978-4-56-789012-3', 'Algoritmos', 'Programación', 'Pedro Torres', 'Colombia', '2021-05-30', 'Editorial E'),
('978-7-89-012345-6', 'Inteligencia Artificial', 'IA', 'Sofia Diaz', 'España', '2022-08-12', 'Editorial F');