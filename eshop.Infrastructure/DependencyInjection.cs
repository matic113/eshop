using System.Net;
using System.Net.Mail;
using System.Text;
using eshop.Application.Contracts;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Email;
using eshop.Infrastructure.Persistence;
using eshop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace eshop.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JwtOptionsKey));

            services.AddDatabase(configuration);

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, opt =>
            {
                opt.ClientId = configuration["Google:ClientId"]!;
                opt.ClientSecret = configuration["Google:ClientSecret"]!;
                opt.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.Scope.Add("email");
                opt.Scope.Add("profile");
            })
            .AddJwtBearer(options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.JwtOptionsKey)
                    .Get<JwtOptions>() ?? throw new ArgumentException(nameof(JwtOptions));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            services.AddScoped<JwtService>();

            services.AddAuthorization();

            // Email

            services.AddFluentEmail(configuration["Email:FromAddress"]!)
                .AddSmtpSender(new SmtpClient
                {
                    Host = configuration["Email:SmtpHost"]!,
                    Port = int.Parse(configuration["Email:SmtpPort"]!),
                    EnableSsl = bool.Parse(configuration["Email:SmtpEnableSsl"]!),
                    Credentials = new NetworkCredential(
                        configuration["Email:SmtpUsername"]!,
                        configuration["Email:SmtpPassword"]!)
                });

            services.AddScoped<IEmailService, EmailService>();

            // Repositories

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVerificationTokensRepository, VerificationTokenRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
        }

        private static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Monster")
            {
                services.AddDbContext<ApplicationDbContext, SqlServerDbContext>(opt =>
                {
                    opt.UseSqlServer(configuration.GetConnectionString("Monster"),
                        b => b.MigrationsAssembly("eshop.Infrastructure"));
                });

                return;
            }

            services.AddDbContext<ApplicationDbContext, PostgresDbContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("eshop.Infrastructure"));

                opt.UseSnakeCaseNamingConvention();
            });
        }
    }
}