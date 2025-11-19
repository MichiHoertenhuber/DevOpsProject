﻿using ContactManager.App.Models;
using ContactManager.App.Persistence;

const string dataFile = "contacts.json";

IContactRepository repository = new JsonContactRepository(dataFile);

if (!repository.GetAll().Any())
{
    var demo = new Contact
    {
        Name = "Max Mustermann",
        Company = "Example GmbH",
        Phone = "0123456789",
        Email = "max@example.com",
        Address = "Musterstraße 1, 1234 Musterstadt"
    };

    repository.Add(demo);
    Console.WriteLine("Demo-Kontakt wurde angelegt.");
}
else
{
    Console.WriteLine("Vorhandene Kontakte:");
}

foreach (var c in repository.GetAll())
{
    Console.WriteLine($"- {c.Name} ({c.Email})");
}

Console.WriteLine("\nProgrammende (Menü kommt später).");
