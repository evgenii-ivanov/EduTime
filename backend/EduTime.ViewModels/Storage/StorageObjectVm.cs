namespace EduTime.ViewModels.Storage
{
    public class StorageObjectVm : BaseVm
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public string Path {  get; set; }
        public string Url {  get; set; }
    }
}
