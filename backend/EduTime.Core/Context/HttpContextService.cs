using EduTime.Foundation.Context;
using Microsoft.AspNetCore.Http;

namespace EduTime.Core.Context
{
	public class HttpContextService : IContextService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IExecutionContext GetContext()
        {
            var context = new HttpExecutionContext();

            // TODO: implement your logic of context resolution here.
            // You may use _httpContextAccessor.

            return context;
        }
    }
}
