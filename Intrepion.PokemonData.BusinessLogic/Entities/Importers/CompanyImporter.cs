using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Intrepion.PokemonData.BusinessLogic.Data;
using Intrepion.PokemonData.BusinessLogic.Entities.Records;
using Microsoft.EntityFrameworkCore;

namespace Intrepion.PokemonData.BusinessLogic.Entities.Importers;

public static class CompanyImporter
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

        if (context.Companies is null)
        {
            Console.WriteLine("Database table not found: context.Companies");
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

        var records = csv.GetRecords<CompanyRecord>();

        // EntityListCodePlaceholder

        foreach (var record in records)
        {
            // ManyToOneCodePlaceholder

            if (true
                // NullCheckCodePlaceholder
            )
            {
                var company = new Company
                {
                    ApplicationUserUpdatedBy = applicationUserUpdatedBy,
                    UpdateDateTime = DateTime.UtcNow,

                    IsTest = record.IsTest,
                    Name = record.Name,
                    NormalizedName = record.Name.ToUpper(CultureInfo.InvariantCulture),
                    // NewEntityCodePlaceholder
                };

                var dbCompany = await context.Companies.SingleOrDefaultAsync(
                    x => true
                    && x.NormalizedName.Equals(company.NormalizedName)
                    // CompositeKeyCodePlaceholder
                );

                if (dbCompany is null)
                {
                    await context.Companies.AddAsync(company);
                }
                else
                {
                    dbCompany.ApplicationUserUpdatedBy = applicationUserUpdatedBy;
                    dbCompany.UpdateDateTime = DateTime.UtcNow;

                    dbCompany.IsTest = record.IsTest;
                    dbCompany.Name = record.Name;
                    // ExistingEntityCodePlaceholder
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
