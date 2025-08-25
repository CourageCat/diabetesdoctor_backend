namespace UserService.Contract.Infrastructure;

public interface IAiService
{
    Task<string?> GenerateContentAsync(string prompt, CancellationToken cancellationToken);
}