using System.Collections.Generic;

namespace NumsGameEngineInterface
{
    public static class ToolBox
    {
        public static bool IsCorrect(this int num)
        {
            if ((num < 01234) || (num > 98765))
            {
                return false;
            }

            var digits = NumsNum.NumToDigits(num);
            
            return (digits[0] != digits[1]) &&
                (digits[0] != digits[2]) &&
                (digits[0] != digits[3]) &&
                (digits[0] != digits[4]) &&
                (digits[1] != digits[2]) &&
                (digits[1] != digits[3]) &&
                (digits[1] != digits[4]) &&
                (digits[2] != digits[3]) &&
                (digits[2] != digits[4]) &&
                (digits[3] != digits[4]);
        }

        public static NumsCount CalculateCount(NumsNum move, NumsNum playerNumber)
        {
            var result = new NumsCount();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (move.Digits[i] == playerNumber.Digits[j])
                    {
                        result.First++;
                        if (i == j)
                        {
                            result.Second++;
                        }
                    }
                }
            }

            return result;
        }

        public static NumsCount GetCountFrom(this NumsNum move, NumsNum playerNumber)
        {
            var result = new NumsCount();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (move.Digits[i] == playerNumber.Digits[j])
                    {
                        result.First++;
                        if (i == j)
                        {
                            result.Second++;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class NumsPosition
    {
        public NumsNum FirstPlayerNumber;
        public List<NumsMove> FirstPlayerMoves;
        public NumsNum SecondPlayerNumber;
        public List<NumsMove> SecondPlayerMoves;

        public byte PlayerToMakeMove { get; set; }
    

        public NumsPosition()
        {
            FirstPlayerNumber = new NumsNum(0);
            FirstPlayerMoves = new List<NumsMove>();
            SecondPlayerNumber = new NumsNum(0);
            SecondPlayerMoves = new List<NumsMove>();
            PlayerToMakeMove = 1;
        }

        public NumsPosition(NumsNum firstPlayerNum, NumsNum secondPlayerNum)
        {
            FirstPlayerNumber = firstPlayerNum;
            FirstPlayerMoves = new List<NumsMove>();
            SecondPlayerNumber = secondPlayerNum;
            SecondPlayerMoves = new List<NumsMove>();
            PlayerToMakeMove = 1;
        }
    }

    public class NumsNum
    {
        public byte[] Digits;

        public int Num
        {
            get { return Digits[0]*10000 + Digits[1]*1000 + Digits[2]*100 + Digits[3]*10 + Digits[4]; }
            set { 
                Digits = NumToDigits(value); 
            }
        }

        public string NumString
        {
            get { return "" + Digits[0] + Digits[1] + Digits[2] + Digits[3] + Digits[4]; }
        }

        public NumsNum(int num)
        {
            Digits = NumToDigits(num);
        }

        public static byte[] NumToDigits(int num)
        {
            var result = new byte[5];
            result[0] = (byte)(num / 10000);
            result[1] = (byte)(num / 1000 - result[0] * 10);
            result[2] = (byte)(num / 100 - result[0] * 100 - result[1] * 10);
            result[3] = (byte)(num / 10 - result[0] * 1000 - result[1] * 100 - result[2] * 10);
            result[4] = (byte)(num % 10);

            return result;
        }
    }

    public class NumsMove
    {
        public NumsNum Guess { get; set; }
        public NumsCount Count { get; set; }
    }

    public class NumsCount
    {
        public byte First { get; set; }
        public byte Second { get; set; }
    }
}
