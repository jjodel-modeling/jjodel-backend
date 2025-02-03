using jjodel_persistence.Models.Entity;
using jjodel_persistence.Models.Settings;
using jjodel_persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;


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

// services
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();



app.Run();

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
/*
 *  1) User postgres; Password=postgres; Port=5432; Database=jjodel;
 */