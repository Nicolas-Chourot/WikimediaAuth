using DAL;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class LikesRepository : Repository<Like>
    {
        public void ToggleLike(int mediaId, int userId)
        {
            Like like = ToList().Where(l => (l.MediaId == mediaId && l.UserId == userId)).FirstOrDefault();

            if (like != null)
            {
                DB.Notifications.Push(like.Media.OwnerId, like.User.Name + " n'aime plus votre commentaire \n");
                Delete(like.Id);
            }
            else
            {
                DB.Notifications.Push(like.Media.OwnerId, like.User.Name + " aime votre commentaire \n");
                Add(new Like { MediaId = mediaId, UserId = userId });
            }
        }
        public void DeleteByMediaId(int mediaId)
        {
            List<Like> list = ToList().Where(l => l.MediaId == mediaId).ToList().Copy();
            list.ForEach(l => Delete(l.Id));
        }
        public void DeleteByUserId(int userId)
        {
            List<Like> list = ToList().Where(l => l.UserId == userId).ToList().Copy();
            list.ForEach(l => Delete(l.Id));
        }
        public void DeleteByCommentId(int commentId)
        {
            List<Like> list = ToList().Where(l => l.CommentId == commentId).ToList();
            list.ForEach(l => Delete(l.Id));
        }
    }
}