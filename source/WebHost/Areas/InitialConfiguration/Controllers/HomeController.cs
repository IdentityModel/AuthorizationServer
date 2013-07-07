/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see license.txt
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Thinktecture.AuthorizationServer.Configuration;
using Thinktecture.AuthorizationServer.Interfaces;
using Thinktecture.AuthorizationServer.Models;
using Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration.Models;

namespace Thinktecture.AuthorizationServer.WebHost.Areas.InitialConfiguration.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        IAuthorizationServerAdministration authorizationServerAdministration;
        public HomeController(IAuthorizationServerAdministration authorizationServerAdministration)
        {
            this.authorizationServerAdministration = authorizationServerAdministration;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (authorizationServerAdministration.GlobalConfiguration != null)
            {
                filterContext.Result = new RedirectResult("~");
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsKeyConfigEmpty)
            {
                ViewData["EmptyKeys"] = true;
            }
        }

        public ActionResult Index()
        {
            if (authorizationServerAdministration.GlobalConfiguration != null)
            {
                return Redirect("~/");
            }

            return View("Index");
        }

        public bool IsKeyConfigEmpty
        {
            get
            {
                return String.IsNullOrWhiteSpace(SymmetricProtectionKeysConfigurationSection.Instance.Confidentiality) &&
                    String.IsNullOrWhiteSpace(SymmetricProtectionKeysConfigurationSection.Instance.Integrity);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InitialConfigurationModel model)
        {
            if (authorizationServerAdministration.GlobalConfiguration != null)
            {
                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {
                if (IsKeyConfigEmpty)
                {
                    GenerateNewSymmetricProtectionKeysConfigurationSection();
                }

                var global = new GlobalConfiguration()
                {
                    AuthorizationServerName = model.Name,
                    Issuer = model.Issuer,
                    Administrators = new List<AuthorizationServerAdministrator>
                    {
                        new AuthorizationServerAdministrator{NameID = model.Admin}
                    }
                };
                authorizationServerAdministration.GlobalConfiguration = global;
                authorizationServerAdministration.SaveChanges();

                if (model.Test == "test")
                {
                    TestData.Populate();
                }

                return View("Success");
            }

            return View("Index");
        }

        const string configTemplate = "<symmetricProtectionKeys confidentiality=\"{0}\" integrity=\"{1}\" />";

        private void GenerateNewSymmetricProtectionKeysConfigurationSection()
        {
            var protectionKeyBytes = IdentityModel.CryptoRandom.CreateRandomKey(32);
            var protectionKeyString = protectionKeyBytes.Select(x => x.ToString("X2")).Aggregate((x, y) => x + y);

            var integrityKeyString = "";

            var fileContents = String.Format(configTemplate, protectionKeyString, integrityKeyString);
            System.IO.File.WriteAllText(Server.MapPath("~/App_Data/symmetricProtectionKeys.config"), fileContents);
            
            DataProtectection.Instance = new LocalKeyProtection(protectionKeyString);
        }
    }
}
