using DogHouse.DTOs;
using DogHouse.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogHouse.Controllers;

[ApiController]
[Route("/")]
public class DogsController : ControllerBase
{
    private readonly IDogService _svc;
    public DogsController(IDogService svc) => _svc = svc;


    [HttpGet("dogs")]
    public async Task<IActionResult> GetDogs([FromQuery] string? attribute, [FromQuery] string? order, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100)
    {
        var (items, total) = await _svc.QueryDogsAsync(attribute, order, pageNumber, pageSize);
        return Ok(items);
    }


    [HttpPost("dog")]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogDto dto)
    {
        if (dto == null) return BadRequest("Invalid JSON or empty body");
        try
        {
            var created = await _svc.CreateDogAsync(dto);
            return CreatedAtAction(nameof(GetDogs), new { name = created.Name }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}