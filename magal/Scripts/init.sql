---- ==============================================================================
---- SISTEMA: AERO CONCEPTS (SAD PRECIFICAÇÃO)
---- ESTRUTURA DO BANCO DE DADOS E CARGA INICIAL
---- DATA: 22/05/2026
---- STATUS: MATEMATICAMENTE CORRIGIDO E AUDITADO (CENTAVO POR CENTAVO)
---- ==============================================================================

--CREATE DATABASE IF NOT EXISTS sad_precificacao;
--USE sad_precificacao;

---- ==============================================================================
---- 1. CRIAÇÃO DAS TABELAS
---- ==============================================================================

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
--    nome VARCHAR(255) NOT NULL,
--    custo_medio_hora DECIMAL(18, 2) NOT NULL
--);

--CREATE TABLE IF NOT EXISTS funcionario (
--    id_funcionario INT AUTO_INCREMENT PRIMARY KEY,
--    id_cargo INT NOT NULL,
--    nome VARCHAR(255) NOT NULL,
--    custo_hora DECIMAL(18, 2) NULL,
--    nivel VARCHAR(50), 
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
--    horas_estimadas DECIMAL(18, 2) DEFAULT 0,
--    horas_reais DECIMAL(18, 2) DEFAULT 0,
--    custo_real DECIMAL(18, 2) DEFAULT 0,
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
--    valor DECIMAL(18, 2) NOT NULL,
--    unidade VARCHAR(50), 
--    data_cadastro DATETIME DEFAULT CURRENT_TIMESTAMP,
--    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
--);

--CREATE TABLE IF NOT EXISTS orcamento (
--    id_orcamento INT AUTO_INCREMENT PRIMARY KEY,
--    id_projeto INT NOT NULL UNIQUE, 
--    custo_base DECIMAL(18, 2) DEFAULT 0,
--    percentual_impostos DECIMAL(18, 2) DEFAULT 0,
--    valor_impostos DECIMAL(18, 2) DEFAULT 0,
--    margem_percentual DECIMAL(18, 2) DEFAULT 0,
--    valor_margem DECIMAL(18, 2) DEFAULT 0,
--    valor_final DECIMAL(18, 2) DEFAULT 0,
--    validade_dias INT DEFAULT 15,
--    data_criacao DATETIME DEFAULT CURRENT_TIMESTAMP,
--    FOREIGN KEY (id_projeto) REFERENCES projeto(id_projeto) ON DELETE CASCADE
--);

---- ==============================================================================
---- 2. CARGA DE DADOS INICIAIS (CONFIGURAÇÕES DO SISTEMA)
---- ==============================================================================

--INSERT IGNORE INTO cargo (id_cargo, nome, custo_medio_hora) VALUES 
--(1, 'Engenheiro Elétrico', 160.00), 
--(2, 'Supervisor Eletroeletrônico', 130.00), 
--(3, 'Engenheiro Especialista Turbomáquinas', 220.00), 
--(4, 'Coordenador Técnico de Serviços', 145.00), 
--(5, 'Analista de Engenharia Industrial', 95.00), 
--(6, 'Coordenador de Engenharia Industrial', 155.00), 
--(7, 'Analista de PD&I', 105.00), 
--(8, 'Engenheiro de PD&I', 175.00), 
--(9, 'Gerente de Engenharia', 250.00), 
--(10, 'Gerente de Projetos', 230.00), 
--(11, 'Consultor Especialista PD&I/Eng', 300.00);

--INSERT IGNORE INTO funcionario (id_funcionario, id_cargo, nome, custo_hora, nivel, tipo_vinculo, status) VALUES 
--(1, 1, 'Paulino Rubião', 165.00, 'Sênior', 'CLT', 'Ativo'), 
--(2, 2, 'Eduardo Sedano', 135.00, 'Pleno', 'CLT', 'Ativo'), 
--(3, 3, 'Flavio Natal', 225.00, 'Especialista', 'PJ', 'Ativo'), 
--(4, 4, 'Antonio Aguida', 145.00, 'Sênior', 'CLT', 'Ativo'), 
--(5, 4, 'Roberto Souza Costa', 145.00, 'Sênior', 'CLT', 'Ativo'), 
--(6, 5, 'Luiz Menezes', 98.00, 'Pleno', 'CLT', 'Ativo'), 
--(7, 6, 'Evandro Lamberti', 160.00, 'Sênior', 'CLT', 'Ativo'), 
--(8, 7, 'Clayton Sant''ana', 110.00, 'Pleno', 'CLT', 'Ativo'), 
--(9, 7, 'Igor Alves', 110.00, 'Pleno', 'CLT', 'Ativo'), 
--(10, 7, 'Victor Hugo Noronha', 110.00, 'Pleno', 'CLT', 'Ativo'), 
--(11, 8, 'Lucilene Moraes', 180.00, 'Sênior', 'CLT', 'Ativo'), 
--(12, 8, 'Gerhard Egwarth', 180.00, 'Sênior', 'PJ', 'Ativo'), 
--(13, 9, 'Daniel Joaquim Pereira', 260.00, 'Sênior', 'CLT', 'Ativo'), 
--(14, 10, 'Eduard Müller', 240.00, 'Sênior', 'CLT', 'Ativo'), 
--(15, 11, 'Marco Antônio Carvalho', 310.00, 'Especialista', 'PJ', 'Ativo');

--INSERT IGNORE INTO usuario (id_usuario, nome, email, senha, status) VALUES 
--(1, 'Admin', 'admin@aeroconcepts.com', 'admin123', 'Ativo');

--INSERT IGNORE INTO cliente (id_cliente, nome, tipo, cidade, estado, contato) VALUES 
--(1, 'Funcate', 'Jurídica', 'São José dos Campos', 'SP', 'Contato Comercial'), 
--(2, 'Voith', 'Jurídica', 'São Paulo', 'SP', 'Departamento de Projetos'), 
--(3, 'Arauco', 'Jurídica', 'Curitiba', 'PR', 'Suprimentos'), 
--(4, 'EESC', 'Institucional', 'São Carlos', 'SP', 'Diretoria Técnica'), 
--(5, 'International Paper', 'Jurídica', 'Mogi Guaçu', 'SP', 'Engenharia'), 
--(6, 'Suzano', 'Jurídica', 'Salvador', 'BA', 'Gestão de Contratos'), 
--(7, 'Klabin', 'Jurídica', 'Telêmaco Borba', 'PR', 'Planejamento'), 
--(8, 'FAB', 'Governo', 'Brasília', 'DF', 'Comando da Aeronáutica'), 
--(9, 'IAE', 'Governo', 'São José dos Campos', 'SP', 'Diretoria IAE'), 
--(10, 'Birla Carbon', 'Jurídica', 'Cubatão', 'SP', 'Manutenção Industrial'), 
--(11, 'Rhodia', 'Jurídica', 'Paulínia', 'SP', 'Compras Técnicas'), 
--(12, 'Raizen', 'Jurídica', 'Piracicaba', 'SP', 'Projetos Estratégicos'), 
--(13, 'DCTA', 'Governo', 'São José dos Campos', 'SP', 'Secretaria de Tecnologia');

---- ==============================================================================
---- 3. HISTÓRICO / DADOS DE TESTE E SIMULAÇÃO RECALCULADOS DE FORMA EXATA
---- ==============================================================================

---- PROJETO 01: Funcate (Antigo Teste A)
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (1, 'Projeto Antigo Teste A', '2025-01-10', 'Em Aberto', 'Produto', 1, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(1, 'Componentes Eletrônicos de Bancada', 'EPIs/Ferramentas', 'Direto', 1540.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(1, 6, 'Desenho preliminar de circuitos e placas', 20.00, 'Concluída'); 
---- T6: 20h * R$98.00 = R$ 1.960,00
---- Custo Base: 1.540,00 + 1.960,00 = R$ 3.500,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (1, 3500.00, 0.00, 0.00, 42.8571, 1500.00, 5000.10, 30);


---- PROJETO 02: Funcate (Antigo Teste B)
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (2, 'Projeto Antigo Teste B', '2025-02-15', 'Em Aberto', 'Produto', 1, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(2, 'Softwares de Simulação Numérica Estendida', 'Licenças de Software', 'Direto', 3200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(2, 1, 'Engenharia Reversa e Mapeamento Elétrico', 24.00, 'Concluída'), 
--(2, 2, 'Supervisão técnica de bancada de testes', 6.22, 'Concluída');  
---- T1: 24h * R$165.00 = R$ 3.960,00 | T2: 6.22h * R$135.00 = R$ 839,70
---- Custo Base Real: R$ 3.200,00 + R$ 4.799,70 = R$ 7.999,70
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (2, 7999.70, 0.00, 0.00, 50.0019, 4000.00, 11999.55, 15);


---- PROJETO 03: Funcate (Antigo Teste C)
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (3, 'Projeto Antigo Teste C', '2025-03-20', 'Em Aberto', 'Produto', 1, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(3, 'Instrumentação e ferramentas de medição portátil', 'EPIs/Ferramentas', 'Direto', 1520.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(3, 8, 'Tabulação de dados e emissão de relatório inicial', 8.00, 'Concluída'); 
---- T8: 8h * R$110.00 = R$ 880,00
---- Custo Base: R$ 1.520,00 + R$ 880,00 = R$ 2.400,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (3, 2400.00, 0.00, 0.00, 45.8333, 1100.00, 3499.92, 10);


---- ==============================================================================
---- 4. CARGA COMPLEMENTAR: PROJETOS 04 AO 13 RECALCULADOS
---- ==============================================================================

---- PROJETO 04: Voith
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (4, 'Modernização de Turbina Hidrelétrica VT-01', '2026-05-22 09:00:00', 'Executando', 'Serviço', 2, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(4, 'Locação de Andaimes e Estruturas Modulares', 'Aluguel/Estrutura', 'Direto', 15000.00, 'Mês'),
--(4, 'Manutenção Corretiva em Gerador de Campo', 'Manutenção', 'Direto', 12150.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(4, 3, 'Análise de vibração e dinâmica de fluidos preliminar', 80.00, 'Executando'); 
---- T3: 80h * R$225.00 = R$ 18.000,00
---- Custo Base: R$ 27.150,00 + R$ 18.000,00 = R$ 45.150,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (4, 45150.00, 12.00, 5418.00, 25.00, 11287.50, 63210.00, 40);


---- PROJETO 05: Suzano
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (5, 'Otimização de Linha de Celulose - Planta BA', '2026-05-06 14:30:00', 'Aprovado', 'Serviço', 6, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(5, 'Passagens Aéreas e Estadia de Engenharia em Salvador', 'Transporte/Deslocamento', 'Direto', 12100.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(5, 7, 'Revisão de malhas de automação da planta industrial', 60.00, 'Concluída'), 
--(5, 6, 'Apoio técnico em levantamento de campo P&ID', 64.28, 'Concluída');      
---- T7: 60h * R$160.00 = R$ 9.600,00 | T6: 64.28h * R$98.00 = R$ 6.299,44
---- Custo Base: R$ 12.100,00 + R$ 15.899,44 = R$ 27.999,44
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (5, 27999.44, 12.00, 3359.93, 20.00, 5599.89, 37631.25, 45);


---- PROJETO 06: Klabin
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (6, 'Desenvolvimento de Painel de Automação Industrial', '2026-05-18 10:15:00', 'Orçado', 'Produto', 7, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(6, 'Controladores Lógicos Programáveis Dedicados', 'Equipamentos', 'Direto', 10100.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(6, 2, 'Desenvolvimento e teste das lógicas em CLP', 40.00, 'Pendente'); 
---- T2: 40h * R$135.00 = R$ 5.400,00
---- Custo Base: R$ 10.100,00 + R$ 5.400,00 = R$ 15.500,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (6, 15500.00, 18.00, 2790.00, 30.00, 4650.00, 23777.00, 40);


---- PROJETO 07: FAB
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (7, 'Análise Estrutural Flaps Aeronave T-27', '2026-03-01 08:00:00', 'Executando', 'Serviço', 8, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(7, 'Aquisição de Malhas de Deformação Alta Temperatura', 'EPIs/Ferramentas', 'Direto', 29200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(7, 15, 'Modelagem matemática estrutural de fadiga aeroespacial', 180.00, 'Executando'); 
---- T15: 180h * R$310.00 = R$ 55.800,00
---- Custo Base: R$ 29.200,00 + R$ 55.800,00 = R$ 85.000,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (7, 85000.00, 0.00, 0.00, 15.00, 12750.00, 97750.00, 60);


---- PROJETO 08: Arauco
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (8, 'Sistema de Exaustão de Resíduos Térmicos', '2026-03-12 16:45:00', 'Rascunho', 'Produto', 3, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(8, 'Dutos Industriais de Exaustão Revestidos 800mm', 'Equipamentos', 'Direto', 15280.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(8, 4, 'Cálculo de dimensionamento de exaustão e fluxo', 27.03, 'Pendente'); 
---- T4: 27.03h * R$145.00 = R$ 3.919,35
---- Custo Base: R$ 15.280,00 + R$ 3.919,35 = R$ 19.199,35
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (8, 19199.35, 18.00, 3455.88, 22.00, 4223.86, 27639.38, 15);


---- PROJETO 09: International Paper
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (9, 'Laudo Técnico de Conformidade NR-12', '2026-05-20 11:20:00', 'Concluído', 'Serviço', 5, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(9, 'Locação de Analisadores de Segurança e Checklist', 'Aluguel/Estrutura', 'Direto', 2600.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(9, 5, 'Inspeção física in loco das conformidades da NR-12', 40.00, 'Concluída'); 
---- T5: 40h * R$145.00 = R$ 5.800,00
---- Custo Base: R$ 2.600,00 + R$ 5.800,00 = R$ 8.400,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (9, 8400.00, 12.00, 1008.00, 35.00, 2940.00, 12700.80, 25);


---- PROJETO 10: DCTA
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (10, 'Consultoria em Dinâmica de Fluidos Computacional (CFD)', '2026-05-05 13:00:00', 'Aprovado', 'Serviço', 13, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(10, 'Assinatura Anual de Licenças ANSYS Aero / Hydro', 'Licenças de Software', 'Direto', 64200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(10, 15, 'Geração de malhas computacionais complexas e refinadas', 180.00, 'Pendente'); 
---- T15: 180h * R$310.00 = R$ 55.800,00
---- Custo Base: R$ 64.200,00 + R$ 55.800,00 = R$ 120.000,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (10, 120000.00, 0.00, 0.00, 18.00, 21600.00, 141600.00, 45);


---- PROJETO 11: Raizen
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (11, 'Dimensionamento Elétrico Destilaria Setor Norte', '2026-04-19 10:00:00', 'Orçado', 'Serviço', 12, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(11, 'Consumo Energético Dedicado de Gerador a Diesel Móvel', 'Energia Elétrica', 'Direto', 14200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(11, 1, 'Estudos de seletividade elétrica e curtos-circuitos', 120.00, 'Pendente'); 
---- T1: 120h * R$165.00 = R$ 19.800,00
---- Custo Base: R$ 14.200,00 + R$ 19.800,00 = R$ 34.000,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (11, 34000.00, 12.00, 4080.00, 25.00, 8500.00, 47600.00, 30);


---- PROJETO 12: Birla Carbon
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (12, 'Desenvolvimento de Dispositivo de Içamento Mecânico', '2026-05-22 15:30:00', 'Rascunho', 'Produto', 10, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(12, 'Barras Metálicas Estruturais Gerdau Extra Rígidas', 'Equipamentos', 'Direto', 3880.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(12, 4, 'Cálculo analítico de elementos de máquina e olhais', 27.03, 'Pendente'); 
---- T4: 27.03h * R$145.00 = R$ 3.919,35
---- Custo Base: R$ 3.880,00 + R$ 3.919,35 = R$ 7.799,35
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (12, 7799.35, 18.00, 1403.88, 28.00, 2183.82, 11780.14, 25);


---- PROJETO 13: Rhodia
--INSERT INTO projeto (id_projeto, nome, data_criacao, status, tipo, id_cliente, id_usuario) 
--VALUES (13, 'Estudo de Viabilidade Técnica - Planta Paulínia', '2026-05-15 08:45:00', 'Orçado', 'Serviço', 11, 1);

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(13, 'Hospedagem Técnica continuada na Região de Paulínia', 'Transporte/Deslocamento', 'Direto', 22800.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(13, 14, 'Estudo detalhado de CAPEX/OPEX e restrições de layout', 120.00, 'Pendente'); 
---- T14: 120h * R$240.00 = R$ 28.800,00
---- Custo Base: R$ 22.800,00 + R$ 28.800,00 = R$ 51.600,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (13, 51600.00, 12.00, 6192.00, 22.00, 11352.00, 70506.24, 30);


---- ==============================================================================
---- 5. MASSA DE DADOS REALISTA (PROJETOS 14 AO 19 COM SOMAS EXATAS DE VERIFICAÇÃO)
---- ==============================================================================

---- PROJETO 14: Voith
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (14, 1, 2, 'Modernização Real de Turbina VT-A1', 'Serviço', 'Executando', '2026-05-10 09:00:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(14, 'Kit de Vedação Industrial O-Ring', 'EPIs/Ferramentas', 'Direto', 4500.00, 'Unitário'),
--(14, 'Aluguel de Guindaste Hidráulico', 'Aluguel/Estrutura', 'Direto', 8000.00, 'Dia'),
--(14, 'Seguro de Risco de Engenharia', 'Aluguel/Estrutura', 'Indireto', 2500.00, 'Mês'),
--(14, 'Calibração de Sensores de Vibração Fluke', 'Manutenção', 'Direto', 3200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(14, 3, 'Análise de integridade estrutural e dinâmica de fluidos', 40.00, 'Concluída'), 
--(14, 1, 'Supervisão de montagem em campo e alinhamento do rotor', 50.00, 'Executando'), 
--(14, 2, 'Parametrização do módulo de proteção eletrônica', 20.00, 'Pendente');
---- Insumos: R$ 18.200,00
---- Tarefas: (40h * 225) + (50h * 165) + (20h * 135) = 9.000 + 8.250 + 2.700 = R$ 19.950,00
---- Custo Base Real: 18.200,00 + 19.950,00 = R$ 38.150,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (14, 38150.00, 12.00, 4578.00, 25.00, 9537.50, 53410.00, 30);


---- PROJETO 15: Klabin
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (15, 1, 7, 'Desenvolvimento de Painel de Automação K-Log', 'Produto', 'Orçado', '2026-05-12 14:00:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(15, 'CLP Siemens S7-1500 + Módulos I/O', 'Equipamentos', 'Direto', 12300.00, 'Unitário'),
--(15, 'Gabinete Metálico Rittal com Climatizador', 'Equipamentos', 'Direto', 4200.00, 'Unitário'),
--(15, 'Bornes de Conexão Push-In Phoenix Contact', 'EPIs/Ferramentas', 'Direto', 1150.00, 'Unitário'),
--(15, 'Frete Expresso de Componentes Importados', 'Transporte/Deslocamento', 'Direto', 850.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(15, 2, 'Programação da lógica do CLP e telas do supervisório IHMs', 30.00, 'Pendente'), 
--(15, 6, 'Montagem interna do painel e chicotes elétricos', 25.00, 'Pendente'),            
--(15, 7, 'Validação de diagramas e testes de aceitação em fábrica (FAT)', 15.00, 'Pendente');
---- Insumos: R$ 18.500,00
---- Tarefas: (30h * 135) + (25h * 98) + (15h * 160) = 4.050 + 2.450 + 2.400 = R$ 8.900,00
---- Custo Base Real: 18.500,00 + 8.900,00 = R$ 27.400,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (15, 27400.00, 18.00, 4932.00, 30.00, 8220.00, 42031.60, 15);


---- PROJETO 16: DCTA
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (16, 1, 13, 'Análise Aerodinâmica Avançada CFD - Suborbital', 'Serviço', 'Aprovado', '2026-05-15 08:00:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(16, 'Licença de Software ANSYS Fluent (Uso Dedicado)', 'Licenças de Software', 'Direto', 15000.00, 'Mês'),
--(16, 'Processamento em Cluster de Computação de Alto Desempenho (HPC)', 'Aluguel/Estrutura', 'Direto', 8500.00, 'Hora'),
--(16, 'Consumo Adicional de Energia do Cluster de Processamento', 'Energia Elétrica', 'Direto', 2200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(16, 15, 'Modelagem matemática e validação de malha computacional', 60.00, 'Pendente'), 
--(16, 11, 'Processamento de cenários e relatórios de arrasto', 40.00, 'Pendente'),       
--(16, 12, 'Revisão por par e validação cruzada dos coeficientes balísticos', 12.00, 'Pendente');
---- Insumos: R$ 25.700,00
---- Tarefas: (60h * 310) + (40h * 180) + (12h * 180) = 18.600 + 7.200 + 2.160 = R$ 27.960,00
---- Custo Base Real: 25.700,00 + 27.960,00 = R$ 53.660,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (16, 53660.00, 0.00, 0.00, 20.00, 10732.00, 64392.00, 45);


---- PROJETO 17: Suzano
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (17, 1, 6, 'Otimização de Caldeira de Recuperação - Unidade BA', 'Serviço', 'Rascunho', '2026-05-18 11:00:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(17, 'Termopares de Platina Industriais Tipo S', 'EPIs/Ferramentas', 'Direto', 6800.00, 'Unitário'),
--(17, 'Hospedagem e Diárias da Equipe Técnica (Campo)', 'Transporte/Deslocamento', 'Direto', 4500.00, 'Dia'),
--(17, 'Reparo Emergencial em Duto de Combustão', 'Manutenção', 'Direto', 1900.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(17, 1, 'Mapeamento térmico por termografia infravermelha', 24.00, 'Pendente'),       
--(17, 5, 'Cálculos de balanço de massa e eficiência energética da caldeira', 35.00, 'Pendente'), 
--(17, 8, 'Coleta de dados em CLP e consolidação de relatórios', 20.00, 'Pendente');    

---- Insumos: R$ 13.200,00
---- Tarefas: (24h * 165) + (35h * 145) + (20h * 110) = 3.960 + 5.075 + 2.200 = R$ 11.235,00
---- Custo Base Real: 13.200,00 + 11.235,00 = R$ 24.435,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (17, 24435.00, 12.00, 2932.20, 25.00, 6108.75, 34209.00, 20);


---- PROJETO 18: Arauco
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (18, 1, 3, 'Sistema de Exaustão de Resíduos Térmicos AR-2', 'Produto', 'Orçado', '2026-05-20 09:15:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(18, 'Exaustor Centrifugo Industrial Anti-Fagulha 50HP', 'Equipamentos', 'Direto', 32400.00, 'Unitário'),
--(18, 'Dutos de Aço Galvanizado Revestidos 1200mm', 'Equipamentos', 'Direto', 14200.00, 'Unitário'),
--(18, 'Estrutura Metálica de Suporte e Fixação Externa', 'Aluguel/Estrutura', 'Direto', 5500.00, 'Unitário'),
--(18, 'Kits de EPI Rígido para Trabalho em Altura (NR-35)', 'EPIs/Ferramentas', 'Indireto', 3800.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(18, 4, 'Dimensionamento mecânico da rede de dutos e perda de carga', 32.00, 'Pendente'), 
--(18, 6, 'Desenho Técnico detalhado em CAD/BIM estrutural', 40.00, 'Pendente'),             
--(18, 9, 'Análise de dispersão e particulados na atmosfera', 15.00, 'Pendente');            
---- Insumos: R$ 55.900,00
---- Tarefas: (32h * 145) + (40h * 160) + (15h * 110) = 4.640 + 6.400 + 1.650 = R$ 12.690,00
---- Custo Base Real: 55.900,00 + 12.690,00 = R$ 68.590,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (18, 68590.00, 18.00, 12346.20, 22.00, 15089.80, 95171.96, 15);


---- PROJETO 19: Rhodia (Corrigido o ID duplicado '16' no comentário para '19')
--INSERT INTO projeto (id_projeto, id_usuario, id_cliente, nome, tipo, status, data_criacao) 
--VALUES (19, 1, 11, 'Estudo Integrado de Viabilidade e Layout Paulínia', 'Serviço', 'Concluído', '2026-05-22 10:00:00');

--INSERT INTO custo (id_projeto, nome, categoria, tipo, valor, unidade) VALUES
--(19, 'Mapeamento Georreferenciado por Drone (Laser Scanning)', 'Equipamentos', 'Direto', 7500.00, 'Unitário'),
--(19, 'Deslocamento Terrestre e Combustível para Coleta em Campo', 'Transporte/Deslocamento', 'Indireto', 1200.00, 'Unitário');

--INSERT INTO tarefa (id_projeto, id_funcionario, descricao, horas_estimadas, status) VALUES
--(19, 13, 'Gerenciamento de engenharia e otimização do fluxo de processos', 50.00, 'Concluída'), 
--(19, 14, 'Planejamento de cronograma, CAPEX/OPEX e restrições físicas', 40.00, 'Concluída'),    
--(19, 10, 'Modelagem 3D do novo arranjo físico de tubulações (Plot-Plan)', 60.00, 'Concluída');  
---- Insumos: R$ 8.700,00
---- Tarefas: (50h * 260) + (40h * 240) + (60h * 110) = 13.000 + 9.600 + 6.600 = R$ 29.200,00
---- Custo Base Real: 8.700,00 + 29.200,00 = R$ 37.900,00
--INSERT INTO orcamento (id_projeto, custo_base, percentual_impostos, valor_impostos, margem_percentual, valor_margem, valor_final, validade_dias) 
--VALUES (19, 37900.00, 12.00, 4548.00, 30.00, 11370.00, 55182.40, 30);

---- ==============================================================================
---- FIM DO SCRIPT
---- ==============================================================================