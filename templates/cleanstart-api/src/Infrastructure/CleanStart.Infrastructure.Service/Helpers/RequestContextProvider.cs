using CleanStart.Service.Abstractions.Helpers;

namespace CleanStart.Infrastructure.Service.Helpers;

public class RequestContextProvider : IRequestContextProvider
{
    private RequestContext? _context;

    public RequestContext Context
    {
        get
        {
            if (_context is null) throw new Exception($"RequestContextProvider: {nameof(_context)} is null");

            return _context;
        }
        private set { _context = value; }
    }

    public void SetContext(RequestContext context)
    {
        _context = context;
    }
}