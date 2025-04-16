using Brainbay.RickAndMorty.Application.Interfaces;

namespace Brainbay.RickAndMorty.Infrastructure.Repositories;


public class EfUnitOfWork : IUnitOfWork
{
    private readonly CharacterDbContext _context;
    
    public EfUnitOfWork(CharacterDbContext context) => _context = context;

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
