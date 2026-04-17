using Common.Interfaces;

namespace FileData;

public class FileGenericCrud<T>(string dataDirectory = "data") : FileCrud<T>(dataDirectory) where T : class, IIdentifyable;