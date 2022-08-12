using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows;

namespace TesteAD2
{
    internal class Program
    {

        private static bool AuthenticateUser(string domainName, string userName, string password)
        {
            bool ret = false;

            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + domainName, userName, password);
                DirectorySearcher dsearch = new DirectorySearcher(de);
                SearchResult results = null;

                results = dsearch.FindOne();

                ret = true;
                Console.WriteLine("OK!");
            }
            catch
            {
                ret = false;
                Console.WriteLine("Não!");
            }

            return ret;
        }


        private static void GetAllGroups()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.Sort = new SortOption("name", SortDirection.Ascending);
            ds.PropertiesToLoad.Add("name");
            ds.PropertiesToLoad.Add("memberof");
            ds.PropertiesToLoad.Add("member");

            ds.Filter = "(&(objectCategory=Group))";

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["name"].Count > 0)
                    Console.WriteLine(sr.Properties["name"][0].ToString());

                if (sr.Properties["memberof"].Count > 0)
                {
                    Console.WriteLine("  Member of...");
                    foreach (string item in sr.Properties["memberof"])
                    {
                        Console.WriteLine("    " + item);
                    }
                }
                if (sr.Properties["member"].Count > 0)
                {
                    Console.WriteLine("  Members");
                    foreach (string item in sr.Properties["member"])
                    {
                        Console.WriteLine("    " + item);
                    }
                }
                Console.WriteLine("==============================================");
            }
        }

        private static void GetAdditionalUserInfo()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.PropertiesToLoad.Add("distinguishedName");// Todo conteudo

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["distinguishedName"].Count > 0)
                {
                    Console.Write("Nome Distindo = ");
                    Console.WriteLine(sr.Properties["distinguishedName"][0].ToString() + " || ");
                }
                Console.WriteLine("==========================================");
            }
        }

        private static void GetAllUsers()
        {
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new
            DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.Filter = "(&(objectCategory=User)(objectClass=person))";

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                /*
                use o index (0) é necessário, Cada propriedade que você recupera precisa usar o 
                índice de 0, ou se essa propriedade for um grupo, você pode percorrer a matriz 
                dessa propriedade incrementando o número do índice até chegar ao final da matriz
                */
                Console.WriteLine(sr.Properties["name"][0].ToString());
                Console.WriteLine("==========================================");
            }
        }

        private static string GetCurrentDomainPath()
        {
            DirectoryEntry de = new DirectoryEntry("LDAP://RootDSE");
            return "LDAP://" + de.Properties["defaultNamingContext"][0].ToString();

            //LDAP://DC=recognition,DC=com,DC=br
        }
        [STAThread]

        private static void teste()
        {
            string login = "";
            string allInfo = "";
            SearchResultCollection results;
            DirectorySearcher ds = null;
            DirectoryEntry de = new DirectoryEntry(GetCurrentDomainPath());

            ds = new DirectorySearcher(de);
            ds.PropertiesToLoad.Add("userPrincipalName");// Login AD
            ds.PropertiesToLoad.Add("distinguishedName");// Todo conteudo

            results = ds.FindAll();

            foreach (SearchResult sr in results)
            {
                if (sr.Properties["userPrincipalName"].Count > 0)
                {
                    //login = login + sr.Properties["userPrincipalName"][0].ToString().Insert(index, ";") + "\n"; adicionando o caracter ; antes do @
                    //index = sr.Properties["userPrincipalName"][0].ToString().IndexOf("@");
                    login = sr.Properties["userPrincipalName"][0].ToString().Replace("@recognition.com.br", "");

                    if (login.Contains(Environment.UserName))
                    {
                        if (sr.Properties["distinguishedName"].Count > 0)
                        {
                            allInfo = sr.Properties["distinguishedName"][0].ToString().Replace("CN=", "").Replace("OU=", " Grupo - ").Replace("DC=", " ");
                            Console.WriteLine($"Usuário: {login}\nInformações de grupo: {allInfo}\n");
                            Console.WriteLine("Encontrado !!");

                        }
                    }
                    
                }

                
            }

            //Clipboard.SetText(login);
        }


        [STAThread]
        static void Main(string[] args)
        {
            //GetAdditionalUserInfo();
            teste();
            Console.Read();
        }
    }
}
