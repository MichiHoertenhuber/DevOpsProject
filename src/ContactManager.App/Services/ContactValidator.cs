using System.Text.RegularExpressions;
using ContactManager.App.Models;

namespace ContactManager.App.Services;

public static class ContactValidator
{
    public static string Validate(Contact contact)
    {
        if (string.IsNullOrWhiteSpace(contact.Name))
            return "Name darf nicht leer sein.";

        if (string.IsNullOrWhiteSpace(contact.Phone))
            return "Phone darf nicht leer sein.";

        if (!IsValidEmail(contact.Email))
            return "Ungültige Email-Adresse.";

        return ""; // OK
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        if (email.Contains(" ")) return false;
        if (!email.Contains("@") || !email.Contains(".")) return false;

        return true;
    }
}
