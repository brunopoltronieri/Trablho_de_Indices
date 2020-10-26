using System;

namespace trabalho_arquivos
{
    class Program
    {
        static string menu()
        {
            Console.WriteLine("Digite o número da função desejada:");
            Console.WriteLine("1 - Montar arquivos");
            Console.WriteLine("2 - Pesquisa tweet (ID)");
            Console.WriteLine("3 - Pesquisa hashtag");
            Console.WriteLine("4 - Hipotese");
            Console.WriteLine("0 - Sair");

            return Console.ReadLine();
        }

        static void Main(string[] args)
        {
            Arquivo arquivo = new Arquivo();
            string opcao;
            opcao = Program.menu();
            while (opcao != "0")
            {
                if (opcao == "1")
                {
                    arquivo.MontaArquivos();
                }
                else if (opcao == "2")
                {
                    arquivo.PesquisaIndice();
                }
                else if (opcao == "3")
                {
                    arquivo.PesquisaHashtag();
                }
                else if (opcao == "4")
                {
                    arquivo.Hipotese();
                }
                else if (opcao == "0")
                {
                    break;
                }

                Console.WriteLine("\n\n\n");
                opcao = Program.menu();
            }
        }
    }
}
