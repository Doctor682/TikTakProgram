using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikTakProgram.Dtos;

namespace TikTakProgram
{
    public static class HistoryViewer
    {
        public static void ShowGameHistory()
        {
            List<GamesHistoryDto> history = HttpRequests.GetGamesHistory();
            if (history.Count == 0)
            {
                Console.WriteLine("History is empty");
                Console.ReadKey(true);
                return;
            }

            int num = 1;
            foreach (GamesHistoryDto game in history)
            {
                Console.WriteLine($"\n=== Game #{num++} ===");

                foreach (var (symbol, name) in game.players)
                {
                    string cup = name == game.winner ? "Winner" : "loser";
                    Console.WriteLine($"{name} ( '{symbol}' ){cup}");
                }

                Console.WriteLine();
                PrintBoard(game.board);
                if (DateTime.TryParse(game.endedAt, out DateTime endedUtc))
                {
                    DateTime endedLocal = endedUtc.ToLocalTime();
                    string formattedTime = endedLocal.ToString("dd.MM.yyyy - HH:mm");
                    Console.WriteLine($"Game ended at: {formattedTime}\n");
                }
                else
                {
                    Console.WriteLine($"Game ended at: [error] (error format)\n");
                }
            }
            Console.WriteLine("Press any key to return to the menu...");
            Console.ReadKey(true);
        }

        private static void PrintBoard(Dictionary<string, string> board)
        {
            int boardDimension = (int)Math.Sqrt(board.Count);

            PrintColumnHeaders(boardDimension);

            for (int rowIndex = 0; rowIndex < boardDimension; rowIndex++)
            {
                PrintRow(board, rowIndex, boardDimension);

                if (rowIndex < boardDimension - 1)
                    PrintRowSeparator(boardDimension);
            }
        }

        private static void PrintColumnHeaders(int boardDimension)
        {
            Console.Write("   ");
            for (int columnIndex = 0; columnIndex < boardDimension; columnIndex++)
                Console.Write($" {(char)('A' + columnIndex)}  ");
            Console.WriteLine();
        }

        private static void PrintRow(Dictionary<string, string> board, int rowIndex, int boardDimension)
        {
            Console.Write($" {rowIndex + 1} ");

            for (int columnIndex = 0; columnIndex < boardDimension; columnIndex++)
            {
                string key = $"{(char)('A' + columnIndex)}{rowIndex + 1}";
                string symbol = GetSymbolForCell(board, key);
                Console.Write($" {symbol} ");

                if (columnIndex < boardDimension - 1)
                    Console.Write("|");
            }

            Console.WriteLine();
        }

        private static void PrintRowSeparator(int boardDimension)
        {
            Console.WriteLine("  " + string.Join("+", Enumerable.Repeat("---", boardDimension)));
        }

        private static string GetSymbolForCell(Dictionary<string, string> board, string key)
        {
            return board.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value)
                ? value
                : "·";
        }

    }
}
