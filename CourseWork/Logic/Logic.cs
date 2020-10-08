using CourseWork.Dao;
using CourseWork.Data;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CourseWork.Logic
{
    public class Logic
    {
        dbDao dao = new dbDao();
        static Logic lg;

        private Logic()
        {
        }
        public static Logic GetInstanse()
        {
            if (lg == null)
            {
                lg = new Logic();
            }
            return lg;
        }

        public bool isStarted(DiscordGuild server)
        {
            return dao.isStarted(server);
        }
        public void setParticipants(List<DiscordMember> members, DiscordGuild server, TimeSpan duration)
        {
            List<CustomUser> users = new List<CustomUser>();
            foreach (var m in members)
            {
                users.Add(new CustomUser(m));
            }
            dao.setParticipants(members, users, server, duration);
        }
        public List<DiscordMember> getParticipants(DiscordGuild server)
        {
            var users = dao.getParticipants(server);
            List<DiscordMember> members = new List<DiscordMember>();
            foreach (var u in users)
            {
                TimeSpan t1 = u.getTotal();
                TimeSpan t2 = dao.getDuration(server);

                if (t1>=t2)
                {
                    members.Add(u.member);
                }
            }
            return members;
        }
        public void updateParticipants(List<DiscordMember> members, DiscordGuild server)
        {
            if (dao.getUsers(server).Count > members.Count)
            {
                removeParticipant(members, server);
            }
            else
            {
                addParticipant(members, server);
            }
        }
        private void removeParticipant(List<DiscordMember> members, DiscordGuild server)
        {
            var users = dao.getUsers(server);
            var user = users.Select(x => members.Contains(x) == false);
            var participants = dao.getParticipants(server);
            for (int i = 0; i < participants.Count; i++)
            {
                if (user == participants[i].member)
                {
                    participants[i].Disconnect();
                    break;
                }
            }
        }
        private void addParticipant(List<DiscordMember> members, DiscordGuild server)
        {
            var users = dao.getParticipants(server);
            foreach (var u in members)
            {
                if (!checkIsContain(u, users))
                {
                    dao.addUser(new CustomUser(u), server);
                    return;
                }
            }
            updateUser(members, server);
        }
        private void updateUser(List<DiscordMember> members, DiscordGuild server)
        {
            var users = dao.getUsers(server);
            var user = members.Select(x => users.Contains(x) == false);
            var participants = dao.getParticipants(server);
            for (int i = 0; i < participants.Count; i++)
            {
                if (user == participants[i].member)
                {
                    participants[i].rCon();
                    break;
                }
            }
        }
        private bool checkIsContain(DiscordMember member, List<CustomUser> users)
        {           
            for (int i = 0; i < users.Count; i++)
            {
                if (member == users[i].member)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
