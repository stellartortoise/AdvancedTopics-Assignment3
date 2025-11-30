using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace A3_Machine_Learning
{
    public partial class Form1 : Form
    {
        private MLContext mlContext;
        private ITransformer trainedModel;  
        private ITransformer externalModel; 
        private PredictionEngine<SentimentData, SentimentPrediction> predictionEngine;
        private CalibratedBinaryClassificationMetrics trainedModelMetrics;
        private CalibratedBinaryClassificationMetrics externalModelMetrics;
        private IDataView testSet;
        private IDataView currentTrainSet;
        private List<SentimentData> additionalTrainingData;
        private string dataPath;

        public Form1()
        {
            InitializeComponent();
            InitializeMLModel();
            additionalTrainingData = new List<SentimentData>();
        }

        private void InitializeMLModel()
        {
            // Get the ML model from Program class (trained model)
            mlContext = SentimentAnalysis.Program.GetMLContext();
            trainedModel = SentimentAnalysis.Program.GetModel();
            trainedModelMetrics = SentimentAnalysis.Program.GetTrainedModelMetrics();
            testSet = SentimentAnalysis.Program.GetTestSet();
            currentTrainSet = SentimentAnalysis.Program.GetTrainSet();
            dataPath = SentimentAnalysis.Program.GetDataPath();
            
            // Load external model from Models folder
            LoadExternalModel();
            
            // Create prediction engine with trained model by default (since radioButton1 is checked)
            predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(trainedModel);
        }

        private void LoadExternalModel()
        {
            try
            {
                // Path to the external model in the Models folder
                string modelPath = Path.Combine(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\")), "Models", "SentimentModel.zip");
                
                if (File.Exists(modelPath))
                {
                    // Load the model from file
                    externalModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
                    Console.WriteLine("External model loaded successfully from: " + modelPath);
                    
                    // Evaluate external model on test set
                    EvaluateExternalModel();
                }
                else
                {
                    Console.WriteLine("External model file not found at: " + modelPath);
                    MessageBox.Show($"External model file not found at:\n{modelPath}\n\nPlease ensure the model file exists.", 
                                    "Model Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading external model: {ex.Message}");
                MessageBox.Show($"Error loading external model:\n{ex.Message}", 
                                "Model Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EvaluateExternalModel()
        {
            if (externalModel != null && testSet != null)
            {
                try
                {
                    IDataView predictions = externalModel.Transform(testSet);
                    externalModelMetrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
                    
                    Console.WriteLine("\nExternal Model Metrics:");
                    Console.WriteLine($"Accuracy: {externalModelMetrics.Accuracy:P2}");
                    Console.WriteLine($"AUC: {externalModelMetrics.AreaUnderRocCurve:P2}");
                    Console.WriteLine($"F1 Score: {externalModelMetrics.F1Score:P2}");
                    Console.WriteLine($"Precision: {externalModelMetrics.PositivePrecision:P2}");
                    Console.WriteLine($"Recall: {externalModelMetrics.PositiveRecall:P2}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error evaluating external model: {ex.Message}");
                }
            }
        }

        private void checkBoxTrainModel_CheckedChanged(object sender, EventArgs e)
        {
            // Enable/disable the training label textbox based on checkbox state
            trainingTextBox.Enabled = checkBoxTrainModel.Checked;
            
            if (!checkBoxTrainModel.Checked)
            {
                trainingTextBox.Clear();
            }
        }

        private ITransformer RetrainModel(IDataView originalTrainSet, SentimentData newData)
        {
            try
            {
                // Load all original training data from file
                IDataView baseData = mlContext.Data.LoadFromTextFile<SentimentData>(dataPath, hasHeader: false);
                
                // Create a list with the new data point plus any previously added data
                var allNewData = new List<SentimentData>(additionalTrainingData);
                allNewData.Add(newData);
                
                // Convert new data to IDataView
                IDataView newDataView = mlContext.Data.LoadFromEnumerable(allNewData);
                
                // Get the data as enumerables and combine them
                var originalDataEnumerable = mlContext.Data.CreateEnumerable<SentimentData>(baseData, reuseRowObject: false);
                var combinedData = originalDataEnumerable.Concat(allNewData).ToList();
                
                // Create IDataView from combined data
                IDataView combinedTrainSet = mlContext.Data.LoadFromEnumerable(combinedData);
                
                // Retrain the model
                var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(SentimentData.Text))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));
                
                var retrainedModel = estimator.Fit(combinedTrainSet);
                
                // Update the current training set
                currentTrainSet = combinedTrainSet;
                
                return retrainedModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retraining model: {ex.Message}");
                MessageBox.Show($"Error retraining model:\n{ex.Message}", 
                                "Retraining Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return trainedModel; // Return the original model if retraining fails
            }
        }

        private string CompareMetrics(CalibratedBinaryClassificationMetrics before, CalibratedBinaryClassificationMetrics after)
        {
            StringBuilder comparison = new StringBuilder();
            comparison.AppendLine("\n========== MODEL COMPARISON ==========");
            comparison.AppendLine("Metric          | Before    | After     | Change");
            comparison.AppendLine("----------------|-----------|-----------|----------");
            
            comparison.AppendLine($"Accuracy        | {before.Accuracy:P2}     | {after.Accuracy:P2}     | {GetChangeIndicator(before.Accuracy, after.Accuracy)}");
            comparison.AppendLine($"AUC             | {before.AreaUnderRocCurve:P2}     | {after.AreaUnderRocCurve:P2}     | {GetChangeIndicator(before.AreaUnderRocCurve, after.AreaUnderRocCurve)}");
            comparison.AppendLine($"F1 Score        | {before.F1Score:P2}     | {after.F1Score:P2}     | {GetChangeIndicator(before.F1Score, after.F1Score)}");
            comparison.AppendLine($"Precision       | {before.PositivePrecision:P2}     | {after.PositivePrecision:P2}     | {GetChangeIndicator(before.PositivePrecision, after.PositivePrecision)}");
            comparison.AppendLine($"Recall          | {before.PositiveRecall:P2}     | {after.PositiveRecall:P2}     | {GetChangeIndicator(before.PositiveRecall, after.PositiveRecall)}");
            comparison.AppendLine("======================================");
            
            // Overall assessment
            double avgImprovement = (
                (after.Accuracy - before.Accuracy) +
                (after.AreaUnderRocCurve - before.AreaUnderRocCurve) +
                (after.F1Score - before.F1Score) +
                (after.PositivePrecision - before.PositivePrecision) +
                (after.PositiveRecall - before.PositiveRecall)
            ) / 5.0;
            
            comparison.AppendLine();
            if (Math.Abs(avgImprovement) < 0.001)
            {
                comparison.AppendLine("RESULT: Model metrics are essentially UNCHANGED.");
            }
            else if (avgImprovement > 0)
            {
                comparison.AppendLine($"RESULT: Model has IMPROVED with average gain of {avgImprovement:P2}!");
            }
            else
            {
                comparison.AppendLine($"RESULT: Model has DECLINED with average loss of {Math.Abs(avgImprovement):P2}.");
            }
            
            return comparison.ToString();
        }

        private string GetChangeIndicator(double before, double after)
        {
            double diff = after - before;
            if (Math.Abs(diff) < 0.001)
                return " (no change)";
            else if (diff > 0)
                return $" + {diff:P2}";
            else
                return $" - {diff:P2}";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the text from the input control
            string _userInput = userInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(_userInput))
            {
                MessageBox.Show("Please enter some text to analyze.", "No Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if user wants to retrain the model
            bool shouldRetrain = checkBoxTrainModel.Checked;
            bool validLabel = false;
            bool userLabel = false;

            if (shouldRetrain)
            {
                // Validate training label
                string labelText = trainingTextBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(labelText))
                {
                    MessageBox.Show("Please enter a training label (0 or 1).", "Missing Label", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (labelText != "0" && labelText != "1")
                {
                    MessageBox.Show("Training label must be 0 (negative) or 1 (positive).", "Invalid Label", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                validLabel = true;
                userLabel = labelText == "1";
            }

            // Check which radio button is selected and use appropriate model
            ITransformer selectedModel;
            string modelChoice;
            CalibratedBinaryClassificationMetrics selectedMetrics;
            
            if (radioButton1.Checked)
            {
                // Use newly trained model
                selectedModel = trainedModel;
                modelChoice = "Newly Trained Model";
                selectedMetrics = trainedModelMetrics;
            }
            else
            {
                // Use external loaded model
                if (externalModel == null)
                {
                    MessageBox.Show("External model is not loaded. Please check the Models folder.", 
                                    "Model Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                selectedModel = externalModel;
                modelChoice = "External Loaded Model";
                selectedMetrics = externalModelMetrics;
            }

            // Recreate prediction engine with selected model
            predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(selectedModel);
            
            // Create sentiment data from user input
            var sentimentData = new SentimentData
            {
                Text = _userInput
            };

            // Predict sentiment
            var prediction = predictionEngine.Predict(sentimentData);

            // Display results
            string sentiment = prediction.Prediction ? "Positive" : "Negative";
            
            // Build result message with metrics
            StringBuilder resultMessage = new StringBuilder();
            resultMessage.AppendLine($"Model: {modelChoice}");
            resultMessage.AppendLine($"Text: {_userInput}");
            resultMessage.AppendLine();
            resultMessage.AppendLine($"Predicted Sentiment: {sentiment}");
            resultMessage.AppendLine($"Confidence: {prediction.Probability:P2}");
            resultMessage.AppendLine($"Score: {prediction.Score:F4}");
            
            // If user wants to retrain, do it now
            if (shouldRetrain && validLabel && radioButton1.Checked)
            {
                resultMessage.AppendLine();
                resultMessage.AppendLine("========================================");
                resultMessage.AppendLine($"User provided label: {(userLabel ? "Positive (1)" : "Negative (0)")}");
                resultMessage.AppendLine("Adding to training data and retraining...");
                
                // Store metrics before retraining
                var metricsBefore = trainedModelMetrics;
                
                // Create new training data point
                var newTrainingData = new SentimentData
                {
                    Text = _userInput,
                    Label = userLabel
                };
                
                // Add to additional training data list
                additionalTrainingData.Add(newTrainingData);
                
                // Retrain the model
                trainedModel = RetrainModel(currentTrainSet, newTrainingData);
                
                // Evaluate retrained model
                IDataView predictions = trainedModel.Transform(testSet);
                trainedModelMetrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
                
                // Update prediction engine with retrained model
                predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(trainedModel);
                selectedMetrics = trainedModelMetrics;
                
                // Compare models
                string comparisonResult = CompareMetrics(metricsBefore, trainedModelMetrics);
                resultMessage.AppendLine();
                resultMessage.Append(comparisonResult);
                
                // Save the updated training data to file
                SaveTrainingData(newTrainingData);
                
                resultMessage.AppendLine();
                resultMessage.AppendLine($"Total additional training examples: {additionalTrainingData.Count}");
            }
            
            // Add model metrics if available
            if (selectedMetrics != null)
            {
                resultMessage.AppendLine();
                resultMessage.AppendLine("--- Current Model Performance Metrics ---");
                resultMessage.AppendLine($"Accuracy: {selectedMetrics.Accuracy:P2}");
                resultMessage.AppendLine($"AUC: {selectedMetrics.AreaUnderRocCurve:P2}");
                resultMessage.AppendLine($"F1 Score: {selectedMetrics.F1Score:P2}");
                resultMessage.AppendLine($"Precision: {selectedMetrics.PositivePrecision:P2}");
                resultMessage.AppendLine($"Recall: {selectedMetrics.PositiveRecall:P2}");
            }

            outputBox.Text = resultMessage.ToString();
        }

        private void SaveTrainingData(SentimentData newData)
        {
            try
            {
                // Append new training data to the file
                string line = $"{newData.Text}\t{(newData.Label ? "1" : "0")}";
                File.AppendAllText(dataPath, Environment.NewLine + line);
                Console.WriteLine($"Training data saved: {line}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving training data: {ex.Message}");
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
