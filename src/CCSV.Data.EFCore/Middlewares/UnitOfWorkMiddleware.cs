using CCSV.Domain.Repositories;
using Microsoft.AspNetCore.Http;

namespace CCSV.Data.EFCore.Middlewares;

public class UnitOfWorkMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context, IUnitOfWork unitOfWork)
    {
        await _next(context);
        unitOfWork.UpdateEditDateTimes();
        await unitOfWork.SaveAsync();
    }
}
