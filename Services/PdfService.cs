using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SAD.Models;
using System;
using System.Globalization;

namespace SAD.Services
{
    /// <summary>
    /// Implementação do serviço de PDF usando QuestPDF (Community License — gratuito).
    /// Instalar via NuGet: dotnet add package QuestPDF
    /// </summary>
    public class PdfService : IPdfService
    {
        private static readonly CultureInfo _ptBR = new("pt-BR");

        public void GerarPdf(Proposta proposta, string caminhoArquivo)
        {
            // QuestPDF v2023+ requer declaração de licença
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Element(ComposeHeader(proposta));
                    page.Content().Element(ComposeContent(proposta));
                    page.Footer().Element(ComposeFooter(proposta));
                });
            })
            .GeneratePdf(caminhoArquivo);
        }

        // ── HEADER ──────────────────────────────────────────────────────────────
        private static Action<IContainer> ComposeHeader(Proposta proposta) => container =>
        {
            container.Column(col =>
            {
                // Barra azul com título
                col.Item().Background("#1E3A5F").Padding(16).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("PROPOSTA COMERCIAL")
                            .FontSize(20).Bold().FontColor(Colors.White);
                        c.Item().Text("Sistema de Apoio à Decisão — Engenharia")
                            .FontSize(9).FontColor("#A8C4E0");
                    });

                    row.ConstantItem(100).AlignRight().AlignMiddle()
                        .Text($"#{DateTime.Now:yyyyMMdd}")
                        .FontSize(9).FontColor("#A8C4E0");
                });

                // Linha de dados do cliente
                col.Item().BorderBottom(1).BorderColor("#E0E0E0").PaddingVertical(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("CLIENTE").FontSize(7).FontColor("#999999").Bold();
                        c.Item().Text(proposta.Cliente).FontSize(12).Bold().FontColor("#1E3A5F");
                    });

                    row.ConstantItem(20);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("PROJETO").FontSize(7).FontColor("#999999").Bold();
                        c.Item().Text(proposta.Projeto).FontSize(12).Bold().FontColor("#1E3A5F");
                    });

                    row.ConstantItem(20);

                    row.ConstantItem(130).Column(c =>
                    {
                        c.Item().Text("DATA DE EMISSÃO").FontSize(7).FontColor("#999999").Bold();
                        c.Item().Text(proposta.DataGeracao.ToString("dd/MM/yyyy")).FontSize(12).Bold().FontColor("#1E3A5F");
                    });
                });
            });
        };

        // ── CONTENT ─────────────────────────────────────────────────────────────
        private static Action<IContainer> ComposeContent(Proposta proposta) => container =>
        {
            container.PaddingTop(16).Column(col =>
            {
                // Tabela de itens
                col.Item().Text("COMPOSIÇÃO DO PROJETO")
                    .FontSize(9).Bold().FontColor("#555555").LetterSpacing(1);

                col.Item().PaddingTop(6).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.RelativeColumn(3);   // Cargo
                        cols.RelativeColumn(1);   // Horas
                        cols.RelativeColumn(2);   // Valor/Hora
                        cols.RelativeColumn(2);   // Total
                    });

                    // Cabeçalho da tabela
                    table.Header(header =>
                    {
                        static IContainer HeaderCell(IContainer c) =>
                            c.Background("#1E3A5F").Padding(8);

                        header.Cell().Element(HeaderCell).Text("CARGO").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Element(HeaderCell).AlignCenter().Text("HORAS").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Element(HeaderCell).AlignRight().Text("VALOR/HORA").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Element(HeaderCell).AlignRight().Text("TOTAL").FontColor(Colors.White).Bold().FontSize(9);
                    });

                    // Linhas de dados
                    bool alternate = false;
                    foreach (var item in proposta.Itens)
                    {
                        var bg = alternate ? "#F5F8FC" : Colors.White;
                        alternate = !alternate;

                        static IContainer DataCell(IContainer c, string bg) =>
                            c.Background(bg).BorderBottom(1).BorderColor("#E8EDF2").Padding(8);

                        table.Cell().Element(c => DataCell(c, bg)).Text(item.Cargo).FontSize(9);
                        table.Cell().Element(c => DataCell(c, bg)).AlignCenter().Text($"{item.Horas:0.#}h").FontSize(9);
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.ValorPorHora.ToString("C2", _ptBR)).FontSize(9);
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.Total.ToString("C2", _ptBR)).FontSize(9).Bold();
                    }
                });

                // Bloco de resumo financeiro
                col.Item().PaddingTop(20).Row(row =>
                {
                    row.RelativeItem(); // espaço à esquerda

                    row.ConstantItem(280).Column(resumo =>
                    {
                        resumo.Item().Text("RESUMO FINANCEIRO")
                            .FontSize(9).Bold().FontColor("#555555").LetterSpacing(1);

                        resumo.Item().PaddingTop(6).Table(t =>
                        {
                            t.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(3);
                                c.RelativeColumn(1);
                                c.RelativeColumn(2);
                            });

                            // Linha de resumo helper
                            void AddResumoRow(string label, string pct, string valor, bool bold = false, bool highlight = false)
                            {
                                var bg = highlight ? "#1E3A5F" : Colors.White;
                                var fg = highlight ? Colors.White : "#333333";
                                var border = highlight ? 0 : 1;

                                t.Cell().Background(bg).BorderBottom(border).BorderColor("#E8EDF2")
                                    .PaddingHorizontal(8).PaddingVertical(6)
                                    .Text(label).FontSize(9).FontColor(fg).Bold(bold);

                                t.Cell().Background(bg).BorderBottom(border).BorderColor("#E8EDF2")
                                    .AlignCenter().PaddingVertical(6)
                                    .Text(pct).FontSize(9).FontColor(highlight ? "#A8C4E0" : "#666666");

                                t.Cell().Background(bg).BorderBottom(border).BorderColor("#E8EDF2")
                                    .AlignRight().PaddingHorizontal(8).PaddingVertical(6)
                                    .Text(valor).FontSize(9).FontColor(fg).Bold(bold);
                            }

                            AddResumoRow("Subtotal", "", proposta.Subtotal.ToString("C2", _ptBR));
                            AddResumoRow("Overhead", $"{proposta.OverheadPercentual:0.#}%", proposta.ValorOverhead.ToString("C2", _ptBR));
                            AddResumoRow("Margem de Lucro", $"{proposta.LucroPercentual:0.#}%", proposta.ValorLucro.ToString("C2", _ptBR));
                            AddResumoRow("Impostos", $"{proposta.ImpostosPercentual:0.#}%", proposta.ValorImpostos.ToString("C2", _ptBR));
                            AddResumoRow("VALOR TOTAL", "", proposta.TotalFinal.ToString("C2", _ptBR), bold: true, highlight: true);
                        });
                    });
                });

                // Observações
                col.Item().PaddingTop(24).BorderTop(1).BorderColor("#E0E0E0").PaddingTop(12).Column(obs =>
                {
                    obs.Item().Text("OBSERVAÇÕES").FontSize(8).Bold().FontColor("#999999").LetterSpacing(1);
                    obs.Item().PaddingTop(4).Text(
                        "Esta proposta tem validade de 30 dias a partir da data de emissão. " +
                        "Os valores apresentados são baseados nas horas estimadas para cada cargo e " +
                        "podem ser ajustados mediante alinhamento com o cliente.")
                        .FontSize(8).FontColor("#777777").Italic();
                });
            });
        };

        // ── FOOTER ──────────────────────────────────────────────────────────────
        private static Action<IContainer> ComposeFooter(Proposta proposta) => container =>
        {
            container.BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("Gerado automaticamente pelo SAD — Sistema de Apoio à Decisão")
                    .FontSize(7).FontColor("#AAAAAA");

                row.ConstantItem(100).AlignRight()
                    .Text(x =>
                    {
                        x.Span("Página ").FontSize(7).FontColor("#AAAAAA");
                        x.CurrentPageNumber().FontSize(7).FontColor("#AAAAAA");
                        x.Span(" de ").FontSize(7).FontColor("#AAAAAA");
                        x.TotalPages().FontSize(7).FontColor("#AAAAAA");
                    });
            });
        };
    }
}
