using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.DTOs
{
    public class EnterpriseDevDto
    {
        public string Administrator { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public int EmployeeQuantity { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int ProductQuantity { get; set; }
    }
}
