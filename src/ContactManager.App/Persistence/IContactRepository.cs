using ContactManager.App.Models;

namespace ContactManager.App.Persistence;

public interface IContactRepository
{
    IEnumerable<Contact> GetAll();
    Contact? GetById(Guid id);
    void Add(Contact contact);
    void Update(Contact contact);
    void Delete(Guid id);
    IEnumerable<Contact> FindByName(string namePart);
    IEnumerable<Contact> FindByFilter(string? name = null, string? company = null);
}
