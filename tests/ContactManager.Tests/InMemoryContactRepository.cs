using ContactManager.App.Models;
using ContactManager.App.Persistence;

namespace ContactManager.Tests;

/// <summary>
/// In-Memory Implementierung des IContactRepository für Testzwecke
/// </summary>
public class InMemoryContactRepository : IContactRepository
{
    private readonly List<Contact> _contacts = new();

    public void Add(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        _contacts.Add(contact);
    }

    public void Delete(Guid id)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == id);
        if (contact != null)
        {
            _contacts.Remove(contact);
        }
    }

    public IEnumerable<Contact> FindByFilter(string? name = null, string? company = null)
    {
        var query = _contacts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            query = query.Where(c => c.Company.Contains(company, StringComparison.OrdinalIgnoreCase));
        }

        return query;
    }

    public IEnumerable<Contact> FindByName(string namePart)
    {
        return _contacts.Where(c => c.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Contact> GetAll()
    {
        return _contacts.ToList();
    }

    public Contact? GetById(Guid id)
    {
        return _contacts.FirstOrDefault(c => c.Id == id);
    }

    public void Update(Contact contact)
    {
        if (contact == null)
            throw new ArgumentNullException(nameof(contact));

        var existing = _contacts.FirstOrDefault(c => c.Id == contact.Id);
        if (existing == null)
            throw new InvalidOperationException($"Contact with id {contact.Id} not found");

        // Ersetze das existierende Objekt
        var index = _contacts.IndexOf(existing);
        _contacts[index] = contact;
    }
}
