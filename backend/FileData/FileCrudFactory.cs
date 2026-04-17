using Common.Interfaces;

namespace FileData;

public class FileCrudFactory(string dataDirectory = "data") : ICrudFactory
{
    public ICrud<T> GetCrud<T>() where T : class, IIdentifyable
    {
        return new FileGenericCrud<T>(dataDirectory);
    }
}