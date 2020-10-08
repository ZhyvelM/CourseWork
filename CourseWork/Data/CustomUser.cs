using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseWork.Data
{
    class CustomUser
    {
        public DiscordMember member { get; private set; }
        public DateTime joinTime { get; set; }
        public DateTime leaveTime { get; set; }

        private TimeSpan total;

        public bool isHere;

        public CustomUser(DiscordMember member)
        {
            this.member = member;
            joinTime = DateTime.Now;
            leaveTime = new DateTime();
            isHere = true;
        }

        public void Disconnect()
        {
            isHere = false;
            getTotal();
        }

        public void rCon()
        {
            joinTime = DateTime.Now;
        }
        public TimeSpan getTotal()
        {
            leaveTime = DateTime.Now;
            total += leaveTime - joinTime;
            return total;
        }
    }
}
