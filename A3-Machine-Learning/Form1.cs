using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.ML;

namespace A3_Machine_Learning
{
    public partial class Form1 : Form
    {
        private MLContext mlContext;
        private ITransformer model;
        private PredictionEngine<SentimentData, SentimentPrediction> predictionEngine;

        public Form1()
        {
            InitializeComponent();
            InitializeMLModel();
        }

        private void InitializeMLModel()
        {
            // Get the ML model from Program class
            mlContext = SentimentAnalysis.Program.GetMLContext();
            model = SentimentAnalysis.Program.GetModel();
            
            // Create prediction engine
            predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
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
            // Get the text from the input control (assuming textBox1 or richTextBox1 contains user input)
            string _userInput = userInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(_userInput))
            {
                MessageBox.Show("Please enter some text to analyze.", "No Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create sentiment data from user input
            var sentimentData = new SentimentData
            {
                Text = _userInput
            };

            // Predict sentiment
            var prediction = predictionEngine.Predict(sentimentData);

            // Display results
            string sentiment = prediction.Prediction ? "Positive" : "Negative";
            string resultMessage = $"Sentiment: {sentiment}\n" +
                                   $"Confidence: {prediction.Probability:P2}\n" +
                                   $"Score: {prediction.Score:F4}";

            //MessageBox.Show(resultMessage, "Sentiment Analysis Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            outputBox.Text = resultMessage;


            // Optionally display in a label or another control on the form
            // label1.Text = resultMessage;
        }
    }
}
