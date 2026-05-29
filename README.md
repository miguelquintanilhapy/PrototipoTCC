# ✈️ AERO Concepts

Sistema desktop desenvolvido para apoio à elaboração de orçamentos, gestão de projetos e análise financeira, criado como Trabalho de Conclusão de Curso (TCC) do Curso Técnico em Informática.

O objetivo do sistema é centralizar o gerenciamento de projetos, clientes, funcionários e custos, permitindo a geração de orçamentos mais precisos e o acompanhamento de indicadores financeiros por meio de dashboards e relatórios.

---

## 📸 Visão Geral

O AERO Concepts oferece:

- Controle de usuários e autenticação
- Cadastro de clientes
- Cadastro de funcionários
- Cadastro de cargos
- Catálogo de custos
- Elaboração de orçamentos
- Histórico de projetos
- Dashboard financeiro
- Relatórios em PDF
- Indicadores gerenciais
- Gráficos analíticos

---

## 🏗️ Arquitetura

O projeto segue o padrão **MVVM (Model-View-ViewModel)**.

```text
AERO Concepts
│
├── Models
├── Views
├── ViewModels
├── Data
│   ├── Repositories
│   └── Database
├── Services
└── Resources
```

---

## ⚙️ Tecnologias Utilizadas

### Front-end

- WPF (.NET)
- XAML

### Back-end

- C#
- .NET

### Banco de Dados

- MySQL

### Bibliotecas

- LiveCharts
- MySql.Data
- iTextSharp / PDF
- MVVM Pattern

---

## 🚀 Funcionalidades

### 🔐 Login

- Autenticação de usuários
- Controle de sessão
- Cadastro de novos usuários

---

### 📋 Orçamentos

- Criação de projetos
- Seleção de clientes
- Seleção de funcionários
- Associação de custos
- Cálculo de margem de lucro
- Definição de impostos
- Geração de proposta financeira

---

### 📚 Histórico

- Consulta de projetos cadastrados
- Pesquisa por filtros
- Edição de projetos existentes
- Visualização de informações completas

---

### 👥 Gestão de Clientes

- Cadastro
- Edição
- Exclusão
- Pesquisa
- Exportação para PDF

---

### 👨‍💼 Gestão de Funcionários

- Cadastro
- Edição
- Exclusão
- Pesquisa
- Exportação para PDF

---

### 🏢 Gestão de Cargos

- Cadastro
- Edição
- Exclusão

---

### 💰 Gestão de Custos

- Catálogo de custos
- Categorias
- Valores de referência
- Pesquisa
- Exportação para PDF

---

### 📊 Dashboard Financeiro

Indicadores:

- Quantidade de projetos
- Valor total das propostas
- Lucro estimado

Gráficos:

- Lucro x Faturamento
- Projetos por Status
- Projetos por Tipo

Filtros:

- 7 dias
- 30 dias
- 6 meses
- 1 ano
- Histórico completo

---

### 📄 Relatórios

Exportação de relatórios em PDF para:

- Clientes
- Funcionários
- Custos
- Projetos

---

## 🗄️ Estrutura Geral do Sistema

```text
Sistema AERO Concepts
│
├── Login
│   ├── Autenticação
│   └── Controle de Sessão
│
├── Home
│
├── Orçamentos
│   ├── Clientes
│   ├── Funcionários
│   ├── Custos
│   └── Cálculo Financeiro
│
├── Histórico
│   ├── Consulta
│   ├── Pesquisa
│   └── Edição
│
├── Dashboard Financeiro
│   ├── Indicadores
│   ├── Gráficos
│   └── Filtros
│
├── Gerenciamento
│   ├── Clientes
│   ├── Funcionários
│   ├── Cargos
│   ├── Custos
│   └── Usuários
│
└── Relatórios
    ├── PDF
    ├── Financeiros
    └── Projetos
```

---

## 🎯 Objetivo do Projeto

O AERO Concepts foi desenvolvido para auxiliar empresas na elaboração de propostas comerciais e no gerenciamento financeiro de projetos, reduzindo erros manuais e oferecendo suporte à tomada de decisão por meio de indicadores e análises visuais.

---

## 👨‍💻 Equipe

Projeto desenvolvido como Trabalho de Conclusão de Curso (TCC).

Autores:

- João Guilherme Pereira Mendes
- Vinícius Brisolla de Vasconcelos
- Miguel Quintanilha

---

## 📌 Status

Projeto em desenvolvimento.

Versão atual inclui:

✅ Sistema de autenticação  
✅ Gestão de clientes  
✅ Gestão de funcionários  
✅ Gestão de cargos  
✅ Gestão de custos  
✅ Orçamentos  
✅ Histórico de projetos  
✅ Dashboard financeiro  
✅ Relatórios PDF

Próximas melhorias:

- Inteligência para recomendação de recursos
- Indicadores avançados
- Dashboard executivo
- Relatórios analíticos avançados

---
