using System.Text.Json;
using ContactManager.App.Models;

namespace ContactManager.App.Persistence;

public class JsonContactRepository : IContactRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    private List<Contact> _contacts = new();

    public JsonContactRepository(string filePath)
    {
        _filePath = filePath;
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        if (!File.Exists(_filePath))
        {
            _contacts = new List<Contact>();
            return;
        }

        var json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            _contacts = new List<Contact>();
            return;
        }

        var loaded = JsonSerializer.Deserialize<List<Contact>>(json);
        _contacts = loaded ?? new List<Contact>();
    }

    private void SaveToFile()
    {
        var json = JsonSerializer.Serialize(_contacts, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }

    public IEnumerable<Contact> GetAll() => _contacts;

    public Contact? GetById(Guid id) =>
        _contacts.FirstOrDefault(c => c.Id == id);

    public void Add(Contact contact)
    {
        _contacts.Add(contact);
        SaveToFile();
    }

    public void Update(Contact contact)
    {
        var existing = GetById(contact.Id);
        if (existing is null) return;

        existing.Name = contact.Name;
        existing.Company = contact.Company;
        existing.Phone = contact.Phone;
        existing.Email = contact.Email;
        existing.Address = contact.Address;

        SaveToFile();
    }

    public void Delete(Guid id)
    {
        var existing = GetById(id);
        if (existing is null) return;

        _contacts.Remove(existing);
        SaveToFile();
    }

    public IEnumerable<Contact> FindByName(string namePart)
    {
        if (string.IsNullOrWhiteSpace(namePart))
            return Enumerable.Empty<Contact>();

        return _contacts
            .Where(c => c.Name.Contains(namePart, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Contact> FindByFilter(string? name = null, string? company = null)
    {
        IEnumerable<Contact> query = _contacts;

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
}
