using System.ComponentModel.DataAnnotations;

namespace Intrepion.PokemonData.BusinessLogic.Entities;

public class GameConsole
{
    public ApplicationUser? ApplicationUserUpdatedBy { get; set; }
    public Guid Id { get; set; }
    public DateTime UpdateDateTime { get; set; }

    public Company? Company { get; set; }
    public bool IsTest { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string NormalizedName { get; set; } = string.Empty;
    // ActualPropertyPlaceholder
}
