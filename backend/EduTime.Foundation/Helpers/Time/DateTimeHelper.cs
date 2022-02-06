using System;

namespace EduTime.Foundation.Helpers.Time
{
    public interface IDateTimeHelper
    {
        DateTime UtcNow { get; }
    }


    public class DateTimeHelper : IDateTimeHelper
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }

    public class FakeDateTimeHelper : IDateTimeHelper
    {
        public FakeDateTimeHelper(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
