using DogHouse.Models;

namespace DogHouse.Repositories
{
    public interface IDogRepository
    {
        Task<IEnumerable<Dog>> GetAllAsync();
        Task<Dog?> GetByNameAsync(string name);
        Task AddAsync(Dog dog);
        Task<(IEnumerable<Dog> Items, int TotalCount)> QueryAsync(string? attribute, string? order, int pageNumber, int pageSize);
    }
}
