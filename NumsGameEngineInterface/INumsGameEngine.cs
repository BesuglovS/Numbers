using System.Collections.Generic;

namespace NumsGameEngineInterface
{
    public interface INumsGameEngine
    {
        List<NumsNum> AnalysePosition(List<NumsMove> moves);

        NumsNum GetMove(List<NumsMove> moves);

        NumsNum CreateOwnNum();
    }
}
