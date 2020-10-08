using CourseWork.Data;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace CourseWork.Dao
{
    class dbDao
    {
        private Dictionary<DiscordGuild, ServerDb> servers = new Dictionary<DiscordGuild, ServerDb>();

        public void setParticipants(List<DiscordMember> users, List<CustomUser> members, DiscordGuild server, TimeSpan duration)
        {
            if (!servers.Keys.Contains(server))
            {
                servers.Add(server, new ServerDb(duration));
            }
            servers[server].participants = members;
            servers[server].membersRightNow = users;
            servers[server].isStarted = true;
        }

        public TimeSpan getDuration(DiscordGuild server)
        {
            return servers[server].durationNeed;
        }

        public List<CustomUser> getParticipants(DiscordGuild server)
        {
            return servers[server].participants;
        }
        public List<DiscordMember> getUsers(DiscordGuild server)
        {
            return servers[server].membersRightNow;
        }

        public void addUser(CustomUser user, DiscordGuild server)
        {
            servers[server].participants.Add(user);
        }

        public bool isStarted(DiscordGuild server)
        {
            if (servers.Keys.Contains(server))
            {
                if(servers[server].isStarted)
                return true;
            }
            return false;
        }

        public void ended(DiscordGuild server)
        { 
            servers[server].isStarted = false;
            servers.Remove(server);
        }
    }
}
