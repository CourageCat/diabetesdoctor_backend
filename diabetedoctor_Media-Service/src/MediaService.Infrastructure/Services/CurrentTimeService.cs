using MediaService.Contract.Infrastructure.Services;

namespace MediaService.Infrastructure.Services;

public class CurrentTimeService : ICurrentTimeService
{
    public DateTime GetCurrentTime() => DateTime.UtcNow;
}
  