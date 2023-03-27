namespace OhneSize
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //JwtBearerDefaults.AuthenticationScheme
            var auth = builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme);
            auth.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
            auth.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"),
                JwtBearerDefaults.AuthenticationScheme);
            /*
               services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                          .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
                              .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                                  .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
                                  .AddInMemoryTokenCaches();

              services.AddAuthentication()
                        .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"),
                                                    JwtBearerDefaults.AuthenticationScheme)
                        .EnableTokenAcquisitionToCallDownstreamApi();
             */
            //auth.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
            //auth.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddRazorPages();
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddServicesWithRegistrator(
              actionAdd: (typeSourceSelector) =>
              {
                  var dependencyContext = Microsoft.Extensions.DependencyModel.DependencyContext.Default;
                  if (dependencyContext is not null)
                  {
                      var implementationTypeSelector =
                          typeSourceSelector.FromApplicationDependencies(
                              context: dependencyContext);
                      implementationTypeSelector.AddClasses().UsingAttributes();
                      implementationTypeSelector.AddOptionsCurrent();
                  }
              }
              );

            builder.Services.AddSpaStaticFiles(
                (configuration) =>
                {
                    configuration.RootPath = @"C:\github.com\FlorianGrimm\OhneSize\ohnesize\dist\ohnesize";
                }
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            //app.UseHttpsRedirection();

            /*
                        app.UseStaticFiles();
                        */
            /*
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                       Path.Combine(builder.Environment.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles"
            });
            */
            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(@"C:\github.com\FlorianGrimm\OhneSize\ohnesize\dist\ohnesize"),
            //     RequestPath = "/app"
            // });
            //Microsoft.Extensions.DependencyInjection.SpaStaticFilesExtensions.UseSpaStaticFiles

            /**/
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            //if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            

            app.MapRazorPages();
            app.MapControllers();
            //app.UseEndpoints(endpoints => {});
            app.UseSpaStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(@"C:\github.com\FlorianGrimm\OhneSize\ohnesize\dist\ohnesize"),
                RequestPath = "/app"
            });
            // app.UseDefaultFiles(new DefaultFilesOptions()
            // {
            //     DefaultFileNames = new List<string>() { "index.html" }
            // }); 
            app.UseSpa(
                (spa) =>
                {
                    spa.Options.DefaultPage = "/app/index.html";
                    spa.Options.SourcePath = @"C:\github.com\FlorianGrimm\OhneSize\ohnesize\dist\ohnesize";
                    //spa.Options.SourcePath = @"/app";
                    spa.Options.DefaultPageStaticFileOptions=new StaticFileOptions(){
                        FileProvider = new PhysicalFileProvider(@"C:\github.com\FlorianGrimm\OhneSize\ohnesize\dist\ohnesize"),
                        RequestPath = "/app"
                    };
                    // if (app.Environment.IsDevelopment())
                    // {
                    //     spa.UseAngularCliServer("start");
                    // }
                }
            );
            app.Run();
        }
    }
}