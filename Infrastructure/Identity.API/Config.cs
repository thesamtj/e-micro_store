// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.API
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalogapi"),
                new ApiScope("basketapi"),
                new ApiScope("catalogapi.read"),
                new ApiScope("catalogapi.write"),
                new ApiScope("e-microstoregateway")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                //List of Microservices can go here.
                new ApiResource("Catalog", "Catalog.API")
                {
                    Scopes = {"catalogapi.read", "catalogapi.write"}
                },
                new ApiResource("Basket", "Basket.API")
                {
                    Scopes = {"basketapi"}
                },
                new ApiResource("E-MicroStoreGateway", "E-MicroStore Gateway")
                {
                    Scopes = { "e-microstoregateway", "basketapi"}
                },
                new ApiResource("E-MicroStoreAngular", "E-MicroStore Angular")
                {
                    Scopes = { "e-microstoregateway", "catalogapi.read", "catalogapi.write", "basketapi", "catalogapi.read"}
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                //m2m flow 
                new Client
                {
                    ClientName = "Catalog API Client",
                    ClientId = "CatalogApiClient",
                    ClientSecrets = {new Secret("5c6eb3b4-61a7-4668-ac57-2b4591ec26d2".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "catalogapi.read", "catalogapi.write" }
                },
                new Client
                {
                    ClientName = "Basket API Client",
                    ClientId = "BasketApiClient",
                    ClientSecrets = {new Secret("5c6ec4c5-61a7-4668-ac57-2b4591ec26d2".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"basketapi"}
                },

                new Client
                {
                    ClientName = "E-MicroStore Gateway Client",
                    ClientId = "E-MicroStoreGatewayClient",
                    ClientSecrets = {new Secret("5c7fd5c5-61a7-4668-ac57-2b4591ec26d2".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "e-microstoregateway", "basketapi", "catalogapi.read" }
                },
                new Client
                {
                    ClientName = "Angular-Client",
                    ClientId = "angular-client",
                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = new List<string>
                        {
                            "http://localhost:4200/signin-callback",
                            "http://localhost:4200/assets/silent-callback.html",
                            "https://localhost:9009/signin-oidc"
                        },
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    Enabled = true,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "e-microstoregateway"
                    },
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    RequireClientSecret = false,
                    AllowRememberConsent = false,
                    //PostLogoutRedirectUris = new List<string> {"http://localhost:4200/signout-callback"},
                    RequireConsent = false,
                    AccessTokenLifetime = 3600,
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4200/signout-callback",
                        "https://localhost:9009/signout-callback-oidc"
                    },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("5c6eb3b4-61a7-4668-ac57-2b4591ec26d2".Sha256())
                    }
                }
            };
    }
}