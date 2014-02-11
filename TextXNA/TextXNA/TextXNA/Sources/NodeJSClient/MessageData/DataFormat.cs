using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TestXNA.Sources.NodeJSClient.MessageData
{
    public class PlayerInfoRoot
    {
        public string name { get; set; }
        public List<PlayerInfo> args { get; set; }
    };

    public class PlayerInfo
    {
        public int gameID { get; set; }
        public string pseudo { get; set; }
        public int score { get; set; }
        public List<int> territories { get; set; }
    };

    public class CaptureInfoRoot
    {
        public string name { get; set; }
        public List<CaptureInfo> args { get; set; }
        //public List<List<int>> args { get; set; }
    };

    public class CaptureInfo
    {
        public List<int> orderedPlayers{ get; set; }
    };

    /*
    public class QuestionData
    {
        public string type { get; set; }
        public string title { get; set; }
        public object answer { get; set; }
        public int id { get; set; }
    };

    public class QuestionDataRoot
    {
        public string name { get; set; }
        public List<QuestionData> args { get; set; }
    };
     */

    public class QuestionDataRoot
    {
        public string name { get; set; }
        public List<string> args { get; set; }
    }


    public class OrderedAnswer
    {
        public int id { get; set; }
        public object value { get; set; }
        public double time { get; set; }
    };

    public class AnswerList
    {
        public List<OrderedAnswer> orderedAnswers { get; set; }
    };

    public class AnswersRoot
    {
        public string name { get; set; }
        public List<AnswerList> args { get; set; }
    };

    public class MoveData
    {
        public int id { get; set; }
    }

    public class MoveRoot
    {
        public string name { get; set; }
        public List<MoveData> args { get; set; }
    }

    public class BattleResult
    {
        public int winner { get; set; }
        public int loser { get; set; }
        public object winVal { get; set; }
        public object lossVal { get; set; }
    }

    public class BattleResultRoot
    {
        public string name { get; set; }
        public List<BattleResult> args { get; set; }
    }
    
    public class ResultRoot
    {
        public string name { get; set; }
        public List<List<int>> args { get; set; }
    }
}
