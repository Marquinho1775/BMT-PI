using BMT_backend.Domain.Entities;

namespace BMT_backend.Presentation.DTOs
{
    public class EnterpriseDevDto
    {
        public Enterprise Enterprise { get; set; }
        public int ProductQuantity { get; set; }
        public int EmployeeQuantity { get; set; }
    }
}
