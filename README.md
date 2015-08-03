AdAuthenticator
==============

Implements Ad authenticator for asp.net, fast and convenient. Can be used with windows authentication and login / password.

*Implementa autenticador Ad para asp.net, rápido e prático. Pode ser usado com windows authentication ou login/senha.*

## Instalation
Get it on nuget: https://www.nuget.org/packages/hbehr-AdAuthentication/

	PM> Install-Package hbehr-AdAuthentication

## How to use
Create a new instance of the class

*Criar uma nova instância da classe*

      AdAuthenticator adAuthenticator = new AdAuthenticator();

Configure

*Configurar*,

      string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
      string ldapDomain = "radixengrj";
      adAuthenticator
          .ConfigureSetLdapPath(ldapPath)
          .ConfigureLdapDomain(ldapDomain);
          
If you are using Windows Authentication, to fetch the user using the system:

*Se estiver usando windows Authentication, para buscar o usuário que está usando o sistema:*

       AdUser adUser = adAuthenticator.GetUserFromAd();
      
If you want to authenticate via login / password, use the function:

*Caso queira autenticar via login/senha, usar a função:*

      string login = "henrique.behr";
      string password = "*******";
      AdUser adUser = adAuthenticator.SearchUserBy(login, password);
      
If you want to search an user in the AD Directory using the login:

*Para buscar um usuário no Diretório AD pelo login:*

	 string login = "henrique.behr"
	 AdUser adUser = adAuthentication.GetUserFromAdBy(login);

	  
Supports *method-chain* :

*Suporta method-chain:*

      string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
      string ldapDomain = "radixengrj";
      AdUser adUser = new AdAuthenticator()
          .ConfigureSetLdapPath(ldapPath)
          .ConfigureLdapDomain(ldapDomain)
          .GetUserFromAd();
          
##Data Structure

User Ad:

*Usuário Ad*

       public class ADUser
       {
           public string Name { get; private set; }
           public string Login { get; private set; }
           public IEnumerable <AdGroup> AdGroups { get; private set; }
       }
      
Ad Groups:

*Grupos Ad*

       public class AdGroup
       {
           public string Name {get; set; }
       }
      
#### Error Types

The exceptions thrown are of type: **AdException**, they come with an identification *AdError* errors that are treated include:

*As exceções lançadas são do tipo: **AdException**, elas vem com uma identificação AdError os erros que são tratados incluem:*

         Generic Error *not detected*
         InvalidLdapDomain * Invalid Ldap Domain, verify network configuration *
         UserNotFound * Login user not found in AD *
         IncorrectPassword * Login found but incorrect password *
