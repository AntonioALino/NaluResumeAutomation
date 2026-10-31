namespace NaluResumeAutomation.Domain.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime createdAt { get; private set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            createdAt = DateTime.UtcNow; 
        }
    }
}