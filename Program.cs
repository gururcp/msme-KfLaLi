//using HotelwebLisMVC.Models;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();



//builder.Services.AddDbContext<HotelWebLisDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));


//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthorization();

//app.MapControllers();

//app.Run();







//-------------------------------





//using HotelwebLisMVC.Models;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();


//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        // Handle circular references and preserve property names
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//        options.JsonSerializerOptions.PropertyNamingPolicy = null;
//    });

//// ✅ Add CORS policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp",
//        policy => policy
//            .WithOrigins("http://localhost:3000")
//            .AllowAnyHeader()
//            .AllowAnyMethod());
//});



////builder.Services.AddCors(options =>
////{
////    options.AddPolicy("AllowAll",
////        builder => builder
////            .AllowAnyOrigin()
////            .AllowAnyMethod()
////            .AllowAnyHeader());
////});

//// And in the app configuration:


//builder.Services.AddDbContext<HotelWebLisDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

//// Swagger/OpenAPI
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//// ✅ Use CORS before UseAuthorization
////app.UseCors("AllowReactApp"); 

//app.UseCors("AllowAll");
//app.UseAuthorization();

//app.MapControllers();

//app.Run();
















using HotelwebLisMVC.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references and preserve property names
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ► CORS: allow React dev server
const string ReactCorsPolicy = "ReactPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:3000",
                          "https://front.kafelaali.com"
        )   // add more origins if needed
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());                   // remove if you don’t send cookies/auth
});

builder.Services.AddDbContext<HotelWebLisDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ───── Configure middleware ────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS before authorization
app.UseCors(ReactCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();

















//using HotelwebLisMVC.Models;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        // Handle circular references and preserve property names
//        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//        options.JsonSerializerOptions.PropertyNamingPolicy = null;
//    });

//// Swagger/OpenAPI support
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
//builder.WebHost.CaptureStartupErrors(true);
//builder.WebHost.UseSetting("detailedErrors", "true");

//// Register DB context using configuration
//builder.Services.AddDbContext<HotelWebLisDBContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));





//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("CORSPolicy", policy =>
//    {
//        policy
//            .WithOrigins(
//               "http://localhost:3000",          // ✅ Local React
//                "https://front.lakshitsolution.com", // ✅ Production frontend 
//                "http://localhost:5173",              // ✅ Electron.js frontend 
//                "https://react.gvish.org" // ✅ Production frontend 

//            )
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .AllowCredentials(); // Only if you're using cookies/auth headers
//    });
//});




//var app = builder.Build();

//// Use detailed error page in development
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//// ✅ Enable CORS before routing/middleware
//app.UseCors("CORSPolicy");

//app.UseAuthorization();

//app.MapControllers();

//app.Run();