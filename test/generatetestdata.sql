-- Generate test data for MySQL database

-- Inserting data into 'posti' table
INSERT INTO `vn`.`posti` (`postinro`, `toimipaikka`) VALUES
('00100', 'Helsinki'),
('00200', 'Helsinki'),
('00300', 'Espoo'),
('00400', 'Vantaa'),
('00500', 'Tampere'),
('00600', 'Turku'),
('00700', 'Oulu'),
('00800', 'Lahti'),
('00900', 'Kuopio'),
('01000', 'Jyväskylä'),
('01100', 'Hämeenlinna'),
('01200', 'Kajaani'),
('01300', 'Kokkola'),
('01400', 'Pori'),
('01500', 'Joensuu');

-- Inserting data into 'asiakas' table
INSERT INTO `vn`.`asiakas` (`postinro`, `etunimi`, `sukunimi`, `lahiosoite`, `email`, `puhelinnro`) VALUES
('00100', 'Matti', 'Meikäläinen', 'Esimerkkikatu 1', 'matti@example.com', '0123456789'),
('00200', 'Maija', 'Mallikas', 'Esimerkkikatu 2', 'maija@example.com', '9876543210'),
('00300', 'Teemu', 'Testaaja', 'Testikatu 3', 'teemu@example.com', '1234567890'),
('00400', 'Laura', 'Lahtinen', 'Lahtikuja 4', 'laura@example.com', '0987654321'),
('00500', 'Jari', 'Järvinen', 'Järvenpolku 5', 'jari@example.com', '2345678901'),
('00600', 'Anna', 'Ahola', 'Aholantie 6', 'anna@example.com', '3456789012'),
('00700', 'Pekka', 'Peltonen', 'Peltoniementie 7', 'pekka@example.com', '4567890123'),
('00800', 'Sari', 'Salmi', 'Salmentie 8', 'sari@example.com', '5678901234'),
('00900', 'Eero', 'Eskola', 'Eskolantie 9', 'eero@example.com', '6789012345'),
('01000', 'Tiina', 'Toivonen', 'Toivontie 10', 'tiina@example.com', '7890123456'),
('01100', 'Mika', 'Mikkonen', 'Mikantie 11', 'mika@example.com', '8901234567'),
('01200', 'Kaisa', 'Kallio', 'Kalliopolku 12', 'kaisa@example.com', '9012345678'),
('01300', 'Antti', 'Anttila', 'Anttilantie 13', 'antti@example.com', '0123456789'),
('01400', 'Hanna', 'Hanninen', 'Hannikatu 14', 'hanna@example.com', '1234567890'),
('01500', 'Olli', 'Ollikainen', 'Ollikuja 15', 'olli@example.com', '2345678901');

-- Inserting data into 'alue' table
INSERT INTO `vn`.`alue` (`nimi`) VALUES
('Ruka'),
('Tahko'),
('Ylläs'),
('Levi'),
('Saariselkä'),
('Himos'),
('Syöte'),
('Pyhä'),
('Iso-Syöte'),
('Vierumäki');

-- Inserting data into 'mokki' table
INSERT INTO `vn`.`mokki` (`alue_id`, `postinro`, `mokkinimi`, `katuosoite`, `hinta`, `kuvaus`, `henkilomaara`, `varustelu`) VALUES
(1, '00100', 'Mökkilä', 'Mökkikuja 1', 150.00, 'Kaunis mökki lähellä järveä', 4, 'Sauna, grilli, vene'),
(2, '00200', 'Aurinkoranta', 'Aurinkokuja 2', 200.00, 'Aivan rannan tuntumassa', 6, 'Sauna, uimaranta, kalastusvälineet'),
(3, '00300', 'Kotiranta', 'Kotitie 3', 120.00, 'Kodikas mökki metsän laidalla', 5, 'Puusauna, takka, luontopolku'),
(4, '00400', 'Levin Lumo', 'Levitie 4', 250.00, 'Lumoava mökki Levin keskustassa', 8, 'Hiihtimet'),
(5, '00100', 'Mökkilä', 'Mökkikuja 1', 150.00, 'Kaunis mökki lähellä järveä', 4, 'Sauna, grilli, vene'),
(6, '00200', 'Aurinkoranta', 'Aurinkokuja 2', 200.00, 'Aivan rannan tuntumassa', 6, 'Sauna, uimaranta, kalastusvälineet'),
(2, '00300', 'Kotiranta', 'Kotitie 3', 120.00, 'Kodikas mökki metsän laidalla', 5, 'Puusauna, takka, luontopolku'),
(2, '00400', 'Levin Lumo', 'Levitie 4', 250.00, 'Lumoava mökki Levin keskustassa', 8, 'Hiihtimet'),
(1, '00200', 'Aurinkoranta', 'Aurinkokuja 2', 200.00, 'Aivan rannan tuntumassa', 6, 'Sauna, uimaranta, kalastusvälineet'),
(1, '00300', 'Kotiranta', 'Kotitie 3', 120.00, 'Kodikas mökki metsän laidalla', 5, 'Puusauna, takka, luontopolku'),
(8, '00400', 'Levin Lumo', 'Levitie 4', 250.00, 'Lumoava mökki Levin keskustassa', 8, 'Hiihtimet');

-- Inserting data into 'varaus' table without null fields
INSERT INTO `vn`.`varaus` (`asiakas_id`, `mokki_mokki_id`, `varattu_pvm`, `vahvistus_pvm`, `varattu_alkupvm`, `varattu_loppupvm`) VALUES
(1, 1, '2024-01-01', '2024-01-02', '2024-02-01', '2024-02-05'),
(2, 2, '2024-02-01', '2024-02-02', '2024-03-01', '2024-03-10'),
(3, 3, '2024-03-01', '2024-03-02', '2024-04-01', '2024-04-07'),
(4, 4, '2024-04-01', '2024-04-02', '2024-05-01', '2024-05-07'),
(5, 5, '2024-05-01', '2024-05-02', '2024-06-01', '2024-06-14'),
(6, 6, '2024-06-01', '2024-06-02', '2024-07-01', '2024-07-10'),
(7, 7, '2024-07-01', '2024-07-02', '2024-08-01', '2024-08-15'),
(8, 8, '2024-08-01', '2024-08-02', '2024-09-01', '2024-09-05'),
(9, 9, '2024-09-01', '2024-09-02', '2024-10-01', '2024-10-14'),
(10, 10, '2024-10-01', '2024-10-02', '2024-11-01', '2024-11-10');

-- Insert data into 'lasku' table
INSERT INTO `vn`.`lasku` (`lasku_id`, `varaus_id`, `summa`, `alv`, `maksettu`) VALUES
(11, 1, 200.00, 24.00, 1),
(12, 2, 300.00, 36.00, 0),
(13, 3, 150.00, 18.00, 1),
(14, 4, 250.00, 30.00, 0),
(15, 5, 180.00, 21.60, 1),
(16, 6, 350.00, 42.00, 0),
(17, 7, 120.00, 14.40, 1),
(18, 8, 280.00, 33.60, 0),
(19, 9, 200.00, 24.00, 1),
(20, 10, 400.00, 48.00, 0);


-- Inserting more data into 'palvelu' table with unique primary key values
INSERT INTO `vn`.`palvelu` (`palvelu_id`, `alue_id`, `nimi`, `tyyppi`, `kuvaus`, `hinta`, `alv`) VALUES
(11, 1, 'Porosafari', 1, 'Jännittävä porosafari lumisessa metsässä', 50.00, 24.00),
(12, 1, 'Lumikenkäily', 2, 'Rentouttava lumikenkäily talvisessa maisemassa', 30.00, 24.00),
(13, 2, 'Lasketteluopetus', 1, 'Aloittelijoille suunnattu lasketteluopetus', 80.00, 24.00),
(14, 2, 'Murtomaahiihto', 2, 'Perinteinen talviurheilu hiihtoladuilla', 40.00, 24.00),
(15, 3, 'Revontuliretki', 1, 'Unohtumaton retki revontulien ihasteluun', 120.00, 24.00),
(16, 3, 'Kelkkasafari', 2, 'Jännittävä kelkkasafari lumisilla reiteillä', 70.00, 24.00),
(17, 1, 'Koiravaljakkosafari', 1, 'Kiehtova seikkailu koiravaljakolla', 100.00, 24.00),
(18, 2, 'Jääkellunta', 2, 'Elämys jääkellunnasta arktisissa olosuhteissa', 60.00, 24.00),
(19, 3, 'Saunailta', 1, 'Rentouttava saunailta revontulien loisteessa', 45.00, 24.00),
(20, 1, 'Mökkipaketti', 2, 'Kaikki mukavuudet mökillä viikonlopun ajaksi', 150.00, 24.00),
(21, 2, 'Kaljakellunta', 1, 'Hyppää takaisin kultaisiin opiskelijavuosiin', 600.00, 24.00);
-- Add more service data as needed

-- Inserting data into 'varauksen_palvelut' table
INSERT INTO `vn`.`varauksen_palvelut` (`varaus_id`, `palvelu_id`, `lkm`) VALUES
(1, 11, 2),
(1, 13, 1),
(2, 12, 1),
(2, 14, 2),
(3, 15, 1),
(3, 16, 1),
(4, 17, 2),
(4, 18, 1),
(5, 19, 1),
(5, 20, 1);