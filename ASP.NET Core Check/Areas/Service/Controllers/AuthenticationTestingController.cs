using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    [Authorize]
    public class AuthenticationTestingController : Controller
    {
        private readonly IAuthenticationHandlerProvider _handlers;
        private readonly IAuthenticationSchemeProvider _schemes;
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _options;
        private readonly ISystemClock _clock;
        private bool _shouldRefresh;
        private bool _signInCalled;
        private bool _signOutCalled;

        private DateTimeOffset? _refreshIssuedUtc;
        private DateTimeOffset? _refreshExpiresUtc;
        private string? _sessionKey;
        private Task<AuthenticateResult>? _readCookieTask;
        private AuthenticationTicket? _refreshTicket;


        public AuthenticationTestingController(IAuthenticationSchemeProvider schemes,
            IAuthenticationHandlerProvider handlers,
            IOptionsMonitor<CookieAuthenticationOptions> options,
            ISystemClock clock)
        {
            _schemes = schemes;
            _handlers = handlers;
            _options = options;
            _clock = clock;
        }


        public async Task<IActionResult> Index()
        {
            foreach (var scheme in await _schemes.GetRequestHandlerSchemesAsync())
            {
                var handler1 = await _handlers.GetHandlerAsync(HttpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler1 != null && await handler1.HandleRequestAsync())
                {
                    return View();
                }
            }
            var target = ResolveTarget(_options.CurrentValue.ForwardAuthenticate);

            var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await HttpContext.AuthenticateAsync(defaultAuthenticate.Name);
                if (result?.Principal != null)
                {
                    HttpContext.User = result.Principal;
                }
            }

            string cookieValue = HttpContext.Request.Cookies[".Aspnetcore.Identity.Application"];

            var provider = _options.CurrentValue.DataProtectionProvider;
            //Get a data protector to use with either approach
            IDataProtector dataProtector = provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "Identity.Application", "v2");

            //Get the decrypted cookie as plain text
            UTF8Encoding specialUtf8Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
            byte[] protectedBytes = Base64UrlTextEncoder.Decode(cookieValue);
            byte[] plainBytes = dataProtector.Unprotect(protectedBytes);
            string plainText = specialUtf8Encoding.GetString(plainBytes);


            //Get the decrypted cookie as a Authentication Ticket
            TicketDataFormat ticketDataFormat = new TicketDataFormat(dataProtector);
            AuthenticationTicket ticket = ticketDataFormat.Unprotect(cookieValue);
            //CookieAuthenticationHandler a;
            //a.AuthenticateAsync()
            //var result = (await handler.AuthenticateAsync()) ?? AuthenticateResult.NoResult();
            //if (!result.Succeeded)
            //{
            //    return View();
            //}
            //var options = _options.CurrentValue;
            //var cookie = options.CookieManager.GetRequestCookie(HttpContext, options.Cookie.Name!);
            var b = await ReadCookieTicket();

            //var a = options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            return View();
        }

        private const string SessionIdClaim = "Microsoft.AspNetCore.Authentication.Cookies-SessionId";
        protected virtual string? ResolveTarget(string? scheme)
        {
            var target = scheme ?? _options.CurrentValue.ForwardDefaultSelector?.Invoke(HttpContext) ?? _options.CurrentValue.ForwardDefault;

            // Prevent self targetting
            return string.Equals(target, "Identity.Application", StringComparison.Ordinal)
                ? null
                : target;
        }
        private async Task<AuthenticateResult> ReadCookieTicket()
        {
            var Options = _options.CurrentValue;
            var Context = HttpContext;
            var a = "";
            var cookie = Options.CookieManager.GetRequestCookie(Context, a);
            var Clock = _clock;

            if (string.IsNullOrEmpty(cookie))
            {
                return AuthenticateResult.NoResult();
            }

            var ticket = Options.TicketDataFormat.Unprotect(cookie, GetTlsTokenBinding());
            if (ticket == null)
            {
                return AuthenticateResult.Fail("Unprotect ticket failed");
            }

            if (Options.SessionStore != null)
            {
                var claim = ticket.Principal.Claims.FirstOrDefault(c => c.Type.Equals(SessionIdClaim));
                if (claim == null)
                {
                    return AuthenticateResult.Fail("SessionId missing");
                }
                // Only store _sessionKey if it matches an existing session. Otherwise we'll create a new one.
                ticket = await Options.SessionStore.RetrieveAsync(claim.Value);
                if (ticket == null)
                {
                    return AuthenticateResult.Fail("Identity missing in session store");
                }
                _sessionKey = claim.Value;
            }

            var currentUtc = Clock.UtcNow;
            var expiresUtc = ticket.Properties.ExpiresUtc;

            if (expiresUtc != null && expiresUtc.Value < currentUtc)
            {
                if (Options.SessionStore != null)
                {
                    await Options.SessionStore.RemoveAsync(_sessionKey!);
                }
                return AuthenticateResult.Fail("Ticket expired");
            }

            CheckForRefresh(ticket);

            // Finally we have a valid ticket
            return AuthenticateResult.Success(ticket);
        }

        private void CheckForRefresh(AuthenticationTicket ticket)
        {
            var Options = _options.CurrentValue;
            var Context = HttpContext;
            var cookie = Options.CookieManager.GetRequestCookie(Context, Options.Cookie.Name!);
            var Clock = _clock;

            var currentUtc = Clock.UtcNow;
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var allowRefresh = ticket.Properties.AllowRefresh ?? true;
            if (issuedUtc != null && expiresUtc != null && Options.SlidingExpiration && allowRefresh)
            {
                var timeElapsed = currentUtc.Subtract(issuedUtc.Value);
                var timeRemaining = expiresUtc.Value.Subtract(currentUtc);

                if (timeRemaining < timeElapsed)
                {
                    RequestRefresh(ticket);
                }
            }
        }

        private void RequestRefresh(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal = null)
        {
            var issuedUtc = ticket.Properties.IssuedUtc;
            var expiresUtc = ticket.Properties.ExpiresUtc;
            var Clock = _clock;

            if (issuedUtc != null && expiresUtc != null)
            {
                _shouldRefresh = true;
                var currentUtc = Clock.UtcNow;
                _refreshIssuedUtc = currentUtc;
                var timeSpan = expiresUtc.Value.Subtract(issuedUtc.Value);
                _refreshExpiresUtc = currentUtc.Add(timeSpan);
                _refreshTicket = CloneTicket(ticket, replacedPrincipal);
            }
        }

        private AuthenticationTicket CloneTicket(AuthenticationTicket ticket, ClaimsPrincipal? replacedPrincipal)
        {
            var principal = replacedPrincipal ?? ticket.Principal;
            var newPrincipal = new ClaimsPrincipal();
            foreach (var identity in principal.Identities)
            {
                newPrincipal.AddIdentity(identity.Clone());
            }

            var newProperties = new AuthenticationProperties();
            foreach (var item in ticket.Properties.Items)
            {
                newProperties.Items[item.Key] = item.Value;
            }

            return new AuthenticationTicket(newPrincipal, newProperties, ticket.AuthenticationScheme);
        }

        private string? GetTlsTokenBinding()
        {
            var binding = HttpContext.Features.Get<ITlsTokenBindingFeature>()?.GetProvidedTokenBindingId();
            return binding == null ? null : Convert.ToBase64String(binding);
        }
    }
}
