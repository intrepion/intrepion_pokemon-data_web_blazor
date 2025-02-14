using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Intrepion.PokemonData.BusinessLogic.Data;
using Intrepion.PokemonData.BusinessLogic.Entities.Records;
using Microsoft.EntityFrameworkCore;

namespace Intrepion.PokemonData.BusinessLogic.Entities.Importers;

public static class GameConsoleImporter
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

        if (context.GameConsoles is null)
        {
            Console.WriteLine("Database table not found: context.GameConsoles");
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

        var records = csv.GetRecords<GameConsoleRecord>();

        var companyList = await context.Companies.ToListAsync();
        // EntityListCodePlaceholder

        foreach (var record in records)
        {
            var company = companyList.FirstOrDefault(x =>
                true
                && x.NormalizedName.Equals(record.Company_NormalizedName)
            );

            // ManyToOneCodePlaceholder

            if (true
                // NullCheckCodePlaceholder
            )
            {
                var gameConsole = new GameConsole
                {
                    ApplicationUserUpdatedBy = applicationUserUpdatedBy,
                    UpdateDateTime = DateTime.UtcNow,

                    Company = company,
                    IsTest = record.IsTest,
                    Name = record.Name,
                    NormalizedName = record.Name.ToUpper(CultureInfo.InvariantCulture),
                    // NewEntityCodePlaceholder
                };

                var dbGameConsole = await context.GameConsoles.SingleOrDefaultAsync(
                    x => true
                    && x.NormalizedName.Equals(gameConsole.NormalizedName)
                    // CompositeKeyCodePlaceholder
                );

                if (dbGameConsole is null)
                {
                    await context.GameConsoles.AddAsync(gameConsole);
                }
                else
                {
                    dbGameConsole.ApplicationUserUpdatedBy = applicationUserUpdatedBy;
                    dbGameConsole.UpdateDateTime = DateTime.UtcNow;

                    dbGameConsole.Company = company;
                    dbGameConsole.IsTest = record.IsTest;
                    dbGameConsole.Name = record.Name;
                    // ExistingEntityCodePlaceholder
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
