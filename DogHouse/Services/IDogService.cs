using DogHouse.DTOs;

namespace DogHouse.Services
{
    public interface IDogService
    {
        Task<string> PingAsync();
        Task<(IEnumerable<DogDto> Items, int TotalCount)> QueryDogsAsync(string? attribute, string? order, int pageNumber, int pageSize);
        Task<DogDto> CreateDogAsync(CreateDogDto dto);
    }
}