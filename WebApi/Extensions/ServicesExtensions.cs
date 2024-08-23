using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.ActionFilters;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using Microsoft.AspNetCore.Mvc.Versioning;
using Presentation.Controllers;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WebApi.Extensions
{
    //Extensions static classlarla yazılır
    public static class ServicesExtensions
    {
        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) => services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, LoggerManager>();

        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
            services.AddSingleton<LogFilterAttribute>();
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination")
                );
            });
        }

        public static void ConfigureDataShaper(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var systemTextJsonOutputFormatter = config
                .OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

                if (systemTextJsonOutputFormatter != null)
                {
                    systemTextJsonOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookstore.apiroot+json");
                }

                var xmlOutputFormatter = config
                .OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter.SupportedMediaTypes
                    .Add("application/vnd.bookstore.apiroot+xml");
                }
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");

                opt.Conventions.Controller<BooksController>()
                    .HasApiVersion(new ApiVersion(1, 0));

                opt.Conventions.Controller<BooksV2Controller>()
                    .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
           services.AddHttpCacheHeaders(expirationOpt =>
           {
               expirationOpt.MaxAge = 90;
               expirationOpt.CacheLocation = CacheLocation.Public;
           },
           validationOpt =>
           {
               validationOpt.MustRevalidate = true;
           });

        public static void ConfigureRateLimitingOptions(this IServiceCollection services)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint = "*",
                    Limit = 5,
                    Period = "1m"
                }
            };

            services.Configure<IpRateLimitOptions>(opt =>
            {
                opt.GeneralRules = rateLimitRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, IdentityRole>(opts =>
            {
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 6;

                opts.User.RequireUniqueEmail = true; //Bir email bir defa kullanılacak
            })
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            // 'JwtSettings' adında bir yapılandırma bölümünü alıyoruz.
            // Bu bölüm appsettings.json dosyasında bulunur ve JWT ayarlarını içerir.
            var jwtSettings = configuration.GetSection("JwtSettings");
            // 'secretKey' anahtarını 'JwtSettings' bölümünden alıyoruz.
            var secretKey = jwtSettings["secretKey"];

            // ASP.NET Core kimlik doğrulama hizmetlerini ekliyoruz ve yapılandırıyoruz.
            services.AddAuthentication(opt =>
            {
                // Kimlik doğrulama ve yetkilendirme işlemlerinde kullanılacak şemaları belirliyoruz.
                // JWT doğrulaması için varsayılan kimlik doğrulama şeması olarak 'JwtBearerDefaults.AuthenticationScheme' kullanılıyor.
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
                // JWT doğrulama için gerekli olan token doğrulama parametrelerini yapılandırıyoruz.
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Token'ı oluşturanın doğrulanmasını etkinleştiriyoruz.
                    ValidateIssuer = true,
                    // Token'ı kullanacak olanın doğrulanmasını etkinleştiriyoruz.
                    ValidateAudience = true,
                    // Token'ın süresinin geçerliliğini doğruluyoruz.
                    ValidateLifetime = true,
                    // Token imza anahtarının doğrulanmasını etkinleştiriyoruz.
                    ValidateIssuerSigningKey = true,
                    // Geçerli issuer (oluşturan) değerini 'JwtSettings' bölümünden alıyoruz.
                    ValidIssuer = jwtSettings["validIssuer"],
                    // Geçerli audience (kullanıcı) değerini 'JwtSettings' bölümünden alıyoruz.
                    ValidAudience = jwtSettings["validAudience"],
                    // Token'ın imza anahtarı olarak 'secretKey' kullanılıyor.
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                }
            );
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "bsStoreApp v1",
                        Version = "v1",
                        Description = "bsStoreApp API",
                        Contact = new OpenApiContact
                        {
                            Name = "Murat Oğuz",
                            Email = "muratouz95@gmail.com"
                        }
                    });

                s.SwaggerDoc("v2", new OpenApiInfo { Title = "bsStoreApp v2", Version = "v2" });

                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Place to add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookManager>();
            services.AddScoped<ICategoryService, CategoryManager>();
            services.AddScoped<IAuthenticationService, AuthenticationManager>();
        }
    }
}
