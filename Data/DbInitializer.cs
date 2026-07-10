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

    public static void SeedAssemblies(AppDbContext db, string csvPath)
    {
        if (db.Assemblies.Any()) return;

        var priceListByCode = db.PriceLists.ToDictionary(p => p.Code);

        var lines = File.ReadAllLines(csvPath);
        var rows = lines.Skip(1).Select(line =>
        {
            var fields = line.Split(',');
            return new
            {
                Name = fields[0],
                Category = fields[1],
                PriceListCode = fields[2],
                Type = Enum.Parse<AssemblyItemType>(fields[3]),
                Value = decimal.Parse(fields[4], CultureInfo.InvariantCulture),
                IsDriver = bool.Parse(fields[5])
            };
        }).ToList();

        var groups = rows.GroupBy(r => new { r.Name, r.Category });

        foreach (var group in groups)
        {
            var assembly = new Assembly
            {
                Name = group.Key.Name,
                Category = group.Key.Category
            };

            foreach (var row in group)
            {
                assembly.AssemblyPriceListItems.Add(new AssemblyPriceList
                {
                    Assembly = assembly,
                    PriceList = priceListByCode[row.PriceListCode],
                    Type = row.Type,
                    Value = row.Value,
                    IsDriver = row.IsDriver
                });
            }

            db.Assemblies.Add(assembly);
        }

        db.SaveChanges();
    }
}
