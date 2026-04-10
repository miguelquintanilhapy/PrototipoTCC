using SAD.Models;

namespace SAD.Services
{
    /// <summary>
    /// Contrato para o serviço de geração de PDF.
    /// Desacoplado para permitir troca de biblioteca sem alterar o ViewModel.
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Gera um PDF da proposta e salva no caminho especificado.
        /// </summary>
        /// <param name="proposta">Dados da proposta a serem renderizados</param>
        /// <param name="caminhoArquivo">Caminho completo do arquivo de saída (.pdf)</param>
        void GerarPdf(Proposta proposta, string caminhoArquivo);
    }
}
