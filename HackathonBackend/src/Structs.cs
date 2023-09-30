using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{
    internal class Structs
    {
        public class SQLIgnore : Attribute { }

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
            public string encriptedPassword;
            public string salt;
            public DateTime lastLogin;
        }

        public struct Bovine
        {
            public ulong id;
            public ulong ownerId;

            public string name;
            public bool male;

            public ulong father;
            public ulong mother;

            public DateTime birth;
            public DateTime death;

            public bool cull;
            public bool culled;
            public bool casterated;
        }

        public struct BreedingHistory
        {
            public ulong bovineId;

            public DateTime inseminationDate;
            public DateTime birthedDate;
            public bool stillborn;

            public ulong cow;
            public ulong bull;
        }

        public struct BovineWeight
        {
            public ulong bovineId;

            public DateTime date;
            public float weight;
        }

        public struct BovinePhotos
        {
            public ulong bovineId;

            public DateTime dateTaken;
            public string filePath;
        }

        public struct BovineNotes
        {
            public ulong bovineId;

            public int category;
            public DateTime creation;
            public string title;
            public string message;
        }

    }
}
