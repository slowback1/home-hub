using Common.Interfaces;

namespace TestUtilities.CrudImplementations;

public class TestGenericCrud<T> : TestCrud<T> where T : class, IIdentifyable;