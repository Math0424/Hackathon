using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HackathonBackend.src
{

    public enum NoteCategory
    {
        Defects = 0,
        History = 1,
        Information = 2,
        Behavior = 3,
        Breeding = 4,
    }

    public struct User
    {
        public ulong id;
        public string username;
            
        [SQLIgnore]
        public string password;
            
        public string encriptedPassword;
        public string salt;
        public long lastLogin;
    }

    public struct Bovine
    {
        public ulong id;
        public ulong ownerId;

        public long registered;

        public string name;
        public bool male;

        public ulong father;
        public ulong mother;

        public long birth;
        public long death;

        public bool cull;
        public bool culled;
        public bool casterated;
    }

    public struct BreedingHistory
    {
        public ulong bovineId;

        public long inseminationDate;
        public long birthedDate;

        public bool stillborn;

        public ulong cow;
        public ulong bull;
    }

    public struct BovineWeight
    {
        public ulong bovineId;

        public long date;
        public float weight;
    }

    public struct BovinePhotos
    {
        public ulong bovineId;

        public long dateTaken;
        public string filePath;
    }

    public struct BovineNotes
    {
        public ulong bovineId;

        public int category;
        public long creation;

        public string title;
        public string message;
    }

}
