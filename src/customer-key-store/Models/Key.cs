// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Models
{
    public interface IKey
    {
        PublicKey GetPublicKey();
        byte[] Decrypt(byte[] encryptedData);
    }
}