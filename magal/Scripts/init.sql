-- ==============================================================================
-- BACKUP DE ESTRUTURA DO BANCO DE DADOS - AERO CONCEPTS (SAD PRECIFICAÇÃO)
-- DATA: 14/05/2026
-- ==============================================================================

--CREATE DATABASE IF NOT EXISTS sad_precificacao;
--USE sad_precificacao;

--CREATE TABLE IF NOT EXISTS usuario (
--    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
--    nome VARCHAR(255) NOT NULL,
--    email VARCHAR(255) NOT NULL UNIQUE,
--    senha VARCHAR(255) NOT NULL,
--    status VARCHAR(50) DEFAULT 'Ativo'
--);

--CREATE TABLE IF NOT EXISTS cliente (
--    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
--    nome VARCHAR(255) NOT NULL,
--    tipo VARCHAR(50),
--    cpf_cnpj VARCHAR(20),
--    cidade VARCHAR(100),
--    estado VARCHAR(50),
--    contato VARCHAR(100)
--);

--CREATE TABLE IF NOT EXISTS cargo (
--    id_cargo INT AUTO_INCREMENT PRIMARY KEY,
--    nome VARCHAR(255) NOT NULL
--);

--CREATE TABLE IF NOT EXISTS funcionario (
--    id_funcionario INT AUTO_INCREMENT PRIMARY KEY,
--    id_cargo INT NOT NULL,
--    nome VARCHAR(255) NOT NULL,
--    nivel VARCHAR(50),
--    custo_hora DECIMAL(18,2) NOT NULL,
--    tipo_vinculo VARCHAR(50),
--    status VARCHAR(50) DEFAULT 'Ativo',
--    FOREIGN KEY (id_cargo) REFERENCES cargo(id_cargo)
--);

--CREATE TABLE IF NOT EXISTS projeto (
--    id_projeto INT AUTO_INCREMENT PRIMARY KEY,
--    id_usuario INT NOT NULL,
--    id_cliente INT NOT NULL,
--    nome VARCHAR(255) NOT NULL,
--    tipo VARCHAR(100),
--    status VARCHAR(50) DEFAULT 'Rascunho',
--    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
--    data_conclusao_prevista DATE,
--    FOREIGN KEY (id_usuario) REFERENCES usuario(id_usuario),
--    FOREIGN KEY (id_cliente) REFERENCES cliente(id_cliente)
--);

--CREATE TABLE IF NOT EXISTS tarefa (
--    id_tarefa INT AUTO_INCREMENT PRIMARY KEY,
--    id_projeto INT NOT NULL,
--    id_funcionario INT NOT NULL,
--    descricao VARCHAR(255),
--    horas_estimadas DECIMAL(18,2) DEFAULT 0,
--    horas_reais DECIMAL(18,2) DEFAULT 0,
--    custo_real DECIMAL(18,2) DEFAULT 0,
--    status VARCHAR(50) DEFAULT 'Pendente',
--    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE,
--    FOREIGN KEY (id_funcionario) REFERENCES funcionario(id_funcionario)
--);

--CREATE TABLE IF NOT EXISTS custo (
--    id_custo INT AUTO_INCREMENT PRIMARY KEY,
--    id_projeto INT NOT NULL,
--    nome VARCHAR(255) NOT NULL,
--    categoria VARCHAR(100),
--    tipo VARCHAR(50),
--    valor DECIMAL(18,2) NOT NULL,
--    unidade VARCHAR(50),
--    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
--    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
--);

--CREATE TABLE IF NOT EXISTS orcamento (
--    id_orcamento INT AUTO_INCREMENT PRIMARY KEY,
--    id_projeto INT NOT NULL UNIQUE,
--    custo_base DECIMAL(18,2) DEFAULT 0,
--    percentual_impostos DECIMAL(18,2) DEFAULT 0,
--    valor_impostos DECIMAL(18,2) DEFAULT 0,
--    margem_percentual DECIMAL(18,2) DEFAULT 0,
--    valor_margem DECIMAL(18,2) DEFAULT 0,
--    valor_final DECIMAL(18,2) DEFAULT 0,
--    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
--    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
--);

--INSERT IGNORE INTO cargo (id_cargo, nome) VALUES
--(1, 'Engenheiro Elétrico'),
--(2, 'Supervisor Eletroeletrônico'),
--(3, 'Engenheiro Especialista Turbomáquinas'),
--(4, 'Coordenador Técnico de Serviços'),
--(5, 'Analista de Engenharia Industrial'),
--(6, 'Coordenador de Engenharia Industrial'),
--(7, 'Analista de PD&I'),
--(8, 'Engenheiro de PD&I'),
--(9, 'Gerente de Engenharia'),
--(10, 'Gerente de Projetos'),
--(11, 'Consultor Especialista PD&I/Eng');

--INSERT IGNORE INTO funcionario 
--(id_funcionario, id_cargo, nome, nivel, custo_hora, tipo_vinculo, status) 
--VALUES
--(1, 1, 'Paulino Rubião', 'Sr', 165.00, 'CLT', 'Ativo'),
--(2, 2, 'Eduardo Sedano', 'Pl', 135.00, 'CLT', 'Ativo'),
--(3, 3, 'Flavio Natal', 'Espec', 225.00, 'PJ', 'Ativo'),
--(4, 4, 'Antonio Aguida', 'Sr', 145.00, 'CLT', 'Ativo'),
--(5, 4, 'Roberto Souza Costa', 'Sr', 145.00, 'CLT', 'Ativo'),
--(6, 5, 'Luiz Menezes', 'Pl', 98.00, 'CLT', 'Ativo'),
--(7, 6, 'Evandro Lamberti', 'Sr', 160.00, 'CLT', 'Ativo'),
--(8, 7, 'Clayton Sant''ana', 'Pl', 110.00, 'CLT', 'Ativo'),
--(9, 7, 'Igor Alves', 'Pl', 110.00, 'CLT', 'Ativo'),
--(10, 7, 'Victor Hugo Noronha', 'Pl', 110.00, 'CLT', 'Ativo'),
--(11, 8, 'Lucilene Moraes', 'Sr', 180.00, 'CLT', 'Ativo'),
--(12, 8, 'Gerhard Egwarth', 'Sr', 180.00, 'PJ', 'Ativo'),
--(13, 9, 'Daniel Joaquim Pereira', 'Sr', 260.00, 'CLT', 'Ativo'),
--(14, 10, 'Eduard Müller', 'Sr', 240.00, 'CLT', 'Ativo'),
--(15, 11, 'Marco Antônio Carvalho', 'Espec', 310.00, 'PJ', 'Ativo');

--INSERT IGNORE INTO usuario (id_usuario, nome, email, senha, status) VALUES
--(1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');

--INSERT IGNORE INTO cliente 
--(id_cliente, nome, tipo, cidade, estado, contato) 
--VALUES
--(1, 'Funcate', 'Jurídica', 'São José dos Campos', 'SP', 'Contato Comercial'),
--(2, 'Voith', 'Jurídica', 'São Paulo', 'SP', 'Departamento de Projetos'),
--(3, 'Arauco', 'Jurídica', 'Curitiba', 'PR', 'Suprimentos');

-- ==============================================================================
-- FIM DO SCRIPT DE BACKUP
-- ==============================================================================