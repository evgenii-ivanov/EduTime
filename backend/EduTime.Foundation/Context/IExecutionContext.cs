using System.Globalization;

namespace EduTime.Foundation.Context
{
    public interface IExecutionContext
    {
	    CultureInfo Culture { get; }
    }
}
