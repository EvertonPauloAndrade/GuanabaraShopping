# 🛒 ClienteManager – CRUD com ASP.NET Core MVC

Sistema web para gerenciamento de clientes, desenvolvido com **ASP.NET Core MVC**, persistência de dados em arquivos **JSON**, autenticação por cookies e suporte a **upload de imagem**.

## 🚀 Funcionalidades

- ✅ Cadastro, edição e exclusão de clientes
- ✅ Registro e login de usuários
- ✅ Autenticação via cookie
- ✅ Upload e exibição de imagem do cliente
- ✅ Validações de formulário com Razor
- ✅ Interface com Bootstrap 5
- ✅ Persistência em arquivos `.json` (sem banco de dados)

---

## 🖼️ Demonstração da interface

![image](https://github.com/user-attachments/assets/d98b0507-60b3-422e-b800-eb145f81b05c)


---

## 🛠️ Tecnologias Utilizadas

- ASP.NET Core MVC
- C#
- HTML5 + Razor
- Bootstrap 5
- Autenticação com Cookies
- Armazenamento local em JSON

---

## 🗂️ Estrutura do Projeto

ClienteManager/
├── Controllers/
│ ├── ClienteController.cs
│ └── AccountController.cs
├── Data/
│ ├── JsonDataService.cs
│ ├── users.json
│ └── clients.json
├── Models/
│ ├── Cliente.cs
│ └── ApplicationUser.cs
├── ViewModels/
│ ├── LoginViewModel.cs
│ └── RegisterViewModel.cs
├── Views/
│ ├── Cliente/
│ ├── Account/
│ ├── Home/
│ └── Shared/
│ └── _Layout.cshtml
├── wwwroot/
│ └── imagens/ # Armazena imagens dos clientes
├── Program.cs
└── appsettings.json


---

## 🧪 Como rodar o projeto

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Editor como [Visual Studio Code](https://code.visualstudio.com/) ou [Rider](https://www.jetbrains.com/rider/)

### Passos


# Clone o repositório
git clone https://github.com/SeuUsuario/ClienteManager.git
cd ClienteManager

# Restaure pacotes e rode
dotnet restore
dotnet run

O sistema estará disponível em:

http://localhost:5000

🔐 Acesso ao sistema

    Ao abrir pela primeira vez, cadastre um usuário.

    O login será armazenado em Data/users.json.

📂 Upload de imagens

    As imagens são salvas em /wwwroot/imagens/.

    O nome do arquivo é gerado com Guid para evitar conflitos.

📄 Licença

Projeto de código aberto para fins educacionais.
🙋‍♂️ Autor

Feito com ❤️ por Everton Paulo
