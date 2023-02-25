using DigitalLibary.Data.Data;
using DigitalLibary.Service.Repository.IRepository;
using DigitalLibary.Service.Repository.RepositoryIPL;
using DigitalLibary.WebApi.Common;
using DigitalLibary.WebApi.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;

namespace DigitalLibary.WebApi
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // connect to database
            services.AddDbContextPool<DataContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DMSEntities")));

            // Register services before add to controller
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IRestDateRepository, RestDateRepository>();
            services.AddTransient<ISchoolYearRepository, SchoolYearRepository>();
            services.AddTransient<IContactAndIntroductionRepository, ContactAndIntroductionRepository>();
            services.AddTransient<IDocumentStockRepository, DocumentStockRepository>();
            services.AddTransient<IIndividualSampleRepository, IndividualSampleRepository>();
            services.AddTransient<JwtService>();
            services.AddTransient<SaveToDiary>();
            services.AddTransient<IDiaryRepository, DiaryRepository>();
            services.AddTransient<CategoriesRepository, CategoriesRepositoryImpl>();
            services.AddTransient<ICategorySignRepository, CategorySignRepository>();
            services.AddTransient<ICategoryPublisherRepository, CategoryPublisherRepositoryImpl>();
            services.AddTransient<ICategorySupplier, ICategorySupplierImpl>();
            services.AddTransient<ICategoryPublisherRepository, CategoryPublisherRepositoryImpl>();
            services.AddTransient<ICategorySupplier, ICategorySupplierImpl>();
            services.AddTransient<IDocumentInvoiceRepository, DocumentInvoiceRepository>();
            services.AddTransient<IUnitRepository, IUnitRepositoryImpl>();
            services.AddTransient<IAnalystRepository, AnalystRepository>();
            services.AddTransient<ISlideRepository, SlideRepository>();
            services.AddTransient<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddTransient<IReceiptRepository, ReceiptRepository>();
            services.AddTransient<ICategorySign_V1Repository, CategorySign_V1Repository>();
            services.AddTransient<IParticipantsRepository, ParticipantRepository>();
            services.AddTransient<ICalculateFolder, CalculateFolder>();    
            services.AddTransient<IAuditMethodRepository, AuditMethodRepository>();
            services.AddTransient<IStatusBookRepository, StatusBookRepository>();
            services.AddTransient<IAuditReceiptRepository, AuditReceiptRepository>();
            services.AddTransient<IAuditorListRepository, AuditorListRepository>();
            services.AddTransient<IAuditBookListRepository, AuditBookListRepository>();

            //get secret key from application.json
            services.Configure<AppSettingModel>(Configuration.GetSection("AppSettings"));

            // get secret key from appsetting.json
            var secretKey = Configuration["AppSettings:SecretKey"];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        //tự cấp token
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        //ký vào token
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                        ClockSkew = TimeSpan.Zero
                    };
                });

            //Enable cors
            services.AddCors(o => o.AddPolicy(MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));


/*            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
            });*/

            //regis service auto mapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();
            services.AddHttpClient();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Digital Libary", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital Libary v1"));
            }


            app.UseHttpsRedirection();
            app.UseRouting();
            //Enable cors
            app.UseCors(MyAllowSpecificOrigins);

            // Enable auth and author service
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
