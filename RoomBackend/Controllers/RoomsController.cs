using Microsoft.AspNetCore.Mvc;
using RoomBackend.Models;

namespace RoomBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoomsController : ControllerBase
{
    private static List<Room> _rooms = new List<Room>()
    {
        new Room(){Id=1, Floor = 2, Capacity = 4, HasProjector = false, IsActive = true, Name = "nazwa", BuildingCode = "2B"},
        new Room(){Id=2, Floor = 1, Capacity = 2, HasProjector = true, IsActive = true, Name = "nazwa2", BuildingCode = "1A"},
        new Room(){Id=3, Floor = 5, Capacity = 6, HasProjector = true, IsActive = true, Name = "nazwa3", BuildingCode = "1A"}
    };

    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetAll([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool? activeOnly)
    {
        var query = _rooms.AsQueryable();
        if (minCapacity.HasValue) query = query.Where(r => r.Capacity >= minCapacity.Value);
        if (hasProjector.HasValue) query = query.Where(r => r.HasProjector == hasProjector.Value);
        if (activeOnly.HasValue) query = query.Where(r => r.IsActive == activeOnly.Value);
        
        return Ok(query.ToList());
    }
    
   [HttpGet("{id:int}")]
   public ActionResult<Room> GetById(int id)
   {
       var room = _rooms.FirstOrDefault(r => r.Id == id);

       if (room == null) return NotFound($"Nie znaleziono pokoju o numerze id: {id}");

       return Ok(room);
   }

   [HttpGet("building/{buildingCode}")]
   public ActionResult<Room> GetByBuildingCode(string buildingCode)
   {
       var rooms = _rooms.Where(r => r.BuildingCode == buildingCode).ToList();

       if (!rooms.Any())
       {
           return NotFound($"Budynek {buildingCode} nie istnieje");
       }

       return Ok(rooms);
   }

   [HttpPost]
   public ActionResult<Room> Create([FromBody] Room newRoom)
   {
       int newId = _rooms.Any() ? _rooms.Max(r => r.Id) + 1 : 1;
       newRoom.Id = newId;
       
       _rooms.Add(newRoom);

       return CreatedAtAction(nameof(GetById), new { id = newRoom.Id }, newRoom);
   }
   
   [HttpPut("{id}")]
   public ActionResult<Room> Update(int id, [FromBody] Room updatedRoom)
   {
       var roomToEdit = _rooms.FirstOrDefault(r => r.Id == id);

       if (roomToEdit == null) return NotFound($"Pokój o id {id} nie istnieje");    
       
       roomToEdit.Name = updatedRoom.Name;
       roomToEdit.BuildingCode = updatedRoom.BuildingCode;
       roomToEdit.Floor = updatedRoom.Floor;
       roomToEdit.Capacity = updatedRoom.Capacity;
       roomToEdit.HasProjector = updatedRoom.HasProjector;
       roomToEdit.IsActive = updatedRoom.IsActive;
       

       return Ok($"Pokój {id}:{roomToEdit.Name} został zaaktualizowany.");
   }
   
   [HttpDelete("{id}")]
   public ActionResult Delete(int id)
   {
       var room = _rooms.FirstOrDefault(r => r.Id == id);

       if (room == null)
       {
           return NotFound($"Pokój o id {id} nie istnieje");
       }

       _rooms.Remove(room);

       return Ok($"Pokój o id {id} został pomyślnie usunięty");
   }
   
}