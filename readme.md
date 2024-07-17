# JuntoTest API

A JuntoTest API é uma aplicação que se integra com um banco de dados SQL Server e permite a autenticação de usuários com diferentes níveis de acesso.



## Configuração do Banco de Dados

------------------------------

Para instanciar uma versão do SQL Server usando Docker, siga os passos abaixo:

1. Abra o terminal ou prompt de comando.

2. Utilize ``docker pull mcr.microsoft.com/mssql/server:2022-latest`` para baixar a imagem do SQL Server

3. Execute o seguinte comando para criar um contêiner Docker com o SQL Server:
   ``   docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Junto123*" -p 1433:1433 --name JuntoTest --hostname sql1 -d mcr.microsoft.com/mssql/server:2022-latest``
   Isso criará um contêiner chamado “JuntoTest” com o SQL Server em execução na porta 1433.

4. Aguarde até que o contêiner esteja pronto para aceitar conexões.

Usuário Admin para Testes Iniciais
----------------------------------

* **Usuário:** admin@example.com
* **Senha:** Admin@123

Apenas usuários com perfil de administrador podem interagir com a API.


## Criação da Tabela (Migration)

-----------------------------

Se você ainda não possui a migração (tabela) no seu banco de dados, siga os passos abaixo:

1. Certifique-se de que o Entity Framework Core CLI (`dotnet ef`) esteja instalado. Caso contrário, instale-o com o seguinte comando:
      ``dotnet tool install --global dotnet-ef``

2. No diretório raiz do seu projeto, execute o seguinte comando para criar uma nova migração:
   ``dotnet ef migrations add NomeDaMigration``
   Substitua “NomeDaMigration” pelo nome desejado para a migração.

3. Em seguida, aplique a migração ao banco de dados:
     ``dotnet ef database update``
   
   

Executando a Aplicação
----------------------

Após o banco de dados estar configurado e a migração aplicada, siga os passos abaixo:

1. Inicie a aplicação.
2. Insira o usuário e senha fornecidos acima para obter um token de autenticação.
3. Autentique-se no Swagger ou na API conforme necessário.
