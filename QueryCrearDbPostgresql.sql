-- Archivo: motoState_schema.sql
-- Esquema para PostgreSQL (motoState)
-- Nota: uso 'contrasena' en lugar de 'contraseña' para evitar problemas con identificadores.

-- (1) Crear la base de datos se hace desde psql o pgAdmin (lo indicamos en los pasos).
-- (2) Este archivo contiene las CREATE TABLE (ejecutar conectado a la BD motoState).

CREATE TABLE Usuario (
    id_usuario SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL,
    contrasena VARCHAR(100) NOT NULL
);

CREATE TABLE Estado (
    id_estado SERIAL PRIMARY KEY,
    nombre_estado VARCHAR(100) NOT NULL
);

CREATE TABLE Moto (
    id_moto SERIAL PRIMARY KEY,
    marca VARCHAR(100) NOT NULL,
    modelo VARCHAR(100) NOT NULL,
    cilindrada VARCHAR(50),
    fecha_ingreso DATE NOT NULL,
    foto_url VARCHAR(255),
    foto_subida SMALLINT NOT NULL DEFAULT 0,
    id_usuario INT NOT NULL,
    CONSTRAINT fk_moto_usuario FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario)
);

CREATE TABLE Seguimiento (
    id_seguimiento SERIAL PRIMARY KEY,
    id_moto INT NOT NULL,
    id_estado INT NOT NULL,
    fecha_actualizacion TIMESTAMP NOT NULL,
    diagnostico TEXT,
    CONSTRAINT fk_seguimiento_moto FOREIGN KEY (id_moto) REFERENCES Moto(id_moto),
    CONSTRAINT fk_seguimiento_estado FOREIGN KEY (id_estado) REFERENCES Estado(id_estado)
);

CREATE TABLE AccesoCliente (
    id_acceso SERIAL PRIMARY KEY,
    id_moto INT NOT NULL,
    url_unica VARCHAR(255) NOT NULL,
    CONSTRAINT fk_acceso_moto FOREIGN KEY (id_moto) REFERENCES Moto(id_moto)
);
