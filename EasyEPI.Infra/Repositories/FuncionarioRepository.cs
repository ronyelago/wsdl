using EasyEPI.Infra.Data;
using EasyEPI.Infra.Entities;
using EasyEPI.Infra.Interfaces;

namespace EasyEPI.Infra.Repositories
{
    public class FuncionarioRepository : Repository<L_FUNCIONARIOS>, IFuncionarioRepository
    {
        public FuncionarioRepository(EasyEpiContext context) : base (context)
        {
            Context = context;
        }
    }
}
