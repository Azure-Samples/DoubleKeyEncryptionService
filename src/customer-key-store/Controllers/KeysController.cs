using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;

//https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-protected-web-api-app-configuration

namespace Microsoft.InformationProtection.Web.Controllers
{
    using ippw = Microsoft.InformationProtection.Web.Models;
    public class KeysController : Controller
    {
        private readonly ippw.KeyManager mKeyManager;

        private Uri GetRequestUri(AspNetCore.Http.HttpRequest request)
        {
            return new Uri(request.GetDisplayUrl());
        }

        public KeysController(ippw.KeyManager keyManager)
        {
            mKeyManager = keyManager;
        }
        
        [HttpGet]
        public IActionResult Key(string keyName)
        {
            try
            {                
                var publicKey = mKeyManager.GetPublicKey(GetRequestUri(Request), keyName);

                return Json(publicKey);
            }
            catch(Exception e)
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
                var decryptedData = mKeyManager.Decrypt(HttpContext.User, keyName, keyId, encryptedData);

                return Json(decryptedData);
            }
            catch(Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
