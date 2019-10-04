using System;

namespace Microsoft.InformationProtection.Web.Models
{
    public interface Key
    {
        PublicKey GetPublicKey();
        byte[] Decrypt(byte[] encryptedData);
    }
}