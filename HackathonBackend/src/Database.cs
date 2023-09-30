using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HackathonBackend.src.Structs;

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
                    ownerId INTEGER REFERENCES User(id),
                    registered INTEGER,
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

        private static string GenerateInsertStatement(object obj, SQLiteCommand cmd, string table)
        {
            var properties = obj.GetType().GetProperties().Where(p => p.GetCustomAttributes(true).Count(e => e.GetType() == typeof(SQLIgnore)) == 0);
            var fieldNames = properties.Select(p => p.Name).ToArray();
            var paramNames = properties.Select(p => "@" + p.Name).ToArray();
            string sql = $"INSERT INTO {table} ({string.Join(", ", fieldNames)}) VALUES ({string.Join(", ", paramNames)})";

            foreach (var p in properties)
                cmd.Parameters.AddWithValue("@" + p.Name, p.GetValue(obj));

            return sql;
        }
        
        //SQL setters
        public async static Task<ulong> CreateCow(ulong ownerId, Bovine bovine)
        {
            ulong uniqueId = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);
            bovine.registered = DateTime.Now.Ticks;
            bovine.id = uniqueId;
            bovine.ownerId = ownerId;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = GenerateInsertStatement(bovine, command, "Bovine");
                await command.ExecuteNonQueryAsync();
            }
            return bovine.id;
        }

        public async static Task CreateNote(BovineNotes note)
        {
            note.creation = DateTime.Now.Ticks;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = GenerateInsertStatement(note, command, "BovineNotes");
                await command.ExecuteNonQueryAsync();
            }
        }

        public async static Task AddWeight(BovineWeight weight)
        {
            weight.date = DateTime.Now.Ticks;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = GenerateInsertStatement(weight, command, "BovineWeight");
                await command.ExecuteNonQueryAsync();
            }
        }

        // SQL getters
        public async static Task<List<ulong>> GetUserBovineIds(ulong id)
        {
            List<ulong> bovineIds = new List<ulong>();

            string sql = $"SELECT id FROM Bovine WHERE ownerId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        bovineIds.Add((ulong)reader["id"]);
                    }
                }
            }

            return bovineIds;
        }

        public async static Task<List<Bovine>> GetUserBovineDetails(ulong id)
        {
            List<Bovine> bovines = new List<Bovine>();

            string sql = $"SELECT id, name, male FROM Bovine WHERE ownerId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        Bovine bovine = new Bovine
                        {
                            id = (ulong)reader["id"],
                            name = reader["name"].ToString(),
                            male = (bool)reader["male"]
                        };
                        bovines.Add(bovine);
                    }
                }
            }
            return bovines;
        }

        public async static Task<Bovine?> GetBovine(ulong id)
        {
            Bovine bovine;
            string sql = $"SELECT * FROM Bovine WHERE id = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        bovine.id = (ulong)reader["id"];
                        bovine.ownerId = (ulong)reader["ownerId"];
                        bovine.name = reader["name"].ToString();
                        bovine.male = (bool)reader["male"];
                        bovine.father = (ulong)reader["father"];
                        bovine.mother = (ulong)reader["mother"];
                        bovine.birth = new DateTime((long)reader["birth"]);
                        bovine.death = new DateTime((long)reader["death"]);
                        bovine.cull = (bool)reader["cull"];
                        bovine.culled = (bool)reader["culled"];
                        bovine.casterated = (bool)reader["casterated"];
                    }
                }
            }
            return null;
        }

        public async static Task<List<BovineNotes>> GetBovineNotes(ulong id)
        {
            List<BovineNotes> notes = new List<BovineNotes>();

            string sql = $"SELECT * FROM BovineNotes WHERE bovineId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        BovineNotes note = new BovineNotes
                        {
                            bovineId = (ulong)reader["bovineId"],
                            category = (int)reader["category"],
                            creation = new DateTime((long)reader["creation"]),
                            title = reader["title"].ToString(),
                            message = reader["message"].ToString()
                        };
                        notes.Add(note);
                    }
                }
            }
            return notes;
        }

        public async static Task<List<BovinePhotos>> GetBovinePhotos(ulong id)
        {
            List<BovinePhotos> photos = new List<BovinePhotos>();

            string sql = $"SELECT * FROM BovinePhotos WHERE bovineId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        BovinePhotos photo = new BovinePhotos
                        {
                            bovineId = (ulong)reader["bovineId"],
                            dateTaken = new DateTime((long)reader["dateTaken"]),
                            filePath = reader["filePath"].ToString()
                        };
                        photos.Add(photo);
                    }
                }
            }
            return photos;
        }

        public async static Task<List<BovineWeight>> GetBovineWeights(ulong id)
        {
            List<BovineWeight> weights = new List<BovineWeight>();

            string sql = $"SELECT * FROM BovineWeight WHERE bovineId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        BovineWeight weight = new BovineWeight
                        {
                            bovineId = (ulong)reader["bovineId"],
                            date = new DateTime((long)reader["date"]),
                            weight = (float)reader["weight"]
                        };
                        weights.Add(weight);
                    }
                }
            }
            return weights;
        }

        public async static Task<BreedingHistory> GetBovineConception(ulong id)
        {
            BreedingHistory conception = new BreedingHistory();

            string sql = $"SELECT * FROM BreedingHistory WHERE bovineId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        conception.bovineId = (ulong)reader["bovineId"];
                        conception.inseminationDate = new DateTime((long)reader["inseminationDate"]);
                        conception.birthedDate = new DateTime((long)reader["birthedDate"]);
                        conception.stillborn = (bool)reader["stillborn"];
                        conception.cow = (ulong)reader["cow"];
                        conception.bull = (ulong)reader["bull"];
                    }
                }
            }
            return conception;
        }

        public async static Task<List<BreedingHistory>> GetBovineChildren(ulong id)
        {
            List<BreedingHistory> births = new List<BreedingHistory>();

            string sql = $"SELECT * FROM BreedingHistory WHERE cow = {id} OR bull = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        BreedingHistory birth = new BreedingHistory
                        {
                            bovineId = (ulong)reader["bovineId"],
                            inseminationDate = new DateTime((long)reader["inseminationDate"]),
                            birthedDate = new DateTime((long)reader["birthedDate"]),
                            stillborn = (bool)reader["stillborn"],
                            cow = (ulong)reader["cow"],
                            bull = (ulong)reader["bull"]
                        };
                        births.Add(birth);
                    }
                }
            }
            return births;
        }

    }
}
