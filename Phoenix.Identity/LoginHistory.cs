using System;

namespace Phoenix.Identity
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IP { get; set; }
        public DateTimeOffset LoginDate { get; set; }
        public bool Successed { get; set; }

    }
}