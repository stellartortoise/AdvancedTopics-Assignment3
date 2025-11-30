using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using A3_Machine_Learning;
using Microsoft.ML;
using Microsoft.ML.Data;
using static Microsoft.ML.DataOperationsCatalog;

namespace SentimentAnalysis
{
    public class Program
    {
        private static MLContext _mlContext;
        private static ITransformer _model;
        private static CalibratedBinaryClassificationMetrics _trainedModelMetrics;
        private static IDataView _testSet;

        static string _dataPath = Path.Combine(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\")), "Data", "yelp_labelled.txt");

        [STAThread]
        static void Main()
        {
            _mlContext = new MLContext();
            TrainTestData splitdataView = LoadData(_mlContext);
            _model = BuildAndTrainModel(_mlContext, splitdataView.TrainSet);
            _testSet = splitdataView.TestSet;
            _trainedModelMetrics = Evaluate(_mlContext, splitdataView.TestSet, _model);
            UseModelWithSingleItem(_mlContext, _model);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static MLContext GetMLContext()
        {
            return _mlContext;
        }

        public static ITransformer GetModel()
        {
            return _model;
        }

        public static CalibratedBinaryClassificationMetrics GetTrainedModelMetrics()
        {
            return _trainedModelMetrics;
        }

        public static IDataView GetTestSet()
        {
            return _testSet;
        }

        static TrainTestData LoadData(MLContext mlContext)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, hasHeader: false);
            TrainTestData splitDataView = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            return splitDataView;
        }

        private static ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainSet)
        {
            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
            var model = estimator.Fit(trainSet);
            return model;

        }

        static CalibratedBinaryClassificationMetrics Evaluate(MLContext mlContext, IDataView testSet, ITransformer model)
        {
            IDataView predictions = model.Transform(testSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
            Console.WriteLine($"Precision: {metrics.PositivePrecision:P2}");
            Console.WriteLine($"Recall: {metrics.PositiveRecall:P2}");
            return metrics;
        }

        static void UseModelWithSingleItem(MLContext mlContext, ITransformer model)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var sampleStatement = new SentimentData
            {
                Text = "This was a very bad steak"
            };
            var resultPrediction = predictionFunction.Predict(sampleStatement);
            Console.WriteLine($"Text: {sampleStatement.Text} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")} sentiment | Probability: {resultPrediction.Probability} ");
        }
    }
}
