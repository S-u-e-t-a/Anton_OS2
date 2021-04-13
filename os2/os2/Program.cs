using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace os2
{
    class Program
    {
        public static void PrintFilesStart(List<int> filesStart)
        {
            Console.WriteLine("Начала файлов:");
            var temp = new List<string> { "A", "B", "C", "D", "X", "Y", "XX", "YY" };
            for (int i = 0; i < filesStart.Count; i++)
            {
                if(i > 3) Console.WriteLine("{0}*) {1}", temp[i], filesStart[i]);
                else Console.WriteLine("{0}) {1}", temp[i], filesStart[i]);
                
            }
            Console.WriteLine();
        }
        static void PrintClusters(List<int> clusters)
        {
            Console.WriteLine("Кластеры:");
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i] == -1) Console.WriteLine("{0}) {1}", i, "eof");
                else if (clusters[i] == -2) Console.WriteLine("{0}) {1}", i, "bad");
                else if (clusters[i] == -3) Console.WriteLine("{0}) {1}", i, "");
                else Console.WriteLine("{0}) {1}", i, clusters[i]);
            }
            Console.WriteLine();
        }

        static void PrintClustersFinal(List<int> clusters)
        {
            Console.WriteLine("Кластеры:");
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i] == -1) Console.WriteLine("{0}) {1}", i, "eof");
                else if (clusters[i] == -2) Console.WriteLine("{0}) {1}", i, "bad");
                else if (clusters[i] == -3) continue;
                else Console.WriteLine("{0}) {1}", i, clusters[i]);
            }
            Console.WriteLine();
        }
        static void BuildChains(List<int> filesStart, List<List<int>> chains, List<int> clusters)
        {
            List<int> temp = new List<int>();
            for (int i = 0; i < filesStart.Count; i++)
            {
                temp.Add(filesStart[i]);
            }
            for (int i = 0; i < filesStart.Count; i++)
            {
                chains.Add(new List<int>());

                chains[i].Add(temp[i]);
                while (true)
                {
                    if (clusters[temp[i]] != -1)
                    {
                        chains[i].Add(clusters[temp[i]]);
                        temp[i] = clusters[temp[i]];
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
        public static void PrintChains(List<List<int>> chains)
        {
            Console.WriteLine("Цепочки файлов:");
            for (int i = 0; i < chains.Count; i++)
            {
                if (i > 3) Console.WriteLine("{0}*) {1}", i + 1, string.Join(",", chains[i]));
                else Console.WriteLine("{0}) {1}", i + 1, string.Join(",", chains[i]));

            }
            Console.WriteLine();
        }

        static void FixLostClusters(List<int> filesStart, List<int> clusters, List<List<int>> chains, List<int> saved)// Отчистка от "потерянных" кластеров
        {
            var tempList = new List<int>();

            for (int i = 0; i < chains.Count; i++)
            {
                for (int j = 0; j < chains[i].Count; j++)
                {
                    tempList.Add(chains[i][j]);
                }
            }

            for (int i = 2; i < clusters.Count - 1; i++)
            {
                if (!tempList.Contains(clusters[i]) && clusters[i] > 0)
                {
                    saved.Add(i);
                    if (clusters[i + 1] == -1) saved.Add(i + 1 - (i + 2));
                }

            }
            for (int k = 1; k < saved.Count; k++)
            {
                if (k == 1)
                {
                    filesStart.Add(saved[k - 1]);
                }
                else if (saved[k - 1] == -1)
                {
                    filesStart.Add(saved[k]);
                }

            }

        }
        static void FindBadAndEmptyClusters(List<int> clusters, List<int> badClusters, List<int> emptyClusters) // Поиск bad и пустых кластеров
        {
            badClusters.Clear();
            emptyClusters.Clear();
            for (int i = 2; i < clusters.Count; i++)
            {
                if (clusters[i] == -2)
                {
                    badClusters.Add(i);
                }
                if (clusters[i] == -3)
                {
                    emptyClusters.Add(i);
                }
            }
        }

        static void FixIntersection(List<int> clusters, List<List<int>> chains, List<int> emptyClusters) // Исправление пересечения цепочек
        {
            int currentCluster = 0;
            for (int i = 0; i < chains.Count; i++)
            {
                for (int j = i + 1; j < chains.Count; j++)
                {
                    var tempList = chains[i].Intersect(chains[j]).ToList();
                    if (tempList.Count > 0)
                    {
                        Console.WriteLine("Цепочка {0} пересекает {1}", string.Join(", ", chains[i]), string.Join(", ", chains[j]));
                        Console.WriteLine();
                        int firstChangingCluster = chains[j].Count - tempList.Count;
                        for (int k = 0; k < chains[j].Count; k++)
                        {
                            if (tempList.Any(cluster => cluster == chains[j][k]))
                            {
                                if (k == firstChangingCluster)
                                {
                                    clusters[chains[j][firstChangingCluster - 1]] = emptyClusters[currentCluster];
                                }
                                if (k == chains[j].Count - 1)
                                {
                                    clusters[emptyClusters[currentCluster]] = -1;
                                    currentCluster++;
                                }
                                else
                                {
                                    clusters[emptyClusters[currentCluster]] = emptyClusters[currentCluster + 1];
                                    currentCluster++;
                                }
                            }
                        }
                    }
                    tempList.Clear();
                }
            }
        }
        static void AnalyzeChains(List<List<int>> chains) // Анализ цепочек
        {
            for (int i = 0; i < chains.Count; i++)
            {
                int counter = 0;
                for (int j = 1; j < chains[i].Count; j++)
                {
                    if (chains[i][j] - 1 == chains[i][j - 1])
                    {
                        counter++;
                        if (counter == chains[i].Count - 1)
                        {
                            Console.WriteLine("{0}) Файл фрагментирован.", i + 1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("{0}) Файл дефрагментирован.", i + 1);
                        break;
                    }
                }
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var filesStart = new List<int> { 8, 16, 18, 29 };
            var clusters = new List<int>() { -3, -3, -3, 4, -1, -3, -1, -3, 9, 10,
                11, -1, -3, 27, -2, -3, 17, 3, 13, 20, 21, 22, -1, -3, -3, -2, -3, 6, -3, 30, 3, -3 };
            var chains = new List<List<int>>();
            var savedClusters = new List<int>();
            var badClusters = new List<int>();
            var emptyClusters = new List<int>();
            PrintFilesStart(filesStart);
            PrintClusters(clusters);
            BuildChains(filesStart, chains, clusters);
            PrintChains(chains);

            for (int i = 0; i < chains.Count; i++)
            {
                filesStart[i] = chains[i][0];
            }

            FixLostClusters(filesStart, clusters, chains, savedClusters);

            chains.Clear();
            BuildChains(filesStart, chains, clusters);

            FindBadAndEmptyClusters(clusters, badClusters, emptyClusters);

            FixIntersection(clusters, chains, emptyClusters);

            chains.Clear();
            BuildChains(filesStart, chains, clusters);

            Console.WriteLine("Результат.");
            PrintFilesStart(filesStart);
            Console.WriteLine("Исправленные кластеры.");
            PrintClustersFinal(clusters);

            Console.WriteLine("Исправленные цепочки файлов.");
            PrintChains(chains);

            AnalyzeChains(chains);
            Console.ReadLine();
        }
    }
}
