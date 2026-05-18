using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using magal.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace magal.Services
{
    /// <summary>
    /// Serviço responsável pela geração e exportação de documentos em formato PDF no sistema.
    /// </summary>
    public class PdfService
    {
        #region Atributos e Campos Privados

        /// <summary>
        /// Cultura padrão utilizada para a formatação de valores monetários e datas em formato PT-BR.
        /// </summary>
        private static readonly CultureInfo _ptBR = new("pt-BR");

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Gera e salva o arquivo de Proposta Técnica Comercial em PDF com base nos dados do projeto e custos extras informados.
        /// </summary>
        /// <param name="projeto">A instância do <see cref="Projeto"/> contendo os dados do cliente, orçamento e tarefas.</param>
        /// <param name="custosExtras">Lista de custos adicionais com equipamentos ou licenças para integrar ao relatório.</param>
        /// <param name="caminhoArquivo">O caminho físico do diretório onde o arquivo .pdf será salvo.</param>
        public void GerarPropostaTecnica(Projeto projeto, List<Custo> custosExtras, string caminhoArquivo)
        {
            // Define o tipo de licença gratuita do QuestPDF exigido para execução do motor de renderização
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Configurações Globais da Página
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // Montagem das Seções do Documento utilizando Métodos Auxiliares
                    page.Header().Column(col => ConstruirCabecalho(col, projeto));

                    page.Content().PaddingTop(16).Column(col => ConstruirConteudoPrincipal(col, projeto, custosExtras));

                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        #endregion

        #region Métodos Auxiliares / Privados

        /// <summary>
        /// Constrói o cabeçalho estilizado do documento contendo o título da empresa, identificador da proposta, dados do cliente e prazos de validade.
        /// </summary>
        private void ConstruirCabecalho(ColumnDescriptor col, Projeto projeto)
        {
            // Faixa azul escura principal
            col.Item().Background("#1E3A5F").Padding(16).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PROPOSTA TÉCNICA COMERCIAL").FontSize(20).Bold().FontColor(Colors.White);
                    c.Item().Text("AERO CONCEPTS — ENGENHARIA AERONÁUTICA").FontSize(9).FontColor("#A8C4E0");
                });
                row.ConstantItem(100).AlignRight().AlignMiddle()
                    .Text($"#{DateTime.Now:yyyyMMdd}")
                    .FontSize(9).FontColor("#A8C4E0");
            });

            // Seção de metadados (Cliente, Projeto, Datas)
            col.Item().BorderBottom(1).BorderColor("#E0E0E0").PaddingVertical(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("CLIENTE").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.Cliente?.nome ?? "Consumidor Final").FontSize(12).Bold().FontColor("#1E3A5F");
                });

                row.ConstantItem(20);

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("PROJETO").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.nome).FontSize(12).Bold().FontColor("#1E3A5F");
                });

                row.ConstantItem(150).Column(c =>
                {
                    c.Item().Text("DATA DE EMISSÃO").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.Orcamento.data_criacao.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#1E3A5F");

                    c.Item().PaddingTop(4);

                    c.Item().Text("VÁLIDO ATÉ").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.DataExpiracao.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#EF4444");
                });
            });
        }

        /// <summary>
        /// Gerencia a inserção das tabelas de composição de mão de obra, custos extras e o bloco de resumo financeiro final.
        /// </summary>
        private void ConstruirConteudoPrincipal(ColumnDescriptor col, Projeto projeto, List<Custo> custosExtras)
        {
            // Tabela de Mão de Obra e Tarefas
            col.Item().Text("1. COMPOSIÇÃO DE MÃO DE OBRA E TAREFAS").FontSize(9).Bold().FontColor("#555555");
            col.Item().PaddingTop(6).PaddingBottom(15).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3); cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(2); cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    void H(string t) => header.Cell().Background("#1E3A5F").Padding(8).Text(t).FontColor(Colors.White).Bold().FontSize(9);
                    H("TAREFAS/DESCRIÇÃO"); H("RESPONSÁVEL"); H("HORAS"); H("VALOR/HORA"); H("TOTAL");
                });

                foreach (var item in projeto.Tarefas)
                {
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.descricao).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(item.Funcionario?.nome ?? "N/D").FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignCenter().Text($"{item.horas_estimadas:0.#}h").FontSize(9);

                    decimal vHora = item.Funcionario?.Cargo?.custo_medio_hora ?? 0;
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(vHora.ToString("C2", _ptBR)).FontSize(9);
                    table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(item.custo_real.ToString("C2", _ptBR)).FontSize(9).Bold();
                }
            });

            // Tabela de Custos Extras
            if (custosExtras != null && custosExtras.Any())
            {
                col.Item().Text("2. EQUIPAMENTOS, LICENÇAS E CUSTOS ADICIONAIS").FontSize(9).Bold().FontColor("#555555");
                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(4); cols.RelativeColumn(2); cols.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        void H(string t) => header.Cell().Background("#4A5568").Padding(8).Text(t).FontColor(Colors.White).Bold().FontSize(9);
                        H("DESCRIÇÃO DO ITEM"); H("CATEGORIA"); H("VALOR");
                    });

                    foreach (var custo in custosExtras)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.nome).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.categoria).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(custo.valor.ToString("C2", _ptBR)).FontSize(9).Bold();
                    }
                });
            }

            // Resumo Financeiro Final
            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem();
                row.ConstantItem(300).Column(resumo =>
                {
                    resumo.Item().Text("RESUMO FINANCEIRO FINAL").FontSize(9).Bold().FontColor("#555555");
                    resumo.Item().PaddingTop(6).Table(t =>
                    {
                        t.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(1); c.RelativeColumn(2); });

                        void Linha(string label, string pct, string valor, bool destaque = false)
                        {
                            var bg = destaque ? "#1E3A5F" : "#FFFFFF";
                            var fg = destaque ? "#FFFFFF" : "#333333";

                            var container = t.Cell().Background(bg).Padding(6);
                            if (destaque == false) container = container.BorderBottom(1).BorderColor("#F1F5F9");

                            // Célula do Label
                            var cLabel = t.Cell().Background(bg).Padding(6);
                            var tLabel = cLabel.Text(label).FontSize(9).FontColor(fg);
                            if (destaque) tLabel.Bold();

                            // Célula da Porcentagem
                            var cPct = t.Cell().Background(bg).Padding(6).AlignCenter();
                            var tPct = cPct.Text(pct).FontSize(9).FontColor(fg);
                            if (destaque) tPct.Bold();

                            // Célula do Valor Final
                            var cValor = t.Cell().Background(bg).Padding(6).AlignRight();
                            var tValor = cValor.Text(valor).FontSize(9).FontColor(fg);
                            if (destaque) tValor.Bold();
                        }

                        Linha("Custo Total Base", "", projeto.Orcamento.custo_base.ToString("C2", _ptBR));
                        Linha("Impostos", $"{projeto.Orcamento.percentual_impostos:0.#}%", projeto.Orcamento.valor_impostos.ToString("C2", _ptBR));
                        Linha("Margem de Lucro", $"{projeto.Orcamento.margem_percentual:0.#}%", projeto.Orcamento.valor_margem.ToString("C2", _ptBR));
                        Linha("VALOR TOTAL DA PROPOSTA", "", projeto.Orcamento.valor_final.ToString("C2", _ptBR), destaque: true);
                    });
                });
            });
        }

        /// <summary>
        /// Desenha a assinatura corporativa e a paginação dinâmica no rodapé de todas as páginas do PDF.
        /// </summary>
        private void ConstruirRodape(RowDescriptor row)
        {
            row.RelativeItem().Text("Aero Concepts — Tecnologia em Engenharia Aeronáutica").FontSize(7).FontColor("#AAAAAA");
            row.ConstantItem(80).AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(7);
                x.CurrentPageNumber().FontSize(7);
                x.Span(" de ").FontSize(7);
                x.TotalPages().FontSize(7);
            });
        }

        #endregion
    }
}