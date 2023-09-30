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
            Defects,
            History,
            Information,
            Behavior,
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
            float birthWeight;
            bool stillborn;

            ulong cow;
            ulong bull;
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
