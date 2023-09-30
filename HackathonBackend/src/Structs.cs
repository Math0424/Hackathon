using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonBackend.src
{
    internal class Structs
    {
        enum NoteCategory
        {
            Defects = 0,
            History = 1,
            Information = 2,
            Behavior = 3,
            Breeding = 4,
        }

        struct User
        {
            ulong id;
            string username;
            string encriptedPassword;
            string salt;
            DateTime lastLogin;
        }

        struct Bovine
        {
            ulong id;
            ulong ownerId;

            string name;
            bool male;

            ulong father;
            ulong mother;

            DateTime birth;
            DateTime death;

            bool cull;
            bool culled;
            bool casterated;
        }

        struct BreedingHistory
        {
            ulong bovineId;

            DateTime inseminationDate;
            DateTime birthedDate;
            bool stillborn;

            ulong cow;
            ulong bull;
        }

        struct BovineWeight
        {
            ulong bovineId;

            DateTime date;
            float weight;
        }

        struct BovinePhotos
        {
            ulong bonvineId;

            DateTime dateTaken;
            string filePath;
        }

        struct BovineNotes
        {
            ulong bovineId;

            int category;
            DateTime creation;
            string title;
            string message;
        }

    }
}
