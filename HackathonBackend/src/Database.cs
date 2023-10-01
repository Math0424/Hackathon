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
            databaseLocation = databaseLocation.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

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
            var fields = obj.GetType().GetFields().Where(f => f.GetCustomAttributes(true).Count(e => e.GetType() == typeof(SQLIgnore)) == 0);
            var fieldNames = fields.Select(f => f.Name).ToArray();
            var paramNames = fields.Select(f => "@" + f.Name).ToArray();
            string sql = $"INSERT INTO {table} ({string.Join(", ", fieldNames)}) VALUES ({string.Join(", ", paramNames)})";

            foreach (var f in fields)
                cmd.Parameters.AddWithValue("@" + f.Name, f.GetValue(obj));

            return sql;
        }

        // SQL deletes
        public async static Task DeleteNote(BovineNotes note)
        {
            note.creation = DateTime.Now.Ticks;
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = $"DELETE FROM BovineNotes WHERE bovineId = {note.bovineId} AND creation = {note.creation};";
                await command.ExecuteNonQueryAsync();
            }
        }

        // SQL setters
        public async static Task<long> CreateCow(long ownerId, Bovine bovine)
        {
            long uniqueId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            uniqueId = uniqueId < 0 ? uniqueId * -1 : uniqueId;

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
            long uniqueId = BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
            uniqueId = uniqueId < 0 ? uniqueId * -1 : uniqueId;

            user.id = uniqueId;
            user.lastLogin = DateTime.Now.Ticks;
            user.salt = Utils.GenerateSalt();
            user.encriptedPassword = Utils.HashPassword(user.password, user.salt);
            using (var command = new SQLiteCommand(connection))
            {
                command.CommandText = GenerateInsertStatement(user, command, "User");
                Console.WriteLine(GenerateInsertStatement(user, command, "User"));
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
                            id = (long)reader["id"],
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

        public async static Task<List<long>> GetUsers()
        {
            string sql = $"SELECT id FROM User";
            List<long> users = new List<long>();
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        users.Add(reader.GetInt64(0));
                    }
                }
            }
            return users;
        }

        public async static Task<List<long>> GetUserBovineIds(long id)
        {
            List<long> bovineIds = new List<long>();

            string sql = $"SELECT id FROM Bovine WHERE ownerId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (reader.Read())
                    {
                        bovineIds.Add((long)reader["id"]);
                    }
                }
            }

            return bovineIds;
        }

        public async static Task<List<Bovine>> GetUserBovineDetails(long id)
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
                            id = (long)reader["id"],
                            name = reader["name"].ToString(),
                            male = (bool)reader["male"]
                        };
                        bovines.Add(bovine);
                    }
                }
            }
            return bovines;
        }

        public async static Task<bool> HasBovine(long id)
        {
            string sql = $"SELECT COUNT(*) FROM Bovine WHERE id = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
            }
        }

        public async static Task<Bovine?> GetBovine(long id)
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
                            id = (long)reader["id"],
                            ownerId = (long)reader["ownerId"],
                            name = reader["name"].ToString(),
                            male = (bool)reader["male"],
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

        public async static Task<List<BovineNotes>> GetBovineNotes(long id)
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
                            bovineId = (long)reader["bovineId"],
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

        public async static Task<List<BovinePhotos>> GetBovinePhotos(long id)
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
                            bovineId = (long)reader["bovineId"],
                            dateTaken = (long)reader["dateTaken"],
                            filePath = reader["filePath"].ToString()
                        };
                        photos.Add(photo);
                    }
                }
            }
            return photos;
        }

        public async static Task<List<BovineWeight>> GetBovineWeights(long id)
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
                            bovineId = (long)reader["bovineId"],
                            date = (long)reader["date"],
                            weight = (float)reader["weight"]
                        };
                        weights.Add(weight);
                    }
                }
            }
            return weights;
        }

        public async static Task<BreedingHistory> GetBovineConception(long id)
        {
            BreedingHistory conception = new BreedingHistory();

            string sql = $"SELECT * FROM BreedingHistory WHERE bovineId = {id}";
            using (var command = new SQLiteCommand(sql, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        conception.bovineId = (long)reader["bovineId"];
                        conception.inseminationDate =(long)reader["inseminationDate"];
                        conception.birthedDate = (long)reader["birthedDate"];
                        conception.stillborn = (bool)reader["stillborn"];
                        conception.cow = (long)reader["cow"];
                        conception.bull = (long)reader["bull"];
                    }
                }
            }
            return conception;
        }

        public async static Task<List<BreedingHistory>> GetBovineChildren(long id)
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
                            bovineId = (long)reader["bovineId"],
                            inseminationDate = (long)reader["inseminationDate"],
                            birthedDate = (long)reader["birthedDate"],
                            stillborn = (bool)reader["stillborn"],
                            cow = (long)reader["cow"],
                            bull = (long)reader["bull"]
                        };
                        births.Add(birth);
                    }
                }
            }
            return births;
        }

    }
}
