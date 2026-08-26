using Microsoft.AspNetCore.Authentication.Cookies;  //x auth tramite cookies
using RagSystemFrontend.Core.ServiceContracts;
using RagSystemFrontend.UI.Security;
using RagSystemFrontend.UI.ServicesImplementations;
using Serilog;  //lib x logging


//potrei anche aggiungere lib FluentValidation, è pro. 

var builder = WebApplication.CreateBuilder(args);  //crea il builder dell'app appsettings.json env vars dependency injection container configs ect

builder.Host.UseSerilog((context, loggerConfiguration) =>   //serilog configs, quindi ora non usi il logs di microsoft ma serilog che è più potente e flessibile puoi loggare su console file database ecc
{
    loggerConfiguration
        .MinimumLevel.Information()   //livello minimo di log, quindi logga info warning error  fatel, ma non debug trace
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)  //riduce il noise 
        .Enrich.FromLogContext()   //aggiunge auto info utili come timestamp thread id ecc
        .WriteTo.Console();

    var seqUrl = context.Configuration["Seq:ServerUrl"];
    if (!string.IsNullOrWhiteSpace(seqUrl))
    {
        loggerConfiguration.WriteTo.Seq(seqUrl);   //invia tutti i logs a Seq server se è configurato, utile per centralizzare i logs e fare query analisi ecc
    }
});

//add services to the container
builder.Services.AddControllersWithViews();   //registra i controller e le view per l'MVC pattern
builder.Services.AddHttpContextAccessor();   //registra un servizio che permette di accedere al contesto HTTP corrente (richiesta, risposta, utente ecc) da altre classi tramite dependency injection usando e.g._httpContextAccessor.HttpContext.User
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");   //protegge dai CSRF, il frontend dovrà inviare un header X-CSRF-TOKEN con il token generato dal server per le richieste POST/PUT/DELETE TODO

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)   //cookie auth
    .AddCookie(options =>
    {
        options.Cookie.Name = "RagAuth";   //il browser riceeverà RagAuth=......
        options.Cookie.HttpOnly = true;   //js NON può leggere il cookie
        options.Cookie.SameSite = SameSiteMode.Lax;   //riduce gli attacchi CSRF! 
        //dove reindirizzare in base a questi casi
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);  //xk sul backend il JWT ha scadenza 60min!
        options.SlidingExpiration = false;   //dopo 60min scade definitivamente
    });

builder.Services.AddAuthorizationBuilder()   //auth policies
    .AddPolicy("TenantAuth", policy => policy.RequireClaim(AuthClaimTypes.TenantToken))
    .AddPolicy("TenantAdmin", policy => policy.RequireClaim(AuthClaimTypes.TenantToken).RequireRole("admin"))
    .AddPolicy("PlatformAuth", policy => policy.RequireClaim(AuthClaimTypes.PlatformToken))
    .AddPolicy("SuperAdminAuth", policy => policy.RequireClaim(AuthClaimTypes.PlatformToken).RequireClaim(AuthClaimTypes.IsSuperAdmin));

var backendBaseUrl = builder.Configuration["BackendApi:BaseUrl"] ?? "http://localhost:8000";
var apiBaseUri = new Uri(backendBaseUrl.TrimEnd('/') + "/api/v1/");  //endpoint backendBaseUrl+/api/v1/ per tutti

void ConfigureBackendClient(HttpClient client) => client.BaseAddress = apiBaseUri;  //function



static void StandardResilience(Microsoft.Extensions.Http.Resilience.HttpStandardResilienceOptions options)  //setti la resilence (nuova feat di .NET)
{
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(30);  //quanto puo durare 1 tentativo: 30secs
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);   //tempo massimo totale dei tentativi sommati: 60sec
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(70);   //x evitare di continuare a chiamare un backend rotto
}

static void LongRunningResilience(Microsoft.Extensions.Http.Resilience.HttpStandardResilienceOptions options)  //resilience x upload documenti(mac 100mb), chat LLM che possono durare tanto
{
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(120);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(180);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(250);
}


//e.g. quando qualcuno chiede IChatApiClient aspnet crea auto ChatApiClient
builder.Services.AddHttpClient<ITenantAuthApiClient, TenantAuthApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<IPlatformAuthApiClient, PlatformAuthApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<ISpacesApiClient, SpacesApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<IChatApiClient, ChatApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(LongRunningResilience);
builder.Services.AddHttpClient<IDocumentsApiClient, DocumentsApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(LongRunningResilience);
builder.Services.AddHttpClient<ICollectionsApiClient, CollectionsApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<IJobsApiClient, JobsApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<IUsersApiClient, UsersApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);
builder.Services.AddHttpClient<ITenantsApiClient, TenantsApiClient>(ConfigureBackendClient)
    .AddStandardResilienceHandler(StandardResilience);

var app = builder.Build();   //l'app viene realmente costruita 



if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  //usa questo invece di mostrare stack trace 
}
app.UseSerilogRequestLogging();   //ogni req genera un log automatico con info come path, status code, durata ecc
app.UseRouting();  //motore di routing
app.UseAuthentication();   //legge il cookie RagAuth e popola HttpContext.User con le claims del JWT lato backend
app.UseAuthorization();   //check le policies
app.MapStaticAssets();  //serve i files statici
app.MapControllerRoute(  //definisce la rotta di default e.g. / -> HomeController.Index()
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();   //avvio del server


