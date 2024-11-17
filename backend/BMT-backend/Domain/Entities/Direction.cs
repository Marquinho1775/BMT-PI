namespace BMT_backend.Domain.Entities
{
    public class Direction
    {
        public string? Id { get; set; }
        public string UserId { get; set; }
        public string NumDirection { get; set; }
        public string? OtherSigns { get; set; }
        public string Coordinates { get; set; }
        public bool IsSoftDeleted { get; set; }
    }
}