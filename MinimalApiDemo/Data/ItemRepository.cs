using MinimalApiDemo.Models;

namespace MinimalApiDemo.Data
{
    public class ItemRepository
    {
        private readonly AppDbContext _db;

        public ItemRepository(AppDbContext db)
        {
            _db = db;
        }

        public List<Item> GetAll() => _db.Itmes.ToList();

        public Item? GetById(int id) => _db.Itmes.FirstOrDefault(x => x.Id == id) == null
            ? null
            : _db.Itmes.First(x => x.Id == id);
        
        public void Add(Item newItem)  {
            _db.Itmes.Add(newItem);
            _db.SaveChanges();
        }
        
        public void Update(Item updItem) 
        {
            var item = GetById(updItem.Id);

            if(item != null)
            {
                item.Todo = updItem.Todo;
                item.Completed = updItem.Completed;

                _db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var item = GetById(id);

            if( item != null ) 
                _db.Remove(item);

            _db.SaveChanges();  
        }
    }
}
