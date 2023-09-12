namespace Bazaar.ShopperInfo.Core.Model
{
    public class Shopper
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        NonBinary,
    }
}
