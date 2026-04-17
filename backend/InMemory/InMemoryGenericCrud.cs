using Common.Interfaces;

namespace InMemory;

public class InMemoryGenericCrud<T> : InMemoryCrud<T> where T : class, IIdentifyable;