using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using WIRS.DataAccess.Implementations;
using WIRS.DataAccess.Interfaces;
using WIRS.DataAccess.Mock;
using WIRS.Services.Implementations;
using WIRS.Services.Interfaces;
using WIRS.Shared.Configuration;
using WIRS.Shared.Extensions;

namespace WIRS.Mvc.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddHttpContextAccessor();

			services.AddConfigurations(configuration);

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Login";
					options.LogoutPath = "/Login/Logout";
					options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
					options.SlidingExpiration = true;
					options.Cookie.Name = "WIRS.Auth";
					options.Cookie.HttpOnly = true;
					options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

					options.Events = new CookieAuthenticationEvents
					{
						OnRedirectToLogin = context =>
						{
							if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
							{
								context.Response.StatusCode = 401;
								return Task.CompletedTask;
							}

							if (context.Request.Path.StartsWithSegments("/Login"))
							{
								context.Response.StatusCode = 401;
								return Task.CompletedTask;
							}

							context.Response.Redirect(context.RedirectUri);
							return Task.CompletedTask;
						}
					};
				});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("AdminOnly", policy => policy.RequireClaim("UserRole", "9"));
				options.AddPolicy("ManagementOnly", policy => policy.RequireClaim("UserRole", "5", "6", "7", "8", "9"));
				options.AddPolicy("WSHOnly", policy => policy.RequireClaim("UserRole", "3", "4", "6"));
				options.AddPolicy("HigherAuthority", policy => policy.RequireClaim("UserRole", "5", "6", "7", "8", "9"));
			});

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromMinutes(30);
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			});

			services.AddControllersWithViews(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			});

			services.AddKendo();

			services.AddMemoryCache();

			services.AddSingleton<AutoMapper.IMapper>(provider =>
			{
				var configuration = new AutoMapper.MapperConfiguration(cfg =>
				{
					cfg.AddProfile<WIRS.Services.Mapping.MappingProfile>();
				});
				return configuration.CreateMapper();
			});

			services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

			var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

			if (appSettings?.UseMockData == true)
			{
				services.AddScoped<ICommonFunDataAccess, MockCommonFunDataAccess>();
				services.AddScoped<IEmailDistributionDataAccess, MockEmailDistributionDataAccess>();
				services.AddScoped<IEmployeeDataAccess, MockEmployeeDataAccess>();
				services.AddScoped<IErrorMessageDataAccess, MockErrorMessageDataAccess>();
				services.AddScoped<IIncidentDataAccess, MockIncidentDataAccess>();
				services.AddScoped<IManHoursDataAccess, MockManHoursDataAccess>();
				services.AddScoped<IMenuDataAccess, MockMenuDataAccess>();
				services.AddScoped<IPrintDataAccess, MockPrintDataAccess>();
				services.AddScoped<IStatisticsReportDataAccess, MockStatisticsReportDataAccess>();
				services.AddScoped<IUserCredentialsDataAccess, MockUserCredentialsDataAccess>();
				services.AddScoped<IUserDataAccess, MockUserDataAccess>();
				services.AddScoped<IWorkflowIncidentDataAccess, MockWorkflowIncidentDataAccess>();
			}
			else
			{
				services.AddDBHelpers();
				services.AddScoped<ICommonFunDataAccess, CommonFunDataAccess>();
				services.AddScoped<IEmailDistributionDataAccess, EmailDistributionDataAccess>();
				services.AddScoped<IEmployeeDataAccess, EmployeeDataAccess>();
				services.AddScoped<IErrorMessageDataAccess, ErrorMessageDataAccess>();
				services.AddScoped<IIncidentDataAccess, IncidentDataAccess>();
				services.AddScoped<IManHoursDataAccess, ManHoursDataAccess>();
				// we used mock menu for now to not impact the behavior
                services.AddScoped<IMenuDataAccess, MockMenuDataAccess>();
                //services.AddScoped<IMenuDataAccess, MenuDataAccess>();
				services.AddScoped<IPrintDataAccess, PrintDataAccess>();
				services.AddScoped<IStatisticsReportDataAccess, StatisticsReportDataAccess>();
				services.AddScoped<IUserCredentialsDataAccess, UserCredentialsDataAccess>();
				services.AddScoped<IUserDataAccess, UserDataAccess>();
				services.AddScoped<IWorkflowIncidentDataAccess, WorkflowIncidentDataAccess>();
			}

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IMasterDataService, MasterDataService>();
			services.AddScoped<IMaintenanceService, MaintenanceService>();
			services.AddScoped<IIncidentService, IncidentService>();
			services.AddScoped<IWorkflowService, WorkflowService>();
			services.AddScoped<IEncryptionService, EncryptionService>();
			services.AddScoped<IDataMapperService, DataMapperService>();
			services.AddScoped<IUrlGeneratorService, UrlGeneratorService>();
			services.AddScoped<IMenuService, MenuService>();

			services.AddScoped<WIRS.Services.Auth.IAuthService, WIRS.Services.Auth.AuthService>();

			return services;
		}
	}
}