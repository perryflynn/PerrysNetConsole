using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PerrysNetConsole
{
    public class SimpleGraph
    {
        public char LineHorizontalJointTop = '┬';
        public char LineHorizontalJointBottom = '┴';
        public char LineVerticalJointLeft = '├';
        public char LineVerticalJointRight = '┤';
        public char LineTJoint = '┼';
        public char LineHorizontal = '─';
        public char LineVertical = '│';
        public char BlockFull = '█';
        public char BlockAvg = '▄';
        public char BlockOverAvg = '▄'; // '▆';
        public char BlockUnderAvg = '▄'; // '▂';

        public int Height { get; set; } = 10;

        public void Draw(List<double> points)
        {
            int i = 1;
            this.Draw(points.ToDictionary(key => $"{i++} {key}", value => value));
        }

        public void Draw(Dictionary<string,double> points)
        {
            var pointKeys = points.Keys.ToArray();
            var pointValues = points.Values.ToArray();
            var maxpointlength = pointValues.Max().ToString().Length + 1;
            var maxpoints = maxpointlength - 3 - 1;

            if (pointValues.Min() < 0)
            {
                throw new ArgumentException("Negative values are not supported right now");
            }

            if (pointValues.Length * 2 > CoEx.Width - maxpoints)
            {
                throw new ArgumentException($"Maximum supported number of points is {(CoEx.Width - maxpoints) / 2}");
            }

            // prepare matrix
            this.CreateMatrix(this.Height, pointValues.Length, out bool[][] pointMatrix, out List<List<double>> rowLabel);

            // fill matrix
            var max = pointValues.Max();
            int matrixColumn = 0;
            foreach (var point in pointValues)
            {
                var rowsToFill = (int)Math.Round(this.Height / max * point);
                if (rowsToFill > 0)
                {
                    rowLabel[this.Height - rowsToFill].Add(point);
                }

                for (int matrixRow = this.Height - 1; matrixRow >= this.Height - rowsToFill; matrixRow--)
                {
                    pointMatrix[matrixRow][matrixColumn] = true;
                }

                matrixColumn++;
            }

            this.FixLabelGaps(ref rowLabel);

            // draw matrix
            CoEx.WriteLine($"{"".PadLeft(maxpointlength, ' ')} {this.LineHorizontalJointTop}");

            int graphRow = 0;
            bool[] columnStatus = new bool[pointValues.Length];
            foreach (var row in pointMatrix)
            {
                var label = "";
                if (rowLabel[graphRow].Count > 0)
                {
                    label = Math.Round(rowLabel[graphRow].Average()).ToString();
                }

                CoEx.Write($"{label.PadLeft(maxpointlength, ' ')} {this.LineVertical} ");

                var pointIndex = 0;
                foreach (var point in row.Select((v, i) => this.BlockParser(v, i, ref columnStatus, rowLabel[graphRow].ToArray(), pointValues[i])))
                {
                    CoEx.Write(point, pointIndex % 2 == 0 ? CoEx.ColorTitlePrimary.Invert() : CoEx.ColorTitleSecondary.Invert());
                    CoEx.Write(" ");
                    pointIndex++;
                }

                CoEx.WriteLine();

                graphRow++;
            }

            // draw x axis
            CoEx.Write($"{this.LineVerticalJointLeft}{"".PadLeft(maxpointlength, this.LineHorizontal)}{this.LineTJoint}{this.LineHorizontal}");
            for (int xi = 0; xi < pointValues.Length; xi++)
            {
                CoEx.Write($"{this.LineTJoint}{this.LineHorizontal}");
            }
            
            CoEx.WriteLine($"{this.LineVerticalJointRight}");
            
            this.CreateMatrix(pointKeys.Max(v => v.Length), pointKeys.Length, out string[][] columnLabelMatrix);

            // fill x axis
            for (int rowIndex = 0; rowIndex < columnLabelMatrix.Length; rowIndex++)
            {
                for (int colIndex = 0; colIndex < columnLabelMatrix[rowIndex].Length; colIndex++)
                {
                    columnLabelMatrix[rowIndex][colIndex] = pointKeys[colIndex].ToCharArray().Select(v => v.ToString()).ElementAtOrDefault(rowIndex) ?? " ";
                }
            }

            // draw x axis
            var labelRowIndex = 0;
            foreach (var labelRow in columnLabelMatrix)
            {
                var joint = labelRowIndex < 1 ? this.LineHorizontalJointBottom : ' ';
                CoEx.Write($"{"".PadLeft(maxpointlength, ' ')} {joint} ");

                var cellIndex = 0;
                foreach (var labelCell in labelRow)
                {
                    CoEx.Write(labelCell, cellIndex % 2 == 0 ? CoEx.ColorTitlePrimary.Invert() : CoEx.ColorTitleSecondary.Invert());
                    CoEx.Write(" ");
                    cellIndex++;
                }

                CoEx.WriteLine();
                labelRowIndex++;
            }
        }

        private void CreateMatrix(int rows, int points, out bool[][] matrix, out List<List<double>> labels)
        {
            matrix = new bool[rows][];
            labels = new List<List<double>>();

            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new bool[points];
                labels.Add(new List<double>());
            }
        }

        private void CreateMatrix(int rows, int points, out string[][] matrix)
        {
            matrix = new string[rows][];
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new string[points];
            }
        }

        private void FixLabelGaps(ref List<List<double>> labels)
        {
            double? lastNumber = null;
            List<int> gaps = new List<int>();

            for (int i = 0; i < labels.Count; i++)
            {
                // found number after a gap
                if (lastNumber != null && labels[i].Count > 0 && gaps.Count > 0)
                {
                    var currentNumber = labels[i].Average();
                    var diff = currentNumber - lastNumber;
                    var avg = diff / (gaps.Count + 1);

                    var temp = lastNumber;
                    foreach (var gap in gaps)
                    {
                        temp += avg;
                        labels[gap].Add(temp.Value);
                    }

                    lastNumber = null;
                    gaps.Clear();
                }

                // waiting for the next gap
                if (labels[i].Count > 0 && gaps.Count < 1)
                {
                    lastNumber = labels[i].Average();
                }
                // inside a gap, track all fields
                else if (lastNumber != null && labels[i].Count < 1)
                {
                    gaps.Add(i);
                }
                // unexpected state
                else
                {
                    throw new Exception("Unhandled state inside of gap loop");
                }
            }

            if (lastNumber != null && gaps.Count > 0)
            {
                var avg = lastNumber / (gaps.Count + 1);
                var temp = lastNumber;
                foreach (var gap in gaps)
                {
                    temp -= avg;
                    labels[gap].Add(temp.Value);
                }
            }
            else if(gaps.Count > 0)
            {
                throw new Exception("Unhandled state for gap at the end");
            }
        }

        private string BlockParser(bool cell, int index, ref bool[] columnStatus, double[] values, double currentValue)
        {
            if (cell)
            {
                var block = this.BlockFull;

                if (!columnStatus[index])
                {
                    if (currentValue > values.Average())
                    {
                        block = this.BlockOverAvg;
                    }
                    else if (currentValue < values.Average())
                    {
                        block = this.BlockUnderAvg;
                    }
                    else
                    {
                        block = this.BlockAvg;
                    }
                }

                columnStatus[index] = true;
                return "" + block;
            }

            return " ";
        }

    }
}
