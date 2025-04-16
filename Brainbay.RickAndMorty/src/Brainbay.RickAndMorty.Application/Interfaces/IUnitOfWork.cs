namespace Brainbay.RickAndMorty.Application.Interfaces;


public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
