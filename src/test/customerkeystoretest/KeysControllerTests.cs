// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Customerkeystoretest
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.InformationProtection.Web.Controllers;
    using Microsoft.InformationProtection.Web.Models;
    using Xunit;

    public class KeyControllerTests
    {
        [Fact]
        public void GetKey_KeyNotFound_Fail()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(true, "testkey2", "2343", new KeyMock("mod", 0), "type", "alg", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var controller = GetKeysController(keyManager, principal, "keystore", "/key1");

            Assert.IsType<BadRequestObjectResult>(controller.GetKey("testkey1"));
        }

        [Fact]
        public void GetKey_KeyFound_Success()
        {
            ClaimsPrincipal principal = new ClaimsPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity(new List<Claim>(){new Claim(ClaimTypes.Email, "testuser@contoso.com")});
            principal.AddIdentity(identity);

            var keyStore = new KeyStoreMock();
            EmailAuthorizer auth = new EmailAuthorizer();
            keyStore.AddKey(false, "testkey1", "3w4534", new KeyMock("mod1", 54), "type1", "alg1", auth, null);
            keyStore.AddKey(true, "testkey1", "2343", new KeyMock("mod2", 54), "type2", "alg2", auth, null);
            KeyManager keyManager = new KeyManager(keyStore);
            var controller = GetKeysController(keyManager, principal, "keystore", "/key1");

            var result = controller.GetKey("testkey1");
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<KeyData>(((OkObjectResult)result).Value);

            var publicKey = (KeyData)((OkObjectResult)result).Value;
            Assert.Null(publicKey.Cache);
            Assert.Equal("alg2", publicKey.Key.Algorithm);
            Assert.Equal(54u, publicKey.Key.Exponent);
            Assert.Equal("https://keystore/key1/2343", publicKey.Key.KeyId);
            Assert.Equal("type2", publicKey.Key.KeyType);
            Assert.Equal("mod2", publicKey.Key.Modulus);
        }

        [Fact]
        public void Decrypt_KeyNameNotFound_Fail()
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
            var controller = GetKeysController(keyManager, principal, "keystore", "/key1");
            Assert.IsType<BadRequestObjectResult>(controller.Decrypt("testkey1", "324234", encryptedData));
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

            var controller = GetKeysController(keyManager, principal, "keystore", "/key1");
            var result = controller.Decrypt("testkey2", "2343", encryptedData);
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(403, ((StatusCodeResult)result).StatusCode);
        }

        [Fact]
        public void Decrypt_Success()
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
            
            var controller = GetKeysController(keyManager, principal, "keystore", "/key1");
            var result = controller.Decrypt("testkey2", "2343", encryptedData);
            Assert.IsType<OkObjectResult>(result);
            Assert.IsType<DecryptedData>(((OkObjectResult)result).Value);

            var decryptedData = (DecryptedData)((OkObjectResult)result).Value;

            Assert.Equal(encryptedData.Value, decryptedData.Value);  //KeyMock returns the encrypted data as the decrypted data
        }

        private KeysController GetKeysController(KeyManager keyManager, ClaimsPrincipal user, string host, string path)
        {
            var controller = new KeysController(keyManager);

            var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();
            context.Request.Path = path;
            context.Request.Host = new Microsoft.AspNetCore.Http.HostString(host);
            context.Request.Scheme = "https";
            context.Request.IsHttps = true;
            context.User = user;
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = context;

            return controller;
        }
    }
}
