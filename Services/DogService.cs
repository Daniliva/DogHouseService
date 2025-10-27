using DogHouse.DTOs;
using DogHouse.Models;
using DogHouse.Repositories;

namespace DogHouse.Services
{
    public class DogService : IDogService
    {
        private readonly IDogRepository _repo;
        public DogService(IDogRepository repo) => _repo = repo;


        public Task<string> PingAsync() => Task.FromResult("Dogshouseservice.Version1.0.1");


        public async Task<(IEnumerable<DogDto> Items, int TotalCount)> QueryDogsAsync(string? attribute, string? order, int pageNumber, int pageSize)
        {
            var (items, total) = await _repo.QueryAsync(attribute, order, pageNumber, pageSize);
            var dtos = items.Select(d => new DogDto { Name = d.Name, Color = d.Color, TailLength = d.TailLength, Weight = d.Weight });
            return (dtos, total);
        }


        public async Task<DogDto> CreateDogAsync(CreateDogDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required");
            if (dto.TailLength < 0) throw new ArgumentException("TailLength must be non-negative");
            if (dto.Weight < 0) throw new ArgumentException("Weight must be non-negative");


            var exists = await _repo.GetByNameAsync(dto.Name);
            if (exists != null) throw new InvalidOperationException("Dog with the same name already exists");


            var dog = new Dog { Name = dto.Name, Color = dto.Color, TailLength = dto.TailLength, Weight = dto.Weight };
            await _repo.AddAsync(dog);
            return new DogDto { Name = dog.Name, Color = dog.Color, TailLength = dog.TailLength, Weight = dog.Weight };
        }
    }
}
