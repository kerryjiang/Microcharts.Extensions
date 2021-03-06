﻿using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microcharts;
using SkiaSharp;
using Newtonsoft.Json.Linq;

namespace Microcharts.Matplotlib
{
    public static class MicrochartsBuilder
    {
        public static Chart CreateBarChart(IList<string> labels, IList<int> values)
        {
            var chartBuilder = new ChartBuilder<BarChart>();
            var chart = chartBuilder
                .Label(labels)
                .Value(values)
                .Build();
            
            return chart;
        }

        public static JObject Show(this Chart chart, int width = 500, int height = 300)
        {
            var imageData = new StringBuilder();
            
            using (var bitmap = new SKBitmap(width, height))
            using (var canvas = new SKCanvas(bitmap))
            {
                chart.Draw(canvas, width, height);
                var pngImage = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);

                using (var ms = new MemoryStream())
                {
                    pngImage.SaveTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    var buffer = new byte[96];

                    while (true)
                    {
                        var read = ms.Read(buffer, 0, buffer.Length);

                        if (read > 0)
                        {
                            imageData.Append(Convert.ToBase64String(buffer, 0, read));
                        }

                        if (read < buffer.Length)
                            break;
                    }
                }
            }

            return new JObject {
                {
                    "metadata", new JObject {
                       {
                           "image/png", new JObject {
                                { "width", width },
                                { "height", height }
                            }
                       }
                    }
                },
                {
                    "data", new JObject {
                        { "image/png", imageData.ToString() }
                    }
                }
            };
        }
    }
}
