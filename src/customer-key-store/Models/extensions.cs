// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models.Extensions
  {
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull<T>(this T argument, string name)
        {
            if(argument == null)
            {
                throw new System.ArgumentNullException(name);
            }
        }
    }
}