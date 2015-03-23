using System;
using System.Collections.Generic;
using NumsGameEngineInterface;

namespace DeathEngine
{
    public class DeathEngine : INumsGameEngine
    {
        public List<NumsNum> AnalysePosition(List<NumsMove> moves)
        {
            var result = new List<NumsNum>();

            for (int i = 1234; i < 98766; i++)
            {
                if (i.IsCorrect())
                {
                    result.Add(new NumsNum(i));
                }
            }

            return result;
        }

        public NumsNum GetMove(List<NumsMove> moves)
        {
            var correctMoves = AnalysePosition(moves);

            var r = new Random();
            var index = r.Next(correctMoves.Count);

            return correctMoves[index];
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
