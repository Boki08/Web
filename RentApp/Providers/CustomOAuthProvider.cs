﻿using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.OAuth;
using Owin;
using RentApp.Models.Entities;
using RentApp.Persistance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
namespace RentApp.Providers
{
    public class CustomOAuthProvider : Microsoft.Owin.Security.OAuth.OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            var allowedOrigin = "*";

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            ApplicationUserManager userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            RAIdentityUser user = await userManager.FindAsync(context.UserName, context.Password);
            
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.!!!!");
                return;
            }

            RADBContext db = new RADBContext();

           

            
            string fullName=db.AppUsers.SingleOrDefault(r => r.UserId == user.AppUserId).FullName;
           

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, "JWT");
            oAuthIdentity.AddClaim(new Claim("UserFullName", fullName));
            var ticket = new AuthenticationTicket(oAuthIdentity, null);
            
            context.Validated(ticket);

        }
    }
}