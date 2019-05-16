using System.Collections.Generic;
using System.Linq;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public class DistribuiEpiService
    {
        private readonly webservicetwos3Entities _context;

        public DistribuiEpiService(webservicetwos3Entities context)
        {
            _context = context;
        }

        public DistribuicaoSuccessViewModel DistribuiEpi(int crachaId, string epiIdListString)
        {
            DistribuicaoSuccessViewModel viewModel = new DistribuicaoSuccessViewModel();
            var cracha = _context.L_ATRIBUICAOCRACHA.FirstOrDefault(c => c.ID == crachaId);
            var funcionario = _context.L_FUNCIONARIOS.FirstOrDefault(f => f.ID == cracha.FK_FUNCIONARIO);
            var listaEpiIds = epiIdListString.Split('|').ToList();

            List<L_PRODUTOS_ITENS> listaEpis = new List<L_PRODUTOS_ITENS>();

            foreach (string epiId in listaEpiIds)
            {
                var epi = _context.L_PRODUTOS_ITENS.FirstOrDefault(x => x.ID == int.Parse(epiId));

                if (epi != null)
                {
                    listaEpis.Add(epi);
                }
            }

            return viewModel;
        }
    }
}