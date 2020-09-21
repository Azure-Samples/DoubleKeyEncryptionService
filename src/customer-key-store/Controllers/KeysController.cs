// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
namespace Microsoft.InformationProtection.Web.Controllers
{
    using System;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Mvc;
    using ippw = Microsoft.InformationProtection.Web.Models;
    //https://docs.microsoft.com/azure/active-directory/develop/scenario-protected-web-api-app-configuration
    public class KeysController : Controller
    {
        private readonly ippw.KeyManager keyManager;

        public KeysController(ippw.KeyManager keyManager)
        {
            this.keyManager = keyManager;
        }

        [HttpGet]
        public IActionResult GetKey(string keyName)
        {
            try
            {
                var publicKey = keyManager.GetPublicKey(GetRequestUri(Request), keyName);

                return Ok(publicKey);
            }
            catch(CustomerKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Decrypt(string keyName, string keyId, [FromBody] ippw.EncryptedData encryptedData)
        {
            try
            {
                var decryptedData = keyManager.Decrypt(HttpContext.User, keyName, keyId, encryptedData);

                return Ok(decryptedData);
            }
            catch(CustomerKeyStore.Models.KeyAccessException)
            {
                return StatusCode(403);
            }
            catch(ArgumentException e)
            {
                return BadRequest(e);
            }
        }

        private static Uri GetRequestUri(AspNetCore.Http.HttpRequest request)
        {
            return new Uri(request.GetDisplayUrl());
        }
    }
}
