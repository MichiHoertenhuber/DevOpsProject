using ContactManager.App.Models;
using ContactManager.App.Services;
using ContactManager.App.Persistence;

namespace ContactManager.Tests;

// ==================== UNIT TESTS ====================
public class ValidatorTests
{
    [Fact]
    public void ValidateEmail_WithValidEmail_ReturnsEmpty()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ValidateEmail_WithInvalidEmail_ReturnsError()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "invalid-email", 
            Phone = "123456789" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("Ungültige Email-Adresse.", result);
    }

    [Fact]
    public void ValidateEmail_WithEmailWithSpace_ReturnsError()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john @example.com", 
            Phone = "123456789" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("Ungültige Email-Adresse.", result);
    }

    [Fact]
    public void ValidatePhone_WithValidPhone_ReturnsEmpty()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void ValidatePhone_WithEmptyPhone_ReturnsError()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("Phone darf nicht leer sein.", result);
    }

    [Fact]
    public void Validate_WithValidData_ReturnsEmpty()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789",
            Company = "ACME Corp",
            Address = "123 Main St"
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void Validate_WithMissingName_ReturnsError()
    {
        // Arrange
        var contact = new Contact 
        { 
            Name = "", 
            Email = "john@example.com", 
            Phone = "123456789" 
        };

        // Act
        var result = ContactValidator.Validate(contact);

        // Assert
        Assert.Equal("Name darf nicht leer sein.", result);
    }
}

// ==================== INTEGRATION TESTS ====================
public class ContactRepositoryIntegrationTests
{
    private InMemoryContactRepository GetRepository()
    {
        return new InMemoryContactRepository();
    }

    [Fact]
    public void AddContact_WithValidData_AddsContactToRepository()
    {
        // Arrange
        var repo = GetRepository();
        var contact = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789",
            Company = "ACME Corp"
        };

        // Act
        repo.Add(contact);
        var result = repo.GetAll().ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("John Doe", result[0].Name);
    }

    [Fact]
    public void AddContact_WithDuplicate_AddsMultipleContacts()
    {
        // Arrange
        var repo = GetRepository();
        var contact1 = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789" 
        };
        var contact2 = new Contact 
        { 
            Name = "John Doe", 
            Email = "john@example.com", 
            Phone = "123456789" 
        };

        // Act
        repo.Add(contact1);
        repo.Add(contact2);
        var result = repo.GetAll().ToList();

        // Assert
        Assert.Equal(2, result.Count);
        // Note: Die beiden Kontakte haben unterschiedliche IDs
        Assert.NotEqual(contact1.Id, contact2.Id);
    }

    [Fact]
    public void AddContact_GetContacts_PersistsInMemory()
    {
        // Arrange
        var repo = GetRepository();
        var contact1 = new Contact { Name = "Alice", Email = "alice@example.com", Phone = "111111" };
        var contact2 = new Contact { Name = "Bob", Email = "bob@example.com", Phone = "222222" };

        // Act
        repo.Add(contact1);
        repo.Add(contact2);
        var allContacts = repo.GetAll().ToList();

        // Assert
        Assert.Equal(2, allContacts.Count);
        Assert.Contains(allContacts, c => c.Name == "Alice");
        Assert.Contains(allContacts, c => c.Name == "Bob");
    }

    [Fact]
    public void UpdateContact_ModifiesExistingContact()
    {
        // Arrange
        var repo = GetRepository();
        var contact = new Contact { Name = "John Doe", Email = "john@example.com", Phone = "123456789" };
        repo.Add(contact);
        
        var contactToUpdate = repo.GetById(contact.Id)!;
        contactToUpdate.Name = "Jane Doe";
        contactToUpdate.Email = "jane@example.com";

        // Act
        repo.Update(contactToUpdate);
        var updated = repo.GetById(contact.Id);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Jane Doe", updated.Name);
        Assert.Equal("jane@example.com", updated.Email);
    }

    [Fact]
    public void FindByFilter_SearchByName()
    {
        // Arrange
        var repo = GetRepository();
        repo.Add(new Contact { Name = "John Doe", Email = "john@example.com", Phone = "111111" });
        repo.Add(new Contact { Name = "Jane Smith", Email = "jane@example.com", Phone = "222222" });
        repo.Add(new Contact { Name = "Bob Johnson", Email = "bob@example.com", Phone = "333333" });

        // Act
        var results = repo.FindByFilter(name: "John").ToList();

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, c => c.Name == "John Doe");
        Assert.Contains(results, c => c.Name == "Bob Johnson");
    }

    [Fact]
    public void FindByFilter_SearchByCompany()
    {
        // Arrange
        var repo = GetRepository();
        repo.Add(new Contact { Name = "John Doe", Company = "ACME Corp", Email = "john@example.com", Phone = "111111" });
        repo.Add(new Contact { Name = "Jane Smith", Company = "Tech Inc", Email = "jane@example.com", Phone = "222222" });

        // Act
        var results = repo.FindByFilter(company: "ACME").ToList();

        // Assert
        Assert.Single(results);
        Assert.Equal("John Doe", results[0].Name);
    }

    [Fact]
    public void SearchAndUpdate_Integration()
    {
        // Arrange
        var repo = GetRepository();
        repo.Add(new Contact { Name = "John Doe", Email = "john@example.com", Phone = "123456789", Company = "Old Corp" });
        repo.Add(new Contact { Name = "Jane Smith", Email = "jane@example.com", Phone = "987654321", Company = "New Corp" });

        // Act - Suche einen Kontakt
        var found = repo.FindByFilter(name: "John").FirstOrDefault();
        
        // Act - Aktualisiere ihn
        found!.Company = "Updated Corp";
        repo.Update(found);

        // Assert
        var updated = repo.GetById(found.Id);
        Assert.Equal("Updated Corp", updated!.Company);
    }
}