using API.ActionFilters;
using API.Extensions;
using API.Middlewares;
using Application.ConfigurationSettings;
using Application.Helpers;
using Infrastructure.Data.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using SendGrid.Helpers.Mail;
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.ConfigureCors();
builder.Services.ConfigureEmail();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureWebHelper();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureAzureStorageServices();
builder.Services.ConfigureMvc();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
//builder.Services.ConfigureHangfire(builder.Configuration);
builder.Services.ConfigureVersioning();
builder.Services.AddHttpContextAccessor();

// Identity
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureControllers();
builder.WebHost.UseSentry(o => {
    o.Dsn = "https://c6da22e2f8054790b08ea63cbb356f67@o4505551146057728.ingest.sentry.io/4505551154053120";
    o.Debug = true;
    o.TracesSampleRate = 1.0;
});


var app = builder.Build();

// Data Seeded in the Database
app.SeedRoleData().Wait();
app.SeedUserData().Wait();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var provider = app.Services.GetService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseSentryTracing();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseErrorHandler();
app.MapControllers();
WebHelper.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.Run();
