namespace Wasl.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidateModelAttribute>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.UnmappedMemberHandling =
                    JsonUnmappedMemberHandling.Skip;
                options.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SchemaFilter<EmptyStringSchemaFilter>();
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddSwaggerService(builder.Configuration);
            builder.Services.AddInfrastructureService(builder.Configuration);

            builder.Services.AddCors(op => op.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            }));

            /*var firebasePath = Path.Combine(
              builder.Environment.ContentRootPath,
              "wwwroot",
              "wwwroot",
              "wasl-2b1be-firebase-adminsdk-fbsvc-022ffcfc3d.json"
          );*/

            /*var firebaseJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
            if (string.IsNullOrEmpty(firebaseJson))
            {
                throw new Exception("Firebase credentials not found");
            }

            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromJson(firebaseJson)
            });*/




            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WaslDbContext>();

                await context.Database.MigrateAsync();
                await app.Services.SeedAsync();
            }

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowAll");


            app.UseGlobalExceptionHandler();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}