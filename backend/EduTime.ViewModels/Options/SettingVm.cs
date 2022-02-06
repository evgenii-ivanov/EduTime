namespace EduTime.ViewModels.Options
{
    public class SettingVm<TValue>
    {
        public string Name { get; set; }
        public TValue Value { get; set; }
    }

    public class SettingVm : SettingVm<object>
    {

    }
}
