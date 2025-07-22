using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TikTakProgram.Dtos;

namespace TikTakProgram
{
    public class TikTakBoardEngine
    {
        private readonly TikTakBoard Board;
        private readonly string? MySymbol;
        private readonly string? PlayerName;
        private string? OpponentName;
        private readonly string SessionId;
        private readonly GameStateValidator Validator;
        private bool gameOver, lobbyCreatorLeave = false;
        private int cursorRow = 0, cursorCol = 0;

        public TikTakBoardEngine(BoardDimensions dims, string? symbol, string? playerName, string sessionId)
        {
            Board = new TikTakBoard(dims);
            MySymbol = symbol;
            PlayerName = playerName;
            SessionId = sessionId;
            Validator = new GameStateValidator(MySymbol, SessionId);
        }

        public void Run()
        {
            Console.CursorVisible = false;
            BoardDimensions dims = Board.Dimensions;

            DateTime lastPoll = DateTime.UtcNow;
            DrawBoard();

            while (!gameOver && !lobbyCreatorLeave)
            {
                if ((DateTime.UtcNow - lastPoll).TotalSeconds >= 2)
                {
                    PollServer();
                    lastPoll = DateTime.UtcNow;
                    DrawBoard();
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(true).Key;
                    HandleKey(key);
                    DrawBoard();
                }
            }
            Console.WriteLine("\nPress any key to return to the lobby...");
            Console.ReadKey(true);
        }

        private void HandleKey(ConsoleKey key)
        {
            BoardDimensions dims = Board.Dimensions;

            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    cursorCol = (cursorCol + dims.ColumnSize - 1) % dims.ColumnSize;
                    break;

                case ConsoleKey.RightArrow:
                    cursorCol = (cursorCol + 1) % dims.ColumnSize;
                    break;

                case ConsoleKey.UpArrow:
                    cursorRow = (cursorRow + dims.RowSize - 1) % dims.RowSize;
                    break;

                case ConsoleKey.DownArrow:
                    cursorRow = (cursorRow + 1) % dims.RowSize;
                    break;

                case ConsoleKey.Spacebar:
                    TryMakeMove();
                    break;

                case ConsoleKey.Escape:
                    HttpRequests.LeaveLobby(SessionId, PlayerName);
                    lobbyCreatorLeave = true;
                    break;
            }
        }

        private void DrawBoard()
        {
            Console.SetCursorPosition(0, 0);
            DrawHeader();

            for (int row = 0; row < Board.Dimensions.RowSize; row++)
            {
                DrawRow(row);
                if (row < Board.Dimensions.RowSize - 1)
                    DrawSeparator();
            }

            DrawFooter();
        }

        private void DrawHeader()
        {
            Console.Write("   ");
            for (int columnIndex = 0; columnIndex < Board.Dimensions.ColumnSize; columnIndex++)
                Console.Write($" {(char)('A' + columnIndex)}  ");
            Console.WriteLine();
        }

        private void DrawRow(int rowIndex)
        {
            Console.Write($" {rowIndex + 1} ");
            for (int col = 0; col < Board.Dimensions.ColumnSize; col++)
            {
                bool isSelected = rowIndex == cursorRow && col == cursorCol;
                if (isSelected) Console.BackgroundColor = ConsoleColor.DarkCyan;

                char sym = Board[rowIndex, col] == '\0' ? ' ' : Board[rowIndex, col];
                Console.Write($" {sym} ");
                Console.ResetColor();

                if (col < Board.Dimensions.ColumnSize - 1) Console.Write("|");
            }
            Console.WriteLine();
        }

        private void DrawSeparator()
        {
            Console.WriteLine("   " + string.Join("+", Enumerable.Repeat("---", Board.Dimensions.ColumnSize)));
        }

        private void DrawFooter()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"\nYou ({PlayerName}) are playing as: {MySymbol}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Your opponent: {OpponentName}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Arrows — move, Space — make a move, Esc — exit");
            Console.ResetColor();
        }

        private void PollServer()
        {
            var state = HttpRequests.GetGameState(SessionId);
            if (state == null) return;

            if (state.players.Count < 2)
            {
                Console.Title = "Waiting for opponent...";
                return;
            }

            EnsureOpponentName(state.players);

            ApplyBoard(state.board);

            string currentTurnPlayerName = "unknown";
            if (!string.IsNullOrEmpty(state.currentTurn) && state.players.TryGetValue(state.currentTurn, out string? name))
            {
                currentTurnPlayerName = name ?? "noname";
            }

            Console.Title = $"Current turn: {state.currentTurn ?? "???"} ({currentTurnPlayerName})";

            if (state.isGameOver)
            {
                Console.WriteLine(state.message);
                Console.Title = "Let`s find new game!";
                gameOver = true;
            }
        }


        private void EnsureOpponentName(Dictionary<string, string> players)
        {
            if (OpponentName != null) return;

            OpponentName = players
                .Where(p => !string.Equals(p.Key, MySymbol, StringComparison.OrdinalIgnoreCase))
                .Select(p => p.Value)
                .FirstOrDefault();
        }

        private void ApplyBoard(Dictionary<string, string?>? boardData)
        {
            if (boardData is null) return;

            foreach (var (cell, symStr) in boardData)
            {
                if (string.IsNullOrEmpty(cell) || cell.Length < 2) continue;

                int col = cell[0] - 'A';
                if (!int.TryParse(cell[1..], out int rowFromCell)) continue;
                int row = rowFromCell - 1;

                BoardDimensions dims = Board.Dimensions;
                if (row < 0 || row >= dims.RowSize || col < 0 || col >= dims.ColumnSize) continue;

                Board[row, col] = !string.IsNullOrEmpty(symStr) ? symStr[0] : '\0';
            }
        }

        private void TryMakeMove()
        {
            if (!Validator.CanMove(out var reason))
            {
                Console.WriteLine($"Move unavailable: {reason}");
                return;
            }

            if (Board[cursorRow, cursorCol] != '\0')
            {
                Console.WriteLine("This cell is already taken.");
                return;
            }

            var cellAddr = new CellAddress(((char)('A' + cursorCol)).ToString(), cursorRow + 1);
            var result = HttpRequests.SendMove(MySymbol!, cellAddr, SessionId);

            if (result != null)
                ApplyBoard(result.board);
        }
    }
}
