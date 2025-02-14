using Bogus;
using Intrepion.PokemonData.BusinessLogic.Entities;

namespace Intrepion.PokemonData.BusinessLogic.Data;

public static class FakeData
{
    public static async Task SeedAsync(ApplicationDbContext applicationDbContext, ApplicationUser adminUser)
    {
        Randomizer.Seed = new Random(8675309);

        var applicationRoleFakes = new Faker<ApplicationRole>()
            .RuleFor(x => x.ApplicationUserUpdatedBy, f => adminUser)
            .RuleFor(x => x.Name, f =>
                f.Hacker.Adjective()
                    + " " + f.Hacker.Noun())
            .RuleFor(x => x.NormalizedName, (f, x) => x.Name?.ToUpperInvariant());
        var applicationRoles = applicationRoleFakes.Generate(8);
        applicationDbContext.Roles.AddRange(applicationRoles);

        var applicationUserFakes = new Faker<ApplicationUser>()
            .RuleFor(x => x.ApplicationUserUpdatedBy, f => adminUser)
            .RuleFor(x => x.Email, f => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
            .RuleFor(x => x.NormalizedEmail, (f, x) => x.Email?.ToUpperInvariant())
            .RuleFor(x => x.NormalizedUserName, (f, x) => x.NormalizedEmail)
            .RuleFor(x => x.UserName, (f, x) => x.Email);
        var applicationUsers = applicationUserFakes.Generate(16);
        applicationDbContext.Users.AddRange(applicationUsers);

        var applicationUserRoleFakes = new Faker<ApplicationUserRole>()
            .RuleFor(x => x.ApplicationUserUpdatedBy, f => adminUser)
            .RuleFor(x => x.ApplicationRole, f => f.PickRandom(applicationRoles))
            .RuleFor(x => x.ApplicationUser, f => f.PickRandom(applicationUsers));
        var applicationUserRoles = applicationUserRoleFakes.Generate(4);
        applicationDbContext.UserRoles.AddRange(applicationUserRoles);

        // FakeEntityPlaceholder

        await applicationDbContext.SaveChangesAsync();
    }
}
