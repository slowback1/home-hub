using Common.Interfaces;

namespace InMemory;

public class InMemoryCrudFactory : ICrudFactory
{
    public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
    {
        return new InMemoryGenericCrud<T>();
    }
}