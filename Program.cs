using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace iBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        //public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)                                   
        //  .ConfigureAppConfiguration((context, config) =>
        //  {
        //      if (context.HostingEnvironment.IsProduction())
        //      {
        //          var builtConfig = config.Build();

        //          var azureServiceTokenProvider = new AzureServiceTokenProvider();
        //          var keyVaultClient = new KeyVaultClient(
        //              new KeyVaultClient.AuthenticationCallback(
        //                  azureServiceTokenProvider.KeyVaultTokenCallback));

        //          config.AddAzureKeyVault(
        //              $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
        //              keyVaultClient,
        //              new DefaultKeyVaultSecretManager());
        //      }
        //  })
        //  .ConfigureWebHostDefaults(webBuilder =>
        //  {
        //      webBuilder.UseStartup<Startup>();
        //  });
    }
}
