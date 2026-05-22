/* ==============================================================================
   AERO CONCEPTS - SAD PRECIFICAÇÃO
   ARQUIVO DE BACKUP E CARGA DO BANCO DE DADOS
   ==============================================================================
   Este arquivo contém o script oficial de migração e massa de dados do banco.
   Ele está protegido por blocos de comentário globais para evitar erros de 
   compilação ou validação caso seja lido diretamente pelo compilador C# / WPF 
   no Gerenciador de Soluções.

   PARA EXECUTAR: Copie apenas o conteúdo interno deste bloco e execute no seu 
   SGBD (MySQL Workbench, DBeaver, etc.).
   ==============================================================================

CREATE DATABASE IF NOT EXISTS sad_precificacao;
USE sad_precificacao;

-- ==============================================================================
-- 1. CRIAÇÃO DAS TABELAS
-- ==============================================================================

-- TABELA: USUARIO
CREATE TABLE IF NOT EXISTS usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    status VARCHAR(50) DEFAULT 'Ativo'
);

-- TABELA: CLIENTE
CREATE TABLE IF NOT EXISTS cliente (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    tipo VARCHAR(50), -- PF/PJ/Governo/Institucional
    cpf_cnpj VARCHAR(20),
    cidade VARCHAR(100),
    estado VARCHAR(50),
    contato VARCHAR(100)
);

-- TABELA: CARGO
CREATE TABLE IF NOT EXISTS cargo (
    id_cargo INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    custo_medio_hora DECIMAL(18, 2) NOT NULL
);

-- TABELA: FUNCIONARIO
CREATE TABLE IF NOT EXISTS funcionario (
    id_funcionario INT AUTO_INCREMENT PRIMARY KEY,
    id_cargo INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    custo_hora DECIMAL(18, 2) NOT NULL,
    nivel VARCHAR(50), -- Jr/Pl/Sr/Espec
    tipo_vinculo VARCHAR(50), -- CLT/PJ/Autônomo
    status VARCHAR(50) DEFAULT 'Ativo',
    FOREIGN KEY (id_cargo) REFERENCES cargo(id_cargo)
);

-- TABELA: PROJETO
CREATE TABLE IF NOT EXISTS projeto (
    id_projeto INT AUTO_INCREMENT PRIMARY KEY,
    id_usuario INT NOT NULL,
    id_cliente INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    tipo VARCHAR(100), -- Produto/Serviço
    status VARCHAR(50) DEFAULT 'Rascunho', -- Rascunho/Orçado/Aprovado/Executando/Concluído
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    data_conclusao_prevista DATE,
    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
);

-- TABELA: TAREFA
CREATE TABLE IF NOT EXISTS tarefa (
    id_tarefa INT AUTO_INCREMENT PRIMARY KEY,
    id_projeto INT NOT NULL,
    id_funcionario INT NOT NULL,
    descricao VARCHAR(255),
    horas_estimadas DECIMAL(18, 2) DEFAULT 0,
    horas_reais DECIMAL(18, 2) DEFAULT 0,
    custo_real DECIMAL(18, 2) DEFAULT 0,
    status VARCHAR(50) DEFAULT 'Pendente', -- Pendente/Executando/Concluída
    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE,
    FOREIGN KEY (id_funcionario) REFERENCES funcionario(id_funcionario)
);

-- TABELA: CUSTO
CREATE TABLE IF NOT EXISTS custo (
    id_custo INT AUTO_INCREMENT PRIMARY KEY,
    id_projeto INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    categoria VARCHAR(100),
    tipo VARCHAR(50), -- Direto/Indireto
    valor DECIMAL(18, 2) NOT NULL,
    unidade VARCHAR(50), -- Unitário/Hora/Dia/Mês
    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
);

-- TABELA: ORCAMENTO
CREATE TABLE IF NOT EXISTS orcamento (
    id_orcamento INT AUTO_INCREMENT PRIMARY KEY,
    id_projeto INT NOT NULL UNIQUE, 
    custo_base DECIMAL(18, 2) DEFAULT 0,
    percentual_impostos DECIMAL(18, 2) DEFAULT 0,
    valor_impostos DECIMAL(18, 2) DEFAULT 0,
    margem_percentual DECIMAL(18, 2) DEFAULT 0,
    valor_margem DECIMAL(18, 2) DEFAULT 0,
    valor_final DECIMAL(18, 2) DEFAULT 0,
    validade_dias INT DEFAULT 15,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
);

-- ==============================================================================
-- 2. CARGA DE DADOS INICIAIS (CONFIGURAÇÕES DO SISTEMA)
-- ==============================================================================

-- INSERTS: CARGO
INSERT IGNORE INTO cargo (id_cargo, nome, custo_medio_hora) VALUES 
(1, 'Engenheiro Elétrico', 160.00), 
(2, 'Supervisor Eletroeletrônico', 130.00), 
(3, 'Engenheiro Especialista Turbomáquinas', 220.00), 
(4, 'Coordenador Técnico de Serviços', 145.00), 
(5, 'Analista de Engenharia Industrial', 95.00), 
(6, 'Coordenador de Engenharia Industrial', 155.00), 
(7, 'Analista de PD&I', 105.00), 
(8, 'Engenheiro de PD&I', 175.00), 
(9, 'Gerente de Engenharia', 250.00), 
(10, 'Gerente de Projetos', 230.00), 
(11, 'Consultor Especialista PD&I/Eng', 300.00);

-- INSERTS: FUNCIONARIO
INSERT IGNORE INTO funcionario (id_funcionario, id_cargo, nome, custo_hora, nivel, tipo_vinculo, status) VALUES 
(1, 1, 'Paulino Rubião', 165.00, 'Sr', 'CLT', 'Ativo'), 
(2, 2, 'Eduardo Sedano', 135.00, 'Pl', 'CLT', 'Ativo'), 
(3, 3, 'Flavio Natal', 225.00, 'Espec', 'PJ', 'Ativo'), 
(4, 4, 'Antonio Aguida', 145.00, 'Sr', 'CLT', 'Ativo'), 
(5, 4, 'Roberto Souza Costa', 145.00, 'Sr', 'CLT', 'Ativo'), 
(6, 5, 'Luiz Menezes', 98.00, 'Pl', 'CLT', 'Ativo'), 
(7, 6, 'Evandro Lamberti', 160.00, 'Sr', 'CLT', 'Ativo'), 
(8, 7, 'Clayton Sant''ana', 110.00, 'Pl', 'CLT', 'Ativo'), 
(9, 7, 'Igor Alves', 110.00, 'Pl', 'CLT', 'Ativo'), 
(10, 7, 'Victor Hugo Noronha', 110.00, 'Pl', 'CLT', 'Ativo'), 
(11, 8, 'Lucilene Moraes', 180.00, 'Sr', 'CLT', 'Ativo'), 
(12, 8, 'Gerhard Egwarth', 180.00, 'Sr', 'PJ', 'Ativo'), 
(13, 9, 'Daniel Joaquim Pereira', 260.00, 'Sr', 'CLT', 'Ativo'), 
(14, 10, 'Eduard Müller', 240.00, 'Sr', 'CLT', 'Ativo'), 
(15, 11, 'Marco Antônio Carvalho', 310.00, 'Espec', 'PJ', 'Ativo');

-- INSERTS: USUARIO
INSERT IGNORE INTO usuario (id_usuario, nome, email, senha, status) VALUES 
(1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');

-- INSERTS: CLIENTE
INSERT IGNORE INTO cliente (id_cliente, nome, tipo, cidade, estado, contato) VALUES 
(1, 'Funcate', 'Jurídica', 'São José dos Campos', 'SP', 'Contato Comercial'), 
(2, 'Voith', 'Jurídica', 'São Paulo', 'SP', 'Departamento de Projetos'), 
(3, 'Arauco', 'Jurídica', 'Curitiba', 'PR', 'Suprimentos'), 
(4, 'EESC', 'Institucional', 'São Carlos', 'SP', 'Diretoria Técnica'), 
(5, 'International Paper', 'Jurídica', 'Mogi Guaçu', 'SP', 'Engenharia'), 
(6, 'Suzano', 'Jurídica', 'Salvador', 'BA', 'Gestão de Contratos'), 
(7, 'Klabin', 'Jurídica', 'Telêmaco Borba', 'PR', 'Planejamento'), 
(8, 'FAB', 'Governo', 'Brasília', 'DF', 'Comando da Aeronáutica'), 
(9, 'IAE', 'Governo', 'São José dos Campos', 'SP', 'Diretoria IAE'), 
(10, 'Birla Carbon', 'Jurídica', 'Cubatão', 'SP', 'Manutenção Industrial'), 
(11, 'Rhodia', 'Jurídica', 'Paulínia', 'SP', 'Compras Técnicas'), 
(12, 'Raizen', 'Jurídica', 'Piracicaba', 'SP', 'Projetos Estratégicos'), 
(13, 'DCTA', 'Governo', 'São José dos Campos', 'SP', 'Secretaria de Tecnologia');

-- ==============================================================================
-- 3. HISTÓRICO / DADOS DE TESTE E SIMULAÇÃO
-- ==============================================================================

-- PROJETO & ORÇAMENTO A
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Projeto Antigo Teste A', '2025-01-10', 'Em Aberto', 'Produto', 1, 1);

INSERT INTO orcamento (id_projeto, valor_final, valor_margem, validade_dias) 
VALUES (LAST_INSERT_ID(), 5000.00, 1500.00, 30);

-- PROJETO & ORÇAMENTO B
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Projeto Antigo Teste B', '2025-02-15', 'Em Aberto', 'Produto', 1, 1);

INSERT INTO orcamento (id_projeto, valor_final, valor_margem, validade_dias) 
VALUES (LAST_INSERT_ID(), 12000.00, 4000.00, 15);

-- PROJETO & ORÇAMENTO C
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Projeto Antigo Teste C', '2025-03-20', 'Em Aberto', 'Produto', 1, 1);

INSERT INTO orcamento (id_projeto, valor_final, valor_margem, validade_dias) 
VALUES (LAST_INSERT_ID(), 3500.00, 1000.00, 10);

-- ==============================================================================
-- 4. PROJETOS
-- ==============================================================================

-- PROJETO 04: Voith
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Modernização de Turbina Hidrelétrica VT-01', '2026-01-15 09:00:00', 'Executando', 'Serviço', 2, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 45000.00, 12.00, 5400.00, 25.00, 11250.00, 61650.00, 30);

-- PROJETO 05: Suzano
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Otimização de Linha de Celulose - Planta BA', '2026-02-02 14:30:00', 'Aprovado', 'Serviço', 6, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 28000.00, 12.00, 3360.00, 20.00, 5600.00, 36960.00, 15);

-- PROJETO 06: Klabin
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Desenvolvimento de Painel de Automação Industrial', '2026-02-18 10:15:00', 'Orçado', 'Produto', 7, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 15500.00, 18.00, 2790.00, 30.00, 4650.00, 22940.00, 20);

-- PROJETO 07: FAB
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Análise Estrutural Flaps Aeronave T-27', '2026-03-01 08:00:00', 'Executando', 'Serviço', 8, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 85000.00, 0.00, 0.00, 15.00, 12750.00, 97750.00, 60);

-- PROJETO 08: Arauco
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Sistema de Exaustão de Resíduos Térmicos', '2026-03-12 16:45:00', 'Rascunho', 'Produto', 3, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 19200.00, 18.00, 3456.00, 22.00, 4224.00, 26880.00, 15);

-- PROJETO 09: International Paper
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Laudo Técnico de Conformidade NR-12', '2026-03-25 11:20:00', 'Concluído', 'Serviço', 5, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 8400.00, 12.00, 1008.00, 35.00, 2940.00, 12348.00, 10);

-- PROJETO 10: DCTA
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Consultoria em Dinâmica de Fluidos Computacional (CFD)', '2026-04-05 13:00:00', 'Aprovado', 'Serviço', 13, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 120000.00, 0.00, 0.00, 18.00, 21600.00, 141600.00, 45);

-- PROJETO 11: Raizen
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Dimensionamento Elétrico Destilaria Setor Norte', '2026-04-19 10:00:00', 'Orçado', 'Serviço', 12, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 34000.00, 12.00, 4080.00, 25.00, 8500.00, 46580.00, 30);

-- PROJETO 12: Birla Carbon
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Desenvolvimento de Dispositivo de Içamento Mecânico', '2026-05-02 15:30:00', 'Rascunho', 'Produto', 10, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 7800.00, 18.00, 1404.00, 28.00, 2184.00, 11388.00, 15);

-- PROJETO 13: Rhodia
INSERT INTO projeto (nome, data_criacao, status, tipo, id_cliente, id_usuario) 
VALUES ('Estudo de Viabilidade Técnico - Planta Paulínia', '2026-05-15 08:45:00', 'Orçado', 'Serviço', 11, 1);

INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
VALUES (LAST_INSERT_ID(), 52000.00, 12.00, 6240.00, 22.00, 11440.00, 69680.00, 20);

*/