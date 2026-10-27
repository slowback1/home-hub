using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class DashboardSlotUserIdTests
{
    private AppDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task EfCrud_PersistsAndReturnsUserId_Unchanged()
    {
        var databaseName = Guid.NewGuid().ToString();

        using (var writeCtx = CreateContext(databaseName))
        {
            ICrud<DashboardSlot> crud = new EfCrud<DashboardSlot>(writeCtx);
            await crud.CreateAsync(new DashboardSlot
            {
                LayoutFormat = "3x2",
                SlotIndex = 0,
                WidgetType = "tasks",
                UserId = "user-1"
            });
        }

        using var readCtx = CreateContext(databaseName);
        ICrud<DashboardSlot> readCrud = new EfCrud<DashboardSlot>(readCtx);
        var stored = await readCrud.GetByQueryAsync(s => s.LayoutFormat == "3x2" && s.SlotIndex == 0);

        Assert.That(stored, Is.Not.Null);
        Assert.That(stored!.UserId, Is.EqualTo("user-1"));
    }
}
