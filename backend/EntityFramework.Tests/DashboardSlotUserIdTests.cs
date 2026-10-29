using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class DashboardSlotUserIdTests
{
    [Test]
    public async Task EfCrud_PersistsAndReturnsUserId_Unchanged()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var factory = new TestDbContextFactory(options);

        ICrud<DashboardSlot> crud = new EfCrud<DashboardSlot>(factory);
        await crud.CreateAsync(new DashboardSlot
        {
            LayoutFormat = "3x2",
            SlotIndex = 0,
            WidgetType = "tasks",
            UserId = "user-1"
        });

        ICrud<DashboardSlot> readCrud = new EfCrud<DashboardSlot>(factory);
        var stored = await readCrud.GetByQueryAsync(s => s.LayoutFormat == "3x2" && s.SlotIndex == 0);

        Assert.That(stored, Is.Not.Null);
        Assert.That(stored!.UserId, Is.EqualTo("user-1"));
    }
}
