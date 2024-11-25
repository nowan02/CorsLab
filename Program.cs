var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

// HTTPS redirection

if(! builder.Environment.IsDevelopment() )
{
    builder.Services.AddHttpsRedirection(options => {
        options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;    
        options.HttpsPort = 443;
    });
}


// CORS
builder.Services.AddCors(options => {

    options.AddPolicy(name: "ArbitraryOrigin",
    policy => {
        policy.WithOrigins("*"); // or the server sets the origin based on the request
        policy.AllowAnyMethod();
        policy.AllowCredentials();
        policy.Build();
    });

    options.AddPolicy(name: "NullOrigin",
    policy => {
        policy.WithOrigins("null");
        policy.AllowAnyMethod();
        policy.AllowCredentials();
        policy.Build();
    });

    options.AddPolicy(name: "BadRegex",
    policy => {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.Build();
    });

    options.AddPolicy(name: "BadSubdomain",
    policy => {
        policy.WithOrigins("*.localhost");
        policy.SetIsOriginAllowedToAllowWildcardSubdomains();
        policy.AllowCredentials();
        policy.AllowAnyMethod();
        policy.Build();
    });

    options.AddPolicy(name: "ValidPolicy",
    policy => {

        // EXPLICIT ADDRESSES!
        policy.WithOrigins("https://localhost.com:443", "https://subdomain.localhost.com:443");

        // EXPLICIT METHODS!
        policy.WithMethods("GET");

        // DISALLOW CREDENTIALS WHERE NOT NEEDED!
        policy.DisallowCredentials();

        // ONLY ALLOW HEADERS WHICH ARE NECESSARY
        policy.WithHeaders("Header-that-is-absolutely-needed");

        // SET PREFLIGHT REQUEST MAX AGE TO PREVENT CACHE POISONING
        policy.SetPreflightMaxAge(TimeSpan.FromMinutes(1));

        policy.Build();
    });
});

builder.Services.AddMvc();
builder.Services.AddControllers();

var app = builder.Build();

// If https is implemented, force redirection, and don't allow http links in CORS policy.
if(! builder.Environment.IsDevelopment() ) app.UseHttpsRedirection();

app.UseCors("ArbitraryOrigin")
.UseCors("NullOrigin")
.UseCors("ClientsideCache")
.UseCors("BadRegex")
.UseCors("ValidPolicy");

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();