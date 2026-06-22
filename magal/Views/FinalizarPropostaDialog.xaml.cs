using System;
using System.Windows;
using System.Windows.Controls;
using magal.Models;

namespace magal.Views
{
    public partial class FinalizarPropostaDialog : Window
    {
        private readonly Orcamento _orcamento;

        public FinalizarPropostaDialog(Orcamento orcamentoAtual)
        {
            InitializeComponent();
            _orcamento = orcamentoAtual ?? throw new ArgumentNullException(nameof(orcamentoAtual));

            PreencherCampos();
        }

        private void PreencherCampos()
        {
            // Sincroniza a forma de pagamento atual no ComboBox
            if (!string.IsNullOrEmpty(_orcamento.forma_pagamento))
            {
                foreach (ComboBoxItem item in ComboFormaPagamento.Items)
                {
                    if (item.Content.ToString() == _orcamento.forma_pagamento)
                    {
                        ComboFormaPagamento.SelectedItem = item;
                        break;
                    }
                }
            }

            // Sincroniza o prazo de entrega se houver
            if (_orcamento.prazo_entrega != null)
            {
                DpPrazoEntrega.SelectedDate = _orcamento.prazo_entrega;
            }

            // Sincroniza o campo de texto de observações
            TxtObservacoes.Text = _orcamento.observacoes ?? string.Empty;
        }

        private void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            // Retorna os dados modificados pelo usuário para o objeto de orçamento de origem
            _orcamento.forma_pagamento = (ComboFormaPagamento.SelectedItem as ComboBoxItem)?.Content?.ToString();
            _orcamento.prazo_entrega = DpPrazoEntrega.SelectedDate;
            _orcamento.observacoes = TxtObservacoes.Text;

            this.DialogResult = true;
            this.Close();
        }
    }
}