using XmlDataProcessor.Domain.Entities;

namespace XmlDataProcessor.Application.Abstractions.Repositories;

public interface IMovimentoRepository
{
    Task<bool> ExistePorIdExternoAsync(string idExterno);

    Task AdicionarAsync(Movimento movimento);
}