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

        public Form1()
        {
            InitializeComponent();
            InitializeMLModel();
        }

        private void InitializeMLModel()
        {
            // Get the ML model from Program class (trained model)
            mlContext = SentimentAnalysis.Program.GetMLContext();
            trainedModel = SentimentAnalysis.Program.GetModel();
            trainedModelMetrics = SentimentAnalysis.Program.GetTrainedModelMetrics();
            testSet = SentimentAnalysis.Program.GetTestSet();
            
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
            resultMessage.AppendLine($"Sentiment: {sentiment}");
            resultMessage.AppendLine($"Confidence: {prediction.Probability:P2}");
            resultMessage.AppendLine($"Score: {prediction.Score:F4}");
            
            // Add model metrics if available
            if (selectedMetrics != null)
            {
                resultMessage.AppendLine();
                resultMessage.AppendLine("--- Model Performance Metrics ---");
                resultMessage.AppendLine($"Accuracy: {selectedMetrics.Accuracy:P2}");
                resultMessage.AppendLine($"AUC: {selectedMetrics.AreaUnderRocCurve:P2}");
                resultMessage.AppendLine($"F1 Score: {selectedMetrics.F1Score:P2}");
                resultMessage.AppendLine($"Precision: {selectedMetrics.PositivePrecision:P2}");
                resultMessage.AppendLine($"Recall: {selectedMetrics.PositiveRecall:P2}");
            }

            outputBox.Text = resultMessage.ToString();
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBoxTrainModel_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTrainModel.Checked)
            {
                trainingTextBox.Enabled = true;
            }
            else
            {
                trainingTextBox.Enabled = false;
            }
        }
    }
}
