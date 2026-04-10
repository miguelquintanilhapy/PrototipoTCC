using SAD.Models;

namespace SAD.Services
{
    public interface IPdfService
    {
        void GerarPdf(Proposta proposta, string caminhoArquivo);
    }
}