using Bogus;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace Intrepion.PokemonData.AcceptanceTests.Admin;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public partial class GameConsoleAdminPages : PageTest
{
    [Test]
    public async Task MainNavigation()
    {
        var faker = new Faker();
        var aRandomString = faker.Random.String2(10);
        var someRandomString = faker.Random.String2(10);
        await Page.GetByTestId("gameConsoleNavLink").ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console Home");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create New" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console Add");

        await Page.GetByLabel("Is Test?:", new() { Exact = true }).CheckAsync();
        await Page.GetByLabel("Name:", new() { Exact = true }).FillAsync("aName" + aRandomString);
        // CreatePropertyCodePlaceholder

        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console View");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Edit" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console Edit");

        await Page.GetByLabel("Is Test?:", new() { Exact = true }).CheckAsync();
        await Page.GetByLabel("Name:", new() { Exact = true }).FillAsync("someName" + someRandomString);
        // ModifyPropertyCodePlaceholder

        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console View");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Confirm" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Back to Grid" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Game Console Home");
    }

    [SetUp]
    public async Task SetUp()
    {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL");
        if (string.IsNullOrEmpty(baseUrl))
        {
            baseUrl = "http://localhost:5074";
        }
        await Page.GotoAsync(baseUrl);

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Log in");
        await Page.GetByTestId("loginEmail").FillAsync("Admin1@Intrepion.PokemonData.com");
        await Page.GetByTestId("loginPassword").FillAsync("Admin1@Intrepion.PokemonData.com");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
    }
}
