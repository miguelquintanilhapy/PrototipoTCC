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
    public class PdfService
    {
        private static readonly CultureInfo _ptBR = new("pt-BR");

        public void GerarPropostaTecnica(Projeto projeto, List<Custo> custosExtras, string caminhoArquivo)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // CABEÇALHO
                    page.Header().Column(col =>
                    {
                        col.Item().Background("#1E3A5F").Padding(16).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("PROPOSTA TÉCNICA COMERCIAL")
                                    .FontSize(20).Bold().FontColor(Colors.White);
                                c.Item().Text("AERO CONCEPTS — ENGENHARIA AERONÁUTICA")
                                    .FontSize(9).FontColor("#A8C4E0");
                            });
                            row.ConstantItem(100).AlignRight().AlignMiddle()
                                .Text($"#{DateTime.Now:yyyyMMdd}")
                                .FontSize(9).FontColor("#A8C4E0");
                        });

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

                            // Ajustado o tamanho para 150 para comportar os dois blocos de data de forma limpa
                            row.ConstantItem(150).Column(c =>
                            {
                                // Bloco da Emissão
                                c.Item().Text("DATA DE EMISSÃO").FontSize(7).FontColor("#999999").Bold();
                                c.Item().Text(projeto.Orcamento.data_criacao.ToString("dd/MM/yyyy"))
                                    .FontSize(11).Bold().FontColor("#1E3A5F");

                                c.Item().PaddingTop(4); // Pequeno espaçamento entre as datas

                                // Bloco da Validade (Minimalista e Destacado)
                                c.Item().Text("VÁLIDO ATÉ").FontSize(7).FontColor("#999999").Bold();
                                c.Item().Text(projeto.DataExpiracao.ToString("dd/MM/yyyy"))
                                    .FontSize(11).Bold().FontColor("#EF4444"); // Destaque em vermelho AeroDanger
                            });
                        });
                    });

                    // CONTEÚDO
                    page.Content().PaddingTop(16).Column(col =>
                    {
                        // TABELA DE MÃO DE OBRA
                        col.Item().Text("1. COMPOSIÇÃO DE MÃO DE OBRA E TAREFAS")
                            .FontSize(9).Bold().FontColor("#555555");

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

                        // TABELA DE CUSTOS EXTRAS
                        if (custosExtras != null && custosExtras.Any())
                        {
                            col.Item().Text("2. EQUIPAMENTOS, LICENÇAS E CUSTOS ADICIONAIS")
                                .FontSize(9).Bold().FontColor("#555555");

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

                        // RESUMO FINANCEIRO
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

                                        void CriarCelula(string texto, bool centralizar = false, bool alinharDireita = false)
                                        {
                                            var container = t.Cell().Background(bg).Padding(6);
                                            if (centralizar) container = container.AlignCenter();
                                            if (alinharDireita) container = container.AlignRight();
                                            var textoFormatado = container.Text(texto).FontSize(9).FontColor(fg);
                                            if (destaque) textoFormatado.Bold();
                                        }

                                        CriarCelula(label);
                                        CriarCelula(pct, centralizar: true);
                                        CriarCelula(valor, alinharDireita: true);
                                    }
                                    Linha("Custo Total Base", "", projeto.Orcamento.custo_base.ToString("C2", _ptBR));
                                    Linha("Impostos", $"{projeto.Orcamento.percentual_impostos:0.#}%", projeto.Orcamento.valor_impostos.ToString("C2", _ptBR));
                                    Linha("Margem de Lucro", $"{projeto.Orcamento.margem_percentual:0.#}%", projeto.Orcamento.valor_margem.ToString("C2", _ptBR)); // Nota: mantido valor_margin/valor_margem conforme seu mapeamento
                                    Linha("VALOR TOTAL DA PROPOSTA", "", projeto.Orcamento.valor_final.ToString("C2", _ptBR), destaque: true);
                                });
                            });
                        });
                    });

                    // RODAPÉ
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(row =>
                    {
                        row.RelativeItem().Text("Aero Concepts — Tecnologia em Engenharia Aeronáutica").FontSize(7).FontColor("#AAAAAA");
                        row.ConstantItem(80).AlignRight().Text(x =>
                        {
                            x.Span("Página ").FontSize(7);
                            x.CurrentPageNumber().FontSize(7);
                            x.Span(" de ").FontSize(7);
                            x.TotalPages().FontSize(7);
                        });
                    });
                });
            }).GeneratePdf(caminhoArquivo);
        }
    }
}