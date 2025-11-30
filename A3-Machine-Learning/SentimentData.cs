using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace A3_Machine_Learning
{
    public class SentimentData
    {
        [LoadColumn(1)]
        public bool Label { get; set; }

        [LoadColumn(0)]
        public string Text { get; set; } = string.Empty;
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        public float Probability { get; set; }
        public float Score { get; set; }
    }
}
