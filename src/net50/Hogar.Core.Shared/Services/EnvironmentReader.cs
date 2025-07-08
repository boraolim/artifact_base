using System;

using MessageConstantsCore = Bankaool.Core.Shared.Constants.MessageConstants;
using EnvironmentConstantsCore = Bankaool.Core.Shared.Constants.EnvironmentConstants;

namespace Bankaool.Core.Shared.Services
{
    public sealed class EnvironmentReader : IEnvironmentReader
    {
        public Result<string> GetVariable(string keyValue)
        {
            var valueEnv = IsDefaultOS() ?
                        Environment.GetEnvironmentVariable(keyValue, EnvironmentVariableTarget.Machine) :
                        Environment.GetEnvironmentVariable(keyValue);

            if(string.IsNullOrEmpty(valueEnv))
                return Result<string>.Failure(string.Format(MessageConstantsCore.MSG_ENVIRONMENT_VAR_NOT_FOUND, keyValue));

            return Result<string>.Success(valueEnv);
        }
        public Result<string> GetVariable(string keyValue, string defaultValue)
        {
            return Result<string>.TryCatch(() =>
            {
                return IsDefaultOS() ?
                    Environment.GetEnvironmentVariable(keyValue, EnvironmentVariableTarget.Machine) :
                    Environment.GetEnvironmentVariable(keyValue);
            }, string.Format(MessageConstantsCore.MSG_ENVIRONMENT_VAR_NOT_FOUND, keyValue));
        }

        private static bool IsDefaultOS() =>
            Environment.OSVersion.Platform.ToString() == EnvironmentConstantsCore.ENV_DEFAULT_OS;
    }
}
