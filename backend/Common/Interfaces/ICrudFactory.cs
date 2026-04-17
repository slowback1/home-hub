namespace Common.Interfaces;

public interface ICrudFactory
{
    ICrud<T> GetCrud<T>() where T : class, IIdentifyable;
}