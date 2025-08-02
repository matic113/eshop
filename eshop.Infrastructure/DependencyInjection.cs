using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Infrastructure.Authentication;
using eshop.Infrastructure.Email;
using eshop.Infrastructure.ObjectStorage;
using eshop.Infrastructure.Payment;
using eshop.Infrastructure.Persistence;
using eshop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

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
                opt.ClaimActions.MapJsonKey("picture", "picture");
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

            // CloudFlare R2 Object Storage

            // 1. Bind the R2Options class from the "CloudflareR2" section of your config.
            var r2Options = new R2Options();
            configuration.GetSection(R2Options.SectionName).Bind(r2Options);
            services.AddSingleton(Options.Create(r2Options)); // Make IOptions<R2Options> available

            // 2. Manually create the credentials for R2.
            var r2Credentials = new BasicAWSCredentials(r2Options.AccessKeyId, r2Options.SecretAccessKey);

            // 3. Manually create the S3-compatible config pointing to the R2 endpoint.
            var r2Config = new AmazonS3Config
            {
                ServiceURL = $"https://{r2Options.AccountId}.r2.cloudflarestorage.com",
                AuthenticationRegion = "auto",
            };

            // 4. Register the IAmazonS3 client as a singleton with the custom credentials and config.
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(r2Credentials, r2Config));

            services.AddScoped<IFileService, CloudflareFileService>();

            // Email

            services.AddFluentEmail(configuration["Email:FromAddress"]!)
                .AddRazorRenderer()
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

            // Payments

            services.Configure<PaymobOptions>(configuration.GetSection(PaymobOptions.PaymobOptionsKey));
            services.AddScoped<IPaymobService, PaymobService>();
            services.AddScoped<IPaymobHmacValidator, PaymobHmacValidator>();

            services.AddHttpClient<IPaymobService, PaymobService>(client =>
            {
                client.BaseAddress = new Uri("https://accept.paymob.com/");
            });

            // Repositories

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IVerificationTokensRepository, VerificationTokenRepository>();
            services.AddScoped<IAddressRepository, AddressRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderHistoryRepository, OrderHistoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
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

            // Default to SQL Server, uncomment Postgres if needed
            services.AddDbContext<ApplicationDbContext, SqlServerDbContext>(opt =>
            {
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("eshop.Infrastructure"));
            });

            //services.AddDbContext<ApplicationDbContext, PostgresDbContext>(opt =>
            //{
            //    opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            //        b => b.MigrationsAssembly("eshop.Infrastructure"));

            //    opt.UseSnakeCaseNamingConvention();
            //});
        }
    }
}