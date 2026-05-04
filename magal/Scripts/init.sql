CREATE DATABASE IF NOT EXISTS sad_precificacao;
USE sad_precificacao;

-- 1. USUARIO
CREATE TABLE IF NOT EXISTS usuario (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha VARCHAR(255) NOT NULL,
    status VARCHAR(50) DEFAULT 'Ativo'
);

-- 2. CLIENTE
CREATE TABLE IF NOT EXISTS cliente (
    id_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    tipo VARCHAR(50), -- PF/PJ
    cpf_cnpj VARCHAR(20),
    cidade VARCHAR(100),
    estado VARCHAR(50),
    contato VARCHAR(100)
);

-- 3. CARGO
CREATE TABLE IF NOT EXISTS cargo (
    id_cargo INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    nivel VARCHAR(50), -- Jr/Pl/Sr
    custo_medio_hora DECIMAL(18, 2) NOT NULL,
    descricao TEXT
);

-- 4. FUNCIONARIO
CREATE TABLE IF NOT EXISTS funcionario (
    id_funcionario INT AUTO_INCREMENT PRIMARY KEY,
    id_cargo INT NOT NULL,
    nome VARCHAR(255) NOT NULL,
    custo_hora DECIMAL(18, 2) NOT NULL,
    tipo_vinculo VARCHAR(50), -- CLT/PJ/Autônomo
    status VARCHAR(50) DEFAULT 'Ativo',
    FOREIGN KEY (id_cargo) REFERENCES cargo(id_cargo)
);

-- 5. PROJETO
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

-- 6. TAREFA
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

-- 7. CUSTO
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

-- 8. ORCAMENTO
CREATE TABLE IF NOT EXISTS orcamento (
    id_orcamento INT AUTO_INCREMENT PRIMARY KEY,
    id_projeto INT NOT NULL UNIQUE, -- Relacionamento 1:1
    custo_base DECIMAL(18, 2) DEFAULT 0,
    percentual_impostos DECIMAL(18, 2) DEFAULT 0,
    valor_impostos DECIMAL(18, 2) DEFAULT 0,
    margem_percentual DECIMAL(18, 2) DEFAULT 0,
    valor_margem DECIMAL(18, 2) DEFAULT 0,
    valor_final DECIMAL(18, 2) DEFAULT 0,
    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
);

-- 9. HISTORICO_PROJETO
CREATE TABLE IF NOT EXISTS historico_projeto (
    id_historico INT AUTO_INCREMENT PRIMARY KEY,
    id_projeto INT NOT NULL UNIQUE, -- Relacionamento 1:1
    tipo_projeto VARCHAR(100),
    complexidade VARCHAR(50),
    custo_real DECIMAL(18, 2),
    margem_estimada DECIMAL(18, 2),
    margem_real DECIMAL(18, 2),
    desvio_percentual DECIMAL(18, 2),
    data_conclusao DATETIME,
    observacoes TEXT,
    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto)
);

-- --- DADOS INICIAIS ---

INSERT IGNORE INTO cargo (id_cargo, nome, nivel, custo_medio_hora) VALUES 
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

INSERT IGNORE INTO funcionario (id_funcionario, id_cargo, nome, custo_hora, tipo_vinculo, status) VALUES 
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

INSERT IGNORE INTO usuario (id_usuario, nome, email, senha, status) VALUES 
(1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');

-- --- INSERÇÃO DE CLIENTES ATUALIZADOS ---
INSERT IGNORE INTO cliente (nome, tipo, cidade, estado, contato) VALUES 
('Funcate', 'Jurídica', 'São José dos Campos', 'SP', 'Contato Comercial'),
('Voith', 'Jurídica', 'São Paulo', 'SP', 'Departamento de Projetos'),
('Arauco', 'Jurídica', 'Curitiba', 'PR', 'Suprimentos'),
('EESC', 'Institucional', 'São Carlos', 'SP', 'Diretoria Técnica'),
('International Paper', 'Jurídica', 'Mogi Guaçu', 'SP', 'Engenharia'),
('Suzano', 'Jurídica', 'Salvador', 'BA', 'Gestão de Contratos'),
('Klabin', 'Jurídica', 'Telêmaco Borba', 'PR', 'Planejamento'),
('FAB', 'Governo', 'Brasília', 'DF', 'Comando da Aeronáutica'),
('IAE', 'Governo', 'São José dos Campos', 'SP', 'Diretoria IAE'),
('Birla Carbon', 'Jurídica', 'Cubatão', 'SP', 'Manutenção Industrial'),
('Rhodia', 'Jurídica', 'Paulínia', 'SP', 'Compras Técnicas'),
('Raizen', 'Jurídica', 'Piracicaba', 'SP', 'Projetos Estratégicos'),
('DCTA', 'Governo', 'São José dos Campos', 'SP', 'Secretaria de Tecnologia');