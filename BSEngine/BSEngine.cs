using System;
using System.Collections.Generic;
using NumsGameEngineInterface;

namespace BSEngine
{
    public class DefaultEngine : INumsGameEngine
    {
        public List<NumsNum> AnalysePosition(List<NumsMove> moves)
        {
            var result = new List<NumsNum>();

            for (int i = 1234; i < 98766; i++)
            {
                if (i.IsCorrect())
                {
                    var possible = true;

                    var iNum = new NumsNum(i);

                    for (int j = 0; j < moves.Count; j++)
                    {
                        var count = iNum.GetCountFrom(new NumsNum(moves[j].Guess.Num));
                        if ((count.First != moves[j].Count.First) || (count.Second != moves[j].Count.Second))
                        {
                            possible = false;
                            break;
                        }
                    }

                    if (possible)
                    {
                        result.Add(new NumsNum(i));
                    }
                }
            }

            return result;
        }

        public NumsNum GetMove(List<NumsMove> moves)
        {
            var possibleMoves = AnalysePosition(moves);

            var r = new Random();
            var index = r.Next(possibleMoves.Count);

            return possibleMoves[index];
        }

        public NumsNum CreateOwnNum()
        {
            var numsList = new List<NumsNum>();

            for (int i = 1234; i < 98766; i++)
            {
                if (i.IsCorrect())
                {
                    numsList.Add(new NumsNum(i));
                }
            }

            var r = new Random();
            var index = r.Next(numsList.Count);

            return numsList[index];
        }
    }
}
