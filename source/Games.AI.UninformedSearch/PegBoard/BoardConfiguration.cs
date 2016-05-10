using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Games.AI.UninformedSearch.PegBoard
{
    /// <summary>
    /// Class which represents the internals needed to create a Peg Board. It can be easily
    /// serialized and deserialized and passed to the Peg Board constructor. 
    /// </summary>
    public class BoardConfiguration
    {
        public const string ResourceName = "Games.AI.UninformedSearch.PegBoard.BoardConfiguration.json";
        public const string SeparatorToken = " -> ";
        public const string HasPegToken = "1";
        public const string EmptyPegToken = "0";

        public Dictionary<string, string> Vertices { get; set; }
        public List<string> Edges { get; set; }

        public IEnumerable<Vertex> CreateVertexListFromVertices()
        {
            if (Vertices == null)
                throw new InvalidOperationException("Vertices property cannot be null.");
            // create vertices
            var vertices = (from item in Vertices
                            let index = Convert.ToInt32(item.Key)
                            let hasPeg = item.Value == HasPegToken
                            select new Vertex(index, hasPeg));
            return vertices;
        }

        public AdjacencyMatrix CreateAdjacencyMatrixFromEdges()
        {
            if (Edges == null)
                throw new InvalidOperationException("Edges property cannot be null.");

            // create adjacency matrix
            var edges = Edges
                .Select(line => line.Split(new[] { SeparatorToken }, StringSplitOptions.None))
                .Select(parsedLine => new Tuple<int, int, int>(
                    Convert.ToInt32(parsedLine[0]),
                    Convert.ToInt32(parsedLine[1]),
                    Convert.ToInt32(parsedLine[2]))
                )
                .ToList();
            var size = Math.Max(edges.Max(edge => edge.Item1), edges.Max(edge => edge.Item3)) + 1;
            var matrix = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = -1;
                }
            }

            foreach (var edge in edges)
            {
                matrix[edge.Item1, edge.Item3] = edge.Item2;
                matrix[edge.Item3, edge.Item1] = edge.Item2;
            }

            return new AdjacencyMatrix(matrix);
        }

        /// <summary>
        /// Gets the default board configuration json.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultJson()
        {            
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName))
            // ReSharper disable once AssignNullToNotNullAttribute
            using (var reader = new StreamReader(stream)) 
            {
                var json = reader.ReadToEnd();
                return json;
            }            
        }

        /// <summary>
        /// Gets the default 15-peg board configuration.
        /// </summary>
        /// <returns></returns>
        public static BoardConfiguration GetDefault()
        {
            var configuration = JsonConvert.DeserializeObject<BoardConfiguration>(GetDefaultJson());
            return configuration;
        }
    }
}
