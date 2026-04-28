CREATE DATABASE IF NOT EXISTS sad_precificacao;
USE sad_precificacao;

-- 1. TABELA USUARIO
CREATE TABLE IF NOT EXISTS Usuario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Senha VARCHAR(255) NOT NULL,
    Status VARCHAR(50) DEFAULT 'Ativo'
);

-- 2. TABELA CARGO
CREATE TABLE IF NOT EXISTS Cargo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    Nivel VARCHAR(50),
    CustoMedioHora DECIMAL(18, 2) NOT NULL,
    Descricao TEXT
);

-- 3. TABELA CLIENTE
CREATE TABLE IF NOT EXISTS Cliente (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nome VARCHAR(255) NOT NULL,
    Tipo VARCHAR(50),
    CpfCnpj VARCHAR(20),
    Cidade VARCHAR(100),
    Estado VARCHAR(50),
    Contato VARCHAR(100)
);

-- 4. TABELA FUNCIONARIO
CREATE TABLE IF NOT EXISTS Funcionario (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    CargoId INT NOT NULL,
    Nome VARCHAR(255) NOT NULL,
    CustoHora DECIMAL(18, 2) NOT NULL,
    TipoVinculo VARCHAR(50),
    Status VARCHAR(50) DEFAULT 'Ativo',
    FOREIGN KEY (CargoId) REFERENCES Cargo(Id)
);

-- 5. TABELA ORCAMENTO
CREATE TABLE IF NOT EXISTS Orcamento (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MargemPercentual DECIMAL(18, 2) DEFAULT 0,
    PercentualImpostos DECIMAL(18, 2) DEFAULT 0,
    CustoBase DECIMAL(18, 2) DEFAULT 0,
    ValorFinal DECIMAL(18, 2) DEFAULT 0,
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 6. TABELA PROJETO
CREATE TABLE IF NOT EXISTS Projeto (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UsuarioId INT NOT NULL,
    ClienteId INT NOT NULL,
    OrcamentoId INT,
    Nome VARCHAR(255) NOT NULL,
    Tipo VARCHAR(100),
    Status VARCHAR(50) DEFAULT 'Em Planejamento',
    DataCriacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    DataConclusaoPrevista DATETIME,
    FOREIGN KEY (UsuarioId) REFERENCES Usuario(Id),
    FOREIGN KEY (ClienteId) REFERENCES Cliente(Id),
    FOREIGN KEY (OrcamentoId) REFERENCES Orcamento(Id)
);

-- 7. TABELA TAREFA
CREATE TABLE IF NOT EXISTS Tarefa (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjetoId INT NOT NULL,
    FuncionarioId INT NOT NULL,
    Descricao VARCHAR(255),
    HorasEstimadas DECIMAL(18, 2) DEFAULT 0,
    HorasReais DECIMAL(18, 2) DEFAULT 0,
    Status VARCHAR(50) DEFAULT 'Pendente',
    FOREIGN KEY (ProjetoId) REFERENCES Projeto(Id) ON DELETE CASCADE,
    FOREIGN KEY (FuncionarioId) REFERENCES Funcionario(Id)
);

-- 8. TABELA CUSTO (EXTRAS)
CREATE TABLE IF NOT EXISTS Custo (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjetoId INT NOT NULL,
    Nome VARCHAR(255) NOT NULL,
    Categoria VARCHAR(100),
    Tipo VARCHAR(50),
    Valor DECIMAL(18, 2) NOT NULL,
    Unidade VARCHAR(50),
    DataCadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProjetoId) REFERENCES Projeto(Id) ON DELETE CASCADE
);

-- 9. TABELA HISTORICO PROJETO
CREATE TABLE IF NOT EXISTS HistoricoProjeto (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    ProjetoId INT NOT NULL,
    TipoProjeto VARCHAR(100),
    Complexidade VARCHAR(50),
    CustoReal DECIMAL(18, 2),
    MargemEstimada DECIMAL(18, 2),
    MargemReal DECIMAL(18, 2),
    DesvioPercentual DECIMAL(18, 2),
    DataConclusao DATETIME,
    Observacoes TEXT
);

-- ==========================================
-- DADOS INICIAIS (AERO CONCEPTS)
-- ==========================================

INSERT IGNORE INTO Cargo (Id, Nome, Nivel, CustoMedioHora) VALUES 
(1, 'Engenheiro Elétrico', 'Sr', 160.00),
(2, 'Supervisor Eletroeletrônico', 'Pl', 130.00),
(3, 'Engenheiro Especialista Turbomáquinas', 'Espec', 220.00),
(4, 'Coordenador Técnico de Serviços', 'Sr', 145.00),
(5, 'Analista de Engenharia Industrial', 'Pl', 95.00),
(6, 'Coordenador de Engenharia Industrial', 'Sr', 155.00),
(7, 'Analista de PD&I', 'Pl', 105.00),
(8, 'Engenheiro de PD&I', 'Sr', 175.00),
(9, 'Gerente de Engenharia', 'Sr', 250.00),
(10, 'Gerente de Projetos', 'Sr', 230.00),
(11, 'Consultor Especialista PD&I/Eng', 'Espec', 300.00);

INSERT IGNORE INTO Funcionario (Id, CargoId, Nome, CustoHora, TipoVinculo, Status) VALUES 
(1, 1, 'Paulino Rubião', 165.00, 'CLT', 'Ativo'),
(2, 2, 'Eduardo Sedano', 135.00, 'CLT', 'Ativo'),
(3, 3, 'Flavio Natal', 225.00, 'PJ', 'Ativo'),
(4, 4, 'Antonio Aguida', 145.00, 'CLT', 'Ativo'),
(5, 4, 'Roberto Souza Costa', 145.00, 'CLT', 'Ativo'),
(6, 5, 'Luiz Menezes', 98.00, 'CLT', 'Ativo'),
(7, 6, 'Evandro Lamberti', 160.00, 'CLT', 'Ativo'),
(8, 7, 'Clayton Sant''ana', 110.00, 'CLT', 'Ativo'),
(9, 7, 'Igor Alves', 110.00, 'CLT', 'Ativo'),
(10, 7, 'Victor Hugo Noronha', 110.00, 'CLT', 'Ativo'),
(11, 8, 'Lucilene Moraes', 180.00, 'CLT', 'Ativo'),
(12, 8, 'Gerhard Egwarth', 180.00, 'PJ', 'Ativo'),
(13, 9, 'Daniel Joaquim Pereira', 260.00, 'CLT', 'Ativo'),
(14, 10, 'Eduard Müller', 240.00, 'CLT', 'Ativo'),
(15, 11, 'Marco Antônio Carvalho', 310.00, 'PJ', 'Ativo');

-- USUÁRIO INICIAL PARA TESTES
INSERT IGNORE INTO Usuario (Id, Nome, Email, Senha, Status) 
VALUES (1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');