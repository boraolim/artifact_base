namespace Utilities.Core.Shared.Services;

public interface IEnvironmentReader
{
    Result<string> GetVariable(string keyValue);
    Result<string> GetVariable(string keyValue, string defaultValue);
}
