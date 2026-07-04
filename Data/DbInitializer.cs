using System.Globalization;
using ConstructionApi.Models;

namespace ConstructionApi.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db, string csvPath)
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
}
