using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class MediasRepository : Repository<Media>
    {
        public List<string> MediasCategories()
        {
            List<string> Categories = new List<string>();
            foreach (Media media in ToList().OrderBy(m => m.Category))
            {
                if (Categories.IndexOf(media.Category) == -1)
                {
                    Categories.Add(media.Category);
                }
            }
            return Categories;
        }
        public List<User> OwnersList()
        {
            List<User> owners = new List<User>();
            foreach (Media media in ToList())
            {
                if (!owners.Where(o => o.Id == media.OwnerId).Any())
                {
                    owners.Add(media.Owner);
                }
            }
            return owners.OrderBy(o => o.Name).ToList();
        }

        public void DeleteByOwnerId(int ownerId)
        {
            List<Media> list = ToList().Where(m => m.OwnerId == ownerId).ToList();
            list.ForEach(m =>
            {
                base.Delete(m.Id);
            });
        }

       
    }
}