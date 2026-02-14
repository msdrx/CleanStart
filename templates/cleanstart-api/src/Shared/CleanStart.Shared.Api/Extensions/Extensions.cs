using System;
using CleanStart.Shared.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace CleanStart.Shared.Api.Extensions;

public static class Exceptions
{
    public static IMvcBuilder ConfigureBadRequest(this IMvcBuilder mvcBuilder)
    {
        return mvcBuilder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                return new BadRequestObjectResult(BadRequest(context.ModelState));
            };
        });
    }

    private static IApiResponse BadRequest(ModelStateDictionary modelState)
    {
        var messages = modelState?.Where(x => x.Value != null)
                                 ?.SelectMany(x => x.Value!.Errors.Select(e => new KeyValuePair<string, string?>(x.Key, e.ErrorMessage)));

        return ApiResponse.BadRequest(messages);
    }
}
