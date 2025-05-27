HoleriteAPI
API RESTful desenvolvida em .NET para gerenciamento de holerites de funcionários. Esta aplicação faz parte de um sistema maior utilizado em ambiente corporativo para o controle e distribuição interna de holerites, integrando-se ao frontend Holerite App.

✨ Funcionalidades
Upload e armazenamento de arquivos PDF de holerites

Extração e associação automática do nome do funcionário

Controle de acesso por perfil de usuário

Consulta dos holerites por funcionário

Integração com frontend Angular

Deploy na infraestrutura de intranet da empresa

🧱 Tecnologias Utilizadas
.NET 8 / ASP.NET Core

Entity Framework Core

SQL Server


Swagger (para documentação da API)

Autenticação baseada em Token JWT

📦 Estrutura do Projeto
bash
Copiar
Editar
HoleriteAPI/
│
├── Controllers/        # Endpoints HTTP
├── Services/           # Lógica de negócios
├── Models/             # Entidades e DTOs
├── Data/               # Contexto do EF Core
├── Migrations/         # Migrations do banco
├── Program.cs          # Entry point
└── appsettings.json    # Configurações da aplicação

🚀 Como Executar Localmente
Pré-requisitos
.NET 6 SDK

Banco de dados PostgreSQL (ou outro especificado)

Visual Studio ou VS Code

Passos
bash
Copiar
Editar
# Clonar o repositório
git clone https://github.com/danielvieiralopes/holeriteAPI.git
cd holeriteAPI

# Restaurar pacotes
dotnet restore

# Aplicar migrations (ajuste connection string no appsettings.json)
dotnet ef database update

# Executar a aplicação
dotnet run
A API estará disponível em https://localhost:5258 ou http://localhost:5000.

Swagger
Após iniciar a aplicação, acesse https://localhost:5258/swagger para visualizar a documentação da API.

🔐 Segurança
Endpoints protegidos com autenticação JWT

Controle de acesso por perfis

Futuro recurso: recuperação de senha gerenciada apenas por administradores

📌 Próximos Passos
 Implementar "Esqueci minha senha" (apenas via administrador)

 Enviar notificações de novo holerite via e-mail interno

 Adicionar logs e auditoria de acesso

 Melhorar tratamento de erros com middleware customizado

 Adicionar testes unitários e de integração

🖥️ Deploy
O sistema está atualmente hospedado na infraestrutura interna (intranet) da empresa, garantindo acesso restrito aos colaboradores autorizados.



📄 Licença
MIT