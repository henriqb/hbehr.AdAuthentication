AdAuthenticator
==============

Implementa autenticador Ad web, rápido e prático. Pode ser usado com windows authentication ou login/senha.


##Como Usar
Criar uma nova instância da classe

      AdAuthenticator adAuthenticator = new AdAuthenticator();

Configurar

      string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
      string ldapDomain = "radixengrj";
      adAuthenticator
          .ConfigureSetLdapPath(WebConfigConstants.LdapPath)
          .ConfigureLdapDomain(WebConfigConstants.LdapDomain)
          
Se estiver usando windows Authentication, para buscar o usuário que está usando o sistema:

      AdUser adUser = adAuthenticator.GetUserFromAd();
      
Caso queira autenticar via login/senha, usar a função:

      string login = "henrique.behr";
      string password = "*******";
      AdUser adUser = adAuthenticator.SearchUserBy(login, password);
      
      
Suporta *method-chain*:
      
      string ldapPath = "LDAP://DC=radixengrj,DC=matriz";
      string ldapDomain = "radixengrj";
      AdUser adUser = new AdAuthenticator()
          .ConfigureSetLdapPath(ldapPath)
          .ConfigureLdapDomain(ldapDomain)
          .GetUserFromAd();
          
##Estrutura de Dados

Usuário Ad:

      public class AdUser
      {
          public string Name { get; private set; }
          public string Login { get; private set; }
          public IEnumerable<AdGroup> AdGroups { get; private set; }
      }
      
Grupos do Ad:

      public class AdGroup
      {
          public string Name { get; set; }
      }
      
####Tipos de erro

As exceções lançadas são do tipo: **AdException**, elas vem com uma identificação *AdError* os erros que são tratados incluem:


        Generic *Erro não detectado*
        InvalidLdapDomain *Ldap Domain invalido, verificar configuração de erro*
        UserNotFound *Login de usuário não encontrado no AD*
        IncorrectPassword *Login encontrado, mas senha incorreta*
    
