-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2025. Már 17. 21:16
-- Kiszolgáló verziója: 10.4.27-MariaDB
-- PHP verzió: 8.2.0

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `teszt_adatok`
--

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `teszt_osszegzes`
--

CREATE TABLE `teszt_osszegzes` (
  `id` int(11) NOT NULL,
  `tanar` varchar(50) NOT NULL,
  `osztaly` varchar(50) NOT NULL,
  `megerthetoseg` varchar(50) NOT NULL,
  `erthetoseg` varchar(50) NOT NULL,
  `tablaiMunka` int(11) NOT NULL,
  `vetitettDiasor` int(11) NOT NULL,
  `oktatofilmek` int(11) NOT NULL,
  `egyeniGyak` int(11) NOT NULL,
  `haziFeladatok` int(11) NOT NULL,
  `ido` varchar(50) NOT NULL,
  `gyakorisag` varchar(50) NOT NULL,
  `erzes` varchar(50) NOT NULL,
  `elmeny` varchar(50) NOT NULL,
  `megjegyzes` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `teszt_osszegzes`
--
ALTER TABLE `teszt_osszegzes`
  ADD PRIMARY KEY (`id`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `teszt_osszegzes`
--
ALTER TABLE `teszt_osszegzes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
