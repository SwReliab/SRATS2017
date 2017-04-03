using System;
using SRATS;
using System.Collections.ObjectModel;
using SRATS2017AddIn.Commons;

namespace SRATS2017AddIn.Models
{
    public enum DataType
    {
        Cumulative,
        TimeInterval
    }

    public class InvalidDataRange : Exception
    {

    }

    public class WrongColumn : Exception
    {

    }

    public class DataModel
    {
        private bool readData;
        private string datarange;
        private SRMData data = null;
        private DataType type = DataType.TimeInterval;

        public bool DataReaded
        {
            get
            {
                return readData;
            }

            set
            {
                readData = value;
            }
        }

        public SRMData SRMData
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
                readData = true;
            }
        }

        public ObservableCollection<PlotPoint> DataPlot
        {
            get
            {
                if (data != null)
                {
                    ObservableCollection<PlotPoint> list = new ObservableCollection<PlotPoint>();
                    double c = 0;
                    double t = 0;
                    for (int i=0; i<data.Size; i++)
                    {
                        t += data.Time[i];
                        if (data.Fault[i] + data.Type[i] != 0)
                        {
                            c += data.Fault[i] + data.Type[i];
                            list.Add(new PlotPoint(t, c));
                        }
                    }
                    return list;
                } else
                {
                    return new ObservableCollection<PlotPoint>();
                }
            }
        }

        public string DataRange
        {
            get
            {
                return datarange;
            }

            set
            {
                datarange = value;
                readData = false;
            }
        }

        public bool Cumulative
        {
            get
            {
                return type == DataType.Cumulative;
            }

            set
            {
                if (value == true)
                {
                    type = DataType.Cumulative;
                } else
                {
                    type = DataType.TimeInterval;
                }
                readData = false;
            }
        }

        public bool TimeInterval
        {
            get
            {
                return type == DataType.TimeInterval;
            }

            set
            {
                if (value == true)
                {
                    type = DataType.TimeInterval;
                }
                else
                {
                    type = DataType.Cumulative;
                }
                readData = false;
            }
        }

        public double Total
        {
            get
            {
                if (readData)
                {
                    return data.TotalFaults;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Mean
        {
            get
            {
                if (readData)
                {
                    return data.MeanTime;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Min
        {
            get
            {
                if (readData)
                {
                    return data.MinTime;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Max
        {
            get
            {
                if (readData)
                {
                    return data.MaxTime;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public void SetData()
        {
            SRMData = CreateSRMFData(datarange, Cumulative);
        }

        private SRMData CreateSRMFData(string datarange, bool cumulative)
        {
            try
            {
                Object[,] dataCells = IOOperation.GetInstance().Read(datarange);
                if (dataCells == null)
                {
                    throw new InvalidDataRange();
                }
                switch (dataCells.GetLength(1))
                {
                    case 1:
                        return SetDataOneColumn(dataCells, cumulative);
                    case 2:
                        return SetDataTwoColumn(dataCells, cumulative);
                    case 3:
                        return SetDataThreeColumn(dataCells, cumulative);
                    default:
                        throw new WrongColumn();
                }
            }
            catch
            {
                throw new InvalidDataRange();
            }
        }

        private SRMData SetDataOneColumn(Object[,] dataCells, bool cumulative)
        {
            int dsize = dataCells.GetUpperBound(0) - dataCells.GetLowerBound(0) + 1;
            double[] time = new double[dsize];
            int[] fault = new int[dsize];
            int[] type = new int[dsize];
            double prev = 0;
            for (int i = 0, j = dataCells.GetLowerBound(0); i < dsize; i++, j++)
            {
                double tmp = System.Convert.ToDouble(dataCells[j, 1]);
                if (cumulative)
                {
                    time[i] =  tmp - prev;
                    prev = tmp;
                }
                else
                {
                    time[i] = tmp;
                }
                fault[i] = 0;
                type[i] = 1;
            }
            SRMData fdat = new SRMData();
            fdat.SetData(time, fault, type);
            return fdat;
        }

        private SRMData SetDataTwoColumn(Object[,] dataCells, bool cumulative)
        {
            int dsize = dataCells.GetUpperBound(0) - dataCells.GetLowerBound(0) + 1;
            double[] time = new double[dsize];
            int[] fault = new int[dsize];
            int[] type = new int[dsize];
            double prev = 0;
            for (int i = 0, j = dataCells.GetLowerBound(0); i < dsize; i++, j++)
            {
                double tmp = System.Convert.ToDouble(dataCells[j, 1]);
                if (cumulative)
                {
                    time[i] = tmp - prev;
                    prev = tmp;
                }
                else
                {
                    time[i] = tmp;
                }
                fault[i] = System.Convert.ToInt32(dataCells[j, 2]);
                type[i] = 0;
            }
            SRMData fdat = new SRMData();
            fdat.SetData(time, fault, type);
            return fdat;
        }

        private SRMData SetDataThreeColumn(Object[,] dataCells, bool cumulative)
        {
            int dsize = dataCells.GetUpperBound(0) - dataCells.GetLowerBound(0) + 1;
            double[] time = new double[dsize];
            int[] fault = new int[dsize];
            int[] type = new int[dsize];
            double prev = 0;
            for (int i = 0, j = dataCells.GetLowerBound(0); i < dsize; i++, j++)
            {
                double tmp = System.Convert.ToDouble(dataCells[j, 1]);
                if (cumulative)
                {
                    time[i] = tmp - prev;
                    prev = tmp;
                }
                else
                {
                    time[i] = tmp;
                }
                fault[i] = System.Convert.ToInt32(dataCells[j, 2]);
                type[i] = System.Convert.ToInt32(dataCells[j, 3]);
            }
            SRMData fdat = new SRMData();
            fdat.SetData(time, fault, type);
            return fdat;
        }
    }
}
