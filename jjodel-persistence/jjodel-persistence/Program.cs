using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Settings;
using jjodel_persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container (DI).

// configuration.
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.Configure<Identity>(builder.Configuration.GetSection(nameof(Identity)));
builder.Services.Configure<Jwt>(builder.Configuration.GetSection(nameof(Jwt)));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
               // use postgre
               options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
               );

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
    options.SignIn.RequireConfirmedAccount =
        builder.Configuration.GetValue<bool>("Identity:RequireConfirmedAccount");
    options.Password.RequiredLength =
        builder.Configuration.GetValue<int>("Identity:RequiredLength");
    options.Password.RequireDigit =
        builder.Configuration.GetValue<bool>("Identity:RequireDigit");
    options.Password.RequireLowercase =
        builder.Configuration.GetValue<bool>("Identity:RequireLowercase");
    options.Password.RequireNonAlphanumeric =
        builder.Configuration.GetValue<bool>("Identity:RequireNonAlphanumeric");
    options.Password.RequireUppercase =
        builder.Configuration.GetValue<bool>("Identity:RequireUppercase");
})
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
     .AddJwtBearer(options => {
         options.TokenValidationParameters = new TokenValidationParameters {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = builder.Configuration["Jwt:Issuer"],
             ValidAudience = builder.Configuration["Jwt:Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))
         };
     });

// services
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<ProjectService>();

builder.Services.AddControllersWithViews(); // add api and MVC
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


//logger.
builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) => {
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration);
}, writeToProviders: true);

builder.Services.
    AddFluentEmail(builder.Configuration.GetValue<string>("MailSettings:FromDefault"))
   .AddRazorRenderer()
   .AddMailKitSender(new FluentEmail.MailKitSmtp.SmtpClientOptions() {
       Server = builder.Configuration.GetValue<string>("MailSettings:Server"),
       Password = builder.Configuration.GetValue<string>("MailSettings:Password"),
       User = builder.Configuration.GetValue<string>("MailSettings:Username"),
       Port = builder.Configuration.GetValue<int>("MailSettings:Port"),
       UseSsl = builder.Configuration.GetValue<bool>("MailSettings:UseSSL"),
       RequiresAuthentication = builder.Configuration.GetValue<bool>("MailSettings:RequiresAuthentication")
   });

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
else {
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// must be before use auth.
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseSerilogRequestLogging();

#if DEBUG

// scope needed to initialize db at app startup.
using(var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;

    ApplicationDbContext applicationDbContext = services.GetService<ApplicationDbContext>();

    applicationDbContext.Database.Migrate();
    // come se fosse DI.
    DbInitializer db = new DbInitializer(
        applicationDbContext,
        services.GetService<UserManager<ApplicationUser>>(),
        services.GetService<RoleManager<ApplicationRole>>());
    db.Initialize();
}
#endif

app.Run();


/*
 *  1) User postgres; Password=postgres; Port=5432; Database=jjodel;
 */