using EduTime.Domain.Infrastructure;

namespace EduTime.Domain.Entities.FileStorage
{
    public class StorageObject : BaseGuidEntity
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
