using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{
    internal class Database
    {

        private static string databaseLocation = @"%appdata%/BovineTracker/database.db";
        private static SQLiteConnection connection;

        static Database()
        {
            databaseLocation = databaseLocation.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            if (!File.Exists(databaseLocation))
            {
                SQLiteConnection.CreateFile(databaseLocation);
            }

            connection = new SQLiteConnection($"Data Source={databaseLocation};Version=3;");
            connection.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS User (
                    id INTEGER PRIMARY KEY,
                    username TEXT,
                    encriptedPassword TEXT,
                    salt TEXT,
                    lastLogin INTEGER
                )";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

            sql = @"
                CREATE TABLE IF NOT EXISTS Bovine (
                    id INTEGER PRIMARY KEY,
                    ownerId INTEGER,
                    name TEXT,
                    male BOOLEAN,
                    father INTEGER,
                    mother INTEGER,
                    birth INTEGER,
                    death INTEGER,
                    cull BOOLEAN,
                    culled BOOLEAN,
                    casterated BOOLEAN
                );";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

            sql = @"
                CREATE TABLE IF NOT EXISTS BovineWeight (
                    bovineId INTEGER REFERENCES Bovine(id),
                    date INTEGER,
                    weight REAL
                );";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

            sql = @"
                CREATE TABLE IF NOT EXISTS BreedingHistory (
                    bovineId INTEGER REFERENCES Bovine(id),
                    inseminationDate INTEGER,
                    birthedDate INTEGER,
                    stillborn BOOLEAN,
                    cow INTEGER,
                    bull INTEGER
                );";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

            sql = @"
                CREATE TABLE IF NOT EXISTS BovinePhotos (
                    bovineId INTEGER REFERENCES Bovine(id),
                    dateTaken INTEGER,
                    filePath TEXT
                );";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

            sql = @"
                CREATE TABLE IF NOT EXISTS BovineNotes (
                    bovineId INTEGER REFERENCES Bovine(id),
                    category INTEGER,
                    creation INTEGER,
                    title TEXT,
                    message TEXT
                );";
            using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery();

        }



    }
}
