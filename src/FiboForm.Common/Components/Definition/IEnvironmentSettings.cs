using System;
using System.Collections.Generic;
using System.Text;

namespace FiboForm.Common.Components.Definition
{
    public interface IEnvironmentSettings
    {
        string GetEnvironmentVariable(string key, string defaultValue = null);
    }
}
