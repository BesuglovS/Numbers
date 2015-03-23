using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NumsGameEngineInterface;

namespace Numbers.Core
{
    public class NumsGame
    {
        private readonly byte _type;
        private NumsPosition _position;
        private static List<string> _dllPaths;

        public NumsGame(byte type)
        {
            _type = type;
            _position = new NumsPosition();
        }

        public void RunGame()
        {
            switch (_type)
            {
                case 1:
                    HumansGame();
                    break;
                case 2:
                    HumanVsSystem(humanPlaysFirst: true);
                    break;
                case 3:
                    HumanVsSystem(humanPlaysFirst: false);
                    break;
                case 4:
                    SystemsGame();
                    break;
                case 5:
                    EnginesTest();
                    break;
            }
        }

        private void EnginesTest()
        {
            var engines = GetNumsGameEngines();

            var engine1Info = ChooseEngine(engines, "Выберите игровой движок для игрока 1: ", true);
            var engine2Info = ChooseEngine(engines, "Выберите игровой движок для игрока 2: ", false);

            Console.Write("Введите количество туров: ");
            var roundQuantity = int.Parse(Console.ReadLine());

            var engine1 = LoadEngine(engine1Info);
            var engine2 = LoadEngine(engine2Info);

            int engine1Wins = 0,   // Побед первого движка
                engine2Wins = 0,   // Побед второго движка
                engine1Faults = 0, // Ошибок первого движка
                engine2Faults = 0, // Ошибок второго движка
                draws = 0;         // Ничьих


            var engineMove = new NumsNum(0);
            byte correctMoves;

            for (int i = 1; i < (roundQuantity * 2) + 1; i++)
            {
                _position = new NumsPosition(engine1.CreateOwnNum(), engine2.CreateOwnNum());

                do
                {
                    correctMoves = 0;

                    switch (_position.PlayerToMakeMove)
                    {
                        case 1: // 1
                            engineMove = i % 2 == 1 ?
                                     engine1.GetMove(_position.FirstPlayerMoves) :
                                     engine2.GetMove(_position.FirstPlayerMoves);

                            break;
                        case 2: // 2
                            engineMove = i % 2 == 1 ?
                                     engine2.GetMove(_position.SecondPlayerMoves) :
                                     engine1.GetMove(_position.SecondPlayerMoves);
                            break;
                    }

                    if (CheckMove(engineMove, _position))
                        MakeMove(engineMove);
                    else
                        correctMoves = (byte)((_position.PlayerToMakeMove == 1) ?
                                              ((i % 2 == 1) ? 1 : 2) :
                                              ((i % 2 == 1) ? 2 : 1));

                } while ((GameResult == 3) && (correctMoves == 0));

                if (correctMoves != 0)
                {
                    if (correctMoves == 1) // Ошибся 1-й движок
                        engine1Faults++;
                    else // Второй
                        engine2Faults++;
                }
                else
                {
                    switch (GameResult)
                    {
                        case 0:
                            draws++;
                            break;
                        case 1:
                            if (i % 2 == 1)
                            {
                                engine1Wins++;
                            }
                            else
                            {
                                engine2Wins++;
                            }
                            break;
                        case 2:
                            if (i % 2 == 1)
                            {
                                engine2Wins++;
                            }
                            else
                            {
                                engine1Wins++;
                            }
                            break;
                    }
                }

                Console.Clear();
                Console.WriteLine("Всего игр = {0}.", i);
                Console.WriteLine("Ничьих = {0}", draws);
                Console.WriteLine("Побед {0} = {1}", engine1Info.Name, engine1Wins);
                Console.WriteLine("Побед {0} = {1}", engine2Info.Name, engine2Wins);
                Console.WriteLine("Ошибок {0} = {1}", engine1Info.Name, engine1Faults);
                Console.WriteLine("Ошибок {0} = {1}", engine2Info.Name, engine2Faults);
            }

            Console.WriteLine("Для продолжения нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void SystemsGame()
        {
            var engines = GetNumsGameEngines();

            var engine1Info = ChooseEngine(engines, "Выберите игровой движок для игрока 1: ", true);
            var engine2Info = ChooseEngine(engines, "Выберите игровой движок для игрока 2: ", false);

            var engine1 = LoadEngine(engine1Info);
            var engine2 = LoadEngine(engine2Info);

            _position.FirstPlayerNumber = engine1.CreateOwnNum();
            _position.SecondPlayerNumber = engine2.CreateOwnNum();

            var engineMove = new NumsNum(0);
            bool correctMove = true;
            do
            {
                switch (_position.PlayerToMakeMove)
                {
                    case 1: // 1
                        engineMove = engine1.GetMove(_position.FirstPlayerMoves);

                        break;
                    case 2: // 2
                        engineMove = engine2.GetMove(_position.SecondPlayerMoves);
                        break;
                }

                if (CheckMove(engineMove, _position))
                {
                    MakeMove(engineMove);
                }
                else
                {
                    PrintWholePosition(true, true);

                    Console.WriteLine("Ошибка движка. Попытка сделать ход = " + engineMove.NumString);
                    correctMove = false;
                }

                PrintWholePosition(true, true);

                if (((GameResult == 3) && (correctMove)))
                {
                    Console.WriteLine("Для продолжения нажмите любую клавишу...");
                    Console.ReadKey();
                }

            } while ((GameResult == 3) && (correctMove));

            if (correctMove)
            {
                PrintWholePosition(true, true);

                if (GameResult == 0)
                {
                    Console.WriteLine("Ничья.");
                }
                else
                {
                    Console.WriteLine("Победа! Выиграл игрок " + GameResult);
                }
            }

            Console.ReadKey();
        }

        private void HumanVsSystem(bool humanPlaysFirst)
        {
            var engines = GetNumsGameEngines();

            var myEngineInfo = ChooseEngine(engines, "Выберите игровой движок: ", true);

            var myEngine = LoadEngine(myEngineInfo);

            if (humanPlaysFirst)
            {
                _position.FirstPlayerNumber = GetPlayerNumber(1);
                _position.SecondPlayerNumber = myEngine.CreateOwnNum();
            }
            else
            {
                _position.FirstPlayerNumber = myEngine.CreateOwnNum();
                _position.SecondPlayerNumber = GetPlayerNumber(2);
            }

            bool correctEngineMove;

            do
            {
                PrintWholePosition(true, false);

                correctEngineMove = true;

                if (((_position.PlayerToMakeMove == 1) && (humanPlaysFirst)) ||
                    ((_position.PlayerToMakeMove == 2) && (!humanPlaysFirst)))
                {
                    MakeMove(HumanMove());
                }
                else
                {
                    var engineMove = myEngine.GetMove((_position.PlayerToMakeMove == 1) ? _position.FirstPlayerMoves : _position.SecondPlayerMoves);
                    if (CheckMove(engineMove, _position))
                    {
                        MakeMove(engineMove);
                    }
                    else
                    {
                        PrintWholePosition(true, true);

                        Console.WriteLine("Ошибка движка. Попытка сделать ход = " + engineMove.NumString);
                        correctEngineMove = false;
                    }
                }
            } while ((GameResult == 3) && (correctEngineMove));

            if (correctEngineMove)
            {
                PrintWholePosition(true, true);

                if (GameResult == 0)
                {
                    Console.WriteLine("Ничья.");
                }
                else
                {
                    Console.WriteLine("Победа! Выиграл игрок " + GameResult);
                }
            }

            Console.ReadKey();
        }

        private bool CheckMove(NumsNum engineMove, NumsPosition position)
        {
            return engineMove.Num.IsCorrect();
        }

        private INumsGameEngine LoadEngine(EngineInfo myEngineInfo)
        {
            var assembly = Assembly.LoadFrom(myEngineInfo.DllPath);

            var engineType =
                assembly.GetTypes().FirstOrDefault
                (dllType => dllType.GetInterface("NumsGameEngineInterface.INumsGameEngine") != null);

            return (engineType != null) ? ((INumsGameEngine)Activator.CreateInstance(engineType)) : null;
        }

        private EngineInfo ChooseEngine(List<EngineInfo> engines, string prompt, bool showEnginesList)
        {
            if (showEnginesList)
            {
                for (int i = 0; i < engines.Count; i++)
                {
                    Console.WriteLine((i + 1).ToString() + ") " +
                                      engines[i].Name + " (" +
                                      engines[i].DllPath + ")");
                }
            }

            Console.Write(prompt);

            var engineIndex = int.Parse(Console.ReadLine()) - 1;

            return engines[engineIndex];

        }

        private List<EngineInfo> GetNumsGameEngines()
        {
            var engines = new List<EngineInfo>();

            string executingPath = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location);

            //var dlls = Directory.GetFiles(executingPath, "*.dll");

            for (int i = 0; i < 3; i++)
            {
                executingPath = Path.GetDirectoryName(executingPath);
            }

            _dllPaths = new List<string>();

            DirSearch(executingPath, "*.dll");

            foreach (var dllPath in _dllPaths)
            {
                var assembly = Assembly.LoadFrom(dllPath);

                var engineType = assembly.GetTypes().FirstOrDefault
                    (dllType => dllType.GetInterface("NumsGameEngineInterface.INumsGameEngine") != null);

                if (engineType != null)
                {
                    engines.Add(new EngineInfo { Name = engineType.Name, DllPath = dllPath });
                }
            }


            return engines;
        }

        private void DirSearch(string searchDir, string searchPattern)
        {
            if (_dllPaths == null)
                _dllPaths = new List<string>();

            try
            {
                foreach (var directory in Directory.GetDirectories(searchDir))
                {
                    _dllPaths.AddRange(Directory.GetFiles(directory, searchPattern));
                    DirSearch(directory, searchPattern);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void HumansGame()
        {
            _position.FirstPlayerNumber = GetPlayerNumber(1);
            _position.SecondPlayerNumber = GetPlayerNumber(2);
            do
            {
                MakeMove(HumanMove());
            } while (GameResult == 3);

            PrintWholePosition(true, true);

            if (GameResult == 0)
            {
                Console.WriteLine("Ничья.");
            }
            else
            {
                Console.WriteLine("Победа! Выиграл игрок " + GameResult);
            }

            Console.ReadKey();
        }

        private void PrintWholePosition(bool clearConsole, bool printNums)
        {
            if (clearConsole)
            {
                Console.Clear();
            }
            var firstPlayerMovesCount = _position.FirstPlayerMoves.Count;
            var secondPlayerMovesCount = _position.SecondPlayerMoves.Count;
            var maxMoves = (firstPlayerMovesCount > secondPlayerMovesCount) ? firstPlayerMovesCount : secondPlayerMovesCount;

            if (printNums)
            {
                Console.WriteLine(_position.SecondPlayerNumber.NumString + "     " +
                                  _position.FirstPlayerNumber.NumString);
                Console.WriteLine("===================");
            }

            for (int i = 0; i < maxMoves; i++)
            {
                var moveString = "";
                if (i < firstPlayerMovesCount)
                {
                    moveString += _position.FirstPlayerMoves[i].Guess.NumString + " " +
                                  _position.FirstPlayerMoves[i].Count.First + ":" +
                                  _position.FirstPlayerMoves[i].Count.Second;
                }
                else
                {
                    moveString += "         ";
                }

                moveString += " ";

                if (i < secondPlayerMovesCount)
                {
                    moveString += _position.SecondPlayerMoves[i].Guess.NumString + " " +
                                  _position.SecondPlayerMoves[i].Count.First + ":" +
                                  _position.SecondPlayerMoves[i].Count.Second;
                }
                else
                {
                    moveString += "         ";
                }
                Console.WriteLine(moveString);
            }
        }

        private NumsNum GetPlayerNumber(int playerIndex)
        {
            var result = new NumsNum(0);
            int res = 0;
            do
            {
                try
                {
                    Console.Write("Введите задуманное число (Игрок " + playerIndex + "): ");
                    var numString = Console.ReadLine();
                    if (numString != null)
                    {
                        res = int.Parse(numString);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            } while (!res.IsCorrect());

            result.Num = res;

            return result;
        }

        private void MakeMove(NumsNum move)
        {
            switch (_position.PlayerToMakeMove)
            {
                case 1:
                    _position.FirstPlayerMoves.Add(new NumsMove { Guess = move, Count = move.GetCountFrom(_position.SecondPlayerNumber) });
                    if ((_position.SecondPlayerMoves.LastOrDefault() == null) || (_position.SecondPlayerMoves.Last().Count.Second != 5))
                    {
                        _position.PlayerToMakeMove = 2;
                    }
                    break;
                case 2:
                    _position.SecondPlayerMoves.Add(new NumsMove { Guess = move, Count = move.GetCountFrom(_position.FirstPlayerNumber) });
                    if ((_position.FirstPlayerMoves.LastOrDefault() == null) || (_position.FirstPlayerMoves.Last().Count.Second != 5))
                    {
                        _position.PlayerToMakeMove = 1;
                    }
                    break;
            }
        }

        public byte GameResult
        {
            get
            {
                if ((_position.FirstPlayerMoves.LastOrDefault() == null) ||
                    (_position.SecondPlayerMoves.LastOrDefault() == null) ||
                    ((_position.FirstPlayerMoves.Last().Count.Second != 5) ||
                    (_position.SecondPlayerMoves.Last().Count.Second != 5)))
                {
                    return 3;
                }

                if (_position.FirstPlayerMoves.Count > _position.SecondPlayerMoves.Count)
                {
                    return 2;
                }

                if (_position.FirstPlayerMoves.Count < _position.SecondPlayerMoves.Count)
                {
                    return 1;
                }

                return 0;
            }
        }

        private NumsNum HumanMove()
        {
            var result = new NumsNum(0);
            int input;
            do
            {
                //PrintPosition(_position.PlayerToMakeMove, true);
                PrintWholePosition(true, false);

                Console.Write("Введите попытку: ");
                input = int.Parse(Console.ReadLine());
            } while (!input.IsCorrect());

            result.Num = input;

            return result;
        }

        private void PrintPosition(int player, bool clearConsole)
        {
            if (clearConsole)
            {
                Console.Clear();
            }

            switch (player)
            {
                case 1:
                    foreach (var move in _position.FirstPlayerMoves)
                    {
                        Console.WriteLine(move.Guess.NumString + " - " + move.Count.First + ":" + move.Count.Second);
                    }
                    break;
                case 2:
                    foreach (var move in _position.SecondPlayerMoves)
                    {
                        Console.WriteLine(move.Guess.NumString + " - " + move.Count.First + ":" + move.Count.Second);
                    }
                    break;
            }
        }
    }
}
