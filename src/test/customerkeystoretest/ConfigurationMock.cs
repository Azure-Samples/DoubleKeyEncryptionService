// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;
    
    public class ConfigurationMock : Microsoft.Extensions.Configuration.IConfiguration
    {
        private Dictionary<string, string> properties = new Dictionary<string, string>();
        public string this[string key]
        { 
            get { return properties.ContainsKey(key) ? properties[key] : null; }
            set { properties[key] = value; }
        }
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new System.NotImplementedException();
        }
        public Microsoft.Extensions.Primitives.IChangeToken GetReloadToken()
        {
            throw new System.NotImplementedException();
        }
        public IConfigurationSection GetSection(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}
