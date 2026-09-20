using System.Globalization;
using ConstructionApi.Models;

namespace ConstructionApi.Data;

public static class DbInitializer
{
    public static void SeedPriceLists(AppDbContext db, string csvPath)
    {
        if (db.PriceLists.Any()) return;

        var lines = File.ReadAllLines(csvPath);
        var records = lines.Skip(1).Select(line =>
        {
            var fields = line.Split(',');
            return new PriceList
            {
                CatalogCode = fields[0],
                CatalogDescription = fields[1],
                CatalogAuthor = fields[2],
                CatalogVersion = fields[3],
                Code = fields[4],
                Description = fields[5],
                Category = fields[6],
                Subcategory = fields[7],
                UnitOfMeasure = fields[8],
                Price = decimal.Parse(fields[9], CultureInfo.InvariantCulture),
                SafetyPercentage = decimal.Parse(fields[10], CultureInfo.InvariantCulture),
                LabourPercentage = decimal.Parse(fields[11], CultureInfo.InvariantCulture),
                MaterialPercentage = decimal.Parse(fields[12], CultureInfo.InvariantCulture),
                EquipmentPercentage = decimal.Parse(fields[13], CultureInfo.InvariantCulture)
            };
        });

        db.PriceLists.AddRange(records);
        db.SaveChanges();
    }

    public static void SeedPhases(AppDbContext db, string csvPath)
    {
        if (db.Phases.Any()) return;

        var lines = File.ReadAllLines(csvPath);
        var records = lines.Skip(1).Select(line =>
        {
            var fields = line.Split(',');
            return new Phase
            {
                Name = fields[0],
                Category = fields[1]
            };
        });

        db.Phases.AddRange(records);
        db.SaveChanges();
    }

    public static void SeedAssemblies(AppDbContext db, string assembliesCsvPath, string materialsCsvPath)
    {
        if (db.Assemblies.Any()) return;

        var phaseByName = db.Phases.ToDictionary(p => p.Name);
        var priceListByCode = db.PriceLists.ToDictionary(p => p.Code);

        var assemblyByName = File.ReadAllLines(assembliesCsvPath).Skip(1).Select(line =>
        {
            var fields = line.Split(',');
            return new Assembly
            {
                Name = fields[0],
                Category = fields[1]
            };
        }).ToDictionary(a => a.Name);

        db.Assemblies.AddRange(assemblyByName.Values);

        foreach (var line in File.ReadAllLines(assembliesCsvPath).Skip(1))
        {
            var fields = line.Split(',');
            db.PhaseAssemblyLists.Add(new PhaseAssemblyList
            {
                Phase = phaseByName[fields[2]],
                Assembly = assemblyByName[fields[0]],
                Quantity = 1
            });
        }

        foreach (var line in File.ReadAllLines(materialsCsvPath).Skip(1))
        {
            var fields = line.Split(',');
            var assemblyName = fields[14];
            if (string.IsNullOrWhiteSpace(assemblyName)) continue;

            assemblyByName[assemblyName].AssemblyPriceListItems.Add(new AssemblyPriceList
            {
                Assembly = assemblyByName[assemblyName],
                PriceList = priceListByCode[fields[4]],
                Type = Enum.Parse<AssemblyItemType>(fields[15]),
                Value = decimal.Parse(fields[16], CultureInfo.InvariantCulture),
                IsDriver = bool.Parse(fields[17])
            });
        }

        db.SaveChanges();
    }
}
