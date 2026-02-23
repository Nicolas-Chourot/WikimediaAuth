using DAL;
using Newtonsoft.Json;
using System;

namespace Models
{
    public class Media : Record
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string YoutubeId { get; set; }
        public int OwnerId { get; set; } = 1;
        public bool Shared { get; set; } = true;
        public DateTime PublishDate { get; set; } = DateTime.Now;
        [JsonIgnore]
        public User Owner
        {
            get
            {
                User user = DB.Users.Get(OwnerId).Copy();
                return user;
            }
        }
    }
}