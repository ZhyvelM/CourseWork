using CourseWork.Data;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork
{
    class ServerDb
    {
        static ServerDb db;

        public List<CustomUser> participants;
        public List<DiscordMember> membersRightNow;

        public TimeSpan durationNeed;
        public bool isStarted;

        public ServerDb(TimeSpan duration)
        {
            participants = new List<CustomUser>();
            durationNeed = duration;
        }
    }
}
