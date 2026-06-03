using System;
using System.Collections.Generic;
using System.Text;

namespace NaluResumeAutomation.Application.Common.Interfaces
{
    public interface IUseCase<in TRequest, TResponse>
    {
        Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }
}
