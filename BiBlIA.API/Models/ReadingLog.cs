namespace BíblIA.Api.Models;

/// <summary>Registro de um dia de leitura concluído por um usuário em um plano.</summary>
public class ReadingLog
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PlanId { get; set; }
    public ReadingPlan Plan { get; set; } = null!;

    public int DayNumber { get; set; }
    public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
}
