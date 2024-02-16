using Hl7.Fhir.Model;

namespace GPMigratorApp.DTOs;

public class ContactDTO
{
    public ContactDTO()
    {
    }

    public ContactDTO(Organization.ContactComponent contact)
    {
        if (contact != null)
        {
            Title = contact.Name.Prefix.ToString();
            GivenName = contact.Name.GivenElement.FirstOrDefault().Value;
            MiddleNames = contact.Name.GivenElement.Skip(1).Select(x => x.Value).ToString();
            Surname = contact.Name.Family;
            Address = new AddressDTO(contact.Address);
        }

    }
    
    public Guid Id { get; set; }
    public string? Title  { get; set; }
    public string GivenName  { get; set; }
    public string MiddleNames  { get; set; }
    public string Surname { get; set; }
    public AddressDTO? Address { get; set; }
}