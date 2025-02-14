using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Intrepion.PokemonData.BusinessLogic.Data;
using Intrepion.PokemonData.BusinessLogic.Entities.Records;
using Microsoft.EntityFrameworkCore;

namespace Intrepion.PokemonData.BusinessLogic.Entities.Importers;

public static class ApplicationRoleImporter
{
    public static async Task ImportAsync(
       ApplicationDbContext context,
       string userName, string csvPath
    )
    {
        if (!File.Exists(csvPath))
        {
            Console.WriteLine("File not found: " + csvPath);
            return;
        }

        if (context.Roles is null)
        {
            Console.WriteLine("Database table not found: context.Roles");
            return;
        }

        var normalizedUserName = userName.ToUpperInvariant();
        var applicationUserUpdatedBy = await context.Users.SingleOrDefaultAsync(x => x.NormalizedUserName != null && x.NormalizedUserName.Equals(normalizedUserName));

        if (applicationUserUpdatedBy is null)
        {
            Console.WriteLine("UserName not found: " + userName);
            return;
        }

        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = x => x.Header.ToUpper(CultureInfo.InvariantCulture),
            Delimiter = "|",
        });

        var records = csv.GetRecords<ApplicationRoleRecord>();

        foreach (var record in records)
        {
            if (
                true
            )
            {
                var applicationRole = new ApplicationRole
                {
                    ApplicationUserUpdatedBy = applicationUserUpdatedBy,
                    UpdateDateTime = DateTime.UtcNow,
                    Name = record.Name,
                    NormalizedName = record.Name.ToUpper(CultureInfo.InvariantCulture),
                };

                var dbApplicationRole = await context.Roles.SingleOrDefaultAsync(
                    x => true
                    && x.NormalizedName != null
                    && x.NormalizedName == applicationRole.NormalizedName
                );

                if (dbApplicationRole is null)
                {
                    await context.Roles.AddAsync(applicationRole);
                }
                else
                {
                    dbApplicationRole.ApplicationUserUpdatedBy = applicationUserUpdatedBy;
                    dbApplicationRole.UpdateDateTime = DateTime.UtcNow;
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
