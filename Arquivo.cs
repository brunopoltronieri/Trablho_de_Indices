using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace trabalho_arquivos
{
    class Arquivo
    {
        void AddHashtagIndex(SortedDictionary<string, List<uint>> dict, string index, uint offset)
        {
            if (dict.TryGetValue(index, out var offsetList))
            {
                offsetList.Add(offset);
            }
            else
            {
                var newOffsetList = new List<uint>
                {
                    offset
                };

                dict.Add(index, newOffsetList);
            }
        }

        bool IndexAlreadyExists(SortedDictionary<string, uint> dict, string index)
        {
            return dict.TryGetValue(index, out var _);
        }

        //System.IO.FileStream arquivoSaida { get; set; }
        System.IO.FileStream arquivoEntrada { get; set; }

        BinaryReader reader;      //oficial


        public void BuscaArquivos()
        {
            //função para teste
            //http://www.linhadecodigo.com.br/artigo/3684/trabalhando-com-arquivos-e-diretorios-em-csharp.aspx
            string[] arquivos = Directory.GetFiles("../../../eleicoes/", "*.raw", SearchOption.AllDirectories);

            int cont = 1;
            foreach (string arq in arquivos)
            {
                //Console.WriteLine(cont + " - " + arq);
                Console.WriteLine(arq);
                cont++;
            }
        }

        public void MontaArquivos()
        {
            string[] arquivos = Directory.GetFiles("../../../eleicoes/", "*.raw", SearchOption.AllDirectories);
            var arquivoSaida = File.OpenWrite("../../../tweets.data");
            var tweetsWriter = new BinaryWriter(arquivoSaida);

            var indexesDict = new SortedDictionary<string, uint>();
            var hashtagsDict = new SortedDictionary<string, List<uint>>();
            const int recordSize = 1356;
            byte[] buffer = new byte[recordSize];
            uint index = 0;
            int contadorTweets = 0;
            int contadorTweetsTotal = 0;

            foreach (var arq in arquivos)
            {
                this.arquivoEntrada = File.OpenRead(arq);
                this.reader = new BinaryReader(arquivoEntrada);

                Console.WriteLine("Arquivo: " + arq + " acrescentado!");

                while (reader.Read(buffer, 0, buffer.Count()) > 0)
                {
                    var linha = Encoding.UTF8.GetString(buffer);
                    string idTwitter = linha.Substring(0, 19);
                    string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

                    contadorTweetsTotal++;
                    if (IndexAlreadyExists(indexesDict, idTwitter))
                        continue;

                    contadorTweets++;
                    //-------NOVA separa hashtags da mensagem-------//
                    int startIndex = 0;
                    while (msgTwitter.IndexOf('#') != -1)
                    {
                        int indiceHash = msgTwitter.IndexOf('#', startIndex);
                        var hashSeparada = string.Concat(msgTwitter.Skip(indiceHash + 1).TakeWhile(c => char.IsLetterOrDigit(c))).ToLower();
                        msgTwitter = msgTwitter.Remove(0, indiceHash + 1);

                        AddHashtagIndex(hashtagsDict, hashSeparada, index * recordSize);

                        //int indiceHash = msgTwitter.IndexOf('#', startIndex);
                        //int indiceEspaco = msgTwitter.IndexOf(' ', indiceHash);
                        //indiceEspaco = (indiceEspaco == -1 ? msgTwitter.Length : indiceEspaco) - indiceHash;
                        //string hashSeparada = msgTwitter.Substring(indiceHash + 1, indiceEspaco);
                        //msgTwitter = msgTwitter.Remove(0, indiceHash);

                        //AddHashtagIndex(hashtagsDict, hashSeparada, index * recordSize);
                    }
                    //-------NOVA separa hashtags da mensagem-------//

                    tweetsWriter.Write(buffer);

                    indexesDict.Add(idTwitter, index * recordSize);
                    index++;
                }

                arquivoEntrada.Close();
            }




            //String entrada = "../../../julio_01_02_19.raw";
            //this.arquivoEntrada = File.OpenRead(entrada);
            //var arquivoSaida = File.OpenWrite("../../../tweets.data");
            //this.reader = new BinaryReader(arquivoEntrada);
            //var tweetsWriter = new BinaryWriter(arquivoSaida);

            //while (reader.Read(buffer, 0, buffer.Count()) > 0)
            //{
            //    var linha = Encoding.UTF8.GetString(buffer);
            //    string idTwitter = linha.Substring(0, 19);
            //    string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

            //    if (IndexAlreadyExists(indexesDict, idTwitter))
            //        continue;

            //    //-------separa hashtags da mensagem-------//
            //    var msgSeparada = msgTwitter.Split('#');
            //    for (int i = 1; i < msgSeparada.Count(); i++)
            //    {
            //        string hashtagSeparada = msgSeparada[i].Split(' ')[0];
            //        if (!Char.IsLetterOrDigit(hashtagSeparada.Last()))
            //        {
            //            hashtagSeparada = hashtagSeparada.Substring(0, hashtagSeparada.Count() - 1);
            //        }

            //        AddHashtagIndex(hashtagsDict, hashtagSeparada, index * recordSize);
            //    }
            //    //-------separa hashtags da mensagem-------//

            //    tweetsWriter.Write(buffer);

            //    indexesDict.Add(idTwitter, index * recordSize);
            //    index++;
            //}

            var indexesFile = File.OpenWrite("../../../id.idx");
            var indexWriter = new BinaryWriter(indexesFile);
            var indexesBuffer = indexesDict.Serialize();
            indexWriter.Write(indexesBuffer);
            indexWriter.Close();
            indexesFile.Close();

            var hashtagsFile = File.OpenWrite("../../../hashtags.idx");
            var hashtagsWriter = new BinaryWriter(hashtagsFile);
            var hashtagsBuffer = hashtagsDict.Serialize();
            hashtagsWriter.Write(hashtagsBuffer);
            hashtagsWriter.Close();
            hashtagsFile.Close();

            arquivoSaida.Close();

            Console.WriteLine("\nArquivos: \n-Tweets\n-Índice de ID\n-Índice de Hashtag\nGerados com sucesso!");
            Console.WriteLine("\nTweets extraidos: " + contadorTweetsTotal);
            double contTweets = contadorTweets;
            double contTweetsTotal = contadorTweetsTotal;
            double percentual = (contTweets / contTweetsTotal) * 100;
            Console.WriteLine("Tweets arquivados: " + contadorTweets + " (" + Math.Round(percentual, 2) + "%)");
        }

        public void PesquisaIndice()
        {
            Console.WriteLine("\nDigite o ID a ser pesquisado: ");
            var idToSearch = Console.ReadLine();

            var buff = File.ReadAllBytes("../../../id.idx");
            var indexesDict = (SortedDictionary<string, uint>)buff.DeSerialize();
            if (indexesDict.TryGetValue(idToSearch, out var index))
            {
                string entrada = "../../../tweets.data";
                this.arquivoEntrada = File.OpenRead(entrada);
                arquivoEntrada.Seek(index, SeekOrigin.Begin);

                this.reader = new BinaryReader(arquivoEntrada);

                const int recordSize = 1356;
                byte[] buffer = new byte[recordSize];

                reader.Read(buffer, 0, buffer.Count());

                string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

                Console.WriteLine("Mensagem do Twitter encontrado: \n" + msgTwitter);
            }
            else
            {
                Console.WriteLine($"Não foi possível encontrar o Tweet de ID \"{idToSearch}\"");
            }
        }

        public void PesquisaHashtag()
        {
            Console.WriteLine("\nDigite a Hashtag a ser pesquisada: ");
            var HashtagToSearch = Console.ReadLine().ToLower();

            var buff = File.ReadAllBytes("../../../hashtags.idx");
            var hashtagsDict = (SortedDictionary<string, List<uint>>)buff.DeSerialize();
            if (hashtagsDict.TryGetValue(HashtagToSearch, out var hashtags))
            {
                string entrada = "../../../tweets.data";
                this.arquivoEntrada = File.OpenRead(entrada);

                Console.WriteLine($"{hashtags.Count()} tweets encontrados");

                foreach (var hashtag in hashtags)
                {
                    arquivoEntrada.Seek(hashtag, SeekOrigin.Begin);

                    this.reader = new BinaryReader(arquivoEntrada);

                    const int recordSize = 1356;

                    byte[] buffer = new byte[recordSize];

                    reader.Read(buffer, 0, buffer.Count());

                    string msgTwitter = Encoding.UTF8.GetString(buffer.Skip(219).Take(1120).ToArray());

                    Console.WriteLine($"Tweet: {msgTwitter}");
                }
            }
            else
            {
                Console.WriteLine($"Não foi possível encontrar a hashtag \"{HashtagToSearch}\"");
            }
        }

        public void Hipotese()
        {
            var buff = File.ReadAllBytes("../../../hashtags.idx");
            var hashtagsDict = (SortedDictionary<string, List<uint>>)buff.DeSerialize();
            var hashtagsPopularity = new SortedDictionary<string, uint>();

            var hashtagsArquivo = new StreamReader(File.OpenRead("../../../hashtags.txt"));

            var l = hashtagsArquivo.ReadLine();
            while (l != null)
            {
                var hashtag = l.Substring(1);
                if (hashtagsDict.TryGetValue(hashtag, out var tweets))
                {
                    hashtagsPopularity.Add(hashtag, (uint)tweets.Count);
                }

                l = hashtagsArquivo.ReadLine();
            }

            var i = 1;
            foreach (var time in hashtagsPopularity.OrderByDescending(h => h.Value).Take(5))
            {
                Console.WriteLine($"#{i}: time {time.Key}, {time.Value} tweets");
                i++;
            }
        }

    }
}
