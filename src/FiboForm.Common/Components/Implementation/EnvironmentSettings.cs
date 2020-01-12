using FiboForm.Common.Components.Definition;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiboForm.Common.Components.Implementation
{
    public class EnvironmentSettings : IEnvironmentSettings
    {

        private readonly ILogger<EnvironmentSettings> _logger;

        public EnvironmentSettings(ILogger<EnvironmentSettings> logger)
        {
            _logger = logger;
        }

        public string GetEnvironmentVariable(string key, string defaultValue = null)
        {
            try
            {
                var value = System.Environment.GetEnvironmentVariable(key);

                if (!string.IsNullOrEmpty(value))
                {
                    _logger.LogWarning($"FOUND - Env Variable: {key}, value: {value}");
                    return value;
                }
                else
                {
                    if (string.IsNullOrEmpty(defaultValue))
                    {
                        _logger.LogError($"NOT FOUND Env Variable: {key}");
                        throw new KeyNotFoundException($"NOT FOUND Env Variable: {key}");
                    }
                    else 
                    {
                        _logger.LogError($"NOT FOUND Env Variable: {key}, use default falue {defaultValue ?? "null"}");
                        return defaultValue;
                    }
                }
                    
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(defaultValue))
                {
                    _logger.LogError($"NOT FOUND Env Variable: {key}");
                    throw new KeyNotFoundException($"NOT FOUND Env Variable: {key}");
                }
                else
                {
                    _logger.LogError($"NOT FOUND Env Variable: {key}, use default falue {defaultValue ?? "null"}");
                    return defaultValue;
                }
            }
        }
    }
}
