using magal.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

        #region Construtor Estático

        static PdfService()
        {
            // Define a licença do QuestPDF uma única vez na inicialização da aplicação
            QuestPDF.Settings.License = LicenseType.Community;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Gera e salva o arquivo de Proposta Técnica Comercial em PDF com base nos dados do projeto e custos extras informados.
        /// </summary>
        public void GerarPropostaTecnica(Projeto projeto, List<Custo> custosExtras, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalho(col, projeto));
                    page.Content().PaddingTop(16).Column(col => ConstruirConteudoPrincipal(col, projeto, custosExtras));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// Gera e salva um relatório gerencial em PDF contendo a listagem tabular dos projetos.
        /// </summary>
        public void GerarRelatorioTabelaProjetos(List<Projeto> projetos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "PROJETOS", projetos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioProjetos(col, projetos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// Gera e salva um relatório gerencial em PDF contendo a listagem tabular dos funcionários filtrados.
        /// </summary>
        public void GerarRelatorioTabelaFuncionarios(List<Funcionario> funcionarios, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "FUNCIONÁRIOS", funcionarios.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioFuncionarios(col, funcionarios));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// Gera e salva um relatório gerencial em PDF contendo a listagem tabular dos cargos.
        /// </summary>
        public void GerarRelatorioTabelaCargos(List<Cargo> cargos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CARGOS", cargos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioCargos(col, cargos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// Gera e salva um relatório gerencial em PDF contendo a listagem tabular dos clientes.
        /// </summary>
        public void GerarRelatorioTabelaClientes(List<Cliente> clientes, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CLIENTES", clientes.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioClientes(col, clientes));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        /// <summary>
        /// ATUALIZADO: Gera e salva um relatório gerencial em PDF contendo os itens do Catálogo de Custos.
        /// </summary>
        public void GerarRelatorioTabelaCustos(List<CatalogoCusto> custos, string caminhoArquivo)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(1.5f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header().Column(col => ConstruirCabecalhoRelatorio(col, "CATÁLOGO DE CUSTOS", custos.Count));
                    page.Content().PaddingTop(16).Column(col => ConstruirTabelaRelatorioCustos(col, custos));
                    page.Footer().BorderTop(1).BorderColor("#E0E0E0").PaddingTop(8).Row(ConstruirRodape);
                });
            }).GeneratePdf(caminhoArquivo);
        }

        #endregion

        #region Métodos Auxiliares / Privados

        private void ConstruirCabecalho(ColumnDescriptor col, Projeto projeto)
        {
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
                    c.Item().Text(projeto.Orcamento?.data_criacao.ToString("dd/MM/yyyy") ?? DateTime.Now.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#1E3A5F");

                    c.Item().PaddingTop(4);

                    c.Item().Text("VÁLIDO ATÉ").FontSize(7).FontColor("#999999").Bold();
                    c.Item().Text(projeto.DataExpiracao.ToString("dd/MM/yyyy")).FontSize(11).Bold().FontColor("#EF4444");
                });
            });
        }

        private void ConstruirConteudoPrincipal(ColumnDescriptor col, Projeto projeto, List<Custo> custosExtras)
        {
            col.Item().Text("1. COMPOSIÇÃO DE MÃO DE OBRA E TAREFAS").FontSize(9).Bold().FontColor("#555555");
            col.Item().PaddingTop(6).PaddingBottom(15).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(3); cols.RelativeColumn(2); cols.RelativeColumn(1); cols.RelativeColumn(2); cols.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Background("#1E3A5F").Padding(8).Text("TAREFAS/DESCRIÇÃO").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).Text("RESPONSÁVEL").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignCenter().Text("HORAS").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("VALOR/HORA").FontColor(Colors.White).Bold().FontSize(9);
                    header.Cell().Background("#1E3A5F").Padding(8).AlignRight().Text("TOTAL").FontColor(Colors.White).Bold().FontSize(9);
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
                        header.Cell().Background("#4A5568").Padding(8).Text("DESCRIÇÃO DO ITEM").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#4A5568").Padding(8).Text("CATEGORIA").FontColor(Colors.White).Bold().FontSize(9);
                        header.Cell().Background("#4A5568").Padding(8).AlignRight().Text("VALOR").FontColor(Colors.White).Bold().FontSize(9);
                    });

                    foreach (var custo in custosExtras)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.nome).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).Text(custo.categoria).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E8EDF2").Padding(8).AlignRight().Text(custo.valor.ToString("C2", _ptBR)).FontSize(9).Bold();
                    }
                });
            }

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

                            var cLabel = t.Cell().Background(bg).Padding(6);
                            if (!destaque) cLabel = cLabel.BorderBottom(1).BorderColor("#F1F5F9");
                            var tLabel = cLabel.Text(label).FontSize(9).FontColor(fg);
                            if (destaque) tLabel.Bold();

                            var cPct = t.Cell().Background(bg).Padding(6).AlignCenter();
                            if (!destaque) cPct = cPct.BorderBottom(1).BorderColor("#F1F5F9");
                            var tPct = cPct.Text(pct).FontSize(9).FontColor(fg);
                            if (destaque) tPct.Bold();

                            var cValor = t.Cell().Background(bg).Padding(6).AlignRight();
                            if (!destaque) cValor = cValor.BorderBottom(1).BorderColor("#F1F5F9");
                            var tValor = cValor.Text(valor).FontSize(9).FontColor(fg);
                            if (destaque) tValor.Bold();
                        }

                        decimal custoBase = projeto.Orcamento?.custo_base ?? 0;
                        decimal pctImpostos = projeto.Orcamento?.percentual_impostos ?? 0;
                        decimal valImpostos = projeto.Orcamento?.valor_impostos ?? 0;
                        decimal pctMargem = projeto.Orcamento?.margem_percentual ?? 0;
                        decimal valMargem = projeto.Orcamento?.valor_margem ?? 0;
                        decimal valFinal = projeto.Orcamento?.valor_final ?? 0;

                        Linha("Custo Total Base", "", custoBase.ToString("C2", _ptBR));
                        Linha("Impostos", $"{pctImpostos:0.#}%", valImpostos.ToString("C2", _ptBR));
                        Linha("Margem de Lucro", $"{pctMargem:0.#}%", valMargem.ToString("C2", _ptBR));
                        Linha("VALOR TOTAL DA PROPOSTA", "", valFinal.ToString("C2", _ptBR), destaque: true);
                    });
                });
            });
        }

        private void ConstruirCabecalhoRelatorio(ColumnDescriptor col, string tipoRelatorio, int totalRegistros)
        {
            col.Item().Background("#1E3A5F").Padding(14).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"RELATÓRIO GERENCIAL DE {tipoRelatorio.ToUpper()}").FontSize(18).Bold().FontColor(Colors.White);
                    c.Item().Text("AERO CONCEPTS — SISTEMA INTERNO DE HISTÓRICO").FontSize(10).FontColor("#A8C4E0");
                });

                row.ConstantItem(220).AlignRight().AlignMiddle().Column(c =>
                {
                    c.Item().Text($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.White);
                    c.Item().Text($"Total de registros exibidos: {totalRegistros}").FontSize(10).FontColor("#A8C4E0").Bold();
                });
            });
        }

        private void ConstruirTabelaRelatorioProjetos(ColumnDescriptor col, List<Projeto> projetos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(45);   // ID
                    cols.RelativeColumn(3);    // Nome do Projeto
                    cols.RelativeColumn(2.5f); // Cliente
                    cols.RelativeColumn(1.3f); // Tipo
                    cols.RelativeColumn(1.5f); // Status
                    cols.ConstantColumn(85);   // Vencimento
                    cols.RelativeColumn(2.2f); // Valor Final
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME DO PROJETO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CLIENTE").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("STATUS").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("VENCIMENTO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignRight().Text("VALOR FINAL").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var p in projetos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(p.id_projeto.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.Cliente?.nome ?? "Consumidor Final").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.tipo ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(p.status?.ToUpper() ?? "N/D").FontSize(10).Bold();

                    string corData = p.EstaVencido ? "#EF4444" : "#333333";
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(p.DataExpiracao.ToString("dd/MM/yyyy")).FontSize(10).FontColor(corData);

                    decimal valorFinal = p.Orcamento?.valor_final ?? 0;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignRight().Text(valorFinal.ToString("C2", _ptBR)).FontSize(10).Bold();

                    listraAlternada = !listraAlternada;
                }
            });

            decimal totalFaturado = projetos.Where(p => p.Orcamento != null).Sum(p => p.Orcamento.valor_final);
            decimal totalLucro = projetos.Where(p => p.Orcamento != null).Sum(p => p.Orcamento.valor_margem);

            col.Item().PaddingTop(12).AlignRight().Width(280).Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(1); });

                t.Cell().Background("#EDF2F7").Padding(6).Text("Lucro Estimado:").FontSize(10).FontColor("#2D3748").Bold();
                t.Cell().Background("#EDF2F7").Padding(6).AlignRight().Text(totalLucro.ToString("C2", _ptBR)).FontSize(10).FontColor("#2D3748").Bold();

                t.Cell().Background("#1E3A5F").Padding(6).Text("Faturamento Total:").FontSize(10).FontColor(Colors.White).Bold();
                t.Cell().Background("#1E3A5F").Padding(6).AlignRight().Text(totalFaturado.ToString("C2", _ptBR)).FontSize(10).FontColor(Colors.White).Bold();
            });
        }

        private void ConstruirTabelaRelatorioCargos(ColumnDescriptor col, List<Cargo> cargos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);   // ID
                    cols.RelativeColumn(6);    // Nome do Cargo
                    cols.RelativeColumn(3);    // Custo Médio/Hora
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME DO CARGO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignCenter().Text("CUSTO MÉDIO / HORA").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in cargos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_cargo.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();

                    decimal custoHora = c.custo_medio_hora;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter()
                        .Text(custoHora.ToString("C2", _ptBR)).FontSize(10).Bold().FontColor("#009140");

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirTabelaRelatorioFuncionarios(ColumnDescriptor col, List<Funcionario> funcionarios)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(50);   // ID
                    cols.RelativeColumn(4);    // Nome Completo
                    cols.RelativeColumn(2.5f); // Tipo de Vínculo
                    cols.RelativeColumn(2.2f); // Custo/Hora
                    cols.RelativeColumn(1.5f); // Status
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME COMPLETO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO DE VÍNCULO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignLeft().Text("CUSTO/HORA").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("STATUS").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var f in funcionarios)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(f.id_funcionario.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.tipo_vinculo ?? "-").FontSize(10);

                    decimal custoHora = f.custo_hora;
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignLeft().Text(custoHora.ToString("C2", _ptBR)).FontSize(10);

                    string corStatus = (f.status?.ToLower() == "ativo") ? "#009140" : "#EF4444";
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(f.status?.ToUpper() ?? "N/D").FontSize(10).Bold().FontColor(corStatus);

                    listraAlternada = !listraAlternada;
                }
            });
        }

        private void ConstruirTabelaRelatorioClientes(ColumnDescriptor col, List<Cliente> clientes)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(45);   // ID
                    cols.RelativeColumn(3.0f); // Nome / Razão Social
                    cols.RelativeColumn(1.0f); // Tipo (Física / Jurídica)
                    cols.RelativeColumn(1.8f); // CPF / CNPJ
                    cols.RelativeColumn(2.2f); // Localização (Cidade - UF)
                    cols.RelativeColumn(1.8f); // Contato (Telefone/Celular)
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("NOME / RAZÃO SOCIAL").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("TIPO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CPF / CNPJ").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("LOCALIZAÇÃO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CONTATO").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in clientes)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_cliente.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.tipo ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.cpf_cnpj ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text($"{c.cidade ?? "-"} - {c.estado ?? "-"}").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.contato ?? "-").FontSize(10);

                    listraAlternada = !listraAlternada;
                }
            });
        }

        /// <summary>
        /// REFEITO: Monta a tabela do relatório gerencial de custos usando CatalogoCusto 
        /// e removendo a coluna "Tipo" que não existe na nova tabela.
        /// </summary>
        private void ConstruirTabelaRelatorioCustos(ColumnDescriptor col, List<CatalogoCusto> custos)
        {
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(60);   // ID (id_catalogo_custo)
                    cols.RelativeColumn(5.5f); // Nome do Item do Catálogo
                    cols.RelativeColumn(3.5f); // Categoria
                    cols.RelativeColumn(2.5f); // Valor Unitário
                });

                table.Header(header =>
                {
                    header.Cell().Background("#2D3748").Padding(8).Text("ID").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("ITEM / DESCRIÇÃO").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).Text("CATEGORIA").FontColor(Colors.White).Bold().FontSize(9.5f);
                    header.Cell().Background("#2D3748").Padding(8).AlignRight().Text("VALOR").FontColor(Colors.White).Bold().FontSize(9.5f);
                });

                bool listraAlternada = false;
                foreach (var c in custos)
                {
                    string corFundo = listraAlternada ? "#F8FAFC" : "#FFFFFF";

                    // Alterado para ler as propriedades corretas da nova model CatalogoCusto
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignCenter().Text(c.id_catalogo_custo.ToString()).FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.nome ?? "-").FontSize(10).Bold();
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).Text(c.categoria ?? "-").FontSize(10);
                    table.Cell().Background(corFundo).BorderBottom(1).BorderColor("#E2E8F0").Padding(8).AlignRight().Text(c.valor.ToString("C2", _ptBR)).FontSize(10).Bold().FontColor("#009140");

                    listraAlternada = !listraAlternada;
                }
            });

            // Somatório dos custos filtrados para o rodapé da tabela
            decimal somaCustos = custos.Sum(c => c.valor);

            col.Item().PaddingTop(12).AlignRight().Width(250).Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(1); c.RelativeColumn(1.2f); });

                t.Cell().Background("#1E3A5F").Padding(6).Text("Valor Acumulado:").FontSize(10).FontColor(Colors.White).Bold();
                t.Cell().Background("#1E3A5F").Padding(6).AlignRight().Text(somaCustos.ToString("C2", _ptBR)).FontSize(10).FontColor(Colors.White).Bold();
            });
        }

        private void ConstruirRodape(RowDescriptor row)
        {
            row.RelativeItem().Text("Aero Concepts — Tecnologia em Engenharia Aeronáutica").FontSize(8).FontColor("#AAAAAA");
            row.ConstantItem(80).AlignRight().Text(x =>
            {
                x.Span("Página ").FontSize(8);
                x.CurrentPageNumber().FontSize(8);
                x.Span(" de ").FontSize(8);
                x.TotalPages().FontSize(8);
            });
        }

        #endregion
    }
}