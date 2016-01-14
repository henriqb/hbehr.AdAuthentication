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
```C#
AdAuthenticator adAuthenticator = new AdAuthenticator();
```
Configure in code

*Configurar pelo código*,
```C#
string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
string ldapDomain = "radixengrj";
adAuthenticator
	.ConfigureSetLdapPath(ldapPath)
	.ConfigureLdapDomain(ldapDomain);
```
Configure in .config, just add the folowing keys to the LdapPath/Domain

*Configurar pelo .config* adicionar as chaves para o LdapPath/Domain
```xml
<configuration>
  <appSettings>
    <add key="LdapPath" value="LDAP://DC=radixengrj,DC=matriz" />
    <add key="LdapDomain" value="radixengrj" />
  </appSettings>
</configuration>
```

If you are using Windows Authentication, to fetch the user using the system:

*Se estiver usando windows Authentication, para buscar o usuário que está usando o sistema:*
```C#
AdUser adUser = adAuthenticator.GetUserFromAd();
```  
If you want to authenticate via login / password, use the function:

*Caso queira autenticar via login/senha, usar a função:*
```C#
string login = "henrique.behr";
string password = "*******";
AdUser adUser = adAuthenticator.SearchUserBy(login, password);
```
If you want to search an user in the AD using the login:

*Para buscar um usuário no AD pelo login:*
```C#
string login = "henrique.behr"
AdUser adUser = adAuthentication.GetUserFromAdBy(login);
```

Search for all users or groups in the AD:

*Buscar por todos os usuários ou grupos do AD*
```C#
IEnumerable<AdGroup> groups = adAuthenticator.GetAdGroups();
IEnumerable<AdUser> users = adAuthenticator.GetAllUsers();
```

Supports *method-chain* :

*Suporta method-chain:*
```C#
string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
string ldapDomain = "radixengrj";
AdUser adUser = new AdAuthenticator()
	.ConfigureSetLdapPath(ldapPath)
	.ConfigureLdapDomain(ldapDomain)
	.GetUserFromAd();
```

##Data Structure

User Ad:

*Usuário Ad*
```C#
public class ADUser
{
	public string Name { get; private set; }
	public string Login { get; private set; }
	public string Mail { get; private set; }
	public string Phone { get; private set; }
	public string Company { get; private set; }
	public IEnumerable<AdGroup> AdGroups { get; private set; }
}
```
Ad Groups:

*Grupos Ad*
```C#
public class AdGroup
{
	public string Code { get; set; }
	public string Name { get; set; }
}
```

#### Error Types

The exceptions thrown are of type: **AdException**, they come with an identification *AdError* errors that are treated include:

*As exceções lançadas são do tipo: **AdException**, elas vem com uma identificação AdError os erros que são tratados incluem:*

         Generic Error *not detected*
         InvalidLdapDomain * Invalid Ldap Domain, verify network configuration *
         UserNotFound * Login user not found in AD *
         IncorrectPassword * Login found but incorrect password *
