
namespace Talabat.Core.Entities.Identity
{
    public class Address 
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        // Type Of Id Of IdentityUser Which App User Inherit From It Is String 
        public string AppUserId { get; set; } // FK

        public AppUser AppUser { get; set; } // [ONE]
    }
}