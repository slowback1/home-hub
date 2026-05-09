using System.Threading.Tasks;

namespace Common.Interfaces;

public interface IOllamaClient
{
    Task<string> ChatAsync(string model, string prompt);
}
