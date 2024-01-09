namespace EngineBay.Persistence
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public static class DataBaseSeeder
    {
        public static void LoadSeedData<TInputParameters, TOutputDto, TCommandHandler>(
                    string seedDataPath,
                    string glob,
                    IServiceProvider serviceProvider)
                    where TInputParameters : class
                    where TCommandHandler : ICommandHandler<TInputParameters, TOutputDto>
        {
            var commandHandler = serviceProvider.GetRequiredService<TCommandHandler>();

            if (Directory.Exists(seedDataPath))
            {
                foreach (string filePath in Directory.EnumerateFiles(seedDataPath, glob, SearchOption.AllDirectories))
                {
                    List<TInputParameters>? data =
                        JsonConvert.DeserializeObject<List<TInputParameters>>(File.ReadAllText(filePath));
                    if (data is not null)
                    {
                        foreach (var entity in data)
                        {
                            _ = commandHandler.Handle(entity, CancellationToken.None).Result;
                        }
                    }
                }
            }
        }

        public static void LoadSeedData<TInputParameters, TOutputDto, TCommandHandler>(
            TInputParameters[] seedData,
            IServiceProvider serviceProvider)
            where TInputParameters : class
            where TCommandHandler : ICommandHandler<TInputParameters, TOutputDto>
        {
            ArgumentNullException.ThrowIfNull(seedData);

            var commandHandler = serviceProvider.GetRequiredService<TCommandHandler>();

            foreach (var entity in seedData)
            {
                _ = commandHandler.Handle(entity, CancellationToken.None).Result;
            }
        }

        public static void LoadSeedData<TInputParameters, TModuleDbContext>(
           string seedDataPath,
           string glob,
           IServiceProvider serviceProvider)
           where TInputParameters : BaseModel
           where TModuleDbContext : DbContext
        {
            var dbContext = serviceProvider.GetRequiredService<TModuleDbContext>();

            if (Directory.Exists(seedDataPath))
            {
                foreach (string filePath in Directory.EnumerateFiles(seedDataPath, glob, SearchOption.AllDirectories))
                {
                    List<TInputParameters>? data = JsonConvert.DeserializeObject<List<TInputParameters>>(File.ReadAllText(filePath));

                    if (data is not null)
                    {
                        dbContext.AddRange(data);
                        dbContext.SaveChanges();
                    }
                }
            }
        }
    }
}