using Microsoft.Office.Interop.Excel;
using SRATS2017AddIn.Commons;
using System;
using SRATS2017AddIn.Models;

namespace SRATSAddIn
{
    public class ExcelOperations : IOOperation
    {
        public ExcelOperations()
        {
            self = this;
        }

        public override Object[,] Read(string dataRange)
        {
            System.Text.RegularExpressions.Regex r =
                new System.Text.RegularExpressions.Regex(@"(.*)!(.*)");

            System.Text.RegularExpressions.Match m = r.Match(dataRange);
            string sheetText = m.Groups[1].ToString();
            string rangeText = m.Groups[2].ToString();

            Worksheet objSheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;
            foreach (Worksheet ws in Globals.ThisAddIn.Application.ActiveWorkbook.Sheets)
            {
                if (ws.Name == sheetText)
                {
                    objSheet = ws;
                    break;
                }
            }
//            Range range = objSheet.get_Range(rangeText, System.Reflection.Missing.Value);
            Range range = objSheet.Range[rangeText];
//            Object[,] dataCells;
            return range.get_Value(System.Reflection.Missing.Value) as Object[,];
//            return range;
        }

        private Range SetFaultData(Worksheet objSheet, Range range, double[] mtime, int[] mfault, int[] mtype)
        {
            double ctime;
            double cfaults;
            ctime = 0;
            cfaults = 0;
            int i = 0;
            for (int k = 0; k < mtime.Length; k++)
            {
                ctime += mtime[k];
                double v = mfault[k] + mtype[k];
                cfaults += v;
                if (v != 0.0)
                {
                    range.Cells[1 + i, 1] = ctime;
                    range.Cells[1 + i, 2] = cfaults;
                    i++;
                }
            }
            return objSheet.Range[range.Cells[1, 1], range.Cells[i, 2]];
        }

        private Range SetSeries(Worksheet objSheet, Range range, double[] time, double[] value)
        {
            for (int k = 0, i = 1; k < time.Length; k++, i++)
            {
                range.Cells[i, 1] = time[k];
                range.Cells[i, 2] = value[k];
            }
            return objSheet.Range[range.Cells[1, 1], range.Cells[time.Length + 1, 2]];
        }

        private void MakeResultSheet(Worksheet objSheet, string[] label, object[] value)
        {
            for (int k = 0, i = 1; k < label.Length; k++, i++)
            {
                objSheet.Cells[i, 1] = label[k];
                objSheet.Cells[i, 2] = value[k];
            }
        }

        private Chart CreateDataMVFChart(Worksheet objSheet, Range chartRange)
        {
            ChartObjects charts = objSheet.ChartObjects() as ChartObjects;

            // Adds a chart at x = 100, y = 300, 500 points wide and 300 tall.
            ChartObject chartObj = charts.Add(100, 300, 500, 300);
            Chart chart = chartObj.Chart;

//            chartRange.Columns
            // Gets the cells that define the bounds of the data to be charted.
            chart.ChartType = XlChartType.xlXYScatter;
            chart.SetSourceData(chartRange.Columns, XlRowCol.xlColumns);
            SeriesCollection seriesCollection = chart.SeriesCollection() as SeriesCollection;
            Series series = seriesCollection.Item(1);
            series.ChartType = XlChartType.xlXYScatter;
            series.Name = "Data";
            series.XValues = chartRange.Columns[1];
            series.Values = chartRange.Columns[2];

            chart.HasTitle = false;

            Axis axis = chart.Axes(
                    XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Number of Faults";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;

            axis = chart.Axes(
                    XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Time";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;

            return chart;
        }

        private Chart AddMVFChart(string name, Chart chart, Range range)
        {
            int row = range.Row;
            SeriesCollection seriesCollection = chart.SeriesCollection() as SeriesCollection;
            seriesCollection.Add(range.Columns[2], XlRowCol.xlColumns);
            Series series = seriesCollection.Item(2);
            series.ChartType = XlChartType.xlXYScatterSmoothNoMarkers;
            series.Name = name;
            series.XValues = range.Columns[1];
            series.Values = range.Columns[2];
            series.MarkerStyle = XlMarkerStyle.xlMarkerStyleNone;
            series.Smooth = false;

            return chart;
        }

        private Chart CreateIntensityChart(string name, Worksheet objSheet, Range chartRange)
        {
            ChartObjects charts = objSheet.ChartObjects() as ChartObjects; ;

            // Adds a chart at x = 100, y = 300, 500 points wide and 300 tall.
            ChartObject chartObj = charts.Add(110, 310, 500, 300);
            Chart chart = chartObj.Chart;

            // Gets the cells that define the bounds of the data to be charted.
            chart.ChartType = XlChartType.xlXYScatterSmoothNoMarkers;
            chart.SetSourceData(chartRange, XlRowCol.xlColumns);
            SeriesCollection seriesCollection = chart.SeriesCollection() as SeriesCollection;
            seriesCollection.Item(1).Name = name;

            chart.HasTitle = false;

            Axis axis = chart.Axes(
                    XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Intensity";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;

            axis = chart.Axes(
                    XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Time";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;
            axis.MinimumScale = 0;

            return chart;
        }

        private Chart CreateReliChart(string name, Worksheet objSheet, Range chartRange, double minT)
        {
            ChartObjects charts = objSheet.ChartObjects() as ChartObjects; ;

            // Adds a chart at x = 100, y = 300, 500 points wide and 300 tall.
            ChartObject chartObj = charts.Add(120, 320, 500, 300);
            Chart chart = chartObj.Chart;

            // Gets the cells that define the bounds of the data to be charted.
            chart.ChartType = XlChartType.xlXYScatterSmoothNoMarkers;
            chart.SetSourceData(chartRange, XlRowCol.xlColumns);
            SeriesCollection seriesCollection = chart.SeriesCollection() as SeriesCollection;
            seriesCollection.Item(1).Name = name;

            chart.HasTitle = false;

            Axis axis = chart.Axes(
                    XlAxisType.xlValue,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Software Reliability";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;
            axis.MaximumScale = 1;

            axis = chart.Axes(
                    XlAxisType.xlCategory,
                    XlAxisGroup.xlPrimary) as Axis;

            axis.HasTitle = true;
            axis.AxisTitle.Text = "Time";
            axis.HasMajorGridlines = true;
            axis.HasMinorGridlines = false;
            axis.MinimumScale = minT;

            return chart;
        }

        public override void MakeReport(PlotModel Model)
        {
            string sheetName;

            Workbook objBook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet objSheet;

            objBook.Worksheets.Add();
            objSheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;

            if (Model.SheetName != "")
            {
                sheetName = Model.SheetName;
            }
            else
            {
                sheetName = objSheet.Name;
            }
            try
            {
                objSheet.Name = sheetName;
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                throw new AlreadyExistSheetName();
            }

            MakeResultSheet(objSheet, Model.ResultLabels, Model.ResultValues);

            objBook.Worksheets.Add();
            objSheet = Globals.ThisAddIn.Application.ActiveSheet as Worksheet;
            objSheet.Name = sheetName + " (graph)";

//            GraphData graph = Model.SRM.graphData(Model.PlotMVFX, Model.PlotReliX);

            // data generation
            if (Model.IsIntensity == true)
            {
                Range IntRange = SetSeries(objSheet, objSheet.Range["E1"], Model.IntensityTime, Model.Intensity);
                CreateIntensityChart(Model.Name, objSheet, IntRange);
            }
            if (Model.IsReliability == true)
            {
                Range ReliRange = SetSeries(objSheet, objSheet.Range["G1"], Model.ReliTime, Model.Reli);
                CreateReliChart(Model.Name, objSheet, ReliRange, Model.Data.TotalTime);
            }

            if (Model.IsMVF == true)
            {
                Range DataRange = SetFaultData(objSheet, objSheet.Range["A1"], Model.Data.Time, Model.Data.Fault, Model.Data.Type);
                Range MVFRange = SetSeries(objSheet, objSheet.Range["C1"], Model.MVFTime, Model.MVF);
                Chart chart = CreateDataMVFChart(objSheet, DataRange);
                AddMVFChart(Model.Name, chart, MVFRange);
            }
        }
    }
}
