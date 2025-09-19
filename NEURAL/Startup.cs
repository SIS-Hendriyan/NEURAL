using Aspose.Slides;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using NEURAL.Models.Request;
using NEURAL.Repositories.Context;
using NEURAL.Repositories.Implementations;
using NEURAL.Repositories.Interfaces;
using NEURAL.Services;
using NEURAL.Services.DPR;
using NEURAL.Services.Implementations;
using NEURAL.Services.Interfaces;
using NEURAL.Services.Outlook;
using NEURAL.Services.ProductionActual;
using System.Threading.Channels;

namespace NEURAL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Server = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly IConfiguration Server;
        public static string connectionstring { get; private set; }
        public static string cookiesName { get; private set; }
        public static string serverConfig { get; private set; }
        public static string AddressWS { get; private set; }
        public static int CookiesExpired { get; private set; }
        public static int Session_Timeout { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
			services.AddControllersWithViews();

            services.AddMvc();
            string conString = this.Configuration.GetConnectionString("dbConnection");
            connectionstring = conString;

            string _appName = Server.GetSection("App").GetSection("Name").Value;
            serverConfig = _appName;

            string _cookiesName = Server.GetSection("Cookies").GetSection("NRP").Value;
            cookiesName = _cookiesName;

            int _CookiesExpired = int.Parse(Configuration["Cookies:timeout"]);
            CookiesExpired = _CookiesExpired;
            int _Session_Timeout = int.Parse(Configuration["Session:timeout"]);
            Session_Timeout = _Session_Timeout;

            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(Session_Timeout);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = ".AspNetCore.Session.SIS";
            });

            services.AddRazorPages();

            services
            .AddControllersWithViews()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = null)
            .AddCookieTempDataProvider(options =>
             {
                 options.Cookie.Name = ".AspNetCore.Session.SIS";
                 options.Cookie.Path = "/";
                 options.Cookie.Domain = "saptaindra.co.id";
             });

            services.AddDbContext<AppDbContext>(opt =>opt.UseSqlServer(conString));

            //DI repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProcessRepository, ProcessRepository>();
            services.AddScoped<IJobsiteRepository, JobsiteRepository>();
            services.AddScoped<IProdSchedHeaderRepository, ProdSchedHeaderRepository>();
            services.AddScoped<IProdSchedPivotRepository,ProdSchedPivotRepository>();
            services.AddScoped<IProdSchedInterventionRepository, ProdSchedInterventionRepository>();
            services.AddScoped<IProdSchedStagingRepository, ProdSchedStagingRepository>();
            services.AddScoped<IProdSchedStagingInterventionRepository, ProdSchedStagingInterventionRepository>();
            services.AddScoped<IProdSchedInterventionExcelExporter, ProdSchedInterventionExcelExporter>();
            services.AddScoped<IProdSchedExcelReader, ProdSchedExcelReader>();

            var channelIntervention = Channel.CreateUnbounded<ProdSchedInterventionUploadRequest>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });
            var channelProdSched = Channel.CreateUnbounded<ProdSchedUploadRequest>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = false });

            services.AddSingleton(channelIntervention);
            services.AddHostedService<ProdSchedUploadInterventionService>();
            services.AddSingleton(channelProdSched);
            services.AddHostedService<ProdSchedUploadExcelService>();

            services.AddSingleton<ISurveyService, SurveyService>();
            services.AddSingleton<IDPRService, DPRService>();
            services.AddSingleton<IProductionActualService, ProductionActualService>();
            services.AddSingleton<IOutlookService, OutlookService>();


            services.AddScoped<IHolidayRepository, HolidayRepository>();

            services.AddControllers();
            services.AddHttpContextAccessor();

            //at home
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme);
          
            //at office
           // services.AddAuthentication(NegotiateDefaults.AuthenticationScheme);
            //    .AddNegotiate();

            services.AddHttpClient<IConnector, Aspose.Slides.Connector>().ConfigurePrimaryHttpMessageHandler(
               serviceProvider =>
               {
                   var httpClientHandler = new HttpClientHandler
                   {
                       UseProxy = false,
                       UseDefaultCredentials = true
                   };
                   return httpClientHandler;
               });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // Add the Kendo UI services to the services container.
            //services.AddKendo().AddMvcCore();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    //pattern: "{controller=Login}/{action=Login}/{id?}");
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
