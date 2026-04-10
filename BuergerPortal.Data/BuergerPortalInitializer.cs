using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Data
{
    /// <summary>
    /// Seeds initial data into the database. Call SeedAsync after EnsureCreated in application startup.
    /// </summary>
    public static class BuergerPortalSeeder
    {
        public static async Task SeedAsync(BuergerPortalContext context)
        {
            await SeedPublicOfficesAsync(context);
            await SeedServiceTypesAsync(context);
            await SeedFeeSchedulesAsync(context);
            await SeedCitizensAsync(context);
        }

        private static async Task SeedPublicOfficesAsync(BuergerPortalContext context)
        {
            if (context.PublicOffices.Any()) return;

            var offices = new List<PublicOffice>
            {
                new PublicOffice { OfficeName = "Buergeramt Berlin-Mitte", DistrictCode = "BER-M", StreetAddress = "Karl-Marx-Allee 31", City = "Berlin", PhoneNumber = "030-9018-0", DistrictMultiplier = 1.15m },
                new PublicOffice { OfficeName = "Kreisverwaltungsreferat Muenchen", DistrictCode = "MUC-Z", StreetAddress = "Ruppertstrasse 19", City = "Munich", PhoneNumber = "089-233-0", DistrictMultiplier = 1.20m },
                new PublicOffice { OfficeName = "Bezirksamt Hamburg-Mitte", DistrictCode = "HAM-M", StreetAddress = "Caffamacherreihe 1-3", City = "Hamburg", PhoneNumber = "040-4279-0", DistrictMultiplier = 1.10m },
                new PublicOffice { OfficeName = "Baurechtsamt Stuttgart", DistrictCode = "STR-C", StreetAddress = "Eberhardstrasse 33", City = "Stuttgart", PhoneNumber = "0711-216-0", DistrictMultiplier = 1.05m },
                new PublicOffice { OfficeName = "Standesamt Frankfurt", DistrictCode = "FFM-C", StreetAddress = "Roemer 1", City = "Frankfurt", PhoneNumber = "069-212-0", DistrictMultiplier = 1.12m }
            };

            context.PublicOffices.AddRange(offices);
            await context.SaveChangesAsync();
        }

        private static async Task SeedServiceTypesAsync(BuergerPortalContext context)
        {
            if (context.ServiceTypes.Any()) return;

            var services = new List<ServiceType>
            {
                new ServiceType { ServiceName = "Residence Registration", ServiceCode = "REG-001", Category = ServiceCategory.Registration, BaseFee = 12.00m, ProcessingDays = 5, Description = "Registration of new residence address (Anmeldung). Required within 14 days of moving.", RequiresInPersonVisit = true },
                new ServiceType { ServiceName = "Building Permit", ServiceCode = "PER-001", Category = ServiceCategory.Permit, BaseFee = 250.00m, ProcessingDays = 45, Description = "Application for building permit (Baugenehmigung) for construction or major renovation projects.", RequiresInPersonVisit = false },
                new ServiceType { ServiceName = "Business License", ServiceCode = "LIC-001", Category = ServiceCategory.License, BaseFee = 60.00m, ProcessingDays = 10, Description = "Trade license registration (Gewerbeanmeldung) for starting a new business.", RequiresInPersonVisit = true },
                new ServiceType { ServiceName = "ID Card Renewal", ServiceCode = "DOC-001", Category = ServiceCategory.Document, BaseFee = 28.80m, ProcessingDays = 21, Description = "Renewal of national identity card (Personalausweis).", RequiresInPersonVisit = true },
                new ServiceType { ServiceName = "Marriage Certificate", ServiceCode = "CRT-001", Category = ServiceCategory.Certificate, BaseFee = 80.00m, ProcessingDays = 30, Description = "Application for marriage certificate (Eheurkunde) at the civil registry office.", RequiresInPersonVisit = true },
                new ServiceType { ServiceName = "Parking Permit", ServiceCode = "PER-002", Category = ServiceCategory.Permit, BaseFee = 30.60m, ProcessingDays = 7, Description = "Resident parking permit (Bewohnerparkausweis) for designated parking zones.", RequiresInPersonVisit = false },
                new ServiceType { ServiceName = "Birth Certificate Copy", ServiceCode = "CRT-002", Category = ServiceCategory.Certificate, BaseFee = 12.00m, ProcessingDays = 14, Description = "Certified copy of birth certificate (Geburtsurkunde) from the civil registry.", RequiresInPersonVisit = false }
            };

            context.ServiceTypes.AddRange(services);
            await context.SaveChangesAsync();
        }

        private static async Task SeedFeeSchedulesAsync(BuergerPortalContext context)
        {
            if (context.FeeSchedules.Any()) return;

            var serviceTypes = context.ServiceTypes.ToList();
            string[] districtCodes = { "BER-M", "MUC-Z", "HAM-M", "STR-C", "FFM-C" };
            decimal[] adjustments = { 1.0m, 1.05m, 0.95m, 0.98m, 1.02m };
            var effectiveFrom = new DateTime(2014, 1, 1);

            var schedules = new List<FeeSchedule>();
            foreach (var serviceType in serviceTypes)
            {
                for (int i = 0; i < districtCodes.Length; i++)
                {
                    schedules.Add(new FeeSchedule
                    {
                        ServiceTypeId = serviceType.ServiceTypeId,
                        DistrictCode = districtCodes[i],
                        AdjustedBaseFee = serviceType.BaseFee * adjustments[i],
                        EffectiveFrom = effectiveFrom,
                        EffectiveTo = null
                    });
                }
            }

            context.FeeSchedules.AddRange(schedules);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCitizensAsync(BuergerPortalContext context)
        {
            if (context.Citizens.Any()) return;

            var citizens = new List<Citizen>
            {
                new Citizen { FirstName = "Anna", LastName = "Schmidt", DateOfBirth = new DateTime(1985, 3, 15), StreetAddress = "Unter den Linden 17", City = "Berlin", PostalCode = "10117", PhoneNumber = "030-1234-5678", Email = "anna.schmidt@example.de", TaxId = "12345678901", IsLowIncome = false, RegistrationDate = new DateTime(2014, 1, 15) },
                new Citizen { FirstName = "Klaus", LastName = "Weber", DateOfBirth = new DateTime(1972, 8, 22), StreetAddress = "Koenigstrasse 42", City = "Stuttgart", PostalCode = "70173", PhoneNumber = "0711-9876-5432", Email = "klaus.weber@example.de", TaxId = "98765432109", IsLowIncome = false, RegistrationDate = new DateTime(2014, 2, 1) },
                new Citizen { FirstName = "Petra", LastName = "Mueller", DateOfBirth = new DateTime(1990, 11, 8), StreetAddress = "Marienplatz 8", City = "Munich", PostalCode = "80331", PhoneNumber = "089-5555-1234", Email = "petra.mueller@example.de", TaxId = "55512349876", IsLowIncome = true, RegistrationDate = new DateTime(2014, 3, 10) },
                new Citizen { FirstName = "Bernd", LastName = "Fischer", DateOfBirth = new DateTime(1968, 5, 30), StreetAddress = "Hafenstrasse 12", City = "Hamburg", PostalCode = "20459", PhoneNumber = "040-7777-9999", Email = "bernd.fischer@example.de", TaxId = "77744455566", IsLowIncome = false, RegistrationDate = new DateTime(2014, 4, 5) }
            };

            context.Citizens.AddRange(citizens);
            await context.SaveChangesAsync();
        }
    }
}
