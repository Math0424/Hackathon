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
                Directory.CreateDirectory(Path.GetDirectoryName(databaseLocation));
                SQLiteConnection.CreateFile(databaseLocation);
            }

            Console.WriteLine($"Database located at {databaseLocation}");

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

        public async static Task CreateUser(User user)
        {
            ulong uniqueId = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0);
            user.id = uniqueId;
            user.lastLogin = DateTime.Now.Ticks;
            user.salt = Utils.GenerateSalt();
            user.encriptedPassword = Utils.HashPassword(user.password, user.salt);
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = GenerateInsertStatement(user, command, "User");
                await command.ExecuteNonQueryAsync();
            }
        }

        // SQL getters
        public async static Task<bool> HasUser(string username)
        {
            string sql = $"SELECT COUNT(*) FROM User WHERE username COLLATE NOCASE = \"{username}\"";
            using (var command = new SQLiteCommand(sql, connection))
            {
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
        }

        public async static Task<User?> GetUser(string username)
        {
            string sql = $"SELECT * FROM User WHERE username COLLATE NOCASE = \"{username}\"";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        User user = new User
                        {
                            id = (ulong)reader["id"],
                            username = reader["username"].ToString(),
                            encriptedPassword = reader["encriptedPassword"].ToString(),
                            salt = reader["salt"].ToString(),
                            lastLogin = (long)reader["lastLogin"],
                        };
                        return user;
                    }
                }
            }
            return null;
        }

        public async static Task<List<ulong>> GetUsers()
        {
            string sql = $"SELECT id FROM User";
            List<ulong> users = new List<ulong>();
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        users.Add((ulong)reader["id"]);
                    }
                }
            }
            return users;
        }

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
            string sql = $"SELECT * FROM Bovine WHERE id = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        return new Bovine()
                        {
                            id = (ulong)reader["id"],
                            ownerId = (ulong)reader["ownerId"],
                            name = reader["name"].ToString(),
                            male = (bool)reader["male"],
                            father = (ulong)reader["father"],
                            mother = (ulong)reader["mother"],
                            birth = (long)reader["birth"],
                            death = (long)reader["death"],
                            cull = (bool)reader["cull"],
                            culled = (bool)reader["culled"],
                            casterated = (bool)reader["casterated"]
                        };
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
                            creation = (long)reader["creation"],
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
                            dateTaken = (long)reader["dateTaken"],
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
                            date = (long)reader["date"],
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
                        conception.inseminationDate =(long)reader["inseminationDate"];
                        conception.birthedDate = (long)reader["birthedDate"];
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
                            inseminationDate = (long)reader["inseminationDate"],
                            birthedDate = (long)reader["birthedDate"],
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
