﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;

namespace UniJoy
{
    /// <summary>
    /// This class attempt to show the psychometric responses according to the user responses until the current round.
    /// In the case of staircases it should show as many graphs as the staircases.
    /// </summary>
    class OnlinePsychGraphMaker
    {
        /// <summary>
        /// The varying parameter names represents series in the chart.
        /// </summary>
        public List<string> VaryingParametrsNames { get; set; }

        /// <summary>
        /// The heading direction regions represented in the chart.
        /// </summary>
        public Region HeadingDireactionRegion { get; set; }

        /// <summary>
        /// The Pasycho online chart control item.
        /// </summary>
        public Chart ChartControl { get; set; }

        /// <summary>
        /// Delegate for invoking the Clearing function for the psycho online graph.
        /// </summary>
        public Delegate ClearDelegate { get; set; }

        /// <summary>
        /// Delegate for invoking the Setting serieses names functio of the psycho online graph.
        /// </summary>
        public Delegate SetSeriesDelegate { get; set; }

        /// <summary>
        /// Delegate for invoking the Setting point in a given series of the psycho online graph.
        /// </summary>
        public Delegate SetPointDelegate { get; set; }

        /// <summary>
        /// The serieses points details (for each series the details of points).
        /// </summary>
        private Dictionary<string, List<SeriesDetail>> _seriesPointsDetails;

        /// <summary>
        /// Constructor.
        /// </summary>
        public OnlinePsychGraphMaker()
        {
            _seriesPointsDetails = new Dictionary<string, List<SeriesDetail>>();
        }

        /// <summary>
        /// Initialize the serieses with their names and the region of the HeadingDirection.
        /// </summary>
        public void InitSerieses()
        {
            //adding the serieses names to the chart.
            ChartControl.BeginInvoke(SetSeriesDelegate, VaryingParametrsNames);

            //clear the old dictionory from the last experiment.
            _seriesPointsDetails.Clear();

            //creating the varying parameter dictionary that holds all points for each.
            foreach (string varyingParameterName in VaryingParametrsNames)
            {
                _seriesPointsDetails[varyingParameterName] = new List<SeriesDetail>();
            }

            //adding all the heading direction to each series.
            for (double i = HeadingDireactionRegion.LowBound; i <= HeadingDireactionRegion.HighBound; i += HeadingDireactionRegion.Increament)
            {
                foreach (string varyingParameterName in VaryingParametrsNames)
                {
                    ChartControl.BeginInvoke(SetPointDelegate, varyingParameterName, i, 0, true, false);
                    _seriesPointsDetails[varyingParameterName].Add(
                        new SeriesDetail
                        {
                            X = i,
                            SuccessNum = 0,
                            Total = 0,
                            TotalRight = 0
                        }
                    );
                }
            }
        }

        /// <summary>
        /// Setting a point to the chart with the given series name and the accumulating result.
        /// </summary>
        /// <param name="varyingParameterName">The series name to add the point to it.</param>
        /// <param name="stimulusType">The stimulus type og the trial decision.</param>
        /// <param name="regionPoint">The x value (heading direction) of the point to be set.</param>
        /// <param name="answerStatus">Indicates if the current result was correct or false.</param>
        public void AddResult(string varyingParameterName, int stimulusType, double regionPoint, AnswerStatus answerStatus)
        {
            if (!(_seriesPointsDetails.ContainsKey(stimulusType.ToString())))
            {
                _seriesPointsDetails.Add(stimulusType.ToString(), new List<SeriesDetail>());
            }

            bool newPoint = _seriesPointsDetails[stimulusType.ToString()].Count(series => series.X == regionPoint) == 0;
            //if added artificially by the user after make trial button pressed.
            if (newPoint)
            {
                _seriesPointsDetails[stimulusType.ToString()].Add(
                    new SeriesDetail
                    {
                        X = regionPoint,
                        SuccessNum = 0,
                        Total = 0,
                        TotalRight = 0
                    }
                    );
            }

            //increase the total number of answers at this point.
            _seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).Total++;

            //increase the total correct answers to the given point if correct answer.
            if (answerStatus.Equals(AnswerStatus.CORRECT))
                _seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).SuccessNum++;

            if ((answerStatus.Equals(AnswerStatus.CORRECT) && regionPoint > 0) || (answerStatus.Equals(AnswerStatus.WRONG) && regionPoint < 0))
                _seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).TotalRight++;


            //invoking the function updates the given series with the updated point.
            ChartControl.BeginInvoke(SetPointDelegate,
                stimulusType.ToString(),
                regionPoint,
                ((double)_seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).TotalRight / (double)_seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).Total), newPoint, _seriesPointsDetails[stimulusType.ToString()].First(series => series.X == regionPoint).Total > 0);
        }

        /// <summary>
        /// Clearing all data in the psycho online graph.
        /// </summary>
        public void Clear()
        {
            ChartControl.BeginInvoke(ClearDelegate);
        }
    }

    /// <summary>
    /// Representing a region details.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// The low bound of the region.
        /// </summary>
        public double LowBound { get; set; }

        /// <summary>
        /// The increment for the steps in the bound.
        /// </summary>
        public double Increament { get; set; }

        /// <summary>
        /// The high bound of the region.
        /// </summary>
        public double HighBound { get; set; }
    }

    /// <summary>
    /// Series Details represent the trade of success vs answers in a point(heading direction).
    /// </summary>
    public class SeriesDetail
    {
        /// <summary>
        /// The value of the point (heading direction).
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The number of correct answers.
        /// </summary>
        public int SuccessNum { get; set; }

        /// <summary>
        /// The total answers.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The total right answers.
        /// </summary>
        public int TotalRight { get; set; }
    }

    /// <summary>
    /// Enum represents the answer status.
    /// </summary>
    public enum AnswerStatus
    {
        /// <summary>
        /// Represents wrong answer.
        /// </summary>
	    WRONG = 0,

        /// <summary>
        /// Represents correct answer.
        /// </summary>
        CORRECT = 1
    };
}
