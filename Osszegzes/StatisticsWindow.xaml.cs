using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Osszegzes
{
    public partial class StatisticsWindow : Window
    {
        private DataView dataView;
        private readonly string connectionString = "datasource=127.0.0.1;port=3306;database=teszt_adatok;username=root;password=;charset=utf8;";

        // Chart colors
        private readonly List<SolidColorBrush> chartColors = new List<SolidColorBrush>
        {
            new SolidColorBrush(Colors.SteelBlue),
            new SolidColorBrush(Colors.ForestGreen),
            new SolidColorBrush(Colors.Crimson),
            new SolidColorBrush(Colors.DarkOrange),
            new SolidColorBrush(Colors.Purple),
            new SolidColorBrush(Colors.Teal),
            new SolidColorBrush(Colors.DarkGoldenrod)
        };

        // Class to hold stat data
        public class StatItem
        {
            public string Category { get; set; }
            public int Count { get; set; }
            public double Percentage { get; set; }
            public string PercentageText => $"{Percentage:F1}%";
        }

        // Store reference to parent window for navigation
        private Window parentWindow;

        public StatisticsWindow(DataView currentData, Window parent = null)
        {
            InitializeComponent();
            this.dataView = currentData;
            this.parentWindow = parent;

            // Set window title
            this.Title = "Teszt Összegzés Statisztikák";

            // Load data after window is loaded
            this.Loaded += StatisticsWindow_Loaded;
        }

        private void StatisticsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                // Debug available columns
                string availableColumns = "Available columns: ";
                foreach (DataColumn column in dataView.Table.Columns)
                {
                    availableColumns += column.ColumnName + ", ";
                }
                Console.WriteLine(availableColumns);

                // Convert DataView to a workable collection
                var rows = dataView.ToTable().AsEnumerable();

                if (!rows.Any())
                {
                    MessageBox.Show("Nincs megjeleníthető adat.", "Információ", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    return;
                }

                // Set window title with filter info
                string teacherFilter = rows.First().Field<string>("tanar");
                string classFilter = rows.First().Field<string>("osztaly");
                bool isFiltered = rows.Count() < dataView.Table.Rows.Count;

                if (isFiltered)
                {
                    if (rows.All(r => r.Field<string>("tanar") == teacherFilter) &&
                        rows.All(r => r.Field<string>("osztaly") == classFilter))
                    {
                        this.Title = $"Statisztikák - {teacherFilter} - {classFilter}";
                    }
                    else if (rows.All(r => r.Field<string>("tanar") == teacherFilter))
                    {
                        this.Title = $"Statisztikák - {teacherFilter}";
                    }
                    else if (rows.All(r => r.Field<string>("osztaly") == classFilter))
                    {
                        this.Title = $"Statisztikák - {classFilter} osztály";
                    }
                }

                // Generate summary statistics
                txtTotalCount.Text = rows.Count().ToString();

                // Calculate average difficulty scores
                double avgOverall = 0;
                int validColumns = 0;

                if (dataView.Table.Columns.Contains("tablaiMunka"))
                {
                    avgOverall += rows.Average(r => Convert.ToDouble(r.Field<int>("tablaiMunka")));
                    validColumns++;
                }

                if (dataView.Table.Columns.Contains("vetitettDiasor"))
                {
                    avgOverall += rows.Average(r => Convert.ToDouble(r.Field<int>("vetitettDiasor")));
                    validColumns++;
                }

                if (dataView.Table.Columns.Contains("oktatofilmek"))
                {
                    avgOverall += rows.Average(r => Convert.ToDouble(r.Field<int>("oktatofilmek")));
                    validColumns++;
                }

                if (dataView.Table.Columns.Contains("egyeniGyak"))
                {
                    avgOverall += rows.Average(r => Convert.ToDouble(r.Field<int>("egyeniGyak")));
                    validColumns++;
                }

                if (dataView.Table.Columns.Contains("haziFeladatok"))
                {
                    avgOverall += rows.Average(r => Convert.ToDouble(r.Field<int>("haziFeladatok")));
                    validColumns++;
                }

                if (validColumns > 0)
                {
                    avgOverall /= validColumns;
                    txtAvgDifficulty.Text = avgOverall.ToString("F1") + " / 6";
                }
                else
                {
                    txtAvgDifficulty.Text = "N/A";
                }

                // Create statistics for each category (only if columns exist)
                var megerthetosegStats = dataView.Table.Columns.Contains("megerthetoseg") ?
                    GetCategoryStats(rows, "megerthetoseg") : new List<StatItem>();

                var erthetosegStats = dataView.Table.Columns.Contains("erthetoseg") ?
                    GetCategoryStats(rows, "erthetoseg") : new List<StatItem>();

                var idoStats = dataView.Table.Columns.Contains("ido") ?
                    GetCategoryStats(rows, "ido") : new List<StatItem>();

                var elmenyStats = dataView.Table.Columns.Contains("elmeny") ?
                    GetCategoryStats(rows, "elmeny") : new List<StatItem>();

                // Create difficulty statistics for each type (only if columns exist)
                var tablaiMunkaStats = dataView.Table.Columns.Contains("tablaiMunka") ?
                    GetNumericStats(rows, "tablaiMunka") : new List<StatItem>();

                var diasorStats = dataView.Table.Columns.Contains("vetitettDiasor") ?
                    GetNumericStats(rows, "vetitettDiasor") : new List<StatItem>();

                var filmekStats = dataView.Table.Columns.Contains("oktatofilmek") ?
                    GetNumericStats(rows, "oktatofilmek") : new List<StatItem>();

                var gyakFeladatokStats = dataView.Table.Columns.Contains("egyeniGyak") ?
                    GetNumericStats(rows, "egyeniGyak") : new List<StatItem>();

                var haziFeladatokStats = dataView.Table.Columns.Contains("haziFeladatok") ?
                    GetNumericStats(rows, "haziFeladatok") : new List<StatItem>();

                // Set top experience
                if (elmenyStats.Any())
                {
                    txtTopExperience.Text = elmenyStats.OrderByDescending(s => s.Count).First().Category;
                }
                else
                {
                    txtTopExperience.Text = "-";
                }

                // Populate data grids
                dgMegerthetoseg.ItemsSource = megerthetosegStats;
                dgErthetosegGrid.ItemsSource = erthetosegStats;
                dgTablaiMunka.ItemsSource = tablaiMunkaStats;
                dgDiasor.ItemsSource = diasorStats;
                dgFilmek.ItemsSource = filmekStats;
                dgGyakFeladatok.ItemsSource = gyakFeladatokStats;
                dgHaziFeladatok.ItemsSource = haziFeladatokStats;
                dgIdo.ItemsSource = idoStats;
                dgElmeny.ItemsSource = elmenyStats;

                // Create charts (only if data exists)
                if (megerthetosegStats.Any())
                {
                    CreatePieChart(megerthetosegChart, megerthetosegStats, "Nehézségek");
                    
                }

                if (erthetosegStats.Any())
                {
                    CreatePieChart(erthetosegChart, erthetosegStats, "Magyarázat érthetősége");
                }


                // Create difficulty charts (only if data exists)
                if (tablaiMunkaStats.Any())
                {
                    CreateBarChart(tablaiMunkaChart, tablaiMunkaStats, "Táblai Munka");
                   
                }

                if (diasorStats.Any())
                {
                    CreateBarChart(diasorChart, diasorStats, "Vetített Diasor");
                }

                if (filmekStats.Any())
                {
                    CreateBarChart(filmekChart, filmekStats, "Oktatófilmek");
                }

                if (gyakFeladatokStats.Any())
                {
                    CreateBarChart(gyakFeladatokChart, gyakFeladatokStats, "Egyéni Gyakorlófeladatok");
                }

                if (haziFeladatokStats.Any())
                {
                    CreateBarChart(haziFeladatokChart, haziFeladatokStats, "Házi Feladatok");
                }

                if (idoStats.Any())
                {
                    CreateBarChart(idoChart, idoStats, "Időráfordítás");
                }

                if (elmenyStats.Any())
                {
                    CreatePieChart(elmenyChart, elmenyStats, "Élmény");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba történt a statisztikák betöltésekor: {ex.Message}", "Hiba",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<StatItem> GetCategoryStats(IEnumerable<DataRow> rows, string columnName)
        {
            try
            {
                return rows.GroupBy(r => r.Field<string>(columnName))
                    .Select(g => new StatItem
                    {
                        Category = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / rows.Count() * 100, 1)
                    })
                    .OrderByDescending(x => x.Count)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<StatItem>();
            }
        }

        private List<StatItem> GetNumericStats(IEnumerable<DataRow> rows, string columnName)
        {
            try
            {
                return rows.GroupBy(r => r.Field<int>(columnName))
                    .Select(g => new StatItem
                    {
                        Category = g.Key.ToString(),
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / rows.Count() * 100, 1)
                    })
                    .OrderBy(x => int.Parse(x.Category))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<StatItem>();
            }
        }

        private void CreatePieChart(Canvas canvas, List<StatItem> data, string title)
        {
            canvas.Children.Clear();

            if (data == null || !data.Any())
                return;

            // Get canvas dimensions
            double width = canvas.ActualWidth > 0 ? canvas.ActualWidth : 400;
            double height = canvas.ActualHeight > 0 ? canvas.ActualHeight : 300;

            // Add title
            TextBlock titleText = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Canvas.SetLeft(titleText, 10);
            Canvas.SetTop(titleText, 10);
            canvas.Children.Add(titleText);

            // Calculate center and radius
            double centerX = width / 2;
            double centerY = height / 2;
            double radius = Math.Min(width, height) * 0.4;

            // Draw pie slices
            double totalValue = data.Sum(d => d.Count);
            double startAngle = 0;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                double sliceAngle = (item.Count / totalValue) * 360;
                double endAngle = startAngle + sliceAngle;

                // Convert angles to radians for calculations
                double startRad = startAngle * Math.PI / 180;
                double endRad = endAngle * Math.PI / 180;

                // Calculate points for the pie slice
                Point startPoint = new Point(centerX, centerY);
                Point arcStart = new Point(
                    centerX + radius * Math.Sin(startRad),
                    centerY - radius * Math.Cos(startRad));
                Point arcEnd = new Point(
                    centerX + radius * Math.Sin(endRad),
                    centerY - radius * Math.Cos(endRad));

                // Create path for pie slice
                Path path = new Path();
                PathGeometry pathGeom = new PathGeometry();
                PathFigure pathFig = new PathFigure();

                pathFig.StartPoint = startPoint;
                pathFig.Segments.Add(new LineSegment(arcStart, true));

                ArcSegment arcSegment = new ArcSegment
                {
                    Point = arcEnd,
                    Size = new Size(radius, radius),
                    SweepDirection = SweepDirection.Clockwise,
                    IsLargeArc = sliceAngle > 180
                };

                pathFig.Segments.Add(arcSegment);
                pathFig.Segments.Add(new LineSegment(startPoint, true));
                pathFig.IsClosed = true;

                pathGeom.Figures.Add(pathFig);
                path.Data = pathGeom;

                // Set color using the color array
                path.Fill = chartColors[i % chartColors.Count];
                path.Stroke = Brushes.White;
                path.StrokeThickness = 1;

                // Add tooltip
                ToolTip tooltip = new ToolTip
                {
                    Content = $"{item.Category}: {item.Count} ({item.Percentage:F1}%)"
                };
                path.ToolTip = tooltip;

                canvas.Children.Add(path);

                // Add legend item
                if (width > 200)  // Only add legend if there's enough space
                {
                    double legendX = width * 0.75;
                    double legendY = height * 0.2 + (i * 25);

                    // Legend color box
                    Rectangle colorBox = new Rectangle
                    {
                        Width = 15,
                        Height = 15,
                        Fill = chartColors[i % chartColors.Count]
                    };
                    Canvas.SetLeft(colorBox, legendX);
                    Canvas.SetTop(colorBox, legendY);
                    canvas.Children.Add(colorBox);

                    // Legend text
                    TextBlock legendText = new TextBlock
                    {
                        Text = $"{item.Category} ({item.Percentage:F1}%)",
                        FontSize = 12
                    };
                    Canvas.SetLeft(legendText, legendX + 20);
                    Canvas.SetTop(legendText, legendY);
                    canvas.Children.Add(legendText);
                }

                startAngle = endAngle;
            }
        }

        private void CreateBarChart(Canvas canvas, List<StatItem> data, string title)
        {
            canvas.Children.Clear();

            if (data == null || !data.Any())
                return;

            // Get canvas dimensions
            double width = canvas.ActualWidth > 0 ? canvas.ActualWidth : 400;
            double height = canvas.ActualHeight > 0 ? canvas.ActualHeight : 300;

            // Chart margins
            double marginLeft = 70;
            double marginRight = 20;
            double marginTop = 50;
            double marginBottom = 50;

            // Chart area dimensions
            double chartWidth = width - marginLeft - marginRight;
            double chartHeight = height - marginTop - marginBottom;

            // Add title
            TextBlock titleText = new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Canvas.SetLeft(titleText, (width - marginLeft - marginRight) / 2);
            Canvas.SetTop(titleText, 10);
            canvas.Children.Add(titleText);

            // Draw axes
            Line xAxis = new Line
            {
                X1 = marginLeft,
                Y1 = height - marginBottom,
                X2 = width - marginRight,
                Y2 = height - marginBottom,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas.Children.Add(xAxis);

            Line yAxis = new Line
            {
                X1 = marginLeft,
                Y1 = marginTop,
                X2 = marginLeft,
                Y2 = height - marginBottom,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            canvas.Children.Add(yAxis);

            // Calculate bar width and spacing
            int dataCount = data.Count;
            double barWidth = chartWidth / (dataCount * 2); // Use half of available space for bars
            double maxValue = data.Max(d => d.Count);

            // Draw y-axis labels (count)
            for (int i = 0; i <= 5; i++)
            {
                double labelValue = maxValue * i / 5;
                double yPos = height - marginBottom - (chartHeight * i / 5);

                Line tickMark = new Line
                {
                    X1 = marginLeft - 5,
                    Y1 = yPos,
                    X2 = marginLeft,
                    Y2 = yPos,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                canvas.Children.Add(tickMark);

                TextBlock label = new TextBlock
                {
                    Text = Math.Round(labelValue, 0).ToString(),
                    FontSize = 12,
                    TextAlignment = TextAlignment.Right,
                    Width = 50
                };
                Canvas.SetLeft(label, marginLeft - 55);
                Canvas.SetTop(label, yPos - 10);
                canvas.Children.Add(label);
            }

            // Draw bars and x-axis labels
            for (int i = 0; i < dataCount; i++)
            {
                var item = data[i];
                double barHeight = (item.Count / maxValue) * chartHeight;
                double xPos = marginLeft + (i * chartWidth / dataCount) + (chartWidth / dataCount / 4);
                double yPos = height - marginBottom - barHeight;

                // Draw bar
                Rectangle bar = new Rectangle
                {
                    Width = barWidth,
                    Height = barHeight,
                    Fill = chartColors[i % chartColors.Count],
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(bar, xPos);
                Canvas.SetTop(bar, yPos);

                // Add tooltip
                ToolTip tooltip = new ToolTip
                {
                    Content = $"{item.Category}: {item.Count} ({item.Percentage:F1}%)"
                };
                bar.ToolTip = tooltip;
                canvas.Children.Add(bar);

                // Add count label on top of bar
                TextBlock countLabel = new TextBlock
                {
                    Text = item.Count.ToString(),
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth
                };
                Canvas.SetLeft(countLabel, xPos);
                Canvas.SetTop(countLabel, yPos - 20);
                canvas.Children.Add(countLabel);

                // Add x-axis label
                TextBlock xLabel = new TextBlock
                {
                    Text = item.Category,
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center,
                    Width = barWidth * 2
                };
                Canvas.SetLeft(xLabel, xPos - barWidth / 2);
                Canvas.SetTop(xLabel, height - marginBottom + 5);
                canvas.Children.Add(xLabel);
            }
        }
    }
}