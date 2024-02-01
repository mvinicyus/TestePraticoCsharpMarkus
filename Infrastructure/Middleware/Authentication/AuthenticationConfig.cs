using Infrastructure.Message;
using Infrastructure.Message.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Middleware.Authentication
{
    public static class AuthenticationConfig
    {
        public static void AddAuthenticationJwt(this IServiceCollection services)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.ClaimsIssuer = "Role";
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = GenTokenValidationParameters();

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        return TokenValidated(context);
                    },
                    OnAuthenticationFailed = context =>
                    {
                        return AuthenticationFailed(context);
                    }
                };
            });
        }

        private static Task TokenValidated(TokenValidatedContext context)
        {
            var authenticate = GetAuthenticate(context.Principal.Claims, context.HttpContext);

            if (authenticate == null)
            {
                return Unauthorized(context.HttpContext, "Sua sessão expirou! Faça login novamente.");
            }

            var bearerContext = context.HttpContext.Request.Headers["authorization"].ToString();

            if (!IsValidToken(authenticate.Token, bearerContext))
            {
                return Unauthorized(context.HttpContext, "Sua sessão expirou! Faça login novamente.");
            }

            var refreshTokenContext = context.HttpContext.Request.Headers["RefreshToken"].ToString();
            var expireDate = GetRefreshTokenExpireDate(refreshTokenContext);
            if (!expireDate.HasValue || expireDate < DateTime.UtcNow)
            {
                return Unauthorized(context.HttpContext, "Sua sessão expirou! Faça login novamente.");
            }

            if (!IsValidRefreshToken(authenticate.RefreshToken, refreshTokenContext))
            {
                return Unauthorized(context.HttpContext, "Sua sessão expirou! Faça login novamente.");
            }

            var newRefreshToken = GenerateRefreshToken();

            context.HttpContext.Response.Headers.Add("refreshToken", newRefreshToken);
            context.Principal = GetPrincipal(authenticate.Token);
            context.Success();
            return Task.CompletedTask;
        }

        private static Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            var refreshTokenContext = context.HttpContext.Request.Headers["RefreshToken"].ToString();
            var accessTokenContext = context.HttpContext.Request.Headers["AccessToken"].ToString();

            if (string.IsNullOrEmpty(refreshTokenContext) || string.IsNullOrEmpty(accessTokenContext))
            {
                return Unauthorized(context.HttpContext, "Faça o login");
            }

            var bearerContext = context.Request.Headers["authorization"].ToString().Replace("Bearer ", "");
            context.Exception = null;
            context.Principal = GetPrincipal(bearerContext);
            context.Success();
            return Task.CompletedTask;
        }

        public static TokenValidationParameters GenTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(GetJwtKey()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
        }

        public static byte[] GetJwtKey()
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("SecretJwt")));
            return key.Key;
        }

        private static string Get(this IEnumerable<Claim> claims, string claim)
        {
            var value = claims.FirstOrDefault(a => a.Type == claim || a.Type == claim.Replace("http://schemas.microsoft.com/ws/2008/06/identity/claims/", ""));
            return value?.Value ?? "";
        }

        private static JwtInfo GetAuthenticate(IEnumerable<Claim> claims, HttpContext context)
        {
            var typeToken = claims.FirstOrDefault().Type;

            if (typeToken != null && typeToken != "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
            {
                return null;
            }

            var tokenName = claims.FirstOrDefault().Value;

            if (string.IsNullOrEmpty(tokenName))
            {
                return null;
            }

            var refreshTokenContext = context.Request.Headers["RefreshToken"].ToString();
            var bearerContext = context.Request.Headers["authorization"].ToString().Replace("Bearer ", "");

            return new JwtInfo
            {
                Token = bearerContext,
                RefreshToken = refreshTokenContext,
                Email = claims.Get(ClaimTypes.Name),
                Id = Convert.ToInt32(claims.Get(ClaimTypes.PrimarySid))
            };
        }

        private static Task Unauthorized(HttpContext context, string because)
        {
            try
            {
                var notifications = context.RequestServices.GetService<IMessagesHandler>();
                notifications.InvalidNotification($"Não autorizado '{because}'");
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void InvalidNotification(this IMessagesHandler handler, string message)
        {
            var isError = true;
            handler.SendDomainNotificationAsync
            (
                new DomainNotification
                (
                    nameof(AuthenticationConfig),
                    message,
                    isError
                )
            ).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static bool IsValidToken(string tokenCache, string bearerContext)
        {
            var tokenContext = bearerContext.Replace("Bearer ", "");
            return tokenCache == tokenContext;
        }

        public static DateTime? GetRefreshTokenExpireDate(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return default;
            }
            var refreshTokenClaims = new JwtSecurityTokenHandler()?.ValidateToken(refreshToken, GenTokenValidationParameters(), out _);
            var refreshTokenExpire = refreshTokenClaims?.FindFirstValue(ClaimTypes.Expiration);
            if (refreshTokenExpire == null)
            {
                return default;
            }
            var refreshTokenExpireDate = DateTime.ParseExact(refreshTokenExpire, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return refreshTokenExpireDate;
        }

        private static bool IsValidRefreshToken(string refreshTokenCache, string refreshTokenContext)
        {
            return refreshTokenCache == refreshTokenContext;
        }

        public static string GenerateRefreshToken()
        {
            var expireDate = DateTime.UtcNow.AddMinutes(Convert.ToInt32(Environment.GetEnvironmentVariable("AuthenticationTimeInMinutes")));
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Expiration, expireDate.ToString("yyyy-MM-dd HH:mm:ss"))
            });

            var key = AuthenticationConfig.GetJwtKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDate,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, AuthenticationConfig.GenTokenValidationParameters(), out _);

            return principal;
        }
    }
}