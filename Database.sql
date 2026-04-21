-- Criar banco de dados
CREATE DATABASE GoodHamburgerDB;
GO

USE GoodHamburgerDB;
GO

-- Tabela de cardápio
CREATE TABLE Cardapio (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL UNIQUE,
    Preco DECIMAL(10,2) NOT NULL,
    Tipo INT NOT NULL, -- 1: Sanduiche, 2: Acompanhamento
    Categoria NVARCHAR(50) NOT NULL,
    Ativo BIT DEFAULT 1
);

-- Tabela de pedidos
CREATE TABLE Pedidos (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    DataCriacao DATETIME2 NOT NULL,
    DataAtualizacao DATETIME2 NULL,
    Status NVARCHAR(50) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    Desconto DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL
);

-- Tabela de itens do pedido
CREATE TABLE ItensPedido (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    PedidoId UNIQUEIDENTIFIER NOT NULL,
    Nome NVARCHAR(100) NOT NULL,
    Tipo INT NOT NULL,
    PrecoUnitario DECIMAL(10,2) NOT NULL,
    Quantidade INT NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id) ON DELETE CASCADE
);

-- Inserir cardápio
INSERT INTO Cardapio (Nome, Preco, Tipo, Categoria) VALUES 
('X Burger', 5.00, 1, 'Sanduíches'),
('X Egg', 4.50, 1, 'Sanduíches'),
('X Bacon', 7.00, 1, 'Sanduíches'),
('Batata frita', 2.00, 2, 'Acompanhamentos'),
('Refrigerante', 2.50, 2, 'Acompanhamentos');

-- Índices
CREATE INDEX IX_ItensPedido_PedidoId ON ItensPedido(PedidoId);
CREATE INDEX IX_Pedidos_DataCriacao ON Pedidos(DataCriacao DESC);