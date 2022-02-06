using EduTime.Domain.Infrastructure;

namespace EduTime.Domain.Entities
{
    public class Setting : BaseGuidEntity
    {
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public string Value { get; set; }
    }
}
