using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Models
{
    public enum Access { Anonymous, View, Write, Admin }

    public class User : Record
    {
        public User()
        {
            SetNew();
            Notify = true;
        }

        public void SetNew()
        {
            Id = 0;
            Blocked = false;
            Access = Access.View;
            Online = false;
            Verified = false;
        }

        #region Data Members
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Access Access { get; set; }
        public bool Blocked { get; set; }
        public bool Verified { get; set; }
        public bool Notify { get; set; }

        public override bool IsValid()
        {
            if (DB.Users.ToList().Where(u => u.Email == Email && u.Id != Id).Any()) return false;
            if (!IsAlpha(Name)) return false;
            if (!IsEmail(Email)) return false;
            if (!HasRequiredLength(Password, 6)) return false;
            return true;
        }

        const string Avatars_Folder = @"/App_Assets/Users/";
        const string Default_Avatar = @"no_avatar.png";
        [ImageAsset(Avatars_Folder, Default_Avatar)]
        public string Avatar { get; set; } = Avatars_Folder + Default_Avatar;

        #endregion

        #region View members
        [JsonIgnore]
        public bool Online
        {
            // todo - flush experired session
            // maintain in server cache a list of online users Id
            get
            {
                return User.GetOnlineUsers().IndexOf(this.Id) > -1;
            }
            set
            {
                if (value)
                {
                    if (User.GetOnlineUsers().IndexOf(this.Id) == -1)
                        User.GetOnlineUsers().Add(this.Id);
                }
                else
                    User.GetOnlineUsers().Remove(this.Id);
            }
        }
        [JsonIgnore]
        public bool IsAdmin { get { return Access == Access.Admin; } }
        [JsonIgnore]
        public bool CanWrite { get { return Access >= Access.Write; } }
        [JsonIgnore]
        public bool IsBlocked { get { return Blocked; } }
        [JsonIgnore]
        public bool IsOnline { get { return Online; } }
        #endregion

        [JsonIgnore]
        public List<Login> Logins { get { return DB.Logins.ToList().Where(l => l.UserId == Id).ToList(); } }

        [JsonIgnore]
        public List<Like> Likes { get { return DB.Likes.ToList().Where(l => l.UserId == Id).ToList(); } }

        [JsonIgnore]
        public List<Media> Medias { get { return DB.Medias.ToList().Where(l => l.OwnerId == Id).ToList(); } }

        public void DeleteLogins()
        {
            foreach (Login login in Logins)
            {
                DB.Logins.Delete(login.Id);
            }
        }
        public void DeleteLikes()
        {
            foreach (Like like in Likes.Copy())
            {
                DB.Likes.Delete(like.Id);
            }
        }
        public void DeleteMedias()
        {
            foreach (Media media in Medias.Copy())
            {
                DB.Medias.Delete(media.Id);
            }
        }
        public void DeleteComments()
        {
            DB.Comments.DeleteByOwnerId(Id);
        }
        public static List<int> GetOnlineUsers()
        {
            if (HttpRuntime.Cache["onlineUsers"] == null)
                HttpRuntime.Cache["onlineUsers"] = new List<int>();
            return (List<int>)HttpRuntime.Cache["onlineUsers"];
        }

        public static User ConnectedUser
        {
            get
            {
                if (HttpContext.Current?.Session["ConnectedUser"] != null)
                {
                    if (DB.Users.IsMarkedChanged)
                    {
                        User connectedUser = ((User)HttpContext.Current.Session["ConnectedUser"]);
                        if (connectedUser != null)
                            connectedUser = DB.Users.Get(connectedUser.Id);
                        // update connected user
                        HttpContext.Current.Session["ConnectedUser"] = connectedUser;
                    }
                    return ((User)HttpContext.Current.Session["ConnectedUser"]).Copy();
                }
                return null;
            }
            set
            {
                HttpContext.Current.Session["ConnectedUser"] = value;
            }
        }
    }
}