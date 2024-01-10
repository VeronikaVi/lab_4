namespace lab4
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;
    using System.Xml.Serialization;
    using System.Data.SQLite;

    /* создание класса музыкального каталога с методами, отвечающими соответствующим командам */
    class MusicCatalog
    {
        static List<MusicComposition> catalog = new List<MusicComposition>();
        const string jsonFilePath = "music_catalog.json";
        const string xmlFilePath = "music_catalog.xml";
        const string sqliteConnectionString = "Data Source=music_catalog.db;Version=3;";


        static void Main1()
        {
            Console.WriteLine("Добро пожаловать в Музыкальный каталог!");
            Console.WriteLine(
                "Используйте команды: search, list, add, delete, savejson, loadjson, savexml, loadxml, savesqlite, loadsqlite, exit");

            while (true)
            {
                Console.Write("Введите команду: ");
                string command = Console.ReadLine().ToLower();

                switch (command)
                {
                    case "search":
                        SearchComposition();
                        break;
                    case "list":
                        ListAllCompositions();
                        break;
                    case "add":
                        AddComposition();
                        break;
                    case "delete":
                        DeleteComposition();
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    case "savejson":
                        SaveToJSON();
                        break;
                    case "loadjson":
                        LoadFromJSON();
                        break;
                    case "savexml":
                        SaveToXML();
                        break;
                    case "loadxml":
                        LoadFromXML();
                        break;
                    case "savesqlite":
                        SaveToSQLite();
                        break;
                    case "loadsqlite":
                        LoadFromSQLite();
                        break;
                    default:
                        Console.WriteLine("Неверная команда. Пожалуйста, введите корректную команду.");
                        break;
                }
            }
        }

        /* метод для команды поиска, выводящий композции по одному из двух критериев */
        static void SearchComposition()
        {
            Console.Write("Введите критерий поиска (имя исполнителя/название композиции): ");
            string searchCriteria = Console.ReadLine().ToLower();

            var searchResults = catalog.FindAll(comp =>
                comp.Artist.ToLower().Contains(searchCriteria) ||
                comp.Title.ToLower().Contains(searchCriteria)
            );

            Console.WriteLine("Результаты поиска:");
            foreach (var result in searchResults)
            {
                Console.WriteLine($"{result.Artist} - {result.Title}");
            }
        }

        /* метод, выводящий в консоль все композиции каталога, если они есть */
        static void ListAllCompositions()
        {
            if (catalog.Count > 0)
            {
                Console.WriteLine("Все композиции в каталоге:");
                foreach (var composition in catalog)
                {
                    Console.WriteLine($"{composition.Artist} - {composition.Title}");
                }
            }
            else
            {
                Console.WriteLine(
                    "Пока что в каталоге пусто :(... Но это всегда можно исправить, добавив композиции с помощью команды add!");
            }
        }

        /* метод для добавления новой композиции в каталог */
        static void AddComposition()
        {
            Console.Write("Введите имя исполнителя: ");
            string artist = Console.ReadLine();

            Console.Write("Введите название композиции: ");
            string title = Console.ReadLine();

            catalog.Add(new MusicComposition(artist, title));
            Console.WriteLine("Композиция добавлена в каталог.");
        }

        /* метод для удаления композиций из каталога, удаляет композицию по артисту и названию */
        static void DeleteComposition()
        {
            Console.Write("Введите имя исполнителя: ");
            string artist = Console.ReadLine().ToLower();

            Console.Write("Введите название композиции: ");
            string title = Console.ReadLine().ToLower();

            var compositionToDelete = catalog.Find(comp =>
                comp.Artist.ToLower() == artist && comp.Title.ToLower() == title
            );

            if (compositionToDelete != null)
            {
                catalog.Remove(compositionToDelete);
                Console.WriteLine("Композиция удалена из каталога.");
            }
            else
            {
                Console.WriteLine("Композиция не найдена в каталоге.");
            }
        }
        
        /* метод для сохранения списка композиций в формате JSON */
        static void SaveToJSON()
        {
            string jsonData = JsonConvert.SerializeObject(catalog);
            File.WriteAllText(jsonFilePath, jsonData);
            Console.WriteLine("Данные сохранены в формате JSON.");
        }

        /* метод для загрузки списка композиций из формата JSON */
        static void LoadFromJSON()
        {
            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                catalog = JsonConvert.DeserializeObject<List<MusicComposition>>(jsonData);
                Console.WriteLine("Данные загружены из формата JSON.");
            }
            else
            {
                Console.WriteLine("Файл с данными в формате JSON не найден.");
            }
        }

        /* метод для сохранения списка композиций в формате XML */
        static void SaveToXML()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<MusicComposition>));
                using (var writer = new StreamWriter(xmlFilePath))
                {
                    serializer.Serialize(writer, catalog);
                }

                Console.WriteLine("Данные сохранены в формате XML.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в XML: {ex.Message}");
            }
        }

        /* метод для загрузки списка композиций из формата XML */
        static void LoadFromXML()
        {
            try
            {
                if (File.Exists(xmlFilePath))
                {
                    var serializer = new XmlSerializer(typeof(List<MusicComposition>));
                    using (var reader = new StreamReader(xmlFilePath))
                    {
                        catalog = (List<MusicComposition>)serializer.Deserialize(reader);
                    }

                    Console.WriteLine("Данные загружены из формата XML.");
                }
                else
                {
                    Console.WriteLine("Файл с данными в формате XML не найден.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке из XML: {ex.Message}");
            }
        }

        /* метод для сохранения списка композиций в формате SQLite */
        static void SaveToSQLite()
        {
            try
            {
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "CREATE TABLE IF NOT EXISTS MusicCatalog (Artist TEXT, Title TEXT)";
                        command.ExecuteNonQuery();

                        foreach (var composition in catalog)
                        {
                            command.CommandText = "INSERT INTO MusicCatalog (Artist, Title) VALUES (@Artist, @Title)";
                            command.Parameters.AddWithValue("@Artist", composition.Artist);
                            command.Parameters.AddWithValue("@Title", composition.Title);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                Console.WriteLine("Данные сохранены в базе данных SQLite.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении в SQLite: {ex.Message}");
            }
        }

        /* метод для загрузки списка композиций из формата SQLite */
        static void LoadFromSQLite()
        {
            try
            {
                catalog.Clear();
                using (var connection = new SQLiteConnection(sqliteConnectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand("SELECT * FROM MusicCatalog", connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string artist = reader["Artist"].ToString();
                                string title = reader["Title"].ToString();
                                catalog.Add(new MusicComposition(artist, title));
                            }
                        }
                    }
                }

                Console.WriteLine("Данные загружены из базы данных SQLite.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке из SQLite: {ex.Message}");
            }
        }

        /* класс музыкальной композиции, хранящий в себе имя исполнителя и название композиции */
        class MusicComposition
        {
            public string Artist { get; set; }
            public string Title { get; set; }

            public MusicComposition(string artist, string title)
            {
                Artist = artist;
                Title = title;
            }
        }
    }
}  