namespace VCardQRCodeGenerator.Models
{
    public class VCard
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }

        public string ToVCardFormat()
        {
            return $"BEGIN:VCARD\n" +
                   $"VERSION:3.0\n" +
                   $"FN:{Firstname} {Surname}\n" +
                   $"EMAIL:{Email}\n" +
                   $"TEL:{Phone}\n" +
                   $"ADR:;;{City};{Country};;;\n" +
                   $"END:VCARD";
        }
    }
}
