using Common.Models;

namespace InMemory.Tests;

[TestFixture]
public class DashboardSlotUserIdTests
{
    [Test]
    public async Task InMemoryCrud_PersistsAndReturnsUserId_Unchanged()
    {
        var crud = new InMemoryGenericCrud<DashboardSlot>();
        var created = await crud.CreateAsync(new DashboardSlot
        {
            LayoutFormat = "3x2",
            SlotIndex = 0,
            WidgetType = "tasks",
            UserId = "user-1"
        });

        var stored = await crud.GetByIdAsync(created.Id);

        Assert.That(stored, Is.Not.Null);
        Assert.That(stored!.UserId, Is.EqualTo("user-1"));
    }
}
