using DAL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                if (user == null)
                {
                    user = DB.Users.Get(1).Copy();
                }
                return user;
            }
        }
        [JsonIgnore]
        private int _likesCount = -1;

        [JsonIgnore]
        private List<Like> _likesList = null;

        public void ResetCountsCalc()
        {
            _likesCount = -1;
            _likesList = null;
        }
        [JsonIgnore]
        public int LikesCount
        {
            get
            {
                if (_likesCount == -1)
                    _likesCount = Likes.Count();
                return _likesCount;
            }
        }
        [JsonIgnore]
        public List<Like> Likes
        {
            get
            {
                if (_likesList == null)
                    _likesList = DB.Likes.ToList().Where(l => l.MediaId == Id).ToList();
                return _likesList;
            }
        }

        [JsonIgnore]
        public string UsersLikesList
        {
            get
            {
                string UsersLikesList = "Usagers qui ont aimé :" + "\n";
                foreach (var like in Likes)
                {
                    User user = DB.Users.Get(like.UserId);
                    if (user != null) 
                        UsersLikesList += user.Name + "\n";
                }
                return UsersLikesList;
            }
        }
        public bool DeleteLikes()
        {
            List<Like> likes = DB.Likes.ToList().Where(l => l.MediaId == this.Id).ToList();
            likes.ForEach(m =>
            {
                DB.Likes.Delete(m.Id);
            });
            return true;
        }
    }
}