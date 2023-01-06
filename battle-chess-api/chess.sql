-- phpMyAdmin SQL Dump
-- version 5.1.0
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 03, 2022 at 08:23 AM
-- Server version: 10.4.18-MariaDB
-- PHP Version: 7.4.16

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `chess`
--

-- --------------------------------------------------------

--
-- Table structure for table `chess_challenges`
--

CREATE TABLE `chess_challenges` (
  `id` bigint(20) UNSIGNED NOT NULL,
  `fromUserId` int(10) UNSIGNED NOT NULL,
  `toUserId` int(10) UNSIGNED NOT NULL,
  `status` tinyint(4) NOT NULL DEFAULT 0,
  `roomId` varchar(50) DEFAULT NULL,
  `timestamp` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------

--
-- Table structure for table `chess_save`
--

CREATE TABLE `chess_save` (
  `id` int(20) NOT NULL,
  `name` datetime NOT NULL,
  `fen` varchar(200) NOT NULL,
  `userId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `chess_save`
--

INSERT INTO `chess_save` (`id`, `name`, `fen`, `userId`) VALUES
(9, '2021-03-19 18:17:57', 'rnbqk1nr/pppp1ppp/3bp3/8/5P2/4P3/PPPP2PP/RNBQKBNR w KQkq - 0 1', 9),
(10, '2021-03-19 18:17:59', 'rnbqk1nr/pppp1ppp/3bp3/8/5P2/4P3/PPPP2PP/RNBQKBNR w KQkq - 0 1', 9),
(11, '2021-03-19 18:17:59', 'rnbqk1nr/pppp1ppp/3bp3/8/5P2/4P3/PPPP2PP/RNBQKBNR w KQkq - 0 1', 9),
(12, '2021-03-19 18:18:00', 'rnbqk1nr/pppp1ppp/3bp3/8/5P2/4P3/PPPP2PP/RNBQKBNR w KQkq - 0 1', 9),
(13, '2021-03-19 19:10:52', 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1', 9);

-- --------------------------------------------------------

--
-- Table structure for table `chess_users`
--

CREATE TABLE `chess_users` (
  `id` int(11) NOT NULL,
  `username` varchar(50) CHARACTER SET utf8 NOT NULL,
  `password` varchar(100) CHARACTER SET utf8 NOT NULL,
  `score` int(11) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `chess_users`
--

INSERT INTO `chess_users` (`id`, `username`, `password`, `score`) VALUES
(2, 'test1', 'test1', 0),
(3, 'test2', 'test2', 0),
(4, 'test3', 'test3', 0),
(5, 'test4', 'test4', 0),
(6, 'test5', 'test5', 0),
(7, 'test6', 'test6', 0),
(8, 'test7', 'test7', 0),
(9, 'test', 'test', 0),
(10, 'sdf', 'sdfs', 0),
(14, 'testing', 'testing', 0);

-- --------------------------------------------------------

--
-- Table structure for table `w_users`
--

CREATE TABLE `w_users` (
  `id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  `score` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `w_users`
--

INSERT INTO `w_users` (`id`, `username`, `password`, `score`) VALUES
(8, '0x3d7bfb70de6a7e1228520cd209f1404526b5db65', '', 0),
(9, 'error', '', 0),
(10, 'asda', 'as', 0);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `chess_challenges`
--
ALTER TABLE `chess_challenges`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `chess_save`
--
ALTER TABLE `chess_save`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `chess_users`
--
ALTER TABLE `chess_users`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `w_users`
--
ALTER TABLE `w_users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `chess_challenges`
--
ALTER TABLE `chess_challenges`
  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=33;

--
-- AUTO_INCREMENT for table `chess_save`
--
ALTER TABLE `chess_save`
  MODIFY `id` int(20) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT for table `chess_users`
--
ALTER TABLE `chess_users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT for table `w_users`
--
ALTER TABLE `w_users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
