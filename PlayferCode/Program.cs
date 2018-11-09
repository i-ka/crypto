using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace PlayferCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyword = "fire";
            var textToEncode = @"IDIOCY OFTEN LOOKS LIKE INTELLIGENCE";
            Console.Write("");
            var sw = Stopwatch.StartNew();
            var encoded = Encode(textToEncode.ToLower(), keyword.ToLower());
            Console.WriteLine(encoded);
            var decoded = Decode(encoded, keyword.ToLower());
            Console.WriteLine(decoded);
            Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms");
            Console.ReadKey();
        }

        private const string Alphabet = "abcdefghijklmnoprstuvwxyz";

        private static string Encode(string textToEncode, string keyword)
        {
            var matrix = BuildMatrix(keyword);
            var textWithoutSpaces = textToEncode.Where(c => !char.IsWhiteSpace(c) && !char.IsSeparator(c)).ToArray();
            var bigramms = new List<string>();
            for (var i = 0; i < textWithoutSpaces.Length;)
            {
                if (i + 1 >= textWithoutSpaces.Length && textWithoutSpaces.Length % 2 != i % 2)
                {
                    bigramms.Add(new string(new char[] { textWithoutSpaces[i], 'x' }));
                    break;
                }
                if (textWithoutSpaces[i] == textWithoutSpaces[i + 1])
                {
                    bigramms.Add(new string(new char[] { textWithoutSpaces[i], 'x' }));
                    i++;
                    continue;
                }
                bigramms.Add(new string(new char[] { textWithoutSpaces[i], textWithoutSpaces[i + 1] }));
                i += 2;
            }

            for (var i = 0; i < bigramms.Count; i ++)
            {
                var first = matrix.Find(bigramms[i][0]);
                var second = matrix.Find(bigramms[i][1]);
                if (first.Item1 == second.Item1)
                {
                    var firstReplacementPos = (first.Item1, (first.Item2 + 1) % (matrix.GetUpperBound(1) + 1));
                    var secondReplacementPos = (second.Item1, (second.Item2 + 1) % (matrix.GetUpperBound(1) + 1));
                    bigramms[i] = new string(new char[]
                    {
                        matrix[firstReplacementPos.Item1, firstReplacementPos.Item2],
                        matrix[secondReplacementPos.Item1, secondReplacementPos.Item2]
                    });
                    continue;
                }
                if (first.Item2 == second.Item2)
                {
                    var firstReplacementPos = ((first.Item1 + 1) % (matrix.GetUpperBound(0) + 1), first.Item2 );
                    var secondReplacementPos = ((second.Item1 + 1) % (matrix.GetUpperBound(0) + 1), second.Item2);
                    bigramms[i] = new string(new char[]
                    {
                        matrix[firstReplacementPos.Item1, firstReplacementPos.Item2],
                        matrix[secondReplacementPos.Item1, secondReplacementPos.Item2]
                    });
                    continue;
                }
                bigramms[i] = new string(new char[]
                {
                    matrix[first.Item1, second.Item2],
                    matrix[second.Item1, first.Item2]
                });
            }

            return string.Join(string.Empty, bigramms);
        }

        private static string Decode(string encodedText, string keyword)
        {
            var matrix = BuildMatrix(keyword);
            var bigramms = encodedText.Batch(2).Select(b => string.Join(string.Empty, b)).ToArray();

            for (var i = 0; i < bigramms.Length; i++)
            {
                var first = matrix.Find(bigramms[i][0]);
                var second = matrix.Find(bigramms[i][1]);
                if (first.Item1 == second.Item1)
                {
                    var firstReplacementPos = (first.Item1, (first.Item2 + matrix.GetUpperBound(1) - 1) % matrix.GetUpperBound(1));
                    var secondReplacementPos = (second.Item1, (second.Item2 + matrix.GetUpperBound(1) - 1) % matrix.GetUpperBound(1));
                    bigramms[i] = new string(new char[]
                    {
                        matrix[firstReplacementPos.Item1, firstReplacementPos.Item2],
                        matrix[secondReplacementPos.Item1, secondReplacementPos.Item2]
                    });
                    continue;
                }
                if (first.Item2 == second.Item2)
                {
                    var firstReplacementPos = ((first.Item1 + matrix.GetUpperBound(0) - 1) % matrix.GetUpperBound(0), first.Item2);
                    var secondReplacementPos = ((second.Item1 + matrix.GetUpperBound(0) - 1) % matrix.GetUpperBound(0), second.Item2);
                    bigramms[i] = new string(new char[]
                    {
                        matrix[firstReplacementPos.Item1, firstReplacementPos.Item2],
                        matrix[secondReplacementPos.Item1, secondReplacementPos.Item2]
                    });
                    continue;
                }
                bigramms[i] = new string(new char[]
                {
                    matrix[first.Item1, second.Item2],
                    matrix[second.Item1, first.Item2]
                });
            }

            return string.Join(string.Empty, bigramms.SelectMany(b => b).Where(c => c != 'x'));
        }

        private static char[,] BuildMatrix(IEnumerable<char> keyword)
        {
            var matrix = new char[5, 5];
            var cleanedKeyword = keyword.Distinct().ToArray();
            var leftLetters = Alphabet.Where(c => !keyword.Contains(c));
            for (var i = 0; i < matrix.Length; i++)
            {
                if (i < cleanedKeyword.Length)
                {
                    matrix[i / 5, i % 5] = cleanedKeyword[i];
                    continue;
                }
                matrix[i / 5, i % 5] = leftLetters.ElementAt(i - cleanedKeyword.Length);
            }
            return matrix;
        }
    }
}
