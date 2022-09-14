// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.InformationProtection.Web.Models;
    using Xunit;
    public class KeyManagerTests
    {
        [Fact]
        public void GetPublicKey_NoKeys_Fail()
        {
            var keyStore = new KeyStoreMock();
            KeyManager keyManager = new KeyManager(keyStore);
            Assert.Throws<System.ArgumentException>(() => keyManager.GetPublicKey(new Uri("https://keystore/key1"), "testkey1"));
        }

        [Fact]
        public void GetPublicKey_KeyNotFound_Fail()
        {
            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            Assert.Throws<System.ArgumentException>(() => keyManager.GetPublicKey(new Uri("https://keystore/key1"), "testkey1"));
        }

        [Fact]
        public void GetPublicKey_KeyFound_NoCache_Success()
        {
            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(false, "testkey1", "3w4534", new KeyMock("mod1", 54), "type1", "alg1", auth, null);
            keyStore.AddKey(true, "testkey1", "2343", new KeyMock("mod2", 54), "type2", "alg2", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var publicKey = keyManager.GetPublicKey(new Uri("https://keystore/key1"), "testkey1");
            Assert.Null(publicKey.Cache);
            Assert.Equal("alg2", publicKey.Key.Algorithm);
            Assert.Equal(54u, publicKey.Key.Exponent);
            Assert.Equal("https://keystore/key1/2343", publicKey.Key.KeyId);
            Assert.Equal("type2", publicKey.Key.KeyType);
            Assert.Equal("mod2", publicKey.Key.Modulus);
        }

        [Fact]
        public void GetPublicKey_KeyFound_WithCache_Success()
        {
            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(false, "testkey1", "3w4534", new KeyMock("mod1", 54), "type1", "alg1", auth, 30);
            keyStore.AddKey(true, "testkey1", "2343", new KeyMock("mod2", 54), "type2", "alg2", auth, 30);
            KeyManager keyManager = new KeyManager(keyStore);
            var publicKey = keyManager.GetPublicKey(new Uri("https://keystore/key1"), "testkey1");
            Assert.NotNull(publicKey.Cache);
            Assert.InRange((DateTime.Parse(publicKey.Cache.Expiration) - DateTime.UtcNow).TotalHours, 30 * 23, 30 * 24);  //It should be 30 days but by the time it gets here it will be a little less
            Assert.Equal("alg2", publicKey.Key.Algorithm);
            Assert.Equal(54u, publicKey.Key.Exponent);
            Assert.Equal("https://keystore/key1/2343", publicKey.Key.KeyId);
            Assert.Equal("type2", publicKey.Key.KeyType);
            Assert.Equal("mod2", publicKey.Key.Modulus);
        }

        public void Decrypt_NoKeys_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            var keyStore = new KeyStoreMock();
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            Assert.Throws<System.ArgumentException>(() => keyManager.Decrypt(principal, "testkey1", "324234", encryptedData));
        }

        [Fact]
        public void Decrypt_KeyNameNotFound_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            Assert.Throws<System.ArgumentException>(() => keyManager.Decrypt(principal, "testkey1", "324234", encryptedData));
        }

        [Fact]
        public void Decrypt_KeyIdNotFound_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            Assert.Throws<System.ArgumentException>(() => keyManager.Decrypt(principal, "testkey2", "324234", encryptedData));
        }

        [Fact]
        public void Decrypt_NoAccessToKey_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            Assert.Throws<CustomerKeyStore.Models.KeyAccessException>(() => keyManager.Decrypt(principal, "testkey2", "2343", encryptedData));
        }

        [Fact]
        public void Decrypt_BadAlgorithm_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");

            var keyStore = new KeyStoreMock();

            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-26";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            Assert.Throws<System.ArgumentException>(() => keyManager.Decrypt(principal, "testkey2", "2343", encryptedData));
        }

        [Fact]
        public void Decrypt_SingleActiveKey_Success()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");

            var keyStore = new KeyStoreMock();

            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            var decryptedData = keyManager.Decrypt(principal, "testkey2", "2343", encryptedData);
            Assert.Equal(encryptedData.Value, decryptedData.Value);  //KeyMock returns the encrypted data as the decrypted data
        }

        [Fact]
        public void Decrypt_OldActiveKey_Success()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");

            var keyStore = new KeyStoreMock();

            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            keyStore.AddKey(false, "testkey2", "75466", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            var decryptedData = keyManager.Decrypt(principal, "testkey2", "75466", encryptedData);
            Assert.Equal(encryptedData.Value, decryptedData.Value);  //KeyMock returns the encrypted data as the decrypted data
        }

        [Fact]
        public void Decrypt_CurrentActiveKey_Success()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            EmailAuthorizer auth = new EmailAuthorizer();
            auth.AddEmail("testuser@contoso.com");

            var keyStore = new KeyStoreMock();

            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            keyStore.AddKey(false, "testkey2", "75466", new KeyMock("mod", 0), "type", "alg", auth, null);
            keyStore.AddKey(false, "testkey1", "654345", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var encryptedData = new EncryptedData();
            encryptedData.Algorithm = "RSA-OAEP-256";
            encryptedData.Value = Convert.ToBase64String(new byte[] {0x1});
            var decryptedData = keyManager.Decrypt(principal, "testkey2", "2343", encryptedData);
            Assert.Equal(encryptedData.Value, decryptedData.Value);  //KeyMock returns the encrypted data as the decrypted data
        }
    }
}
