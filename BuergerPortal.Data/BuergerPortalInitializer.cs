using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BuergerPortal.Domain.Entities;
using BuergerPortal.Domain.Enums;

namespace BuergerPortal.Data
{
    public class BuergerPortalInitializer : CreateDatabaseIfNotExists<BuergerPortalContext>
    {
        protected override void Seed(BuergerPortalContext context)
        {
            SeedPublicOffices(context);
            SeedServiceTypes(context);
            SeedFeeSchedules(context);
            SeedCitizens(context);
            SeedSampleApplications(context);

            base.Seed(context);
        }

        private void SeedPublicOffices(BuergerPortalContext context)
        {
            var offices = new List<PublicOffice>
            {
                new PublicOffice
                {
                    OfficeName = "Buergeramt Berlin-Mitte",
                    DistrictCode = "BER-M",
                    StreetAddress = "Karl-Marx-Allee 31",
                    City = "Berlin",
                    PhoneNumber = "030-9018-0",
                    DistrictMultiplier = 1.15m
                },
                new PublicOffice
                {
                    OfficeName = "Kreisverwaltungsreferat Muenchen",
                    DistrictCode = "MUC-Z",
                    StreetAddress = "Ruppertstrasse 19",
                    City = "Munich",
                    PhoneNumber = "089-233-0",
                    DistrictMultiplier = 1.20m
                },
                new PublicOffice
                {
                    OfficeName = "Bezirksamt Hamburg-Mitte",
                    DistrictCode = "HAM-M",
                    StreetAddress = "Caffamacherreihe 1-3",
                    City = "Hamburg",
                    PhoneNumber = "040-4279-0",
                    DistrictMultiplier = 1.10m
                },
                new PublicOffice
                {
                    OfficeName = "Baurechtsamt Stuttgart",
                    DistrictCode = "STR-C",
                    StreetAddress = "Eberhardstrasse 33",
                    City = "Stuttgart",
                    PhoneNumber = "0711-216-0",
                    DistrictMultiplier = 1.05m
                },
                new PublicOffice
                {
                    OfficeName = "Standesamt Frankfurt",
                    DistrictCode = "FFM-C",
                    StreetAddress = "Roemer 1",
                    City = "Frankfurt",
                    PhoneNumber = "069-212-0",
                    DistrictMultiplier = 1.12m
                }
            };

            foreach (var office in offices)
            {
                if (!context.PublicOffices.Any(o => o.DistrictCode == office.DistrictCode))
                {
                    context.PublicOffices.Add(office);
                }
            }
            context.SaveChanges();
        }

        private void SeedServiceTypes(BuergerPortalContext context)
        {
            var services = new List<ServiceType>
            {
                new ServiceType
                {
                    ServiceName = "Residence Registration",
                    ServiceCode = "REG-001",
                    Category = ServiceCategory.Registration,
                    BaseFee = 12.00m,
                    ProcessingDays = 5,
                    Description = "Registration of new residence address (Anmeldung). Required within 14 days of moving.",
                    RequiresInPersonVisit = true
                },
                new ServiceType
                {
                    ServiceName = "Building Permit",
                    ServiceCode = "PER-001",
                    Category = ServiceCategory.Permit,
                    BaseFee = 250.00m,
                    ProcessingDays = 45,
                    Description = "Application for building permit (Baugenehmigung) for construction or major renovation projects.",
                    RequiresInPersonVisit = false
                },
                new ServiceType
                {
                    ServiceName = "Business License",
                    ServiceCode = "LIC-001",
                    Category = ServiceCategory.License,
                    BaseFee = 60.00m,
                    ProcessingDays = 10,
                    Description = "Trade license registration (Gewerbeanmeldung) for starting a new business.",
                    RequiresInPersonVisit = true
                },
                new ServiceType
                {
                    ServiceName = "ID Card Renewal",
                    ServiceCode = "DOC-001",
                    Category = ServiceCategory.Document,
                    BaseFee = 28.80m,
                    ProcessingDays = 21,
                    Description = "Renewal of national identity card (Personalausweis).",
                    RequiresInPersonVisit = true
                },
                new ServiceType
                {
                    ServiceName = "Marriage Certificate",
                    ServiceCode = "CRT-001",
                    Category = ServiceCategory.Certificate,
                    BaseFee = 80.00m,
                    ProcessingDays = 30,
                    Description = "Application for marriage certificate (Eheurkunde) at the civil registry office.",
                    RequiresInPersonVisit = true
                },
                new ServiceType
                {
                    ServiceName = "Parking Permit",
                    ServiceCode = "PER-002",
                    Category = ServiceCategory.Permit,
                    BaseFee = 30.60m,
                    ProcessingDays = 7,
                    Description = "Resident parking permit (Bewohnerparkausweis) for designated parking zones.",
                    RequiresInPersonVisit = false
                },
                new ServiceType
                {
                    ServiceName = "Birth Certificate Copy",
                    ServiceCode = "CRT-002",
                    Category = ServiceCategory.Certificate,
                    BaseFee = 12.00m,
                    ProcessingDays = 14,
                    Description = "Certified copy of birth certificate (Geburtsurkunde) from the civil registry.",
                    RequiresInPersonVisit = false
                }
            };

            foreach (var service in services)
            {
                if (!context.ServiceTypes.Any(s => s.ServiceCode == service.ServiceCode))
                {
                    context.ServiceTypes.Add(service);
                }
            }
            context.SaveChanges();
        }

        private void SeedFeeSchedules(BuergerPortalContext context)
        {
            var serviceTypes = context.ServiceTypes.ToList();
            string[] districtCodes = { "BER-M", "MUC-Z", "HAM-M", "STR-C", "FFM-C" };
            decimal[] adjustments = { 1.0m, 1.05m, 0.95m, 0.98m, 1.02m };

            var effectiveFrom = new DateTime(2014, 1, 1);

            foreach (var serviceType in serviceTypes)
            {
                for (int i = 0; i < districtCodes.Length; i++)
                {
                    string code = districtCodes[i];
                    if (!context.FeeSchedules.Any(f => f.ServiceTypeId == serviceType.ServiceTypeId && f.DistrictCode == code))
                    {
                        context.FeeSchedules.Add(new FeeSchedule
                        {
                            ServiceTypeId = serviceType.ServiceTypeId,
                            DistrictCode = code,
                            AdjustedBaseFee = serviceType.BaseFee * adjustments[i],
                            EffectiveFrom = effectiveFrom,
                            EffectiveTo = null
                        });
                    }
                }
            }
            context.SaveChanges();
        }

        private void SeedCitizens(BuergerPortalContext context)
        {
            var citizens = new List<Citizen>
            {
                new Citizen
                {
                    FirstName = "Anna",
                    LastName = "Schmidt",
                    DateOfBirth = new DateTime(1985, 3, 15),
                    StreetAddress = "Unter den Linden 17",
                    City = "Berlin",
                    PostalCode = "10117",
                    PhoneNumber = "030-1234-5678",
                    Email = "anna.schmidt@example.de",
                    TaxId = "12345678901",
                    IsLowIncome = false,
                    RegistrationDate = new DateTime(2014, 1, 15)
                },
                new Citizen
                {
                    FirstName = "Klaus",
                    LastName = "Weber",
                    DateOfBirth = new DateTime(1972, 8, 22),
                    StreetAddress = "Koenigstrasse 42",
                    City = "Stuttgart",
                    PostalCode = "70173",
                    PhoneNumber = "0711-9876-5432",
                    Email = "klaus.weber@example.de",
                    TaxId = "98765432109",
                    IsLowIncome = false,
                    RegistrationDate = new DateTime(2014, 2, 1)
                },
                new Citizen
                {
                    FirstName = "Petra",
                    LastName = "Mueller",
                    DateOfBirth = new DateTime(1990, 11, 8),
                    StreetAddress = "Marienplatz 8",
                    City = "Munich",
                    PostalCode = "80331",
                    PhoneNumber = "089-5555-1234",
                    Email = "petra.mueller@example.de",
                    TaxId = "55512349876",
                    IsLowIncome = true,
                    RegistrationDate = new DateTime(2014, 3, 10)
                },
                new Citizen
                {
                    FirstName = "Bernd",
                    LastName = "Fischer",
                    DateOfBirth = new DateTime(1968, 5, 30),
                    StreetAddress = "Hafenstrasse 12",
                    City = "Hamburg",
                    PostalCode = "20457",
                    PhoneNumber = "040-2222-3333",
                    Email = "bernd.fischer@example.de",
                    TaxId = "22233344455",
                    IsLowIncome = false,
                    RegistrationDate = new DateTime(2014, 1, 20)
                },
                new Citizen
                {
                    FirstName = "Sabine",
                    LastName = "Bauer",
                    DateOfBirth = new DateTime(1995, 1, 12),
                    StreetAddress = "Zeil 45",
                    City = "Frankfurt",
                    PostalCode = "60313",
                    PhoneNumber = "069-7777-8888",
                    Email = "sabine.bauer@example.de",
                    TaxId = "77788899900",
                    IsLowIncome = true,
                    RegistrationDate = new DateTime(2014, 4, 5)
                }
            };

            foreach (var citizen in citizens)
            {
                if (!context.Citizens.Any(c => c.TaxId == citizen.TaxId))
                {
                    context.Citizens.Add(citizen);
                }
            }
            context.SaveChanges();
        }

        private void SeedSampleApplications(BuergerPortalContext context)
        {
            if (context.ServiceApplications.Any())
            {
                return;
            }

            var anna = context.Citizens.Local.FirstOrDefault(c => c.TaxId == "12345678901")
                       ?? context.Citizens.FirstOrDefault(c => c.TaxId == "12345678901");
            var klaus = context.Citizens.Local.FirstOrDefault(c => c.TaxId == "98765432109")
                        ?? context.Citizens.FirstOrDefault(c => c.TaxId == "98765432109");

            var regService = context.ServiceTypes.Local.FirstOrDefault(s => s.ServiceCode == "REG-001")
                             ?? context.ServiceTypes.FirstOrDefault(s => s.ServiceCode == "REG-001");
            var buildService = context.ServiceTypes.Local.FirstOrDefault(s => s.ServiceCode == "PER-001")
                               ?? context.ServiceTypes.FirstOrDefault(s => s.ServiceCode == "PER-001");

            var berlinOffice = context.PublicOffices.Local.FirstOrDefault(o => o.DistrictCode == "BER-M")
                               ?? context.PublicOffices.FirstOrDefault(o => o.DistrictCode == "BER-M");
            var stuttgartOffice = context.PublicOffices.Local.FirstOrDefault(o => o.DistrictCode == "STR-C")
                                  ?? context.PublicOffices.FirstOrDefault(o => o.DistrictCode == "STR-C");

            if (anna != null && regService != null && berlinOffice != null)
            {
                var app1 = new ServiceApplication
                {
                    ApplicationNumber = "APP-2014-0001",
                    CitizenId = anna.CitizenId,
                    ServiceTypeId = regService.ServiceTypeId,
                    OfficeId = berlinOffice.OfficeId,
                    Status = ApplicationStatus.Approved,
                    SubmissionDate = new DateTime(2014, 1, 20),
                    ReviewDate = new DateTime(2014, 1, 22),
                    CompletionDate = new DateTime(2014, 1, 25),
                    IsExpressProcessing = false,
                    CalculatedFee = 16.42m,
                    Notes = "Standard residence registration for Berlin-Mitte"
                };
                context.ServiceApplications.Add(app1);
            }

            if (klaus != null && buildService != null && stuttgartOffice != null)
            {
                var app2 = new ServiceApplication
                {
                    ApplicationNumber = "APP-2014-0002",
                    CitizenId = klaus.CitizenId,
                    ServiceTypeId = buildService.ServiceTypeId,
                    OfficeId = stuttgartOffice.OfficeId,
                    Status = ApplicationStatus.UnderReview,
                    SubmissionDate = new DateTime(2014, 2, 10),
                    ReviewDate = null,
                    CompletionDate = null,
                    IsExpressProcessing = true,
                    CalculatedFee = 468.56m,
                    Notes = "Building permit for garage extension - express processing requested"
                };
                context.ServiceApplications.Add(app2);
            }

            context.SaveChanges();
        }
    }
}
