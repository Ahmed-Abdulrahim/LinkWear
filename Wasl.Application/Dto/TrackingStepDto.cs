namespace Wasl.Application.Dto;

public class TrackingStepDto
{
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsCurrent { get; set; }
    public DateTime? Date { get; set; }
}
