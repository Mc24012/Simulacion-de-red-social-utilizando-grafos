-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 14-06-2026 a las 06:34:50
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `red_db`
--

DELIMITER $$
--
-- Procedimientos
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `EliminarUsuario` (IN `p_Id` INT)   BEGIN
  UPDATE usuarios
  SET  Activo            = 0,
       Fecha_Eliminacion = NOW()
  WHERE Id_Usuario = p_Id;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `intereses`
--

CREATE TABLE `intereses` (
  `Id_Interes` int(11) NOT NULL,
  `Nombre_Interes` varchar(80) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `intereses`
--

INSERT INTO `intereses` (`Id_Interes`, `Nombre_Interes`) VALUES
(9, 'Arte'),
(2, 'Cine'),
(8, 'Cocina'),
(3, 'Deportes'),
(11, 'Fitness'),
(10, 'Fotografia'),
(4, 'Lectura'),
(1, 'Musica'),
(12, 'Programacion'),
(6, 'Tecnologia'),
(7, 'Viajes'),
(5, 'Videojuegos');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `posibles_relaciones`
--

CREATE TABLE `posibles_relaciones` (
  `Id_Origen` int(11) NOT NULL,
  `Id_Destino` int(11) NOT NULL,
  `Interes_Comun` varchar(80) NOT NULL,
  `Puntaje` int(11) NOT NULL DEFAULT 1,
  `Fecha_Calculo` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `posibles_relaciones`
--

INSERT INTO `posibles_relaciones` (`Id_Origen`, `Id_Destino`, `Interes_Comun`, `Puntaje`, `Fecha_Calculo`) VALUES
(1, 13, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 25, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 37, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 49, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 61, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 73, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 85, 'Arte', 1, '2026-06-13 22:24:18'),
(1, 97, 'Arte', 1, '2026-06-13 22:24:18'),
(2, 14, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 26, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 38, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 50, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 62, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 74, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 86, 'Deportes', 1, '2026-06-13 22:24:18'),
(2, 98, 'Deportes', 1, '2026-06-13 22:24:18'),
(3, 15, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 27, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 39, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 51, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 63, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 75, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 87, 'Fitness', 1, '2026-06-13 22:24:18'),
(3, 99, 'Fitness', 1, '2026-06-13 22:24:18'),
(4, 16, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 28, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 40, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 52, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 64, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 76, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 88, 'Cocina', 1, '2026-06-13 22:24:18'),
(4, 100, 'Cocina', 1, '2026-06-13 22:24:18'),
(5, 17, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 29, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 41, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 53, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 65, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 77, 'Arte', 1, '2026-06-13 22:24:18'),
(5, 89, 'Arte', 1, '2026-06-13 22:24:18'),
(6, 18, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 30, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 42, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 54, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 66, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 78, 'Cine', 1, '2026-06-13 22:24:18'),
(6, 90, 'Cine', 1, '2026-06-13 22:24:18'),
(7, 19, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 31, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 43, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 55, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 67, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 79, 'Cocina', 1, '2026-06-13 22:24:18'),
(7, 91, 'Cocina', 1, '2026-06-13 22:24:18'),
(8, 20, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 32, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 44, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 56, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 68, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 80, 'Arte', 1, '2026-06-13 22:24:18'),
(8, 92, 'Arte', 1, '2026-06-13 22:24:18'),
(9, 21, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 33, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 45, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 57, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 69, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 81, 'Fotografia', 1, '2026-06-13 22:24:18'),
(9, 93, 'Fotografia', 1, '2026-06-13 22:24:18'),
(10, 22, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 34, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 46, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 58, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 70, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 82, 'Cine', 1, '2026-06-13 22:24:18'),
(10, 94, 'Cine', 1, '2026-06-13 22:24:18'),
(11, 23, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 35, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 47, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 59, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 71, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 83, 'Deportes', 1, '2026-06-13 22:24:18'),
(11, 95, 'Deportes', 1, '2026-06-13 22:24:18'),
(12, 24, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 36, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 48, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 60, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 72, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 84, 'Cocina', 1, '2026-06-13 22:24:18'),
(12, 96, 'Cocina', 1, '2026-06-13 22:24:18'),
(13, 25, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 37, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 49, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 61, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 73, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 85, 'Arte', 1, '2026-06-13 22:24:18'),
(13, 97, 'Arte', 1, '2026-06-13 22:24:18'),
(14, 26, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 38, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 50, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 62, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 74, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 86, 'Deportes', 1, '2026-06-13 22:24:18'),
(14, 98, 'Deportes', 1, '2026-06-13 22:24:18'),
(15, 27, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 39, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 51, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 63, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 75, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 87, 'Fitness', 1, '2026-06-13 22:24:18'),
(15, 99, 'Fitness', 1, '2026-06-13 22:24:18'),
(16, 28, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 40, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 52, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 64, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 76, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 88, 'Cocina', 1, '2026-06-13 22:24:18'),
(16, 100, 'Cocina', 1, '2026-06-13 22:24:18'),
(17, 29, 'Arte', 1, '2026-06-13 22:24:18'),
(17, 41, 'Arte', 1, '2026-06-13 22:24:18'),
(17, 53, 'Arte', 1, '2026-06-13 22:24:18'),
(17, 65, 'Arte', 1, '2026-06-13 22:24:18'),
(17, 77, 'Arte', 1, '2026-06-13 22:24:18'),
(17, 89, 'Arte', 1, '2026-06-13 22:24:18'),
(18, 30, 'Cine', 1, '2026-06-13 22:24:18'),
(18, 42, 'Cine', 1, '2026-06-13 22:24:18'),
(18, 54, 'Cine', 1, '2026-06-13 22:24:18'),
(18, 66, 'Cine', 1, '2026-06-13 22:24:18'),
(18, 78, 'Cine', 1, '2026-06-13 22:24:18'),
(18, 90, 'Cine', 1, '2026-06-13 22:24:18'),
(19, 31, 'Cocina', 1, '2026-06-13 22:24:18'),
(19, 43, 'Cocina', 1, '2026-06-13 22:24:18'),
(19, 55, 'Cocina', 1, '2026-06-13 22:24:18'),
(19, 67, 'Cocina', 1, '2026-06-13 22:24:18'),
(19, 79, 'Cocina', 1, '2026-06-13 22:24:18'),
(19, 91, 'Cocina', 1, '2026-06-13 22:24:18'),
(20, 32, 'Arte', 1, '2026-06-13 22:24:18'),
(20, 44, 'Arte', 1, '2026-06-13 22:24:18'),
(20, 56, 'Arte', 1, '2026-06-13 22:24:18'),
(20, 68, 'Arte', 1, '2026-06-13 22:24:18'),
(20, 80, 'Arte', 1, '2026-06-13 22:24:18'),
(20, 92, 'Arte', 1, '2026-06-13 22:24:18'),
(21, 33, 'Fotografia', 1, '2026-06-13 22:24:18'),
(21, 45, 'Fotografia', 1, '2026-06-13 22:24:18'),
(21, 57, 'Fotografia', 1, '2026-06-13 22:24:18'),
(21, 69, 'Fotografia', 1, '2026-06-13 22:24:18'),
(21, 81, 'Fotografia', 1, '2026-06-13 22:24:18'),
(21, 93, 'Fotografia', 1, '2026-06-13 22:24:18'),
(22, 34, 'Cine', 1, '2026-06-13 22:24:18'),
(22, 46, 'Cine', 1, '2026-06-13 22:24:18'),
(22, 58, 'Cine', 1, '2026-06-13 22:24:18'),
(22, 70, 'Cine', 1, '2026-06-13 22:24:18'),
(22, 82, 'Cine', 1, '2026-06-13 22:24:18'),
(22, 94, 'Cine', 1, '2026-06-13 22:24:18'),
(23, 35, 'Deportes', 1, '2026-06-13 22:24:18'),
(23, 47, 'Deportes', 1, '2026-06-13 22:24:18'),
(23, 59, 'Deportes', 1, '2026-06-13 22:24:18'),
(23, 71, 'Deportes', 1, '2026-06-13 22:24:18'),
(23, 83, 'Deportes', 1, '2026-06-13 22:24:18'),
(23, 95, 'Deportes', 1, '2026-06-13 22:24:18'),
(24, 36, 'Cocina', 1, '2026-06-13 22:24:18'),
(24, 48, 'Cocina', 1, '2026-06-13 22:24:18'),
(24, 60, 'Cocina', 1, '2026-06-13 22:24:18'),
(24, 72, 'Cocina', 1, '2026-06-13 22:24:18'),
(24, 84, 'Cocina', 1, '2026-06-13 22:24:18'),
(24, 96, 'Cocina', 1, '2026-06-13 22:24:18');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `relaciones`
--

CREATE TABLE `relaciones` (
  `Id_Seguidor` int(11) NOT NULL,
  `Id_Seguido` int(11) NOT NULL,
  `FechaInicio` datetime NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `relaciones`
--

INSERT INTO `relaciones` (`Id_Seguidor`, `Id_Seguido`, `FechaInicio`) VALUES
(1, 2, '2024-01-05 10:00:00'),
(1, 3, '2024-01-06 11:00:00'),
(1, 10, '2024-01-07 12:00:00'),
(1, 50, '2024-03-01 10:00:00'),
(2, 1, '2024-01-08 09:30:00'),
(2, 5, '2024-01-09 14:00:00'),
(2, 50, '2024-03-02 10:00:00'),
(3, 1, '2024-01-10 08:00:00'),
(3, 7, '2024-01-11 16:00:00'),
(3, 50, '2024-03-03 10:00:00'),
(4, 2, '2024-01-12 10:30:00'),
(4, 6, '2024-01-13 11:45:00'),
(5, 1, '2024-01-14 09:00:00'),
(5, 4, '2024-01-15 15:00:00'),
(6, 3, '2024-01-16 13:00:00'),
(6, 8, '2024-01-17 17:00:00'),
(7, 2, '2024-01-18 10:00:00'),
(7, 9, '2024-01-19 11:00:00'),
(8, 5, '2024-01-20 14:30:00'),
(8, 10, '2024-01-21 16:00:00'),
(9, 1, '2024-01-22 09:00:00'),
(9, 6, '2024-01-23 10:00:00'),
(10, 3, '2024-01-24 12:00:00'),
(10, 7, '2024-01-25 14:00:00'),
(11, 1, '2024-02-01 08:00:00'),
(11, 12, '2024-02-02 09:00:00'),
(12, 11, '2024-02-03 10:00:00'),
(12, 13, '2024-02-04 11:00:00'),
(13, 14, '2024-02-05 12:00:00'),
(14, 13, '2024-02-06 13:00:00'),
(15, 16, '2024-02-07 14:00:00'),
(16, 15, '2024-02-08 15:00:00'),
(17, 18, '2024-02-09 16:00:00'),
(18, 17, '2024-02-10 17:00:00'),
(19, 20, '2024-02-11 08:30:00'),
(20, 19, '2024-02-12 09:30:00'),
(21, 22, '2024-02-13 10:30:00'),
(22, 21, '2024-02-14 11:30:00'),
(23, 24, '2024-02-15 12:30:00'),
(24, 23, '2024-02-16 13:30:00'),
(25, 26, '2024-02-17 14:30:00'),
(26, 25, '2024-02-18 15:30:00'),
(27, 28, '2024-02-19 16:30:00'),
(28, 27, '2024-02-20 17:30:00'),
(50, 1, '2024-03-04 10:00:00'),
(50, 51, '2024-03-05 10:00:00'),
(51, 50, '2024-03-06 10:00:00'),
(51, 52, '2024-03-07 10:00:00'),
(52, 51, '2024-03-08 10:00:00'),
(52, 53, '2024-03-09 10:00:00');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuarios`
--

CREATE TABLE `usuarios` (
  `Id_Usuario` int(11) NOT NULL,
  `Nombre_Usuario` varchar(100) NOT NULL,
  `Genero_Usuario` varchar(20) NOT NULL,
  `Email_Usuario` varchar(150) NOT NULL,
  `Activo` tinyint(1) NOT NULL DEFAULT 1,
  `Fecha_Eliminacion` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `usuarios`
--

INSERT INTO `usuarios` (`Id_Usuario`, `Nombre_Usuario`, `Genero_Usuario`, `Email_Usuario`, `Activo`, `Fecha_Eliminacion`) VALUES
(1, 'Carlos Mendoza', 'Masculino', 'carlos.mendoza@gmail.com', 1, NULL),
(2, 'María López', 'Femenino', 'maria.lopez@gmail.com', 1, NULL),
(3, 'José García', 'Masculino', 'jose.garcia@gmail.com', 1, NULL),
(4, 'Ana Martínez', 'Femenino', 'ana.martinez@gmail.com', 1, NULL),
(5, 'Luis Hernández', 'Masculino', 'luis.hernandez@gmail.com', 1, NULL),
(6, 'Laura Díaz', 'Femenino', 'laura.diaz@gmail.com', 1, NULL),
(7, 'Pedro Rodríguez', 'Masculino', 'pedro.rodriguez@gmail.com', 1, NULL),
(8, 'Sofía Torres', 'Femenino', 'sofia.torres@gmail.com', 1, NULL),
(9, 'Miguel Flores', 'Masculino', 'miguel.flores@gmail.com', 1, NULL),
(10, 'Valentina Cruz', 'Femenino', 'valentina.cruz@gmail.com', 1, NULL),
(11, 'Andrés Ramírez', 'Masculino', 'andres.ramirez@gmail.com', 1, NULL),
(12, 'Camila Morales', 'Femenino', 'camila.morales@gmail.com', 1, NULL),
(13, 'Fernando Herrera', 'Masculino', 'fernando.herrera@gmail.com', 1, NULL),
(14, 'Isabella Jiménez', 'Femenino', 'isabella.jimenez@gmail.com', 1, NULL),
(15, 'Ricardo Vargas', 'Masculino', 'ricardo.vargas@gmail.com', 1, NULL),
(16, 'Daniela Castillo', 'Femenino', 'daniela.castillo@gmail.com', 1, NULL),
(17, 'Jorge Romero', 'Masculino', 'jorge.romero@gmail.com', 1, NULL),
(18, 'Natalia Gutiérrez', 'Femenino', 'natalia.gutierrez@gmail.com', 1, NULL),
(19, 'Alberto Núñez', 'Masculino', 'alberto.nunez@gmail.com', 1, NULL),
(20, 'Paola Medina', 'Femenino', 'paola.medina@gmail.com', 1, NULL),
(21, 'Sebastián Ruiz', 'Masculino', 'sebastian.ruiz@gmail.com', 1, NULL),
(22, 'Mariana Suárez', 'Femenino', 'mariana.suarez@gmail.com', 1, NULL),
(23, 'Eduardo Peña', 'Masculino', 'eduardo.pena@gmail.com', 1, NULL),
(24, 'Lucía Aguilar', 'Femenino', 'lucia.aguilar@gmail.com', 1, NULL),
(25, 'Héctor Vega', 'Masculino', 'hector.vega@gmail.com', 1, NULL),
(26, 'Gabriela Rojas', 'Femenino', 'gabriela.rojas@gmail.com', 1, NULL),
(27, 'Manuel Ortega', 'Masculino', 'manuel.ortega@gmail.com', 1, NULL),
(28, 'Fernanda Soto', 'Femenino', 'fernanda.soto@gmail.com', 1, NULL),
(29, 'Óscar Ríos', 'Masculino', 'oscar.rios@gmail.com', 1, NULL),
(30, 'Alejandra Ramos', 'Femenino', 'alejandra.ramos@gmail.com', 1, NULL),
(31, 'Ernesto Delgado', 'Masculino', 'ernesto.delgado@gmail.com', 1, NULL),
(32, 'Verónica Herrera', 'Femenino', 'veronica.herrera@gmail.com', 1, NULL),
(33, 'Raúl Mendez', 'Masculino', 'raul.mendez@gmail.com', 1, NULL),
(34, 'Mónica Castro', 'Femenino', 'monica.castro@gmail.com', 1, NULL),
(35, 'Arturo Navarro', 'Masculino', 'arturo.navarro@gmail.com', 1, NULL),
(36, 'Karla Ibarra', 'Femenino', 'karla.ibarra@gmail.com', 1, NULL),
(37, 'Roberto Paredes', 'Masculino', 'roberto.paredes@gmail.com', 1, NULL),
(38, 'Claudia Silva', 'Femenino', 'claudia.silva@gmail.com', 1, NULL),
(39, 'Gustavo Lara', 'Masculino', 'gustavo.lara@gmail.com', 1, NULL),
(40, 'Patricia Mora', 'Femenino', 'patricia.mora@gmail.com', 1, NULL),
(41, 'Francisco Campos', 'Masculino', 'francisco.campos@gmail.com', 1, NULL),
(42, 'Ximena Contreras', 'Femenino', 'ximena.contreras@gmail.com', 1, NULL),
(43, 'David Espinoza', 'Masculino', 'david.espinoza@gmail.com', 1, NULL),
(44, 'Rebeca Fuentes', 'Femenino', 'rebeca.fuentes@gmail.com', 1, NULL),
(45, 'Alejandro Bravo', 'Masculino', 'alejandro.bravo@gmail.com', 1, NULL),
(46, 'Silvia Cabrera', 'Femenino', 'silvia.cabrera@gmail.com', 1, NULL),
(47, 'Armando Sandoval', 'Masculino', 'armando.sandoval@gmail.com', 1, NULL),
(48, 'Estefanía Molina', 'Femenino', 'estefania.molina@gmail.com', 1, NULL),
(49, 'Jaime Ávila', 'Masculino', 'jaime.avila@gmail.com', 1, NULL),
(50, 'Lorena Guerrero', 'Femenino', 'lorena.guerrero@gmail.com', 1, NULL),
(51, 'Enrique Salazar', 'Masculino', 'enrique.salazar@gmail.com', 1, NULL),
(52, 'Vanessa Miranda', 'Femenino', 'vanessa.miranda@gmail.com', 1, NULL),
(53, 'Marco Carrillo', 'Masculino', 'marco.carrillo@gmail.com', 1, NULL),
(54, 'Diana Padilla', 'Femenino', 'diana.padilla@gmail.com', 1, NULL),
(55, 'Víctor Acosta', 'Masculino', 'victor.acosta@gmail.com', 1, NULL),
(56, 'Alicia Mendoza', 'Femenino', 'alicia.mendoza2@gmail.com', 1, NULL),
(57, 'Rodrigo Serrano', 'Masculino', 'rodrigo.serrano@gmail.com', 1, NULL),
(58, 'Gloria Vázquez', 'Femenino', 'gloria.vazquez@gmail.com', 1, NULL),
(59, 'Ignacio Montes', 'Masculino', 'ignacio.montes@gmail.com', 1, NULL),
(60, 'Adriana Pedraza', 'Femenino', 'adriana.pedraza@gmail.com', 1, NULL),
(61, 'César Quintero', 'Masculino', 'cesar.quintero@gmail.com', 1, NULL),
(62, 'Esperanza Luna', 'Femenino', 'esperanza.luna@gmail.com', 1, NULL),
(63, 'Julio Domínguez', 'Masculino', 'julio.dominguez@gmail.com', 1, NULL),
(64, 'Marcela Tapia', 'Femenino', 'marcela.tapia@gmail.com', 1, NULL),
(65, 'Nelson Palacios', 'Masculino', 'nelson.palacios@gmail.com', 1, NULL),
(66, 'Yesenia Trejo', 'Femenino', 'yesenia.trejo@gmail.com', 1, NULL),
(67, 'Hugo Villanueva', 'Masculino', 'hugo.villanueva@gmail.com', 1, NULL),
(68, 'Brenda Zárate', 'Femenino', 'brenda.zarate@gmail.com', 1, NULL),
(69, 'Mauricio Ángeles', 'Masculino', 'mauricio.angeles@gmail.com', 1, NULL),
(70, 'Araceli Bernal', 'Femenino', 'araceli.bernal@gmail.com', 1, NULL),
(71, 'Rafael Ocampo', 'Masculino', 'rafael.ocampo@gmail.com', 1, NULL),
(72, 'Esmeralda Cano', 'Femenino', 'esmeralda.cano@gmail.com', 1, NULL),
(73, 'Salvador Pacheco', 'Masculino', 'salvador.pacheco@gmail.com', 1, NULL),
(74, 'Fabiola Mercado', 'Femenino', 'fabiola.mercado@gmail.com', 1, NULL),
(75, 'Iván Rangel', 'Masculino', 'ivan.rangel@gmail.com', 1, NULL),
(76, 'Samantha Osorio', 'Femenino', 'samantha.osorio@gmail.com', 1, NULL),
(77, 'Omar Velázquez', 'Masculino', 'omar.velazquez@gmail.com', 1, NULL),
(78, 'Tania Gallardo', 'Femenino', 'tania.gallardo@gmail.com', 1, NULL),
(79, 'Gilberto Solís', 'Masculino', 'gilberto.solis@gmail.com', 1, NULL),
(80, 'Norma Ibáñez', 'Femenino', 'norma.ibanez@gmail.com', 1, NULL),
(81, 'Eliseo Coronel', 'Masculino', 'eliseo.coronel@gmail.com', 1, NULL),
(82, 'Blanca Valdez', 'Femenino', 'blanca.valdez@gmail.com', 1, NULL),
(83, 'Rubén Figueroa', 'Masculino', 'ruben.figueroa@gmail.com', 1, NULL),
(84, 'Yolanda Cisneros', 'Femenino', 'yolanda.cisneros@gmail.com', 1, NULL),
(85, 'Oswaldo Pedroza', 'Masculino', 'oswaldo.pedroza@gmail.com', 1, NULL),
(86, 'Miriam Castellanos', 'Femenino', 'miriam.castellanos@gmail.com', 1, NULL),
(87, 'Tomás Escobedo', 'Masculino', 'tomas.escobedo@gmail.com', 1, NULL),
(88, 'Hilda Andrade', 'Femenino', 'hilda.andrade@gmail.com', 1, NULL),
(89, 'Abel Barrera', 'Masculino', 'abel.barrera@gmail.com', 1, NULL),
(90, 'Marisol Esquivel', 'Femenino', 'marisol.esquivel@gmail.com', 1, NULL),
(91, 'Dante Meza', 'Masculino', 'dante.meza@gmail.com', 1, NULL),
(92, 'Rocío Alvarado', 'Femenino', 'rocio.alvarado@gmail.com', 1, NULL),
(93, 'Ramón Espejo', 'Masculino', 'ramon.espejo@gmail.com', 1, NULL),
(94, 'Irene Solano', 'Femenino', 'irene.solano@gmail.com', 1, NULL),
(95, 'Benjamín Quiroz', 'Masculino', 'benjamin.quiroz@gmail.com', 1, NULL),
(96, 'Dolores Palma', 'Femenino', 'dolores.palma@gmail.com', 1, NULL),
(97, 'Lorenzo Ponce', 'Masculino', 'lorenzo.ponce@gmail.com', 1, NULL),
(98, 'Cristina Anaya', 'Femenino', 'cristina.anaya@gmail.com', 1, NULL),
(99, 'Sergio Villalba', 'Masculino', 'sergio.villalba@gmail.com', 1, NULL),
(100, 'Nadia Cervantes', 'Femenino', 'nadia.cervantes@gmail.com', 1, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario_intereses`
--

CREATE TABLE `usuario_intereses` (
  `Id_Usuario` int(11) NOT NULL,
  `Id_Interes` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `usuario_intereses`
--

INSERT INTO `usuario_intereses` (`Id_Usuario`, `Id_Interes`) VALUES
(1, 2),
(1, 5),
(1, 9),
(2, 3),
(2, 6),
(2, 10),
(3, 4),
(3, 7),
(3, 11),
(4, 5),
(4, 8),
(4, 12),
(5, 1),
(5, 6),
(5, 9),
(6, 2),
(6, 7),
(6, 10),
(7, 3),
(7, 8),
(7, 11),
(8, 4),
(8, 9),
(8, 12),
(9, 1),
(9, 5),
(9, 10),
(10, 2),
(10, 6),
(10, 11),
(11, 3),
(11, 7),
(11, 12),
(12, 1),
(12, 4),
(12, 8),
(13, 2),
(13, 5),
(13, 9),
(14, 3),
(14, 6),
(14, 10),
(15, 4),
(15, 7),
(15, 11),
(16, 5),
(16, 8),
(16, 12),
(17, 1),
(17, 6),
(17, 9),
(18, 2),
(18, 7),
(18, 10),
(19, 3),
(19, 8),
(19, 11),
(20, 4),
(20, 9),
(20, 12),
(21, 1),
(21, 5),
(21, 10),
(22, 2),
(22, 6),
(22, 11),
(23, 3),
(23, 7),
(23, 12),
(24, 1),
(24, 4),
(24, 8),
(25, 2),
(25, 5),
(25, 9),
(26, 3),
(26, 6),
(26, 10),
(27, 4),
(27, 7),
(27, 11),
(28, 5),
(28, 8),
(28, 12),
(29, 1),
(29, 6),
(29, 9),
(30, 2),
(30, 7),
(30, 10),
(31, 3),
(31, 8),
(31, 11),
(32, 4),
(32, 9),
(32, 12),
(33, 1),
(33, 5),
(33, 10),
(34, 2),
(34, 6),
(34, 11),
(35, 3),
(35, 7),
(35, 12),
(36, 1),
(36, 4),
(36, 8),
(37, 2),
(37, 5),
(37, 9),
(38, 3),
(38, 6),
(38, 10),
(39, 4),
(39, 7),
(39, 11),
(40, 5),
(40, 8),
(40, 12),
(41, 1),
(41, 6),
(41, 9),
(42, 2),
(42, 7),
(42, 10),
(43, 3),
(43, 8),
(43, 11),
(44, 4),
(44, 9),
(44, 12),
(45, 1),
(45, 5),
(45, 10),
(46, 2),
(46, 6),
(46, 11),
(47, 3),
(47, 7),
(47, 12),
(48, 1),
(48, 4),
(48, 8),
(49, 2),
(49, 5),
(49, 9),
(50, 3),
(50, 6),
(50, 10),
(51, 4),
(51, 7),
(51, 11),
(52, 5),
(52, 8),
(52, 12),
(53, 1),
(53, 6),
(53, 9),
(54, 2),
(54, 7),
(54, 10),
(55, 3),
(55, 8),
(55, 11),
(56, 4),
(56, 9),
(56, 12),
(57, 1),
(57, 5),
(57, 10),
(58, 2),
(58, 6),
(58, 11),
(59, 3),
(59, 7),
(59, 12),
(60, 1),
(60, 4),
(60, 8),
(61, 2),
(61, 5),
(61, 9),
(62, 3),
(62, 6),
(62, 10),
(63, 4),
(63, 7),
(63, 11),
(64, 5),
(64, 8),
(64, 12),
(65, 1),
(65, 6),
(65, 9),
(66, 2),
(66, 7),
(66, 10),
(67, 3),
(67, 8),
(67, 11),
(68, 4),
(68, 9),
(68, 12),
(69, 1),
(69, 5),
(69, 10),
(70, 2),
(70, 6),
(70, 11),
(71, 3),
(71, 7),
(71, 12),
(72, 1),
(72, 4),
(72, 8),
(73, 2),
(73, 5),
(73, 9),
(74, 3),
(74, 6),
(74, 10),
(75, 4),
(75, 7),
(75, 11),
(76, 5),
(76, 8),
(76, 12),
(77, 1),
(77, 6),
(77, 9),
(78, 2),
(78, 7),
(78, 10),
(79, 3),
(79, 8),
(79, 11),
(80, 4),
(80, 9),
(80, 12),
(81, 1),
(81, 5),
(81, 10),
(82, 2),
(82, 6),
(82, 11),
(83, 3),
(83, 7),
(83, 12),
(84, 1),
(84, 4),
(84, 8),
(85, 2),
(85, 5),
(85, 9),
(86, 3),
(86, 6),
(86, 10),
(87, 4),
(87, 7),
(87, 11),
(88, 5),
(88, 8),
(88, 12),
(89, 1),
(89, 6),
(89, 9),
(90, 2),
(90, 7),
(90, 10),
(91, 3),
(91, 8),
(91, 11),
(92, 4),
(92, 9),
(92, 12),
(93, 1),
(93, 5),
(93, 10),
(94, 2),
(94, 6),
(94, 11),
(95, 3),
(95, 7),
(95, 12),
(96, 1),
(96, 4),
(96, 8),
(97, 2),
(97, 5),
(97, 9),
(98, 3),
(98, 6),
(98, 10),
(99, 4),
(99, 7),
(99, 11),
(100, 5),
(100, 8),
(100, 12);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_usuarios_activos`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_usuarios_activos` (
`Id_Usuario` int(11)
,`Nombre_Usuario` varchar(100)
,`Genero_Usuario` varchar(20)
,`Email_Usuario` varchar(150)
);

-- --------------------------------------------------------

--
-- Estructura Stand-in para la vista `v_usuarios_todos`
-- (Véase abajo para la vista actual)
--
CREATE TABLE `v_usuarios_todos` (
`Id_Usuario` int(11)
,`Nombre_Usuario` varchar(100)
,`Genero_Usuario` varchar(20)
,`Email_Usuario` varchar(150)
,`Activo` tinyint(1)
,`Fecha_Eliminacion` datetime
);

-- --------------------------------------------------------

--
-- Estructura para la vista `v_usuarios_activos`
--
DROP TABLE IF EXISTS `v_usuarios_activos`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_usuarios_activos`  AS SELECT `usuarios`.`Id_Usuario` AS `Id_Usuario`, `usuarios`.`Nombre_Usuario` AS `Nombre_Usuario`, `usuarios`.`Genero_Usuario` AS `Genero_Usuario`, `usuarios`.`Email_Usuario` AS `Email_Usuario` FROM `usuarios` WHERE `usuarios`.`Activo` = 1 ;

-- --------------------------------------------------------

--
-- Estructura para la vista `v_usuarios_todos`
--
DROP TABLE IF EXISTS `v_usuarios_todos`;

CREATE ALGORITHM=UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `v_usuarios_todos`  AS SELECT `usuarios`.`Id_Usuario` AS `Id_Usuario`, `usuarios`.`Nombre_Usuario` AS `Nombre_Usuario`, `usuarios`.`Genero_Usuario` AS `Genero_Usuario`, `usuarios`.`Email_Usuario` AS `Email_Usuario`, `usuarios`.`Activo` AS `Activo`, `usuarios`.`Fecha_Eliminacion` AS `Fecha_Eliminacion` FROM `usuarios` ;

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `intereses`
--
ALTER TABLE `intereses`
  ADD PRIMARY KEY (`Id_Interes`),
  ADD UNIQUE KEY `uq_nombre_interes` (`Nombre_Interes`);

--
-- Indices de la tabla `posibles_relaciones`
--
ALTER TABLE `posibles_relaciones`
  ADD PRIMARY KEY (`Id_Origen`,`Id_Destino`,`Interes_Comun`),
  ADD KEY `fk_posible_destino` (`Id_Destino`);

--
-- Indices de la tabla `relaciones`
--
ALTER TABLE `relaciones`
  ADD PRIMARY KEY (`Id_Seguidor`,`Id_Seguido`),
  ADD UNIQUE KEY `uk_relacion` (`Id_Seguidor`,`Id_Seguido`),
  ADD KEY `fk_seguido` (`Id_Seguido`);

--
-- Indices de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  ADD PRIMARY KEY (`Id_Usuario`),
  ADD UNIQUE KEY `Email_Usuario` (`Email_Usuario`);

--
-- Indices de la tabla `usuario_intereses`
--
ALTER TABLE `usuario_intereses`
  ADD PRIMARY KEY (`Id_Usuario`,`Id_Interes`),
  ADD KEY `fk_usuario_interes_interes` (`Id_Interes`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `intereses`
--
ALTER TABLE `intereses`
  MODIFY `Id_Interes` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=37;

--
-- AUTO_INCREMENT de la tabla `usuarios`
--
ALTER TABLE `usuarios`
  MODIFY `Id_Usuario` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=102;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `posibles_relaciones`
--
ALTER TABLE `posibles_relaciones`
  ADD CONSTRAINT `fk_posible_destino` FOREIGN KEY (`Id_Destino`) REFERENCES `usuarios` (`Id_Usuario`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_posible_origen` FOREIGN KEY (`Id_Origen`) REFERENCES `usuarios` (`Id_Usuario`) ON DELETE CASCADE;

--
-- Filtros para la tabla `relaciones`
--
ALTER TABLE `relaciones`
  ADD CONSTRAINT `fk_seguido` FOREIGN KEY (`Id_Seguido`) REFERENCES `usuarios` (`Id_Usuario`),
  ADD CONSTRAINT `fk_seguidor` FOREIGN KEY (`Id_Seguidor`) REFERENCES `usuarios` (`Id_Usuario`);

--
-- Filtros para la tabla `usuario_intereses`
--
ALTER TABLE `usuario_intereses`
  ADD CONSTRAINT `fk_usuario_interes_interes` FOREIGN KEY (`Id_Interes`) REFERENCES `intereses` (`Id_Interes`) ON DELETE CASCADE,
  ADD CONSTRAINT `fk_usuario_interes_usuario` FOREIGN KEY (`Id_Usuario`) REFERENCES `usuarios` (`Id_Usuario`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
