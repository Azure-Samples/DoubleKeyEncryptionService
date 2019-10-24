using System;

namespace Microsoft.InformationProtection.Web.Models
{
    public interface IKey
    {
        PublicKey GetPublicKey();
        byte[] Decrypt(byte[] encryptedData);
    }
}