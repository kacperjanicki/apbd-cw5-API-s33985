using System.ComponentModel.DataAnnotations;

namespace RoomBackend.Models;

public enum StatusOptions
{
    Planned,
    Confirmed,
    Cancelled
}

public class Reservation
{
    public int Id { get; set; }
    
    [Required]
    public int RoomId { get; set; }
    [Required]
    public string OrganizerName { get; set; }
    [Required]
    public string Topic { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StatusOptions Status { get; set; }  
    
}