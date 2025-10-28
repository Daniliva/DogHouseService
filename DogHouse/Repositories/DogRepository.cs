using DogHouse.Data;
using DogHouse.Models;
using Microsoft.EntityFrameworkCore;

namespace DogHouse.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly AppDbContext _db;
        public DogRepository(AppDbContext db) => this._db = db;


        public async Task AddAsync(Dog dog)
        {
            await _db.Dogs.AddAsync(dog);
            await _db.SaveChangesAsync();
        }


        public async Task<IEnumerable<Dog>> GetAllAsync() => await _db.Dogs.AsNoTracking().ToListAsync();


        public async Task<Dog?> GetByNameAsync(string name) => await _db.Dogs.FirstOrDefaultAsync(d => d.Name == name);


        public async Task<(IEnumerable<Dog> Items, int TotalCount)> QueryAsync(string? attribute, string? order, int pageNumber, int pageSize)
        {
            var q = _db.Dogs.AsNoTracking().AsQueryable();


            bool desc = string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(attribute))
            {
                q = attribute.ToLower() switch
                {
                    "name" => desc ? q.OrderByDescending(d => d.Name) : q.OrderBy(d => d.Name),
                    "color" => desc ? q.OrderByDescending(d => d.Color) : q.OrderBy(d => d.Color),
                    "tail_length" => desc ? q.OrderByDescending(d => d.TailLength) : q.OrderBy(d => d.TailLength),
                    "weight" => desc ? q.OrderByDescending(d => d.Weight) : q.OrderBy(d => d.Weight),
                    _ => q
                };
            }


            var total = await q.CountAsync();


            if (pageNumber < 1) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;


            var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }
    }
}