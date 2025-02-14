using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Intrepion.PokemonData.BusinessLogic.Data;
using Intrepion.PokemonData.BusinessLogic.Entities.Records;
using Microsoft.EntityFrameworkCore;

namespace Intrepion.PokemonData.BusinessLogic.Entities.Importers;

public static class ApplicationUserRoleImporter
{
    public static async Task ImportAsync(
       ApplicationDbContext context,
       string userName, string csvPath
    )
    {
        if (!File.Exists(csvPath))
        {
            Console.WriteLine($"ApplicationUserRole CSV file not found: {csvPath}");
            return;
        }

        if (context.Users is null)
        {
            Console.WriteLine("Database table not found: context.Users");
            return;
        }

        if (context.UserRoles is null)
        {
            Console.WriteLine("Database table not found: context.UserRoles");
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

        var records = csv.GetRecords<ApplicationUserRoleRecord>();

        var applicationRoleList = await context.Roles.ToListAsync();
        var applicationUserList = await context.Users.ToListAsync();

        foreach (var record in records)
        {
            var applicationRole = applicationRoleList.SingleOrDefault(x =>
                x.NormalizedName == record.ApplicationRole_NormalizedName
            );
            var applicationUser = applicationUserList.SingleOrDefault(x =>
                x.NormalizedUserName == record.ApplicationUser_NormalizedUserName
            );

            if (
                true
                && applicationRole != null
                && applicationUser != null
            )
            {
                var applicationUserRole = new ApplicationUserRole
                {
                    ApplicationUserUpdatedBy = applicationUserUpdatedBy,
                    UpdateDateTime = DateTime.UtcNow,
                    RoleId = applicationRole.Id,
                    UserId = applicationUser.Id
                };

                var dbApplicationUserRole = await context.UserRoles.SingleOrDefaultAsync(
                    x => true
                    && x.RoleId == applicationRole.Id
                    && x.UserId == applicationUser.Id);

                if (dbApplicationUserRole is null)
                {
                    await context.UserRoles.AddAsync(applicationUserRole);
                }
                else
                {
                    dbApplicationUserRole.ApplicationUserUpdatedBy = applicationUserUpdatedBy;
                    dbApplicationUserRole.UpdateDateTime = DateTime.UtcNow;
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
