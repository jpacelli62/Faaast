﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Faaast.Tests.Authentication.Utility;
using Xunit;

namespace Faaast.Tests.Authentication.ServerTests
{
    public class ChallengeAuthorizationCodeFlowTests : IClassFixture<ServerFixture>
    {
        public ServerFixture Fixture { get; set; }

        public CustomTestServer Server { get; set; }

        public ChallengeAuthorizationCodeFlowTests(ServerFixture fixture)
        {
            this.Fixture = fixture;
            this.Server = fixture.CreateServer(builder => builder.AddAuthorizationCodeGrantFlow());
        }

        private async Task<Transaction> QueryAsync(CustomTestServer server, string clientId, string scopes, string redirectUri, bool state, Action<HttpRequestMessage> configure = null)
        {
            var dic = new Dictionary<string, string>
            {
                {"client_id", clientId},
                {"scope", scopes },
                {"response_type", "code" },
                {"redirect_uri", redirectUri ?? $"https://{this.Fixture.ClientHost}/faaastoauth/signin" },
                { "state", state ? "CfDJ8Np1eFHOzoVJni6nVfHZpxxtPlOOOHr8csuyU7jfcKYoseFfn7kHq_e1yKbTVbDqvDoMNPIaoB0emAX8DhXQc7eOyIzHsYZYxwcsDLzcQIdrAMVre16lL2ni2c2F7s_6lY2p136sPyBtUi503YOndrnaKp6j3rlb" : string.Empty}
            };
            return await server.SendGetAsync(this.Fixture.AuthorizeEndpoint, dic, configure);
        }

        private static HttpRequestMessage Authenticated(HttpRequestMessage req)
        {
            req.Headers.Add("fakeLoggedUser", "1");
            return req;
        }

        private static HttpRequestMessage DisabledFlow(HttpRequestMessage req)
        {
            req.Headers.Add("IsAllowedFlow", "0");
            return req;
        }

        private static HttpRequestMessage InvalidRedirectUri(HttpRequestMessage req)
        {
            req.Headers.Add("IsAllowedRedirectUrl", "0");
            return req;
        }

        [Fact]
        public async Task Test_should_not_handle()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, this.Fixture.TokenEndpoint);
            var transaction = await this.Server.SendAsync(request);
            Assert.Equal(HttpStatusCode.NotFound, transaction.Response.StatusCode);
        }

        [Fact]
        public async Task Test_invalid_client()
        {
            var transaction = await this.QueryAsync(this.Server,
                "wrongid",
                this.Fixture.Client.Scope,
                redirectUri: null,
                state: true);
            Assert.Equal(HttpStatusCode.BadRequest, transaction.Response.StatusCode);
            Assert.Equal(Faaast.OAuth2Server.Resources.Msg_InvalidClient, transaction.ResponseText);
        }

        [Fact]
        public async Task Test_forbidden_flow()
        {
            var transaction = await this.QueryAsync(this.Server,
                this.Fixture.Client.ClientId,
                this.Fixture.Client.Scope,
                redirectUri: null,
                state: true,
                req => Authenticated(DisabledFlow(Authenticated(req))));
            Assert.Equal(HttpStatusCode.BadRequest, transaction.Response.StatusCode);
            Assert.Equal(Faaast.OAuth2Server.Resources.Msg_ForbiddenFlow, transaction.ResponseText);
        }

        [Fact]
        public async Task Test_invalid_redirecturl()
        {
            var transaction = await this.QueryAsync(
                this.Server,
                this.Fixture.Client.ClientId,
                this.Fixture.Client.Scope,
                "https://donotredirect.com/",
                true,
                 req => Authenticated(InvalidRedirectUri(req)));
            Assert.Equal(HttpStatusCode.BadRequest, transaction.Response.StatusCode);
            Assert.Equal(Faaast.OAuth2Server.Resources.Msg_InvalidRedirectUri, transaction.ResponseText);
        }

        [Fact]
        public async Task Test_notAuthenticated_redirectToLogin()
        {
            var transaction = await this.QueryAsync(
                this.Server,
                this.Fixture.Client.ClientId,
                this.Fixture.Client.Scope,
                redirectUri: null,
                state: true);

            Assert.Equal(HttpStatusCode.Redirect, transaction.Response.StatusCode);
            var redirectPath = transaction.Response.Headers.Location.OriginalString;
            var uri = new UriBuilder(redirectPath);
            Assert.Equal(this.Fixture.Options.Issuer, uri.Host);
            Assert.Equal("/login", uri.Path);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            Assert.Single(queryDictionary);
            Assert.Equal(
                $"https://sso.mycompany.com/oauth/authorize?client_id=MyClientId&scope=identity&response_type=code&redirect_uri=https://{this.Fixture.ClientHost}/faaastoauth/signin&state=CfDJ8Np1eFHOzoVJni6nVfHZpxxtPlOOOHr8csuyU7jfcKYoseFfn7kHq_e1yKbTVbDqvDoMNPIaoB0emAX8DhXQc7eOyIzHsYZYxwcsDLzcQIdrAMVre16lL2ni2c2F7s_6lY2p136sPyBtUi503YOndrnaKp6j3rlb",
                System.Web.HttpUtility.UrlDecode(queryDictionary["returnUrl"]));
        }

        [Fact]
        public async Task Test_invalidScope()
        {
            var server = this.Fixture.CreateServer(builder => builder.AddAuthorizationCodeGrantFlow(), options => { });
            var transaction = await this.QueryAsync(
                server, 
                this.Fixture.Client.ClientId,
                "wrongScope",
                redirectUri: null,
                state: true,
                 req => Authenticated(req));

            Assert.Equal(HttpStatusCode.BadRequest, transaction.Response.StatusCode);
            Assert.Equal(Faaast.OAuth2Server.Resources.Msg_InvalidScope, transaction.ResponseText);
        }

        [Fact]
        public async Task Test_nominal()
        {
            var fixture = new ServerFixture();
            var server = fixture.CreateServer(builder => builder.AddAuthorizationCodeGrantFlow(), options => { });
            var transaction = await this.QueryAsync(
                server,
                fixture.Client.ClientId,
                fixture.Client.Scope,
                redirectUri: null,
                state: true,
                req => Authenticated(req));

            Assert.Equal(HttpStatusCode.Redirect, transaction.Response.StatusCode);
            var redirectPath = transaction.Response.Headers.Location.OriginalString;
            var uri = new UriBuilder(redirectPath);
            Assert.Equal(fixture.ClientHost, uri.Host);
            Assert.Equal("/faaastoauth/signin", uri.Path);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            Assert.False(string.IsNullOrWhiteSpace(System.Web.HttpUtility.UrlDecode(queryDictionary["code"])));
            Assert.False(string.IsNullOrWhiteSpace(System.Web.HttpUtility.UrlDecode(queryDictionary["state"])));
            Assert.NotNull(fixture.Code);
        }

        [Fact]
        public async Task Test_nominal_withoutState()
        {
            var fixture = new ServerFixture();
            var server = fixture.CreateServer(builder => builder.AddAuthorizationCodeGrantFlow(), options => { });

            fixture.Code = null;
            var transaction = await this.QueryAsync(
                server,
                fixture.Client.ClientId,
                fixture.Client.Scope,
                redirectUri: null,
                state: false,
                req => Authenticated(req));

            Assert.Equal(HttpStatusCode.Redirect, transaction.Response.StatusCode);
            var redirectPath = transaction.Response.Headers.Location.OriginalString;
            var uri = new UriBuilder(redirectPath);
            Assert.Equal(fixture.ClientHost, uri.Host);
            Assert.Equal("/faaastoauth/signin", uri.Path);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);
            Assert.False(string.IsNullOrWhiteSpace(System.Web.HttpUtility.UrlDecode(queryDictionary["code"])));
            Assert.NotNull(System.Web.HttpUtility.UrlDecode(queryDictionary["state"]));
            Assert.NotNull(fixture.Code);
        }

        [Fact]
        public void Empty_authorizationEndpoint_throws_exception() => Assert.Throws<ArgumentException>(() => this.Fixture.CreateServer(builder => builder.AddAuthorizationCodeGrantFlow(), options => options.AuthorizeEndpointPath = null));
    }
}
