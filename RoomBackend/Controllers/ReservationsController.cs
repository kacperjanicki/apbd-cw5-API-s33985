namespace RoomBackend.Controllers;

using Microsoft.AspNetCore.Mvc;
using RoomBackend.Models;
[Route("api/[controller]")]
[ApiController]
public class ReservationsController : ControllerBase
{
    public static List<Reservation> _reservations = new List<Reservation>()
    {
        new Reservation(){Id = 1, RoomId = 2, Status = StatusOptions.Confirmed, OrganizerName = "Jacob", Topic = "Stock trading", Date = DateTime.Parse("2026-04-15"), StartTime = DateTime.Parse("2026-04-15T08:00:00"), EndTime = DateTime.Parse("2026-04-15T10:00:00")},
        new Reservation(){Id = 2, RoomId = 1, Status = StatusOptions.Confirmed, OrganizerName = "Anna Nowak", Topic = "Project Kickoff", Date = DateTime.Parse("2026-04-16"), StartTime = DateTime.Parse("2026-04-16T09:30:00"), EndTime = DateTime.Parse("2026-04-16T11:00:00")},
        new Reservation(){Id = 3, RoomId = 3, Status = StatusOptions.Planned, OrganizerName = "Marek Kowalski", Topic = "Quarterly Review", Date = DateTime.Parse("2026-04-16"), StartTime = DateTime.Parse("2026-04-16T13:00:00"), EndTime = DateTime.Parse("2026-04-16T15:30:00")},
        new Reservation(){Id = 4, RoomId = 2, Status = StatusOptions.Cancelled, OrganizerName = "Karolina Lis", Topic = "Brainstorming Session", Date = DateTime.Parse("2026-04-17"), StartTime = DateTime.Parse("2026-04-17T10:00:00"), EndTime = DateTime.Parse("2026-04-17T12:00:00")},
        new Reservation(){Id = 5, RoomId = 5, Status = StatusOptions.Confirmed, OrganizerName = "Piotr Zieliński", Topic = "Final Interviews", Date = DateTime.Parse("2026-04-18"), StartTime = DateTime.Parse("2026-04-18T08:00:00"), EndTime = DateTime.Parse("2026-04-18T16:00:00")}
    };

    [HttpGet("{id:int}")]
   public ActionResult<Reservation> GetById(int id)
   {
       var reservation = _reservations.FirstOrDefault(r => r.Id == id);
   
       if (reservation == null) return NotFound($"Nie znaleziono rezerwacji o numerze id: {id}");
   
       return Ok(reservation);
   }

   [HttpGet]
   public ActionResult<IEnumerable<Reservation>> GetAll([FromQuery] DateTime? date, [FromQuery] string? status, [FromQuery] int? roomId)
   {
       var query = _reservations.AsQueryable();
       if (date.HasValue) query = query.Where(r => r.Date == date.Value);
       if (!string.IsNullOrWhiteSpace(status)) query = query.Where(r => r.Status.ToString().ToLower() == status.ToLower());
       if (roomId.HasValue) query = query.Where(r => r.RoomId == roomId.Value);

       if (query.ToList().Count == 0)
       {
           return Ok("Nie znaleziono żadnych pozycji spełniających twoje parametry");
       }
       
       return Ok(query.ToList());
   }

   [HttpPost]
   public ActionResult<Reservation> Create([FromBody] Reservation newReservation)
   {
       int newId = _reservations.Any() ? _reservations.Max(r => r.Id) + 1 : 1;
       newReservation.Id = newId;
       
       // Nie wolno dodać rezerwacji dla sali, która nie istnieje
       var roomExists = RoomsController._rooms.Any(r => r.Id == newReservation.RoomId);
       if (!roomExists) return NotFound($"Pokój o id {newReservation.RoomId} nie istnieje");
       
       // Nie wolno dodać rezerwacji dla sali oznaczonej jako nieaktywna.
       var room = RoomsController._rooms.FirstOrDefault(r => r.Id == newReservation.RoomId);
       if (!room.IsActive) return Conflict($"Pokój o id {newReservation.RoomId} ma status nieaktywny");
       
       // Dwie rezerwacje tej samej sali nie mogą nakładać się czasowo tego samego dnia.
       bool hasOverlap = _reservations.Any(r => 
           r.RoomId == newReservation.RoomId && 
           r.StartTime < newReservation.EndTime && 
           r.EndTime > newReservation.StartTime
       );
       if (hasOverlap) return Conflict($"Pokój o id {newReservation.RoomId} jest już zarezerwowany w tym terminie");
       
       
       _reservations.Add(newReservation);
   
       return CreatedAtAction(nameof(GetById), new { id = newReservation.Id }, newReservation);
   }
   
   [HttpPut("{id}")]
   public ActionResult<Room> Update(int id, [FromBody] Reservation updatedReservation)
   {
       var reservationToEdit = _reservations.FirstOrDefault(r => r.Id == id);
   
       if (reservationToEdit == null) return NotFound($"Rezerwacja o id {id} nie istnieje");    
       
       reservationToEdit.RoomId = updatedReservation.RoomId;
       reservationToEdit.OrganizerName = updatedReservation.OrganizerName;
       reservationToEdit.Topic = updatedReservation.Topic;
       reservationToEdit.Date = updatedReservation.Date;
       reservationToEdit.StartTime = updatedReservation.StartTime;
       reservationToEdit.EndTime = updatedReservation.EndTime;
       reservationToEdit.Status = updatedReservation.Status;
       
       return Ok($"Rezerwacja {id}:{reservationToEdit.Topic} została zaaktualizowany.");
   }
   
   [HttpDelete("{id}")]
   public ActionResult Delete(int id)
   {
       var room = _reservations.FirstOrDefault(r => r.Id == id);
   
       if (room == null)
       {
           return NotFound($"Rezerwacja o id {id} nie istnieje");
       }
       
       _reservations.Remove(room);
   
       return NoContent();
   }
   
}