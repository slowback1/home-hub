using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface ISystemConfigProvider
{
    Task<SystemConfig> GetAsync(string @namespace, string key);
    Task<SystemConfig> GetSecretAsync(string @namespace, string key);
    Task<IEnumerable<SystemConfig>> GetAllAsync();
    Task<SystemConfig> UpdateAsync(string @namespace, string key, string value);
}
