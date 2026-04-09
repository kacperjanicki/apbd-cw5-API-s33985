using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoomBackend.DTOs;
using RoomBackend.Models;

namespace RoomBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private List<Animal> animals = new List<Animal>()
        {
            new Animal() { id = 1, Name = "Słoń", Age = 2 },
            new Animal() { id = 2, Name = "Koń", Age = 8 }
        };
    
        
        // [FromQuery] w parametrze to Query Parameters, czyli
        // /api/test?id=2
        [HttpGet]
        public IActionResult Get([FromQuery] int id)
        {
            var animal = new Animal() { id = 1, Name = "Słoń", Age = 8 };
            if (id != 0)
            {
                Console.WriteLine($"Użytkownik wpisał {id} w query parameters");
                return Ok(animal);
            }
            else
            {
                return Ok(animals);
            }
        }
        
        //[Route] nad funkcją to Router Parameters, czyli
        // /api/test/1
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById(int id)
        {
            var animal = animals.FirstOrDefault(x => x.id == id);
            return Ok(animal);
        }

        // [HttpPost]
        // public IActionResult Post([FromBody] CreateAnimalDTO animalDTO)
        // {
        //     var animal = new Animal()
        //     {
        //         id = animals.Count + 1,
        //         Name = animalDTO.Name,
        //         Age = animalDTO.Age
        //     };
        //     animals.Add(animal);
        //     
        //     return Created();
        // }
    }
}
